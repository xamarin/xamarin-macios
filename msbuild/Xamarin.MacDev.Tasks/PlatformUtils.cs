#nullable enable

namespace Xamarin.MacDev.Tasks {
	public static class PlatformUtils {
		public static string? GetTargetPlatform (string sdkPlatform, bool isWatchApp)
		{
			switch (sdkPlatform) {
			case "iPhoneSimulator":
				return isWatchApp ? "watchsimulator" : "iphonesimulator";
			case "iPhoneOS":
				return isWatchApp ? "watchos" : "iphoneos";
			case "MacOSX":
				return "macosx";
			case "WatchSimulator":
				return "watchsimulator";
			case "WatchOS":
				return "watchos";
			case "AppleTVSimulator":
				return "appletvsimulator";
			case "AppleTVOS":
				return "appletvos";
			case "MacCatalyst":
				return "macosx";
			}

			return null;
		}
	}
}
