//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native matrix_float3x3 type, which has a column-major layout
// (as opposed to OpenTK.Matrix3, which has a row-major layout).
//

using System;
using System.Runtime.InteropServices;

namespace Simd
{
	[StructLayout (LayoutKind.Sequential)]
	public struct MatrixFloat3x3 : IEquatable<MatrixFloat3x3>
	{
		/* Due to memory alignment, vectors of length 3 are 
		 * represented as vectors of length 4, so we pad here
		 * with dummy fields.
		 * See top of /usr/include/simd/matrix_types.h for more information. */
		public float M11;
		public float M21;
		public float M31;
		float dummy0;
		public float M12;
		public float M22;
		public float M32;
		float dummy1;
		public float M13;
		public float M23;
		public float M33;
		float dummy2;

		public readonly static MatrixFloat3x3 Identity = new MatrixFloat3x3 
		{
			M11 = 1f,
			M22 = 1f,
			M33 = 1f,
		};

		public MatrixFloat3x3 (VectorFloat3 column0, VectorFloat3 column1, VectorFloat3 column2)
		{
			M11 = column0.X;
			M21 = column0.Y;
			M31 = column0.Z;
			M12 = column1.X;
			M22 = column1.Y;
			M32 = column1.Z;
			M13 = column2.X;
			M23 = column2.Y;
			M33 = column2.Z;
			dummy0 = 0;
			dummy1 = 0;
			dummy2 = 0;
		}

		public MatrixFloat3x3 (global::OpenTK.Vector3 column0, global::OpenTK.Vector3 column1, global::OpenTK.Vector3 column2)
		{
			M11 = column0.X;
			M21 = column0.Y;
			M31 = column0.Z;
			M12 = column1.X;
			M22 = column1.Y;
			M32 = column1.Z;
			M13 = column2.X;
			M23 = column2.Y;
			M33 = column2.Z;
			dummy0 = 0;
			dummy1 = 0;
			dummy2 = 0;
		}

		public MatrixFloat3x3 (
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33)
		{
			M11 = m11;
			M21 = m21;
			M31 = m31;
			M12 = m12;
			M22 = m22;
			M32 = m32;
			M13 = m13;
			M23 = m23;
			M33 = m33;
			dummy0 = 0;
			dummy1 = 0;
			dummy2 = 0;
		}

		public float Determinant {
			get {
				return
					M11 * (M22 * M33 - M23 * M32) -
					M12 * (M21 * M33 - M23 * M31) +
					M13 * (M21 * M32 - M22 * M31);
			}
		}

		public void Transpose ()
		{
			this = Transpose (this);
		}

		public static MatrixFloat3x3 Transpose (MatrixFloat3x3 mat)
		{
			MatrixFloat3x3 result = new MatrixFloat3x3 ();
			Transpose (ref mat, out result);
			return result;
		}

		public static void Transpose (ref MatrixFloat3x3 mat, out MatrixFloat3x3 result)
		{
			result.M11 = mat.M11;
			result.M21 = mat.M12;
			result.M31 = mat.M13;
			result.M12 = mat.M21;
			result.M22 = mat.M22;
			result.M32 = mat.M23;
			result.M13 = mat.M31;
			result.M23 = mat.M32;
			result.M33 = mat.M33;
			result.dummy0 = 0;
			result.dummy1 = 0;
			result.dummy2 = 0;
		}

		public static MatrixFloat3x3 Multiply (MatrixFloat3x3 left, MatrixFloat3x3 right)
		{
			MatrixFloat3x3 result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref MatrixFloat3x3 left, ref MatrixFloat3x3 right, out MatrixFloat3x3 result)
		{
			result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31;
			result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32;
			result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33;
			result.dummy0 = 0;

			result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31;
			result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32;
			result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33;
			result.dummy1 = 0;

			result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31;
			result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32;
			result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33;
			result.dummy2 = 0;
		}

		public static MatrixFloat3x3 operator * (MatrixFloat3x3 left, MatrixFloat3x3 right)
		{
			return Multiply (left, right);
		}

		public static bool operator == (MatrixFloat3x3 left, MatrixFloat3x3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (MatrixFloat3x3 left, MatrixFloat3x3 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Matrix3 (MatrixFloat3x3 value)
		{
			return new global::OpenTK.Matrix3 (
				value.M11, value.M12, value.M13,
				value.M21, value.M22, value.M23,
				value.M31, value.M32, value.M33);
		}

		public static explicit operator MatrixFloat3x3 (global::OpenTK.Matrix3 value)
		{
			return new MatrixFloat3x3 (
				value.R0C0, value.R0C1, value.R0C2,
				value.R1C0, value.R1C1, value.R1C2,
				value.R2C0, value.R2C1, value.R2C2);
		}

		public override string ToString ()
		{
			return
				$"({M11}, {M12}, {M13})\n" +
				$"({M21}, {M22}, {M23})\n" +
				$"({M31}, {M32}, {M33})";
		}

		public override int GetHashCode ()
		{
			return
				M11.GetHashCode () ^ M12.GetHashCode () ^ M13.GetHashCode () ^
				M21.GetHashCode () ^ M22.GetHashCode () ^ M23.GetHashCode () ^
				M31.GetHashCode () ^ M32.GetHashCode () ^ M33.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is MatrixFloat3x3))
				return false;

			return Equals ((MatrixFloat3x3) obj);
		}

		public bool Equals (MatrixFloat3x3 other)
		{
			return
				M11 == other.M11 && M12 == other.M12 && M13 == other.M13 &&
				M21 == other.M21 && M22 == other.M22 && M23 == other.M23 &&
				M31 == other.M31 && M32 == other.M32 && M33 == other.M33;
		}
	}
}
