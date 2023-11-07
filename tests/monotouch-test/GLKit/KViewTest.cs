// Copyright 2011 Xamarin Inc. All rights reserved

#if HAS_GLKIT && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using GLKit;
using NUnit.Framework;

namespace MonoTouchFixtures.GLKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (GLKView glkv = new GLKView (frame)) {
				Assert.That (glkv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
	}
}

#endif // HAS_GLKIT && !MONOMAC
