using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class ACToolTaskBase : XcodeCompilerToolTask {
		ITaskItem partialAppManifest;
		string outputSpecs;

		#region Inputs

		public string AccentColor { get; set; }

		public string DeviceModel { get; set; }

		public string DeviceOSVersion { get; set; }

		public bool EnableOnDemandResources { get; set; }

		[Required]
		public ITaskItem [] ImageAssets { get; set; }

		public bool IsWatchApp { get; set; }

		[Required]
		public bool OptimizePNGs { get; set; }

		[Required]
		public string OutputPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem PartialAppManifest { get; set; }

		#endregion

		protected override string DefaultBinDir {
			get { return DeveloperRootBinDir; }
		}

		protected override string ToolName {
			get { return "actool"; }
		}

		bool IsMessagesExtension {
			get {
				return NSExtensionPointIdentifier == "com.apple.message-payload-provider";
			}
		}

		protected override void AppendCommandLineArguments (IDictionary<string, string> environment, CommandLineArgumentBuilder args, ITaskItem [] items)
		{
			var assetDirs = new HashSet<string> (items.Select (x => BundleResource.GetVirtualProjectPath (ProjectDir, x, !string.IsNullOrEmpty (SessionId))));

			if (!string.IsNullOrEmpty (XSAppIconAssets)) {
				int index = XSAppIconAssets.IndexOf (".xcassets" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
				string assetDir = null;
				var rpath = XSAppIconAssets;

				if (index != -1)
					assetDir = rpath.Substring (0, index + ".xcassets".Length);

				if (assetDirs is not null && assetDirs.Contains (assetDir)) {
					var assetName = Path.GetFileNameWithoutExtension (rpath);

					if (PartialAppManifest is null) {
						args.Add ("--output-partial-info-plist");
						args.AddQuoted (partialAppManifest.GetMetadata ("FullPath"));

						PartialAppManifest = partialAppManifest;
					}

					args.Add ("--app-icon");
					args.AddQuoted (assetName);

					if (IsMessagesExtension)
						args.Add ("--product-type com.apple.product-type.app-extension.messages");
				}
			}

			if (!string.IsNullOrEmpty (AccentColor)) {
				args.Add ("--accent-color", AccentColor);
			}

			if (!string.IsNullOrEmpty (XSLaunchImageAssets)) {
				int index = XSLaunchImageAssets.IndexOf (".xcassets" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
				string assetDir = null;
				var rpath = XSLaunchImageAssets;

				if (index != -1)
					assetDir = rpath.Substring (0, index + ".xcassets".Length);

				if (assetDirs is not null && assetDirs.Contains (assetDir)) {
					var assetName = Path.GetFileNameWithoutExtension (rpath);

					if (PartialAppManifest is null) {
						args.Add ("--output-partial-info-plist");
						args.AddQuoted (partialAppManifest.GetMetadata ("FullPath"));

						PartialAppManifest = partialAppManifest;
					}

					args.Add ("--launch-image");
					args.AddQuoted (assetName);
				}
			}

			if (!string.IsNullOrEmpty (CLKComplicationGroup))
				args.Add ("--complication", CLKComplicationGroup);

			if (OptimizePNGs)
				args.Add ("--compress-pngs");

			if (AppleSdkSettings.XcodeVersion.Major >= 7) {
				if (!string.IsNullOrEmpty (outputSpecs))
					args.Add ("--enable-on-demand-resources", EnableOnDemandResources ? "YES" : "NO");

				if (!string.IsNullOrEmpty (DeviceModel))
					args.Add ("--filter-for-device-model", DeviceModel);

				if (!string.IsNullOrEmpty (DeviceOSVersion))
					args.Add ("--filter-for-device-os-version", DeviceOSVersion);

				if (!string.IsNullOrEmpty (outputSpecs)) {
					args.Add ("--asset-pack-output-specifications");
					args.AddQuoted (Path.GetFullPath (outputSpecs));
				}
			}

			if (Platform == ApplePlatform.MacCatalyst) {
				args.Add ("--ui-framework-family");
				args.Add ("uikit");
			}

			foreach (var targetDevice in GetTargetDevices ())
				args.Add ("--target-device", targetDevice);

			args.Add ("--minimum-deployment-target", MinimumOSVersion);

			var platform = PlatformUtils.GetTargetPlatform (SdkPlatform, IsWatchApp);

			if (platform is not null)
				args.Add ("--platform", platform);
		}

		IEnumerable<ITaskItem> GetCompiledBundleResources (PDictionary output, string intermediateBundleDir)
		{
			var pwd = PathUtils.ResolveSymbolicLinks (Environment.CurrentDirectory);
			PDictionary dict;
			PArray array;

			if (output.TryGetValue (string.Format ("com.apple.{0}.compilation-results", ToolName), out dict) && dict.TryGetValue ("output-files", out array)) {
				foreach (var path in array.OfType<PString> ().Select (x => x.Value)) {
					// don't include the generated plist files as BundleResources
					if (path.EndsWith ("partial-info.plist", StringComparison.Ordinal))
						continue;

					var vpath = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLinks (path));
					var item = new TaskItem (vpath);

					// Note: the intermediate bundle dir functions as a top-level bundle dir
					var logicalName = PathUtils.AbsoluteToRelative (intermediateBundleDir, path);

					if (logicalName.StartsWith ("../OnDemandResources/", StringComparison.Ordinal)) {
						logicalName = logicalName.Substring (3);

						var outputPath = Path.Combine (OutputPath, logicalName);

						item.SetMetadata ("OutputPath", outputPath);
					}

					item.SetMetadata ("LogicalName", logicalName);
					item.SetMetadata ("Optimize", "false");

					yield return item;
				}
			}

			yield break;
		}

		public override bool Execute ()
		{
			var intermediate = Path.Combine (IntermediateOutputPath, ToolName);
			var intermediateBundleDir = Path.Combine (intermediate, "bundle");
			var intermediateCloneDir = Path.Combine (intermediate, "cloned-assets");
			var manifest = new TaskItem (Path.Combine (intermediate, "asset-manifest.plist"));
			var bundleResources = new List<ITaskItem> ();
			var outputManifests = new List<ITaskItem> ();
			var catalogs = new List<ITaskItem> ();
			var unique = new HashSet<string> ();

			var knownSpecs = new HashSet<string> ();
			var clones = new HashSet<string> ();
			var items = new List<ITaskItem> ();
			var specs = new PArray ();

			for (int i = 0; i < ImageAssets.Length; i++) {
				var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, ImageAssets [i], !string.IsNullOrEmpty (SessionId));

				// Ignore MacOS .DS_Store files...
				if (Path.GetFileName (vpath).Equals (".DS_Store", StringComparison.OrdinalIgnoreCase))
					continue;

				// get the parent (which will typically be .appiconset, .launchimage, .imageset, .iconset, etc)
				var catalog = Path.GetDirectoryName (vpath);

				// keep walking up the directory structure until we get to the .xcassets directory
				while (!string.IsNullOrEmpty (catalog) && Path.GetExtension (catalog) != ".xcassets")
					catalog = Path.GetDirectoryName (catalog);

				if (string.IsNullOrEmpty (catalog)) {
					Log.LogWarning (null, null, null, ImageAssets [i].ItemSpec, 0, 0, 0, 0, MSBStrings.W0090, ImageAssets [i].ItemSpec);
					continue;
				}

				if (!string.IsNullOrEmpty (ImageAssets [i].GetMetadata ("Link"))) {
					// Note: if any of the files within a catalog are linked, we'll have to clone the *entire* catalog
					clones.Add (catalog);
					continue;
				}

				// filter out everything except paths containing a Contents.json file since our main processing loop only cares about these
				if (Path.GetFileName (vpath) != "Contents.json")
					continue;

				items.Add (ImageAssets [i]);
			}

			// clone any *.xcassets dirs that need cloning
			if (clones.Count > 0) {
				if (Directory.Exists (intermediateCloneDir))
					Directory.Delete (intermediateCloneDir, true);

				Directory.CreateDirectory (intermediateCloneDir);

				items.Clear ();

				for (int i = 0; i < ImageAssets.Length; i++) {
					var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, ImageAssets [i], !string.IsNullOrEmpty (SessionId));
					var clone = false;
					ITaskItem item;

					// Ignore MacOS .DS_Store files...
					if (Path.GetFileName (vpath).Equals (".DS_Store", StringComparison.OrdinalIgnoreCase))
						continue;

					foreach (var catalog in clones) {
						if (vpath.Length > catalog.Length && vpath [catalog.Length] == '/' && vpath.StartsWith (catalog, StringComparison.Ordinal)) {
							clone = true;
							break;
						}
					}

					if (clone) {
						var src = ImageAssets [i].GetMetadata ("FullPath");

						if (!File.Exists (src)) {
							Log.LogError (null, null, null, src, 0, 0, 0, 0, MSBStrings.E0091, src);
							return false;
						}

						var dest = Path.Combine (intermediateCloneDir, vpath);
						var dir = Path.GetDirectoryName (dest);

						Directory.CreateDirectory (dir);

						File.Copy (src, dest, true);

						// filter out everything except paths containing a Contents.json file since our main processing loop only cares about these
						if (Path.GetFileName (vpath) != "Contents.json")
							continue;

						item = new TaskItem (dest);
						ImageAssets [i].CopyMetadataTo (item);
						item.SetMetadata ("Link", vpath);
					} else {
						// filter out everything except paths containing a Contents.json file since our main processing loop only cares about these
						if (Path.GetFileName (vpath) != "Contents.json")
							continue;

						item = ImageAssets [i];
					}

					items.Add (item);
				}
			}

			// Note: `items` contains only the Contents.json files at this point
			for (int i = 0; i < items.Count; i++) {
				var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, items [i], !string.IsNullOrEmpty (SessionId));
				var path = items [i].GetMetadata ("FullPath");

				// get the parent (which will typically be .appiconset, .launchimage, .imageset, .iconset, etc)
				var catalog = Path.GetDirectoryName (vpath);
				path = Path.GetDirectoryName (path);

				// keep walking up the directory structure until we get to the .xcassets directory
				while (!string.IsNullOrEmpty (catalog) && Path.GetExtension (catalog) != ".xcassets") {
					catalog = Path.GetDirectoryName (catalog);
					path = Path.GetDirectoryName (path);
				}

				if (unique.Add (catalog)) {
					var item = new TaskItem (path);
					item.SetMetadata ("Link", catalog);

					catalogs.Add (item);
				}

				if (AppleSdkSettings.XcodeVersion.Major >= 7 && SdkPlatform != "WatchSimulator") {
					var text = File.ReadAllText (items [i].ItemSpec);

					if (string.IsNullOrEmpty (text))
						continue;

					JsonDocument json;
					JsonElement value;

					try {
						var options = new JsonDocumentOptions () {
							AllowTrailingCommas = true,
						};
						json = JsonDocument.Parse (text, options);
					} catch (JsonException je) {
						var line = (int) (je.LineNumber + 1 ?? 0);
						var col = (int) (je.BytePositionInLine + 1 ?? 0);
						Log.LogError (null, null, null, items [i].ItemSpec, line, col, line, col, "{0}", je.Message);
						return false;
					} catch (Exception e) {
						Log.LogError (null, null, null, items [i].ItemSpec, 0, 0, 0, 0, MSBStrings.E0092, e.Message);
						return false;

					}

					if (!json.RootElement.TryGetProperty ("properties", out value) || value.ValueKind != JsonValueKind.Object)
						continue;

					var properties = value;

					if (!properties.TryGetProperty ("on-demand-resource-tags", out value) || value.ValueKind != JsonValueKind.Array)
						continue;

					var resourceTags = value;
					var tags = new HashSet<string> ();
					string hash;

					foreach (var tag in resourceTags.EnumerateArray ()) {
						if (tag.ValueKind == JsonValueKind.String)
							tags.Add (tag.GetString ());
					}

					var tagList = tags.ToList ();
					tagList.Sort ();

					var assetDir = AssetPackUtils.GetAssetPackDirectory (intermediate, BundleIdentifier, tagList, out hash);

					if (knownSpecs.Add (hash)) {
						var assetpack = new PDictionary ();
						var ptags = new PArray ();

						Directory.CreateDirectory (assetDir);

						for (int j = 0; j < tagList.Count; j++)
							ptags.Add (new PString (tagList [j]));

						assetpack.Add ("bundle-id", new PString (string.Format ("{0}.asset-pack-{1}", BundleIdentifier, hash)));
						assetpack.Add ("bundle-path", new PString (Path.GetFullPath (assetDir)));
						assetpack.Add ("tags", ptags);
						specs.Add (assetpack);
					}
				}
			}

			if (catalogs.Count == 0) {
				// There are no (supported?) asset catalogs
				return !Log.HasLoggedErrors;
			}

			partialAppManifest = new TaskItem (Path.Combine (intermediate, "partial-info.plist"));

			if (specs.Count > 0) {
				outputSpecs = Path.Combine (intermediate, "output-specifications.plist");
				specs.Save (outputSpecs, true);
			}

			Directory.CreateDirectory (intermediateBundleDir);

			// Note: Compile() will set the PartialAppManifest property if it is used...
			if ((Compile (catalogs.ToArray (), intermediateBundleDir, manifest)) != 0)
				return false;

			if (PartialAppManifest is not null && !File.Exists (PartialAppManifest.GetMetadata ("FullPath")))
				Log.LogError (MSBStrings.E0093, PartialAppManifest.GetMetadata ("FullPath"));

			try {
				var manifestOutput = PDictionary.FromFile (manifest.ItemSpec);

				LogWarningsAndErrors (manifestOutput, catalogs [0]);

				bundleResources.AddRange (GetCompiledBundleResources (manifestOutput, intermediateBundleDir));
				outputManifests.Add (manifest);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0094, ToolName, manifest.ItemSpec, ex.Message);
			}

			foreach (var assetpack in specs.OfType<PDictionary> ()) {
				var path = Path.Combine (assetpack.GetString ("bundle-path").Value, "Info.plist");
				var bundlePath = PathUtils.AbsoluteToRelative (intermediate, path);
				var outputPath = Path.Combine (OutputPath, bundlePath);
				var rpath = Path.Combine (intermediate, bundlePath);
				var dict = new PDictionary ();

				dict.SetCFBundleIdentifier (assetpack.GetString ("bundle-id").Value);
				dict.Add ("Tags", assetpack.GetArray ("tags").Clone ());

				dict.Save (path, true, true);

				var item = new TaskItem (rpath);
				item.SetMetadata ("LogicalName", bundlePath);
				item.SetMetadata ("OutputPath", outputPath);
				item.SetMetadata ("Optimize", "false");

				bundleResources.Add (item);
			}

			BundleResources = bundleResources.ToArray ();
			OutputManifests = outputManifests.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
