//
// QuaternionDouble.cs:
//     This represents the native simd_quatd double type, which is 32 bytes.
//
// One day we might be able to remove this type in favor of a System.Numerics.Quaterniond type (which doesn't exist yet),
// so the only API available here is the one that I _think_ System.Numerics.Quanterniond will have (based on the existing
// System.Numerics.Quaternion type). In particular it must not have any API that *doesn't* exist in a potential
// System.Numerics.Quaterniond type (🔮).
//
// Authors:
//     Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2021 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;

// This type does not come from the CoreGraphics framework; it's defined in /usr/include/simd/quaternion.h
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

		public NQuaterniond (NVector3d vectorPart, double scalarPart)
		{
			X = vectorPart.X;
			Y = vectorPart.Y;
			Z = vectorPart.Z;
			W = scalarPart;
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

		public static bool operator == (NQuaterniond left, NQuaterniond right) =>
			left.Equals (right);

		public static bool operator != (NQuaterniond left, NQuaterniond right) =>
			!left.Equals (right);

		public override string ToString () =>
			$"({X}, {Y}, {Z}, {W})";

		public override int GetHashCode () =>
			HashCode.Combine(X, Y, Z, W);

		public override bool Equals (object? obj)
		{
			if (!(obj is NQuaterniond quat))
				return false;

			return Equals (quat);
		}

		public bool Equals (NQuaterniond other) =>
			X == other.X && Y == other.Y && Z == other.Z && W == other.W;

		internal double Length =>
			(double) System.Math.Sqrt (W * W + Xyz.LengthSquared);

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

			result.W = 2.0f * (float) System.Math.Acos (q.W); // angle
			float den = (float) System.Math.Sqrt (1.0 - q.W * q.W);
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
