using Xamarin.Tests;

namespace Xamarin.Utils {
	public static class ApplePlatformExtensionsWithVersions {
		public static string ToFrameworkWithPlatformVersion (this ApplePlatform @this)
		{
			var netVersion = Configuration.DotNetTfm;
			var targetPlatformVersion = GetTargetPlatformVersion (@this);
			switch (@this) {
			case ApplePlatform.iOS:
				return netVersion + "-ios" + targetPlatformVersion;
			case ApplePlatform.MacOSX:
				return netVersion + "-macos" + targetPlatformVersion;
			case ApplePlatform.TVOS:
				return netVersion + "-tvos" + targetPlatformVersion;
			case ApplePlatform.MacCatalyst:
				return netVersion + "-maccatalyst" + targetPlatformVersion;
			default:
				return "Unknown";
			}
		}

		public static string GetTargetPlatformVersion (this ApplePlatform @this)
		{
			switch (@this) {
			case ApplePlatform.iOS: return SdkVersions.TargetPlatformVersioniOS;
			case ApplePlatform.TVOS: return SdkVersions.TargetPlatformVersiontvOS;
			case ApplePlatform.MacOSX: return SdkVersions.TargetPlatformVersionmacOS;
			case ApplePlatform.MacCatalyst: return SdkVersions.TargetPlatformVersionMacCatalyst;
			default:
				return "Unknown";
			}
		}
	}
}
