//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native vector_float4 type
//

using System;
using System.Runtime.InteropServices;

namespace Simd
{
	[StructLayout (LayoutKind.Sequential)]
	public struct VectorFloat4 : IEquatable<VectorFloat4>
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public VectorFloat4 (float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public static bool operator == (VectorFloat4 left, VectorFloat4 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorFloat4 left, VectorFloat4 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector4 (VectorFloat4 value)
		{
			return new global::OpenTK.Vector4 (value.X, value.Y, value.Z, value.W);
		}

		public static explicit operator VectorFloat4 (global::OpenTK.Vector4 value)
		{
			return new VectorFloat4 (value.X, value.Y, value.Z, value.W);
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
			if (!(obj is VectorFloat4))
				return false;

			return Equals ((VectorFloat4) obj);
		}

		public bool Equals (VectorFloat4 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}
	}
}
