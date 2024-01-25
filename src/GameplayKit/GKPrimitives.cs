//
// GKPrimitives.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#if NET
using Vector2 = global::System.Numerics.Vector2;
using Vector3 = global::System.Numerics.Vector3;
#else
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
#endif

namespace GameplayKit {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct GKBox {
		public Vector3 Min;
		public Vector3 Max;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct GKQuad {
		public Vector2 Min;
		public Vector2 Max;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct GKTriangle {
		Vector3 point1;
		Vector3 point2;
		Vector3 point3;
		public Vector3 [] Points {
			get {
				return new Vector3 [] { point1, point2, point3 };
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				if (value.Length != 3)
					throw new ArgumentOutOfRangeException (nameof (value), "The length of the Value array must be 3");
				point1 = value [0];
				point2 = value [1];
				point3 = value [2];
			}
		}
	}
}
