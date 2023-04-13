//
// UIPanGestureRecognizer Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class PanGestureRecognizerTest {

		[Test]
		public void Null ()
		{
			using (var pgr = new UIPanGestureRecognizer (Null)) {
				pgr.SetTranslation (CGPoint.Empty, null);

				var pt = pgr.TranslationInView (null);
				Assert.That (pt, Is.EqualTo (CGPoint.Empty), "TranslationInView");

				pt = pgr.VelocityInView (null);
				Assert.That (pt, Is.EqualTo (CGPoint.Empty), "VelocityInView");
			}
		}
	}
}

#endif // !__WATCHOS__
