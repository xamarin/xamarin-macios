//
// ARSkeleton2D.cs: Nicer code for ARSkeleton2D
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2019 Microsoft Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
#if NET
using Vector2 = global::System.Numerics.Vector2;
#else
using Vector2 = global::OpenTK.Vector2;
#endif

using ObjCRuntime;

#nullable enable

namespace ARKit {
	public partial class ARSkeleton2D {

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (13,0)]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
#endif
		public unsafe Vector2 [] JointLandmarks {
			get {
				var count = (int)JointCount;
				var rv = new Vector2 [count];
				var ptr = (Vector2 *) RawJointLandmarks;
				for (int i = 0; i < count; i++)
					rv [i] = *ptr++;
				return rv;
			}
		}
	}
}
