using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	class DotNetTestTask : RunTestTask {
		public string Filter { get; set; } = string.Empty;
		public DotNetTestTask (Jenkins jenkins, MSBuildTask build_task, IMlaunchProcessManager processManager)
			: base (jenkins, build_task, processManager)
		{
			MSBuildTask.SetDotNetEnvironmentVariables (Environment);
		}

		public override async Task RunTestAsync ()
		{
			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				var trx = Logs.Create ($"results-{Timestamp}.trx", LogType.TrxLog.ToString ());
				var html = Logs.Create ($"results-{Timestamp}.html", LogType.HtmlLog.ToString ());

				var args = new List<string> {
					"test",
					BuildTask.ProjectFile,
					"--results-directory:" + Logs.Directory,
					"--logger:console;verbosity=detailed",
					"--logger:trx;LogFileName=" + Path.GetFileName (trx.FullPath),
					"--logger:html;LogFileName=" + Path.GetFileName (html.FullPath)
				};

				var envTestFilter = global::System.Environment.GetEnvironmentVariable ("TEST_FILTER");
				if (!string.IsNullOrEmpty (envTestFilter)) {
					if (!string.IsNullOrEmpty (Filter)) {
						Filter = $"({envTestFilter}) & ({Filter})";
					} else {
						Filter = $"{envTestFilter}";
					}
					Jenkins.MainLog.WriteLine ($"Using test filter '{envTestFilter}' for '{TestName}'. Final filter: '{Filter}'");
				}

				if (!string.IsNullOrEmpty (Filter)) {
					args.Add ("--filter");
					args.Add (Filter);
				}

				WorkingDirectory = Path.GetDirectoryName (ProjectFile);

				await ExecuteProcessAsync (Jenkins.Harness.GetDotNetExecutable (Path.GetDirectoryName (ProjectFile)), args);
			}
		}
	}
}
