//
// Unit tests for SKPhysicsJointLimit
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PhysicsJointLimitTest {

		[Test]
		public void Create ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var s = new SKScene (new SizeF (320, 240)))
			using (var b1 = SKPhysicsBody.CreateCircularBody (1.0f))
			using (var b2 = SKPhysicsBody.CreateCircularBody (2.0f)) {
				// <quote>The body must be connected to a node that is already part of the scene’s node tree.</quote>
				SKNode n1 = new SKNode ();
				n1.PhysicsBody = b1;
				s.AddChild (n1);

				SKNode n2 = new SKNode ();
				n2.PhysicsBody = b2;
				s.AddChild (n2);

				// if you create the SKPhysicsJointLimit *before* adding the nodes
				// to a scene then you'll crash and burn. ref: bug #14793
				using (var j = SKPhysicsJointLimit.Create (b1, b2, PointF.Empty, new PointF (10, 20))) {
					Assert.That (j.BodyA, Is.SameAs (b1), "BodyA");
					Assert.That (j.BodyB, Is.SameAs (b2), "BodyB");

					s.PhysicsWorld.AddJoint (j);
				}
			}
		}
	}
}

#endif // !__WATCHOS__
