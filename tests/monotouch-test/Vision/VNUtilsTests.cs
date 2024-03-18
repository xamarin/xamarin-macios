//
// Unit tests for VNUtils
//
// Authors:
//	Rachel Kang <rachelkang@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#if !__WATCHOS__

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using CoreGraphics;
using Foundation;
using Vision;

#if NET
using Vector2 = global::System.Numerics.Vector2;
#else
using Vector2 = global::OpenTK.Vector2;
#endif

namespace MonoTouchFixtures.Vision {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VNUtilsTests {

		[SetUp]
		public void Setup () => TestRuntime.AssertXcodeVersion (13, 0);

		[Test]
		public void GetImagePointTest ()
		{
			var imagePoint = VNUtils.GetImagePoint (new CGPoint (2, 345), 6, 78, new CGRect (90, 12, 34, 56));
			Assert.That (imagePoint, Is.Not.EqualTo (CGPoint.Empty), "VNImagePointForNormalizedPointUsingRegionOfInterest is not empty");
		}

		[Test]
		public void GetNormalizedPointTest ()
		{
			var normalizedPoint = VNUtils.GetNormalizedPoint (new CGPoint (2, 345), 6, 78, new CGRect (90, 12, 34, 56));
			Assert.That (normalizedPoint, Is.Not.EqualTo (CGPoint.Empty), "VNNormalizedPointForImagePointUsingRegionOfInterest is not empty");
		}

		[Test]
		public void GetImageRectTest ()
		{
			var imageRect = VNUtils.GetImageRect (new CGRect (2, 34, 5, 67), 8, 90, new CGRect (123, 4, 56, 7));
			Assert.That (imageRect, Is.Not.EqualTo (CGRect.Empty), "VNImageRectForNormalizedRectUsingRegionOfInterest is not empty");
		}

		[Test]
		public void GetNormalizedRectTest ()
		{
			var normalizedRect = VNUtils.GetImageRect (new CGRect (2, 34, 5, 67), 8, 90, new CGRect (123, 4, 56, 7));
			Assert.That (normalizedRect, Is.Not.EqualTo (CGRect.Empty), "VNNormalizedRectForImageRectUsingRegionOfInterest is not empty");
		}

		[Test]
		public void GetNormalizedFaceBoundingBoxPointTest ()
		{
			var normalizedPoint = VNUtils.GetNormalizedFaceBoundingBoxPoint (new Vector2 (9, 8), new CGRect (3, 14, 15, 92), 2, 11);
			Assert.That (normalizedPoint, Is.Not.EqualTo (CGPoint.Empty), "VNNormalizedFaceBoundingBoxPointForLandmarkPoint is not empty");
		}


		[Test]
		public void IsIdentityTest ()
		{
			Assert.True (VNUtils.IsIdentityRect (new CGRect (0, 0, 1, 1)), "Identity");
			Assert.False (VNUtils.IsIdentityRect (new CGRect (0, 0, 2, 2)), "Not Identity A");
			Assert.False (VNUtils.IsIdentityRect (new CGRect (1, 1, 1, 1)), "Not Identity B");
			Assert.False (VNUtils.IsIdentityRect (new CGRect (1, 1, 0, 0)), "Not Identity C");
		}

	}
}
#endif
