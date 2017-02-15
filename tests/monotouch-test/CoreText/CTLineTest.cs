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
using ObjCRuntime;
using UIKit;
using CoreGraphics;
using CoreText;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
using MonoTouch.ObjCRuntime;
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
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
				Assert.Ignore ("Requires iOS9+");

			var sa = new CTStringAttributes ();
			sa.ForegroundColor = UIColor.Blue.CGColor;
			sa.Font = new CTFont ("Georgia-BoldItalic", 24);
			sa.UnderlineStyle = CTUnderlineStyle.Double; // It does not seem to do anything
			sa.UnderlineColor = UIColor.Blue.CGColor;
			sa.UnderlineStyleModifiers = CTUnderlineStyleModifiers.PatternDashDotDot;

			var attributedString = new NSAttributedString ("Hello world.\nWoohooo!\nThere", sa);

			// there's a bug in iOS 10.3 (beta1 and 2 at least) where a crash happens, only on i386, if execution continue
			// this works fine with earlier simulator (on Xcode 8.3) or on devices (with 10.3) with 32bits builds
			if (TestRuntime.CheckSDKVersion (8, 3) && (Runtime.Arch == Arch.SIMULATOR) && (IntPtr.Size == 4))
				return;
			
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

#endif // !__WATCHOS__
