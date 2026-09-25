using System;
using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

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

		public ISimulatorLoader CreateLoader () => new SimulatorLoader (processManager, new SimulatorSelector (processManager.XcodeRoot));
	}

	public class SimulatorSelector : DefaultSimulatorSelector {
		readonly string xcodeBuildVersion;

		public SimulatorSelector (string xcodeRoot)
		{
			ArgumentNullException.ThrowIfNull (xcodeRoot);

			var versionPlistPath = Path.Combine (xcodeRoot, "Contents", "version.plist");
			var versionPlist = new XmlDocument ();
			versionPlist.LoadWithoutNetworkAccess (versionPlistPath);
			xcodeBuildVersion = versionPlist.SelectSingleNode ("/plist/dict/key[. = 'ProductBuildVersion']/following-sibling::string[1]")?.InnerText
				?? throw new InvalidOperationException ($"Could not find ProductBuildVersion in '{versionPlistPath}'.");
		}

		public override string GetDeviceType (TestTargetOs target, bool minVersion)
		{
			return target.Platform switch {
				TestTarget.Simulator_iOS64 => GetiOSDeviceType (Version.Parse (target.OSVersion!)),
				TestTarget.Simulator_tvOS => GettvOSDeviceType (Version.Parse (target.OSVersion!)),
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

			// Xcode 27.1 beta 1's iOS 27.1 runtime only supports iPhone Duo.
			if (iOSVersion.Major == 27 && iOSVersion.Minor == 1 && xcodeBuildVersion == "27A9269")
				return "com.apple.CoreSimulator.SimDeviceType.iPhone-Duo";

			return "com.apple.CoreSimulator.SimDeviceType.iPhone-14";
		}

		string GettvOSDeviceType (Version tvOSVersion)
		{
			// tvOS 27 (Xcode 27) removed the legacy non-4K "Apple TV" (Apple-TV-1080p) simulator device
			// type; those runtimes only support the Apple TV 4K device types. Keep using the legacy device
			// type on older tvOS versions (which is everything Xcode < 27 ships) so behavior is unchanged there.
			if (tvOSVersion.Major >= 27)
				return "com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-3rd-generation-4K";

			return "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p";
		}

		public override void GetCompanionRuntimeAndDeviceType (TestTargetOs target, bool minVersion, out string? companionRuntime, out string? companionDeviceType)
		{
			companionRuntime = null;
			companionDeviceType = null;
		}
	}
}
