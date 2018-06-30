//
// Unit tests for AREnvironmentProbeAnchor
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2018 Microsoft. All rights reserved.
//

#if XAMCORE_2_0 && __IOS__

using System;
using ARKit;
using Foundation;
using NUnit.Framework;

using VectorFloat3 = global::OpenTK.NVector3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AREnvironmentProbeAnchorTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void MarshallingTest ()
		{
			var probeAnchor = new AREnvironmentProbeAnchor (MatrixFloat4x4.Identity, new VectorFloat3 (1, 1, 1));
			Assert.AreEqual (MatrixFloat4x4.Identity, probeAnchor.Transform, "Transform");
			Assert.AreEqual (new VectorFloat3 (1, 1, 1), probeAnchor.Extent, "Extent");

			var probeAnchorWithName = new AREnvironmentProbeAnchor ("My Anchor", MatrixFloat4x4.Identity, new VectorFloat3 (1, 1, 1));
			Assert.AreEqual (MatrixFloat4x4.Identity, probeAnchorWithName.Transform, "Transform");
			Assert.AreEqual (new VectorFloat3 (1, 1, 1), probeAnchorWithName.Extent, "Extent");
		}
	}
}

#endif