using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CodesignTaskBase : Task
	{
		const string ToolName = "codesign";
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

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

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

			Parallel.ForEach (Resources, new ParallelOptions { MaxDegreeOfParallelism = Math.Max (Environment.ProcessorCount / 2, 1) }, (item) => {
				Codesign (item);
			});

			return !Log.HasLoggedErrors;
		}
	}
}
