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
				TestTarget.Simulator_iOS => "com.apple.CoreSimulator.SimDeviceType.iPhone-5s",
				TestTarget.Simulator_iOS32 => "com.apple.CoreSimulator.SimDeviceType.iPhone-5s",
				TestTarget.Simulator_iOS64 => GetiOSDeviceType (Version.Parse (target.OSVersion!)),
				TestTarget.Simulator_tvOS => "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p",
				TestTarget.Simulator_watchOS => GetWatchOSDeviceType (Version.Parse (target.OSVersion!)),
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

		string GetWatchOSDeviceType (Version watchOSVersion)
		{
			if (watchOSVersion.Major < 7)
				return "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-3-38mm";
			if (watchOSVersion.Major < 8)
				return "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-4-40mm";
			return "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-7-41mm";
		}

		public override void GetCompanionRuntimeAndDeviceType (TestTargetOs target, bool minVersion, out string? companionRuntime, out string? companionDeviceType)
		{
			companionRuntime = null;
			companionDeviceType = null;

			if (target.Platform == TestTarget.Simulator_watchOS) {
				var companionVersion = minVersion ? SdkVersions.MinWatchOSCompanionSimulator : SdkVersions.MaxWatchOSCompanionSimulator;
				companionRuntime = "com.apple.CoreSimulator.SimRuntime.iOS-" + companionVersion.Replace ('.', '-');
				companionDeviceType = GetiOSDeviceType (Version.Parse (companionVersion));
			}
		}
	}
}
