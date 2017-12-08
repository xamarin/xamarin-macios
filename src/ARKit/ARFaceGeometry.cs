//
// ARFaceGeometry.cs: Nicer code for ARFaceGeometry
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using System.Runtime.InteropServices;

namespace XamCore.ARKit {
	public partial class ARFaceGeometry {

		// Calling this one 'TriangleIndexes' so it doesn't clash with 'TriangleIndices'.
		// We could use a method here but a property is more consistent with other ARKit APIs like ARPointCloud.
		public unsafe short [] TriangleIndexes {
			get {
				// There are always 3x more 'TriangleIndices' than 'TriangleCount' since 'TriangleIndices' represents Triangles (set of three indices).
				var count = (int)TriangleCount * 3;
				var rv = new short [count];
				var ptr = (short *) GetTriangleIndexes ();
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}
	}
}

#endif