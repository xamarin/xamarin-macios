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
			if (TestRuntime.IsSimulator && TestRuntime.CheckXcodeVersion (12, 0))
				Assert.Ignore ("broken with beta 1 - can't instantiate the object");
#if __MACCATALYST__
			Assert.Ignore ("The [ARReferenceObject initWithArchiveURL:error:] selector is hardcoded to just return nil."); // true as of macOS 12.2.1
#endif
			var model3 = new ARReferenceObject (new NSUrl (NSBundle.MainBundle.ResourceUrl.AbsoluteString + "Model3.arobject"), out NSError error);
			Assert.AreEqual ("Model3", model3.Name, "Name");
			Assert.NotNull (model3.Center, "Center");
			Assert.NotNull (model3.Extent, "Extent");
			Assert.NotNull (model3.Scale, "Scale");
			Assert.NotNull (model3.ApplyTransform (MatrixFloat4x4.Identity), "ApplyTransform");
		}
	}
}

#endif // HAS_ARKIT
