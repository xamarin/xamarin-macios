using System;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness {

	public interface ISimulatorLoaderFactory {
		ISimulatorLoader CreateLoader ();
	}

	public class SimulatorLoaderFactory : ISimulatorLoaderFactory {
		readonly IMlaunchProcessManager processManager;

		public SimulatorLoaderFactory (IMlaunchProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public ISimulatorLoader CreateLoader () => new SimulatorLoader (processManager, new SimulatorSelector ());
	}

	public class SimulatorSelector : DefaultSimulatorSelector {
		public override string GetDeviceType (TestTargetOs target, bool minVersion)
		{
			return target.Platform switch {
				TestTarget.Simulator_iOS => "com.apple.CoreSimulator.SimDeviceType.iPhone-5s",
				TestTarget.Simulator_iOS32 => "com.apple.CoreSimulator.SimDeviceType.iPhone-5s",
				TestTarget.Simulator_iOS64 => "com.apple.CoreSimulator.SimDeviceType." + (minVersion ? "iPhone-6" : "iPhone-X"),
				TestTarget.Simulator_tvOS => "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p",
				TestTarget.Simulator_watchOS => "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-3-38mm",
				_ => throw new Exception (string.Format ("Invalid simulator target: {0}", target))
			};
		}
	}
}
