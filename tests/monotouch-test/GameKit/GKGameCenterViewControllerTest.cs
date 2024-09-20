//
// Unit tests for GKGameCenterViewControllerTest
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
	public class GKGameCenterViewControllerTest {
		[Test]
		public void StringCtor ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			using var controller = new GKGameCenterViewController ("achievementId");
			Assert.AreEqual (controller.ViewState, GKGameCenterViewControllerState.Achievements, "ViewState");
		}

		[Test]
		public void StringOptionCtor_AchievementId ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			using var controller = new GKGameCenterViewController ("achievementId", GKGameCenterViewControllerInitializationOption.Achievement);
			Assert.AreEqual (controller.ViewState, GKGameCenterViewControllerState.Achievements, "ViewState");
		}

		[Test]
		public void StringOptionCtor_LeaderboardSetId ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			using var controller = new GKGameCenterViewController ("achievementId", GKGameCenterViewControllerInitializationOption.LeaderboardSet);
			Assert.AreEqual (controller.ViewState, GKGameCenterViewControllerState.Leaderboards, "ViewState");
		}
	}
}

#endif // !__WATCHOS__
