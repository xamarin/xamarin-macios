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

		[Test]
		public void CtorObjects ()
		{
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((object []) null), "null");
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((object []) new [] { String.Empty, null }), "null element");
			Assert.Throws<ArgumentException> (() => new UISegmentedControl ((object []) new [] { NSZone.Default }), "invalid type");

			using (var ns = new NSString ("NSString"))
			using (var img = UIImage.FromFile ("basn3p08.png"))
			using (UISegmentedControl sc = new UISegmentedControl ("string", ns, img)) {
				Assert.That (sc.NumberOfSegments, Is.EqualTo (3), "NumberOfSegments");
			}
		}

		[Test]
		public void CtorNSArray ()
		{
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((NSArray) null), "null");

			using (UISegmentedControl sc = new UISegmentedControl (new NSArray ())) {
				Assert.That(sc.NumberOfSegments, Is.EqualTo (0), "Empty");
			}

			using (var ns = new NSString ("NSString"))
			using (var img = UIImage.FromFile ("basn3p08.png"))
			using (var a = NSArray.FromObjects ("string", ns, img))
			using (UISegmentedControl sc = new UISegmentedControl (a)) {
				Assert.That (sc.NumberOfSegments, Is.EqualTo (3), "NumberOfSegments");
			}
		}

		[Test]
		public void CtorNSString ()
		{
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((NSString) null), "null");
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((NSString[]) null), "null array");

			using (var ns = new NSString ("NSString"))
			using (UISegmentedControl sc = new UISegmentedControl (ns)) {
				Assert.That (sc.NumberOfSegments, Is.EqualTo (1), "NumberOfSegments");
			}
		}

		[Test]
		public void CtorString ()
		{
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((string) null), "null");
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((string[]) null), "null array");

			using (UISegmentedControl sc = new UISegmentedControl ("one", "two")) {
				Assert.That (sc.NumberOfSegments, Is.EqualTo (2), "NumberOfSegments");
			}
		}

		[Test]
		public void CtorUIImage ()
		{
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((UIImage) null), "null");
			Assert.Throws<ArgumentNullException> (() => new UISegmentedControl ((UIImage[]) null), "null array");

			using (var img = UIImage.FromFile ("basn3p08.png"))
			using (UISegmentedControl sc = new UISegmentedControl (img)) {
				Assert.That (sc.NumberOfSegments, Is.EqualTo (1), "NumberOfSegments");
			}
		}
	}
}

#endif // !__WATCHOS__
