//
// Unit tests for VTMultiPassStorage
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using System.Runtime.InteropServices;

using Foundation;
using VideoToolbox;
using CoreMedia;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using AVFoundation;
using CoreFoundation;
using CoreVideo;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTUtilitiesTests {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static int CFGetRetainCount (IntPtr handle);

		[Test]
		public void ToCGImageTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 9, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 2, throwIfOtherPlatform: false);

#if MONOMAC
			var originalImage = new NSImage (NSBundle.MainBundle.PathForResource ("Xam", "png", "CoreImage"));
#else
			var originalImage = UIImage.FromBundle ("CoreImage/Xam.png");
#endif
			var originalCGImage = originalImage.CGImage;

			var pxbuffer = new CVPixelBuffer (originalCGImage.Width, originalCGImage.Height, CVPixelFormatType.CV32ARGB,
							   new CVPixelBufferAttributes { CGImageCompatibility = true, CGBitmapContextCompatibility = true });
#if !XAMCORE_3_0
			pxbuffer.Lock (CVOptionFlags.None);
#else
			pxbuffer.Lock (CVPixelBufferLock.None);
#endif
			using (var colorSpace = CGColorSpace.CreateDeviceRGB ())
			using (var ctx = new CGBitmapContext (pxbuffer.BaseAddress, originalCGImage.Width, originalCGImage.Height, 8,
								 4 * originalCGImage.Width, colorSpace, CGBitmapFlags.NoneSkipLast)) {
				ctx.RotateCTM (0);
				ctx.DrawImage (new CGRect (0, 0, originalCGImage.Width, originalCGImage.Height), originalCGImage);
#if !XAMCORE_3_0
				pxbuffer.Unlock (CVOptionFlags.None);
#else
				pxbuffer.Unlock (CVPixelBufferLock.None);
#endif
			}

			Assert.NotNull (pxbuffer, "VTUtilitiesTests.ToCGImageTest pxbuffer should not be null");

			CGImage newImage;
			var newImageStatus = pxbuffer.ToCGImage (out newImage);

			Assert.That (newImageStatus == VTStatus.Ok, "VTUtilitiesTests.ToCGImageTest must be ok");
			Assert.NotNull (newImage, "VTUtilitiesTests.ToCGImageTest pxbuffer should not be newImage");
			Assert.AreEqual (originalCGImage.Width, newImage.Width, "VTUtilitiesTests.ToCGImageTest");
			Assert.AreEqual (originalCGImage.Height, newImage.Height, "VTUtilitiesTests.ToCGImageTest");

			var retainCount = CFGetRetainCount (newImage.Handle);
			Assert.That (retainCount, Is.EqualTo (1), "RetainCount");
		}

#if MONOMAC

		[TestCase (CMVideoCodecType.YUV422YpCbCr8)]
		[TestCase (CMVideoCodecType.Animation)]
		[TestCase (CMVideoCodecType.Cinepak)]
		[TestCase (CMVideoCodecType.JPEG)]
		[TestCase (CMVideoCodecType.JPEG_OpenDML)]
		[TestCase (CMVideoCodecType.SorensonVideo)]
		[TestCase (CMVideoCodecType.SorensonVideo3)]
		[TestCase (CMVideoCodecType.H263)]
		[TestCase (CMVideoCodecType.H264)]
		[TestCase (CMVideoCodecType.Mpeg4Video)]
		[TestCase (CMVideoCodecType.Mpeg2Video)]
		[TestCase (CMVideoCodecType.Mpeg1Video)]
		[TestCase (CMVideoCodecType.VP9)]
		[TestCase (CMVideoCodecType.DvcNtsc)]
		[TestCase (CMVideoCodecType.DvcPal)]
		[TestCase (CMVideoCodecType.DvcProPal)]
		[TestCase (CMVideoCodecType.DvcPro50NTSC)]
		[TestCase (CMVideoCodecType.DvcPro50PAL)]
		[TestCase (CMVideoCodecType.DvcProHD720p60)]
		[TestCase (CMVideoCodecType.DvcProHD720p50)]
		[TestCase (CMVideoCodecType.DvcProHD1080i60)]
		[TestCase (CMVideoCodecType.DvcProHD1080i50)]
		[TestCase (CMVideoCodecType.DvcProHD1080p30)]
		[TestCase (CMVideoCodecType.DvcProHD1080p25)]
		[TestCase (CMVideoCodecType.AppleProRes4444)]
		[TestCase (CMVideoCodecType.AppleProRes422HQ)]
		[TestCase (CMVideoCodecType.AppleProRes422)]
		[TestCase (CMVideoCodecType.AppleProRes422LT)]
		[TestCase (CMVideoCodecType.AppleProRes422Proxy)]
		[TestCase (CMVideoCodecType.Hevc)]
		public void TestRegisterSupplementalVideoDecoder (CMVideoCodecType codec)
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			// ensure that the call does not crash, we do not have anyother thing to test since there is 
			// no way to know if it was a success
			VTUtilities.RegisterSupplementalVideoDecoder (codec);
		}
#endif

	}
}

#endif // !__WATCHOS__
