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

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if NET
using Vector4d = global::CoreGraphics.NVector4d;
#else
using Vector4d = global::OpenTK.Vector4d;
#endif

// This type does not come from the CoreGraphics framework; it's defined in /usr/include/simd/matrix_types.h
#if NET
namespace CoreGraphics
#else
namespace OpenTK
#endif
{
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct NMatrix4d : IEquatable<NMatrix4d> {
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

		public readonly static NMatrix4d Identity = new NMatrix4d {
			M11 = 1,
			M22 = 1,
			M33 = 1,
			M44 = 1,
		};

		public NMatrix4d (Vector4d row0, Vector4d row1, Vector4d row2, Vector4d row3)
		{
			M11 = row0.X;
			M21 = row1.X;
			M31 = row2.X;
			M41 = row3.X;
			M12 = row0.Y;
			M22 = row1.Y;
			M32 = row2.Y;
			M42 = row3.Y;
			M13 = row0.Z;
			M23 = row1.Z;
			M33 = row2.Z;
			M43 = row3.Z;
			M14 = row0.W;
			M24 = row1.W;
			M34 = row2.W;
			M44 = row3.W;
		}

		public NMatrix4d (
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

		public Vector4d Column0 {
			get {
				return new Vector4d (M11, M21, M31, M41);
			}
			set {
				M11 = value.X;
				M21 = value.Y;
				M31 = value.Z;
				M41 = value.W;
			}
		}

		public Vector4d Column1 {
			get {
				return new Vector4d (M12, M22, M32, M42);
			}
			set {
				M12 = value.X;
				M22 = value.Y;
				M32 = value.Z;
				M42 = value.W;
			}
		}

		public Vector4d Column2 {
			get {
				return new Vector4d (M13, M23, M33, M43);
			}
			set {
				M13 = value.X;
				M23 = value.Y;
				M33 = value.Z;
				M43 = value.W;
			}
		}

		public Vector4d Column3 {
			get {
				return new Vector4d (M14, M24, M34, M44);
			}
			set {
				M14 = value.X;
				M24 = value.Y;
				M34 = value.Z;
				M44 = value.W;
			}
		}

		public Vector4d Row0 {
			get {
				return new Vector4d (M11, M12, M13, M14);
			}
			set {
				M11 = value.X;
				M12 = value.Y;
				M13 = value.Z;
				M14 = value.W;
			}
		}

		public Vector4d Row1 {
			get {
				return new Vector4d (M21, M22, M23, M24);
			}
			set {
				M21 = value.X;
				M22 = value.Y;
				M23 = value.Z;
				M24 = value.W;
			}
		}

		public Vector4d Row2 {
			get {
				return new Vector4d (M31, M32, M33, M34);
			}
			set {
				M31 = value.X;
				M32 = value.Y;
				M33 = value.Z;
				M34 = value.W;
			}
		}

		public Vector4d Row3 {
			get {
				return new Vector4d (M41, M42, M43, M44);
			}
			set {
				M41 = value.X;
				M42 = value.Y;
				M43 = value.Z;
				M44 = value.W;
			}
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

		public static NMatrix4d Transpose (NMatrix4d mat)
		{
			NMatrix4d result;
			Transpose (ref mat, out result);
			return result;
		}

		public static void Transpose (ref NMatrix4d mat, out NMatrix4d result)
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

		public static NMatrix4d Multiply (NMatrix4d left, NMatrix4d right)
		{
			NMatrix4d result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref NMatrix4d left, ref NMatrix4d right, out NMatrix4d result)
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

		public static NMatrix4d operator * (NMatrix4d left, NMatrix4d right)
		{
			return Multiply (left, right);
		}

		public static bool operator == (NMatrix4d left, NMatrix4d right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NMatrix4d left, NMatrix4d right)
		{
			return !left.Equals (right);
		}

#if !NET
		public static explicit operator global::OpenTK.Matrix4d (NMatrix4d value)
		{
			return new global::OpenTK.Matrix4d (
				value.M11, value.M12, value.M13, value.M14,
				value.M21, value.M22, value.M23, value.M24,
				value.M31, value.M32, value.M33, value.M34,
				value.M41, value.M42, value.M43, value.M44);
		}

		public static explicit operator NMatrix4d (global::OpenTK.Matrix4d value)
		{
			return new NMatrix4d (value.Row0, value.Row1, value.Row2, value.Row3);
		}
#endif // !NET
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
			var hash = new HashCode ();
			hash.Add (M11);
			hash.Add (M12);
			hash.Add (M13);
			hash.Add (M14);
			hash.Add (M21);
			hash.Add (M22);
			hash.Add (M23);
			hash.Add (M24);
			hash.Add (M31);
			hash.Add (M32);
			hash.Add (M33);
			hash.Add (M34);
			hash.Add (M41);
			hash.Add (M42);
			hash.Add (M43);
			hash.Add (M44);
			return hash.ToHashCode ();
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is NMatrix4d matrix))
				return false;

			return Equals (matrix);
		}

		public bool Equals (NMatrix4d other)
		{
			return
				M11 == other.M11 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14 &&
				M21 == other.M21 && M22 == other.M22 && M23 == other.M23 && M24 == other.M24 &&
				M31 == other.M31 && M32 == other.M32 && M33 == other.M33 && M34 == other.M34 &&
				M41 == other.M41 && M42 == other.M42 && M43 == other.M43 && M44 == other.M44;
		}
	}
}
