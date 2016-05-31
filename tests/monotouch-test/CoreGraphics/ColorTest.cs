//
// Unit tests for CGColor
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using CoreGraphics;
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
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
	public class ColorTest {

#if !XAMCORE_3_0 || MONOMAC

		// those API are not availably (officially) in iOS according to Apple header files
		// still they do work which makes them _private_ API that we should not expose

		[Test]
		public void GetConstantColor ()
		{
			// !unknown-pinvoke! CGColorGetConstantColor bound
			// CGColorGetConstantColor -> CG_AVAILABLE_STARTING(__MAC_10_5, __IPHONE_NA);
			// constants to be used are not available either (and not bound)
			// kCGColorWhite, kCGColorBlack and kCGColorClear

			var lib = Dlfcn.dlopen (Constants.CoreGraphicsLibrary, 0);
			var clear = Dlfcn.GetStringConstant (lib, "kCGColorClear"); // constant also marked as __IPHONE_NA
			using (var c = new CGColor (clear)) {
				Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "CGColorGetConstantColor");
			}
		}

		[Test]
		public void CreateGenericGray ()
		{
			// !unknown-pinvoke! CGColorCreateGenericGray bound
			// CGColorCreateGenericGray -> CG_AVAILABLE_STARTING(__MAC_10_5, __IPHONE_NA);
			using (var c = new CGColor (0.5f, 0.5f)) {
				Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "CGColorCreateGenericGray");
			}
		}

		[Test]
		public void CreateGenericRGB ()
		{
			// !unknown-pinvoke! CGColorCreateGenericRGB bound
			// CGColorCreateGenericRGB -> CG_AVAILABLE_STARTING(__MAC_10_5, __IPHONE_NA);
			using (var c = new CGColor (0.5f, 0.5f, 0.5f, 0.5f)) {
				Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "CGColorCreateGenericRGB");
			}
		}

		[Test]
		public void ColorSpace ()
		{
			using (var c = new CGColor (0.5f, 0.5f, 0.5f, 0.5f)) {
				using (var spc = c.ColorSpace)
					Assert.IsNotNull (spc, "ColorSpace");
			}
		}
#endif
	}
}

