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
using CoreGraphics;
using CoreImage;
using Foundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
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
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Inconclusive ("Custom filters require iOS8+");

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
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("Ignoring ColorSpace test: CIColorCubeWithColorSpace requires iOS7+");

			using (var f = new CIColorCubeWithColorSpace ()) {
				Assert.Null (f.ColorSpace, "ColorSpace/default");
				using (var cs = CGColorSpace.CreateDeviceGray ()) {
					f.ColorSpace = cs;
					var rc = CFGetRetainCount (cs.Handle);
					for (int i = 0; i < 5; i++)
						Assert.NotNull (f.ColorSpace, i.ToString ());
					Assert.That (CFGetRetainCount (cs.Handle), Is.EqualTo (rc), "RetainCount");
					f.ColorSpace = null;
				}
				Assert.Null (f.ColorSpace, "ColorSpace/reset-null");
			}
		}
	}
}

#endif // !__WATCHOS__
