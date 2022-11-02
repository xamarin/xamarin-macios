//
// Unit tests for ARPlaneGeometry
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2018 Microsoft. All rights reserved.
//

#if HAS_ARKIT

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

#if NET
using VectorFloat2 = global::System.Numerics.Vector2;
using VectorFloat3 = global::CoreGraphics.NVector3;
#else
using VectorFloat2 = global::OpenTK.Vector2;
using VectorFloat3 = global::OpenTK.NVector3;
#endif

namespace MonoTouchFixtures.ARKit {

	class ARPlaneGeometryPoker : ARPlaneGeometry {

		GCHandle verticesArrayHandle;
		GCHandle textureCoordinatesArrayHandle;
		GCHandle indicesArrayHandle;
		GCHandle boundaryVerticesArrayHandle;
		VectorFloat3 [] vertices;
		VectorFloat2 [] textureCoordinates;
		short [] indices;
		VectorFloat3 [] boundaryVertices;

		public ARPlaneGeometryPoker () : base (IntPtr.Zero)
		{
		}

		public override nuint VertexCount => 2;

		public override nuint TextureCoordinateCount => 2;

		// There are always 3x more 'TriangleIndices' than 'TriangleCount' since 'TriangleIndices' represents Triangles (set of three indices).
		// So 2 'TriangleCount' = 6 'TriangleIndices'.
		public override nuint TriangleCount => 2;

		public override nuint BoundaryVertexCount => 2;

		public override IntPtr GetRawVertices ()
		{
			vertices = new VectorFloat3 [] { new VectorFloat3 (1, 2, 3), new VectorFloat3 (4, 5, 6) };
			if (!verticesArrayHandle.IsAllocated)
				verticesArrayHandle = GCHandle.Alloc (vertices, GCHandleType.Pinned);
			return verticesArrayHandle.AddrOfPinnedObject ();
		}

		public override IntPtr GetRawTextureCoordinates ()
		{
			textureCoordinates = new VectorFloat2 [] { new VectorFloat2 (1, 2), new VectorFloat2 (3, 4) };
			if (!textureCoordinatesArrayHandle.IsAllocated)
				textureCoordinatesArrayHandle = GCHandle.Alloc (textureCoordinates, GCHandleType.Pinned);
			return textureCoordinatesArrayHandle.AddrOfPinnedObject ();
		}

		public override IntPtr GetRawTriangleIndices ()
		{
			// Two triangles (set of 3 indices)
			indices = new short [] { 1, 2, 3, 4, 5, 6 };
			if (!indicesArrayHandle.IsAllocated)
				indicesArrayHandle = GCHandle.Alloc (indices, GCHandleType.Pinned);
			return indicesArrayHandle.AddrOfPinnedObject ();
		}

		public override IntPtr GetRawBoundaryVertices ()
		{
			boundaryVertices = new VectorFloat3 [] { new VectorFloat3 (1, 2, 3), new VectorFloat3 (4, 5, 6) };
			if (!boundaryVerticesArrayHandle.IsAllocated)
				boundaryVerticesArrayHandle = GCHandle.Alloc (boundaryVertices, GCHandleType.Pinned);
			return boundaryVerticesArrayHandle.AddrOfPinnedObject ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (verticesArrayHandle.IsAllocated)
				verticesArrayHandle.Free ();
			if (textureCoordinatesArrayHandle.IsAllocated)
				textureCoordinatesArrayHandle.Free ();
			if (indicesArrayHandle.IsAllocated)
				indicesArrayHandle.Free ();
			if (boundaryVerticesArrayHandle.IsAllocated)
				boundaryVerticesArrayHandle.Free ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARPlaneGeometryTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

		[Test]
		public void VerticesTest ()
		{
			var face = new ARPlaneGeometryPoker ();
			var vertices = face.GetVertices ();
			Assert.AreEqual (new VectorFloat3 (1, 2, 3), vertices [0], "Vertex 1");
			Assert.AreEqual (new VectorFloat3 (4, 5, 6), vertices [1], "Vertex 2");
		}

		[Test]
		public void TextureCoordinatesTest ()
		{
			var face = new ARPlaneGeometryPoker ();
			var textureCoordinates = face.GetTextureCoordinates ();
			Assert.AreEqual (new VectorFloat2 (1, 2), textureCoordinates [0], "Texture Coordinates 1");
			Assert.AreEqual (new VectorFloat2 (3, 4), textureCoordinates [1], "Texture Coordinates 2");
		}

		[Test]
		public void TriangleIndicesTest ()
		{
			var face = new ARPlaneGeometryPoker ();
			Assert.AreEqual (new short [] { 1, 2, 3, 4, 5, 6 }, face.GetTriangleIndices ());
		}

		[Test]
		public void BoundaryVerticesTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 3);

			var face = new ARPlaneGeometryPoker ();
			var boundaryVertices = face.GetBoundaryVertices ();
			Assert.AreEqual (new VectorFloat3 (1, 2, 3), boundaryVertices [0], "Boundary Vertex 1");
			Assert.AreEqual (new VectorFloat3 (4, 5, 6), boundaryVertices [1], "Boundary Vertex 2");
		}
	}
}

#endif // HAS_ARKIT
