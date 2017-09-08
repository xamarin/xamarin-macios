//
// VectorDouble2.cs:
//     This represents the native vector_double2 type
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
	public struct VectorDouble2 : IEquatable<VectorDouble2> {
		public double X;
		public double Y;

		public VectorDouble2 (double x, double y)
		{
			X = x;
			Y = y;
		}

		public static bool operator == (VectorDouble2 left, VectorDouble2 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (VectorDouble2 left, VectorDouble2 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Vector2d (VectorDouble2 value)
		{
			return new global::OpenTK.Vector2d (value.X, value.Y);
		}

		public static explicit operator VectorDouble2 (global::OpenTK.Vector2d value)
		{
			return new VectorDouble2 (value.X, value.Y);
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
			if (!(obj is VectorDouble2))
				return false;

			return Equals ((VectorDouble2) obj);
		}

		public bool Equals (VectorDouble2 other)
		{
			return X == other.X && Y == other.Y;
		}
	}
}
