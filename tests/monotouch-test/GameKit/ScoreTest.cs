//
// Unit tests for GKScore
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.IO;
using System.Threading;
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
	public class ScoreTest {

		[Test]
		public void Ctor_String ()
		{
			// Apple deprecated `initWithCategory:` for `initWithLeaderboardIdentifier:` which  *seems* to do the 
			// same thing, only with new names - but we need to be sure since 7.0 will map the .ctor(string) API
			// to the new selector
			using (var s = new GKScore ("category-or-identifier")) {
#if !__TVOS__
				Assert.That (s.Category, Is.EqualTo ("category-or-identifier"), "Category");
#endif
				Assert.That (s.Context, Is.EqualTo (0), "Context");
				Assert.NotNull (s.Date, "Date");
				Assert.Null (s.FormattedValue, "FormattedValue");

				// this is a new API in iOS8 (it was private before that) and returned an empty instance like:
				// "<<GKPlayer: 0x81254e60>(playerID:(null) alias:(null) name:(null) status:(null))>"
				if (TestRuntime.CheckiOSSystemVersion (8, 0, throwIfOtherPlatform: false)) {
					Assert.Null (s.Player, "Player");
				}

				if (TestRuntime.CheckiOSSystemVersion (7, 0, throwIfOtherPlatform: false)) {
					Assert.That (s.LeaderboardIdentifier, Is.EqualTo ("category-or-identifier"), "LeaderboardIdentifier");
				}

				Assert.That (s.RetainCount, Is.EqualTo ((nint) 1), "RetainCount");
			}
		}
	}
}

#endif // !__WATCHOS__
