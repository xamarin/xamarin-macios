//
// Unit tests for CGColorConversionInfo
//
// Authors:
//	Vincent Dondain <vincent@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

#if NET
using GColorConversionInfoTriple = CoreGraphics.CGColorConversionInfoTriple;
#endif

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ColorConversionInfoTest {

		[Test]
		public void CreateNone ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Assert.Throws<ArgumentNullException> (() => new CGColorConversionInfo (null, (CGColorSpace) null), "null");
			Assert.Throws<ArgumentNullException> (() => new CGColorConversionInfo ((NSDictionary) null, (GColorConversionInfoTriple []) null), "null-2");
			Assert.Throws<ArgumentNullException> (() => new CGColorConversionInfo ((NSDictionary) null, new GColorConversionInfoTriple [0]), "empty");
		}

		[Test]
		public void CreateSingle ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var triple = new GColorConversionInfoTriple () {
				Space = CGColorSpace.CreateGenericRgb (),
				Intent = CGColorRenderingIntent.Default,
				Transform = CGColorConversionInfoTransformType.ApplySpace
			};

			var options = new CGColorConversionOptions () {
				BlackPointCompensation = false
			};

			using (var converter = new CGColorConversionInfo (options, triple)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CreateDual ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var triple = new GColorConversionInfoTriple () {
				Space = CGColorSpace.CreateGenericRgb (),
				Intent = CGColorRenderingIntent.Default,
				Transform = CGColorConversionInfoTransformType.ApplySpace
			};

			var options = new CGColorConversionOptions () {
				BlackPointCompensation = true
			};

			using (var converter = new CGColorConversionInfo ((CGColorConversionOptions) null, triple, triple)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CreateMax ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var first = new GColorConversionInfoTriple () {
				Space = CGColorSpace.CreateGenericRgb (),
				Intent = CGColorRenderingIntent.Default,
				Transform = CGColorConversionInfoTransformType.ApplySpace
			};
			var second = new GColorConversionInfoTriple () {
				Space = CGColorSpace.CreateGenericGray (),
				Intent = CGColorRenderingIntent.Perceptual,
				Transform = CGColorConversionInfoTransformType.FromSpace
			};
			var third = new GColorConversionInfoTriple () {
				Space = CGColorSpace.CreateGenericXyz (),
				Intent = CGColorRenderingIntent.Saturation,
				Transform = CGColorConversionInfoTransformType.ToSpace
			};

			using (var converter = new CGColorConversionInfo ((NSDictionary) null, first, first, first)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CreateTooMany ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			Assert.Throws<ArgumentException> (() => new CGColorConversionInfo ((CGColorConversionOptions) null, new GColorConversionInfoTriple [4]));
		}

		[Test]
		public void CreateSimple ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			using (var from = CGColorSpace.CreateGenericGray ())
			using (var to = CGColorSpace.CreateGenericRgb ())
			using (var converter = new CGColorConversionInfo (from, to)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorConversionInfoCreate (IntPtr src, IntPtr dst);

		[Test]
		public void CreateSimple_GetINativeObject ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			using (var from = CGColorSpace.CreateGenericGray ())
			using (var to = CGColorSpace.CreateGenericRgb ()) {
				var handle = CGColorConversionInfoCreate (from is null ? IntPtr.Zero : from.Handle,
														   to is null ? IntPtr.Zero : to.Handle);
				using (var o = Runtime.GetINativeObject<CGColorConversionInfo> (handle, false)) {
					Assert.That (o.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				}
			}
		}

		[Test]
		public void CreateSimple_DeviceColorSpace ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			// Requirements: CG color spaces must be calibrated
			// (no Device{Gray,RGB,CMYK}, Indexed or DeviceN).
			// This test lets us know if Apple changes that behavior.
			using (var from = CGColorSpace.CreateDeviceGray ())
			using (var to = CGColorSpace.CreateDeviceRGB ()) {
				Assert.Throws<Exception> (() => new CGColorConversionInfo (from, to));
			}
		}

		[Test]
		public void CGColorSpace_CGColorSpace_NSDictionary ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var from = CGColorSpace.CreateGenericGray ())
			using (var to = CGColorSpace.CreateGenericRgb ()) {
				using (var converter = new CGColorConversionInfo (from, to, (NSDictionary) null)) {
					Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle - null");
				}
				using (var d = new NSDictionary ())
				using (var converter = new CGColorConversionInfo (from, to, d)) {
					Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				}
			}
		}

		[Test]
		public void CGColorSpace_CGColorSpace_CGColorConversionOptions ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var from = CGColorSpace.CreateGenericGray ())
			using (var to = CGColorSpace.CreateGenericRgb ()) {
				using (var converter = new CGColorConversionInfo (from, to, (CGColorConversionOptions) null)) {
					Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-null");
				}

				var o = new CGColorConversionOptions ();
				o.BlackPointCompensation = true;
				using (var converter = new CGColorConversionInfo (from, to, o)) {
					Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				}
			}
		}
	}
}
