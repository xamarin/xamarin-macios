//
// GKPath.cs: Implements some nicer methods for GKPath
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using Foundation;
using ObjCRuntime;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using System.Runtime.InteropServices;

namespace GameplayKit {
	public partial class GKPath {

		public static GKPath FromPoints (Vector2[] points, float radius, bool cyclical)
		{
			if (points == null)
				throw new ArgumentNullException ("points");

			var buffer = IntPtr.Zero;
			try {
				PrepareBuffer (out buffer, ref points);

				return FromPoints (buffer, (nuint)points.Length, radius, cyclical);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		[DesignatedInitializer]
		public GKPath (Vector2 [] points, float radius, bool cyclical)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			
			var buffer = IntPtr.Zero;
			try {
				PrepareBuffer (out buffer, ref points);

				Handle = InitWithPoints (buffer, (nuint)points.Length, radius, cyclical);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		public static GKPath FromPoints (Vector3 [] points, float radius, bool cyclical)
		{
			if (points == null)
				throw new ArgumentNullException ("points");

			var buffer = IntPtr.Zero;
			try {
				PrepareBuffer (out buffer, ref points);

				return FromFloat3Points (buffer, (nuint) points.Length, radius, cyclical);
			}
			finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		public GKPath (Vector3 [] points, float radius, bool cyclical)
		{
			if (points == null)
				throw new ArgumentNullException ("points");

			var buffer = IntPtr.Zero;
			try {
				PrepareBuffer (out buffer, ref points);

				Handle = InitWithFloat3Points (buffer, (nuint) points.Length, radius, cyclical);
			}
			finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		static void PrepareBuffer<T> (out IntPtr buffer, ref T[] points) where T : struct
		{
			var type = typeof (T);
			// Vector3 is 12 bytes but vector_float3 is 16
			var size = type == typeof (Vector3) ? 16 : Marshal.SizeOf (type);
			var length = points.Length * size;
			buffer = Marshal.AllocHGlobal (length);

			for (int i = 0; i < points.Length; i++)
				Marshal.StructureToPtr (points [i], IntPtr.Add (buffer, i * size), false);
		}
	}
}
#endif