//
// ARPlaneGeometry.cs: Nicer code for ARPlaneGeometry
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2018 Microsoft Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using System.Runtime.InteropServices;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.NVector3;

namespace XamCore.ARKit {
	public partial class ARPlaneGeometry {

		// Using GetXXX methods so it's similar to the ARFaceGeometry methods.
		public unsafe Vector3 [] GetVertices ()
		{
			var count = (int)VertexCount;
			var rv = new Vector3 [count];
			var ptr = (Vector3 *) GetRawVertices ();
			for (int i = 0; i < count; i++)
				rv [i] = *ptr++;
			return rv;
		}

		// Using GetXXX methods so it's similar to the ARFaceGeometry methods.
		public unsafe Vector2 [] GetTextureCoordinates ()
		{
			var count = (int)TextureCoordinateCount;
			var rv = new Vector2 [count];
			var ptr = (Vector2 *) GetRawTextureCoordinates ();
			for (int i = 0; i < count; i++)
				rv [i] = *ptr++;
			return rv;
		}

		// Using GetXXX methods so it's similar to the ARFaceGeometry methods.
		public unsafe short [] GetTriangleIndices ()
		{
			// There are always 3x more 'TriangleIndices' than 'TriangleCount' since 'TriangleIndices' represents Triangles (set of three indices).
			var count = (int)TriangleCount * 3;
			var rv = new short [count];
			var ptr = (short *) GetRawTriangleIndices ();
			for (int i = 0; i < count; i++)
				rv [i] = *ptr++;
			return rv;
		}

		// Using GetXXX methods so it's similar to the ARFaceGeometry methods.
		public unsafe Vector3 [] GetBoundaryVertices ()
		{
			var count = (int)BoundaryVertexCount;
			var rv = new Vector3 [count];
			var ptr = (Vector3 *) GetRawBoundaryVertices ();
			for (int i = 0; i < count; i++)
				rv [i] = *ptr++;
			return rv;
		}
	}
}

#endif
