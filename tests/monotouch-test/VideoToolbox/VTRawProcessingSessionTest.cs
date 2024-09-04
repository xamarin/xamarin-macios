//
// Unit tests for VTRawProcessingSession
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
	public class VTRawProcessingSessionTest {
		[Test]
		public void Create_NSDictionary_Test ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
			using var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out var fde);
			Assert.IsNotNull (desc, "Desc");
			Assert.AreEqual (CMFormatDescriptionError.None, fde, "vdf error");
			using var session = VTRawProcessingSession.Create (desc, (NSDictionary) null, (NSDictionary) null, out var vtStatus);
			Assert.AreEqual (VTStatus.Ok, vtStatus, "status");
		}

		[Test]
		public void Create_CVPixelBufferAttributes_Test ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
			using var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out var fde);
			Assert.IsNotNull (desc, "Desc");
			Assert.AreEqual (CMFormatDescriptionError.None, fde, "vdf error");
			using var session = VTRawProcessingSession.Create (desc, (CVPixelBufferAttributes) null, (VTRawProcessingParameters) null, out var vtStatus);
			Assert.AreEqual (VTStatus.Ok, vtStatus, "status");
		}

		[Test]
		public void GetTypeId ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			Assert.AreNotEqual (0, VTRawProcessingSession.GetTypeId (), "GetTypeId");
		}

		[Test]
		public void ProcessingTest ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
			using var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out var fde);
			Assert.IsNotNull (desc, "Desc");
			Assert.AreEqual (CMFormatDescriptionError.None, fde, "vdf error");
			using var session = VTRawProcessingSession.Create (desc, (CVPixelBufferAttributes) null, (VTRawProcessingParameters) null, out var vtStatus);
			Assert.AreEqual (VTStatus.Ok, vtStatus, "Create status");

			session.SetParameterChangedHandler ((NSObject []? newParameters) => {
				Console.WriteLine ($"ParameterChanged: {newParameters}");
			});

			var parameters = session.CopyProcessingParameters (out vtStatus);
			Assert.AreEqual (VTStatus.Ok, vtStatus, "CopyProcessingParameters status");
			Assert.IsNotNull (parameters, "Parameters");
			Console.WriteLine (parameters);
			var prms = session.ProcessingParameters;
			Assert.IsNotNull (prms, "ProcessingParameters");
			Console.WriteLine (prms);

			vtStatus = session.SetProcessingParameters (new NSDictionary ());
			Assert.AreEqual (VTStatus.Ok, vtStatus, "SetProcessingParameters status");

			vtStatus = session.SetProcessingParameters (new VTRawProcessingParameters ());
			Assert.AreEqual (VTStatus.Ok, vtStatus, "SetProcessingParameters status (VTRawProcessingParameter)");

			session.ProcessFrame (pixelBuffer, (NSDictionary) null, (VTStatus status, CVPixelBuffer? processedPixelBuffer) =>
			{
				Console.WriteLine ($"ProcessFrameCallback, status: {status} pb: {processedPixelBuffer}");
			});

			session.CompleteFrames ();

			session.Invalidate ();
		}
	}
}

#endif // NET
#endif // !__WATCHOS__
