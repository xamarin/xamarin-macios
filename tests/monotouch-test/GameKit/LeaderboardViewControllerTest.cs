//
// Unit tests for GKLeaderboardViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

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

namespace MonoTouchFixtures.GameKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LeaderboardViewControllerTest {

		[Test]
		public void DefaultCtor ()
		{
			// because of the inheritance changes in GameKit running this on iOS 5.1 and the new registrar gives an:
			// MonoTouch.MonoTouchException> (Wrapper type 'MonoTouch.GameKit.GKGameCenterViewController' is missing its native ObjectiveC class 'GKGameCenterViewController'.)
			// ref: https://trello.com/c/OOlimcfJ/230-changing-objc-base-class-can-be-a-breaking-change
			if (!TestRuntime.CheckSystemAndSDKVersion (6, 0))
				Assert.Ignore ("Can't run before iOS 6 with the new registrar enabled");

			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);
			using (var vc = new GKLeaderboardViewController ()) {
				Assert.Null (vc.Category, "Category");
				Assert.Null (vc.Delegate, "Delegate");
				// default Scope vary by iOS version and can't be changed on iOS7 - not worth testing
			}
		}

		[Test]
		public void CustomCtor ()
		{
			// because of the inheritance changes in GameKit running this on iOS 5.1 and the new registrar gives an:
			// MonoTouch.MonoTouchException> (Wrapper type 'MonoTouch.GameKit.GKGameCenterViewController' is missing its native ObjectiveC class 'GKGameCenterViewController'.)
			// ref: https://trello.com/c/OOlimcfJ/230-changing-objc-base-class-can-be-a-breaking-change
			if (!TestRuntime.CheckSystemAndSDKVersion (6, 0))
				Assert.Ignore ("Can't run before iOS 6 with the new registrar enabled");

#if !XAMCORE_2_0
			// initWithTimeScope:playerScope: does not exists - Apple only list it in iOS4 API diff but it's neither
			// * in the web site documentation;
			// * in the header files
			// * a selector that respond on iOS or OSX
			// -[GKLeaderboardViewController initWithTimeScope:playerScope:]: unrecognized selector sent to instance 0x150f7220
			Assert.Throws<NotSupportedException> (delegate {
				new GKLeaderboardViewController (GKLeaderboardTimeScope.Week, GKLeaderboardPlayerScope.Global);
			});
#endif
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
