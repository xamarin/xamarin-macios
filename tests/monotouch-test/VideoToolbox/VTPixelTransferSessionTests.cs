//
// Unit tests for VTPixelTransferSession
//
// Authors:
//	Israel Soto <issoto@microsoft.com>
//	
//
// Copyright 2022 Microsoft Corporation.
//

#if !__WATCHOS__

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Foundation;
using VideoToolbox;
using CoreMedia;
using CoreVideo;
using AVFoundation;
using CoreFoundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTPixelTransferSessionTests {
		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (14, 0);

		[Test]
		public void PixelTransferSessionCreateTest ()
		{
			using var session = VTPixelTransferSession.Create ();
			Assert.IsNotNull (session, "Session should not be null");
		}

		[Test]
		public void PixelTransferSessionTransferImageTest ()
		{
			using var session = VTPixelTransferSession.Create ();
			using var sourcePixelBuffer = new CVPixelBuffer (640, 480, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);
			using var destinationPixelBuffer = new CVPixelBuffer (320, 240, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);
			var result = session.TransferImage (sourcePixelBuffer, destinationPixelBuffer);
			Assert.AreEqual (result, VTStatus.Ok, "TransferImage");
		}

		[Test]
		public void SetTransferPropertiesTest ()
		{
			using var session = VTPixelTransferSession.Create ();
			var result = session.SetTransferProperties (new VTPixelTransferProperties {
				ScalingMode = VTScalingMode.Letterbox
			});

			Assert.AreEqual (result, VTStatus.Ok, "SetTransferProperties");
		}
	}
}

#endif // !__WATCHOS__
