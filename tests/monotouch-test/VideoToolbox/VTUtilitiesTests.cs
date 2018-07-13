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

#if XAMCORE_2_0
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
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.VideoToolbox;
using MonoTouch.UIKit;
using MonoTouch.CoreMedia;
using MonoTouch.AVFoundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreGraphics;
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
				ctx.DrawImage (new RectangleF (0, 0, originalCGImage.Width, originalCGImage.Height), originalCGImage);
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
