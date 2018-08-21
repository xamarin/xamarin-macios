//
// Unit tests for CTStringAttributes
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using ObjCRuntime;
using UIKit;
#endif
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
	public class CTLineTests
	{
		[Test]
		public void EnumerateCaretOffsets ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS9+ or macOS 10.11+");
			
			var sa = new CTStringAttributes ();
			sa.ForegroundColor = TestRuntime.GetCGColor (UIColor.Blue);
			sa.Font = new CTFont ("Georgia-BoldItalic", 24);
			sa.UnderlineStyle = CTUnderlineStyle.Double; // It does not seem to do anything
			sa.UnderlineColor = TestRuntime.GetCGColor (UIColor.Blue);
			sa.UnderlineStyleModifiers = CTUnderlineStyleModifiers.PatternDashDotDot;

			var attributedString = new NSAttributedString ("Hello world.\nWoohooo!\nThere", sa);

			var line = new CTLine (attributedString);
			bool executed = false;
#if XAMCORE_2_0
			line.EnumerateCaretOffsets ((double o, nint charIndex, bool leadingEdge, ref bool stop) => {
#else
			line.EnumerateCaretOffsets ((double o, int charIndex, bool leadingEdge, ref bool stop) => {
#endif
				executed = true;
			});
			Assert.IsTrue (executed);
		}

		[Test]
		public void GetImageBounds ()
		{
			using (var a = new NSAttributedString ())
			using (var l = new CTLine (a)) {
				Assert.True (l.GetImageBounds (null).IsEmpty, "GetImageBounds");
			}
		}
	}
}
