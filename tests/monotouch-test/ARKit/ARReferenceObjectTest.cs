//
// Unit tests for ARReferenceObject
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
	public class ARReferenceObjectTest {

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
			TestRuntime.AssertNotSimulator (); // The Objective-C constructor is just stubbed out to return NULL in the simulator, so this test only works on device.

			var model3 = new ARReferenceObject (NSUrl.FromFilename ("Model3.arobject"), out NSError error);
			Assert.IsNull (error, "Error");
			Assert.AreEqual ("Model3", model3.Name, "Name");
			Assert.NotNull (model3.Center, "Center");
			Assert.NotNull (model3.Extent, "Extent");
			Assert.NotNull (model3.Scale, "Scale");
			Assert.NotNull (model3.ApplyTransform (MatrixFloat4x4.Identity), "ApplyTransform");
		}
	}
}

#endif // HAS_ARKIT
