//
// Unit tests for CGGradient
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012, 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
using CoreGraphics;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
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

namespace MonoTouchFixtures.CoreGraphics {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GradientTest {

		static CGColor [] array = {
			TestRuntime.GetCGColor (UIColor.Black),
			TestRuntime.GetCGColor (UIColor.Clear),
			TestRuntime.GetCGColor (UIColor.Blue),
		};
		
		[Test]
		public void Colorspace_Null ()
		{
			using (var g = new CGGradient (null, array)) {
				Assert.That (g.Handle, Is.Not.EqualTo (IntPtr.Zero), "null,CGColor[]");
			}
			using (var g = new CGGradient (null, array, new nfloat [3] { 0f, 1f, 0.5f })) {
				Assert.That (g.Handle, Is.Not.EqualTo (IntPtr.Zero), "null,CGColor[],float[]");
			}
			
			using (var g = new CGGradient (null, array, new nfloat [3] { 0f, 1f, 0.5f })) {
				Assert.That (g.Handle, Is.Not.EqualTo (IntPtr.Zero), "null,CGColor[],float[]");
			}
		}
		
		static CGColorSpace [] spaces = {
			CGColorSpace.CreateDeviceGray (),
			CGColorSpace.CreateDeviceRGB (),
#if !XAMCORE_3_0
			CGColorSpace.Null
#endif
		};

		[Test]
		public void Colorspaces ()
		{
			foreach (var cs in spaces) {
				using (var g = new CGGradient (null, array, new nfloat [3] { 0f, 1f, 0.5f })) {
					Assert.That (g.Handle, Is.Not.EqualTo (IntPtr.Zero), cs.ToString ());
				}
			}
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGGradientRef */ IntPtr CGGradientCreateWithColorComponents (/* CGColorSpaceRef */ IntPtr colorspace, /* CGFloat[] */ nfloat [] components, /* CGFloat[] */ nfloat [] locations, /* size_t */ nint count);

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGGradientRef */ IntPtr CGGradientCreateWithColors (/* CGColorSpaceRef */ IntPtr space, /* CFArrayRef */ IntPtr colors, /* CGFloat[] */ nfloat [] locations);

		[Test]
		public void Nullable ()
		{
			// either a null CGColorSpace or a null CGFloat* array will return nil, i.e. not a valid instance
			using (var cs = CGColorSpace.CreateDeviceGray ())
				Assert.That (CGGradientCreateWithColorComponents (cs.Handle, null, null, 0), Is.EqualTo (IntPtr.Zero), "CGGradientCreateWithColorComponents-1");
			Assert.That (CGGradientCreateWithColorComponents (IntPtr.Zero, new nfloat [3] { 0f, 1f, 0.5f }, null, 0), Is.EqualTo (IntPtr.Zero), "CGGradientCreateWithColorComponents-2");

			// a null CFArray won't return a valid instance
			using (var cs = CGColorSpace.CreateDeviceGray ())
				Assert.That (CGGradientCreateWithColors (cs.Handle, IntPtr.Zero, null), Is.EqualTo (IntPtr.Zero), "CGGradientCreateWithColors-1");

			// a null CGColorSpace can return a valid instance
			using (var a = NSArray.FromNSObjects (array))
				Assert.That (CGGradientCreateWithColors (IntPtr.Zero, a.Handle, null), Is.Not.EqualTo (IntPtr.Zero), "CGGradientCreateWithColors-2");
		}

		[Test]
		public void GradientDrawingOptions ()
		{
			var gdo = CGGradientDrawingOptions.DrawsAfterEndLocation | CGGradientDrawingOptions.DrawsBeforeStartLocation;
			// this would be "3" without a [Flags] attribute
			Assert.That (gdo.ToString (), Is.EqualTo ("DrawsBeforeStartLocation, DrawsAfterEndLocation"), "ToString/Flags");
		}
	}
}
