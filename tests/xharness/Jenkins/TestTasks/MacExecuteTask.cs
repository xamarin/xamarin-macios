using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.Common.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	class MacExecuteTask : MacTask {
		protected ICrashSnapshotReporterFactory CrashReportSnapshotFactory { get; }

		public string Path;
		public bool BCLTest;
		public bool IsUnitTest;

		public MacExecuteTask (Jenkins jenkins, BuildToolTask build_task, IMlaunchProcessManager processManager, ICrashSnapshotReporterFactory crashReportSnapshotFactory)
			: base (jenkins, build_task, processManager)
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

		public override async Task RunTestAsync ()
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
				string outputPath;
				if (TestProject?.IsDotNetProject == true) {
					outputPath = await Harness.AppBundleLocator.LocateAppBundle (project, ProjectFile, TestTarget.None, BuildTask.ProjectConfiguration);
				} else {
					outputPath = project.GetOutputPath (BuildTask.ProjectPlatform, BuildTask.ProjectConfiguration).Replace ('\\', '/');
				}
				var assemblyName = project.GetAssemblyName ();
				Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (ProjectFile), outputPath, assemblyName + ".app", "Contents", "MacOS", assemblyName);
			}

			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				using (var proc = new Process ()) {
					proc.StartInfo.FileName = Path;
					var arguments = new List<string> ();
					IFileBackedLog xmlLog = null;
					var useXmlOutput = Harness.InCI || true;
					if (IsUnitTest) {
						var extension = useXmlOutput ? "xml" : "log";
						var type = useXmlOutput ? LogType.XmlLog : LogType.NUnitResult;
						xmlLog = Logs.Create ($"test-{Platform}-{Timestamp}.{extension}", type.ToString ());
						arguments.Add ($"-transport:FILE");
						proc.StartInfo.EnvironmentVariables ["NUNIT_TRANSPORT"] = "FILE";
						arguments.Add ($"--logfile:{xmlLog.FullPath}");
						proc.StartInfo.EnvironmentVariables ["NUNIT_LOG_FILE"] = xmlLog.FullPath;
						if (useXmlOutput) {
							arguments.Add ("--enablexml");
							proc.StartInfo.EnvironmentVariables ["NUNIT_ENABLE_XML_OUTPUT"] = "true";
							arguments.Add ("--xmlmode=wrapped");
							proc.StartInfo.EnvironmentVariables ["NUNIT_ENABLE_XML_MODE"] = "wrapped";
							arguments.Add ("--xmlversion=nunitv3");
							proc.StartInfo.EnvironmentVariables ["NUNIT_XML_VERSION"] = "nunitv3";
						}
						arguments.Add ("--autostart");
						proc.StartInfo.EnvironmentVariables ["NUNIT_AUTOSTART"] = "true";
						arguments.Add ("--autoexit");
						proc.StartInfo.EnvironmentVariables ["NUNIT_AUTOEXIT"] = "true";
					}
					if (!Harness.GetIncludeSystemPermissionTests (Platform, false))
						proc.StartInfo.EnvironmentVariables ["DISABLE_SYSTEM_PERMISSION_TESTS"] = "1";
					proc.StartInfo.EnvironmentVariables ["MONO_DEBUG"] = "no-gdb-backtrace";
					proc.StartInfo.EnvironmentVariables.Remove ("DYLD_FALLBACK_LIBRARY_PATH"); // VSMac might set this, and the test may end up crashing
					proc.StartInfo.Arguments = StringUtils.FormatArguments (arguments);
					Jenkins.MainLog.WriteLine ("Executing {0} ({1})", TestName, Mode);
					var log = Logs.Create ($"execute-{Platform}-{Timestamp}.txt", LogType.ExecutionLog.ToString ());
					ICrashSnapshotReporter snapshot = null;
					if (!Jenkins.Harness.DryRun) {
						ExecutionResult = TestExecutingResult.Running;

						snapshot = CrashReportSnapshotFactory.Create (log, Logs, isDevice: false, deviceName: null);
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

					if (IsUnitTest) {
						var reporterFactory = new TestReporterFactory (ProcessManager);
						var listener = new Microsoft.DotNet.XHarness.iOS.Shared.Listeners.SimpleFileListener (xmlLog.FullPath, log, xmlLog, useXmlOutput);
						var reporter = reporterFactory.Create (Harness.HarnessLog, log, Logs, snapshot, listener, Harness.ResultParser, new AppBundleInformation ("N/A", "N/A", "N/A", "N/A", true, null), RunMode.MacOS, Harness.XmlJargon, "no device here", TimeSpan.Zero);
						var rv = await reporter.ParseResult ();

						if (ExecutionResult == TestExecutingResult.Succeeded) {
							// The process might have crashed, timed out at exit, or otherwise returned a non-zero exit code when all the unit tests passed, in which we shouldn't override the execution result here,
							ExecutionResult = rv.ExecutingResult;
						}

						// Set or replace the failure message, depending on whether there already is a failure message or not.
						if (string.IsNullOrEmpty (FailureMessage))
							FailureMessage = rv.ResultMessage;
						else if (!string.IsNullOrEmpty (rv.ResultMessage))
							FailureMessage += "\n" + rv.ResultMessage;
					}
				}
			}
		}
	}
}
