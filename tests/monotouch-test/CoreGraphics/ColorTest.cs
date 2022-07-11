//
// Unit tests for CGColor
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ColorTest {

		[Test]
		public void GetConstantColor ()
		{
#if __IOS__
			// existing for a while outside iOS
			TestRuntime.AssertXcodeVersion (12, 0);
#endif
			using (var c = new CGColor (CGConstantColor.Clear)) {
				Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "CGColorGetConstantColor");
			}
		}

		[Test]
		public void CreateGenericGray ()
		{
#if __IOS__
			// existing for a while outside iOS
			TestRuntime.AssertXcodeVersion (12, 0);
#endif
			using (var c = new CGColor (0.5f, 0.5f)) {
				Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "CGColorCreateGenericGray");
			}
		}

		[Test]
		public void CreateGenericRGB ()
		{
#if __IOS__
			// existing for a while outside iOS
			TestRuntime.AssertXcodeVersion (12, 0);
#endif
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

		[Test]
		public void Cmyk ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			using (var c = CGColor.CreateCmyk (0.1f, 0.2f, 0.3f, 0.4f, 0.5f)) {
				Assert.That (c.NumberOfComponents, Is.EqualTo ((nint) 5), "NumberOfComponents");
				Assert.That ((Single) c.Alpha, Is.InRange (0.5f, 0.50001f), "Alpha");
				Assert.That (c.ColorSpace.Model, Is.EqualTo (CGColorSpaceModel.CMYK), "CGColorSpaceModel");
			}
		}

		[Test]
		public void GetAXName ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			using (var c = new CGColor (CGConstantColor.Black)) {
				Assert.IsNotNull (c.AXName, "AXName");
			}
		}

		[Test]
		public void CreateByMatchingToColorSpace ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var c = CGColor.CreateByMatchingToColorSpace (null, CGColorRenderingIntent.Default, null, null)) {
				Assert.IsNull (c, "0");
			}

			using (var cs = CGColorSpace.CreateGenericRgbLinear ())
			using (var c1 = CGColor.CreateSrgb (1, 2, 3, 4))
			using (var c2 = CGColor.CreateByMatchingToColorSpace (cs, CGColorRenderingIntent.Default, c1, null)) {
				Assert.IsNotNull (c1, "1");
				Assert.IsNotNull (c2, "2");
			}
		}
	}
}
