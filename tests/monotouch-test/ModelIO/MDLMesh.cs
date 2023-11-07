//
// MDLLight Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

#if NET
using System.Numerics;
using Vector2i = global::CoreGraphics.NVector2i;
using Vector3i = global::CoreGraphics.NVector3i;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLMeshTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void CreateBoxWithDimensonTest ()
		{
			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector3i V3i = new Vector3i (4, 5, 6);

			using (var obj = MDLMesh.CreateBox (V3, V3i, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.5f, 1, 1.5f), MinBounds = new Vector3 (-0.5f, -1, -1.5f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.AreEqual (TestRuntime.CheckXcodeVersion (7, 3) ? (nuint) 214 : (nuint) 24, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateBoxWithExtentTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector3i V3i = new Vector3i (4, 5, 6);

			using (var obj = MDLMesh.CreateBox (V3, V3i, MDLGeometryType.Triangles, true, null, MDLMesh.MDLMeshVectorType.Extent)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.5f, 1, 1.5f), MinBounds = new Vector3 (-0.5f, -1, -1.5f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.AreEqual (TestRuntime.CheckXcodeVersion (7, 3) ? (nuint) 214 : (nuint) 24, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreatePlaneTest ()
		{
			var V2 = new Vector2 (3, 3);
			var V2i = new Vector2i (3, 3);

			using (var obj = MDLMesh.CreatePlane (V2, V2i, MDLGeometryType.Triangles, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (1.5f, 0, 1.5f), MinBounds = new Vector3 (-1.5f, 0, -1.5f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual ((nuint) 16, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateEllipsoidTest ()
		{
			Vector3 V3 = new Vector3 (1, 1, 1);

			using (var obj = MDLMesh.CreateEllipsoid (V3, 3, 3, MDLGeometryType.Triangles, true, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 1f, 0.75f), MinBounds = new Vector3 (-0.433012784f, 0.49999997f, -0.75000006f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo ((nuint) 9), "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateCylindroidTest ()
		{
			var V2 = new Vector2 (1, 1);

			using (var obj = MDLMesh.CreateCylindroid (1, V2, 3, 1, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
#if MONOMAC
				if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 12)) {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 0.5f, 1f), MinBounds = new Vector3 (-0.866025388f, -0.5f, -0.5f) }, obj.BoundingBox, "BoundingBox");
				} else {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (1f, 0.5f, 1f), MinBounds = new Vector3 (-0.866025388f, -0.5f, -0.866025388f) }, obj.BoundingBox, "BoundingBox");
				}
#else
				if (TestRuntime.CheckXcodeVersion (8, 2)) {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 0.5f, 1f), MinBounds = new Vector3 (-0.866025388f, -0.5f, -0.5f) }, obj.BoundingBox, "BoundingBox");
				} else if (TestRuntime.CheckXcodeVersion (8, 0)) {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 1.0f, 1f), MinBounds = new Vector3 (-0.866025388f, -1.0f, -0.5f) }, obj.BoundingBox, "BoundingBox");
				} else {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (1f, 0.5f, 1f), MinBounds = new Vector3 (-0.866025388f, -0.5f, -0.866025388f) }, obj.BoundingBox, "BoundingBox");
				}
#endif
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual ((nuint) 18, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateCylinderTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var V3 = new Vector3 (1, 1, 1);
			var V2i = new Vector2i (3, 3);

			using (var obj = MDLMesh.CreateCylinder (V3, V2i, true, true, true, MDLGeometryType.Triangles, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Length");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual ((nuint) 26, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateEllipticalConeTest ()
		{
			var V2 = new Vector2 (1, 1);

			using (var obj = MDLMesh.CreateEllipticalCone (5, V2, 3, 1, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Length");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual ((nuint) 13, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateSphereTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector2i V2i = new Vector2i (4, 5);

			using (var obj = MDLMesh.CreateSphere (V3, V2i, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.9510565f, 2, 2.85317f), MinBounds = new Vector3 (-0.9510565f, -2, -2.85317f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo ((nuint) 22), "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateHemisphereTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector2i V2i = new Vector2i (4, 5);

			using (var obj = MDLMesh.CreateHemisphere (V3, V2i, MDLGeometryType.Triangles, true, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.9510565f, 2, 2.85317f), MinBounds = new Vector3 (-0.9510565f, 0.6180339f, -2.85317f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo ((nuint) 16), "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateCapsuleTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector2i V2i = new Vector2i (4, 5);

			using (var obj = MDLMesh.CreateCapsule (V3, V2i, MDLGeometryType.Triangles, true, 10, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo ((nuint) 122), "VertexCount");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateConeTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector2i V2i = new Vector2i (4, 5);

			using (var obj = MDLMesh.CreateCone (V3, V2i, MDLGeometryType.Triangles, true, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual ((nuint) 36, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreatePaneTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector2i V2i = new Vector2i (4, 5);

			using (var obj = MDLMesh.CreatePlane (V3, V2i, MDLGeometryType.Triangles, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual ((nuint) 30, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateIcosahedronTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);

			using (var obj = MDLMesh.CreateIcosahedron (V3, true, MDLGeometryType.Triangles, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.AreEqual ((nuint) 1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual ((nuint) 12, obj.VertexCount, "VertexCount");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual ((nuint) 31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		//		FIXME: figure out valid input arguments for GenerateAmbientOcclusionTexture
		//		[Test]
		//		public void GenerateAmbientOcclusionTextureTest ()
		//		{
		//			var V2 = new Vector2 (1, 1);
		//			var V2i = new Vector2i (1, 2);
		//
		//			using (var obj = MDLMesh.CreateEllipticalCone (5, V2, 3, 1, MDLGeometryKind.Triangles, true, null)) {
		//				Assert.IsTrue (obj.GenerateAmbientOcclusionTexture (V2i, 1, 1, new MDLObject[] { }, "vname", "mname"), "GenerateAmbientOcclusionTexture");
		//			}
		//		}
		//
		//		FIXME: figure out valid input arguments for GenerateLightMapTexture
		//		[Test]
		//		public void GenerateLightMapTextureTest ()
		//		{
		//			var V2 = new Vector2 (1, 1);
		//			var V2i = new Vector2i (1, 2);
		//
		//			using (var obj = MDLMesh.CreateEllipticalCone (5, V2, 3, 1, MDLGeometryKind.Triangles, true, null)) {
		//				Assert.IsTrue (obj.GenerateLightMapTexture (V2i, new MDLLight[] {}, new MDLObject[] { }, "vname", "mname"), "GenerateLightMapTexture");
		//			}
		//		}
	}
}

#endif // !__WATCHOS__
