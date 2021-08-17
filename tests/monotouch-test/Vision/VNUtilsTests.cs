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
using OpenTK;

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
            var imagePoint = VNUtils.GetImagePoint (new CGPoint(0, 0), 1, 1, new CGRect(0, 0, 1, 1));
            Assert.NotNull (imagePoint, $"VNImagePointForNormalizedPointUsingRegionOfInterest is not null");
        }

        [Test]
        public void GetNormalizedPointTest ()
        {
            var normalizedPoint = VNUtils.GetNormalizedPoint (new CGPoint(0, 0), 1, 1, new CGRect(0, 0, 1, 1));
            Assert.NotNull (normalizedPoint, $"VNNormalizedPointForImagePointUsingRegionOfInterest is not null");
        }

        [Test]
        public void GetImageRectTest ()
        {
            var normalizedPoint = VNUtils.GetImageRect (new CGRect(0, 0, 1, 1), 1, 1, new CGRect(0, 0, 1, 1));
            Assert.NotNull (normalizedPoint, $"VNImageRectForNormalizedRectUsingRegionOfInterest is not null");
        }

        [Test]
        public void GetNormalizedRectTest ()
        {
            var normalizedPoint = VNUtils.GetImageRect (new CGRect(0, 0, 1, 1), 1, 1, new CGRect(0, 0, 1, 1));
            Assert.NotNull (normalizedPoint, $"VNNormalizedRectForImageRectUsingRegionOfInterest is not null");
        }
    }
}
#endif
