using System;

#if MTOUCH || MMP || BUNDLER || PRETRIM
using Xamarin.Bundler;
#endif

using Xamarin.Utils;

#if MTOUCH
using MonoTouch;
#endif

#nullable enable

namespace Xamarin {
	static class SdkVersions {
		public const string Xcode = "16.2";
		public const string OSX = "15.2";
		public const string iOS = "18.2";
		public const string TVOS = "18.2";
		public const string MacCatalyst = "18.2";

		public const string MinOSX = "12.0";
		public const string MiniOS = "12.2";
		public const string MinTVOS = "12.2";
		public const string MinMacCatalyst = "15.0";

		public const string DotNetMinOSX = "12.0";
		public const string DotNetMiniOS = "12.2";
		public const string DotNetMinTVOS = "12.2";
		public const string DotNetMinMacCatalyst = "15.0";
		public const string LegacyMinOSX = "12.0";
		public const string LegacyMiniOS = "12.2";
		public const string LegacyMinTVOS = "12.2";

		public const string MiniOSSimulator = "15.0";
		public const string MinTVOSSimulator = "15.0";

		public const string MaxiOSSimulator = "18.2";
		public const string MaxTVOSSimulator = "18.2";

		public const string MaxiOSDeploymentTarget = "18.2";
		public const string MaxTVOSDeploymentTarget = "18.2";

		public const string TargetPlatformVersionExecutableiOS = "18.2";
		public const string TargetPlatformVersionExecutabletvOS = "18.2";
		public const string TargetPlatformVersionExecutablemacOS = "15.2";
		public const string TargetPlatformVersionExecutableMacCatalyst = "18.2";

		public const string TargetPlatformVersionLibraryiOS = "18.0";
		public const string TargetPlatformVersionLibrarytvOS = "18.0";
		public const string TargetPlatformVersionLibrarymacOS = "15.0";
		public const string TargetPlatformVersionLibraryMacCatalyst = "18.0";

		public static Version OSXVersion { get { return new Version (OSX); } }
		public static Version iOSVersion { get { return new Version (iOS); } }
		public static Version TVOSVersion { get { return new Version (TVOS); } }
		public static Version MacCatalystVersion { get { return new Version (MacCatalyst); } }

		public static Version iOSTargetVersion { get { return new Version (MaxiOSDeploymentTarget); } }
		public static Version TVOSTargetVersion { get { return new Version (MaxTVOSDeploymentTarget); } }

		public static Version MinOSXVersion { get { return new Version (MinOSX); } }
		public static Version MiniOSVersion { get { return new Version (MiniOS); } }
		public static Version MinTVOSVersion { get { return new Version (MinTVOS); } }
		public static Version MinMacCatalystVersion { get { return new Version (MinMacCatalyst); } }

		public static Version MiniOSSimulatorVersion { get { return new Version (MiniOSSimulator); } }
		public static Version MinTVOSSimulatorVersion { get { return new Version (MinTVOSSimulator); } }

		public static Version MaxiOSSimulatorVersion { get { return new Version (MaxiOSSimulator); } }
		public static Version MaxTVOSSimulatorVersion { get { return new Version (MaxTVOSSimulator); } }

		public static Version XcodeVersion { get { return new Version (Xcode); } }

#if MTOUCH || MMP || BUNDLER || PRETRIM
		public static Version GetVersion (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSVersion;
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

	static class DotNetVersions {
		public const string Tfm = "net9.0";
		public const string Version = "9.0";
	}
}
