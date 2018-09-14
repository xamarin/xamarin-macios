// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

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
	[Preserve (AllMembers = true)]
	public class StepperTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIStepper s = new UIStepper (frame)) {
				Assert.That (s.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (s.Frame.Y, Is.EqualTo (frame.Y), "Y");
				// Width and Height are set by the Slider (e.g. 94 x 27 for the iPhone)
			}
		}
		
		[Test]
		public void BackgroundImage ()
		{
			using (var s = new UIStepper ()) {
				s.SetBackgroundImage (null, UIControlState.Application);
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
