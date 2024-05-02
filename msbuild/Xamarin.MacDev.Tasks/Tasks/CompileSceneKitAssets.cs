using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

using Microsoft.Build.Framework;
using TaskItem = Microsoft.Build.Utilities.TaskItem;

using Xamarin.MacDev;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class CompileSceneKitAssets : XamarinTask, ICancelableTask {
		string toolExe;

		#region Inputs

		[Required]
		public string AppBundleName { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		public bool IsWatchApp { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public ITaskItem [] SceneKitAssets { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] BundleResources { get; set; }

		#endregion

		static string ToolName {
			get { return "copySceneKitAssets"; }
		}

		protected virtual string OperatingSystem {
			get {
				return PlatformFrameworkHelper.GetOperatingSystem (TargetFrameworkMoniker);
			}
		}

		string DeveloperRootBinDir {
			get { return Path.Combine (SdkDevPath, "usr", "bin"); }
		}

		string GetFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (DeveloperRootBinDir, ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		Task CopySceneKitAssets (string scnassets, string output, string intermediate)
		{
			var environment = new Dictionary<string, string> ();
			var args = new List<string> ();

			environment.Add ("PATH", DeveloperRootBinDir);
			environment.Add ("XCODE_DEVELOPER_USR_PATH", DeveloperRootBinDir);

			args.Add (Path.GetFullPath (scnassets));
			args.Add ("-o");
			args.Add (Path.GetFullPath (output));
			args.Add ($"--sdk-root={SdkRoot}");

			if (AppleSdkSettings.XcodeVersion.Major >= 10) {
				var platform = PlatformUtils.GetTargetPlatform (SdkPlatform, IsWatchApp);
				if (platform is not null)
					args.Add ($"--target-platform={platform}");

				args.Add ($"--target-version={SdkVersion}");
			} else {
				args.Add ($"--target-version-{OperatingSystem}={SdkVersion}");
			}
			args.Add ($"--target-build-dir={Path.GetFullPath (intermediate)}");
			args.Add ($"--resources-folder-path={AppBundleName}");

			return ExecuteAsync (GetFullPathToTool (), args, sdkDevPath: SdkDevPath, environment: environment, showErrorIfFailure: true);
		}

		static bool TryGetScnAssetsPath (string file, out string scnassets)
		{
			scnassets = file;
			while (scnassets.Length > 0 && Path.GetExtension (scnassets).ToLowerInvariant () != ".scnassets")
				scnassets = Path.GetDirectoryName (scnassets);
			return scnassets.Length > 0;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);

				taskRunner.FixReferencedItems (this, SceneKitAssets);

				FixUpRootedPaths (SceneKitAssets);

				return taskRunner.RunAsync (this).Result;
			}

			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var intermediate = Path.Combine (IntermediateOutputPath, ToolName, AppBundleName);
			var bundleResources = new List<ITaskItem> ();
			var modified = new HashSet<string> ();
			var items = new List<ITaskItem> ();

			foreach (var asset in SceneKitAssets) {
				if (!File.Exists (asset.ItemSpec))
					continue;

				// get the .scnassets directory path
				if (!TryGetScnAssetsPath (asset.ItemSpec, out var scnassets))
					continue;

				var bundleName = BundleResource.GetLogicalName (ProjectDir, prefixes, asset, !string.IsNullOrEmpty (SessionId));
				var output = new TaskItem (Path.Combine (intermediate, bundleName));

				if (!modified.Contains (scnassets) && (!File.Exists (output.ItemSpec) || File.GetLastWriteTimeUtc (asset.ItemSpec) > File.GetLastWriteTimeUtc (output.ItemSpec))) {
					// Base the new item on @asset, to get the `DefiningProject*` metadata too
					var scnassetsItem = new TaskItem (asset);

					// .. but we really want it to be for @scnassets, so set ItemSpec accordingly
					scnassetsItem.ItemSpec = scnassets;

					// .. and set LogicalName, the original one is for @asset
					if (!TryGetScnAssetsPath (bundleName, out var logicalScnAssetsPath)) {
						Log.LogError (null, null, null, asset.ItemSpec, $"Unable to compute the path of the *.scnassets path from the item's LogicalName '{bundleName}'");
						continue;
					}
					scnassetsItem.SetMetadata ("LogicalName", logicalScnAssetsPath);

					// .. and remove the @OriginalItemSpec which is for @asset
					scnassetsItem.RemoveMetadata ("OriginalItemSpec");

					// The Link metadata is for the asset, but 'scnassetsItem' is the containing *.scnasset directory,
					// so we need to update the Link metadata accordingly (if it exists).
					var link = scnassetsItem.GetMetadata ("Link");
					if (!string.IsNullOrEmpty (link)) {
						var newLinkLength = link.Length - (asset.ItemSpec.Length - scnassets.Length);
						if (newLinkLength > 0 && newLinkLength < link.Length) {
							link = link.Substring (0, newLinkLength);
							scnassetsItem.SetMetadata ("Link", link);
						}
					}

					var assetMetadata = asset.GetMetadata ("DefiningProjectFullPath");
					if (assetMetadata != scnassetsItem.GetMetadata ("DefiningProjectFullPath")) {
						// xbuild doesn't set this, so we'll do it
						//
						// `DefiningProjectFullPath` is a reserved metadata for msbuild, so
						// setting this is not allowed anyway
						scnassetsItem.SetMetadata ("DefiningProjectFullPath", assetMetadata);
					}

					modified.Add (scnassets);
					items.Add (scnassetsItem);
				}

				output.SetMetadata ("LogicalName", bundleName);
				output.SetMetadata ("Optimize", "false");
				bundleResources.Add (output);
			}

			if (modified.Count == 0) {
				BundleResources = bundleResources.ToArray ();
				return !Log.HasLoggedErrors;
			}

			if (!Directory.Exists (intermediate))
				Directory.CreateDirectory (intermediate);

			var tasks = new List<Task> ();
			foreach (var item in items) {
				var bundleDir = BundleResource.GetLogicalName (ProjectDir, prefixes, new TaskItem (item), !string.IsNullOrEmpty (SessionId));
				var output = Path.Combine (intermediate, bundleDir);

				tasks.Add (CopySceneKitAssets (item.ItemSpec, output, intermediate));
			}
			Task.WaitAll (tasks.ToArray ());

			BundleResources = bundleResources.ToArray ();

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		void FixUpRootedPaths (ITaskItem [] sceneKitAssets)
		{
			foreach (var item in sceneKitAssets) {
				item.ItemSpec = item.ItemSpec.Replace (":", "");
			}
		}
	}
}
