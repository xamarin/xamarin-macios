//
// MDLAssert Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft Inc.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using CoreGraphics;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
#if !__TVOS__
using MonoTouch.MultipeerConnectivity;
#endif
using MonoTouch.UIKit;
using MonoTouch.ModelIO;
using MonoTouch.ObjCRuntime;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO
{

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLAssetTest
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void BoundingBoxTest ()
		{
			using (var obj = new MDLAsset ()) {
				MDLAxisAlignedBoundingBox box = new MDLAxisAlignedBoundingBox (
					new Vector3 (-1, -1, -1),
					new Vector3 (0, 0, 0)
				);
				Asserts.AreEqual (box, obj.BoundingBox, "BoundingBox");
			}
		}
	}
}

#endif // !__WATCHOS__
