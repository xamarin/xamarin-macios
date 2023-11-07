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
	}
}
#endif
