// Copyright 2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using GLKit;
#else
using MonoTouch.Foundation;
using MonoTouch.GLKit;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.GLKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EffectPropertytTransformTest {
		
		[Test]
		public void Properties ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			var transform = new GLKEffectPropertyTransform ();
			Assert.That (transform.ModelViewMatrix.ToString (), Is.EqualTo ("(1, 0, 0, 0)\n(0, 1, 0, 0)\n(0, 0, 1, 0)\n(0, 0, 0, 1)"), "ModelViewMatrix");
			Assert.That (transform.ProjectionMatrix.ToString (), Is.EqualTo ("(1, 0, 0, 0)\n(0, 1, 0, 0)\n(0, 0, 1, 0)\n(0, 0, 0, 1)"), "ProjectionMatrix");
			// Is TextureMatrix supposed to be here? I can't find it in apple's docs, and it throws a selector not found exception
			// Assert.That (transform.TextureMatrix.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "TextureMatrix");

			transform = new GLKBaseEffect ().Transform;
			Assert.That (transform.ModelViewMatrix.ToString (), Is.EqualTo ("(1, 0, 0, 0)\n(0, 1, 0, 0)\n(0, 0, 1, 0)\n(0, 0, 0, 1)"), "ModelViewMatrix");
			Assert.That (transform.ProjectionMatrix.ToString (), Is.EqualTo ("(1, 0, 0, 0)\n(0, 1, 0, 0)\n(0, 0, 1, 0)\n(0, 0, 0, 1)"), "ProjectionMatrix");
		}
	}
}

#endif // !__WATCHOS__
