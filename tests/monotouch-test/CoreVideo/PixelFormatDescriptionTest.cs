//
// Unit tests for CVPixelFormatDescription
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using System.Text;

using Foundation;
using CoreGraphics;
using CoreVideo;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelFormatDescriptionTest {

		[Test]
		public void AllTypes ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=13917
			Assert.NotNull (CVPixelFormatDescription.AllTypes);
		}

		[Test]
		public void Create ()
		{
			// 0 is not defined
			Assert.Null (CVPixelFormatDescription.Create (0), "0");

			using (var dict = CVPixelFormatDescription.Create (CVPixelFormatType.CV16Gray)) {
				Assert.NotNull (dict, "CV16Gray");
			}

			using (var dict = CVPixelFormatDescription.Create (CVPixelFormatType.CV32ARGB)) {
				Assert.NotNull (dict, "CV32ARGB");
			}
		}

		static bool registerDone;
		[Test]
		public void Register ()
		{
			if (registerDone)
				Assert.Ignore ("This test can only be executed once, it modifies global state.");
			registerDone = true;

			Assert.Null (CVPixelFormatDescription.Create ((CVPixelFormatType) 3), "3a");

			using (var dict = CVPixelFormatDescription.Create (CVPixelFormatType.CV24RGB)) {
				Assert.NotNull (dict, "CV24RGB");
				CVPixelFormatDescription.Register (dict, (CVPixelFormatType) 3);
			}

			Assert.NotNull (CVPixelFormatDescription.Create ((CVPixelFormatType) 3), "3b");
		}

#if NET
		[Test]
		public void CV32ARGB ()
		{
			Assert.Multiple (() => {
				var pf = CVPixelFormatType.CV32ARGB;
				var desc = CVPixelFormatDescription.CreatePixelFormat (pf);
				Assert.IsNull (desc.Name, "Name");
				Assert.AreEqual (pf, desc.Constant ?? ((CVPixelFormatType) 0xFFFFFFFF), "Constant");
				Assert.IsNull (desc.CodecType, "CodecType");
				Assert.IsNull (desc.FourCC, "FourCC");
				Assert.AreEqual (true, desc.ContainsAlpha, "ContainsAlpha");
				Assert.AreEqual (false, desc.FormatContainsYCbCr, "FormatContainsYCbCr");
				Assert.AreEqual (true, desc.FormatContainsRgb, "FormatContainsRgb");
				Assert.AreEqual (false, desc.ContainsGrayscale, "ContainsGrayscale");
				Assert.IsNull (desc.FormatContainsSenselArray, "FormatContainsSenselArray");
				Assert.IsNull (desc.ComponentRange, "ComponentRange");
				Assert.IsNull (desc.Planes, "Planes");
				Assert.IsNull (desc.BlockWidth, "BlockWidth");
				Assert.IsNull (desc.BlockHeight, "BlockHeight");
				Assert.AreEqual (32, desc.BitsPerBlock, "BitsPerBlock");
				Assert.IsNull (desc.BlockHorizontalAlignment, "BlockHorizontalAlignment");
				Assert.IsNull (desc.BlockVerticalAlignment, "BlockVerticalAlignment");
				Assert.IsNotNull (desc.BlackBlock, "BlackBlock");
				Assert.IsNull (desc.HorizontalSubsampling, "HorizontalSubsampling");
				Assert.IsNull (desc.VerticalSubsampling, "VerticalSubsampling");
#if (__IOS__ && !__MACCATALYST__) || __TVOS__
				Assert.IsNull (desc.OpenGLFormat, "OpenGLFormat");
				Assert.IsNull (desc.OpenGLType, "OpenGLType");
				Assert.IsNull (desc.OpenGLInternalFormat, "OpenGLInternalFormat");
				Assert.IsNull (desc.OpenGLCompatibility, "OpenGLCompatibility");
#else
				Assert.AreEqual (32993, desc.OpenGLFormat, "OpenGLFormat");
				Assert.AreEqual (32821, desc.OpenGLType, "OpenGLType");
				Assert.AreEqual (32856, desc.OpenGLInternalFormat, "OpenGLInternalFormat");
				Assert.AreEqual (true, desc.OpenGLCompatibility, "OpenGLCompatibility");
#endif
				Assert.AreEqual (CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.First, desc.CGBitmapInfo, "CGBitmapInfo");
				Assert.AreEqual (true, desc.QDCompatibility, "QDCompatibility");
				Assert.AreEqual (true, desc.CGBitmapContextCompatibility, "CGBitmapContextCompatibility");
				Assert.AreEqual (true, desc.CGImageCompatibility, "CGImageCompatibility");
				Assert.IsNotNull (desc.FillExtendedPixelsCallback, "FillExtendedPixelsCallback");
				Assert.IsNotNull (desc.FillExtendedPixelsCallbackStruct, "FillExtendedPixelsCallbackStruct");
			});
		}
#endif // NET
	}
}

#endif // !__WATCHOS__
