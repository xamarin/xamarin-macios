//
// VectorInt4.cs
//     This represents the native vector_int4 type, which is 16 bytes.
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

#if NET
// This type does not come from the CoreGraphics framework; it's defined in /usr/include/simd/vector_types.h
namespace CoreGraphics
{
	[StructLayout (LayoutKind.Sequential)]
	public struct NVector4i : IEquatable<NVector4i>
	{
		public int X;
		public int Y;
		public int Z;
		public int W;

		public NVector4i (int x, int y, int z, int w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public static bool operator == (NVector4i left, NVector4i right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector4i left, NVector4i right)
		{
			return !left.Equals (right);
		}

		public override string ToString ()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine(X, Y, Z, W);
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is NVector4i vector))
				return false;

			return Equals (vector);
		}

		public bool Equals (NVector4i other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public static NVector4i Zero {
			get => default;
		}
	}
}
#endif // NET
