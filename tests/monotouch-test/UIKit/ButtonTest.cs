// Copyright 2011, 2013 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
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
	public class ButtonTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIButton b = new UIButton (frame)) {
				Assert.That (b.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void NullAllowed ()
		{
			using (var b = new UIButton ()) {
				b.SetTitle (null, UIControlState.Normal);
				Assert.IsNull (b.Title (UIControlState.Normal), "title");

				b.SetTitleColor (null, UIControlState.Normal);
				var hasTitleColor = true;
#if __IOS__
				if (!TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 0))
					hasTitleColor = false;
#endif
				if (hasTitleColor)
					Assert.That (b.TitleColor (UIControlState.Normal), Is.EqualTo (UIColor.White), "titlecolor");
				else
					Assert.IsNull (b.TitleColor (UIControlState.Normal), "titlecolor");

				b.SetTitleShadowColor (null, UIControlState.Normal);
				Assert.IsNull (b.TitleShadowColor (UIControlState.Normal), "titleshadowcolor");
			}
		}

		[Test]
		public void Tag_12557 ()
		{
			using (UIButton b = new UIButton ()) {
				Assert.That (b.Tag, Is.EqualTo ((nint) 0), "default");
				b.Tag = nint.MaxValue;
				Assert.That (b.Tag, Is.EqualTo (nint.MaxValue), "nint.MaxValue");
				b.Tag = nint.MinValue;
				Assert.That (b.Tag, Is.EqualTo (nint.MinValue), "nint.MinValue");
				b.Tag = 0;
				Assert.That (b.Tag, Is.EqualTo ((nint) 0), "0");
			}
		}
	}
}

#endif // !__WATCHOS__
