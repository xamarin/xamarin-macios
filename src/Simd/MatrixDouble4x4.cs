//
// MatrixDouble4x4.cs:
//     This represents the native matrix_double4x4 type, which has a column-major layout
//     (as opposed to OpenTK.Matrix4d, which has a row-major layout).
//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//     Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) 2017 Microsoft Inc
//

using System;
using System.Runtime.InteropServices;

namespace Simd {
	[StructLayout (LayoutKind.Sequential)]
	public struct MatrixDouble4x4 : IEquatable<MatrixDouble4x4> {
		public double M11;
		public double M21;
		public double M31;
		public double M41;

		public double M12;
		public double M22;
		public double M32;
		public double M42;

		public double M13;
		public double M23;
		public double M33;
		public double M43;

		public double M14;
		public double M24;
		public double M34;
		public double M44;

		public readonly static MatrixDouble4x4 Identity = new MatrixDouble4x4 {
			M11 = 1f,
			M22 = 1f,
			M33 = 1f,
			M44 = 1f,
		};

		public MatrixDouble4x4 (VectorDouble4 column0, VectorDouble4 column1, VectorDouble4 column2, VectorDouble4 column3)
		{
			M11 = column0.X;
			M21 = column0.Y;
			M31 = column0.Z;
			M41 = column0.W;
			M12 = column1.X;
			M22 = column1.Y;
			M32 = column1.Z;
			M42 = column1.W;
			M13 = column2.X;
			M23 = column2.Y;
			M33 = column2.Z;
			M43 = column2.W;
			M14 = column3.X;
			M24 = column3.Y;
			M34 = column3.Z;
			M44 = column3.W;
		}

		public MatrixDouble4x4 (global::OpenTK.Vector4d column0, global::OpenTK.Vector4d column1, global::OpenTK.Vector4d column2, global::OpenTK.Vector4d column3)
		{
			M11 = column0.X;
			M21 = column0.Y;
			M31 = column0.Z;
			M41 = column0.W;
			M12 = column1.X;
			M22 = column1.Y;
			M32 = column1.Z;
			M42 = column1.W;
			M13 = column2.X;
			M23 = column2.Y;
			M33 = column2.Z;
			M43 = column2.W;
			M14 = column3.X;
			M24 = column3.Y;
			M34 = column3.Z;
			M44 = column3.W;
		}

		public MatrixDouble4x4 (
			double m11, double m12, double m13, double m14,
			double m21, double m22, double m23, double m24,
			double m31, double m32, double m33, double m34,
			double m41, double m42, double m43, double m44)
		{
			M11 = m11;
			M21 = m21;
			M31 = m31;
			M41 = m41;
			M12 = m12;
			M22 = m22;
			M32 = m32;
			M42 = m42;
			M13 = m13;
			M23 = m23;
			M33 = m33;
			M43 = m43;
			M14 = m14;
			M24 = m24;
			M34 = m34;
			M44 = m44;
		}

		public double Determinant {
			get {
				double a = M33 * M44 - M34 * M43;
				double b = M32 * M44 - M34 * M42;
				double c = M32 * M43 - M33 * M42;
				double d = M31 * M44 - M34 * M41;
				double e = M31 * M43 - M33 * M41;
				double f = M31 * M42 - M32 * M41;

				return
					M11 * (M22 * a - M23 * b + M24 * c) -
					M12 * (M21 * a - M23 * d + M24 * e) +
					M13 * (M21 * b - M22 * d + M24 * f) -
					M14 * (M21 * c - M22 * e + M23 * f);
			}
		}

		public void Transpose ()
		{
			this = Transpose (this);
		}

		public static MatrixDouble4x4 Transpose (MatrixDouble4x4 mat)
		{
			MatrixDouble4x4 result;
			Transpose (ref mat, out result);
			return result;
		}

		public static void Transpose (ref MatrixDouble4x4 mat, out MatrixDouble4x4 result)
		{
			result.M11 = mat.M11;
			result.M21 = mat.M12;
			result.M31 = mat.M13;
			result.M41 = mat.M14;

			result.M12 = mat.M21;
			result.M22 = mat.M22;
			result.M32 = mat.M23;
			result.M42 = mat.M24;

			result.M13 = mat.M31;
			result.M23 = mat.M32;
			result.M33 = mat.M33;
			result.M43 = mat.M34;

			result.M14 = mat.M41;
			result.M24 = mat.M42;
			result.M34 = mat.M43;
			result.M44 = mat.M44;
		}

		public static MatrixDouble4x4 Multiply (MatrixDouble4x4 left, MatrixDouble4x4 right)
		{
			MatrixDouble4x4 result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref MatrixDouble4x4 left, ref MatrixDouble4x4 right, out MatrixDouble4x4 result)
		{
			result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
			result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
			result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
			result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

			result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
			result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
			result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
			result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

			result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
			result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
			result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
			result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

			result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
			result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
			result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
			result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;
		}

		public static MatrixDouble4x4 operator * (MatrixDouble4x4 left, MatrixDouble4x4 right)
		{
			return Multiply (left, right);
		}

		public static bool operator == (MatrixDouble4x4 left, MatrixDouble4x4 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (MatrixDouble4x4 left, MatrixDouble4x4 right)
		{
			return !left.Equals (right);
		}

		public static explicit operator global::OpenTK.Matrix4d (MatrixDouble4x4 value)
		{
			return new global::OpenTK.Matrix4d (
				value.M11, value.M12, value.M13, value.M14,
				value.M21, value.M22, value.M23, value.M24,
				value.M31, value.M32, value.M33, value.M34,
				value.M41, value.M42, value.M43, value.M44);
		}

		public static explicit operator MatrixDouble4x4 (global::OpenTK.Matrix4d value)
		{
			return new MatrixDouble4x4 (value.Column0, value.Column1, value.Column2, value.Column3);
		}

		public override string ToString ()
		{
			return
				$"({M11}, {M12}, {M13}, {M14})\n" +
				$"({M21}, {M22}, {M23}, {M24})\n" +
				$"({M31}, {M32}, {M33}, {M34})\n" +
				$"({M41}, {M42}, {M43}, {M44})";
		}

		public override int GetHashCode ()
		{
			return
				M11.GetHashCode () ^ M12.GetHashCode () ^ M13.GetHashCode () ^ M14.GetHashCode () ^
				M21.GetHashCode () ^ M22.GetHashCode () ^ M23.GetHashCode () ^ M24.GetHashCode () ^
				M31.GetHashCode () ^ M32.GetHashCode () ^ M33.GetHashCode () ^ M34.GetHashCode () ^
				M41.GetHashCode () ^ M42.GetHashCode () ^ M43.GetHashCode () ^ M44.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is MatrixDouble4x4))
				return false;

			return Equals ((MatrixDouble4x4) obj);
		}

		public bool Equals (MatrixDouble4x4 other)
		{
			return
				M11 == other.M11 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14 &&
				M21 == other.M21 && M22 == other.M22 && M23 == other.M23 && M24 == other.M24 &&
				M31 == other.M31 && M32 == other.M32 && M33 == other.M33 && M34 == other.M34 &&
				M41 == other.M41 && M42 == other.M42 && M43 == other.M43 && M44 == other.M44;
		}
	}
}
