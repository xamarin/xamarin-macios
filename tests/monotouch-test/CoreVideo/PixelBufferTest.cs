//
// Unit tests for CVPixelBuffer
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreVideo;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelBufferTest {
		[Test]
		public void CreateWithBytes ()
		{
			nint width = 1280;
			nint height = 720;
			nint bytesPerRow = width * 4;
			CVReturn status;

			var data = new byte [height * bytesPerRow];

			using (var buf = CVPixelBuffer.Create (width, height, CVPixelFormatType.CV32RGBA, data, bytesPerRow, null, out status)) {
				Assert.AreEqual (status, CVReturn.InvalidPixelFormat, "CV32RGBA");
				Assert.IsNull (buf, "CV32RGBA - null");
			}

			using (var buf = CVPixelBuffer.Create (width, height, CVPixelFormatType.CV32BGRA, data, bytesPerRow, null, out status)) {
				Assert.AreEqual (status, CVReturn.Success, "CV32RGBA");
				Assert.IsNotNull (buf, "CV32BGRA - null");
			}

			var dict = new CVPixelBufferAttributes ();
			using (var buf = CVPixelBuffer.Create (width, height, CVPixelFormatType.CV32BGRA, data, bytesPerRow, dict)) {
				Assert.IsNotNull (buf);
			}

			Assert.Throws<ArgumentNullException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV32BGRA, null, bytesPerRow, null), "null data");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV32BGRA, data, bytesPerRow + 1, null), "bytesPerRow+1");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height + 1, CVPixelFormatType.CV32BGRA, data, bytesPerRow + 1, null), "height+1");
		}

		[Test]
		public void CreateWithPlanarBytes ()
		{
			nint width = 1280;
			nint height = 720;
			nint [] planeWidths = new nint [] { width, width / 2 };
			nint [] planeHeights = new nint [] { height, height / 2 };
			nint [] planeBytesPerRow = new nint [] { width, width };
			CVReturn status;

			var data = new byte [] [] {
				new byte [planeHeights [0] * planeBytesPerRow [0]],
				new byte [planeHeights [1] * planeBytesPerRow [1]],
			};

			using (var buf = CVPixelBuffer.Create (width, height, CVPixelFormatType.CV32RGBA, data, planeWidths, planeHeights, planeBytesPerRow, null, out status)) {
				Assert.IsNull (buf);
				Assert.AreEqual (CVReturn.InvalidPixelFormat, status, "invalid status");
			}

			using (var buf = CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, planeHeights, planeBytesPerRow, null)) {
				Assert.IsNotNull (buf);
			}

			var dict = new CVPixelBufferAttributes ();
			using (var buf = CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, planeHeights, planeBytesPerRow, dict)) {
				Assert.IsNotNull (buf);
			}

			Assert.Throws<ArgumentNullException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, null, planeWidths, planeHeights, planeBytesPerRow, null), "null data");
			Assert.Throws<ArgumentNullException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, null, planeHeights, planeBytesPerRow, null), "null widths");
			Assert.Throws<ArgumentNullException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, null, planeBytesPerRow, null), "null heights");
			Assert.Throws<ArgumentNullException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, planeHeights, null, null), "null bytesPerRow");

			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, new nint [] { width }, planeHeights, planeBytesPerRow, null), "invalid widths a");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, new nint [] { width, width, width }, planeHeights, planeBytesPerRow, null), "invalid widths b");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, new nint [] { height }, planeBytesPerRow, null), "invalid heights a");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, new nint [] { height, height, height }, planeBytesPerRow, null), "invalid heights b");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, planeHeights, new nint [] { width }, null), "invalid bytesPerRow");
			Assert.Throws<ArgumentOutOfRangeException> (() => CVPixelBuffer.Create (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange, data, planeWidths, planeHeights, new nint [] { width, width, width }, null), "invalid bytesPerRow");
		}

		[Test]
		public void CheckInvalidPtr ()
		{
			var invalid = Runtime.GetINativeObject<CVPixelBuffer> (IntPtr.Zero, false);
			Assert.Null (invalid, "CheckInvalidPtr");
		}
	}
}

#endif // !__WATCHOS__
