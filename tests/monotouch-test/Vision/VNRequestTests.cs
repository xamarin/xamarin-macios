//
// Unit tests for VNRequestTests
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2018 Microsoft Corporation.
//

#if !__WATCHOS__ && XAMCORE_2_0

using System;
using NUnit.Framework;

using Foundation;
using Vision;
using System.Collections.Generic;
using CoreGraphics;

namespace MonoTouchFixtures.Vision {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VNRequestTests {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void VNSupportedRevisionsTest ()
		{
			// As of iOS 12, the following classes supports only
			// revision1 using >= 1 so this does not break in the future
			// the only intention of this test is to excercise manual code.
			var v1requests = new List<VNRequestRevision []> {
				VNCoreMLRequest.SupportedRevisions,
				VNDetectBarcodesRequest.SupportedRevisions,
				VNDetectFaceLandmarksRequest.SupportedRevisions,
				VNDetectFaceRectanglesRequest.SupportedRevisions,
				VNDetectHorizonRequest.SupportedRevisions,
				VNDetectRectanglesRequest.SupportedRevisions,
				VNDetectTextRectanglesRequest.SupportedRevisions,
				VNTranslationalImageRegistrationRequest.SupportedRevisions,
				VNHomographicImageRegistrationRequest.SupportedRevisions,
				VNTrackObjectRequest.SupportedRevisions,
				VNTrackRectangleRequest.SupportedRevisions,
			};

			for (int i = 0; i < v1requests.Count; i++)
				Assert.That (v1requests [i].Length, Is.GreaterThanOrEqualTo (1), $"v1requests[{i}]");

			// As of iOS 12, the following classes supports two revisions
			var v2requests = new List<VNRequestRevision []> {
				VNDetectFaceLandmarksRequest.SupportedRevisions,
				VNDetectFaceRectanglesRequest.SupportedRevisions,
			};

			for (int i = 0; i < v2requests.Count; i++)
				Assert.That (v2requests [i].Length, Is.GreaterThanOrEqualTo (2), $"v2requests[{i}]");
		}

		[Test]
		public void VNSupportedRevisionsUnsupportedTest ()
		{
			var allSupported = new List<(string type, VNRequestRevision [] revs)> {
				(nameof (VNCoreMLRequest), VNCoreMLRequest.SupportedRevisions),
				(nameof (VNDetectBarcodesRequest), VNDetectBarcodesRequest.SupportedRevisions),
				(nameof (VNDetectFaceLandmarksRequest), VNDetectFaceLandmarksRequest.SupportedRevisions),
				(nameof (VNDetectFaceRectanglesRequest), VNDetectFaceRectanglesRequest.SupportedRevisions),
				(nameof (VNDetectHorizonRequest), VNDetectHorizonRequest.SupportedRevisions),
				(nameof (VNDetectRectanglesRequest), VNDetectRectanglesRequest.SupportedRevisions),
				(nameof (VNDetectTextRectanglesRequest), VNDetectTextRectanglesRequest.SupportedRevisions),
				(nameof (VNImageRegistrationRequest), VNImageRegistrationRequest.SupportedRevisions),
				(nameof (VNTranslationalImageRegistrationRequest), VNTranslationalImageRegistrationRequest.SupportedRevisions),
				(nameof (VNHomographicImageRegistrationRequest), VNHomographicImageRegistrationRequest.SupportedRevisions),
				(nameof (VNRequest), VNRequest.SupportedRevisions),
				(nameof (VNImageBasedRequest), VNImageBasedRequest.SupportedRevisions),
				(nameof (VNTargetedImageRequest), VNTargetedImageRequest.SupportedRevisions),
				(nameof (VNTrackObjectRequest), VNTrackObjectRequest.SupportedRevisions),
				(nameof (VNTrackRectangleRequest), VNTrackRectangleRequest.SupportedRevisions),
				(nameof (VNTrackingRequest), VNTrackingRequest.SupportedRevisions),
			};

			foreach (var revisions in allSupported) {
				var type = revisions.type;
				foreach (var rev in revisions.revs)
					Assert.That (rev, Is.Not.EqualTo (VNRequestRevision.Unspecified), $"SupportedRevisions Unspecified found: {type}");
			}

			var allDefault = new List<(string type, VNRequestRevision rev)> {
				(nameof (VNCoreMLRequest), VNCoreMLRequest.DefaultRevision),
				(nameof (VNDetectBarcodesRequest), VNDetectBarcodesRequest.DefaultRevision),
				(nameof (VNDetectFaceLandmarksRequest), VNDetectFaceLandmarksRequest.DefaultRevision),
				(nameof (VNDetectFaceRectanglesRequest), VNDetectFaceRectanglesRequest.DefaultRevision),
				(nameof (VNDetectHorizonRequest), VNDetectHorizonRequest.DefaultRevision),
				(nameof (VNDetectRectanglesRequest), VNDetectRectanglesRequest.DefaultRevision),
				(nameof (VNDetectTextRectanglesRequest), VNDetectTextRectanglesRequest.DefaultRevision),
				(nameof (VNImageRegistrationRequest), VNImageRegistrationRequest.DefaultRevision),
				(nameof (VNTranslationalImageRegistrationRequest), VNTranslationalImageRegistrationRequest.DefaultRevision),
				(nameof (VNHomographicImageRegistrationRequest), VNHomographicImageRegistrationRequest.DefaultRevision),
				(nameof (VNRequest), VNRequest.DefaultRevision),
				(nameof (VNImageBasedRequest), VNImageBasedRequest.DefaultRevision),
				(nameof (VNTargetedImageRequest), VNTargetedImageRequest.DefaultRevision),
				(nameof (VNTrackObjectRequest), VNTrackObjectRequest.DefaultRevision),
				(nameof (VNTrackRectangleRequest), VNTrackRectangleRequest.DefaultRevision),
				(nameof (VNTrackingRequest), VNTrackingRequest.DefaultRevision),
			};

			foreach (var defrev in allDefault)
				Assert.That (defrev.rev, Is.Not.EqualTo (VNRequestRevision.Unspecified), $"DefaultRevision Unspecified found: {defrev.type}");

			var allCurrent = new List<(string type, VNRequestRevision rev)> {
				(nameof (VNCoreMLRequest), VNCoreMLRequest.CurrentRevision),
				(nameof (VNDetectBarcodesRequest), VNDetectBarcodesRequest.CurrentRevision),
				(nameof (VNDetectFaceLandmarksRequest), VNDetectFaceLandmarksRequest.CurrentRevision),
				(nameof (VNDetectFaceRectanglesRequest), VNDetectFaceRectanglesRequest.CurrentRevision),
				(nameof (VNDetectHorizonRequest), VNDetectHorizonRequest.CurrentRevision),
				(nameof (VNDetectRectanglesRequest), VNDetectRectanglesRequest.CurrentRevision),
				(nameof (VNDetectTextRectanglesRequest), VNDetectTextRectanglesRequest.CurrentRevision),
				(nameof (VNImageRegistrationRequest), VNImageRegistrationRequest.CurrentRevision),
				(nameof (VNTranslationalImageRegistrationRequest), VNTranslationalImageRegistrationRequest.CurrentRevision),
				(nameof (VNHomographicImageRegistrationRequest), VNHomographicImageRegistrationRequest.CurrentRevision),
				(nameof (VNRequest), VNRequest.CurrentRevision),
				(nameof (VNImageBasedRequest), VNImageBasedRequest.CurrentRevision),
				(nameof (VNTargetedImageRequest), VNTargetedImageRequest.CurrentRevision),
				(nameof (VNTrackObjectRequest), VNTrackObjectRequest.CurrentRevision),
				(nameof (VNTrackRectangleRequest), VNTrackRectangleRequest.CurrentRevision),
				(nameof (VNTrackingRequest), VNTrackingRequest.CurrentRevision),
			};

			foreach (var currev in allCurrent)
				Assert.That (currev.rev, Is.Not.EqualTo (VNRequestRevision.Unspecified), $"CurrentRevision Unspecified found: {currev.type}");

			// Tests 'VNRequestRevision.Unspecified' given to APIs.
			var rect = new CGRect (0, 0, 10, 10);
			Assert.DoesNotThrow (() => {
				var detectedObjectObservation = VNDetectedObjectObservation.FromBoundingBox (VNRequestRevision.Unspecified, rect);
				Assert.NotNull (detectedObjectObservation, "detectedObjectObservation is null");
				Assert.That (detectedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var faceObservation = VNFaceObservation.FromBoundingBox (VNRequestRevision.Unspecified, rect);
				Assert.NotNull (faceObservation, "faceObservation is null");
				Assert.That (faceObservation.BoundingBox, Is.EqualTo (rect));

				var recognizedObjectObservation = VNRecognizedObjectObservation.FromBoundingBox (VNRequestRevision.Unspecified, rect);
				Assert.NotNull (recognizedObjectObservation, "recognizedObjectObservation is null");
				Assert.That (recognizedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var rectangleObservation = VNRectangleObservation.FromBoundingBox (VNRequestRevision.Unspecified, rect);
				Assert.NotNull (rectangleObservation, "rectangleObservation is null");
				Assert.That (rectangleObservation.BoundingBox, Is.EqualTo (rect));

				var textObservation = VNTextObservation.FromBoundingBox (VNRequestRevision.Unspecified, rect);
				Assert.NotNull (textObservation, "textObservation is null");
				Assert.That (textObservation.BoundingBox, Is.EqualTo (rect));

				var barcodeObservation = VNBarcodeObservation.FromBoundingBox (VNRequestRevision.Unspecified, rect);
				Assert.NotNull (barcodeObservation, "barcodeObservation is null");
				Assert.That (barcodeObservation.BoundingBox, Is.EqualTo (rect));
			}, "VNRequestRevision.Unspecified throw");

			// Tests random request revision
			var randomRevision = (VNRequestRevision) 50000;
			Assert.DoesNotThrow (() => {
				var detectedObjectObservation = VNDetectedObjectObservation.FromBoundingBox (randomRevision, rect);
				Assert.NotNull (detectedObjectObservation, "randomRevision detectedObjectObservation is null");
				Assert.That (detectedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var faceObservation = VNFaceObservation.FromBoundingBox (randomRevision, rect);
				Assert.NotNull (faceObservation, "randomRevision faceObservation is null");
				Assert.That (faceObservation.BoundingBox, Is.EqualTo (rect));

				var recognizedObjectObservation = VNRecognizedObjectObservation.FromBoundingBox (randomRevision, rect);
				Assert.NotNull (recognizedObjectObservation, "randomRevision recognizedObjectObservation is null");
				Assert.That (recognizedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var rectangleObservation = VNRectangleObservation.FromBoundingBox (randomRevision, rect);
				Assert.NotNull (rectangleObservation, "randomRevision rectangleObservation is null");
				Assert.That (rectangleObservation.BoundingBox, Is.EqualTo (rect));

				var textObservation = VNTextObservation.FromBoundingBox (randomRevision, rect);
				Assert.NotNull (textObservation, "randomRevision textObservation is null");
				Assert.That (textObservation.BoundingBox, Is.EqualTo (rect));

				var barcodeObservation = VNBarcodeObservation.FromBoundingBox (randomRevision, rect);
				Assert.NotNull (barcodeObservation, "randomRevision barcodeObservation is null");
				Assert.That (barcodeObservation.BoundingBox, Is.EqualTo (rect));
			}, "randomRevision throw");
		}

		[Test]
		public void VNSupportedRevisionsTwoTest ()
		{
			// Tests 'VNRequestRevision.Two' given to APIs.
			var rect = new CGRect (0, 0, 10, 10);
			Assert.DoesNotThrow (() => {
				var detectedObjectObservation = VNDetectedObjectObservation.FromBoundingBox (VNRequestRevision.Two, rect);
				Assert.NotNull (detectedObjectObservation, "detectedObjectObservation is null");
				Assert.That (detectedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var faceObservation = VNFaceObservation.FromBoundingBox (VNRequestRevision.Two, rect);
				Assert.NotNull (faceObservation, "faceObservation is null");
				Assert.That (faceObservation.BoundingBox, Is.EqualTo (rect));

				var recognizedObjectObservation = VNRecognizedObjectObservation.FromBoundingBox (VNRequestRevision.Two, rect);
				Assert.NotNull (recognizedObjectObservation, "recognizedObjectObservation is null");
				Assert.That (recognizedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var rectangleObservation = VNRectangleObservation.FromBoundingBox (VNRequestRevision.Two, rect);
				Assert.NotNull (rectangleObservation, "rectangleObservation is null");
				Assert.That (rectangleObservation.BoundingBox, Is.EqualTo (rect));

				var textObservation = VNTextObservation.FromBoundingBox (VNRequestRevision.Two, rect);
				Assert.NotNull (textObservation, "textObservation is null");
				Assert.That (textObservation.BoundingBox, Is.EqualTo (rect));

				var barcodeObservation = VNBarcodeObservation.FromBoundingBox (VNRequestRevision.Two, rect);
				Assert.NotNull (barcodeObservation, "barcodeObservation is null");
				Assert.That (barcodeObservation.BoundingBox, Is.EqualTo (rect));
			}, "VNRequestRevision.Two throw");
		}
	}
}
#endif
