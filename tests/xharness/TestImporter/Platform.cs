using System;

namespace Xharness.TestImporter {
	/// <summary>
	/// Represents the supported platforms to which we can create projects.
	/// </summary>
	public enum Platform {
		iOS,
		WatchOS,
		TvOS,
		MacOSFull,
		MacOSModern,
	}

	/// <summary>
	/// Represents the different types of wathcOS apps.
	/// </summary>
	public enum WatchAppType {
		App,
		Extension
	}

	public static class Platform_Extensions {
		public static string GetMinOSVersion (this Platform @this)
		{
			switch (@this) {
			case Platform.iOS:
				return global::Xamarin.SdkVersions.MiniOS;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return global::Xamarin.SdkVersions.MinOSX;
			case Platform.TvOS:
				return global::Xamarin.SdkVersions.MinTVOS;
			case Platform.WatchOS:
				return global::Xamarin.SdkVersions.MinWatchOS;
			default:
				throw new NotImplementedException (@this.ToString ());
			}
		}
	}
}
