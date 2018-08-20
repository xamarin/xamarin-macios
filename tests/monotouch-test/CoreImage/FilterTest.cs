//
// Unit tests for CIFilter
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreText;
using Foundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.AVFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
using MonoTouch.CoreText;
using MonoTouch.Foundation;
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

namespace MonoTouchFixtures.CoreImage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FilterTest {

		[Test]
		public void HighlightShadowAdjust ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var url = NSUrl.FromFilename (file))
			using (var input = CIImage.FromUrl (url))
			using (var filter = new CIHighlightShadowAdjust ()) {
				filter.Image = input;
				filter.HighlightAmount = 0.75f;
				filter.ShadowAmount = 1.5f;
				// https://bugzilla.xamarin.com/show_bug.cgi?id=15465
				Assert.NotNull (filter.OutputImage, "OutputImage");
			}
		}

		class MyFilter : CIFilter
		{
			public int Input { get; set; }
		}

		[Test]
		public void CustomFilterTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 11, throwIfOtherPlatform: false);

			MyFilter filter = new MyFilter ();
			Assert.NotNull (filter);
			filter.Input = 10;
			Assert.AreEqual(10, filter.Input);
		}

		[Test]
		public void UnsupportedInputImage ()
		{
			// some filters do not support inputImage (which we bound to the Image property)
			using (var filter = new CICheckerboardGenerator ()) {
				// but if we call ObjC then we get a native exception and crash on devices
				Assert.False (filter.RespondsToSelector (new Selector ("inputImage")), "inputImage");
				// so we return null in those cases
				Assert.Null (filter.Image, "Image");
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public void ColorSpace ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);

			using (var f = new CIColorCubeWithColorSpace ()) {
				Assert.Null (f.ColorSpace, "ColorSpace/default");
				using (var cs = CGColorSpace.CreateDeviceGray ()) {
					f.ColorSpace = cs;
					var rc = CFGetRetainCount (cs.Handle);
					for (int i = 0; i < 5; i++) {
						using (var fcs = f.ColorSpace)
							Assert.NotNull (fcs, i.ToString ());
					}
					Assert.That (CFGetRetainCount (cs.Handle), Is.EqualTo (rc), "RetainCount");
					f.ColorSpace = null;
				}
				Assert.Null (f.ColorSpace, "ColorSpace/reset-null");
			}
		}

		[Test]
		public void CIBarcodeDescriptorTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var f = new CIBarcodeGenerator ()) {
				Assert.Null (f.BarcodeDescriptor, "CIBarcodeDescriptor/default");
				using (var d = new NSData ())
				using (var b = new CIQRCodeDescriptor (d, 1, 0, CIQRCodeErrorCorrectionLevel.Q)) {
					f.BarcodeDescriptor = b;
					var rc = CFGetRetainCount (b.Handle);
					for (int i = 0; i < 5; i++)
						Assert.NotNull (f.BarcodeDescriptor, i.ToString ());
					Assert.That (CFGetRetainCount (b.Handle), Is.EqualTo (rc), "RetainCount");
					f.BarcodeDescriptor = null;
				}
				Assert.Null (f.BarcodeDescriptor, "CIBarcodeDescriptor/reset-null");
			}
		}

		[Test]
		public void CIAttributedTextImageGenerator ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var f = new CIAttributedTextImageGenerator ()) {
				Assert.Null (f.Text, "NSAttributedString/default");
				var attr = new CTStringAttributes () {
					ForegroundColorFromContext = true,
					Font = new CTFont ("Arial", 24)
				};
				using (var s = new NSAttributedString ("testString", attr)) {
					f.Text = s;
					Assert.NotNull (f.Text, "NSAttributedString/not-null");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
