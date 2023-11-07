//
// Unit tests for GKBehavior
//
// Authors:
//	TJ Lambert <TJ.Lambert@microsoft.com>
//
//
// Copyright 2022 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using NUnit.Framework;

using Foundation;
using GameplayKit;

namespace MonoTouchFixtures.GamePlayKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKBehaviorTests {

		[Test]
		public void ObjectForKeyedSubscriptTest ()
		{
			// Create 2 GKGoals to use in the gkBehavior
			var goal1 = GKGoal.GetGoalToReachTargetSpeed (10);
			var goal2 = GKGoal.GetGoalToReachTargetSpeed (15);

			// Create a GKGoal that will not be included in gkBehavior
			var goal3 = GKGoal.GetGoalToReachTargetSpeed (15);

			// Create a GKBehavior from those first 2 goals
			var gkBehavior = GKBehavior.FromGoals (new GKGoal [] { goal1, goal2 });

			// Searching the gkBehavior for a non-exisitant goal should throw an ArgumentOutOfRangeException
			Assert.Throws<ArgumentOutOfRangeException> (() => { _ = gkBehavior [goal3]; }, "The ObjectForKeyedSubscript indexer should throw ArgumentOutOfRangeException if the goal is not in the behavior's list of GKGoals.");
		}
	}
}
#endif
