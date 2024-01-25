using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.iOS.Shared;

#nullable enable

namespace Xharness.Jenkins.TestTasks {
	// This class groups simulator run tasks according to the
	// simulator they'll run from, so that we minimize switching
	// between different simulators (which is slow).
	class AggregatedRunSimulatorTask : AppleTestTask {
		public IEnumerable<RunSimulatorTask> Tasks;

		// Due to parallelization this isn't the same as the sum of the duration for all the build tasks.
		readonly Stopwatch buildTimer = new ();
		public TimeSpan BuildDuration { get { return buildTimer.Elapsed; } }

		readonly Stopwatch runTimer = new ();
		public TimeSpan RunDuration { get { return runTimer.Elapsed; } }

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
			buildTimer.Start ();
			await Task.WhenAll (Tasks.Select ((v) => v.BuildAsync ()).Distinct ());
			buildTimer.Stop ();

			var executingTasks = Tasks.Where ((v) => !v.Ignored && !v.Failed);
			if (!executingTasks.Any ()) {
				ExecutionResult = TestExecutingResult.Failed;
				return;
			}

			using (var desktop = await NotifyBlockingWaitAsync (ResourceManager.DesktopResource.AcquireExclusiveAsync ())) {
				runTimer.Start ();

				// We need to set the dialog permissions for all the apps
				// before launching the simulator, because once launched
				// the simulator caches the values in-memory.
				foreach (var task in executingTasks) {
					await task.VerifyRunAsync ();
					await task.SelectSimulatorAsync ();
				}

				var devices = executingTasks.FirstOrDefault ()?.Simulators;
				if (devices is null || !devices.Any ()) {
					ExecutionResult = TestExecutingResult.DeviceNotFound;
					return;
				}
				Jenkins.MainLog.WriteLine ("Selected simulator: {0}", devices.Any () ? devices.First ().Name : "none");

				foreach (var dev in devices) {
					using var tcclog = Logs.Create ($"prepare-simulator-{Xharness.Harness.Helpers.Timestamp}.log", "Simulator preparation");
					var rv = await dev.PrepareSimulator (tcclog, executingTasks.Select ((v) => v.BundleIdentifier).ToArray ());
					tcclog.Description += rv ? " ✅ " : " (failed) ⚠️";
					foreach (var task in executingTasks.Where ((v) => v.Simulators.Contains (dev)))
						task.Logs.Add (tcclog);
				}

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
				if (device is not null)
					await device.KillEverything (Jenkins.MainLog);

				runTimer.Stop ();
			}

			if (Tasks.All ((v) => v.Ignored)) {
				ExecutionResult = TestExecutingResult.Ignored;
			} else {
				ExecutionResult = Tasks.Any ((v) => v.Failed) ? TestExecutingResult.Failed : TestExecutingResult.Succeeded;
			}
		}
	}
}
