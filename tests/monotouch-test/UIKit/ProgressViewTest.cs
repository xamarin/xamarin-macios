// Copyright 2011 Xamarin Inc. All rights reserved

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
	[Preserve (AllMembers = true)]
	public class ProgressViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIProgressView pv = new UIProgressView (frame)) {
				Assert.That (pv.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (pv.Frame.Y, Is.EqualTo (frame.Y), "Y");
				Assert.That (pv.Frame.Width, Is.EqualTo (frame.Width), "Width");
				// Height is set by the ProgressView (e.g. 9 for the iPhone)
			}
		}
	}
}

#endif // !__WATCHOS__
