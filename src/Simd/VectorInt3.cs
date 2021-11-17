//
// VectorInt3.cs:
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
	public struct NVector3i : IEquatable<NVector3i>
	{
		public int X;
		public int Y;
		public int Z;

		public NVector3i (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static bool operator == (NVector3i left, NVector3i right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NVector3i left, NVector3i right)
		{
			return !left.Equals (right);
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
			if (!(obj is NVector3i))
				return false;

			return Equals ((NVector3i) obj);
		}

		public bool Equals (NVector3i other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public static readonly NVector3i Zero = new NVector3i(0, 0, 0);
	}
}
#endif //!NET
