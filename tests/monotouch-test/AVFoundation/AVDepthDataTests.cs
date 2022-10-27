//
// AVDepthDataTests.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using NUnit.Framework;

using AVFoundation;
using Foundation;
using ImageIO;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVDepthDataTests {

		[Test]
		public void AvailableDepthDataTypesTest ()
		{
#if !MONOMAC
			TestRuntime.AssertDevice ();
#endif
			TestRuntime.AssertXcodeVersion (9, 0);

			// xamarinmonkey.heic is the new photo format, also this one includes depth data
			var imgdata = NSData.FromUrl (NSBundle.MainBundle.GetUrlForResource ("xamarinmonkey", "heic", "CoreImage"));
			Assert.NotNull (imgdata, "imgdata");

			var imageSource = CGImageSource.FromData (imgdata);
			Assert.NotNull (imageSource, "imageSource");

			// fetching the image count works around a crash in CopyAuxiliaryDataInfo on macOS 10.15 (https://github.com/xamarin/maccore/issues/1802).
			Assert.AreNotEqual (0, imageSource.ImageCount, "ImageCount");

			var info = imageSource.CopyAuxiliaryDataInfo (0, CGImageAuxiliaryDataType.Disparity);
			Assert.NotNull (info, "info");

			NSError err;
			var depthData = AVDepthData.Create (info, out err);
			Assert.NotNull (depthData, "depthData");
			Assert.NotNull (depthData.AvailableDepthDataTypes, "AvailableDepthDataTypes");
		}
	}
}
#endif
