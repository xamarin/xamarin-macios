//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native vector_float2 type
//

using System;
using System.Runtime.InteropServices;

namespace Simd
{
	[StructLayout (LayoutKind.Sequential)]
	public struct VectorFloat2 : IEquatable<VectorFloat2>
	{
		public float X;
		public float Y;

		public VectorFloat2 (float x, float y)
		{
			X = x;
			Y = y;
		}

		public static bool operator == (VectorFloat2 left, VectorFloat2 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorFloat2 left, VectorFloat2 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector2 (VectorFloat2 value)
		{
			return new global::OpenTK.Vector2 (value.X, value.Y);
		}

		public static explicit operator VectorFloat2 (global::OpenTK.Vector2 value)
		{
			return new VectorFloat2 (value.X, value.Y);
		}

		public override string ToString ()
		{
			return $"({X}, {Y})";
		}

		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is VectorFloat2))
				return false;

			return Equals ((VectorFloat2) obj);
		}

		public bool Equals (VectorFloat2 other)
		{
			return X == other.X && Y == other.Y;
		}
	}
}
