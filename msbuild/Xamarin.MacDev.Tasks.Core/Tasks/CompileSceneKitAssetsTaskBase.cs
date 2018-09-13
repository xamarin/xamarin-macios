using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CompileSceneKitAssetsTaskBase : Task
	{
		string toolExe;

		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		public bool IsWatchApp { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public ITaskItem[] SceneKitAssets { get; set; }

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
		public ITaskItem[] BundleResources { get; set; }

		#endregion

		static string ToolName {
			get { return "copySceneKitAssets"; }
		}

		protected abstract string OperatingSystem {
			get;
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

		static ProcessStartInfo GetProcessStartInfo (IDictionary<string, string> environment, string tool, string args)
		{
			var startInfo = new ProcessStartInfo (tool, args);

			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			foreach (var variable in environment)
				startInfo.EnvironmentVariables[variable.Key] = variable.Value;

			startInfo.RedirectStandardOutput = true;
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;

			return startInfo;
		}

		int CopySceneKitAssets (string scnassets, string output, string intermediate)
		{
			var environment = new Dictionary<string, string> ();
			var args = new CommandLineArgumentBuilder ();

			environment.Add ("PATH", DeveloperRootBinDir);
			environment.Add ("DEVELOPER_DIR", SdkDevPath);
			environment.Add ("XCODE_DEVELOPER_USR_PATH", DeveloperRootBinDir);

			args.AddQuoted (Path.GetFullPath (scnassets));
			args.Add ("-o");
			args.AddQuoted (Path.GetFullPath (output));
			args.AddQuotedFormat ("--sdk-root={0}", SdkRoot);

			if (AppleSdkSettings.XcodeVersion.Major >= 10) {
				var platform = PlatformUtils.GetTargetPlatform (SdkPlatform, IsWatchApp);
				if (platform != null)
					args.AddQuotedFormat ("--target-platform={0}", platform);

				args.AddQuotedFormat ("--target-version={0}", SdkVersion);
			} else {
				args.AddQuotedFormat ("--target-version-{0}={1}", OperatingSystem, SdkVersion);
			}
			args.AddQuotedFormat ("--target-build-dir={0}", Path.GetFullPath (intermediate));

			var startInfo = GetProcessStartInfo (environment, GetFullPathToTool (), args.ToString ());

			try {
				using (var process = new Process ()) {
					Log.LogMessage (MessageImportance.Normal, "Tool {0} execution started with arguments: {1}", startInfo.FileName, startInfo.Arguments);

					process.StartInfo = startInfo;
					process.OutputDataReceived += (sender, e) => {
						if (e.Data == null)
							return;

						Log.LogMessage (MessageImportance.Low, "{0}", e.Data);
					};

					process.Start ();
					process.BeginOutputReadLine ();
					process.WaitForExit ();

					Log.LogMessage (MessageImportance.Low, "Tool {0} execution finished.", startInfo.FileName);

					return process.ExitCode;
				}
			} catch (Exception ex) {
				Log.LogError ("Error executing tool '{0}': {1}", startInfo.FileName, ex.Message);
				return -1;
			}
		}

		public override bool Execute ()
		{
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var intermediate = Path.Combine (IntermediateOutputPath, ToolName);
			var bundleResources = new List<ITaskItem> ();
			var modified = new HashSet<string> ();
			var items = new List<ITaskItem> ();
			string metadata;

			foreach (var asset in SceneKitAssets) {
				if (!File.Exists (asset.ItemSpec))
					continue;

				// get the .scnassets directory path
				var scnassets = Path.GetDirectoryName (asset.ItemSpec);
				while (scnassets.Length > 0 && Path.GetExtension (scnassets).ToLowerInvariant () != ".scnassets")
					scnassets = Path.GetDirectoryName (scnassets);

				if (scnassets.Length == 0)
					continue;

				metadata = asset.GetMetadata ("LogicalName");
				if (!string.IsNullOrEmpty (metadata))
					asset.SetMetadata ("LogicalName", string.Empty);

				var bundleName = BundleResource.GetLogicalName (ProjectDir, prefixes, asset, !string.IsNullOrEmpty(SessionId));
				var output = new TaskItem (Path.Combine (intermediate, bundleName));

				if (!modified.Contains (scnassets) && (!File.Exists (output.ItemSpec) || File.GetLastWriteTimeUtc (asset.ItemSpec) > File.GetLastWriteTimeUtc (output.ItemSpec))) {
					// Base the new item on @asset, to get the `DefiningProject*` metadata too
					var scnassetsItem = new TaskItem (asset);

					// .. but we really want it to be for @scnassets, so set ItemSpec accordingly
					scnassetsItem.ItemSpec = scnassets;

					// .. and remove the @OriginalItemSpec which is for @asset
					scnassetsItem.RemoveMetadata ("OriginalItemSpec");

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

			foreach (var item in items) {
				var bundleDir = BundleResource.GetLogicalName (ProjectDir, prefixes, new TaskItem (item), !string.IsNullOrEmpty(SessionId));
				var output = Path.Combine (intermediate, bundleDir);

				if (CopySceneKitAssets (item.ItemSpec, output, intermediate) == -1)
					return false;
			}

			BundleResources = bundleResources.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
