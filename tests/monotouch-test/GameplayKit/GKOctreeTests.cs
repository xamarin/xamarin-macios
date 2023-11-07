//
// Unit tests for GKOctree
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
	public class GKOctreeTests {

		[Test]
		public void GKBoxTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS10+");

			var box = new GKBox {
				Max = new Vector3 (9, 8, 7),
				Min = new Vector3 (2, 3, 4)
			};
			var foo = new NSString ("Foo");
			using (var octree = new GKOctree<NSString> (box, 1)) {
				Assert.NotNull (octree, "octree is null");
				var node = octree.AddElement (foo, box);
				Assert.AreEqual (box, node.Box, "boxes are different");
				var strs = octree.GetElements (box);
				Assert.That (strs.Length, Is.GreaterThan (0), "Must have elements");
				Assert.AreSame (foo, strs [0], "must be the same object");
			}
		}
	}
}
#endif
