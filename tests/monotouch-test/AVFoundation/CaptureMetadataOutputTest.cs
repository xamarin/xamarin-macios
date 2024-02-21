//
// Unit tests for AVMetadataObject
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Reflection;
using CoreGraphics;
using Foundation;
using AVFoundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CaptureMetadataOutputTest {

		[Test]
		public void Defaults ()
		{
			using (var obj = new AVCaptureMetadataOutput ()) {
				Assert.AreEqual (AVMetadataObjectType.None, obj.AvailableMetadataObjectTypes, "AvailableMetadataObjectTypes");
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");

				Assert.IsNotNull (obj.WeakAvailableMetadataObjectTypes, "WeakAvailableMetadataObjectTypes");
				Assert.AreEqual (0, obj.WeakAvailableMetadataObjectTypes.Length, "WeakAvailableMetadataObjectTypes#");
				Assert.IsNotNull (obj.WeakMetadataObjectTypes, "WeakMetadataObjectTypes");
				Assert.AreEqual (0, obj.WeakMetadataObjectTypes.Length, "WeakMetadataObjectTypes#");
				if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false))
					Assert.AreEqual (new CGRect (0, 0, 1, 1), obj.RectOfInterest, "RectOfInterest");

#if !__MACCATALYST__ // https://github.com/xamarin/maccore/issues/2345
				obj.WeakMetadataObjectTypes = null;
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");
				obj.MetadataObjectTypes = AVMetadataObjectType.None;
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");
				obj.SetDelegate (null, null);
#endif // !__MACCATALYST__
			}
		}

		[Test]
		public void Flags ()
		{
			// single
			var flags = AVMetadataObjectType.Face;
			var result = AVMetadataObjectTypeExtensions.ToFlags (new NSString [] { flags.GetConstant () });
			Assert.AreEqual (flags, result, "a2e 1");

			var back = result.ToArray ();
			Assert.That (back.Length, Is.EqualTo (1), "l 1");
			Assert.That (back [0], Is.EqualTo (flags.GetConstant ()), "e2a 1");

			// constants are only available in recent xcode (and not on any 32bits OS)
			if (TestRuntime.CheckXcodeVersion (11, 0)) {
				// multiple (flags)
				flags = AVMetadataObjectType.CatBody | AVMetadataObjectType.DogBody | AVMetadataObjectType.HumanBody;
				var array = new NSString [] {
					AVMetadataObjectType.CatBody.GetConstant (),
					AVMetadataObjectType.DogBody.GetConstant (),
					AVMetadataObjectType.HumanBody.GetConstant ()
				};
				result = AVMetadataObjectTypeExtensions.ToFlags (array);
				Assert.AreEqual (flags, result, "a2e 3");
				back = result.ToArray ();
				Assert.That (back.Length, Is.EqualTo (3), "l 3");
				Assert.That (back [0], Is.EqualTo (array [0]), "e2a 3a");
				Assert.That (back [1], Is.EqualTo (array [1]), "e2a 3b");
				Assert.That (back [2], Is.EqualTo (array [2]), "e2a 3c");
			}

			var all = (AVMetadataObjectType) ulong.MaxValue;
			var someArray = all.ToArray (); // converting all flags to an array will only return strings for flags that exist in the current OS.
			Assert.That (someArray.Length, Is.GreaterThan (1), "some back");
			var someFlags = AVMetadataObjectTypeExtensions.ToFlags (someArray);
			Assert.That (someFlags, Is.Not.EqualTo (AVMetadataObjectType.None), "Some, but not None");
			Assert.That (someFlags, Is.Not.EqualTo (all), "Some, but not all");
		}

		[Test]
		public void MetadataObjectTypesTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertDevice ("This test only runs on device (requires camera access)");
			TestRuntime.RequestCameraPermission (AVMediaTypes.Video.GetConstant (), true);

			using (var captureSession = new AVCaptureSession ()) {
				using (var videoDevice = AVCaptureDevice.GetDefaultDevice (AVMediaTypes.Video.GetConstant ())) {

					NSError error;
					using (var videoInput = new AVCaptureDeviceInput (videoDevice, out error)) {
						if (captureSession.CanAddInput (videoInput))
							captureSession.AddInput (videoInput);

						using (var metadataOutput = new AVCaptureMetadataOutput ()) {

							if (captureSession.CanAddOutput (metadataOutput))
								captureSession.AddOutput (metadataOutput);

							AVMetadataObjectType all = AVMetadataObjectType.None;
#if NET
							foreach (var val in Enum.GetValues<AVMetadataObjectType> ()) {
#else
							foreach (AVMetadataObjectType val in Enum.GetValues (typeof (AVMetadataObjectType))) {
#endif
								switch (val) {
								case AVMetadataObjectType.CatBody:
								case AVMetadataObjectType.DogBody:
								case AVMetadataObjectType.HumanBody:
								case AVMetadataObjectType.SalientObject:
									// fail *and crash* on iOS 8 (at least on 32bits devices)
									if (!TestRuntime.CheckXcodeVersion (11, 0))
										continue;
									// xcode 12 beta 1 on device
									if (TestRuntime.IsDevice && TestRuntime.CheckXcodeVersion (12, 0))
										continue;
									break;
								case AVMetadataObjectType.CodabarCode:
								case AVMetadataObjectType.GS1DataBarCode:
								case AVMetadataObjectType.GS1DataBarExpandedCode:
								case AVMetadataObjectType.GS1DataBarLimitedCode:
								case AVMetadataObjectType.MicroQRCode:
								case AVMetadataObjectType.MicroPdf417Code:
									if (!TestRuntime.CheckXcodeVersion (13, 3))
										continue;
									break;
								}
								metadataOutput.MetadataObjectTypes = val;
								all |= val;
								Assert.AreEqual (val, metadataOutput.MetadataObjectTypes, val.ToString ());
							}
							metadataOutput.MetadataObjectTypes = all;
							Assert.AreEqual (all, metadataOutput.MetadataObjectTypes, all.ToString ());
						}
					}
				}
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
