//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

// 
// This represents the native matrix_float2x2 type, which has a column-major layout
// (as opposed to OpenTK.Matrix2, which has a row-major layout).
// 

#nullable enable

using System;
using System.Runtime.InteropServices;

// This type does not come from the CoreGraphics framework; it's defined in /usr/include/simd/matrix_types.h
#if NET
namespace CoreGraphics
#else
namespace OpenTK
#endif
{
	[StructLayout (LayoutKind.Sequential)]
	public struct NMatrix2 : IEquatable<NMatrix2>
	{
		public float R0C0;
		public float R1C0;
		public float R0C1;
		public float R1C1;

		public readonly static NMatrix2 Identity = new NMatrix2 (
			1, 0,
			0, 1);

		public NMatrix2 (
			float r0c0, float r0c1,
			float r1c0, float r1c1)
		{
			R0C0 = r0c0;
			R1C0 = r1c0;
			R0C1 = r0c1;
			R1C1 = r1c1;
		}

		public float Determinant {
			get {
				return R0C0 * R1C1 - R1C0 * R0C1;
			}
		}

		public void Transpose ()
		{
			this = Transpose (this);
		}

		public static NMatrix2 Transpose (NMatrix2 mat)
		{
			return new NMatrix2 (mat.R0C0, mat.R1C0, mat.R0C1, mat.R1C1);
		}

		public static void Transpose (ref NMatrix2 mat, out NMatrix2 result)
		{
			result.R0C0 = mat.R0C0;
			result.R0C1 = mat.R1C0;
			result.R1C0 = mat.R0C1;
			result.R1C1 = mat.R1C1;
		}

		public static NMatrix2 Multiply (NMatrix2 left, NMatrix2 right)
		{
			NMatrix2 result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref NMatrix2 left, ref NMatrix2 right, out NMatrix2 result)
		{
			result.R0C0 = left.R0C0 * right.R0C0 + left.R0C1 * right.R1C0;
			result.R0C1 = left.R0C0 * right.R0C1 + left.R0C1 * right.R1C1;

			result.R1C0 = left.R1C0 * right.R0C0 + left.R1C1 * right.R1C0;
			result.R1C1 = left.R1C0 * right.R0C1 + left.R1C1 * right.R1C1;
		}

		public static NMatrix2 operator * (NMatrix2 left, NMatrix2 right)
		{
			return Multiply (left, right);
		}

		public static bool operator == (NMatrix2 left, NMatrix2 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NMatrix2 left, NMatrix2 right)
		{
			return !left.Equals (right);
		}

#if !NET
		public static explicit operator global::OpenTK.Matrix2 (NMatrix2 value)
		{
			return new global::OpenTK.Matrix2 (
				value.R0C0, value.R0C1,
				value.R1C0, value.R1C1);
		}

		public static explicit operator NMatrix2 (global::OpenTK.Matrix2 value)
		{
			return new NMatrix2 (
				value.R0C0, value.R0C1,
				value.R1C0, value.R1C1);
		}
#endif // !NET

		public override string ToString ()
		{
			return $"({R0C0}, {R0C1})\n({R1C0}, {R1C1})";
		}

		public override int GetHashCode ()
		{
			return R0C0.GetHashCode () ^ R0C1.GetHashCode () ^ R1C0.GetHashCode () ^ R1C1.GetHashCode ();
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is NMatrix2 matrix))
				return false;

			return Equals (matrix);
		}

		public bool Equals (NMatrix2 other)
		{
			return
				R0C0 == other.R0C0 &&
				R0C1 == other.R0C1 &&
				R1C0 == other.R1C0 &&
				R1C1 == other.R1C1;
		}
	}
}
