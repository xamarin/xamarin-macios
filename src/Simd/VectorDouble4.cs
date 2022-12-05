//
// VectorDouble4.cs:
//     This represents the native vector_double4 type, which is 32 bytes.
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
	public struct NVector4d : IEquatable<NVector4d>
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

		public NVector4d (double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		internal NVector3d Xyz
		{
			get => new NVector3d (X, Y, Z);
			set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public static bool operator == (NVector4d left, NVector4d right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector4d left, NVector4d right)
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
			if (!(obj is NVector4d vector))
				return false;

			return Equals (vector);
		}

		public bool Equals (NVector4d other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public static NVector4d Zero {
			get => default;
		}
	}
}
#endif // NET
