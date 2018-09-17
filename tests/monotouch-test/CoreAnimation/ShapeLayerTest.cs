//
// Unit tests for CAShapeLayer
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreAnimation;
using CoreGraphics;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreAnimation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ShapeLayerTest {
		[Test]
		public void NullableProperties ()
		{
			var sl = new CAShapeLayer ();
			Assert.NotNull (sl.FillColor, "FillColor");
			sl.FillColor = null;
			Assert.Null (sl.Path, "Path");
			sl.Path = null;
			Assert.Null (sl.LineDashPattern, "LineDashPattern");
			sl.LineDashPattern = null;
			Assert.Null (sl.StrokeColor, "StrokeColor");
			sl.StrokeColor = null;

			sl.FillColor = TestRuntime.GetCGColor (UIColor.Black);
			Assert.NotNull (sl.FillColor, "FillColor");
			sl.Path = new CGPath ();
			Assert.NotNull (sl.Path, "Path");
			sl.LineDashPattern = new [] { new NSNumber (5), new NSNumber (10) };
			Assert.NotNull (sl.LineDashPattern, "LineDashPattern");
			sl.StrokeColor = TestRuntime.GetCGColor (UIColor.White);
			Assert.NotNull (sl.StrokeColor, "StrokeColor");
		}
	}
}

#endif // !__WATCHOS__
