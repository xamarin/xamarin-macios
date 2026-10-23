//
// Unit tests for STWebHistory
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2025 Microsoft Corp. All rights reserved.
//

#if HAS_SCREENTIME

using ScreenTime;

namespace MonoTouchFixtures.ScreenTime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class STWebHistoryTest {

		[Test]
		public void Create_WithBundleIdentifier ()
		{
			TestRuntime.AssertXcodeVersion (16, 3);
			var bundleId = NSBundle.MainBundle.BundleIdentifier;
			using var obj = STWebHistory.Create (bundleId, out var error);
			Assert.That (obj, Is.Not.Null, "Object");
			Assert.That (error, Is.Null, "Error");
		}

		[Test]
		public void Create_WithBundleIdentifierAndProfile ()
		{
			TestRuntime.AssertXcodeVersion (16, 3);
			var bundleId = NSBundle.MainBundle.BundleIdentifier;
			using var obj = STWebHistory.Create (bundleId, (NSString) "profile", out var error);
			Assert.That (obj, Is.Not.Null, "Object");
			Assert.That (error, Is.Null, "Error");
		}
	}
}

#endif
