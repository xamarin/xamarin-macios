using System;
using System.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared;
using NUnit.Framework;

namespace Xharness.Tests.Tests {

	[TestFixture]
	public class TestTargetExtensionsTests {

		[TestCase (TestPlatform.tvOS, new [] { TestTarget.Simulator_tvOS })]
		public void GetAppRunnerTargetsTest (TestPlatform platform, TestTarget [] expectedTargets)
		{
			var targets = platform.GetTestTargetsForSimulator ();
			Assert.AreEqual (expectedTargets.Length, targets.Length);
			foreach (var t in expectedTargets) {
				Assert.Contains (t, targets);
			}
		}
	}
}
