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
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using NUnit.Framework;

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
		[Test]
		public void CreateSrgb ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var c = CGColor.CreateSrgb (0.1f, 0.2f, 0.3f, 0.4f)) {
				Assert.That (c.NumberOfComponents, Is.EqualTo ((nint) 4), "NumberOfComponents");
				Assert.That (c.Alpha, Is.InRange ((nfloat) 0.4f, (nfloat) 0.40001f), "Alpha");
				Assert.That (c.ColorSpace.Model, Is.EqualTo (CGColorSpaceModel.RGB), "CGColorSpaceModel");
			}
		}

		[Test]
		public void CreateGenericGrayGamma2_2 ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var c = CGColor.CreateGenericGrayGamma2_2 (0.1f, 0.2f)) {
				Assert.That (c.NumberOfComponents, Is.EqualTo ((nint) 2), "NumberOfComponents");
				Assert.That (c.Alpha, Is.InRange ((nfloat) 0.2f, (nfloat) 0.20001f), "Alpha");
				Assert.That (c.ColorSpace.Model, Is.EqualTo (CGColorSpaceModel.Monochrome), "CGColorSpaceModel");
			}
		}
	}
}

