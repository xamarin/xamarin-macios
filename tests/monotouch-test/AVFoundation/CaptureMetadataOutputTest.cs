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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using CoreGraphics;
using Foundation;
using AVFoundation;
using CoreMedia;
using ObjCRuntime;
using NUnit.Framework;

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
				if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false))
					Assert.AreEqual (new CGRect (0, 0, 1, 1), obj.RectOfInterest, "RectOfInterest");

				obj.WeakMetadataObjectTypes = null;
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");
				obj.MetadataObjectTypes = AVMetadataObjectType.None;
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");
				obj.SetDelegate (null, null);
			}
		}

		[Test]
		public void Flags ()
		{
			// we can only only work with [Weak]AvailableMetadataObjectTypes on an instance of AVCaptureMetadataOutput
			// so we use reflection to test the internal of the flags/enum/constants conversions
			// the previous tests ensure what we need is not removed by the linker
			var t = typeof (AVMetadataObject);
			var array_to_enum = t.GetMethod ("ArrayToEnum", BindingFlags.Static | BindingFlags.NonPublic);
			Assert.NotNull (array_to_enum, "ArrayToEnum");

			var enum_to_array = t.GetMethod ("EnumToArray", BindingFlags.Static | BindingFlags.NonPublic);
			Assert.NotNull (enum_to_array, "EnumToArray");

			// single
			var flags = AVMetadataObjectType.Face;
			var result = (AVMetadataObjectType) array_to_enum.Invoke (null, new [] { new NSString [] { flags.GetConstant () } });
			Assert.AreEqual (flags, result, "a2e 1");
			var back = (NSString[]) enum_to_array.Invoke (null, new object [] { result });
			Assert.That (back.Length, Is.EqualTo (1), "l 1");
			Assert.That (back [0], Is.EqualTo (flags.GetConstant ()), "e2a 1");

			// constants are only available in recent xcode (and not on any 32bits OS)
			if (TestRuntime.CheckXcodeVersion (11,0)) {
				// multiple (flags)
				flags = AVMetadataObjectType.CatBody | AVMetadataObjectType.DogBody | AVMetadataObjectType.HumanBody;
				var array = new NSString [] {
					AVMetadataObjectType.CatBody.GetConstant (),
					AVMetadataObjectType.DogBody.GetConstant (),
					AVMetadataObjectType.HumanBody.GetConstant ()
				};
				result = (AVMetadataObjectType)array_to_enum.Invoke (null, new [] { array });
				Assert.AreEqual (flags, result, "a2e 3");
				back = (NSString [])enum_to_array.Invoke (null, new object [] { result });
				Assert.That (back.Length, Is.EqualTo (3), "l 3");
				Assert.That (back [0], Is.EqualTo (array [0]), "e2a 3a");
				Assert.That (back [1], Is.EqualTo (array [1]), "e2a 3b");
				Assert.That (back [2], Is.EqualTo (array [2]), "e2a 3c");
			}
		}

		[Test]
		public void MetadataObjectTypesTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);

			if (Runtime.Arch != Arch.DEVICE)
				Assert.Ignore ("This test only runs on device (requires camera access)");

			TestRuntime.RequestCameraPermission (AVMediaType.Video, true);

			using (var captureSession = new AVCaptureSession ()) {
				using (var videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video)) {

					NSError error;
					using (var videoInput = new AVCaptureDeviceInput (videoDevice, out error)) {
						if (captureSession.CanAddInput (videoInput))
							captureSession.AddInput (videoInput);

						using (var metadataOutput = new AVCaptureMetadataOutput ()) {

							if (captureSession.CanAddOutput (metadataOutput))
								captureSession.AddOutput (metadataOutput);

							AVMetadataObjectType all = AVMetadataObjectType.None;
							foreach (AVMetadataObjectType val in Enum.GetValues (typeof (AVMetadataObjectType))) {
								switch (val) {
								case AVMetadataObjectType.CatBody:
								case AVMetadataObjectType.DogBody:
								case AVMetadataObjectType.HumanBody:
								case AVMetadataObjectType.SalientObject:
									// fail *and crash* on iOS 8 (at least on 32bits devices)
									if (!TestRuntime.CheckXcodeVersion (11, 0))
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
