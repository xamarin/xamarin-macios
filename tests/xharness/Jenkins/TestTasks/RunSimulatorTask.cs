using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness.Jenkins.TestTasks {
	class RunSimulatorTask : RunXITask<ISimulatorDevice>
	{
		readonly ISimulatorsLoader simulators;
		public IAcquiredResource AcquiredResource;

		public ISimulatorDevice [] Simulators {
			get {
				if (Device == null) {
					return new ISimulatorDevice [] { };
				} else if (CompanionDevice == null) {
					return new ISimulatorDevice [] { Device };
				} else {
					return new ISimulatorDevice [] { Device, CompanionDevice };
				}
			}
		}

		public RunSimulatorTask (ISimulatorsLoader simulators, MSBuildTask build_task, IProcessManager ProcessManager, IEnumerable<ISimulatorDevice> candidates = null)
			: base (build_task, ProcessManager, candidates)
		{
			var project = Path.GetFileNameWithoutExtension (ProjectFile);
			if (project.EndsWith ("-tvos", StringComparison.Ordinal)) {
				AppRunnerTarget = TestTarget.Simulator_tvOS;
			} else if (project.EndsWith ("-watchos", StringComparison.Ordinal)) {
				AppRunnerTarget = TestTarget.Simulator_watchOS;
			} else {
				AppRunnerTarget = TestTarget.Simulator_iOS;
			}

			this.simulators = simulators ?? throw new ArgumentNullException (nameof (simulators));
		}

		public async Task FindSimulatorAsync ()
		{
			if (Device != null)
				return;

			var asyncEnumerable = Candidates as IAsyncEnumerable;
			if (asyncEnumerable != null)
				await asyncEnumerable.ReadyTask;

			if (!Candidates.Any ()) {
				ExecutionResult = TestExecutingResult.DeviceNotFound;
				FailureMessage = "No applicable devices found.";
			} else {
				Device = Candidates.First ();
				if (Platform == TestPlatform.watchOS)
					CompanionDevice = simulators.FindCompanionDevice (Jenkins.SimulatorLoadLog, Device);
			}

		}

		public async Task SelectSimulatorAsync ()
		{
			if (Finished)
				return;

			if (!BuildTask.Succeeded) {
				ExecutionResult = TestExecutingResult.BuildFailure;
				return;
			}

			await FindSimulatorAsync ();

			var clean_state = false;//Platform == TestPlatform.watchOS;
			runner = new AppRunner (ProcessManager,
				new AppBundleInformationParser (),
				new SimulatorsLoaderFactory (ProcessManager),
				new SimpleListenerFactory (),
				new DeviceLoaderFactory (ProcessManager),
				new CrashSnapshotReporterFactory (ProcessManager),
				new CaptureLogFactory (),
				new DeviceLogCapturerFactory (ProcessManager),
				new TestReporterFactory (ProcessManager),
				AppRunnerTarget,
				Harness,
				mainLog: Logs.Create ($"run-{Device.UDID}-{Timestamp}.log", "Run log"),
				logs: Logs,
				projectFilePath: ProjectFile,				
				ensureCleanSimulatorState: clean_state,
				buildConfiguration: ProjectConfiguration,
				timeoutMultiplier: TimeoutMultiplier,
				variation: Variation,
				buildTask: BuildTask,
				simulators: Simulators);
		}

		Task<IAcquiredResource> AcquireResourceAsync ()
		{
			if (AcquiredResource != null) {
				// We don't own the acquired resource, so wrap it in a class that won't dispose it.
				return Task.FromResult<IAcquiredResource> (new NondisposedResource () { Wrapped = AcquiredResource });
			} else {
				return Jenkins.DesktopResource.AcquireExclusiveAsync ();
			}
		}

		protected override async Task RunTestAsync ()
		{
			Jenkins.MainLog.WriteLine ("Running XI on '{0}' ({2}) for {1}", Device?.Name, ProjectFile, Device?.UDID);

			ExecutionResult = ExecutionResult & ~TestExecutingResult.InProgressMask | TestExecutingResult.Running;
			await BuildTask.RunAsync ();
			if (!BuildTask.Succeeded) {
				ExecutionResult = TestExecutingResult.BuildFailure;
				return;
			}
			using (var resource = await NotifyBlockingWaitAsync (AcquireResourceAsync ())) {
				if (runner == null)
					await SelectSimulatorAsync ();
				await runner.RunAsync ();
			}
			ExecutionResult = runner.Result;

			KnownFailure = null;
			if (Jenkins.IsHE0038Error (runner.MainLog))
				KnownFailure = $"<a href='https://github.com/xamarin/maccore/issues/581'>HE0038</a>";
		}

		protected override string XIMode {
			get {
				return "simulator";
			}
		}

		class NondisposedResource : IAcquiredResource
		{
			public IAcquiredResource Wrapped;

			public Resource Resource {
				get {
					return Wrapped.Resource;
				}
			}

			public void Dispose ()
			{
				// Nope, no disposing here.
			}
		}
	}
}
