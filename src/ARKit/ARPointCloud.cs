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
using Vector3 = global::OpenTK.Vector3;

namespace XamCore.ARKit {
	public partial class ARPointCloud {

		public Vector3 [] Points {
			get {
				var count = (int)Count;
				var rv = new Vector3 [count];
				var ptr = (Vector3*)_GetPoints ();
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}
	}
}