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
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
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

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class PanGestureRecognizerTest {

		[Test]
		public void Null ()
		{
			using (var pgr = new UIPanGestureRecognizer (Null)) {
				pgr.SetTranslation (PointF.Empty, null);

				var pt = pgr.TranslationInView (null);
				Assert.That (pt, Is.EqualTo (PointF.Empty), "TranslationInView");

				pt = pgr.VelocityInView (null);
				Assert.That (pt, Is.EqualTo (PointF.Empty), "VelocityInView");
			}
		}
	}
}

#endif // !__WATCHOS__
