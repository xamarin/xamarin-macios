using System;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {

	public static class TestTargetExtensions {

		public static TestTarget[] GetAppRunnerTargets (this TestPlatform platform)
		{
			switch (platform) {
			case TestPlatform.tvOS:
				return new TestTarget [] { TestTarget.Simulator_tvOS };
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
				return new TestTarget [] { TestTarget.Simulator_watchOS };
			case TestPlatform.iOS_Unified:
				return new TestTarget [] { TestTarget.Simulator_iOS32, TestTarget.Simulator_iOS64 };
			case TestPlatform.iOS_Unified32:
				return new TestTarget [] { TestTarget.Simulator_iOS32 };
			case TestPlatform.iOS_Unified64:
			case TestPlatform.iOS_TodayExtension64:
				return new TestTarget [] { TestTarget.Simulator_iOS64 };
			default:
				throw new NotImplementedException (platform.ToString ());
			}
		}
	}
}
