//
// Unit tests for SKPhysicsJointFixed
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using SpriteKit;
using NUnit.Framework;

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PhysicsJointFixedTest {

		[Test]
		public void Create ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var s = new SKScene (new CGSize (320, 240)))
			using (var b1 = SKPhysicsBody.CreateCircularBody (1.0f))
			using (var b2 = SKPhysicsBody.CreateCircularBody (2.0f)) {
				// according to Apple docs we should not create this joint before adding it to a scene
				// <quote>The body must be connected to a node that is already part of the scene’s node tree.</quote>
				// Note that doing the same for a `SKPhysicsJointLimit` does crash
				using (var j = SKPhysicsJointFixed.Create (b1, b2, new CGPoint (10, 20))) {
					Assert.That (j.BodyA, Is.SameAs (b1), "BodyA");
					Assert.That (j.BodyB, Is.SameAs (b2), "BodyB");

					SKNode n1 = new SKNode ();
					n1.PhysicsBody = b1;
					s.AddChild (n1);

					SKNode n2 = new SKNode ();
					n2.PhysicsBody = b2;
					s.AddChild (n2);

					// using the default ctor (for `j`) would crash (and no way to set the PointF argument)
					// https://bugzilla.xamarin.com/show_bug.cgi?id=14511
					s.PhysicsWorld.AddJoint (j);
				}
			}
		}
	}
}

#endif // !__WATCHOS__
