using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xharness.Collections;
using Xharness.Hardware;

namespace Xharness.Jenkins.TestTasks
{
	class RunSimulatorTask : RunXITask<ISimulatorDevice>
	{
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

		public RunSimulatorTask (MSBuildTask build_task, IEnumerable<ISimulatorDevice> candidates = null)
			: base (build_task, candidates)
		{
			var project = Path.GetFileNameWithoutExtension (ProjectFile);
			if (project.EndsWith ("-tvos", StringComparison.Ordinal)) {
				AppRunnerTarget = AppRunnerTarget.Simulator_tvOS;
			} else if (project.EndsWith ("-watchos", StringComparison.Ordinal)) {
				AppRunnerTarget = AppRunnerTarget.Simulator_watchOS;
			} else {
				AppRunnerTarget = AppRunnerTarget.Simulator_iOS;
			}
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
					CompanionDevice = Jenkins.Simulators.FindCompanionDevice (Jenkins.SimulatorLoadLog, Device);
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
			runner = new AppRunner () {
				Harness = Harness,
				ProjectFile = ProjectFile,
				EnsureCleanSimulatorState = clean_state,
				Target = AppRunnerTarget,
				LogDirectory = LogDirectory,
				MainLog = Logs.Create ($"run-{Device.UDID}-{Timestamp}.log", "Run log"),
				Configuration = ProjectConfiguration,
				TimeoutMultiplier = TimeoutMultiplier,
				Variation = Variation,
				BuildTask = BuildTask,
			};
			runner.Simulators = Simulators;
			runner.Initialize ();
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
