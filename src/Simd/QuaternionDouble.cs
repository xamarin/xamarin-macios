//
// QuaternionDouble.cs:
//     This represents the native quaternion double type, which is 32 bytes.
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
	public struct NQuaterniond : IEquatable<NQuaterniond>
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

		public NQuaterniond (double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public NQuaterniond (NVector3d xyz, double w)
		{
			X = xyz.X;
			Y = xyz.Y;
			Z = xyz.Z;
			W = w;
		}

		public NVector3d Xyz
		{
		    get => new NVector3d (X, Y, Z);
		    set
		    {
			X = value.X;
			Y = value.Y;
			Z = value.Z;       
		    }
		}


		public static bool operator == (NQuaterniond left, NQuaterniond right) =>
			left.Equals (right);

		public static bool operator != (NQuaterniond left, NQuaterniond right) =>
			!left.Equals (right);

		public override string ToString () =>
			$"({X}, {Y}, {Z}, {W})";

		public override int GetHashCode () =>
			X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode () ^ W.GetHashCode ();

		public override bool Equals (object obj)
		{
			if (!(obj is NQuaterniond))
				return false;

			return Equals ((NQuaterniond) obj);
		}

		public bool Equals (NQuaterniond other) =>
			X == other.X && Y == other.Y && Z == other.Z && W == other.W;

		internal double Length =>
			(double)System.Math.Sqrt (W * W + Xyz.LengthSquared);
        
		internal void Normalize ()
		{
			double scale = 1.0f / Length;
			Xyz *= scale;
			W *= scale;
		}

		internal void ToAxisAngle (out NVector3d axis, out double angle)
		{
		    NVector4d result = ToAxisAngle ();
		    axis = result.Xyz;
		    angle = result.W;
		}

		internal NVector4d ToAxisAngle ()
		{
		    NQuaterniond q = this;
		    if (q.W > 1.0f)
			    q.Normalize();

		    NVector4d result = new NVector4d ();

		    result.W = 2.0f * (float)System.Math.Acos (q.W); // angle
		    float den = (float)System.Math.Sqrt (1.0 - q.W * q.W);
		    if (den > 0.0001f)
			    result.Xyz = q.Xyz / den;
		    else {
			    // This occurs when the angle is zero. 
			    // Not a problem: just set an arbitrary normalized axis.
			    result.Xyz = NVector3d.UnitX;
		    }

		    return result;
		}
	}
}
#endif // NET
