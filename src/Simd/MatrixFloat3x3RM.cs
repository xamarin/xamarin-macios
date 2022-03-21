//
// Authors:
//     Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2022 Microsoft Inc
//

//
// This represents the native GLKMatrix3 type, which has a row-major layout
// (same as OpenTK.Matrix3).
//

#nullable enable

using System;
using System.Runtime.InteropServices;

// This type does not come from the CoreGraphics framework
#if NET
namespace CoreGraphics
{
	[StructLayout (LayoutKind.Sequential)]
	public struct RMatrix3 : IEquatable<RMatrix3>
	{
		public float R0C0;
		public float R0C1;
		public float R0C2;
		public float R1C0;
		public float R1C1;
		public float R1C2;
		public float R2C0;
		public float R2C1;
		public float R2C2;

		public RMatrix3 (
			float r0c0, float r0c1, float r0c2,
			float r1c0, float r1c1, float r1c2,
			float r2c0, float r2c1, float r2c2)
		{
			R0C0 = r0c0;
			R1C0 = r1c0;
			R2C0 = r2c0;
			R0C1 = r0c1;
			R1C1 = r1c1;
			R2C1 = r2c1;
			R0C2 = r0c2;
			R1C2 = r1c2;
			R2C2 = r2c2;
		}

		public float Determinant {
			get {
				return
					R0C0 * (R1C1 * R2C2 - R1C2 * R2C1) -
					R0C1 * (R1C0 * R2C2 - R1C2 * R2C0) +
					R0C2 * (R1C0 * R2C1 - R1C1 * R2C0);
			}
		}

		public void Transpose ()
		{
			this = Transpose (this);
		}

		public static RMatrix3 Transpose (RMatrix3 mat)
		{
			RMatrix3 result = new RMatrix3 ();
			Transpose (ref mat, out result);
			return result;
		}

		public static void Transpose (ref RMatrix3 mat, out RMatrix3 result)
		{
			result.R0C0 = mat.R0C0;
			result.R1C0 = mat.R0C1;
			result.R2C0 = mat.R0C2;
			result.R0C1 = mat.R1C0;
			result.R1C1 = mat.R1C1;
			result.R2C1 = mat.R1C2;
			result.R0C2 = mat.R2C0;
			result.R1C2 = mat.R2C1;
			result.R2C2 = mat.R2C2;
		}

		public static RMatrix3 Multiply (RMatrix3 left, RMatrix3 right)
		{
			RMatrix3 result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref RMatrix3 left, ref RMatrix3 right, out RMatrix3 result)
		{
			result.R0C0 = left.R0C0 * right.R0C0 + left.R0C1 * right.R1C0 + left.R0C2 * right.R2C0;
			result.R0C1 = left.R0C0 * right.R0C1 + left.R0C1 * right.R1C1 + left.R0C2 * right.R2C1;
			result.R0C2 = left.R0C0 * right.R0C2 + left.R0C1 * right.R1C2 + left.R0C2 * right.R2C2;

			result.R1C0 = left.R1C0 * right.R0C0 + left.R1C1 * right.R1C0 + left.R1C2 * right.R2C0;
			result.R1C1 = left.R1C0 * right.R0C1 + left.R1C1 * right.R1C1 + left.R1C2 * right.R2C1;
			result.R1C2 = left.R1C0 * right.R0C2 + left.R1C1 * right.R1C2 + left.R1C2 * right.R2C2;

			result.R2C0 = left.R2C0 * right.R0C0 + left.R2C1 * right.R1C0 + left.R2C2 * right.R2C0;
			result.R2C1 = left.R2C0 * right.R0C1 + left.R2C1 * right.R1C1 + left.R2C2 * right.R2C1;
			result.R2C2 = left.R2C0 * right.R0C2 + left.R2C1 * right.R1C2 + left.R2C2 * right.R2C2;
		}

		public static RMatrix3 operator * (RMatrix3 left, RMatrix3 right)
		{
			return Multiply (left, right);
		}

		public static bool operator == (RMatrix3 left, RMatrix3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (RMatrix3 left, RMatrix3 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator NMatrix3 (RMatrix3 value)
		{
			return new NMatrix3 (
				value.R0C0, value.R0C1, value.R0C2,
				value.R1C0, value.R1C1, value.R1C2,
				value.R2C0, value.R2C1, value.R2C2);
		}

		public static explicit operator RMatrix3 (NMatrix3 value)
		{
			return new RMatrix3 (
				value.R0C0, value.R0C1, value.R0C2,
				value.R1C0, value.R1C1, value.R1C2,
				value.R2C0, value.R2C1, value.R2C2);
		}

		public override string ToString ()
		{
			return
				$"({R0C0}, {R0C1}, {R0C2})\n" +
				$"({R1C0}, {R1C1}, {R1C2})\n" +
				$"({R2C0}, {R2C1}, {R2C2})";
		}

		public override int GetHashCode ()
		{
			return
				R0C0.GetHashCode () ^ R0C1.GetHashCode () ^ R0C2.GetHashCode () ^
				R1C0.GetHashCode () ^ R1C1.GetHashCode () ^ R1C2.GetHashCode () ^
				R2C0.GetHashCode () ^ R2C1.GetHashCode () ^ R2C2.GetHashCode ();
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is RMatrix3 matrix))
				return false;

			return Equals (matrix);
		}

		public bool Equals (RMatrix3 other)
		{
			return
				R0C0 == other.R0C0 && R0C1 == other.R0C1 && R0C2 == other.R0C2 &&
				R1C0 == other.R1C0 && R1C1 == other.R1C1 && R1C2 == other.R1C2 &&
				R2C0 == other.R2C0 && R2C1 == other.R2C1 && R2C2 == other.R2C2;
		}
	}
}
#endif
