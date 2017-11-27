// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if !__WATCHOS__
using System.Drawing;
#endif
#if XAMCORE_2_0
using Foundation;
#if !__WATCHOS__
using iAd;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.iAd;
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

namespace MonoTouchFixtures.iAd {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BannerViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (ADBannerView bv = new ADBannerView (frame)) {
				Assert.That (bv.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (bv.Frame.Y, Is.EqualTo (frame.Y), "Y");
				// Width and Height are set by the Ad (e.g. 320 x 50 for the iPhone)
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
