//
// VectorInt4.cs
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
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode () ^ W.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is NVector4i))
				return false;

			return Equals ((NVector4i) obj);
		}

		public bool Equals (NVector4i other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}
	}
}
#endif // NET
