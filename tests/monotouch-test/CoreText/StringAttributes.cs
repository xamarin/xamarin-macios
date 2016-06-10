//
// Unit tests for CTStringAttributes
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using CoreGraphics;
using CoreText;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;
using System.Drawing;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreText
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StringAttributesTests
	{
		[Test]
		public void SimpleValuesSet ()
		{
			var sa = new CTStringAttributes ();
			sa.ForegroundColor = UIColor.Blue.CGColor;
			sa.Font = new CTFont ("Georgia-BoldItalic", 24);
			sa.UnderlineStyle = CTUnderlineStyle.Double; // It does not seem to do anything
			sa.UnderlineColor = UIColor.Blue.CGColor;
			sa.UnderlineStyleModifiers = CTUnderlineStyleModifiers.PatternDashDotDot;

			// CTBaseline and CTWritingDirection support is new in iOS 6.0 and cause NRE before it
			if (TestRuntime.CheckSystemAndSDKVersion (6,0)) {
				Assert.IsNull (sa.BaselineClass, "#0");
				sa.BaselineClass = CTBaselineClass.IdeographicHigh;
				Assert.AreEqual (CTBaselineClass.IdeographicHigh, sa.BaselineClass, "#1");

				sa.SetBaselineInfo (CTBaselineClass.Roman, 13);
				sa.SetBaselineInfo (CTBaselineClass.IdeographicHigh, 3);
				sa.SetWritingDirection (CTWritingDirection.LeftToRight);
			}

			var size = new SizeF (300, 300);
			UIGraphics.BeginImageContext (size);
			var gctx = UIGraphics.GetCurrentContext ();
            
			gctx.SetFillColor (UIColor.Green.CGColor);
            
			var attributedString = new NSAttributedString ("Test_ME~`", sa);
                
			using (var textLine = new CTLine (attributedString)) { 
				textLine.Draw (gctx);
			}

			UIGraphics.EndImageContext ();
		}
	}
}

#endif // !__WATCHOS__
