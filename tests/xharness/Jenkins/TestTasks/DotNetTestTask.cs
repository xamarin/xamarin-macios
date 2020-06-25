using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	class DotNetTestTask : RunTestTask {
		public DotNetTestTask (Jenkins jenkins, DotNetBuildTask build_task, IProcessManager processManager)
			: base (jenkins, build_task, processManager)
		{
			DotNetBuildTask.SetDotNetEnvironmentVariables (Environment);
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

				await ExecuteProcessAsync (Jenkins.Harness.GetDotNetExecutable (Path.GetDirectoryName (ProjectFile)), args);
			}
		}
	}
}
