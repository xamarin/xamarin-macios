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
#if XAMCORE_2_0
using Foundation;
using CoreVideo;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreVideo;
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
	}
}

#endif // !__WATCHOS__
