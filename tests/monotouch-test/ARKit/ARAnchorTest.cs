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

		[Test]
		public void MarshallingTest ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			var faceAnchor = new ARAnchor ("My Anchor", MatrixFloat4x4.Identity);
			if ((Runtime.Arch == Arch.SIMULATOR) && TestRuntime.CheckXcodeVersion (12, 0))
				Assert.Ignore ("broken with beta 1");
			Assert.AreEqual (MatrixFloat4x4.Identity, faceAnchor.Transform, "Transform");
		}
	}
}

#endif