namespace Microsoft.DotNet.XHarness.iOS.Shared {
	/// <summary>
	/// This file is auto-generated from xamarin-macios/Make.config:
	/// https://github.com/xamarin/xamarin-macios/blob/54194e71f17dcbba76e6ca6b079f347f13f36dcf/Make.config
	/// It is generated on build time and thus tied to the MacOS where it's being built.
	/// 
	/// TODO: We will need to start syncing this file or make it dynamic based on the host.
	/// </summary>
	public static class SdkVersions {
		public static string Xcode { get; private set; } = "11.3";
		public static string OSX { get; private set; } = "10.15";
		public static string iOS { get; private set; } = "13.2";
		public static string WatchOS { get; private set; } = "6.1";
		public static string TVOS { get; private set; } = "13.2";

		public static string MinOSX { get; private set; } = "10.9";
		public static string MiniOS { get; private set; } = "7.0";
		public static string MinWatchOS { get; private set; } = "2.0";
		public static string MinTVOS { get; private set; } = "9.0";

		public static string MiniOSSimulator { get; private set; } = "10.3";
		public static string MinWatchOSSimulator { get; private set; } = "3.2";
		public static string MinWatchOSCompanionSimulator { get; private set; } = "10.3";
		public static string MinTVOSSimulator { get; private set; } = "10.2";

		public static string MaxiOSSimulator { get; private set; } = "13.3";
		public static string MaxWatchOSSimulator { get; private set; } = "6.1";
		public static string MaxWatchOSCompanionSimulator { get; private set; } = "13.3";

		public static string MaxTVOSSimulator { get; private set; } = "13.3";
		public static string MaxiOSDeploymentTarget { get; private set; } = "13.3";
		public static string MaxWatchDeploymentTarget { get; private set; } = "6.1";
		public static string MaxTVOSDeploymentTarget { get; private set; } = "13.3";

		public static void OverrideVersions (string xcode,
			string oSX,
			string iOS,
			string watchOS,
			string tVOS,
			string minOSX,
			string miniOS,
			string minWatchOS,
			string minTVOS,
			string miniOSSimulator,
			string minWatchOSSimulator,
			string minWatchOSCompanionSimulator,
			string minTVOSSimulator,
			string maxiOSSimulator,
			string maxWatchOSSimulator,
			string maxWatchOSCompanionSimulator,
			string maxTVOSSimulator,
			string maxiOSDeploymentTarget,
			string maxWatchDeploymentTarget,
			string maxTVOSDeploymentTarget)
		{
			Xcode = xcode;
			OSX = oSX;
			SdkVersions.iOS = iOS;
			WatchOS = watchOS;
			TVOS = tVOS;
			MinOSX = minOSX;
			MiniOS = miniOS;
			MinWatchOS = minWatchOS;
			MinTVOS = minTVOS;
			MiniOSSimulator = miniOSSimulator;
			MinWatchOSSimulator = minWatchOSSimulator;
			MinWatchOSCompanionSimulator = minWatchOSCompanionSimulator;
			MinTVOSSimulator = minTVOSSimulator;
			MaxiOSSimulator = maxiOSSimulator;
			MaxWatchOSSimulator = maxWatchOSSimulator;
			MaxWatchOSCompanionSimulator = maxWatchOSCompanionSimulator;
			MaxTVOSSimulator = maxTVOSSimulator;
			MaxiOSDeploymentTarget = maxiOSDeploymentTarget;
			MaxWatchDeploymentTarget = maxWatchDeploymentTarget;
			MaxTVOSDeploymentTarget = maxTVOSDeploymentTarget;
		}
	}
}
