using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CoreMLCompilerTaskBase : Task
	{
		string toolExe;

		public string ToolName { get { return "coremlc"; } }

		#region Inputs

		public string SessionId { get; set; }

		public bool EnableOnDemandResources { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public ITaskItem[] Models { get; set; }

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
		public ITaskItem[] BundleResources { get; set; }

		[Output]
		public ITaskItem[] PartialAppManifests { get; set; }

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

		static ProcessStartInfo GetProcessStartInfo (IDictionary<string, string> environment, string tool, string args)
		{
			var startInfo = new ProcessStartInfo (tool, args);

			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			foreach (var variable in environment)
				startInfo.EnvironmentVariables[variable.Key] = variable.Value;

			startInfo.CreateNoWindow = true;

			return startInfo;
		}

		int Compile (ITaskItem item, string outputDir, string log, string partialPlist)
		{
			var environment = new Dictionary<string, string> ();
			var args = new CommandLineArgumentBuilder ();

			args.Add ("compile");
			args.AddQuoted (item.ItemSpec);
			args.AddQuoted (Path.GetFullPath (outputDir));
			args.Add ("--output-partial-info-plist");
			args.AddQuoted (partialPlist);

			var startInfo = GetProcessStartInfo (environment, GetFullPathToTool (), args.ToString ());
			var errors = new StringBuilder ();
			int exitCode;

			try {
				Log.LogMessage (MessageImportance.Normal, "Tool {0} execution started with arguments: {1}", startInfo.FileName, startInfo.Arguments);

				using (var stdout = File.CreateText (log)) {
					using (var stderr = new StringWriter (errors)) {
						using (var process = ProcessUtils.StartProcess (startInfo, stdout, stderr)) {
							process.Wait ();

							exitCode = process.Result;
						}
					}

					Log.LogMessage (MessageImportance.Low, "Tool {0} execution finished (exit code = {1}).", startInfo.FileName, exitCode);
				}
			} catch (Exception ex) {
				Log.LogError ("Error executing tool '{0}': {1}", startInfo.FileName, ex.Message);
				File.Delete (log);
				return -1;
			}

			if (exitCode != 0) {
				// Note: coremlc exited with an error. Dump everything we can to help the user
				// diagnose the issue and then delete the log file so that rebuilding tries
				// again.
				if (errors.Length > 0)
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "{0}", errors);

				Log.LogError ("{0} exited with code {1}", ToolName, exitCode);

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

				if (metadata == null)
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
					mapping[outputPath + "/"] = metadata;

					if (FileChanged (model, partialPlist)) {
						Directory.CreateDirectory (outputDir);

						if ((Compile (model, outputDir, log, partialPlist)) != 0)
							return false;
					} else {
						Log.LogMessage (MessageImportance.Low, "Skipping `{0}' as the output file, `{1}', is newer.", model.ItemSpec, partialPlist);
					}
				}

				bundleResources.AddRange (GetCompiledOutput (coremlcOutputDir, mapping));

				foreach (var path in Directory.EnumerateFiles (coremlcOutputDir, "*-partial.plist", SearchOption.AllDirectories))
					partialPlists.Add (new TaskItem (path));
			}

			BundleResources = bundleResources.ToArray ();
			PartialAppManifests = partialPlists.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
