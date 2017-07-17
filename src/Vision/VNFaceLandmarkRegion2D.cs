//
// VNFaceLandmarkRegion2D.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using Vector2 = global::OpenTK.Vector2;

namespace XamCore.Vision {
	public partial class VNFaceLandmarkRegion2D {

		public Vector2 this [nuint index] {
			get { return GetPoint (index); }
		}

		public virtual Vector2 [] Points {
			get {
				var ret = _GetPoints ();
				if (ret == IntPtr.Zero)
					return null;

				unsafe {
					var count = (int) PointCount;
					var rv = new Vector2 [count];
					var ptr = (Vector2*) ret;
					for (int i = 0; i < count; i++)
						rv [i] = *ptr++;
					return rv;
				}
			}
		}
	}
}
#endif
