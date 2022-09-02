using System;
using System.IO;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;
using Xamarin.MacDev.Tasks;

namespace Xamarin.MacDev {
	public static class Sdks {
		const string MTOUCH_LOCATION_ENV_VAR = "MD_MTOUCH_SDK_ROOT";

		public static XamMacSdk XamMac { get; private set; }
		public static AppleIPhoneSdk IOS { get; private set; }

		public static MacOSXSdk MacOS { get; private set; }
		public static MonoTouchSdk XamIOS { get; internal set; }
		public static AppleWatchSdk Watch { get; private set; }
		public static AppleTVOSSdk TVOS { get; private set; }

		static Sdks ()
		{
			Reload ();
		}

		static void Reload ()
		{
			var monotouch = Environment.GetEnvironmentVariable (MTOUCH_LOCATION_ENV_VAR);

			if (string.IsNullOrEmpty (monotouch)) {
				foreach (var location in MonoTouchSdk.DefaultLocations) {
					if (Directory.Exists (location)) {
						monotouch = location;
						break;
					}
				}
			}

			XamIOS = new MonoTouchSdk (monotouch);
			IOS = new AppleIPhoneSdk (AppleSdkSettings.DeveloperRoot, AppleSdkSettings.DeveloperRootVersionPlist);
			Watch = new AppleWatchSdk (AppleSdkSettings.DeveloperRoot, AppleSdkSettings.DeveloperRootVersionPlist);
			TVOS = new AppleTVOSSdk (AppleSdkSettings.DeveloperRoot, AppleSdkSettings.DeveloperRootVersionPlist);

			XamMac = new XamMacSdk (null);
			MacOS = new MacOSXSdk (AppleSdkSettings.DeveloperRoot, AppleSdkSettings.DeveloperRootVersionPlist);
		}

		public static AppleSdk GetSdk (ApplePlatform framework)
		{
			switch (framework) {
			case ApplePlatform.iOS:
				return IOS;
			case ApplePlatform.WatchOS:
				return Watch;
			case ApplePlatform.TVOS:
				return TVOS;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidFramework, framework));
			}
		}

		public static AppleSdk GetSdk (string targetFrameworkMoniker)
		{
			return GetSdk (PlatformFrameworkHelper.GetFramework (targetFrameworkMoniker));
		}

		public static IAppleSdk GetAppleSdk (ApplePlatform framework)
		{
			switch (framework) {
			case ApplePlatform.iOS:
				return IOS;
			case ApplePlatform.WatchOS:
				return Watch;
			case ApplePlatform.TVOS:
				return TVOS;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				return MacOS;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidFramework, framework));
			}
		}

		public static IAppleSdk GetAppleSdk (string targetFrameworkMoniker)
		{
			return GetAppleSdk (PlatformFrameworkHelper.GetFramework (targetFrameworkMoniker));
		}

	}
}
