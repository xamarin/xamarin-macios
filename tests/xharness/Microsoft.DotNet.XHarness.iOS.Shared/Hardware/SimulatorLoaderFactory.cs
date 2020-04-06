using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {

	public interface ISimulatorLoaderFactory {
		ISimulatorLoader CreateLoader ();
	}

	public class SimulatorLoaderFactory : ISimulatorLoaderFactory {
		readonly IProcessManager processManager;

		public SimulatorLoaderFactory (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public ISimulatorLoader CreateLoader () => new SimulatorLoader (processManager);
	}
}
