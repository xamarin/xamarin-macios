//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native vector_float3 type, which due to padding is 16 bytes (not 12).
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
	public struct NVector3 : IEquatable<NVector3> {
		public float X;
		public float Y;
		public float Z;
		float dummy;

		public NVector3 (float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
			dummy = 0;
		}

		public static bool operator == (NVector3 left, NVector3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector3 left, NVector3 right)
		{
			return !left.Equals (right);
		}

#if NET
		public static explicit operator global::System.Numerics.Vector3 (NVector3 value)
		{
			return new global::System.Numerics.Vector3 (value.X, value.Y, value.Z);
		}

		public static explicit operator NVector3 (global::System.Numerics.Vector3 value)
		{
			return new NVector3 (value.X, value.Y, value.Z);
		}
#else
		public static explicit operator global::OpenTK.Vector3 (NVector3 value)
		{
			return new global::OpenTK.Vector3 (value.X, value.Y, value.Z);
		}

		public static explicit operator NVector3 (global::OpenTK.Vector3 value)
		{
			return new NVector3 (value.X, value.Y, value.Z);
		}
#endif

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
			if (!(obj is NVector3 vector))
				return false;

			return Equals (vector);
		}

		public bool Equals (NVector3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public static NVector3 Zero {
			get => default;
		}
	}
}
