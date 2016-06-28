//
// Unit tests for CGColorSpace
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using CoreGraphics;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
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

namespace MonoTouchFixtures.CoreGraphics {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ColorSpaceTest {
		
		void CheckUnknown (CGColorSpace cs)
		{
			Assert.That (cs.Components, Is.EqualTo ((nint) 0), "Unknown-0");
			Assert.That (cs.Handle, Is.EqualTo (IntPtr.Zero), "Unknown-Handle");
			Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.Unknown), "Unknown-Model");
			Assert.That (cs.GetColorTable ().Length, Is.EqualTo (0), "Unknown-GetColorTable");
		}

#if !XAMCORE_3_0
		[Test]
		public void Null ()
		{
			CheckUnknown (CGColorSpace.Null);
		}
#endif
		
		[Test]
		public void CreateDeviceGray ()
		{
			using (var cs = CGColorSpace.CreateDeviceGray ()) {
				Assert.That (cs.Components, Is.EqualTo ((nint) 1), "1");
				Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.Monochrome), "Monochrome");
				Assert.Null (cs.GetBaseColorSpace (), "GetBaseColorSpace");
				// not indexed so no color table
				Assert.That (cs.GetColorTable ().Length, Is.EqualTo (0), "GetColorTable");

				if (TestRuntime.CheckXcodeVersion (5, 0))
					Assert.Null (cs.GetICCProfile (), "GetICCProfile");
			}
		}

		[Test]
		public void CreateDeviceRGB ()
		{
			using (var cs = CGColorSpace.CreateDeviceRGB ()) {
				Assert.That (cs.Components, Is.EqualTo ((nint) 3), "3");
				Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.RGB), "RGB");
				Assert.Null (cs.GetBaseColorSpace (), "GetBaseColorSpace");
				// not indexed so no color table
				Assert.That (cs.GetColorTable ().Length, Is.EqualTo (0), "GetColorTable");

				if (TestRuntime.CheckXcodeVersion (5, 0))
					Assert.Null (cs.GetICCProfile (), "GetICCProfile");
			}
		}

		[Test]
		public void CreateDeviceCMYK ()
		{
			using (var cs = CGColorSpace.CreateDeviceCmyk ()) {
				Assert.That (cs.Components, Is.EqualTo ((nint) 4), "4");
				Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.CMYK), "CMYK");
				Assert.Null (cs.GetBaseColorSpace (), "GetBaseColorSpace");
				// not indexed so no color table
				Assert.That (cs.GetColorTable ().Length, Is.EqualTo (0), "GetColorTable");

				if (TestRuntime.CheckXcodeVersion (5, 0))
					Assert.Null (cs.GetICCProfile (), "GetICCProfile");
			}
		}

		[Test]
		public void CreateIndexed ()
		{
			// from: http://developer.apple.com/library/ios/#documentation/GraphicsImaging/Reference/CGColorSpace/Reference/reference.html
			//  m is the number of color components in the base color space
			nint m = 3; // RGB
			const int lastIndex = 2;
			// An array of m*(lastIndex+1) bytes
			byte[] table = new byte [3 * (lastIndex + 1)] { 1, 2, 3, 4, 5, 6, 255, 255, 255 };
			using (var base_cs = CGColorSpace.CreateDeviceRGB ())
			using (var cs = CGColorSpace.CreateIndexed (base_cs, lastIndex, table)) {
				Assert.That (cs.Components, Is.EqualTo ((nint) 1), "1");
				Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.Indexed), "Indexed");
				var bcs = cs.GetBaseColorSpace ();
				Assert.That (bcs.Components, Is.EqualTo (m), "Components");
				Assert.That (base_cs.Model, Is.EqualTo (bcs.Model), "GetBaseColorSpace");
				var new_table = cs.GetColorTable ();
				Assert.That (table, Is.EqualTo (new_table), "GetColorTable");

				if (TestRuntime.CheckXcodeVersion (5, 0))
					Assert.Null (cs.GetICCProfile (), "GetICCProfile");
			}
		}

		[Test]
		public void CreateICCProfile ()
		{
			// of all the .icc profiles I have on my Mac then only one I found working is
			// for my old 15" sharp (secondary) display. Added it to the test suite
			// that should work on the iOS simulator - at least some as I'm not sure every Mac
			// has the file(s) so we're not trying (and fialing) to copy it into the bundle
			using (var icc = NSData.FromFile (Path.Combine (NSBundle.MainBundle.ResourcePath, "LL-171A-B-B797E457-16AB-C708-1E0F-32C19DBD47B5.icc")))
			using (var cs = CGColorSpace.CreateICCProfile (icc)) {
				Assert.That (cs.Components, Is.EqualTo ((nint) 3), "Components");
				Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.RGB), "Model");
				Assert.Null (cs.GetBaseColorSpace (), "GetBaseColorSpace");
				// not indexed so no color table
				Assert.That (cs.GetColorTable ().Length, Is.EqualTo (0), "GetColorTable");

				if (TestRuntime.CheckXcodeVersion (5, 0)) {
					using (var icc_profile = cs.GetICCProfile ())
						Assert.That (icc_profile.Length, Is.EqualTo (3284), "GetICCProfile");
				}
			}
		}

		void CheckIndexedFile (CGImage img)
		{
			CGColorSpace cs = img.ColorSpace;
			Assert.That (cs.Components, Is.EqualTo ((nint) 1), "Components");
			Assert.That (cs.Model, Is.EqualTo (CGColorSpaceModel.Indexed), "GetBaseColorSpace");
			var table = cs.GetColorTable ();
			Assert.That (table.Length, Is.EqualTo (768), "GetColorTable");
			cs.Dispose ();
		}
		
		[Test]
		public void Indexed_UIImage ()
		{
			// downloaded from http://www.schaik.com/pngsuite/#palette
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var img = UIImage.FromFile (file)) {
				using (var cgimg = img.CGImage)
					CheckIndexedFile (cgimg);
			}
		}

		[Test]
		public void Indexed_Provider ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var dp = new CGDataProvider (file)) {
				using (var img = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default)) {
					CheckIndexedFile (img);
				}
			}
		}
	}
}
