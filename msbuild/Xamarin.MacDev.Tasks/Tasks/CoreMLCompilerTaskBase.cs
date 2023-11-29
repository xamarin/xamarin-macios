using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CoreMLCompilerTaskBase : XamarinTask {
		string toolExe;

		public string ToolName { get { return "coremlc"; } }

		#region Inputs

		public bool EnableOnDemandResources { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public ITaskItem [] Models { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		string sdkDevPath;
		public string SdkDevPath {
			get { return string.IsNullOrEmpty (sdkDevPath) ? "/" : sdkDevPath; }
			set { sdkDevPath = value; }
		}

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] BundleResources { get; set; }

		[Output]
		public ITaskItem [] PartialAppManifests { get; set; }

		#endregion

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

		int Compile (ITaskItem item, string outputDir, string log, string partialPlist)
		{
			var args = new List<string> ();

			args.Add ("compile");
			args.Add (item.ItemSpec);
			args.Add (Path.GetFullPath (outputDir));
			args.Add ("--output-partial-info-plist");
			args.Add (partialPlist);

			var fileName = GetFullPathToTool ();

			var rv = ExecuteAsync (fileName, args, sdkDevPath, mergeOutput: false).Result;
			var exitCode = rv.ExitCode;
			var output = rv.StandardOutput.ToString ();
			File.WriteAllText (log, output);

			if (exitCode != 0) {
				// Note: coremlc exited with an error. Dump everything we can to help the user
				// diagnose the issue and then delete the log file so that rebuilding tries
				// again.
				var errors = rv.StandardError.ToString ();
				if (errors.Length > 0)
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "{0}", errors);

				Log.LogError (MSBStrings.E0117, ToolName, exitCode);

				// Note: If the log file exists, log those warnings/errors as well...
				if (File.Exists (log)) {
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "{0}", File.ReadAllText (log));
					File.Delete (log);
				}
			}

			return exitCode;
		}

		static string GetPathWithoutExtension (string path)
		{
			var fileName = Path.GetFileNameWithoutExtension (path);
			var dir = Path.GetDirectoryName (path);

			if (string.IsNullOrEmpty (dir))
				return fileName;

			return Path.Combine (dir, fileName);
		}

		static bool FileChanged (ITaskItem model, string log)
		{
			return !File.Exists (log) || File.GetLastWriteTimeUtc (log) < File.GetLastWriteTimeUtc (model.ItemSpec);
		}

		IEnumerable<ITaskItem> GetCompiledOutput (string baseOutputDir, IDictionary<string, IDictionary> mapping)
		{
			foreach (var path in Directory.EnumerateFiles (baseOutputDir, "*.*", SearchOption.AllDirectories)) {
				IDictionary metadata = null;
				string rpath = null;

				foreach (var kvp in mapping) {
					if (path.StartsWith (kvp.Key, StringComparison.Ordinal)) {
						rpath = path.Substring (kvp.Key.Length);
						metadata = kvp.Value;
						break;
					}
				}

				if (metadata is null)
					continue;

				var compiled = new TaskItem (path, metadata);

				// adjust the logical name
				var logicalName = compiled.GetMetadata ("LogicalName");
				var bundleName = Path.Combine (logicalName, rpath);

				compiled.SetMetadata ("LogicalName", bundleName);

				yield return compiled;
			}

			yield break;
		}

		public override bool Execute ()
		{
			var coremlcOutputDir = Path.Combine (IntermediateOutputPath, "coremlc");
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var mapping = new Dictionary<string, IDictionary> ();
			var bundleResources = new List<ITaskItem> ();
			var partialPlists = new List<ITaskItem> ();

			if (Models.Length > 0) {
				Directory.CreateDirectory (coremlcOutputDir);

				foreach (var model in Models) {
					var logicalName = BundleResource.GetLogicalName (ProjectDir, prefixes, model, !string.IsNullOrEmpty (SessionId));
					var bundleName = GetPathWithoutExtension (logicalName) + ".mlmodelc";
					var outputPath = Path.Combine (coremlcOutputDir, bundleName);
					var outputDir = Path.GetDirectoryName (outputPath);
					var partialPlist = GetPathWithoutExtension (outputPath) + "-partial.plist";
					var log = GetPathWithoutExtension (outputPath) + ".log";
					var resourceTags = model.GetMetadata ("ResourceTags");
					var output = new TaskItem (outputPath);

					output.SetMetadata ("LogicalName", bundleName);
					output.SetMetadata ("Optimize", "false");

					if (EnableOnDemandResources && !string.IsNullOrEmpty (resourceTags))
						output.SetMetadata ("ResourceTags", resourceTags);

					var metadata = output.CloneCustomMetadata ();
					mapping [outputPath + "/"] = metadata;

					if (FileChanged (model, partialPlist)) {
						Directory.CreateDirectory (outputDir);

						if ((Compile (model, outputDir, log, partialPlist)) != 0)
							return false;
					} else {
						Log.LogMessage (MessageImportance.Low, MSBStrings.M0119, model.ItemSpec, partialPlist);
					}
				}

				bundleResources.AddRange (GetCompiledOutput (coremlcOutputDir, mapping));

				foreach (var path in Directory.EnumerateFiles (coremlcOutputDir, "*-partial.plist", SearchOption.AllDirectories))
					partialPlists.Add (new TaskItem (path));
			}

			BundleResources = bundleResources.ToArray ();

			if (PartialAppManifests is not null)
				partialPlists.AddRange (PartialAppManifests);
			PartialAppManifests = partialPlists.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
