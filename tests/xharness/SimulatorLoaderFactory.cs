using System;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

#nullable enable

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
				TestTarget.Simulator_iOS64 => GetiOSDeviceType (Version.Parse (target.OSVersion!)),
				TestTarget.Simulator_tvOS => "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p",
				_ => throw new Exception (string.Format ("Invalid simulator target: {0}", target))
			};
		}

		string GetiOSDeviceType (Version iOSVersion)
		{
			if (iOSVersion.Major < 13)
				return "com.apple.CoreSimulator.SimDeviceType.iPhone-7";
			if (iOSVersion.Major < 14)
				return "com.apple.CoreSimulator.SimDeviceType.iPhone-8";
			if (iOSVersion.Major < 15)
				return "com.apple.CoreSimulator.SimDeviceType.iPhone-X";
			if (iOSVersion.Major < 16)
				return "com.apple.CoreSimulator.SimDeviceType.iPhone-11";

			return "com.apple.CoreSimulator.SimDeviceType.iPhone-14";
		}

		public override void GetCompanionRuntimeAndDeviceType (TestTargetOs target, bool minVersion, out string? companionRuntime, out string? companionDeviceType)
		{
			companionRuntime = null;
			companionDeviceType = null;
		}
	}
}
