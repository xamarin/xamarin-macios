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

#if !MONOMAC
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
				using (var obj = MDLMesh.CreateSphere (new Vector3 (1, 2, 3), new Vector2i (4, 5), MDLGeometryType.Triangles, true, null)) {
				}
				using (var obj = MDLMesh.CreateHemisphere (new Vector3 (1, 2, 3), new Vector2i (4, 5), MDLGeometryType.Triangles, true, true, null)) {
				}
				using (var obj = MDLMesh.CreateCapsule (new Vector3 (1, 2, 3), new Vector2i (4, 5), MDLGeometryType.Triangles, true, 10, null)) {
				}
				using (var obj = MDLMesh.CreateCone (new Vector3 (1, 2, 3), new Vector2i (4, 5), MDLGeometryType.Triangles, true, true, null)) {
				}
//				using (var obj = MDLMesh.CreateSubdividedMesh (new MDLMesh (), 0, 0)) {
//				}

			}
#endif
		}

		[Test]
		public void CreateBoxWithDimensonTest ()
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
		public void CreateBoxWithExtentTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector3i V3i = new Vector3i (4, 5, 6);

			using (var obj = MDLMesh.CreateBox (V3, V3i, MDLGeometryType.Triangles, true, null, MDLMesh.MDLMeshVectorType.Extent)) {
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
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
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
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo (9), "VertexCount");
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
#if MONOMAC
				if (TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 12)) {
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (18, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (26, obj.VertexCount, "VertexCount");
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
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Length");
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (13, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo (22), "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.AreEqual (1, obj.VertexBuffers.Length, "VertexBuffers Count");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo (16), "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateCapsuleTest ()
		{
			TestRuntime.AssertXcodeVersion (8,0);

			Vector3 V3 = new Vector3 (1, 2, 3);
			Vector2i V2i = new Vector2i (4, 5);

			using (var obj = MDLMesh.CreateCapsule (V3, V2i, MDLGeometryType.Triangles, true, 10, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.That (obj.VertexCount, Is.GreaterThanOrEqualTo (122), "VertexCount");
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual (36, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
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
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual (30, obj.VertexCount, "VertexCount");
				Assert.AreEqual (31, obj.VertexDescriptor.Attributes.Count, "VertexDescriptor Attributes Count");
				Assert.AreEqual (31, obj.VertexDescriptor.Layouts.Count, "VertexDescriptor Layouts Count");
			}
		}

		[Test]
		public void CreateIcosahedronTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			Vector3 V3 = new Vector3 (1, 2, 3);

			using (var obj = MDLMesh.CreateIcosahedron (V3, true, MDLGeometryType.Triangles, null)) {
				Assert.IsNotNull (obj, "obj");
				Assert.AreEqual (1, obj.Submeshes.Count, "Submeshes Count");
				Assert.That (obj.VertexBuffers.Length, Is.GreaterThanOrEqualTo (1), "VertexBuffers Count");
				Assert.AreEqual (12, obj.VertexCount, "VertexCount");
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
