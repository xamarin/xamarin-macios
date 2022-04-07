using Xamarin.Tests;

namespace Xamarin.Utils {
	public static class ApplePlatformExtensionsWithVersions {
		public static string ToFrameworkWithDefaultVersion (this ApplePlatform @this)
		{
			var netVersion = Configuration.DotNetTfm;
			switch (@this) {
			case ApplePlatform.iOS:
				return netVersion + "-ios" + SdkVersions.iOS;
			case ApplePlatform.MacOSX:
				return netVersion + "-macos" + SdkVersions.OSX;
			case ApplePlatform.TVOS:
				return netVersion + "-tvos" + SdkVersions.TVOS;
			case ApplePlatform.MacCatalyst:
				return netVersion + "-maccatalyst" + SdkVersions.MacCatalyst;
			default:
				return "Unknown";
			}
		}
	}
}
