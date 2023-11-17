using System;
using System.IO;
using System.Linq;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CodesignTaskBase : XamarinTask {
		const string ToolName = "codesign";
		const string MacOSDirName = "MacOS";
		const string CodeSignatureDirName = "_CodeSignature";
		string toolExe;

		#region Inputs

		// Can also be specified per resource using the 'CodesignStampFile' metadata
		public string StampFile { get; set; }

		// Can also be specified per resource using the 'CodesignAllocate' metadata
		public string CodesignAllocate { get; set; }

		// Can also be specified per resource using the 'CodesignDisableTimestamp' metadata
		public bool DisableTimestamp { get; set; }

		// Can also be specified per resource using the 'CodesignEntitlements' metadata
		public string Entitlements { get; set; }

		// Can also be specified per resource using the 'CodesignKeychain' metadata
		public string Keychain { get; set; }

		[Required]
		public ITaskItem [] Resources { get; set; }

		// Can also be specified per resource using the 'CodesignResourceRules' metadata
		public string ResourceRules { get; set; }

		// Can also be specified per resource using the 'CodesignSigningKey' metadata
		public string SigningKey { get; set; }

		// Can also be specified per resource using the 'CodesignExtraArgs' metadata
		public string ExtraArgs { get; set; }

		// Can also be specified per resource using the 'CodesignDeep' metadata (yes, the naming difference is correct and due to historical reasons)
		public bool IsAppExtension { get; set; }

		// Can also be specified per resource using the 'CodesignUseHardenedRuntime' metadata
		public bool UseHardenedRuntime { get; set; }

		// Can also be specified per resource using the 'CodesignUseSecureTimestamp' metadata
		public bool UseSecureTimestamp { get; set; }

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

		#endregion

		#region Outputs

		// This output value is not observed anywhere in our targets, but it's required for building on Windows
		// to make sure any codesigned files other tasks depend on are copied back to the windows machine.
		[Output]
		public ITaskItem [] CodesignedFiles { get; set; }

		#endregion

		string GetFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		string GetCodesignStampFile (ITaskItem item)
		{
			var rv = GetNonEmptyStringOrFallback (item, "CodesignStampFile", StampFile, "StampFile", required: true);
			rv = PathUtils.ConvertToMacPath (rv);
			return rv;
		}

		string GetCodesignAllocate (ITaskItem item)
		{
			return GetNonEmptyStringOrFallback (item, "CodesignAllocate", CodesignAllocate, "CodesignAllocate", required: true);
		}

		// 'sortedItems' is sorted by length of path, longest first.
		bool NeedsCodesign (ITaskItem [] sortedItems, int index, string stampFileContents)
		{
			var item = sortedItems [index];
			var stampFile = GetCodesignStampFile (item);
			if (!File.Exists (stampFile)) {
				Log.LogMessage (MessageImportance.Low, "The stamp file '{0}' does not exist, so the item '{1}' needs to be codesigned.", stampFile, item.ItemSpec);
				return true;
			}

			if (File.GetLastWriteTimeUtc (item.ItemSpec) >= File.GetLastWriteTimeUtc (stampFile)) {
				Log.LogMessage (MessageImportance.Low, "The stamp file '{0}' for the item '{1}' is not up-to-date, so the item needs to be codesigned.", stampFile, item.ItemSpec);
				return true;
			}

			if (Directory.Exists (item.ItemSpec)) {
				// We're signing a directory. First check if any of the
				// previous items in the sorted item array must be signed, and
				// if that item is inside this directory, we'll have to sign
				// this directory too.
				var itemPath = EnsureEndsWithDirectorySeparator (item.ItemSpec);
				var resolvedStampFile = Path.GetFullPath (PathUtils.ResolveSymbolicLinks (stampFile));

				for (var i = 0; i < index; i++) {
					if (sortedItems [i] is null)
						continue; // this item does not need to be signed
					if (sortedItems [i].ItemSpec.StartsWith (itemPath, StringComparison.OrdinalIgnoreCase)) {
						Log.LogMessage (MessageImportance.Low, "The item '{0}' contains '{1}', which must be signed, which means that the item must be signed too.", item.ItemSpec, sortedItems [i].ItemSpec);
						return true; // there's an item inside this directory that needs to be signed, so this directory must be signed too
					}
				}

				// we also need to check every file inside this directory
				foreach (var file in Directory.EnumerateFiles (itemPath, "*", SearchOption.AllDirectories)) {
					if (string.Equals (resolvedStampFile, Path.GetFullPath (PathUtils.ResolveSymbolicLinks (file)), StringComparison.OrdinalIgnoreCase))
						continue; // we check every file except the stamp file, which may be inside the directory we want to sign (example: _CodeSignature/CodeResources is inside the app bundle, and also the stamp file).

					if (!IsUpToDate (file, stampFile)) {
						Log.LogMessage (MessageImportance.Low, "The item '{0}' contains '{1}', which is not up-to-date with regards to the stamp file '{2}', so the item must be codesigned.", item.ItemSpec, file, stampFile);
						return true;
					}
				}
			}

			if (File.ReadAllText (stampFile) != stampFileContents) {
				Log.LogMessage (MessageImportance.Low, "The expected content in the stamp file '{0}' for the item '{1}' is not up-to-date, so the item must be codesigned.", stampFile, item.ItemSpec);
				return true;
			}

			Log.LogMessage (MessageImportance.Low, "The stamp file '{0}' for the item '{1}' is up-to-date, so the item does not need to be codesigned.", stampFile, item.ItemSpec);
			return false;
		}

		bool ParseBoolean (ITaskItem item, string metadataName, bool fallbackValue)
		{
			var metadataValue = item.GetMetadata (metadataName);
			if (string.IsNullOrEmpty (metadataValue))
				return fallbackValue;
			return string.Equals (metadataValue, "true", StringComparison.OrdinalIgnoreCase);
		}

		string ResolvePath (ITaskItem item, string path)
		{
			if (string.IsNullOrEmpty (path))
				return path;

			path = PathUtils.ConvertToMacPath (path);
			if (Path.IsPathRooted (path))
				return path;

			var sourceProjectPath = GetNonEmptyStringOrFallback (item, "SourceProjectPath", null);
			if (sourceProjectPath is null)
				return path;

			return Path.Combine (sourceProjectPath, path);
		}

		string GetCodesignResourceRules (ITaskItem item)
		{
			var rv = GetNonEmptyStringOrFallback (item, "CodesignResourceRules", out var foundInMetadata, ResourceRules);
			// The ResourceRules value is a path, and as such it might be a relative path from a different project, in which case we have to resolve it accordingly.
			if (foundInMetadata)
				rv = ResolvePath (item, rv);
			return rv;
		}

		string GetCodesignEntitlements (ITaskItem item)
		{
			var rv = GetNonEmptyStringOrFallback (item, "CodesignEntitlements", out var foundInMetadata, ResourceRules);
			// The ResourceRules value is a path, and as such it might be a relative path from a different project, in which case we have to resolve it accordingly.
			if (foundInMetadata)
				rv = ResolvePath (item, rv);
			return rv;
		}

		IList<string> GenerateCommandLineArguments (ITaskItem item)
		{
			var args = new List<string> ();
			var isDeep = ParseBoolean (item, "CodesignDeep", IsAppExtension);
			var useHardenedRuntime = ParseBoolean (item, "CodesignUseHardenedRuntime", UseHardenedRuntime);
			var useSecureTimestamp = ParseBoolean (item, "CodesignUseSecureTimestamp", UseSecureTimestamp);
			var disableTimestamp = ParseBoolean (item, "CodesignDisableTimestamp", DisableTimestamp);
			var signingKey = GetNonEmptyStringOrFallback (item, "CodesignSigningKey", SigningKey, "SigningKey", required: true);
			var keychain = GetNonEmptyStringOrFallback (item, "CodesignKeychain", Keychain);
			var resourceRules = GetCodesignResourceRules (item);
			var entitlements = GetCodesignEntitlements (item);
			var extraArgs = GetNonEmptyStringOrFallback (item, "CodesignExtraArgs", ExtraArgs);

			args.Add ("-v");
			args.Add ("--force");

			if (isDeep)
				args.Add ("--deep");

			if (useHardenedRuntime) {
				args.Add ("-o");
				args.Add ("runtime");
			}

			if (useSecureTimestamp) {
				if (disableTimestamp) {
					// Conflicting '{0}' and '{1}' options. '{1}' will be ignored.
					Log.LogWarning (MSBStrings.W0176, "UseSecureTimestamp", "DisableTimestamp");
				}
				args.Add ("--timestamp");
			} else
				args.Add ("--timestamp=none");

			args.Add ("--sign");
			args.Add (signingKey);

			if (!string.IsNullOrEmpty (keychain)) {
				args.Add ("--keychain");
				args.Add (Path.GetFullPath (keychain));
			}

			if (!string.IsNullOrEmpty (resourceRules)) {
				resourceRules = PathUtils.ConvertToMacPath (resourceRules);
				args.Add ("--resource-rules");
				args.Add (Path.GetFullPath (resourceRules));
			}

			if (!string.IsNullOrEmpty (entitlements)) {
				entitlements = PathUtils.ConvertToMacPath (entitlements);
				args.Add ("--entitlements");
				args.Add (Path.GetFullPath (entitlements));
			}

			if (!string.IsNullOrEmpty (extraArgs))
				args.Add (extraArgs);

			// signing a framework and a file inside a framework is not *always* identical
			// on macOS apps {item.ItemSpec} can be a symlink to `Versions/Current/{item.ItemSpec}`
			// and `Current` also a symlink to `A`... and `_CodeSignature` will be found there
			var path = item.ItemSpec;
			var parent = Path.GetDirectoryName (path);

			// so do not don't sign `A.framework/A`, sign `A.framework` which will always sign the *bundle*
			if ((Path.GetExtension (parent) == ".framework") && (Path.GetFileName (path) == Path.GetFileNameWithoutExtension (parent)))
				path = parent;

			path = PathUtils.ResolveSymbolicLinks (path);
			args.Add (Path.GetFullPath (path));

			return args;
		}

		void Codesign (SignInfo info)
		{
			var item = info.Item;
			var fileName = GetFullPathToTool ();
			var arguments = info.GetCommandLineArguments (this);
			var environment = new Dictionary<string, string> () {
				{ "CODESIGN_ALLOCATE", GetCodesignAllocate (item) },
			};
			var rv = ExecuteAsync (fileName, arguments, null, environment, mergeOutput: false).Result;
			var exitCode = rv.ExitCode;
			var messages = rv.StandardOutput.ToString ();

			if (messages.Length > 0)
				Log.LogMessage (MessageImportance.Normal, "{0}", messages.ToString ());

			if (exitCode != 0) {
				var errors = rv.StandardError.ToString ();
				if (errors.Length > 0)
					Log.LogError (MSBStrings.E0004, item.ItemSpec, errors);
				else
					Log.LogError (MSBStrings.E0005, item.ItemSpec);
			} else {
				var stampFile = GetCodesignStampFile (item);
				if (string.IsNullOrEmpty (stampFile)) {
					Log.LogMessage (MessageImportance.Low, "No stamp file '{0}' available for the item '{1}'", stampFile, item.ItemSpec);
				} else if (IsUpToDate (item.ItemSpec, stampFile)) {
					Log.LogMessage (MessageImportance.Low, "The stamp file '{0}' is already up-to-date for the item '{1}', updating it anyway", stampFile, item.ItemSpec);
					File.WriteAllText (stampFile, info.GetStampFileContents (this));
				} else if (File.Exists (stampFile)) {
					Log.LogMessage (MessageImportance.Low, "The stamp file '{0}' is not up-to-date for the item '{1}', and it will be updated", stampFile, item.ItemSpec);
					File.WriteAllText (stampFile, info.GetStampFileContents (this));
				} else {
					Log.LogMessage (MessageImportance.Low, "The stamp file '{0}' does not exit for the item '{1}', and it will be created", stampFile, item.ItemSpec);
					Directory.CreateDirectory (Path.GetDirectoryName (stampFile));
					File.WriteAllText (stampFile, info.GetStampFileContents (this));
				}

				var additionalFilesToTouch = item.GetMetadata ("CodesignAdditionalFilesToTouch").Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var file in additionalFilesToTouch) {
					if (IsUpToDate (item.ItemSpec, file)) {
						Log.LogMessage (MessageImportance.Low, "The additional file '{0}' is already up-to-date for the item '{1}'", file, item.ItemSpec);
					} else if (File.Exists (file)) {
						Log.LogMessage (MessageImportance.Low, "The additional file '{0}' for the item '{1}' exists, but is not up-to-date, and it will be touched", file, item.ItemSpec);
						File.SetLastWriteTimeUtc (file, DateTime.UtcNow);
					} else {
						Log.LogMessage (MessageImportance.Low, "The additional file '{0}' for the item '{1}' does not exist, and it won't be created", file, item.ItemSpec);
					}
				}
			}
		}

		static bool IsUpToDate (string itemPath, string stampFile)
		{
			if (!File.Exists (stampFile))
				return false;

			var stampDate = File.GetLastWriteTimeUtc (stampFile);
			DateTime itemDate;
			if (File.Exists (itemPath)) {
				itemDate = File.GetLastWriteTimeUtc (itemPath);
			} else if (Directory.Exists (itemPath)) {
				itemDate = Directory.GetLastWriteTimeUtc (itemPath);
			} else {
				return false;
			}

			return stampDate > itemDate;
		}

		static string EnsureEndsWithDirectorySeparator (string dir)
		{
			if (string.IsNullOrEmpty (dir))
				return dir;

			if (dir [dir.Length - 1] == Path.DirectorySeparatorChar)
				return dir;

			return dir + Path.DirectorySeparatorChar;
		}

		public override bool Execute ()
		{
			try {
				return ExecuteUnsafe ();
			} catch (Exception e) {
				return Log.LogErrorsFromException (e);
			}
		}

		bool ExecuteUnsafe ()
		{
			if (Resources.Length == 0)
				return true;

			var codesignedFiles = new List<ITaskItem> ();
			var resourcesToSign = Resources;

			// 1. Rewrite requests to sign executables inside frameworks to sign the framework itself
			//    signing a framework and a file inside a framework is not *always* identical
			//    on macOS apps {item.ItemSpec} can be a symlink to `Versions/Current/{item.ItemSpec}`
			//    and `Current` also a symlink to `A`... and `_CodeSignature` will be found there
			// 2. Resolve symlinks in the input.
			// 3. Make sure we're working with full paths.
			// All this makes it easier to sort and split the input files into buckets that can be codesigned together,
			// while also not codesigning directories before files inside them.
			foreach (var res in resourcesToSign) {
				var path = res.ItemSpec;
				var parent = Path.GetDirectoryName (path);

				// so do not don't sign `A.framework/A`, sign `A.framework` which will always sign the *bundle*
				if (Path.GetExtension (parent) == ".framework" && Path.GetFileName (path) == Path.GetFileNameWithoutExtension (parent))
					path = parent;

				path = PathUtils.ResolveSymbolicLinks (path);
				path = Path.GetFullPath (path);

				res.ItemSpec = path;
			}

			// first sort all the items by path length, longest path first.
			resourcesToSign = resourcesToSign.OrderBy (v => v.ItemSpec.Length).Reverse ().ToArray ();

			// remove items that are up-to-date
			var itemsToSign = new List<SignInfo> ();
			for (var i = 0; i < resourcesToSign.Length; i++) {
				var item = resourcesToSign [i];
				var info = new SignInfo { Item = item };
				if (NeedsCodesign (resourcesToSign, i, info.GetStampFileContents (this)))
					itemsToSign.Add (info);
			}

			// Then we need to split the input into buckets, where everything in a bucket can be signed in parallel
			// (i.e. no item in a bucket depends on any other item in the bucket being signed first).
			// any such items must go into a different bucket. The bucket themselves are also sorted, where
			// we have to sign the first bucket first, and so on.
			// Since we've sorted by path length, we know that if we find a directory, we won't find any containing
			// files from that directory later.
			var buckets = new List<List<SignInfo>> ();
			for (var i = 0; i < itemsToSign.Count; i++) {
				var info = itemsToSign [i];
				var res = info.Item;
				// All files can go into the first bucket.
				if (File.Exists (res.ItemSpec)) {
					if (buckets.Count == 0)
						buckets.Add (new List<SignInfo> ());
					var bucket = buckets [0];
					bucket.Add (info);
					continue;
				}

				if (Directory.Exists (res.ItemSpec)) {
					var dir = res.ItemSpec;

					// Add the directory separator, so we can do easy substring matches
					dir = EnsureEndsWithDirectorySeparator (dir);

					// This is a directory, which can contain other files or directories that must be signed first
					// If this item is a containing directory for any of the items in a bucket, then we need to
					// add this item to the next bucket. So we go through the buckets in reverse order.
					var added = false;
					for (var b = buckets.Count - 1; b >= 0; b--) {
						var bucket = buckets [b];
						var anyContainingFile = bucket.Any (v => v.Item.ItemSpec.StartsWith (dir, StringComparison.OrdinalIgnoreCase));
						if (anyContainingFile) {
							if (b + 1 >= buckets.Count)
								buckets.Add (new List<SignInfo> ());
							buckets [b + 1].Add (info);
							added = true;
							break;
						}
					}
					if (!added) {
						// This directory doesn't contain any other signed files, so we can add it to the first bucket.
						if (buckets.Count == 0)
							buckets.Add (new List<SignInfo> ());
						var bucket = buckets [0];
						bucket.Add (info);
					}
					continue;
				}

				Log.LogWarning ("Unable to sign '{0}': file or directory not found.", res.ItemSpec);
			}

#if false
			Log.LogWarning ("Codesigning {0} buckets", buckets.Count);
			for (var b = 0; b < buckets.Count; b++) {
				var bucket = buckets [b];
				Log.LogWarning ($"    Bucket #{b + 1} contains {bucket.Count} items:");
				foreach (var item in bucket) {
					Log.LogWarning ($"        {item.Item.ItemSpec}");
				}
			}
#endif

			for (var b = 0; b < buckets.Count; b++) {
				var bucket = buckets [b];
				Parallel.ForEach (bucket, new ParallelOptions { MaxDegreeOfParallelism = Math.Max (Environment.ProcessorCount / 2, 1) }, (item) => {
					Codesign (item);

					var files = GetCodesignedFiles (item.Item);
					lock (codesignedFiles)
						codesignedFiles.AddRange (files);
				});
			}

			// The list of codesigned files has two requirements for Windows:
			// * Only files, no directories
			// * No absolute paths.
			for (var i = codesignedFiles.Count - 1; i >= 0; i--) {
				var item = codesignedFiles [i];
				// Remove directories
				if (Directory.Exists (item.ItemSpec)) {
					codesignedFiles.RemoveAt (i);
					continue;
				}
				if (!Path.IsPathRooted (item.ItemSpec))
					continue;

				// Make path relative. Unfortunately Path.GetRelativePath isn't available in netstandard2.0, which we're targetting, so use a very simple substitute.
				var absolutePath = item.ItemSpec;
				var relativeTo = Environment.CurrentDirectory;
				if (absolutePath.StartsWith (relativeTo, StringComparison.Ordinal)) {
					var relativePath = absolutePath.Substring (relativeTo.Length);
					relativePath = relativePath.TrimStart (Path.DirectorySeparatorChar);
					codesignedFiles [i] = new TaskItem (relativePath);
				}
			}
			CodesignedFiles = codesignedFiles.ToArray ();

			return !Log.HasLoggedErrors;
		}

		IEnumerable<ITaskItem> GetCodesignedFiles (ITaskItem item)
		{
			var codesignedFiles = new List<ITaskItem> ();

			if (Directory.Exists (item.ItemSpec)) {
				var codeSignaturePath = Path.Combine (item.ItemSpec, CodeSignatureDirName);

				if (!Directory.Exists (codeSignaturePath))
					return codesignedFiles;

				codesignedFiles.AddRange (Directory.EnumerateFiles (codeSignaturePath).Select (x => new TaskItem (x)));

				var extension = Path.GetExtension (item.ItemSpec);

				if (extension == ".app" || extension == ".appex") {
					var executableName = Path.GetFileName (item.ItemSpec);
					var manifestPath = Path.Combine (item.ItemSpec, "Info.plist");

					if (File.Exists (manifestPath)) {
						var bundleExecutable = PDictionary.FromFile (manifestPath).GetCFBundleExecutable ();

						if (!string.IsNullOrEmpty (bundleExecutable))
							executableName = bundleExecutable;
					}

					var basePath = item.ItemSpec;

					if (Directory.Exists (Path.Combine (basePath, MacOSDirName)))
						basePath = Path.Combine (basePath, MacOSDirName);

					var executablePath = Path.Combine (basePath, executableName);

					if (File.Exists (executablePath))
						codesignedFiles.Add (new TaskItem (executablePath));
				}
			} else if (File.Exists (item.ItemSpec)) {
				codesignedFiles.Add (item);
			}

			return codesignedFiles;
		}

		class SignInfo {
			public ITaskItem Item;

			IList<string> arguments;
			public IList<string> GetCommandLineArguments (CodesignTaskBase task)
			{
				if (arguments is null)
					arguments = task.GenerateCommandLineArguments (Item);
				return arguments;
			}

			public string GetStampFileContents (CodesignTaskBase task)
			{
				return string.Join (" ", GetCommandLineArguments (task));
			}
		}
	}
}
