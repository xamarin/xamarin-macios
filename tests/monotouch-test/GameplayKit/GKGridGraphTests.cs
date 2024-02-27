//
// Unit tests for GKGridGraph
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Collections.Generic;

using Foundation;
using GameplayKit;
using NUnit.Framework;

#if NET
using Vector2i = global::CoreGraphics.NVector2i;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GamePlayKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKGridGraphTests {

		enum TileType {
			Open,
			Wall,
			Portal,
			Start
		}

		int [] maze = {
			1,1,1,1,1,1,1,1,1,1,
			1,3,0,0,1,1,1,1,1,1,
			1,0,1,0,2,0,0,0,0,1,
			1,0,1,1,1,1,1,1,0,1,
			1,0,1,0,2,0,0,0,0,1,
			1,0,1,0,1,1,1,1,1,1,
			1,0,1,0,2,0,0,0,0,1,
			1,0,1,1,1,1,1,1,0,1,
			1,0,0,0,2,0,0,0,0,1,
			1,1,1,1,1,1,1,1,1,1,
		};

		[Test]
		public void FromGridStartingAtTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var graph = GKGridGraph.FromGridStartingAt (Vector2i.Zero, 10, 10, false);
			Assert.NotNull (graph, "GKGridGraph.FromGridStartingAt should not be null");

			var walls = new List<GKGridGraphNode> (10 * 10);
			var spawnPoints = new List<GKGridGraphNode> ();
			GKGridGraphNode startPosition = null;

			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					var tile = maze [i * 10 + j];
					switch ((TileType) tile) {
					case TileType.Wall:
#if NET
						walls.Add (graph.GetNodeAt<GKGridGraphNode> (new Vector2i (i, j)));
#else
						walls.Add (graph.GetNodeAt (new Vector2i (i, j)));
#endif
						break;
					case TileType.Portal:
#if NET
						spawnPoints.Add (graph.GetNodeAt<GKGridGraphNode> (new Vector2i (i, j)));
#else
						spawnPoints.Add (graph.GetNodeAt (new Vector2i (i, j)));
#endif
						break;
					case TileType.Start:
#if NET
						startPosition = graph.GetNodeAt<GKGridGraphNode> (new Vector2i (i, j));
#else
						startPosition = graph.GetNodeAt (new Vector2i (i, j));
#endif
						break;
					default:
						break;
					}
				}
			}

			// increasing min required version due to in iOS 9.2 we get a crash from GameplayKit
			// pretty similar to this report on stackoverflow, this does not happen on 9.3
			// http://stackoverflow.com/questions/35811432/gameplaykit-gkgraph-dealloc-crash-on-ios9-2
			if (TestRuntime.CheckXcodeVersion (7, 3))
				graph.RemoveNodes (walls.ToArray ());

			Assert.NotNull (startPosition, "startPosition must not be null");
			Assert.AreEqual (new Vector2i (1, 1), startPosition.GridPosition, "GridPosition must be (1,1)");
			Assert.That (walls.Count > 0, "walls list must be higher than zero");
			Assert.That (spawnPoints.Count > 0, "spawnPoints list must be higher than zero");
		}

		[Test]
		public void InitFromGridStartingAtTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var graph = new GKGridGraph (Vector2i.Zero, 10, 10, false);
			Assert.NotNull (graph, "GKGridGraph.FromGridStartingAt should not be null");

			var walls = new List<GKGridGraphNode> (10 * 10);
			var spawnPoints = new List<GKGridGraphNode> ();
			GKGridGraphNode startPosition = null;

			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					var tile = maze [i * 10 + j];
					switch ((TileType) tile) {
					case TileType.Wall:
#if NET
						walls.Add (graph.GetNodeAt<GKGridGraphNode> (new Vector2i (i, j)));
#else
						walls.Add (graph.GetNodeAt (new Vector2i (i, j)));
#endif
						break;
					case TileType.Portal:
#if NET
						spawnPoints.Add (graph.GetNodeAt<GKGridGraphNode> (new Vector2i (i, j)));
#else
						spawnPoints.Add (graph.GetNodeAt (new Vector2i (i, j)));
#endif
						break;
					case TileType.Start:
#if NET
						startPosition = graph.GetNodeAt<GKGridGraphNode> (new Vector2i (i, j));
#else
						startPosition = graph.GetNodeAt (new Vector2i (i, j));
#endif
						break;
					default:
						break;
					}
				}
			}

			// increasing min required version due to in 9.2 we get a crash from GameplayKit
			// pretty similar to this report on stackoverflow, this does not happen on 9.3
			// http://stackoverflow.com/questions/35811432/gameplaykit-gkgraph-dealloc-crash-on-ios9-2
			if (TestRuntime.CheckXcodeVersion (7, 3))
				graph.RemoveNodes (walls.ToArray ());

			Assert.NotNull (startPosition, "startPosition must not be null");
			Assert.AreEqual (new Vector2i (1, 1), startPosition.GridPosition, "GridPosition must be (1,1)");
			Assert.That (walls.Count > 0, "walls list must be higher than zero");
			Assert.That (spawnPoints.Count > 0, "spawnPoints list must be higher than zero");
		}
	}
}

#endif // __WATCHOS__
