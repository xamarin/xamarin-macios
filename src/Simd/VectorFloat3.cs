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

namespace OpenTK
{
	[StructLayout (LayoutKind.Sequential)]
	public struct NVector3 : IEquatable<NVector3>
	{
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

		public static explicit operator global::OpenTK.Vector3 (NVector3 value)
		{
			return new global::OpenTK.Vector3 (value.X, value.Y, value.Z);
		}

		public static explicit operator NVector3 (global::OpenTK.Vector3 value)
		{
			return new NVector3 (value.X, value.Y, value.Z);
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
			if (!(obj is NVector3))
				return false;

			return Equals ((NVector3) obj);
		}

		public bool Equals (NVector3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
	}
}
