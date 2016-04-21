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
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using Vector2 = global::OpenTK.Vector2;
using System.Runtime.InteropServices;

namespace XamCore.GameplayKit {
	public partial class GKPath {

		public static GKPath FromPoints (Vector2[] points, float radius, bool cyclical)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			
			var size = Marshal.SizeOf (typeof (Vector2));
			var length = points.Length * size;
			var buffer = Marshal.AllocHGlobal (length);

			try {
				for (int i = 0; i < points.Length; i++)
					Marshal.StructureToPtr (points[i], IntPtr.Add (buffer, i * size), false);

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
			
			var size = Marshal.SizeOf (typeof (Vector2));
			var length = points.Length * size;
			var buffer = Marshal.AllocHGlobal (length);

			try {
				for (int i = 0; i < points.Length; i++)
					Marshal.StructureToPtr (points[i], IntPtr.Add (buffer, i * size), false);

				Handle = InitWithPoints (buffer, (nuint)points.Length, radius, cyclical);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}
	}
}
#endif