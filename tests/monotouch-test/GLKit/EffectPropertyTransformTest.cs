// Copyright 2012 Xamarin Inc. All rights reserved

#if HAS_GLKIT

using System;
using System.Drawing;
using Foundation;
using GLKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.GLKit {

#if !NET
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EffectPropertytTransformTest {
		
		[Test]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

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
#endif // !NET
}

#endif // HAS_GLKIT
