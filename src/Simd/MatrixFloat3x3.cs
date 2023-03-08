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

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
	public struct NMatrix3 : IEquatable<NMatrix3> {
		/* Due to memory alignment, vectors of length 3 are 
		 * represented as vectors of length 4, so we pad here
		 * with dummy fields.
		 * See top of /usr/include/simd/matrix_types.h for more information. */
#if NET
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

#if !XAMCORE_5_0
		// Add hidden obsolete members for names used in OpenTK's Matrix3 type to ease migration to .NET.
		[Obsolete ("Use 'M11' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R0C0 { get => M11; set => M11 = value; }
		[Obsolete ("Use 'M21' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R1C0 { get => M21; set => M21 = value; }
		[Obsolete ("Use 'M31' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R2C0 { get => M31; set => M31 = value; }
		[Obsolete ("Use 'M12' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R0C1 { get => M12; set => M12 = value; }
		[Obsolete ("Use 'M22' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R1C1 { get => M22; set => M22 = value; }
		[Obsolete ("Use 'M32' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R2C1 { get => M32; set => M32 = value; }
		[Obsolete ("Use 'M13' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R0C2 { get => M13; set => M13 = value; }
		[Obsolete ("Use 'M23' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R1C2 { get => M23; set => M23 = value; }
		[Obsolete ("Use 'M33' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R2C2 { get => M33; set => M33 = value; }
#endif // !XAMCORE_5_0
#else
		[Obsolete ("Use 'M11' instead.")]
		public float R0C0;
		[Obsolete ("Use 'M21' instead.")]
		public float R1C0;
		[Obsolete ("Use 'M31' instead.")]
		public float R2C0;
		float dummy0;

		[Obsolete ("Use 'M12' instead.")]
		public float R0C1;
		[Obsolete ("Use 'M22' instead.")]
		public float R1C1;
		[Obsolete ("Use 'M32' instead.")]
		public float R2C1;
		float dummy1;

		[Obsolete ("Use 'M13' instead.")]
		public float R0C2;
		[Obsolete ("Use 'M23' instead.")]
		public float R1C2;
		[Obsolete ("Use 'M33' instead.")]
		public float R2C2;
		float dummy2;

		public float M11 { get => R0C0; set => R0C0 = value; }
		public float M21 { get => R1C0; set => R1C0 = value; }
		public float M31 { get => R2C0; set => R2C0 = value; }
		public float M12 { get => R0C1; set => R0C1 = value; }
		public float M22 { get => R1C1; set => R1C1 = value; }
		public float M32 { get => R2C1; set => R2C1 = value; }
		public float M13 { get => R0C2; set => R0C2 = value; }
		public float M23 { get => R1C2; set => R1C2 = value; }
		public float M33 { get => R2C2; set => R2C2 = value; }
#endif

#if NET
		static readonly NMatrix3 _identity = new NMatrix3
		(
			1, 0, 0,
			0, 1, 0,
			0, 0, 1
		);

		public static NMatrix3 Identity { get => _identity; }
#else
		public readonly static NMatrix3 Identity = new NMatrix3 {
			M11 = 1f,
			M22 = 1f,
			M33 = 1f,
		};
#endif

		public NMatrix3 (
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33)
		{
#if NET
			M11 = m11;
			M21 = m21;
			M31 = m31;
			M12 = m12;
			M22 = m22;
			M32 = m32;
			M13 = m13;
			M23 = m23;
			M33 = m33;
#else
			R0C0 = m11;
			R1C0 = m21;
			R2C0 = m31;
			R0C1 = m12;
			R1C1 = m22;
			R2C1 = m32;
			R0C2 = m13;
			R1C2 = m23;
			R2C2 = m33;
#endif
			dummy0 = 0;
			dummy1 = 0;
			dummy2 = 0;
		}

		public readonly float GetDeterminant ()
		{
			return
#if NET
				M11 * (M22 * M33 - M23 * M32) -
				M12 * (M21 * M33 - M23 * M31) +
				M13 * (M21 * M32 - M22 * M31);
#else
				R0C0 * (R1C1 * R2C2 - R1C2 * R2C1) -
				R0C1 * (R1C0 * R2C2 - R1C2 * R2C0) +
				R0C2 * (R1C0 * R2C1 - R1C1 * R2C0);
#endif
		}

#if !XAMCORE_5_0
		public float Determinant {
			get {
				return
					M11 * (M22 * M33 - M23 * M32) -
					M12 * (M21 * M33 - M23 * M31) +
					M13 * (M21 * M32 - M22 * M31);
			}
		}
#endif

		public void Transpose ()
		{
			this = Transpose (this);
		}

		public static NMatrix3 Transpose (NMatrix3 matrix)
		{
			NMatrix3 result = new NMatrix3 ();
			Transpose (ref matrix, out result);
			return result;
		}

		public static void Transpose (ref NMatrix3 mat, out NMatrix3 result)
		{
#if !NET
			result = new NMatrix3 ();
#endif
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

		public static NMatrix3 Multiply (NMatrix3 left, NMatrix3 right)
		{
			NMatrix3 result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref NMatrix3 left, ref NMatrix3 right, out NMatrix3 result)
		{
#if !NET
			result = new NMatrix3 ();
#endif
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

		public static NMatrix3 operator * (NMatrix3 left, NMatrix3 right)
		{
			return Multiply (left, right);
		}

		public static bool operator == (NMatrix3 left, NMatrix3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (NMatrix3 left, NMatrix3 right)
		{
			return !left.Equals (right);
		}

#if !NET
		public static explicit operator global::OpenTK.Matrix3 (NMatrix3 value)
		{
			return new global::OpenTK.Matrix3 (
				value.M11, value.M12, value.M13,
				value.M21, value.M22, value.M23,
				value.M31, value.M32, value.M33);
		}

		public static explicit operator NMatrix3 (global::OpenTK.Matrix3 value)
		{
			return new NMatrix3 (
				value.R0C0, value.R0C1, value.R0C2,
				value.R1C0, value.R1C1, value.R1C2,
				value.R2C0, value.R2C1, value.R2C2);
		}
#endif // !NET

		public override string ToString ()
		{
			return
				$"({M11}, {M12}, {M13})\n" +
				$"({M21}, {M22}, {M23})\n" +
				$"({M31}, {M32}, {M33})";
		}

		public override int GetHashCode ()
		{
			var hash = new HashCode ();
			hash.Add (M11);
			hash.Add (M12);
			hash.Add (M13);
			hash.Add (M21);
			hash.Add (M22);
			hash.Add (M23);
			hash.Add (M31);
			hash.Add (M32);
			hash.Add (M33);
			return hash.ToHashCode ();
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is NMatrix3 matrix))
				return false;

			return Equals (matrix);
		}

		public bool Equals (NMatrix3 other)
		{
			return
				M11 == other.M11 && M12 == other.M12 && M13 == other.M13 &&
				M21 == other.M21 && M22 == other.M22 && M23 == other.M23 &&
				M31 == other.M31 && M32 == other.M32 && M33 == other.M33;
		}
	}
}
