using System;

#if LEGACY_TOOLS || BUNDLER
using Xamarin.Bundler;
#endif

using Xamarin.Utils;

#nullable enable

namespace Xamarin {
	static class SdkVersions {
		public const string Xcode = "27.1";
		public const string OSX = "27.0";
		public const string iOS = "27.1";
		public const string TVOS = "27.0";
		public const string MacCatalyst = "27.0";

		public const string MinOSX = "14.0";
		public const string MiniOS = "15.0";
		public const string MinTVOS = "15.0";
		public const string MinMacCatalyst = "17.0";

		public const string DotNetMiniOS = "15.0";
		public const string DotNetMinTVOS = "15.0";
		public const string DotNetMinMacCatalyst = "17.0";

		public const string MiniOSSimulator = "17.0";
		public const string MinTVOSSimulator = "17.0";

		public const string MaxiOSSimulator = "27.1";
		public const string MaxTVOSSimulator = "27.0";

		public const string MaxiOSDeploymentTarget = "27.1";
		public const string MaxTVOSDeploymentTarget = "27.0";

		public const string TargetPlatformVersionExecutableiOS = "27.1";
		public const string TargetPlatformVersionExecutabletvOS = "27.0";
		public const string TargetPlatformVersionExecutablemacOS = "27.0";
		public const string TargetPlatformVersionExecutableMacCatalyst = "27.0";

		public const string TargetPlatformVersionLibraryiOS = "27.0";
		public const string TargetPlatformVersionLibrarytvOS = "27.0";
		public const string TargetPlatformVersionLibrarymacOS = "27.0";
		public const string TargetPlatformVersionLibraryMacCatalyst = "27.0";

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

#if LEGACY_TOOLS || BUNDLER
		public static Version GetVersion (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSVersion;
			case ApplePlatform.TVOS: return TVOSVersion;
			case ApplePlatform.MacCatalyst: return MacCatalystVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/dotnet/macios/issues/new with a test case.", app.Platform, app.ProductName);
			}
		}

		public static Version GetTargetVersion (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.MacOSX: return OSXVersion;
			case ApplePlatform.iOS: return iOSTargetVersion;
			case ApplePlatform.TVOS: return TVOSTargetVersion;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/dotnet/macios/issues/new with a test case.", app.Platform, app.ProductName);
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
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/dotnet/macios/issues/new with a test case.", app.Platform, app.ProductName);
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

	static class DotNetVersions {
		public const string Tfm = "net11.0";
		public const string Version = "11.0";
	}
}
