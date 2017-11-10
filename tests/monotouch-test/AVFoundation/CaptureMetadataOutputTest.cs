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
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using AVFoundation;
using CoreMedia;
using ObjCRuntime;
#else
using MonoTouch.AVFoundation;
using MonoTouch.CoreMedia;
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

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CaptureMetadataOutputTest {

		[Test]
		public void Defaults ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Ignore ("requires iOS6+");

			using (var obj = new AVCaptureMetadataOutput ()) {
#if XAMCORE_2_0
				Assert.AreEqual (AVMetadataObjectType.None, obj.AvailableMetadataObjectTypes, "AvailableMetadataObjectTypes");
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");

				Assert.IsNotNull (obj.WeakAvailableMetadataObjectTypes, "WeakAvailableMetadataObjectTypes");
				Assert.AreEqual (0, obj.WeakAvailableMetadataObjectTypes.Length, "WeakAvailableMetadataObjectTypes#");
				Assert.IsNotNull (obj.WeakMetadataObjectTypes, "WeakMetadataObjectTypes");
				Assert.AreEqual (0, obj.WeakMetadataObjectTypes.Length, "WeakMetadataObjectTypes#");
#else
				Assert.IsNotNull (obj.AvailableMetadataObjectTypes, "AvailableMetadataObjectTypes");
				Assert.AreEqual (0, obj.AvailableMetadataObjectTypes.Length, "AvailableMetadataObjectTypes#");
				Assert.IsNotNull (obj.MetadataObjectTypes, "MetadataObjectTypes");
				Assert.AreEqual (0, obj.MetadataObjectTypes.Length, "MetadataObjectTypes#");
#endif
				if (TestRuntime.CheckSystemAndSDKVersion (7,0))
					Assert.AreEqual (new RectangleF (0, 0, 1, 1), obj.RectOfInterest, "RectOfInterest");

#if XAMCORE_2_0
				obj.WeakMetadataObjectTypes = null;
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");
				obj.MetadataObjectTypes = AVMetadataObjectType.None;
				Assert.AreEqual (AVMetadataObjectType.None, obj.MetadataObjectTypes, "MetadataObjectTypes");
#else
				obj.MetadataObjectTypes = null;
				Assert.IsNotNull (obj.MetadataObjectTypes, "MetadataObjectTypes");
				Assert.AreEqual (0, obj.MetadataObjectTypes.Length, "MetadataObjectTypes#");
#endif
				obj.SetDelegate (null, null);
			}
		}

#if XAMCORE_2_0
		[Test]
		public void MetadataObjectTypesTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Test only works correctly in iOS 8+");

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
#endif
	}
}

#endif // !__TVOS__ && !__WATCHOS__
