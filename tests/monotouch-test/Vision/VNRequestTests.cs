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
	}
}
#endif
