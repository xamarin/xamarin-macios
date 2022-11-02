//
// ARPointCloud.cs: Nicer code for ARPointCloud
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
#if NET
using Vector3 = global::CoreGraphics.NVector3;
#else
using Vector3 = global::OpenTK.NVector3;
#endif

#nullable enable

namespace ARKit {
	public partial class ARPointCloud {

		public unsafe Vector3 [] Points {
			get {
				var count = (int) Count;
				var rv = new Vector3 [count];
				var ptr = (Vector3*) GetRawPoints ();
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}

		public unsafe ulong [] Identifiers {
			get {
				var count = (int) Count;
				var rv = new ulong [count];
				var ptr = (ulong*) GetRawIdentifiers ();
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}
	}
}
