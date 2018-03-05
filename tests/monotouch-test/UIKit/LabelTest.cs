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
	public class LabelTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UILabel l = new UILabel (frame)) {
				Assert.That (l.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void HighlightedTextColor ()
		{
			UILabel label = new UILabel ();
			Assert.Null (label.HighlightedTextColor, "HighlightedTextColor/default");
			label.HighlightedTextColor = UIColor.Blue;
			Assert.That (label.HighlightedTextColor, Is.EqualTo (UIColor.Blue), "HighlightedTextColor/blue");
			label.HighlightedTextColor = null;
			Assert.Null (label.HighlightedTextColor, "HighlightedTextColor/null");
		}
	}
}

#endif // !__WATCHOS__
