//
// Unit tests for GKLeaderboard
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using GameKit;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.GameKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LeaderboardTest {

		void Check (GKLeaderboard lb)
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

#if !__TVOS__
			Assert.Null (lb.Category, "Category");
#endif
#if __MACOS__
			var hasGroupIdentifier = TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9);
			var hasIdentifier = TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10);
			var hasRange = TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10);
#elif __IOS__
			var hasGroupIdentifier = TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 6, 0);
			var hasIdentifier = TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0);
			var hasRange = TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0);
#elif __TVOS__
			var hasGroupIdentifier = true;
			var hasIdentifier = true;
			var hasRange = true;
#elif __WATCHOS__
			var hasGroupIdentifier = true;
			var hasIdentifier = true;
			var hasRange = TestRuntime.CheckSystemVersion (ApplePlatform.WatchOS, 3, 0);
#endif
			if (hasGroupIdentifier) {
				Assert.Null (lb.GroupIdentifier, "GroupIdentifier");
				if (hasIdentifier)
					Assert.Null (lb.Identifier, "Identifier");
			}
			Assert.Null (lb.LocalPlayerScore, "LocalPlayerScore");
			Assert.That (lb.MaxRange, Is.EqualTo ((nint) 0), "MaxRange");
			Assert.That (lb.PlayerScope, Is.EqualTo (GKLeaderboardPlayerScope.Global), "PlayerScope");
			if (hasRange) {
				// depending on the ctor 1,10 (or 0,0) is returned before iOS7 - but 1,25 is documented (in iOS7)
				Assert.That (lb.Range.Location, Is.EqualTo ((nint) 1), "Range.Location");
				Assert.That (lb.Range.Length, Is.EqualTo ((nint) 25), "Range.Length");
			}
			Assert.Null (lb.Scores, "Scores");
			Assert.That (lb.TimeScope, Is.EqualTo (GKLeaderboardTimeScope.AllTime), "TimeScope");
			Assert.Null (lb.Title, "Title");
		}

		[Test]
		public void DefaultCtor ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			using (var lb = new GKLeaderboard ()) {
				Check (lb);
			}
		}

		[Test]
		public void PlayersCtor ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			// note: Mavericks does not like (respond to) this selector - but it did work with ML and is documented
			using (var lb = new GKLeaderboard (new string [0])) {
				Check (lb);
			}
		}
	}
}

#endif // !__WATCHOS__
