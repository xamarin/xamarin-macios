using System;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {

	public static class TestTargetExtensions {

		public static TestTarget [] GetAppRunnerTargets (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.tvOS:
				return new TestTarget [] { TestTarget.Simulator_tvOS };
			case TestPlatform.iOS:
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
				_ => throw new Exception (string.Format ("Invalid simulator target: {0}", target))
			};

		}
	}
}
