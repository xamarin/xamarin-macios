using System;
using System.IO;
using System.Json;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class ACToolTaskBase : XcodeCompilerToolTask
	{
		ITaskItem partialAppManifest;
		string outputSpecs;
		PDictionary plist;

		#region Inputs

		public string DeviceModel { get; set; }

		public string DeviceOSVersion { get; set; }

		public bool EnableOnDemandResources { get; set; }

		[Required]
		public ITaskItem[] ImageAssets { get; set; }

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

		static bool IsWatchExtension (PDictionary plist)
		{
			PDictionary extension;
			PString id;

			if (!plist.TryGetValue ("NSExtension", out extension))
				return false;

			if (!extension.TryGetValue ("NSExtensionPointIdentifier", out id))
				return false;

			return id.Value == "com.apple.watchkit";
		}

		static bool IsMessagesExtension (PDictionary plist)
		{
			PDictionary extension;
			PString id;

			if (!plist.TryGetValue ("NSExtension", out extension))
				return false;

			if (!extension.TryGetValue ("NSExtensionPointIdentifier", out id))
				return false;

			return id.Value == "com.apple.message-payload-provider";
		}

		protected override void AppendCommandLineArguments (IDictionary<string, string> environment, CommandLineArgumentBuilder args, ITaskItem[] items)
		{
			string minimumDeploymentTarget;

			if (plist != null) {
				PString value;

				if (!plist.TryGetValue (MinimumDeploymentTargetKey, out value) || string.IsNullOrEmpty (value.Value))
					minimumDeploymentTarget = SdkVersion;
				else
					minimumDeploymentTarget = value.Value;

				var assetDirs = new HashSet<string> (items.Select (x => BundleResource.GetVirtualProjectPath (ProjectDir, x, !string.IsNullOrEmpty (SessionId))));

				if (plist.TryGetValue (ManifestKeys.XSAppIconAssets, out value) && !string.IsNullOrEmpty (value.Value)) {
					int index = value.Value.IndexOf (".xcassets" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
					string assetDir = null;
					var rpath = value.Value;

					if (index != -1)
						assetDir = rpath.Substring (0, index + ".xcassets".Length);

					if (assetDirs != null && assetDirs.Contains (assetDir)) {
						var assetName = Path.GetFileNameWithoutExtension (rpath);

						if (PartialAppManifest == null) {
							args.Add ("--output-partial-info-plist");
							args.AddQuoted (partialAppManifest.GetMetadata ("FullPath"));

							PartialAppManifest = partialAppManifest;
						}

						args.Add ("--app-icon");
						args.AddQuoted (assetName);

						if (IsMessagesExtension (plist))
							args.Add ("--product-type com.apple.product-type.app-extension.messages");
					}
				}

				if (plist.TryGetValue (ManifestKeys.XSLaunchImageAssets, out value) && !string.IsNullOrEmpty (value.Value)) {
					int index = value.Value.IndexOf (".xcassets" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
					string assetDir = null;
					var rpath = value.Value;

					if (index != -1)
						assetDir = rpath.Substring (0, index + ".xcassets".Length);

					if (assetDirs != null && assetDirs.Contains (assetDir)) {
						var assetName = Path.GetFileNameWithoutExtension (rpath);

						if (PartialAppManifest == null) {
							args.Add ("--output-partial-info-plist");
							args.AddQuoted (partialAppManifest.GetMetadata ("FullPath"));

							PartialAppManifest = partialAppManifest;
						}

						args.Add ("--launch-image");
						args.AddQuoted (assetName);
					}
				}

				if (plist.TryGetValue (ManifestKeys.CLKComplicationGroup, out value) && !string.IsNullOrEmpty (value.Value))
					args.Add ("--complication", value);
			} else {
				minimumDeploymentTarget = SdkVersion;
			}

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

			if (plist != null) {
				foreach (var targetDevice in GetTargetDevices (plist))
					args.Add ("--target-device", targetDevice);
			}

			args.Add ("--minimum-deployment-target", minimumDeploymentTarget);

			var platform = PlatformUtils.GetTargetPlatform (SdkPlatform, IsWatchApp);

			if (platform != null)
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
			string bundleIdentifier = null;
			var knownSpecs = new HashSet<string> ();
			var clones = new HashSet<string> ();
			var items = new List<ITaskItem> ();
			var specs = new PArray ();

			switch (SdkPlatform) {
			case "iPhoneSimulator":
			case "iPhoneOS":
			case "MacOSX":
			case "WatchSimulator":
			case "WatchOS":
			case "AppleTVSimulator":
			case "AppleTVOS":
				break;
			default:
				Log.LogError ("Unrecognized platform: {0}", SdkPlatform);
				return false;
			}

			if (AppManifest != null) {
				try {
					plist = PDictionary.FromFile (AppManifest.ItemSpec);
				} catch (Exception ex) {
					Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, "{0}", ex.Message);
					return false;
				}

				bundleIdentifier = plist.GetCFBundleIdentifier ();
			}

			for (int i = 0; i < ImageAssets.Length; i++) {
				var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, ImageAssets[i], !string.IsNullOrEmpty (SessionId));

				// Ignore MacOS .DS_Store files...
				if (Path.GetFileName (vpath).Equals (".DS_Store", StringComparison.OrdinalIgnoreCase))
					continue;

				// get the parent (which will typically be .appiconset, .launchimage, .imageset, .iconset, etc)
				var catalog = Path.GetDirectoryName (vpath);

				// keep walking up the directory structure until we get to the .xcassets directory
				while (!string.IsNullOrEmpty (catalog) && Path.GetExtension (catalog) != ".xcassets")
					catalog = Path.GetDirectoryName (catalog);

				if (string.IsNullOrEmpty (catalog)) {
					Log.LogWarning (null, null, null, ImageAssets[i].ItemSpec, 0, 0, 0, 0, "Asset not part of an asset catalog: {0}", ImageAssets[i].ItemSpec);
					continue;
				}

				if (ImageAssets[i].GetMetadata ("Link") != null) {
					// Note: if any of the files within a catalog are linked, we'll have to clone the *entire* catalog
					clones.Add (catalog);
					continue;
				}

				// filter out everything except paths containing a Contents.json file since our main processing loop only cares about these
				if (Path.GetFileName (vpath) != "Contents.json")
					continue;

				items.Add (ImageAssets[i]);
			}

			// clone any *.xcassets dirs that need cloning
			if (clones.Count > 0) {
				if (Directory.Exists (intermediateCloneDir))
					Directory.Delete (intermediateCloneDir, true);

				Directory.CreateDirectory (intermediateCloneDir);

				items.Clear ();

				for (int i = 0; i < ImageAssets.Length; i++) {
					var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, ImageAssets[i], !string.IsNullOrEmpty (SessionId));
					var clone = false;
					ITaskItem item;

					// Ignore MacOS .DS_Store files...
					if (Path.GetFileName (vpath).Equals (".DS_Store", StringComparison.OrdinalIgnoreCase))
						continue;

					foreach (var catalog in clones) {
						if (vpath.Length > catalog.Length && vpath[catalog.Length] == '/' && vpath.StartsWith (catalog, StringComparison.Ordinal)) {
							clone = true;
							break;
						}
					}

					if (clone) {
						var src = ImageAssets[i].GetMetadata ("FullPath");

						if (!File.Exists (src)) {
							Log.LogError (null, null, null, src, 0, 0, 0, 0, "File not found: {0}", src);
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
						ImageAssets[i].CopyMetadataTo (item);
						item.SetMetadata ("Link", vpath);
					} else {
						// filter out everything except paths containing a Contents.json file since our main processing loop only cares about these
						if (Path.GetFileName (vpath) != "Contents.json")
							continue;

						item = ImageAssets[i];
					}

					items.Add (item);
				}
			}

			// Note: `items` contains only the Contents.json files at this point
			for (int i = 0; i < items.Count; i++) {
				var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, items[i], !string.IsNullOrEmpty (SessionId));
				var path = items[i].GetMetadata ("FullPath");

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

				if (AppleSdkSettings.XcodeVersion.Major >= 7 && !string.IsNullOrEmpty (bundleIdentifier) && SdkPlatform != "WatchSimulator") {
					var text = File.ReadAllText (items[i].ItemSpec);

					if (string.IsNullOrEmpty (text))
						continue;

					JsonObject json;
					JsonValue value;

					try {
						json = (JsonObject) JsonValue.Parse (text);
					} catch (ArgumentException ex) {
						// ... At line ###, column ###
						int line = 0, column = 0;
						int index, endIndex;

						var message = ex.Message;
						if (message.EndsWith (".", StringComparison.Ordinal))
							message = message.Substring (0, message.Length - 1);
						if ((index = message.IndexOf ("At line ", StringComparison.Ordinal)) != -1) {
							index += "At line ".Length;

							if ((endIndex = message.IndexOf (", column ", index, StringComparison.Ordinal)) != -1) {
								var columnBuf = message.Substring (endIndex + ", column ".Length);
								var lineBuf = message.Substring (index, endIndex - index);

								int.TryParse (columnBuf, out column);
								int.TryParse (lineBuf, out line);
							}
						}

						Log.LogError (null, null, null, items[i].ItemSpec, line, column, line, column, "{0}", ex.Message);
						return false;
					} catch (InvalidCastException) {
						Log.LogError (null, null, null, items[i].ItemSpec, 0, 0, 0, 0, "Invalid json.");
						return false;
					}

					if (!json.TryGetValue ("properties", out value) || value.JsonType != JsonType.Object)
						continue;

					var properties = (JsonObject) value;

					if (!properties.TryGetValue ("on-demand-resource-tags", out value) || value.JsonType != JsonType.Array)
						continue;

					var resourceTags = (JsonArray) value;
					var tags = new HashSet<string> ();
					string hash;

					foreach (var tag in resourceTags) {
						if (tag.JsonType == JsonType.String)
							tags.Add ((string) tag);
					}

					var tagList = tags.ToList ();
					tagList.Sort ();

					var assetDir = AssetPackUtils.GetAssetPackDirectory (intermediate, bundleIdentifier, tagList, out hash);

					if (knownSpecs.Add (hash)) {
						var assetpack = new PDictionary ();
						var ptags = new PArray ();

						Directory.CreateDirectory (assetDir);

						for (int j = 0; j < tagList.Count; j++)
							ptags.Add (new PString (tagList[j]));

						assetpack.Add ("bundle-id", new PString (string.Format ("{0}.asset-pack-{1}", bundleIdentifier, hash)));
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

			if (PartialAppManifest != null && !File.Exists (PartialAppManifest.GetMetadata ("FullPath")))
				Log.LogError ("Partial Info.plist file was not generated: {0}", PartialAppManifest.GetMetadata ("FullPath"));

			try {
				var manifestOutput = PDictionary.FromFile (manifest.ItemSpec);

				LogWarningsAndErrors (manifestOutput, catalogs[0]);

				bundleResources.AddRange (GetCompiledBundleResources (manifestOutput, intermediateBundleDir));
				outputManifests.Add (manifest);
			} catch (Exception ex) {
				Log.LogError ("Failed to load {0} log file `{1}`: {2}", ToolName, manifest.ItemSpec, ex.Message);
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
