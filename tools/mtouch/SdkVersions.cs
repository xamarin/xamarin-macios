using System;

#if MTOUCH || MMP
using Xamarin.Bundler;
using Xamarin.Utils;
#endif

#if MTOUCH
using MonoTouch;
#endif

namespace Xamarin {
	static class SdkVersions {
		public const string Xcode = "11.5";
		public const string OSX = "10.15";
		public const string iOS = "13.5";
		public const string WatchOS = "6.2";
		public const string TVOS = "13.4";

		public const string MinOSX = "10.9";
		public const string MiniOS = "7.0";
		public const string MinWatchOS = "2.0";
		public const string MinTVOS = "9.0";

		public const string MiniOSSimulator = "10.3";
		public const string MinWatchOSSimulator = "3.2";
		public const string MinWatchOSCompanionSimulator = "10.3";
		public const string MinTVOSSimulator = "10.2";

		public const string MaxiOSSimulator = "13.5";
		public const string MaxWatchOSSimulator = "6.2";
		public const string MaxWatchOSCompanionSimulator = "13.5";
		public const string MaxTVOSSimulator = "13.4";

		public const string MaxiOSDeploymentTarget = "13.5";
		public const string MaxWatchDeploymentTarget = "6.2";
		public const string MaxTVOSDeploymentTarget = "13.4";

		public static Version OSXVersion { get { return new Version (OSX); }}
		public static Version iOSVersion { get { return new Version (iOS); }}
		public static Version WatchOSVersion { get { return new Version (WatchOS); }}
		public static Version TVOSVersion { get { return new Version (TVOS); }}

		public static Version iOSTargetVersion { get { return new Version (MaxiOSDeploymentTarget); }}
		public static Version WatchOSTargetVersion { get { return new Version (MaxWatchDeploymentTarget); }}
		public static Version TVOSTargetVersion { get { return new Version (MaxTVOSDeploymentTarget); }}

		public static Version MinOSXVersion { get { return new Version (MinOSX); }}
		public static Version MiniOSVersion { get { return new Version (MiniOS); }}
		public static Version MinWatchOSVersion { get { return new Version (MinWatchOS); }}
		public static Version MinTVOSVersion { get { return new Version (MinTVOS); }}

		public static Version MiniOSSimulatorVersion { get { return new Version (MiniOSSimulator); }}
		public static Version MinWatchOSSimulatorVersion { get { return new Version (MinWatchOSSimulator); }}
		public static Version MinWatchOSCompanionSimulatorVersion { get { return new Version (MinWatchOSCompanionSimulator); }}
		public static Version MinTVOSSimulatorVersion { get { return new Version (MinTVOSSimulator); }}

		public static Version MaxiOSSimulatorVersion { get { return new Version (MaxiOSSimulator); }}
		public static Version MaxWatchOSSimulatorVersion { get { return new Version (MaxWatchOSSimulator); }}
		public static Version MaxWatchOSCompanionSimulatorVersion { get { return new Version (MaxWatchOSCompanionSimulator); }}
		public static Version MaxTVOSSimulatorVersion { get { return new Version (MaxTVOSSimulator); }}

		public static Version XcodeVersion { get { return new Version (Xcode); }}

#if MTOUCH || MMP
		public static Version GetVersion (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSVersion;
			case ApplePlatform.WatchOS: return WatchOSVersion;
			case ApplePlatform.TVOS: return TVOSVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", platform, Application.ProductName);
			}
		}

		public static Version GetTargetVersion (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSTargetVersion;
			case ApplePlatform.WatchOS: return WatchOSTargetVersion;
			case ApplePlatform.TVOS: return TVOSTargetVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", platform, Application.ProductName);
			}
		}

		public static Version GetMinVersion (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX: return MinOSXVersion;
			case ApplePlatform.iOS: return MiniOSVersion;
			case ApplePlatform.WatchOS: return MinWatchOSVersion;
			case ApplePlatform.TVOS: return MinTVOSVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", platform, Application.ProductName);
			}
		}
#endif

	}

#if MMP
	static class MonoVersions {
		public static string MinimumMono = "6.4.0.94";
		public static Version MinimumMonoVersion { get { return new Version (MinimumMono); }}
	}
#endif
}
