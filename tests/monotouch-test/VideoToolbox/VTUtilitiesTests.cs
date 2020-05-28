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
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreMedia;
using AVFoundation;
using CoreFoundation;
using CoreVideo;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTUtilitiesTests {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static int CFGetRetainCount (IntPtr handle);

		[Test]
		public void ToCGImageTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 9, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 11, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.TvOS, 10, 2, throwIfOtherPlatform: false);

#if MONOMAC
			var originalImage = new NSImage (NSBundle.MainBundle.PathForResource ("Xam", "png", "CoreImage"));
#else
			var originalImage = UIImage.FromBundle ("CoreImage/Xam.png");
#endif
			var originalCGImage = originalImage.CGImage;

			var pxbuffer = new CVPixelBuffer (originalCGImage.Width, originalCGImage.Height, CVPixelFormatType.CV32ARGB,
				               new CVPixelBufferAttributes { CGImageCompatibility = true, CGBitmapContextCompatibility = true });
#if !__TVOS__
			pxbuffer.Lock (CVOptionFlags.None);
#else
			pxbuffer.Lock (CVPixelBufferLock.None);
#endif
			using (var colorSpace = CGColorSpace.CreateDeviceRGB ())
			using (var ctx = new CGBitmapContext (pxbuffer.BaseAddress, originalCGImage.Width, originalCGImage.Height, 8, 
				                 4 * originalCGImage.Width, colorSpace, CGBitmapFlags.NoneSkipLast)) {
				ctx.RotateCTM (0);
				ctx.DrawImage (new CGRect (0, 0, originalCGImage.Width, originalCGImage.Height), originalCGImage);
#if !__TVOS__
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
	}
}

#endif // !__WATCHOS__
