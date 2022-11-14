//
// Unit tests for CIImage
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreImage;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreImage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImageTest {

		[Test]
		public void EmptyImage ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			Assert.IsNull (CIImage.EmptyImage.Properties);
		}

		[Test]
		public void InitializationWithCustomMetadata ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var dp = new CGDataProvider (file)) {
				using (var img = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default)) {

					var opt = new CIImageInitializationOptionsWithMetadata () {
						Properties = new CGImageProperties () {
							ProfileName = "test profile name"
						}
					};

					using (var ci = new CIImage (img, opt)) {
						Assert.AreEqual ("test profile name", ci.Properties.ProfileName);
					}
				}
			}
		}

#if !MONOMAC // Testing an issue with UIImage.CIImage, iOS specific (NSImage has no CIImage property)
		[Test]
		public void UIImageInterop ()
		{
			// to validate http://stackoverflow.com/q/14464154/220643
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var url = NSUrl.FromFilename (file))
			using (var ci = CIImage.FromUrl (url))
			using (var ui = new UIImage (ci, 1.0f, UIImageOrientation.Up)) {
				Assert.IsNotNull (ui.CIImage, "CIImage");
			}
		}
#endif

		[Test]
		public void AreaHistogram ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			// validate that a null NSDictionary is correct (i.e. uses filter defaults)
			using (var h = CIImage.EmptyImage.CreateByFiltering ("CIAreaHistogram", null)) {
				// broken on simulator/64 bits on iOS9 beta 2 - radar 21564256 -> fixed in beta 4
				var success = true;
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 11))
					success = false;
#endif
				if (success) {
					Assert.That (h.Extent.Height, Is.EqualTo ((nfloat) 1), "Height");
				} else {
					Assert.IsNull (h, "Image");
				}
			}
		}

		[Test]
		public void CIImageColorSpaceTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Inconclusive ("requires iOS9+");

			using (var cgimage = new CIImage (NSBundle.MainBundle.GetUrlForResource ("xamarin1", "png")))
			using (var cs = cgimage.ColorSpace) {
				Assert.NotNull (cs, "ColorSpace");
				Assert.That (cs.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}

#endif // !__WATCHOS__
