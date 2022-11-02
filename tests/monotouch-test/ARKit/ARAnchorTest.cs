//
// Unit tests for ARAnchor
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
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARAnchorTest {

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
			var faceAnchor = new ARAnchor ("My Anchor", MatrixFloat4x4.Identity);
			Assert.AreEqual (MatrixFloat4x4.Identity, faceAnchor.Transform, "Transform");
		}
	}
}

#endif // HAS_ARKIT
