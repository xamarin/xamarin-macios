using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	//
	// This task is responsible for computing everything we need to know for code
	// signing.
	//
	// For each app bundle to be signed, a few more items to be code signed:
	//
	// * All *.dylib and *.metallib files
	// * All *.framework directories
	//
	// In both cases we iterate over what we find in the app bundle instead of
	// relying on what the msbuild tasks built and copied in the app bundle,
	// because customer logic might have added additional frameworks, and those
	// need to be signed too.
	//
	// This task will also figure out a stamp file path we use to determine if
	// something needs (re-)signing.
	//
	public abstract class ComputeCodesignItemsTaskBase : XamarinTask {

		[Required]
		public string AppBundleDir { get; set; } = string.Empty;

		[Required]
		public ITaskItem [] CodesignBundle { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public ITaskItem [] CodesignItems { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string CodesignStampPath { get; set; } = string.Empty;

		public ITaskItem [] GenerateDSymItems { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] NativeStripItems { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] OutputCodesignItems { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] SkipCodesignItems { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			var output = new List<ITaskItem> ();

			// Make sure AppBundleDir has a trailing slash
			var appBundlePath = PathUtils.EnsureTrailingSlash (Path.GetFullPath (AppBundleDir));

			// Add the app bundles themselves
			foreach (var bundle in CodesignBundle) {
				// An app bundle is signed if either 'RequireCodeSigning' is true
				// or a 'CodesignSigningKey' has been provided.
				var requireCodeSigning = bundle.GetMetadata ("RequireCodeSigning");
				var codesignSigningKey = bundle.GetMetadata ("CodesignSigningKey");
				if (!string.Equals (requireCodeSigning, "true") && string.IsNullOrEmpty (codesignSigningKey))
					continue;

				// Create a new item for the app bundle, and copy any metadata over.
				var bundlePath = Path.Combine (Path.GetDirectoryName (AppBundleDir), bundle.ItemSpec);
				var item = new TaskItem (bundlePath);
				bundle.CopyMetadataTo (item);

				// Compute the stamp file to use
				item.SetMetadataIfNotSet ("CodesignStampFile", Path.Combine (CodesignStampPath, Path.GetFileName (bundle.ItemSpec), ".stampfile"));

				// Get any additional stamp files we must touch when the item is signed.
				var additionalStampFiles = new List<string> ();
				// We must touch the dSYM directory's Info.plist, to ensure that we don't end up running dsymutil again after codesigning in the next build
				var generateDSymItem = GenerateDSymItems.FirstOrDefault (v => {
					return string.Equals (Path.Combine (Path.GetDirectoryName (AppBundleDir), Path.GetDirectoryName (v.ItemSpec)), item.ItemSpec, StringComparison.OrdinalIgnoreCase);
				});
				if (generateDSymItem is not null)
					additionalStampFiles.Add (generateDSymItem.GetMetadata ("dSYMUtilStampFile"));
				// We must touch the stamp file for native stripping, to ensure that we don't want to run strip again after codesigning in the next build
				var nativeStripItem = NativeStripItems.FirstOrDefault (v => string.Equals (Path.Combine (Path.GetDirectoryName (AppBundleDir), Path.GetDirectoryName (v.ItemSpec)), item.ItemSpec, StringComparison.OrdinalIgnoreCase));
				if (nativeStripItem is not null)
					additionalStampFiles.Add (nativeStripItem.GetMetadata ("StripStampFile"));
				// Set the CodesignAdditionalFilesToTouch metadata
				if (additionalStampFiles.Count > 0) {
					additionalStampFiles.AddRange (item.GetMetadata ("CodesignAdditionalFilesToTouch").Split (';'));
					additionalStampFiles.RemoveAll (v => string.IsNullOrEmpty (v));
					item.SetMetadata ("CodesignAdditionalFilesToTouch", string.Join (";", additionalStampFiles));
				}

				output.Add (item);
			}

			// Find all:
			//	- *.dylib and *.metallib files
			//	- *.framework directories
			foreach (var bundle in CodesignBundle) {
				var bundlePath = Path.Combine (Path.GetDirectoryName (Path.GetDirectoryName (appBundlePath)), bundle.ItemSpec);
				var filesToSign = FindFilesToSign (bundlePath);
				foreach (var lib in filesToSign) {
					var relativeLib = Path.Combine (AppBundleDir, lib.Substring (appBundlePath.Length));
					var item = new TaskItem (relativeLib);
					bundle.CopyMetadataTo (item);

					// These items must not use the entitlements for the app
					item.RemoveMetadata ("CodesignEntitlements");

					// These files are a bit special, because they're always signed. This is done
					// by setting the signing key to '-' if it's not set.
					item.SetMetadataIfNotSet ("CodesignSigningKey", "-");

					// Set the stamp file even if already set (because any existing values would be copied from
					// the bundle, which would be the wrong stamp file, so it must be overridden)
					if (Directory.Exists (relativeLib)) {
						item.SetMetadata ("CodesignStampFile", Path.Combine (CodesignStampPath, relativeLib, ".stampfile"));
					} else {
						item.SetMetadata ("CodesignStampFile", Path.Combine (CodesignStampPath, relativeLib));
					}

					output.Add (item);
				}
			}

			// Add all additional items
			foreach (var item in CodesignItems) {
				// Set the stamp file if not already set.
				item.SetMetadataIfNotSet ("CodesignStampFile", Path.Combine (CodesignStampPath, item.ItemSpec));

				output.Add (item);
			}

			// Remove any items we were asked to remove
			RemoveFilesToNotSign (output, appBundlePath);

			// We may be asked to sign the same item multiple times (in particular for universal .NET builds)
			// Here we de-duplicate (based on itemspec). We also verify that the metadata is the same between
			// all deduplicated items, and if not, we show a warning.
			var grouped = output.GroupBy (v => v.ItemSpec);
			foreach (var group in grouped) {
				if (group.Count () < 2)
					continue;

				var all = group.ToArray ();
				var firstMetadata = all [0].CloneCustomMetadataToDictionary ();
				for (var i = 1; i < all.Length; i++) {
					var nextMetadata = all [i].CloneCustomMetadataToDictionary ();
					if (nextMetadata.Count != firstMetadata.Count) {
						Log.LogWarning (MSBStrings.W7095, /* Code signing has been requested multiple times for '{0}', with different metadata. The metadata for one are: '{1}', while the metadata for the other are: '{2}' */ group.Key, string.Join (", ", firstMetadata.Keys), string.Join (", ", nextMetadata.Keys));
					} else {
						foreach (var kvp in firstMetadata) {
							if (!nextMetadata.TryGetValue (kvp.Key, out var nextValue)) {
								Log.LogWarning (MSBStrings.W7096, /* Code signing has been requested multiple times for '{0}', with different metadata. The metadata '{1}={2}' has been set for one item, but not the other. */ group.Key, kvp.Key, kvp.Value);
								continue;
							}
							if (nextValue != kvp.Value) {
								Log.LogWarning (MSBStrings.W7097, /* Code signing has been requested multiple times for '{0}', with different metadata. The metadata '{1}' has different values for each item (once it's '{2}', another time it's '{3}'). */ group.Key, kvp.Key, kvp.Value, nextValue);
								continue;
							}
						}
					}
					output.Remove (all [i]);
				}
			}

			OutputCodesignItems = output.ToArray ();

			return !Log.HasLoggedErrors;
		}

		string CodeSignatureRelativePath {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return string.Empty;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return "Contents";
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		void RemoveFilesToNotSign (List<ITaskItem> output, string appBundlePath)
		{
			if (SkipCodesignItems.Length == 0)
				return;

			// Canonicalize the paths and split into files and directories
			var canonicalizedItemsToSkip = SkipCodesignItems
				.Select (v => Path.Combine (Path.GetDirectoryName (appBundlePath), v.ItemSpec))
				.ToArray ();
			var canonicalizedDirectoriesToSkip = canonicalizedItemsToSkip
				.Where (v => Directory.Exists (v))
				.Select (v => PathUtils.ResolveSymbolicLinks (v))
				.Select (v => PathUtils.EnsureTrailingSlash (v))
				.ToArray ();
			var canonicalizedFilesToSkip = canonicalizedItemsToSkip
				.Where (File.Exists)
				.Select (PathUtils.ResolveSymbolicLinks)
				.ToArray ();

			for (var i = output.Count - 1; i >= 0; i--) {
				var item = output [i];
				var outputPath = Path.GetFullPath (item.ItemSpec); // item.ItemSpec is relative to the project directory
				outputPath = PathUtils.ResolveSymbolicLinks (outputPath); // Canonicalize

				if (canonicalizedFilesToSkip.Contains (outputPath)) {
					Log.LogMessage (MessageImportance.Low, $"Not signing '{output [i].ItemSpec}' because it's in the list of files that skips signing");
					output.RemoveAt (i);
					continue;
				}

				if (!Directory.Exists (outputPath))
					continue;

				var matchingDirectory = canonicalizedDirectoriesToSkip.FirstOrDefault (v => outputPath.StartsWith (v, StringComparison.OrdinalIgnoreCase));
				if (matchingDirectory is not null) {
					Log.LogMessage (MessageImportance.Low, $"Not signing '{output [i].ItemSpec}' because it's in the list of directories that skips signing ({matchingDirectory})");
					output.RemoveAt (i);
					continue;
				}
			}
		}

		IEnumerable<string> FindFilesToSign (string appPath)
		{
			var rv = new List<string> ();

			// Canonicalize the app path, so string comparisons work later on
			appPath = PathUtils.ResolveSymbolicLinks (Path.GetFullPath (appPath));

			// Make sure path ends with trailing slash to ease logic
			appPath = PathUtils.EnsureTrailingSlash (appPath);

			string dylibDirectory;
			string metallibDirectory;
			string frameworksDirectory;
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				dylibDirectory = appPath;
				metallibDirectory = appPath;
				frameworksDirectory = Path.Combine (appPath, "Frameworks");
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				dylibDirectory = Path.Combine (appPath, "Contents");
				metallibDirectory = Path.Combine (appPath, "Contents", "Resources");
				frameworksDirectory = Path.Combine (appPath, "Contents", "Frameworks");
				break;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}

			dylibDirectory = PathUtils.EnsureTrailingSlash (dylibDirectory);
			metallibDirectory = PathUtils.EnsureTrailingSlash (metallibDirectory);

			foreach (var entry in Directory.EnumerateFileSystemEntries (appPath, "*", SearchOption.AllDirectories)) {
				var relativePath = entry.Substring (appPath.Length);
				// Don't recurse into the PlugIns directory, that's already handled for any app bundle inside the PlugIns directory
				if (relativePath.StartsWith ("PlugIns" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
					continue;
				// Don't recurse into the Watch directory, for the same reason
				if (relativePath.StartsWith ("Watch" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
					continue;
				// Don't sign symlinks, just the real file (this avoids trying to sign the same file multiple times through a symlink)
				if (PathUtils.IsSymlink (entry))
					continue;

				if (entry.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase) && entry.StartsWith (dylibDirectory, StringComparison.OrdinalIgnoreCase)) {
					// We find *.dylibs in any subdirectory
					rv.Add (entry);
				} else if (entry.EndsWith (".metallib", StringComparison.OrdinalIgnoreCase) && entry.StartsWith (metallibDirectory, StringComparison.OrdinalIgnoreCase)) {
					// We find *.metallib in any subdirectory
					rv.Add (entry);
				} else if (entry.EndsWith (".framework", StringComparison.OrdinalIgnoreCase) && string.Equals (Path.GetDirectoryName (entry), frameworksDirectory, StringComparison.OrdinalIgnoreCase)) {
					// We only find *.frameworks inside the Frameworks subdirectory, not recursively
					// (not quite sure if this is the right thing to do, but it's what we've been doing so far).
					rv.Add (entry);
				}
			}

			return rv;
		}
	}
}
