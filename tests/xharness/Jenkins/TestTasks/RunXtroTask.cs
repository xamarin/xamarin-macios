using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	class RunXtroTask : MacExecuteTask {
		public string AnnotationsDirectory;
		string mode;

		public RunXtroTask (Jenkins jenkins, BuildToolTask build_task, IMlaunchProcessManager processManager, ICrashSnapshotReporterFactory crashReportSnapshotFactory)
			: base (jenkins, build_task, processManager, crashReportSnapshotFactory)
		{
		}

		public override string Mode {
			get => mode;
			set => mode = value;
		}

		public override async Task RunTestAsync ()
		{
			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				using (var proc = new Process ()) {
					proc.StartInfo.FileName = "/Library/Frameworks/Mono.framework/Commands/mono";
					var reporter = System.IO.Path.Combine (WorkingDirectory, "xtro-report/bin/Debug/xtro-report.exe");
					var results = System.IO.Path.Combine (Logs.Directory, $"xtro-{Timestamp}");
					proc.StartInfo.Arguments = $"--debug {reporter} {AnnotationsDirectory} {results}";

					Jenkins.MainLog.WriteLine ("Executing {0} ({1})", TestName, Mode);
					var log = Logs.Create ($"execute-xtro-{Timestamp}.txt", LogType.ExecutionLog.ToString ());
					log.WriteLine ("{0} {1}", proc.StartInfo.FileName, proc.StartInfo.Arguments);
					if (!Jenkins.Harness.DryRun) {
						ExecutionResult = TestExecutingResult.Running;

						var snapshot = CrashReportSnapshotFactory.Create (log, Logs, isDevice: false, deviceName: null);
						await snapshot.StartCaptureAsync ();

						try {
							var timeout = TimeSpan.FromMinutes (20);

							var result = await ProcessManager.RunAsync (proc, log, timeout);
							if (result.TimedOut) {
								FailureMessage = $"Execution timed out after {timeout.TotalSeconds} seconds.";
								log.WriteLine (FailureMessage);
								ExecutionResult = TestExecutingResult.TimedOut;
							} else if (result.Succeeded) {
								ExecutionResult = TestExecutingResult.Succeeded;
							} else {
								ExecutionResult = TestExecutingResult.Failed;
								FailureMessage = result.ExitCode != 1 ? $"Test run crashed (exit code: {result.ExitCode})." : "Test run failed.";
								log.WriteLine (FailureMessage);
							}
						} finally {
							await snapshot.EndCaptureAsync (TimeSpan.FromSeconds (Succeeded ? 0 : 5));
						}
					}
					Jenkins.MainLog.WriteLine ("Executed {0} ({1})", TestName, Mode);

					Logs.AddFile (System.IO.Path.Combine (results, "index.html"), "HTML Report");
				}
			}
		}
	}
}
