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
		public const string Xcode = "@XCODE_VERSION@";
		public const string OSX = "@MACOS_SDK_VERSION@";
		public const string iOS = "@IOS_SDK_VERSION@";
		public const string TVOS = "@TVOS_SDK_VERSION@";
		public const string MacCatalyst = "@MACCATALYST_SDK_VERSION@";

		public const string MinOSX = "@DOTNET_MIN_MACOS_SDK_VERSION@";
		public const string MiniOS = "@DOTNET_MIN_IOS_SDK_VERSION@";
		public const string MinTVOS = "@DOTNET_MIN_TVOS_SDK_VERSION@";
		public const string MinMacCatalyst = "@DOTNET_MIN_MACCATALYST_SDK_VERSION@";

		public const string DotNetMinOSX = "@DOTNET_MIN_MACOS_SDK_VERSION@";
		public const string DotNetMiniOS = "@DOTNET_MIN_IOS_SDK_VERSION@";
		public const string DotNetMinTVOS = "@DOTNET_MIN_TVOS_SDK_VERSION@";
		public const string DotNetMinMacCatalyst = "@DOTNET_MIN_MACCATALYST_SDK_VERSION@";
		public const string LegacyMinOSX = "@MIN_MACOS_SDK_VERSION@";
		public const string LegacyMiniOS = "@MIN_IOS_SDK_VERSION@";
		public const string LegacyMinTVOS = "@MIN_TVOS_SDK_VERSION@";

		public const string MiniOSSimulator = "@MIN_IOS_SIMULATOR_VERSION@";
		public const string MinTVOSSimulator = "@MIN_TVOS_SIMULATOR_VERSION@";

		public const string MaxiOSSimulator = "@MAX_IOS_SIMULATOR_VERSION@";
		public const string MaxTVOSSimulator = "@MAX_TVOS_SIMULATOR_VERSION@";

		public const string MaxiOSDeploymentTarget = "@MAX_IOS_DEPLOYMENT_TARGET@";
		public const string MaxTVOSDeploymentTarget = "@MAX_TVOS_DEPLOYMENT_TARGET@";

		public const string TargetPlatformVersionExecutableiOS = "@TARGET_PLATFORM_VERSION_EXECUTABLE_IOS@";
		public const string TargetPlatformVersionExecutabletvOS = "@TARGET_PLATFORM_VERSION_EXECUTABLE_TVOS@";
		public const string TargetPlatformVersionExecutablemacOS = "@TARGET_PLATFORM_VERSION_EXECUTABLE_MACOS@";
		public const string TargetPlatformVersionExecutableMacCatalyst = "@TARGET_PLATFORM_VERSION_EXECUTABLE_MACCATALYST@";

		public const string TargetPlatformVersionLibraryiOS = "@TARGET_PLATFORM_VERSION_LIBRARY_IOS@";
		public const string TargetPlatformVersionLibrarytvOS = "@TARGET_PLATFORM_VERSION_LIBRARY_TVOS@";
		public const string TargetPlatformVersionLibrarymacOS = "@TARGET_PLATFORM_VERSION_LIBRARY_MACOS@";
		public const string TargetPlatformVersionLibraryMacCatalyst = "@TARGET_PLATFORM_VERSION_LIBRARY_MACCATALYST@";

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

#if MTOUCH || MMP || BUNDLER
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
		public static string MinimumMono = "@MIN_XM_MONO_VERSION@";
		public static Version MinimumMonoVersion { get { return new Version (MinimumMono); } }
	}
#endif

	static class DotNetVersions {
		public const string Tfm = "@DOTNET_TFM@";
		public const string Version = "@DOTNET_VERSION@";
	}
}
