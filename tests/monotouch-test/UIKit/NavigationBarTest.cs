// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
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
	public class NavigationBarTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UINavigationBar nb = new UINavigationBar (frame)) {
				Assert.That (nb.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void BackgroundImage ()
		{
			// http://stackoverflow.com/q/10504966/220643
			using (UINavigationBar nb = new UINavigationBar ()) {
				Assert.Null (nb.GetBackgroundImage (UIBarMetrics.Default), "Get");
				nb.SetBackgroundImage (null, UIBarMetrics.Default);
			}
		}
	}
}

#endif // !__WATCHOS__
