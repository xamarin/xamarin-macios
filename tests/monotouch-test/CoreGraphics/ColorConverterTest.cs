//
// Unit tests for ColorConverterTest
//
// Authors:
//	Vincent Dondain <vincent@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using CoreGraphics;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics
{
	[TestFixture]
	public class ColorConverterTest
	{
		[Test]
		public void CreateNone ()
		{
			TestRuntime.AssertXcodeVersion (7, 3);

			Assert.Throws<ArgumentNullException> (() => new CGColorConverter (null, (CGColorSpace) null), "null");
			Assert.Throws<ArgumentNullException> (() => new CGColorConverter (null, (CGColorConverterTriple[]) null), "null-2");
			Assert.Throws<ArgumentNullException> (() => new CGColorConverter (null, new CGColorConverterTriple [0]), "empty");
		}

		[Test]
		public void CreateSingle ()
		{
			TestRuntime.AssertXcodeVersion (7, 3);

			var triple = new CGColorConverterTriple () {
				Space = CGColorSpace.CreateGenericRgb (),
				Intent = CGColorRenderingIntent.Default,
				Transform = CGColorConverterTransformType.ApplySpace
			};

			using (var converter = new CGColorConverter (null, triple)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CreateDual ()
		{
			TestRuntime.AssertXcodeVersion (7, 3);

			var triple = new CGColorConverterTriple () {
				Space = CGColorSpace.CreateGenericRgb (),
				Intent = CGColorRenderingIntent.Default,
				Transform = CGColorConverterTransformType.ApplySpace
			};

			using (var converter = new CGColorConverter (null, triple, triple)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CreateMax ()
		{
			TestRuntime.AssertXcodeVersion (7, 3);

			var first = new CGColorConverterTriple () {
				Space = CGColorSpace.CreateGenericRgb (),
				Intent = CGColorRenderingIntent.Default,
				Transform = CGColorConverterTransformType.ApplySpace
			};
			var second = new CGColorConverterTriple () {
				Space = CGColorSpace.CreateGenericGray (),
				Intent = CGColorRenderingIntent.Perceptual,
				Transform = CGColorConverterTransformType.FromSpace
			};
			var third = new CGColorConverterTriple () {
				Space = CGColorSpace.CreateGenericXyz (),
				Intent = CGColorRenderingIntent.Saturation,
				Transform = CGColorConverterTransformType.ToSpace
			};

			using (var converter = new CGColorConverter (null, first, first, first)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CreateTooMany ()
		{
			TestRuntime.AssertXcodeVersion (7, 3);
			Assert.Throws<ArgumentException> (() => new CGColorConverter (null, new CGColorConverterTriple [4]));
		}

		[Test]
		public void CreateSimple ()
		{
			TestRuntime.AssertXcodeVersion (8,0);

			using (var from = CGColorSpace.CreateGenericGray ())
			using (var to = CGColorSpace.CreateGenericRgb ())
			using (var converter = new CGColorConverter (from, to)) {
				Assert.That (converter.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorConverterCreateSimple (IntPtr from, IntPtr to);

		[Test]
		public void CreateSimple_GetINativeObject ()
		{
			TestRuntime.AssertXcodeVersion (8,0);

			using (var from = CGColorSpace.CreateGenericGray ())
			using (var to = CGColorSpace.CreateGenericRgb ()) {
				var handle = CGColorConverterCreateSimple (from == null ? IntPtr.Zero : from.Handle,
				                                           to == null ? IntPtr.Zero : to.Handle);
				using (var o = Runtime.GetINativeObject<CGColorConverter> (handle, false)) {
					Assert.That (o.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				}
			}
		}

		[Test]
		public void CreateSimple_DeviceColorSpace ()
		{
			TestRuntime.AssertXcodeVersion (8,0);

			// Requirements: CG color spaces must be calibrated
			// (no Device{Gray,RGB,CMYK}, Indexed or DeviceN).
			// This test lets us know if Apple changes that behavior.
			using (var from = CGColorSpace.CreateDeviceGray ())
			using (var to = CGColorSpace.CreateDeviceRGB ()) {
				Assert.Throws<Exception> (() => new CGColorConverter (from, to));
			}
		}
	}
}

