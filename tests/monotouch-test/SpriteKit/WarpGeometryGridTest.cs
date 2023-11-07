#if !__WATCHOS__

using System;
using Foundation;
using SpriteKit;
using NUnit.Framework;
#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.SpriteKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WarpGeometryGridTest {
		Vector2 [] points = new Vector2 [] {
			new Vector2 (0,0), new Vector2 (0,1), new Vector2 (0,2), new Vector2 (0,3),
			new Vector2 (1,3), new Vector2 (1,2), new Vector2 (1,1), new Vector2 (1,0)
		};

		[Test]
		public void SKWarpGeometryGridTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var grid = new SKWarpGeometryGrid (1, 1, points, points);
			Assert.NotNull (grid, "new SKWarpGeometryGrid () should not return null");
		}

		[Test]
		public void CreateTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var grid = SKWarpGeometryGrid.Create (1, 1, points, points);
			Assert.NotNull (grid, "SKWarpGeometryGrid.Create should not return null");
		}

		[Test]
		public void GetGridByReplacingSourcePositionsTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			using (var grid = SKWarpGeometryGrid.GetGrid ()) {
				var r = grid.GetGridByReplacingSourcePositions (points);
				Assert.NotNull (r, "GetGridByReplacingSourcePositions should not return null");
			}
		}

		[Test]
		public void GetGridByReplacingDestPositionsTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			using (var grid = SKWarpGeometryGrid.GetGrid ()) {
				var r = grid.GetGridByReplacingDestPositions (points);
				Assert.NotNull (r, "GetGridByReplacingDestPositions should not return null");
			}
		}
	}
}

#endif // !__WATCHOS__
