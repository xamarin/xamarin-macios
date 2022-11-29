//
// Unit tests for GKMeshGraph
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using NUnit.Framework;

using Foundation;
using GameplayKit;

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GamePlayKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKMeshGraphTests {

		[Test]
		public void GKTriangleTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS10+");

			var max = new Vector2 (10, 10);
			var min = new Vector2 (1, 1);
			var def = new GKTriangle ();

			using (var mesh = new GKMeshGraph<GKGraphNode2D> (2, min, max, typeof (GKGraphNode2D))) {
				Assert.NotNull (mesh, "mesh is null");
				mesh.AddObstacles (new [] {
					new GKPolygonObstacle (new [] {
						new Vector2 (3,1),
						new Vector2 (3,2),
						new Vector2 (2,1),
						new Vector2 (2,2)
					}),
					new GKPolygonObstacle (new [] {
						new Vector2 (4,1),
						new Vector2 (4,2),
						new Vector2 (3,1),
						new Vector2 (3,2)
					})
				});
				mesh.Triangulate ();
				Assert.That (mesh.TriangleCount, Is.GreaterThan ((nuint) 0), "No Triangles");
				var triangle = mesh.GetTriangle (0);
				Assert.AreNotEqual (def, triangle, "Default triangle");
			}
		}
	}
}
#endif
