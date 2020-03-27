using System;

namespace Microsoft.DotNet.XHarness.iOS.Shared {
	public static class SdkVersions {
		public const string Xcode = "11.3";
		public const string OSX = "10.15";
		public const string iOS = "13.2";
		public const string WatchOS = "6.1";
		public const string TVOS = "13.2";

		public const string MinOSX = "10.9";
		public const string MiniOS = "7.0";
		public const string MinWatchOS = "2.0";
		public const string MinTVOS = "9.0";

		public const string MiniOSSimulator = "10.3";
		public const string MinWatchOSSimulator = "3.2";
		public const string MinWatchOSCompanionSimulator = "10.3";
		public const string MinTVOSSimulator = "10.2";

		public const string MaxiOSSimulator = "13.3";
		public const string MaxWatchOSSimulator = "6.1";
		public const string MaxWatchOSCompanionSimulator = "13.3";
		public const string MaxTVOSSimulator = "13.3";

		public const string MaxiOSDeploymentTarget = "13.3";
		public const string MaxWatchDeploymentTarget = "6.1";
		public const string MaxTVOSDeploymentTarget = "13.3";

		public static Version OSXVersion => new Version (OSX);
		public static Version iOSVersion => new Version (iOS);
		public static Version WatchOSVersion => new Version (WatchOS);
		public static Version TVOSVersion => new Version (TVOS);

		public static Version iOSTargetVersion => new Version (MaxiOSDeploymentTarget);
		public static Version WatchOSTargetVersion => new Version (MaxWatchDeploymentTarget);
		public static Version TVOSTargetVersion => new Version (MaxTVOSDeploymentTarget);

		public static Version MinOSXVersion => new Version (MinOSX);
		public static Version MiniOSVersion => new Version (MiniOS);
		public static Version MinWatchOSVersion => new Version (MinWatchOS);
		public static Version MinTVOSVersion => new Version (MinTVOS);

		public static Version MiniOSSimulatorVersion => new Version (MiniOSSimulator);
		public static Version MinWatchOSSimulatorVersion => new Version (MinWatchOSSimulator);
		public static Version MinWatchOSCompanionSimulatorVersion => new Version (MinWatchOSCompanionSimulator);
		public static Version MinTVOSSimulatorVersion => new Version (MinTVOSSimulator);

		public static Version MaxiOSSimulatorVersion => new Version (MaxiOSSimulator);
		public static Version MaxWatchOSSimulatorVersion => new Version (MaxWatchOSSimulator);
		public static Version MaxWatchOSCompanionSimulatorVersion => new Version (MaxWatchOSCompanionSimulator);
		public static Version MaxTVOSSimulatorVersion => new Version (MaxTVOSSimulator);

		public static Version XcodeVersion => new Version (Xcode);
	}
}
