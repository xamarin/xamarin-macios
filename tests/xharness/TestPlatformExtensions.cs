using System;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {
	public static class TestPlatformExtensions {

		public static string GetSimulatorMinVersion (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.iOS:
				return "iOS " + Xamarin.SdkVersions.MiniOSSimulator;
			case TestPlatform.tvOS:
				return "tvOS " + Xamarin.SdkVersions.MinTVOSSimulator;
			default:
				throw new NotImplementedException (platform.ToString ());
			}
		}

		public static bool IsMac (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.Mac:
				return true;
			default:
				return false;
			}
		}

		public static bool CanSymlink (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.iOS:
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
				return "iOS";
			case TestPlatform.tvOS:
				return "tvOS";
			case TestPlatform.MacCatalyst:
				return "MacCatalyst";
			case TestPlatform.Mac:
				return "macOS";
			default:
				return null;
			}
		}
	}
}
