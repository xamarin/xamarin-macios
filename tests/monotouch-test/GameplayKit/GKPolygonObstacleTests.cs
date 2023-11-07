//
// Unit tests for GKPolygonObstacle
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using Foundation;
using GameplayKit;
using NUnit.Framework;

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GamePlayKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKPolygonObstacleTests {

		Vector2 [] points = new Vector2 [] {
			new Vector2 (0,0), new Vector2 (0,1), new Vector2 (0,2), new Vector2 (0,3),
			new Vector2 (1,0), new Vector2 (1,1), new Vector2 (1,2), new Vector2 (1,3)
		};

		[Test]
		public void FromPointsTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var obstacle = GKPolygonObstacle.FromPoints (points);
			Assert.NotNull (obstacle, "GKPolygonObstacle.FromPoints should not be null");

			var count = obstacle.VertexCount;
			Assert.AreEqual (points.Length, (int) count, "GKPolygonObstacle lengt should be equal");

			for (nuint i = 0; i < count; i++)
				Assert.AreEqual (points [(int) i], obstacle.GetVertex (i), "GKPolygonObstacle vectors should be equal");
		}

		[Test]
		public void InitWithPointsTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var obstacle = new GKPolygonObstacle (points);
			Assert.NotNull (obstacle, "GKPolygonObstacle ctor should not be null");

			var count = obstacle.VertexCount;
			Assert.AreEqual (points.Length, (int) count, "GKPolygonObstacle lengt should be equal");

			for (nuint i = 0; i < count; i++)
				Assert.AreEqual (points [(int) i], obstacle.GetVertex (i), "GKPolygonObstacle vectors should be equal");
		}
	}
}

#endif // __WATCHOS__
