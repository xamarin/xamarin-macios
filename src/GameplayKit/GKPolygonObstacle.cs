//
// GKPolygonObstacle.cs: Implements some nicer methods for GKPolygonObstacle
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
using System.Runtime.InteropServices;

namespace GameplayKit {
	public partial class GKPolygonObstacle {

		public static GKPolygonObstacle FromPoints (Vector2 [] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			
			var size = Marshal.SizeOf (typeof (Vector2));
			var length = points.Length * size;
			var buffer = Marshal.AllocHGlobal (length);

			try {
				for (int i = 0; i < points.Length; i++)
					Marshal.StructureToPtr (points[i], IntPtr.Add (buffer, i * size), false);

				return FromPoints (buffer, (nuint)points.Length);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		[ThreadStatic]
		static IntPtr ctor_pointer;

		static unsafe IntPtr GetPointer (Vector2[] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");

			if (ctor_pointer != IntPtr.Zero) {
				// This can occur of a previous call to the base ctor threw an exception
				Marshal.FreeHGlobal (ctor_pointer);
				ctor_pointer = IntPtr.Zero;
			}

			var size = Marshal.SizeOf (typeof (Vector2));
			var length = points.Length * size;
			var buffer = Marshal.AllocHGlobal (length);

			for (int i = 0; i < points.Length; i++)
				Marshal.StructureToPtr (points[i], IntPtr.Add (buffer, i * size), false);

			ctor_pointer = buffer;
			return ctor_pointer = buffer;
		}

		public unsafe GKPolygonObstacle (Vector2 [] points)
			: this (GetPointer (points), (nuint) points.Length)
		{
			if (ctor_pointer != IntPtr.Zero) {
				Marshal.FreeHGlobal (ctor_pointer);
				ctor_pointer = IntPtr.Zero;
			}
		}
	}
}
#endif
