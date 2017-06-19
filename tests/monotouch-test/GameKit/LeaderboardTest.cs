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
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using GameKit;
#else
using MonoTouch.Foundation;
using MonoTouch.GameKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.GameKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LeaderboardTest {

		void Check (GKLeaderboard lb)
		{
#if !__TVOS__
			Assert.Null (lb.Category, "Category");
#endif
			if (TestRuntime.CheckSystemAndSDKVersion (6,0)) {
				Assert.Null (lb.GroupIdentifier, "GroupIdentifier");
				if (TestRuntime.CheckSystemAndSDKVersion (7,0))
					Assert.Null (lb.Identifier, "Identifier");
			}
			Assert.Null (lb.LocalPlayerScore, "LocalPlayerScore");
			Assert.That (lb.MaxRange, Is.EqualTo ((nint) 0), "MaxRange");
			Assert.That (lb.PlayerScope, Is.EqualTo (GKLeaderboardPlayerScope.Global), "PlayerScope");
			if (TestRuntime.CheckSystemAndSDKVersion (7,0)) {
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
			using (var lb = new GKLeaderboard ()) {
				Check (lb);
			}
		}

		[Test]
		public void PlayersCtor ()
		{
			// note: Mavericks does not like (respond to) this selector - but it did work with ML and is documented
			using (var lb = new GKLeaderboard (new string [0])) {
				Check (lb);
			}
		}
	}
}

#endif // !__WATCHOS__