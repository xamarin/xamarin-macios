//
// VectorDouble3.cs:
//     This represents the native vector_double3 type, which is 32 bytes.
//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//     Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) 2017 Microsoft Inc
//

using System;
using System.Runtime.InteropServices;

namespace Simd {
	[StructLayout (LayoutKind.Sequential)]
	public struct VectorDouble3 : IEquatable<VectorDouble3> {
		public double X;
		public double Y;
		public double Z;
		double dummy;

		public VectorDouble3 (double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
			dummy = 0;
		}

		public static bool operator == (VectorDouble3 left, VectorDouble3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorDouble3 left, VectorDouble3 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector3d (VectorDouble3 value)
		{
			return new global::OpenTK.Vector3d (value.X, value.Y, value.Z);
		}

		public static explicit operator VectorDouble3 (global::OpenTK.Vector3d value)
		{
			return new VectorDouble3 (value.X, value.Y, value.Z);
		}

		public override string ToString ()
		{
			return $"({X}, {Y}, {Z})";
		}

		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is VectorDouble3))
				return false;

			return Equals ((VectorDouble3) obj);
		}

		public bool Equals (VectorDouble3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
	}
}
