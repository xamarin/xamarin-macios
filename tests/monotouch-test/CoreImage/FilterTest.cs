//
// Unit tests for CIFilter
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.
//

using System.IO;

using CoreGraphics;
using CoreImage;
using CoreText;
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
				filter.InputImage = input;
				filter.HighlightAmount = 0.75f;
				filter.ShadowAmount = 1.5f;
				// https://bugzilla.xamarin.com/show_bug.cgi?id=15465
				Assert.That (filter.OutputImage, Is.Not.Null, "OutputImage");
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
			Assert.That (filter, Is.Not.Null);
			filter.Input = 10;
			Assert.That (filter.Input, Is.EqualTo (10));
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public void ColorSpace ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			using (var f = new CIColorCubeWithColorSpace ()) {
				Assert.That (f.ColorSpace, Is.Null, "ColorSpace/default");
				using (var cs = CGColorSpace.CreateDeviceGray ()) {
					f.ColorSpace = cs;
					var rc = CFGetRetainCount (cs.Handle);
					for (int i = 0; i < 5; i++) {
						using (var fcs = f.ColorSpace)
							Assert.That (fcs, Is.Not.Null, i.ToString ());
					}
					Assert.That (CFGetRetainCount (cs.Handle), Is.EqualTo (rc), "RetainCount");
					f.ColorSpace = null;
				}
				Assert.That (f.ColorSpace, Is.Null, "ColorSpace/reset-null");
			}
		}

		[Test]
		public void CIBarcodeDescriptorTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var f = new CIBarcodeGenerator ()) {
				Assert.That (f.BarcodeDescriptor, Is.Null, "CIBarcodeDescriptor/default");
				using (var d = new NSData ())
				using (var b = new CIQRCodeDescriptor (d, 1, 0, CIQRCodeErrorCorrectionLevel.Q)) {
					f.BarcodeDescriptor = b;
					var rc = CFGetRetainCount (b.Handle);
					for (int i = 0; i < 5; i++)
						Assert.That (f.BarcodeDescriptor, Is.Not.Null, i.ToString ());
					Assert.That (CFGetRetainCount (b.Handle), Is.EqualTo (rc), "RetainCount");
					f.BarcodeDescriptor = null;
				}
				Assert.That (f.BarcodeDescriptor, Is.Null, "CIBarcodeDescriptor/reset-null");
			}
		}

		[Test]
		public void CIAttributedTextImageGenerator ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var f = new CIAttributedTextImageGenerator ()) {
				Assert.That (f.Text, Is.Null, "NSAttributedString/default");
				var attr = new CTStringAttributes () {
					ForegroundColorFromContext = true,
					Font = new CTFont ("Arial", 24)
				};
				using (var s = new NSAttributedString ("testString", attr)) {
					f.Text = s;
					Assert.That (f.Text, Is.Not.Null, "NSAttributedString/not-null");
				}
			}
		}

		[Test]
		public void CIVectorArray ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);

			using (var f = new CIMeshGenerator ()) {
				Assert.That (f.Mesh, Is.Null, "Mesh/Null");
				f.Mesh = new CIVector [1] { new CIVector (1) };
				Assert.That (f.Mesh.Length, Is.EqualTo (1), "Mesh/Non-null");
				f.Mesh = null;
				Assert.That (f.Mesh, Is.Null, "Mesh/Null/again");
			}
		}

		[Test]
		public void FilterNamesInCategories ()
		{
			var a = CIFilter.FilterNamesInCategories ();
			var b = CIFilter.FilterNamesInCategories (null);

			Assert.That (a, Is.EquivalentTo (b), "Filtering");
			Assert.That (a.Length, Is.GreaterThan (0), "Filtered A");
			Assert.That (b.Length, Is.GreaterThan (0), "Filtered B");
		}

		[TestCase (true)]
		[TestCase (false)]
		public void ConvertLab (bool labToRgb)
		{
			TestRuntime.AssertXcodeVersion (14, 0);

			var name = labToRgb ? "CIConvertLabToRGB" : "CIConvertRGBtoLab";
			using (CIFilter filter = labToRgb ? new CIConvertLabToRGB () : new CIConvertRGBtoLab ()) {
				Assert.That (filter.Name, Is.EqualTo (name), "Name");
				var conversion = (ICIConvertLabProtocol) filter;
				Assert.That (conversion.InputImage, Is.Null, "InputImage/default");
				conversion.Normalize = true;
				Assert.That (conversion.Normalize, Is.True, "Normalize/true");
				conversion.Normalize = false;
				Assert.That (conversion.Normalize, Is.False, "Normalize/false");

				using (var image = new CIImage (CIColor.White)) {
					conversion.InputImage = image;
					Assert.That (conversion.InputImage, Is.EqualTo (image), "InputImage");
					using (var output = conversion.OutputImage)
						Assert.That (output, Is.Not.Null, "OutputImage");
					conversion.InputImage = null;
					Assert.That (conversion.InputImage, Is.Null, "InputImage/reset-null");
				}

				using (var namedFilter = CIFilter.FromName (name))
					Assert.That (namedFilter, Is.TypeOf (labToRgb ? typeof (CIConvertLabToRGB) : typeof (CIConvertRGBtoLab)), "FromName");
			}
		}
	}
}
