using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CodesignTaskBase : Task
	{
		const string ToolName = "codesign";
		const string MacOSDirName = "MacOS";
		const string CodeSignatureDirName = "_CodeSignature";
		string toolExe;

		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string CodesignAllocate { get; set; }

		public bool DisableTimestamp { get; set; }

		public string Entitlements { get; set; }

		public string Keychain { get; set; }

		[Required]
		public ITaskItem[] Resources { get; set; }

		public string ResourceRules { get; set; }

		[Required]
		public string SigningKey { get; set; }

		public string ExtraArgs { get; set; }

		public bool IsAppExtension { get; set; }

		public bool UseHardenendRuntime { get; set; }

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] CodesignedFiles { get; set; }

		#endregion

		string GetFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		ProcessStartInfo GetProcessStartInfo (string tool, string args)
		{
			var startInfo = new ProcessStartInfo (tool, args);

			startInfo.WorkingDirectory = Environment.CurrentDirectory;
			startInfo.EnvironmentVariables["CODESIGN_ALLOCATE"] = CodesignAllocate;

			startInfo.CreateNoWindow = true;

			return startInfo;
		}

		string GenerateCommandLineArguments (ITaskItem item)
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("-v");
			args.Add ("--force");

			if (IsAppExtension)
				args.Add ("--deep");

			if (UseHardenendRuntime)
				args.Add ("-o runtime");

			args.Add ("--sign");
			args.AddQuoted (SigningKey);

			if (!string.IsNullOrEmpty (Keychain)) {
				args.Add ("--keychain");
				args.AddQuoted (Path.GetFullPath (Keychain));
			}

			if (!string.IsNullOrEmpty (ResourceRules)) {
				args.Add ("--resource-rules");
				args.AddQuoted (Path.GetFullPath (ResourceRules));
			}

			if (!string.IsNullOrEmpty (Entitlements)) {
				args.Add ("--entitlements");
				args.AddQuoted (Path.GetFullPath (Entitlements));
			}

			if (DisableTimestamp)
				args.Add ("--timestamp=none");

			if (!string.IsNullOrEmpty (ExtraArgs))
				args.Add (ExtraArgs);

			args.AddQuoted (Path.GetFullPath (item.ItemSpec));

			return args.ToString ();
		}

		void Codesign (ITaskItem item)
		{
			var startInfo = GetProcessStartInfo (GetFullPathToTool (), GenerateCommandLineArguments (item));
			var messages = new StringBuilder ();
			var errors = new StringBuilder ();
			int exitCode;

			try {
				Log.LogMessage (MessageImportance.Normal, "Tool {0} execution started with arguments: {1}", startInfo.FileName, startInfo.Arguments);

				using (var stdout = new StringWriter (messages)) {
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
				return;
			}

			if (messages.Length > 0)
				Log.LogMessage (MessageImportance.Normal, "{0}", messages.ToString ());

			if (exitCode != 0) {
				if (errors.Length > 0)
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "{0}", errors);
				else
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "{0} failed.", startInfo.FileName);
			}
		}

		public override bool Execute ()
		{
			if (Resources.Length == 0)
				return true;

			var codesignedFiles = new List<ITaskItem> ();

			Parallel.ForEach (Resources, new ParallelOptions { MaxDegreeOfParallelism = Math.Max (Environment.ProcessorCount / 2, 1) }, (item) => {
				Codesign (item);

				codesignedFiles.AddRange (GetCodesignedFiles (item));
			});

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

					if (File.Exists(manifestPath)) {
						var bundleExecutable = PDictionary.FromFile (manifestPath).GetCFBundleExecutable ();

						if (!string.IsNullOrEmpty(bundleExecutable))
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

				var dirName = Path.GetDirectoryName (item.ItemSpec);

				if (Path.GetExtension (dirName) == ".framework")
					codesignedFiles.AddRange (Directory.EnumerateFiles (Path.Combine (dirName, CodeSignatureDirName)).Select (x => new TaskItem (x)));
			}

			return codesignedFiles;
		}
	}
}
