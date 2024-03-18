//
// VectorInt2.cs:
//     This represents the native vector_int2 type, which is 8 bytes.
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
	public struct NVector2i : IEquatable<NVector2i>
	{
		public int X;
		public int Y;

		public NVector2i (int x, int y)
		{
			X = x;
			Y = y;
		}

		public static bool operator == (NVector2i left, NVector2i right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector2i left, NVector2i right)
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
			if (!(obj is NVector2i vector))
				return false;

			return Equals (vector);
		}

		public bool Equals (NVector2i other)
		{
			return X == other.X && Y == other.Y;
		}

		public static NVector2i Zero {
			get => default;
		}
	}
}
#endif // NET
