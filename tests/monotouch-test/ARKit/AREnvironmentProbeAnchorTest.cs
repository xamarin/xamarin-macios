//
// Unit tests for AREnvironmentProbeAnchor
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2018 Microsoft. All rights reserved.
//

#if HAS_ARKIT

using System;
using ARKit;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

#if NET
using VectorFloat3 = global::CoreGraphics.NVector3;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using VectorFloat3 = global::OpenTK.NVector3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AREnvironmentProbeAnchorTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

		[Test]
		public void MarshallingTest ()
		{
			var probeAnchor = new AREnvironmentProbeAnchor (MatrixFloat4x4.Identity, new VectorFloat3 (1, 1, 1));
			Assert.AreEqual (MatrixFloat4x4.Identity, probeAnchor.Transform, "Transform");
			// broken since Xcode 12 on simulator (only), fixed in simulator in Xcode 14
			if ((Runtime.Arch == Arch.DEVICE) || !TestRuntime.CheckXcodeVersion (12, 0) || TestRuntime.CheckXcodeVersion (14, 0))
				Assert.AreEqual (new VectorFloat3 (1, 1, 1), probeAnchor.Extent, "Extent");
		}

		[Test]
		public void MarshallingTest2 ()
		{
			var probeAnchorWithName = new AREnvironmentProbeAnchor ("My Anchor", MatrixFloat4x4.Identity, new VectorFloat3 (1, 1, 1));
			Assert.AreEqual (MatrixFloat4x4.Identity, probeAnchorWithName.Transform, "Transform");
			// broken since Xcode 12 on simulator (only), fixed in simulator in Xcode 14
			if ((Runtime.Arch == Arch.DEVICE) || !TestRuntime.CheckXcodeVersion (12, 0) || TestRuntime.CheckXcodeVersion (14, 0))
				Assert.AreEqual (new VectorFloat3 (1, 1, 1), probeAnchorWithName.Extent, "Extent");
		}
	}
}

#endif // HAS_ARKIT
