//
// Unit tests for VTHdrPerFrameMetadataGenerationSession
//

#if !__WATCHOS__
#if NET

using System;
using System.Drawing;
using System.Runtime.InteropServices;

using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using ObjCRuntime;
using VideoToolbox;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

using NUnit.Framework;

using Xamarin.Utils;

namespace MonoTouchFixtures.VideoToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VTHdrPerFrameMetadataGenerationSessionTest {
		[Test]
		public void Create_NSDictionary_Test ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var session = VTHdrPerFrameMetadataGenerationSession.Create (30, (NSDictionary) null, out var vtStatus);
			Assert.IsNotNull (session, "session");
			Assert.AreEqual (VTStatus.Ok, vtStatus, "status");
		}

		[Test]
		public void Create_VTHdrPerFrameMetadataGenerationOptions_Test ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var session = VTHdrPerFrameMetadataGenerationSession.Create (30, (VTHdrPerFrameMetadataGenerationOptions) null, out var vtStatus);
			Assert.IsNotNull (session, "session");
			Assert.AreEqual (VTStatus.Ok, vtStatus, "status");
		}

		[Test]
		public void AttachMetadataTest ()
		{
			var width = 640;
			var height = 480;

			TestRuntime.AssertXcodeVersion (16, 0);

			using var session = VTHdrPerFrameMetadataGenerationSession.Create (30, (VTHdrPerFrameMetadataGenerationOptions) null, out var vtStatus);
			Assert.IsNotNull (session, "session");
			Assert.AreEqual (VTStatus.Ok, vtStatus, "status");
			using var pixelBuffer = new CVPixelBuffer (width, height, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);
			vtStatus = session.AttachMetadata (pixelBuffer, false);
			Assert.AreEqual (VTStatus.Ok, vtStatus, "status AttachMetadata");
		}

		[Test]
		public void GetTypeId ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			Assert.AreNotEqual (0, VTHdrPerFrameMetadataGenerationSession.GetTypeId (), "GetTypeId");
		}
	}
}

#endif // NET
#endif // !__WATCHOS__
