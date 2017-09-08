//
// VectorDouble4.cs:
//     This represents the native vector_double4 type.
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
	public struct VectorDouble4 : IEquatable<VectorDouble4> {
		public double X;
		public double Y;
		public double Z;
		public double W;

		public VectorDouble4 (double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public static bool operator == (VectorDouble4 left, VectorDouble4 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorDouble4 left, VectorDouble4 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector4d (VectorDouble4 value)
		{
			return new global::OpenTK.Vector4d (value.X, value.Y, value.Z, value.W);
		}

		public static explicit operator VectorDouble4 (global::OpenTK.Vector4d value)
		{
			return new VectorDouble4 (value.X, value.Y, value.Z, value.W);
		}

		public override string ToString ()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}

		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode () ^ W.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is VectorDouble4))
				return false;

			return Equals ((VectorDouble4) obj);
		}

		public bool Equals (VectorDouble4 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}
	}
}
