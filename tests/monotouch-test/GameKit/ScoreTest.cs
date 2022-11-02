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
				if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false)) {
					Assert.Null (s.Player, "Player");
				}

				if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false)) {
					Assert.That (s.LeaderboardIdentifier, Is.EqualTo ("category-or-identifier"), "LeaderboardIdentifier");
				}

				Assert.That (s.RetainCount, Is.EqualTo ((nuint) 1), "RetainCount");
			}
		}
	}
}

#endif // !__WATCHOS__
