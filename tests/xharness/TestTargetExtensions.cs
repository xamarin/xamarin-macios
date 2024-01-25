using System;

using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {

	public static class TestTargetExtensions {

		public static TestTarget [] GetAppRunnerTargets (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.tvOS:
				return new TestTarget [] { TestTarget.Simulator_tvOS };
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
				return new TestTarget [] { TestTarget.Simulator_watchOS };
			case TestPlatform.iOS_Unified:
				return new TestTarget [] { TestTarget.Simulator_iOS64 };
			case TestPlatform.iOS_Unified32:
				throw new NotSupportedException ($"32-bit simulators aren't supported anymore.");
			case TestPlatform.iOS_Unified64:
			case TestPlatform.iOS_TodayExtension64:
				return new TestTarget [] { TestTarget.Simulator_iOS64 };
			default:
				throw new NotImplementedException (platform.ToString ());
			}
		}

		public static TestTargetOs GetTargetOs (this TestTarget target, bool minVersion)
		{
			return target switch {
				TestTarget.Simulator_iOS64 => new TestTargetOs (target, minVersion ? Xamarin.SdkVersions.MiniOSSimulator : Xamarin.SdkVersions.MaxiOSSimulator),
				TestTarget.Simulator_tvOS => new TestTargetOs (target, minVersion ? Xamarin.SdkVersions.MinTVOSSimulator : Xamarin.SdkVersions.MaxTVOSSimulator),
				TestTarget.Simulator_watchOS => new TestTargetOs (target, minVersion ? Xamarin.SdkVersions.MinWatchOSSimulator : Xamarin.SdkVersions.MaxWatchOSSimulator),
				_ => throw new Exception (string.Format ("Invalid simulator target: {0}", target))
			};

		}
	}
}
