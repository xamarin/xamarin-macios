using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness.Jenkins.TestTasks {
	// This class groups simulator run tasks according to the
	// simulator they'll run from, so that we minimize switching
	// between different simulators (which is slow).
	class AggregatedRunSimulatorTask : AppleTestTask
	{
		public IEnumerable<RunSimulatorTask> Tasks;

		// Due to parallelization this isn't the same as the sum of the duration for all the build tasks.
		Stopwatch build_timer = new Stopwatch ();
		public TimeSpan BuildDuration { get { return build_timer.Elapsed; } }

		Stopwatch run_timer = new Stopwatch ();
		public TimeSpan RunDuration { get { return run_timer.Elapsed; } }

		public AggregatedRunSimulatorTask (Jenkins jenkins, IEnumerable<RunSimulatorTask> tasks) : base (jenkins)
		{
			this.Tasks = tasks;
		}

		protected override void PropagateResults ()
		{
			foreach (var task in Tasks) {
				task.ExecutionResult = ExecutionResult;
				task.FailureMessage = FailureMessage;
			}
		}

		protected override async Task ExecuteAsync ()
		{
			if (Tasks.All ((v) => v.Ignored)) {
				ExecutionResult = TestExecutingResult.Ignored;
				return;
			}

			// First build everything. This is required for the run simulator
			// task to properly configure the simulator.
			build_timer.Start ();
			await Task.WhenAll (Tasks.Select ((v) => v.BuildAsync ()).Distinct ());
			build_timer.Stop ();

			var executingTasks = Tasks.Where ((v) => !v.Ignored && !v.Failed);
			if (!executingTasks.Any ()) {
				ExecutionResult = TestExecutingResult.Failed;
				return;
			}

			using (var desktop = await NotifyBlockingWaitAsync (ResourceManager.DesktopResource.AcquireExclusiveAsync ())) {
				run_timer.Start ();

				// We need to set the dialog permissions for all the apps
				// before launching the simulator, because once launched
				// the simulator caches the values in-memory.
				foreach (var task in executingTasks) {
					await task.VerifyRunAsync ();
					await task.SelectSimulatorAsync ();
				}

				var devices = executingTasks.FirstOrDefault ()?.Simulators; 
				if (devices == null) { 
					ExecutionResult = TestExecutingResult.DeviceNotFound;
					return;
				}
				Jenkins.MainLog.WriteLine ("Selected simulator: {0}", devices.Length > 0 ? devices [0].Name : "none");

				foreach (var dev in devices)
					await dev.PrepareSimulator (Jenkins.MainLog, executingTasks.Select ((v) => v.BundleIdentifier).ToArray ());

				foreach (var task in executingTasks) {
					task.AcquiredResource = desktop;
					try {
						await task.RunAsync ();
					} finally {
						task.AcquiredResource = null;
					}
				}

				foreach (var dev in devices)
					await dev.Shutdown (Jenkins.MainLog);

				var device = devices.FirstOrDefault ();
				if (device != null)
					await device.KillEverything (Jenkins.MainLog);

				run_timer.Stop ();
			}

			if (Tasks.All ((v) => v.Ignored)) {
				ExecutionResult = TestExecutingResult.Ignored;
			} else {
				ExecutionResult = Tasks.Any ((v) => v.Failed) ? TestExecutingResult.Failed : TestExecutingResult.Succeeded;
			}
		}
	}
}
