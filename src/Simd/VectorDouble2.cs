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

using System;
using System.Runtime.InteropServices;

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
			return X.GetHashCode () ^ Y.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is NVector2d))
				return false;

			return Equals ((NVector2d) obj);
		}

		public bool Equals (NVector2d other)
		{
			return X == other.X && Y == other.Y;
		}
	}
}
#endif // NET
