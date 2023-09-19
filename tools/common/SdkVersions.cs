using System;

#if MTOUCH || MMP || BUNDLER
using Xamarin.Bundler;
#endif

using Xamarin.Utils;

#if MTOUCH
using MonoTouch;
#endif

#nullable enable

namespace Xamarin {
	static class SdkVersions {
		public const string Xcode = "14.3.1";
		public const string OSX = "13.3";
		public const string iOS = "16.4";
		public const string WatchOS = "9.4";
		public const string TVOS = "16.4";
		public const string MacCatalyst = "16.4";

#if NET
		public const string MinOSX = "10.15";
		public const string MiniOS = "11.0";
		public const string MinWatchOS = "99.99"; // TODO not supported, many changes required to remove it
		public const string MinTVOS = "11.0";
		public const string MinMacCatalyst = "13.1";
#else
		public const string MinOSX = "10.15";
		public const string MiniOS = "11.0";
		public const string MinWatchOS = "4.0";
		public const string MinTVOS = "11.0";
		public const string MinMacCatalyst = "13.1";
#endif

		public const string MiniOSSimulator = "13.7";
		public const string MinWatchOSSimulator = "7.0";
		public const string MinWatchOSCompanionSimulator = "14.5";
		public const string MinTVOSSimulator = "13.4";

		public const string MaxiOSSimulator = "16.4";
		public const string MaxWatchOSSimulator = "9.4";
		public const string MaxWatchOSCompanionSimulator = "16.4";
		public const string MaxTVOSSimulator = "16.4";

		public const string MaxiOSDeploymentTarget = "16.4";
		public const string MaxWatchDeploymentTarget = "9.4";
		public const string MaxTVOSDeploymentTarget = "16.4";

		public const string DefaultTargetPlatformVersioniOS = "16.1";
		public const string DefaultTargetPlatformVersiontvOS = "16.1";
		public const string DefaultTargetPlatformVersionmacOS = "13.0";
		public const string DefaultTargetPlatformVersionMacCatalyst = "16.1";

		public static Version OSXVersion { get { return new Version (OSX); } }
		public static Version iOSVersion { get { return new Version (iOS); } }
		public static Version WatchOSVersion { get { return new Version (WatchOS); } }
		public static Version TVOSVersion { get { return new Version (TVOS); } }
		public static Version MacCatalystVersion { get { return new Version (MacCatalyst); } }

		public static Version iOSTargetVersion { get { return new Version (MaxiOSDeploymentTarget); } }
		public static Version WatchOSTargetVersion { get { return new Version (MaxWatchDeploymentTarget); } }
		public static Version TVOSTargetVersion { get { return new Version (MaxTVOSDeploymentTarget); } }

		public static Version MinOSXVersion { get { return new Version (MinOSX); } }
		public static Version MiniOSVersion { get { return new Version (MiniOS); } }
		public static Version MinWatchOSVersion { get { return new Version (MinWatchOS); } }
		public static Version MinTVOSVersion { get { return new Version (MinTVOS); } }
		public static Version MinMacCatalystVersion { get { return new Version (MinMacCatalyst); } }

		public static Version MiniOSSimulatorVersion { get { return new Version (MiniOSSimulator); } }
		public static Version MinWatchOSSimulatorVersion { get { return new Version (MinWatchOSSimulator); } }
		public static Version MinWatchOSCompanionSimulatorVersion { get { return new Version (MinWatchOSCompanionSimulator); } }
		public static Version MinTVOSSimulatorVersion { get { return new Version (MinTVOSSimulator); } }

		public static Version MaxiOSSimulatorVersion { get { return new Version (MaxiOSSimulator); } }
		public static Version MaxWatchOSSimulatorVersion { get { return new Version (MaxWatchOSSimulator); } }
		public static Version MaxWatchOSCompanionSimulatorVersion { get { return new Version (MaxWatchOSCompanionSimulator); } }
		public static Version MaxTVOSSimulatorVersion { get { return new Version (MaxTVOSSimulator); } }

		public static Version XcodeVersion { get { return new Version (Xcode); } }

#if MTOUCH || MMP || BUNDLER
		public static Version GetVersion (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSVersion;
			case ApplePlatform.WatchOS: return WatchOSVersion;
			case ApplePlatform.TVOS: return TVOSVersion;
			case ApplePlatform.MacCatalyst: return MacCatalystVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", app.Platform, app.ProductName);
			}
		}

		public static Version GetTargetVersion (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSTargetVersion;
			case ApplePlatform.WatchOS: return WatchOSTargetVersion;
			case ApplePlatform.TVOS: return TVOSTargetVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", app.Platform, app.ProductName);
			}
		}

		public static Version GetMinVersion (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.MacOSX: return MinOSXVersion;
			case ApplePlatform.iOS: return MiniOSVersion;
			case ApplePlatform.WatchOS: return MinWatchOSVersion;
			case ApplePlatform.TVOS: return MinTVOSVersion;
			case ApplePlatform.MacCatalyst: return MinMacCatalystVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", app.Platform, app.ProductName);
			}
		}
#endif

		public static Version GetVersion (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSVersion;
			case ApplePlatform.WatchOS: return WatchOSVersion;
			case ApplePlatform.TVOS: return TVOSVersion;
			case ApplePlatform.MacCatalyst: return MacCatalystVersion;
			default:
				throw new ArgumentOutOfRangeException (nameof (platform), platform, $"Unknown platform: {platform}");
			}
		}

		public static Version GetMinVersion (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX: return MinOSXVersion;
			case ApplePlatform.iOS: return MiniOSVersion;
			case ApplePlatform.WatchOS: return MinWatchOSVersion;
			case ApplePlatform.TVOS: return MinTVOSVersion;
			case ApplePlatform.MacCatalyst: return MinMacCatalystVersion;
			default:
				throw new ArgumentOutOfRangeException (nameof (platform), platform, $"Unknown platform: {platform}");
			}
		}
	}

#if MMP
	static class MonoVersions {
		public static string MinimumMono = "6.4.0.94";
		public static Version MinimumMonoVersion { get { return new Version (MinimumMono); } }
	}
#endif
}
