//
// Unit tests for CMFormatDescription
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2014 Xamarin Inc All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Collections.Generic;
#if XAMCORE_2_0
using Foundation;
using CoreMedia;
using AVFoundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.CoreMedia;
using MonoTouch.AVFoundation;
using MonoTouch.UIKit;
using nuint = global::System.UInt32;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMFormatDescriptionTest
	{
		[Test]
		public void ClosedCaption ()
		{
			CMFormatDescriptionError fde;
			using (var fd = CMFormatDescription.Create (CMMediaType.ClosedCaption, (uint) CMClosedCaptionFormatType.CEA608, out fde))
			{
				Assert.AreEqual (CMFormatDescriptionError.None, fde, "#1");
				Assert.AreEqual ((CMMuxedStreamType) 0, fd.MuxedStreamType, "#2");
				Assert.AreEqual (CMMediaType.ClosedCaption, fd.MediaType, "#3");
				Assert.AreEqual (CMClosedCaptionFormatType.CEA608, fd.ClosedCaptionFormatType, "#4");
			}
		}


#if !__TVOS__ && !MONOMAC // GetAuthorizationStatus is not available on mac
		[Test]
		public void Video ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("This test fails on iOS 6 (even though the API exists).");

			CMFormatDescriptionError fde;

			var auth = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);
			switch (auth) {
			case AVAuthorizationStatus.Restricted:
			case AVAuthorizationStatus.Denied:
			case AVAuthorizationStatus.NotDetermined:
				// We can't test the below, since the some other tests may have initialized whatever we need for the API to work correctly.
//				Assert.Null (CMFormatDescription.Create (CMMediaType.Video, (uint) CMVideoCodecType.H264, out fde), "null ({0})", auth);
//				Assert.That (fde, Is.EqualTo (CMFormatDescriptionError.InvalidParameter), "CMFormatDescriptionError");
				break;
			case AVAuthorizationStatus.Authorized:
				// We can't test the below, since the some other tests may have initialized whatever we need for the API to work correctly.
//				Assert.Null (CMFormatDescription.Create (CMMediaType.Video, (uint) CMVideoCodecType.H264, out fde), "null (authorized)");
//				Assert.That (fde, Is.EqualTo (CMFormatDescriptionError.InvalidParameter), "CMFormatDescriptionError (authorized)");

				using (var captureSession = new AVCaptureSession ()) {
					using (var videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video)) {
						NSError error;
						using (var videoInput = new AVCaptureDeviceInput (videoDevice, out error)) {
							// this seems to initialize something.
						}
					}
				}

				Assert.IsNotNull (CMFormatDescription.Create (CMMediaType.Video, (uint) CMVideoCodecType.H264, out fde), "not null (authorized)");
				Assert.That (fde, Is.EqualTo (CMFormatDescriptionError.None), "CMFormatDescriptionError #2 (authorized)");
				break;
			}
		}

		[Test]
		public void RefcountTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("This test uses iOS 7 API");
			
			// Bug #27205

			var auth = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);
			switch (auth) {
			case AVAuthorizationStatus.Restricted:
			case AVAuthorizationStatus.Denied:
			case AVAuthorizationStatus.NotDetermined:
				Assert.Inconclusive ("This test requires video recording permissions.");
				return;
			}

			using (var captureSession = new AVCaptureSession ()) {
				using (var videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video)) {
					foreach (var format in videoDevice.Formats) {
						for (int i = 0; i < 10; i++) {
							using (var f = format.FormatDescription) {
							}
						}
					}
				}
			}
		}
#endif // !__TVOS__

		[Test]
		public void H264ParameterSetsTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (5, 0, 1))
				Assert.Inconclusive ("CMVideoFormatDescription.FromH264ParameterSets is iOS7+ and macOS 10.9+");
			
			var arr0 = new byte[] { 0x67, 0x64, 0x00, 0x29, 0xAC, 0x56, 0x80, 0x78, 0x02, 0x27, 0xE5, 0x9A, 0x80, 0x80, 0x80, 0x81 };
			var arr1 = new byte[] { 0x28, 0xEE, 0x04, 0xF2, 0xC0 };

			var props = new List<byte[]> { arr0, arr1 };
			CMFormatDescriptionError error;
			var desc = CMVideoFormatDescription.FromH264ParameterSets (props, 4, out error);
			props = null;
			Assert.That (error == CMFormatDescriptionError.None, "H264ParameterSetsTest");
			Assert.NotNull (desc, "H264ParameterSetsTest");
			Assert.That (desc.Dimensions.Height == 1080 && desc.Dimensions.Width == 1920, "H264ParameterSetsTest");

			CMFormatDescriptionError err;
			nuint paramCount;
			int nalCount;
			var bytes = desc.GetH264ParameterSet (0, out paramCount, out nalCount, out err);
			Assert.That (err == CMFormatDescriptionError.None, "H264ParameterSetsTest");
			Assert.NotNull (bytes, "H264ParameterSetsTest");
			Assert.True (nalCount == 4 && paramCount == 2);
			Assert.That (arr0, Is.EqualTo (bytes), "H264ParameterSetsTest roundtrip");

			bytes = desc.GetH264ParameterSet (1, out paramCount, out nalCount, out err);
			Assert.That (err == CMFormatDescriptionError.None, "H264ParameterSetsTest");
			Assert.NotNull (bytes, "H264ParameterSetsTest");
			Assert.True (nalCount == 4 && paramCount == 2);
			Assert.That (arr1, Is.EqualTo (bytes), "H264ParameterSetsTest roundtrip");
		}

		[Test]
		public void VideoFormatDescriptionConstructors ()
		{
#if __UNIFIED__
			using (var obj = new CMVideoFormatDescription (CMVideoCodecType.H264, new CMVideoDimensions (960, 540))) {
			}
#else
			using (var obj = new CMVideoFormatDescription (CMVideoCodecType.H264, new System.Drawing.Size (960, 540))) {
			}
#endif
		}
	}
}

#endif // !__WATCHOS__
