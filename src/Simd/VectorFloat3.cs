//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native vector_float3 type, which is 16 bytes.
//

using System;
using System.Runtime.InteropServices;

namespace Simd
{
	[StructLayout (LayoutKind.Sequential)]
	public struct VectorFloat3 : IEquatable<VectorFloat3>
	{
		public float X;
		public float Y;
		public float Z;
		float dummy;

		public VectorFloat3 (float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
			dummy = 0;
		}

		public static bool operator == (VectorFloat3 left, VectorFloat3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorFloat3 left, VectorFloat3 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector3 (VectorFloat3 value)
		{
			return new global::OpenTK.Vector3 (value.X, value.Y, value.Z);
		}

		public static explicit operator VectorFloat3 (global::OpenTK.Vector3 value)
		{
			return new VectorFloat3 (value.X, value.Y, value.Z);
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
			if (!(obj is VectorFloat3))
				return false;

			return Equals ((VectorFloat3) obj);
		}

		public bool Equals (VectorFloat3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
	}
}
