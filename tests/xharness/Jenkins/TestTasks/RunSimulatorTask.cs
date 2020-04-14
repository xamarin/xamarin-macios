using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Xharness.TestTasks;

namespace Xharness.Jenkins.TestTasks {
	class RunSimulatorTask : RunXITask<ISimulatorDevice>, IRunSimulatorTask
	{
		readonly RunSimulator runSimulator;
		public IAcquiredResource AcquiredResource;


		public ISimulatorDevice [] Simulators => runSimulator.Simulators;

		public RunSimulatorTask (Jenkins jenkins, ISimulatorLoader simulators, MSBuildTask buildTask, IProcessManager processManager, IEnumerable<ISimulatorDevice> candidates = null)
			: base (jenkins, buildTask, processManager, candidates) =>  runSimulator = new RunSimulator (
				testTask: this,
				simulators: simulators,
				errorKnowledgeBase: Jenkins,
				mainLog: Jenkins.MainLog,
				simulatorLoadLog: Jenkins.SimulatorLoadLog);

		public Task FindSimulatorAsync () => runSimulator.FindSimulatorAsync ();

		public Task SelectSimulatorAsync () => runSimulator.SelectSimulatorAsync ();

		public Task<IAcquiredResource> AcquireResourceAsync ()
		{
			if (AcquiredResource != null) {
				// We don't own the acquired resource, so wrap it in a class that won't dispose it.
				return Task.FromResult<IAcquiredResource> (new NondisposedResource () { Wrapped = AcquiredResource });
			} else {
				return Jenkins.DesktopResource.AcquireExclusiveAsync ();
			}
		}

		public override Task RunTestAsync () => runSimulator.RunTestAsync ();

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
