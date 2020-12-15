//
// Unit tests for ARAnchor
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2018 Microsoft. All rights reserved.
//

#if __IOS__

using System;
using ARKit;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

using MatrixFloat4x4 = global::OpenTK.NMatrix4;

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARAnchorTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			// Mac Catalyst system versions follow the macOS system versions, and ARKit was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (PlatformName.MacCatalyst, 11, 0, throwIfOtherPlatform: false);
		}

		[Test]
		public void MarshallingTest ()
		{
			var faceAnchor = new ARAnchor ("My Anchor", MatrixFloat4x4.Identity);
			Assert.AreEqual (MatrixFloat4x4.Identity, faceAnchor.Transform, "Transform");
		}
	}
}

#endif