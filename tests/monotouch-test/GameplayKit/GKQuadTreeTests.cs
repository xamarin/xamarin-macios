//
// Unit tests for GKQuadTreeTests
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
	public class GKQuadTreeTests {

		[Test]
		public void GKQuadTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS10+");

			var quad = new GKQuad {
				Max = new Vector2 (10, 10),
				Min = new Vector2 (1, 1)
			};

			var expectedQuad = new GKQuad {
				Max = new Vector2 (0, 0),
				Min = new Vector2 (0, 0)
			};
			var foo = new NSString ("Foo");
			using (var quadTree = new GKQuadTree (quad, 1)) {
				Assert.NotNull (quadTree, "quadTree is null");
				var node = quadTree.AddElement (foo, quad);
				Assert.AreEqual (expectedQuad, node.Quad, $"quads are different");
				var strs = quadTree.GetElements (quad);
				Assert.That (strs.Length, Is.GreaterThan (0), "Must have elements");
				Assert.AreSame (foo, strs [0], "must be the same object");
			}
		}
	}
}
#endif
