//
// ARPointCloud.cs: Nicer code for ARPointCloud
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using System.Runtime.InteropServices;
using Vector3 = global::OpenTK.Vector3;

namespace XamCore.ARKit {
	public partial class ARPointCloud {

		public unsafe Vector3 [] Points {
			get {
				var count = (int)Count;
				var rv = new Vector3 [count];
				var ptr = GetRawPoints ();
				for (int i = 0; i < count; i++) {
					rv [i] = *(Vector3 *) ptr;
					ptr += 16; // 3 floats + 1 padding float = 16 bytes
				}
				return rv;
			}
		}

		public unsafe ulong [] Identifiers {
			get {
				var count = (int)Count;
				var rv = new ulong [count];
				var ptr = (ulong*)GetRawIdentifiers ();
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}
	}
}

#endif