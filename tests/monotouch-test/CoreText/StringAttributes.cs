//
// Unit tests for CTStringAttributes
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
using CoreGraphics;
using CoreText;
using NUnit.Framework;
using System.Drawing;

namespace MonoTouchFixtures.CoreText {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StringAttributesTests {
#if !MONOMAC // No UIGraphics on mac
		[Test]
		public void SimpleValuesSet ()
		{
			if (TestRuntime.CheckXcodeVersion (15, 0))
				Assert.Ignore ("Test timeouts on Xcode 15 beta 4: https://github.com/xamarin/xamarin-macios/issues/18656");

			var sa = new CTStringAttributes ();
			sa.ForegroundColor = UIColor.Blue.CGColor;
			sa.Font = new CTFont ("Georgia-BoldItalic", 24);
			sa.UnderlineStyle = CTUnderlineStyle.Double; // It does not seem to do anything
			sa.UnderlineColor = UIColor.Blue.CGColor;
			sa.UnderlineStyleModifiers = CTUnderlineStyleModifiers.PatternDashDotDot;

			Assert.IsNull (sa.BaselineClass, "#0");
			sa.BaselineClass = CTBaselineClass.IdeographicHigh;
			Assert.AreEqual (CTBaselineClass.IdeographicHigh, sa.BaselineClass, "#1");

			sa.SetBaselineInfo (CTBaselineClass.Roman, 13);
			sa.SetBaselineInfo (CTBaselineClass.IdeographicHigh, 3);
			sa.SetWritingDirection (CTWritingDirection.LeftToRight);

			if (TestRuntime.CheckXcodeVersion (11, 0))
				sa.TrackingAdjustment = 1.0f;

			var size = new CGSize (300, 300);
			UIGraphics.BeginImageContext (size);
			var gctx = UIGraphics.GetCurrentContext ();

			gctx.SetFillColor (UIColor.Green.CGColor);

			var attributedString = new NSAttributedString ("Test_ME~`", sa);

			using (var textLine = new CTLine (attributedString)) {
				textLine.Draw (gctx);
			}

			UIGraphics.EndImageContext ();
		}
#endif

		[Test]
		public void BackgroundColor ()
		{
			var sa = new CTStringAttributes ();
			Assert.DoesNotThrow (() => { sa.BackgroundColor = TestRuntime.GetCGColor (UIColor.Blue); }, "#0");
			Assert.DoesNotThrow (() => { var x = sa.BackgroundColor; }, "#1");
		}

		[Test]
		public void HorizontalInVerticalForms ()
		{
			var sa = new CTStringAttributes ();
			Assert.DoesNotThrow (() => { sa.HorizontalInVerticalForms = 1; }, "#0");
			Assert.DoesNotThrow (() => { var x = sa.HorizontalInVerticalForms; }, "#1");
		}
	}
}
