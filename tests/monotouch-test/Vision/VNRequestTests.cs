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
using System.Collections;
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

			var w = VNCoreMLRequest.SupportedRevisions;

			var v1requests = new List<Array> {
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
			var v2requests = new List<Array> {
				VNDetectFaceLandmarksRequest.SupportedRevisions,
				VNDetectFaceRectanglesRequest.SupportedRevisions,
			};

			for (int i = 0; i < v2requests.Count; i++)
				Assert.That (v2requests [i].Length, Is.GreaterThanOrEqualTo (2), $"v2requests[{i}]");
		}

		[Test]
		public void VNSupportedRevisionsUnsupportedTest ()
		{
			var allSupported = new List<(string type, IEnumerable revs)> {
				(nameof (VNCoreMLRequest), VNCoreMLRequest.SupportedRevisions),
				(nameof (VNDetectBarcodesRequest), VNDetectBarcodesRequest.SupportedRevisions),
				(nameof (VNDetectFaceLandmarksRequest), VNDetectFaceLandmarksRequest.SupportedRevisions),
				(nameof (VNDetectFaceRectanglesRequest), VNDetectFaceRectanglesRequest.SupportedRevisions),
				(nameof (VNDetectHorizonRequest), VNDetectHorizonRequest.SupportedRevisions),
				(nameof (VNDetectRectanglesRequest), VNDetectRectanglesRequest.SupportedRevisions),
				(nameof (VNDetectTextRectanglesRequest), VNDetectTextRectanglesRequest.SupportedRevisions),
				(nameof (VNTranslationalImageRegistrationRequest), VNTranslationalImageRegistrationRequest.SupportedRevisions),
				(nameof (VNHomographicImageRegistrationRequest), VNHomographicImageRegistrationRequest.SupportedRevisions),
				(nameof (VNTrackObjectRequest), VNTrackObjectRequest.SupportedRevisions),
				(nameof (VNTrackRectangleRequest), VNTrackRectangleRequest.SupportedRevisions),
			};

			foreach (var revisions in allSupported) {
				var type = revisions.type;
				foreach (object rev in revisions.revs)
					Assert.That (Convert.ChangeType (rev, Enum.GetUnderlyingType (rev.GetType ())), Is.Not.EqualTo (0), $"SupportedRevisions Unspecified found: {type}");
			}

			var allDefault = new List<(string type, object rev)> {
				(nameof (VNCoreMLRequest), VNCoreMLRequest.DefaultRevision),
				(nameof (VNDetectBarcodesRequest), VNDetectBarcodesRequest.DefaultRevision),
				(nameof (VNDetectFaceLandmarksRequest), VNDetectFaceLandmarksRequest.DefaultRevision),
				(nameof (VNDetectFaceRectanglesRequest), VNDetectFaceRectanglesRequest.DefaultRevision),
				(nameof (VNDetectHorizonRequest), VNDetectHorizonRequest.DefaultRevision),
				(nameof (VNDetectRectanglesRequest), VNDetectRectanglesRequest.DefaultRevision),
				(nameof (VNDetectTextRectanglesRequest), VNDetectTextRectanglesRequest.DefaultRevision),
				(nameof (VNTranslationalImageRegistrationRequest), VNTranslationalImageRegistrationRequest.DefaultRevision),
				(nameof (VNHomographicImageRegistrationRequest), VNHomographicImageRegistrationRequest.DefaultRevision),
				(nameof (VNTrackObjectRequest), VNTrackObjectRequest.DefaultRevision),
				(nameof (VNTrackRectangleRequest), VNTrackRectangleRequest.DefaultRevision),
			};

			foreach (var defrev in allDefault)
				Assert.That (Convert.ChangeType (defrev.rev, Enum.GetUnderlyingType (defrev.rev.GetType ())), Is.Not.EqualTo (0), $"DefaultRevision Unspecified found: {defrev.type}");

			var allCurrent = new List<(string type, object rev)> {
				(nameof (VNCoreMLRequest), VNCoreMLRequest.CurrentRevision),
				(nameof (VNDetectBarcodesRequest), VNDetectBarcodesRequest.CurrentRevision),
				(nameof (VNDetectFaceLandmarksRequest), VNDetectFaceLandmarksRequest.CurrentRevision),
				(nameof (VNDetectFaceRectanglesRequest), VNDetectFaceRectanglesRequest.CurrentRevision),
				(nameof (VNDetectHorizonRequest), VNDetectHorizonRequest.CurrentRevision),
				(nameof (VNDetectRectanglesRequest), VNDetectRectanglesRequest.CurrentRevision),
				(nameof (VNDetectTextRectanglesRequest), VNDetectTextRectanglesRequest.CurrentRevision),
				(nameof (VNTranslationalImageRegistrationRequest), VNTranslationalImageRegistrationRequest.CurrentRevision),
				(nameof (VNHomographicImageRegistrationRequest), VNHomographicImageRegistrationRequest.CurrentRevision),
				(nameof (VNTrackObjectRequest), VNTrackObjectRequest.CurrentRevision),
				(nameof (VNTrackRectangleRequest), VNTrackRectangleRequest.CurrentRevision),
			};

			foreach (var currev in allCurrent)
				Assert.That (Convert.ChangeType (currev.rev, Enum.GetUnderlyingType (currev.rev.GetType ())), Is.Not.EqualTo (0), $"CurrentRevision Unspecified found: {currev.type}");

			// Tests 'VNRequestRevision.Unspecified' given to APIs.
			var rect = new CGRect (0, 0, 10, 10);
			Assert.DoesNotThrow (() => {
				var detectedObjectObservation = VNDetectedObjectObservation.FromBoundingBox (VNDetectedObjectObservationRequestRevision.Unspecified, rect);
				Assert.NotNull (detectedObjectObservation, "detectedObjectObservation is null");
				Assert.That (detectedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var faceObservation = VNFaceObservation.FromBoundingBox (VNFaceObservationRequestRevision.Unspecified, rect);
				Assert.NotNull (faceObservation, "faceObservation is null");
				Assert.That (faceObservation.BoundingBox, Is.EqualTo (rect));

				var recognizedObjectObservation = VNRecognizedObjectObservation.FromBoundingBox (VNRecognizedObjectObservationRequestRevision.Unspecified, rect);
				Assert.NotNull (recognizedObjectObservation, "recognizedObjectObservation is null");
				Assert.That (recognizedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var rectangleObservation = VNRectangleObservation.FromBoundingBox (VNRectangleObservationRequestRevision.Unspecified, rect);
				Assert.NotNull (rectangleObservation, "rectangleObservation is null");
				Assert.That (rectangleObservation.BoundingBox, Is.EqualTo (rect));

				var textObservation = VNTextObservation.FromBoundingBox (VNTextObservationRequestRevision.Unspecified, rect);
				Assert.NotNull (textObservation, "textObservation is null");
				Assert.That (textObservation.BoundingBox, Is.EqualTo (rect));

				var barcodeObservation = VNBarcodeObservation.FromBoundingBox (VNBarcodeObservationRequestRevision.Unspecified, rect);
				Assert.NotNull (barcodeObservation, "barcodeObservation is null");
				Assert.That (barcodeObservation.BoundingBox, Is.EqualTo (rect));
			}, "VNRequestRevision.Unspecified throw");

			// Tests random request revision
			Assert.DoesNotThrow (() => {
				var detectedObjectObservation = VNDetectedObjectObservation.FromBoundingBox ((VNDetectedObjectObservationRequestRevision) 5000, rect);
				Assert.NotNull (detectedObjectObservation, "randomRevision detectedObjectObservation is null");
				Assert.That (detectedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var faceObservation = VNFaceObservation.FromBoundingBox ((VNFaceObservationRequestRevision) 5000, rect);
				Assert.NotNull (faceObservation, "randomRevision faceObservation is null");
				Assert.That (faceObservation.BoundingBox, Is.EqualTo (rect));

				var recognizedObjectObservation = VNRecognizedObjectObservation.FromBoundingBox ((VNRecognizedObjectObservationRequestRevision) 5000, rect);
				Assert.NotNull (recognizedObjectObservation, "randomRevision recognizedObjectObservation is null");
				Assert.That (recognizedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var rectangleObservation = VNRectangleObservation.FromBoundingBox ((VNRectangleObservationRequestRevision) 5000, rect);
				Assert.NotNull (rectangleObservation, "randomRevision rectangleObservation is null");
				Assert.That (rectangleObservation.BoundingBox, Is.EqualTo (rect));

				var textObservation = VNTextObservation.FromBoundingBox ((VNTextObservationRequestRevision) 5000, rect);
				Assert.NotNull (textObservation, "randomRevision textObservation is null");
				Assert.That (textObservation.BoundingBox, Is.EqualTo (rect));

				var barcodeObservation = VNBarcodeObservation.FromBoundingBox ((VNBarcodeObservationRequestRevision) 5000, rect);
				Assert.NotNull (barcodeObservation, "randomRevision barcodeObservation is null");
				Assert.That (barcodeObservation.BoundingBox, Is.EqualTo (rect));
			}, "randomRevision throw");
		}

		[Test]
		public void VNSupportedRevisionsTwoTest ()
		{
			// Tests '*RequestRevision.Two' given to APIs.
			var rect = new CGRect (0, 0, 10, 10);
			Assert.DoesNotThrow (() => {
				var detectedObjectObservation = VNDetectedObjectObservation.FromBoundingBox (VNDetectedObjectObservationRequestRevision.Two, rect);
				Assert.NotNull (detectedObjectObservation, "detectedObjectObservation is null");
				Assert.That (detectedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var faceObservation = VNFaceObservation.FromBoundingBox (VNFaceObservationRequestRevision.Two, rect);
				Assert.NotNull (faceObservation, "faceObservation is null");
				Assert.That (faceObservation.BoundingBox, Is.EqualTo (rect));

				var recognizedObjectObservation = VNRecognizedObjectObservation.FromBoundingBox (VNRecognizedObjectObservationRequestRevision.Two, rect);
				Assert.NotNull (recognizedObjectObservation, "recognizedObjectObservation is null");
				Assert.That (recognizedObjectObservation.BoundingBox, Is.EqualTo (rect));

				var rectangleObservation = VNRectangleObservation.FromBoundingBox (VNRectangleObservationRequestRevision.Two, rect);
				Assert.NotNull (rectangleObservation, "rectangleObservation is null");
				Assert.That (rectangleObservation.BoundingBox, Is.EqualTo (rect));

				var textObservation = VNTextObservation.FromBoundingBox (VNTextObservationRequestRevision.Two, rect);
				Assert.NotNull (textObservation, "textObservation is null");
				Assert.That (textObservation.BoundingBox, Is.EqualTo (rect));

				var barcodeObservation = VNBarcodeObservation.FromBoundingBox (VNBarcodeObservationRequestRevision.Two, rect);
				Assert.NotNull (barcodeObservation, "barcodeObservation is null");
				Assert.That (barcodeObservation.BoundingBox, Is.EqualTo (rect));
			}, "*RequestRevision.Two throw");
		}
	}
}
#endif
