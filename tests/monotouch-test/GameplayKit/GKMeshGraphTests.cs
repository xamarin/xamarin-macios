//
// Unit tests for GKMeshGraph
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using NUnit.Framework;

using Foundation;
using GameplayKit;

using System.Numerics;

#nullable enable

namespace MonoTouchFixtures.GamePlayKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKMeshGraphTests {

		[Test]
		public void GKTriangleTest ()
		{
			TestRuntime.AssertNotInterpreter ("This test does not work in the interpreter: https://github.com/dotnet/runtime/issues/110644");

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
