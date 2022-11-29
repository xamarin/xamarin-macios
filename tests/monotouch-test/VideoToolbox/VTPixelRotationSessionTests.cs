//
// Unit tests for VTPixelRotationSession
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
	public class VTPixelRotationSessionTests {
		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (14, 0);

		[Test]
		public void CreateTest ()
		{
			using var session = VTPixelRotationSession.Create ();
			Assert.IsNotNull (session, "Session should not be null");
		}

		[Test]
		public void RotateImageTest ()
		{
			using var session = VTPixelRotationSession.Create ();
			using var sourcePixelBuffer = new CVPixelBuffer (640, 480, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);
			using var destinationPixelBuffer = new CVPixelBuffer (480, 640, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);

			var result = session.SetProperty (VTPixelRotationPropertyKeys.Rotation, VTRotation.ClockwiseNinety.GetConstant ());
			Assert.AreEqual (result, VTStatus.Ok, "SetProperty");

			result = session.RotateImage (sourcePixelBuffer, destinationPixelBuffer);
			Assert.AreEqual (result, VTStatus.Ok, "RotateImage");
		}

		[Test]
		public void SetRotationPropertiesTest ()
		{
			using var session = VTPixelRotationSession.Create ();
			var result = session.SetRotationProperties (new VTPixelRotationProperties {
				Rotation = VTRotation.ClockwiseNinety
			});

			Assert.AreEqual (result, VTStatus.Ok, "SetRotationProperties");
		}
	}
}

#endif // !__WATCHOS__
