// Copyright 2011 Xamarin Inc. All rights reserved

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
	public class ToolbarTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIToolbar tb = new UIToolbar (frame)) {
				Assert.That (tb.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void BackgroundImage ()
		{
			using (UIToolbar tb = new UIToolbar ()) {
				Assert.Null (tb.GetBackgroundImage (UIToolbarPosition.Any, UIBarMetrics.Default), "Get");
				tb.SetBackgroundImage (null, UIToolbarPosition.Any, UIBarMetrics.Default);
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
