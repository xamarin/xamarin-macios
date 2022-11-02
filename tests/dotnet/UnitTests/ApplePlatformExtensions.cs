using Xamarin.Tests;

namespace Xamarin.Utils {
	public static class ApplePlatformExtensionsWithVersions {
		public static string ToFrameworkWithDefaultVersion (this ApplePlatform @this)
		{
			var netVersion = Configuration.DotNetTfm;
			var defaultTargetPlatformVersion = GetDefaultTargetPlatformVersion (@this);
			switch (@this) {
			case ApplePlatform.iOS:
				return netVersion + "-ios" + defaultTargetPlatformVersion;
			case ApplePlatform.MacOSX:
				return netVersion + "-macos" + defaultTargetPlatformVersion;
			case ApplePlatform.TVOS:
				return netVersion + "-tvos" + defaultTargetPlatformVersion;
			case ApplePlatform.MacCatalyst:
				return netVersion + "-maccatalyst" + defaultTargetPlatformVersion;
			default:
				return "Unknown";
			}
		}

		public static string GetDefaultTargetPlatformVersion (this ApplePlatform @this)
		{
			switch (@this) {
			case ApplePlatform.iOS: return SdkVersions.DefaultTargetPlatformVersioniOS;
			case ApplePlatform.TVOS: return SdkVersions.DefaultTargetPlatformVersiontvOS;
			case ApplePlatform.MacOSX: return SdkVersions.DefaultTargetPlatformVersionmacOS;
			case ApplePlatform.MacCatalyst: return SdkVersions.DefaultTargetPlatformVersionMacCatalyst;
			default:
				return "Unknown";
			}
		}
	}
}
