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
using NUnit.Framework;

#if NET
using System.Numerics;
using Vector4i = global::CoreGraphics.NVector4i;
using Vector3d = global::CoreGraphics.NVector3d;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLVoxelArrayTest {
		[OneTimeSetUp]
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

#if NET
					var extents = new MDLVoxelIndexExtent (
#else
					var extents = new MDLVoxelIndexExtent2 (
#endif
						new Vector4i (1, 2, 3, 4),
						new Vector4i (5, 6, 7, 8));
					var voxels = obj.GetVoxels (extents);
					Assert.IsNull (voxels, "GetVoxels");

#if NET
					extents = obj.VoxelIndexExtent;
#else
					extents = obj.VoxelIndexExtent2;
#endif
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
