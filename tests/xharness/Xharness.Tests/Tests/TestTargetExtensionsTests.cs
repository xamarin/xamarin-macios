using System;
using System.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared;
using NUnit.Framework;

namespace Xharness.Tests.Tests {

	[TestFixture]
	public class TestTargetExtensionsTests {

		[TestCase (TestPlatform.tvOS, new [] { TestTarget.Simulator_tvOS })]
		[TestCase (TestPlatform.watchOS, new [] { TestTarget.Simulator_watchOS })]
		[TestCase (TestPlatform.watchOS_32, new [] { TestTarget.Simulator_watchOS })]
		[TestCase (TestPlatform.watchOS_64_32, new [] { TestTarget.Simulator_watchOS })]
		[TestCase (TestPlatform.iOS_Unified, new [] { TestTarget.Simulator_iOS64 })]
		[TestCase (TestPlatform.iOS_Unified64, new [] { TestTarget.Simulator_iOS64 })]
		[TestCase (TestPlatform.iOS_TodayExtension64, new [] { TestTarget.Simulator_iOS64 })]
		public void GetAppRunnerTargetsTest (TestPlatform platform, TestTarget [] expectedTargets)
		{
			var targets = platform.GetAppRunnerTargets ();
			Assert.AreEqual (expectedTargets.Length, targets.Length);
			foreach (var t in expectedTargets) {
				Assert.Contains (t, targets);
			}
		}
	}
}
