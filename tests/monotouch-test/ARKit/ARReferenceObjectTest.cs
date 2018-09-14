//
// Unit tests for ARReferenceObject
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
	public class ARReferenceObjectTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void MarshallingTest ()
		{
			var model3 = new ARReferenceObject (NSUrl.FromFilename ("Model3.arobject"), out NSError error);
			Assert.AreEqual ("Model3", model3.Name, "Name");
			Assert.NotNull (model3.Center, "Center");
			Assert.NotNull (model3.Extent, "Extent");
			Assert.NotNull (model3.Scale, "Scale");
			Assert.NotNull (model3.ApplyTransform (MatrixFloat4x4.Identity), "ApplyTransform");
		}
	}
}

#endif