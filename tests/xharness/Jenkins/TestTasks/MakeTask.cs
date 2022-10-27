using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	class MakeTask : BuildToolTask {
		public string Target;
		public string WorkingDirectory;
		public TimeSpan Timeout = TimeSpan.FromMinutes (5);

		public MakeTask (Jenkins jenkins, IProcessManager processManager) : base (jenkins, null, processManager)
		{
		}

		protected override async Task ExecuteAsync ()
		{
			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				using (var make = new Process ()) {
					make.StartInfo.FileName = "make";
					make.StartInfo.WorkingDirectory = WorkingDirectory;
					make.StartInfo.Arguments = Target;
					SetEnvironmentVariables (make);
					var log = Logs.Create ($"make-{Platform}-{Timestamp}.txt", LogType.BuildLog.ToString ());
					LogEvent (log, "Making {0} in {1}", Target, WorkingDirectory);
					if (!Jenkins.Harness.DryRun) {
						var timeout = Timeout;
						var result = await ProcessManager.RunAsync (make, log, timeout);
						if (result.TimedOut) {
							ExecutionResult = TestExecutingResult.TimedOut;
							log.WriteLine ("Make timed out after {0} seconds.", timeout.TotalSeconds);
						} else if (result.Succeeded) {
							ExecutionResult = TestExecutingResult.Succeeded;
						} else {
							ExecutionResult = TestExecutingResult.Failed;
						}
					}
					using (var reader = log.GetReader ())
						AddCILogFiles (reader);
					Jenkins.MainLog.WriteLine ("Made {0} ({1})", TestName, Mode);
				}
			}
		}
	}
}
