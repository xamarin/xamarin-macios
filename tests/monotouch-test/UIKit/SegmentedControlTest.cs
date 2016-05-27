// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__

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
	public class SegmentedControlTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UISegmentedControl sc = new UISegmentedControl (frame)) {
				Assert.That (sc.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void BackgroundImage ()
		{
			using (UISegmentedControl sc = new UISegmentedControl ()) {
				Assert.Null (sc.GetBackgroundImage (UIControlState.Application, UIBarMetrics.Default), "Get");
				sc.SetBackgroundImage (null, UIControlState.Application, UIBarMetrics.Default);
			}
		}

		[Test]
		public void Appearance_7 ()
		{
			// iOS 7 beta 3 throws "-setTintColor: is not allowed for use with the appearance proxy."
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=13286
			UISegmentedControl.Appearance.TintColor = UIColor.Blue;
		}
	}
}

#endif // !__WATCHOS__
