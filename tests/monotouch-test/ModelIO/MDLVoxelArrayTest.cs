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
	public class MDLVoxelArrayTest
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void BoundingBoxTest ()
		{
			MDLAxisAlignedBoundingBox box = new MDLAxisAlignedBoundingBox (
				new Vector3 (4, 5, 6),
				new Vector3 (1, 2, 3)
			);
			using (var data = new NSData ()) {
				using (var obj = new MDLVoxelArray (data, box, 1.0f)) {
					Asserts.AreEqual (box, obj.BoundingBox, "BoundingBox");

					var extents = new MDLVoxelIndexExtent2 (
						new Vector4i (1, 2, 3, 4),
						new Vector4i (5, 6, 7, 8));
					var voxels = obj.GetVoxels (extents);
					Assert.IsNull (voxels, "GetVoxels");

					extents = obj.VoxelIndexExtent2;
					Assert.That (extents.MaximumExtent.X, Is.EqualTo (-1).Or.EqualTo (0), "MaxX");
					Assert.That (extents.MaximumExtent.Y, Is.EqualTo (-1).Or.EqualTo (0), "MaxY");
					Assert.That (extents.MaximumExtent.Z, Is.EqualTo (-1).Or.EqualTo (0), "MaxZ");
					Asserts.AreEqual (0, extents.MaximumExtent.W, "MaxW");
					Asserts.AreEqual (0, extents.MinimumExtent.X, "MinX");
					Asserts.AreEqual (0, extents.MinimumExtent.Y, "MinY");
					Asserts.AreEqual (0, extents.MinimumExtent.Z, "MinZ");
					Asserts.AreEqual (0, extents.MinimumExtent.W, "MinW");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
