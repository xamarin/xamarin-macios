//
// VectorDouble3.cs:
//     This represents the native vector_double3 type, which is 32 bytes.
//
//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//     Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) 2017 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// This type does not come from the CoreGraphics framework; it's defined in /usr/include/simd/vector_types.h
#if NET
namespace CoreGraphics
#else
namespace OpenTK
#endif
{
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct NVector3d : IEquatable<NVector3d> {
		public double X;
		public double Y;
		public double Z;
		double dummy;

		public NVector3d (double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
			dummy = 0;
		}

		public static bool operator == (NVector3d left, NVector3d right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector3d left, NVector3d right)
		{
			return !left.Equals (right);
		}

#if NET
		public static NVector3d operator * (NVector3d vec, double scale)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		public static NVector3d operator / (NVector3d vec, double scale)
		{
			double mult = 1 / scale;
			vec.X *= mult;
			vec.Y *= mult;
			vec.Z *= mult;
			return vec;
		}
#endif // NET

#if !NET
		public static explicit operator global::OpenTK.Vector3d (NVector3d value)
		{
			return new global::OpenTK.Vector3d (value.X, value.Y, value.Z);
		}

		public static explicit operator NVector3d (global::OpenTK.Vector3d value)
		{
			return new NVector3d (value.X, value.Y, value.Z);
		}
#endif // !NET

		public override string ToString ()
		{
			return $"({X}, {Y}, {Z})";
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (X, Y, Z);
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is NVector3d vector))
				return false;

			return Equals (vector);
		}

		public bool Equals (NVector3d other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public static NVector3d Zero {
			get => default;
		}

#if NET
		internal double Length =>
			System.Math.Sqrt (X * X + Y * Y + Z * Z);

		internal double LengthSquared =>
			X * X + Y * Y + Z * Z;

		internal void Normalize ()
		{
			double scale = 1.0 / Length;
			X *= scale;
			Y *= scale;
			Z *= scale;
		}

		internal static readonly NVector3d UnitX = new NVector3d (1, 0, 0);

		internal static readonly NVector3d UnitY = new NVector3d (0, 1, 0);

		internal static readonly NVector3d UnitZ = new NVector3d (0, 0, 1);

		internal static readonly NVector3d One = new NVector3d (1, 1, 1);
#endif // NET
	}
}
