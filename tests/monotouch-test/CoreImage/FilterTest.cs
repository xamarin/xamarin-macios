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

using CoreGraphics;
using CoreImage;
using CoreText;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

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
#if NET
				filter.InputImage = input;
#else
				filter.Image = input;
#endif
				filter.HighlightAmount = 0.75f;
				filter.ShadowAmount = 1.5f;
				// https://bugzilla.xamarin.com/show_bug.cgi?id=15465
				Assert.NotNull (filter.OutputImage, "OutputImage");
			}
		}

		class MyFilter : CIFilter {
			public int Input { get; set; }
		}

		[Test]
		public void CustomFilterTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			MyFilter filter = new MyFilter ();
			Assert.NotNull (filter);
			filter.Input = 10;
			Assert.AreEqual (10, filter.Input);
		}

#if !NET
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
#endif // !NET

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public void ColorSpace ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

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

		[Test]
		public void CIVectorArray ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);

			using (var f = new CIMeshGenerator ()) {
				Assert.Null (f.Mesh, "Mesh/Null");
				f.Mesh = new CIVector [1] { new CIVector (1) };
				Assert.That (f.Mesh.Length, Is.EqualTo (1), "Mesh/Non-null");
				f.Mesh = null;
				Assert.Null (f.Mesh, "Mesh/Null/again");
			}
		}
	}
}

#endif // !__WATCHOS__
