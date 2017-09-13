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

using System;
using System.Runtime.InteropServices;

namespace OpenTK
{
	[StructLayout (LayoutKind.Sequential)]
	public struct NVector3d : IEquatable<NVector3d>
	{
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

		public static explicit operator global::OpenTK.Vector3d (NVector3d value)
		{
			return new global::OpenTK.Vector3d (value.X, value.Y, value.Z);
		}

		public static explicit operator NVector3d (global::OpenTK.Vector3d value)
		{
			return new NVector3d (value.X, value.Y, value.Z);
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
			if (!(obj is NVector3d))
				return false;

			return Equals ((NVector3d) obj);
		}

		public bool Equals (NVector3d other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
	}
}
