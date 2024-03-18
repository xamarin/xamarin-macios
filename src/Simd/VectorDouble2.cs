//
// VectorDouble2.cs:
//     This represents the native vector_double2 type, which is 16 bytes.
//
//
// Authors:
//     Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2021 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;

// This type does not come from the CoreGraphics framework; it's defined in /usr/include/simd/vector_types.h
#if NET
namespace CoreGraphics
{
	[StructLayout (LayoutKind.Sequential)]
	public struct NVector2d : IEquatable<NVector2d>
	{
		public double X;
		public double Y;

		public NVector2d (double x, double y)
		{
			X = x;
			Y = y;
		}

		public static bool operator == (NVector2d left, NVector2d right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector2d left, NVector2d right)
		{
			return !left.Equals (right);
		}

		public override string ToString ()
		{
			return $"({X}, {Y})";
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine(X, Y);
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is NVector2d vector))
				return false;

			return Equals (vector);
		}

		public bool Equals (NVector2d other)
		{
			return X == other.X && Y == other.Y;
		}

		public static NVector2d Zero {
			get => default;
		}
	}
}
#endif // NET
