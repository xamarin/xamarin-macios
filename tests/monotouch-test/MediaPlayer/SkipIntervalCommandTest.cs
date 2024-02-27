//
// Unit tests for MPSkipIntervalCommand
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using MediaPlayer;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SkipIntervalCommandTest {

		static bool manualBindingDone;
		[Test]
		public void ManualBinding ()
		{
			if (manualBindingDone)
				Assert.Ignore ("This test can only be executed once, it modifies global state.");
			manualBindingDone = true;

			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 1, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 12, 2, throwIfOtherPlatform: false);

			MPSkipIntervalCommand skip = MPRemoteCommandCenter.Shared.SkipBackwardCommand;

			if (TestRuntime.CheckXcodeVersion (11, 0)) {
				Assert.That (skip.PreferredIntervals, Is.EqualTo (new double [] { 10.0d }), "PreferredIntervals");
			} else {
				Assert.Null (skip.PreferredIntervals, "PreferredIntervals");
			}
			double [] intervals = new [] { 1.0d, 3.14d };
			skip.PreferredIntervals = intervals;

			Assert.That (skip.PreferredIntervals, Is.EqualTo (intervals), "identical");
		}
	}
}

#endif // !__WATCHOS__
