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
#if XAMCORE_2_0
using Foundation;
using UIKit;
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
#else
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

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLMeshTest {
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				object dummy;
				using (var obj = MDLMesh.CreateBox (Vector3.Zero, Vector3i.Zero, MDLGeometryType.Triangles, true, null)) {
					obj.AddAttribute ("foo", MDLVertexFormat.Char);
//					obj.AddNormals (null, 0); // throws NSInvalidArgumentException, need to figure out valid arguments
//					obj.AddTangentBasis ("foo", "bar", "zap"); // throws "Need float or half UV components Need float or half UV components"
//					obj.AddTangentBasisWithNormals ("foo", "bar", "zap"); // throws "Need float or half UV components Reason: Need float or half UV components"
					dummy = obj.BoundingBox;
//					obj.GenerateAmbientOcclusionTexture (1, 1, new MDLObject [] { }, "name", "name");
//					obj.GenerateAmbientOcclusionVertexColors (1, 1, new MDLObject[] {}, "name");
//					obj.GenerateAmbientOcclusionVertexColors (1.1, 1, new MDLObject[] [] { }, "name");
//					obj.GenerateLightMapTexture (Vector2i.Zero, new MDLLight[] {}, new MDLObject[] {}, "str", "str");
//					obj.GenerateLightMapVertexColors (new MDLLight[] { }, new MDLObject[] { }, "v");
					obj.MakeVerticesUnique ();
					dummy = obj.Submeshes;
					dummy = obj.VertexBuffers;
					dummy = obj.VertexCount;
					dummy = obj.VertexDescriptor;
				}

				using (var obj = MDLMesh.CreateCylindroid (1, Vector2.Zero, 3, 0, MDLGeometryType.Triangles, false, null)) {
				}
				using (var obj = MDLMesh.CreateEllipsoid (Vector3.Zero, 3, 2, MDLGeometryType.Triangles, false, false, null)) {
				}
				using (var obj = MDLMesh.CreateEllipticalCone (0, Vector2.Zero, 3, 1, MDLGeometryType.Triangles, false, null)) {
				}
				using (var obj = MDLMesh.CreateIcosahedron (0, false, null)) {
				}
				using (var obj = MDLMesh.CreatePlane (new Vector2 (1, 1), new Vector2i (1, 1), MDLGeometryType.Triangles, null)) {
				}
//				using (var obj = MDLMesh.CreateSubdividedMesh (new MDLMesh (), 0, 0)) {
//				}

			}
		}

		[Test]
		public void CreateBoxTest ()
		{
			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector3i V3i = new Vector3i (4, 5, 6);

			using (var obj = MDLMesh.CreateBox (V3, V3i, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.5f, 1, 1.5f), MinBounds = new Vector3 (-0.5f, -1, -1.5f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.AreEqual (TestRuntime.CheckXcodeVersion (7, 3) ? 214 : 24, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				var vb = TestRuntime.CheckXcodeVersion (8,0) ? 3 : 1;
				Assert.AreEqual (vb, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.AreEqual (16, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateEllipsoidTest ()
		{
			Vector3 V3 = new Vector3 (1, 1, 1);

			using (var obj = MDLMesh.CreateEllipsoid (V3, 3, 3, MDLGeometryType.Triangles, true, true, null)) {
				Assert.IsNotNull (obj, "obj");
				Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 1f, 0.75f), MinBounds = new Vector3 (-0.433012784f, 0.49999997f, -0.75000006f) }, obj.BoundingBox, "BoundingBox");
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.AreEqual (9, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateCylindroidTest ()
		{
			var V2 = new Vector2 (1, 1);

			using (var obj = MDLMesh.CreateCylindroid (1, V2, 3, 1, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
				if (TestRuntime.CheckXcodeVersion (8, 0)) {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 1.0f, 1f), MinBounds = new Vector3 (-0.866025388f, -1.0f, -0.5f) }, obj.BoundingBox, "BoundingBox");
					Assert.AreEqual (3, obj.VertexBuffers.Length, "VertexBuffers Count");
				} else {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (1f, 0.5f, 1f), MinBounds = new Vector3 (-0.866025388f, -0.5f, -0.866025388f) }, obj.BoundingBox, "BoundingBox");
					Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				}
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (18, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateEllipticalConeTest ()
		{
			var V2 = new Vector2 (1, 1);

			using (var obj = MDLMesh.CreateEllipticalCone (5, V2, 3, 1, MDLGeometryType.Triangles, true, null)) {
				Assert.IsNotNull (obj, "obj");
				if (TestRuntime.CheckXcodeVersion (8, 0)) {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.433012783f, 4.5f, 0.5f), MinBounds = new Vector3 (-0.433012783f, -0.5f, -0.25f) }, obj.BoundingBox, "BoundingBox");
					Assert.AreEqual (3, obj.VertexBuffers.Length, "VertexBuffers Count");
				} else {
					Asserts.AreEqual (new MDLAxisAlignedBoundingBox { MaxBounds = new Vector3 (0.866025448f, 0f, 1f), MinBounds = new Vector3 (-0.866025388f, -5f, -0.50000006f) }, obj.BoundingBox, "BoundingBox");
					Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				}
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (13, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
