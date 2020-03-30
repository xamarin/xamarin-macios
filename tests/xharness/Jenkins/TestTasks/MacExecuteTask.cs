using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	class MacExecuteTask : MacTask
	{
		protected ICrashSnapshotReporterFactory CrashReportSnapshotFactory { get; }

		public string Path;
		public bool BCLTest;
		public bool IsUnitTest;

		public MacExecuteTask (BuildToolTask build_task, IProcessManager processManager, ICrashSnapshotReporterFactory crashReportSnapshotFactory)
			: base (build_task, processManager)
		{
			this.CrashReportSnapshotFactory = crashReportSnapshotFactory ?? throw new ArgumentNullException (nameof (crashReportSnapshotFactory));
		}

		public override bool SupportsParallelExecution {
			get {
				if (TestName.Contains ("xammac")) {
					// We run the xammac tests in both Debug and Release configurations.
					// These tests are not written to support parallel execution
					// (there are hard coded paths used for instance), so disable
					// parallel execution for these tests.
					return false;
				}
				if (BCLTest) {
					// We run the BCL tests in multiple flavors (Full/Modern),
					// and the BCL tests are not written to support parallel execution,
					// so disable parallel execution for these tests.
					return false;
				}

				return base.SupportsParallelExecution;
			}
		}

		public override IEnumerable<ILog> AggregatedLogs {
			get {
				return base.AggregatedLogs.Union (BuildTask.Logs);
			}
		}

		protected override async Task RunTestAsync ()
		{
			var projectDir = System.IO.Path.GetDirectoryName (ProjectFile);
			var name = System.IO.Path.GetFileName (projectDir);
			if (string.Equals ("mac", name, StringComparison.OrdinalIgnoreCase))
				name = System.IO.Path.GetFileName (System.IO.Path.GetDirectoryName (projectDir));
			var suffix = string.Empty;
			switch (Platform) {
			case TestPlatform.Mac_Modern:
				suffix = "-modern";
				break;
			case TestPlatform.Mac_Full:
				suffix = "-full";
				break;
			case TestPlatform.Mac_System:
				suffix = "-system";
				break;
			}
			if (ProjectFile.EndsWith (".sln", StringComparison.Ordinal)) {
				Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (ProjectFile), "bin", BuildTask.ProjectPlatform, BuildTask.ProjectConfiguration + suffix, name + ".app", "Contents", "MacOS", name);
			} else {
				var project = new XmlDocument ();
				project.LoadWithoutNetworkAccess (ProjectFile);
				var outputPath = project.GetOutputPath (BuildTask.ProjectPlatform, BuildTask.ProjectConfiguration).Replace ('\\', '/');
				var assemblyName = project.GetAssemblyName ();
				Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (ProjectFile), outputPath, assemblyName + ".app", "Contents", "MacOS", assemblyName);
			}

			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				using (var proc = new Process ()) {
					proc.StartInfo.FileName = Path;
					if (IsUnitTest) {
						var xml = Logs.CreateFile ($"test-{Platform}-{Timestamp}.xml", LogType.NUnitResult.ToString ());
						proc.StartInfo.Arguments = StringUtils.FormatArguments ($"-result=" + xml);
					}
					if (!Harness.GetIncludeSystemPermissionTests (Platform, false))
						proc.StartInfo.EnvironmentVariables ["DISABLE_SYSTEM_PERMISSION_TESTS"] = "1";
					proc.StartInfo.EnvironmentVariables ["MONO_DEBUG"] = "no-gdb-backtrace";
					Jenkins.MainLog.WriteLine ("Executing {0} ({1})", TestName, Mode);
					var log = Logs.Create ($"execute-{Platform}-{Timestamp}.txt", LogType.ExecutionLog.ToString ());
					if (!Harness.DryRun) {
						ExecutionResult = TestExecutingResult.Running;

						var snapshot = CrashReportSnapshotFactory.Create (log, Logs, isDevice: false, deviceName: null);
						await snapshot.StartCaptureAsync ();

						ProcessExecutionResult result = null;
						try {
							var timeout = TimeSpan.FromMinutes (20);

							result = await ProcessManager.RunAsync (proc, log, timeout);
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
							await snapshot.EndCaptureAsync (TimeSpan.FromSeconds (Succeeded ? 0 : result?.ExitCode > 1 ? 120 : 5));
						}
					}
					Jenkins.MainLog.WriteLine ("Executed {0} ({1})", TestName, Mode);
				}
			}
		}
	}
}
