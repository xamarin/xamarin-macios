//
// Unit tests for ARFaceGeometry
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if XAMCORE_2_0 && __IOS__

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;

namespace MonoTouchFixtures.ARKit {

	class ARFaceGeometryPoker : ARFaceGeometry {

		GCHandle indicesArrayHandle;
		short [] indices;

		public ARFaceGeometryPoker () : base (IntPtr.Zero)
		{
		}

		public override nuint TriangleCount {
			get {
				// There are always 3x more 'TriangleIndices' than 'TriangleCount' since 'TriangleIndices' represents Triangles (set of three indices).
				// So 2 'TriangleCount' = 6 'TriangleIndices'.
				return 2;
			}
		}

		public unsafe override IntPtr GetTriangleIndexes ()
		{
			// Two triangles (set of 3 indices)
			indices = new short [] { 1, 2, 3, 4, 5, 6 };
			if (!indicesArrayHandle.IsAllocated)
				indicesArrayHandle = GCHandle.Alloc (indices, GCHandleType.Pinned);
			return indicesArrayHandle.AddrOfPinnedObject ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (indicesArrayHandle.IsAllocated)
				indicesArrayHandle.Free ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARFaceGeometryTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void TriangleIndicesTest ()
		{
			var face = new ARFaceGeometryPoker ();
			var indices = face.TriangleIndexes;
			Assert.AreEqual (new short [] { 1, 2, 3, 4, 5, 6 }, indices);
		}
	}
}

#endif // XAMCORE_2_0 && __IOS__
