//
// ARSkeleton3D.cs: Nicer code for ARSkeleton3D
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2019 Microsoft Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
#if NET
using Matrix4 = global::CoreGraphics.NMatrix4;
#else
using Matrix4 = global::OpenTK.NMatrix4;
#endif

#nullable enable

namespace ARKit {
	public partial class ARSkeleton3D {

		public unsafe Matrix4 [] JointModelTransforms {
			get {
				var count = (int) JointCount;
				var rv = new Matrix4 [count];
				var ptr = (Matrix4*) RawJointModelTransforms;
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}

		public unsafe Matrix4 [] JointLocalTransforms {
			get {
				var count = (int) JointCount;
				var rv = new Matrix4 [count];
				var ptr = (Matrix4*) RawJointLocalTransforms;
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}
	}
}
