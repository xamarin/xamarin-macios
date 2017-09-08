//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native vector_int4 type
//

using System;
using System.Runtime.InteropServices;

namespace Simd
{
	[StructLayout (LayoutKind.Sequential)]
	public struct VectorInt4 : IEquatable<VectorInt4>
	{
		public int X;
		public int Y;
		public int Z;
		public int W;

		public VectorInt4 (int x, int y, int z, int w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public static bool operator == (VectorInt4 left, VectorInt4 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorInt4 left, VectorInt4 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector4i (VectorInt4 value)
		{
			return new global::OpenTK.Vector4i (value.X, value.Y, value.Z, value.W);
		}

		public static explicit operator VectorInt4 (global::OpenTK.Vector4i value)
		{
			return new VectorInt4 (value.X, value.Y, value.Z, value.W);
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
			if (!(obj is VectorInt4))
				return false;

			return Equals ((VectorInt4) obj);
		}

		public bool Equals (VectorInt4 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}
	}
}
