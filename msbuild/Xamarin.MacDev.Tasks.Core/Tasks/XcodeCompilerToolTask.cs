using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class XcodeCompilerToolTask : Task
	{
		protected bool Link { get; set; }
		IList<string> prefixes;
		string toolExe;

		#region Inputs

		public string SessionId { get; set; }

		public ITaskItem AppManifest { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		public string SdkBinPath { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		string sdkDevPath;
		public string SdkDevPath {
			get { return string.IsNullOrEmpty (sdkDevPath) ? "/" : sdkDevPath; }
			set { sdkDevPath = value; }
		}

		public string SdkUsrPath { get; set; }

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

		[Output]
		public ITaskItem[] OutputManifests { get; set; }

		#endregion

		protected abstract string DefaultBinDir {
			get;
		}

		protected string DeveloperRootBinDir {
			get { return Path.Combine (SdkDevPath, "usr", "bin"); }
		}

		protected IList<string> ResourcePrefixes {
			get {
				if (prefixes == null)
					prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);

				return prefixes;
			}
		}

		protected abstract string ToolName { get; }

		protected abstract string MinimumDeploymentTargetKey { get; }

		protected virtual bool UseCompilationDirectory {
			get { return false; }
		}

		protected virtual IEnumerable<string> GetTargetDevices (PDictionary plist)
		{
			yield break;
		}

		protected abstract void AppendCommandLineArguments (IDictionary<string, string> environment, CommandLineArgumentBuilder args, ITaskItem[] items);

		string GetFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (DefaultBinDir, ToolExe);

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

		protected int Compile (ITaskItem[] items, string output, ITaskItem manifest)
		{
			var environment = new Dictionary<string, string> ();
			var args = new CommandLineArgumentBuilder ();

			if (!string.IsNullOrEmpty (SdkBinPath))
				environment.Add ("PATH", SdkBinPath);

			if (!string.IsNullOrEmpty (SdkUsrPath))
				environment.Add ("XCODE_DEVELOPER_USR_PATH", SdkUsrPath);

			args.Add ("--errors", "--warnings", "--notices");
			args.Add ("--output-format", "xml1");

			AppendCommandLineArguments (environment, args, items);

			if (Link)
				args.Add ("--link");
			else if (UseCompilationDirectory)
				args.Add ("--compilation-directory");
			else
				args.Add ("--compile");

			args.AddQuoted (Path.GetFullPath (output));

			foreach (var item in items)
				args.AddQuoted (item.GetMetadata ("FullPath"));

			var startInfo = GetProcessStartInfo (environment, GetFullPathToTool (), args.ToString ());
			var errors = new StringBuilder ();
			int exitCode;

			try {
				Log.LogMessage (MessageImportance.Normal, "Tool {0} execution started with arguments: {1}", startInfo.FileName, startInfo.Arguments);

				using (var stdout = File.CreateText (manifest.ItemSpec)) {
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
				File.Delete (manifest.ItemSpec);
				return -1;
			}

			if (exitCode != 0) {
				// Note: ibtool or actool exited with an error. Dump everything we can to help the user
				// diagnose the issue and then delete the manifest log file so that rebuilding tries
				// again (in case of ibtool's infamous spurious errors).
				if (errors.Length > 0)
					Log.LogError (null, null, null, items[0].ItemSpec, 0, 0, 0, 0, "{0}", errors);

				Log.LogError ("{0} exited with code {1}", ToolName, exitCode);

				// Note: If the log file exists and is parseable, log those warnings/errors as well...
				if (File.Exists (manifest.ItemSpec)) {
					try {
						var plist = PDictionary.FromFile (manifest.ItemSpec);

						LogWarningsAndErrors (plist, items[0]);
					} catch (Exception ex) {
						Log.LogError ("Failed to load {0} log file `{1}`: {2}", ToolName, manifest.ItemSpec, ex.Message);
					}

					File.Delete (manifest.ItemSpec);
				}
			}

			return exitCode;
		}

		protected void LogWarningsAndErrors (PDictionary plist, ITaskItem file)
		{
			PDictionary dictionary;
			PString message;
			PArray array;

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.notices", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("message", out message))
						Log.LogMessage (MessageImportance.Low, "{0}", message.Value);
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.warnings", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("message", out message))
						Log.LogWarning (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.errors", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("message", out message))
						Log.LogError (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}

			//Trying to parse document warnings and erros using a PDictionary first since it's what ibtool is returning when building a storyboard.
			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.notices", ToolName), out dictionary)) {
				foreach (var valuePair in dictionary) {
					array = valuePair.Value as PArray;
					foreach (var item in array.OfType<PDictionary> ()) {
						if (item.TryGetValue ("message", out message))
							Log.LogMessage (MessageImportance.Low, "{0}", message.Value);
					}
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.warnings", ToolName), out dictionary)) {
				foreach (var valuePair in dictionary) {
					array = valuePair.Value as PArray;
					foreach (var item in array.OfType<PDictionary> ()) {
						if (item.TryGetValue ("message", out message))
							Log.LogWarning (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
					}
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.errors", ToolName), out dictionary)) {
				foreach (var valuePair in dictionary) {
					array = valuePair.Value as PArray;
					foreach (var item in array.OfType<PDictionary> ()) {
						if (item.TryGetValue ("message", out message))
							Log.LogError (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
					}
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.errors", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("description", out message))
						Log.LogError (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.notices", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("description", out message))
						Log.LogWarning (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}
		}
	}
}
