//
// Unit tests for ARAnchor
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

using MatrixFloat4x4 = global::OpenTK.NMatrix4;

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARAnchorTest {

		[Test]
		public void MarshallingTest ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			var faceAnchor = new ARAnchor ("My Anchor", MatrixFloat4x4.Identity);
			Assert.AreEqual (MatrixFloat4x4.Identity, faceAnchor.Transform, "Transform");
		}
	}
}

#endif