//
// Unit tests for SKPhysicsBody
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
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PhysicsBodyTest {

		[Test]
		public void BodyWithEdgeLoopFromRect ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			// bug 13772 - that call actually return a PKPhysicsBody (private PhysicKit framework)
			var size = new CGSize (3, 2);
			using (var body = SKPhysicsBody.CreateRectangularBody (size)) {
				Assert.That (body, Is.TypeOf<SKPhysicsBody> (), "SKPhysicsBody");
			}
		}

#if false
		// reminder that the default ctor is bad (uncomment to try again in future iOS release)
		// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=14502

		[Test]
		public void DefaultCtor ()
		{
			using (var Scene = new SKScene(UIScreen.MainScreen.Bounds.Size))
			using (var b = new SKSpriteNode (UIColor.Red, new CGSize (10, 10))) {
				b.PhysicsBody = new SKPhysicsBody ();
				Scene.AddChild (b); //BOOM
			}
		}
#endif
	}
}

#endif // !__WATCHOS__
