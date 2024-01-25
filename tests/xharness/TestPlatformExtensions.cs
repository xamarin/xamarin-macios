using System;

using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {
	public static class TestPlatformExtensions {

		public static string GetSimulatorMinVersion (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.iOS:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_TodayExtension64:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
				return "iOS " + Xamarin.SdkVersions.MiniOSSimulator;
			case TestPlatform.tvOS:
				return "tvOS " + Xamarin.SdkVersions.MinTVOSSimulator;
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
				return "watchOS " + Xamarin.SdkVersions.MinWatchOSSimulator;
			default:
				throw new NotImplementedException (platform.ToString ());
			}
		}

		public static bool IsMac (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.Mac:
			case TestPlatform.Mac_Modern:
			case TestPlatform.Mac_Full:
			case TestPlatform.Mac_System:
				return true;
			default:
				return false;
			}
		}

		public static bool CanSymlink (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.iOS:
			case TestPlatform.iOS_TodayExtension64:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
				return true;
			default:
				return false;
			}
		}

		// This must match our $(_PlatformName) variable in our MSBuild logic.
		public static string ToPlatformName (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.iOS:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
			case TestPlatform.iOS_TodayExtension64:
				return "iOS";
			case TestPlatform.tvOS:
				return "tvOS";
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
				return "watchOS";
			case TestPlatform.MacCatalyst:
				return "MacCatalyst";
			case TestPlatform.Mac:
			case TestPlatform.Mac_Modern:
			case TestPlatform.Mac_Full:
			case TestPlatform.Mac_System:
				return "macOS";
			default:
				return null;
			}
		}
	}
}
