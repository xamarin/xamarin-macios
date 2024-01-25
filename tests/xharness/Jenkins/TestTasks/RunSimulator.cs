using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	public class RunSimulator {

		readonly ILog mainLog;
		readonly ILog simulatorLoadLog;
		readonly ISimulatorLoader simulators;
		readonly IRunSimulatorTask testTask;
		readonly IErrorKnowledgeBase errorKnowledgeBase;

		public IEnumerable<ISimulatorDevice> Simulators {
			get {
				if (testTask.Device is null) return Array.Empty<ISimulatorDevice> ();
				else if (testTask.CompanionDevice is null) {
					return new ISimulatorDevice [] { testTask.Device };
				} else {
					return new ISimulatorDevice [] { testTask.Device, testTask.CompanionDevice };
				}
			}
		}

		public RunSimulator (IRunSimulatorTask testTask,
							 ISimulatorLoader simulators,
							 IErrorKnowledgeBase errorKnowledgeBase,
							 ILog mainLog,
							 ILog simulatorLoadLog)
		{
			this.testTask = testTask ?? throw new ArgumentNullException (nameof (testTask));
			this.simulators = simulators ?? throw new ArgumentNullException (nameof (simulators));
			this.errorKnowledgeBase = errorKnowledgeBase ?? throw new ArgumentNullException (nameof (errorKnowledgeBase));
			this.mainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.simulatorLoadLog = simulatorLoadLog ?? throw new ArgumentNullException (nameof (simulatorLoadLog));

			var project = Path.GetFileNameWithoutExtension (testTask.ProjectFile);
			if (project.EndsWith ("-tvos", StringComparison.Ordinal)) testTask.AppRunnerTarget = TestTarget.Simulator_tvOS;
			else if (project.EndsWith ("-watchos", StringComparison.Ordinal)) {
				testTask.AppRunnerTarget = TestTarget.Simulator_watchOS;
			} else {
				testTask.AppRunnerTarget = TestTarget.Simulator_iOS64;
			}
		}

		public async Task FindSimulatorAsync ()
		{
			if (testTask.Device is not null)
				return;

			var asyncEnumerable = testTask.Candidates as IAsyncEnumerable;
			if (asyncEnumerable is not null)
				await asyncEnumerable.ReadyTask;

			try {
				if (!testTask.Candidates.Any ()) {
					testTask.ExecutionResult = TestExecutingResult.DeviceNotFound;
					testTask.FailureMessage = "No applicable devices found.";
				} else {
					testTask.Device = testTask.Candidates.First ();
					if (testTask.Platform == TestPlatform.watchOS)
						testTask.CompanionDevice = await simulators.FindCompanionDevice (simulatorLoadLog, testTask.Device);
				}
			} catch (Exception e) {
				testTask.ExecutionResult = TestExecutingResult.DeviceNotFound;
				testTask.FailureMessage = $"No applicable devices found ({e.Message})";
			}
		}

		public async Task SelectSimulatorAsync ()
		{
			if (testTask.Finished)
				return;

			if (!testTask.BuildTask.Succeeded) {
				testTask.ExecutionResult = TestExecutingResult.BuildFailure;
				return;
			}

			await FindSimulatorAsync ();

			var clean_state = false;//Platform == TestPlatform.watchOS;
			testTask.Runner = new AppRunner (testTask.ProcessManager,
				new AppBundleInformationParser (testTask.ProcessManager, testTask.Harness.AppBundleLocator),
				new SimulatorLoaderFactory (testTask.ProcessManager),
				new SimpleListenerFactory (null), // sims cannot use tunnels
				new DeviceLoaderFactory (testTask.ProcessManager),
				new CrashSnapshotReporterFactory (testTask.ProcessManager),
				new CaptureLogFactory (),
				new DeviceLogCapturerFactory (testTask.ProcessManager),
				new TestReporterFactory (testTask.ProcessManager),
				testTask.AppRunnerTarget,
				testTask.Harness,
				mainLog: testTask.Logs.Create ($"run-{testTask.Device.UDID}-{Harness.Helpers.Timestamp}.log", "Run log"),
				logs: testTask.Logs,
				projectFilePath: testTask.ProjectFile,
				ensureCleanSimulatorState: clean_state,
				buildConfiguration: testTask.ProjectConfiguration,
				timeoutMultiplier: testTask.TimeoutMultiplier,
				variation: testTask.Variation,
				buildTask: testTask.BuildTask,
				simulator: testTask.Device,
				companionSimulator: testTask.CompanionDevice);
			await testTask.Runner.InitializeAsync ();
		}

		public async Task RunTestAsync ()
		{
			mainLog.WriteLine ("Running XI on '{0}' ({2}) for {1}", testTask.Device?.Name, testTask.ProjectFile, testTask.Device?.UDID);

			testTask.ExecutionResult = testTask.ExecutionResult & ~TestExecutingResult.InProgressMask | TestExecutingResult.Running;
			await testTask.BuildTask.RunAsync ();
			if (!testTask.BuildTask.Succeeded) {
				testTask.ExecutionResult = TestExecutingResult.BuildFailure;
				return;
			}
			using (var resource = await testTask.NotifyBlockingWaitAsync (testTask.AcquireResourceAsync ())) {
				if (testTask.Runner is null)
					await SelectSimulatorAsync ();
				await testTask.Runner.RunAsync ();
			}
			testTask.ExecutionResult = testTask.Runner.Result;

			testTask.KnownFailure = null;
			if (errorKnowledgeBase.IsKnownTestIssue (testTask.Runner.MainLog, out var failure)) {
				testTask.KnownFailure = failure;
				mainLog.WriteLine ($"Test run has a known failure: '{testTask.KnownFailure}'");
			}
		}
	}
}
