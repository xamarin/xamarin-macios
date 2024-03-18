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
	public struct NMatrix2 : IEquatable<NMatrix2> {
#if NET
		public float M11;
		public float M21;
		public float M12;
		public float M22;

#if !XAMCORE_5_0
		[Obsolete ("Use 'M11' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R0C0 { get => M11; set => M11 = value; }
		[Obsolete ("Use 'M21' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R1C0 { get => M21; set => M21 = value; }
		[Obsolete ("Use 'M12' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R0C1 { get => M12; set => M12 = value; }
		[Obsolete ("Use 'M22' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public float R1C1 { get => M22; set => M22 = value; }
#endif // !XAMCORE_5_0
#else
		[Obsolete ("Use 'M11' instead.")]
		public float R0C0;
		[Obsolete ("Use 'M21' instead.")]
		public float R1C0;
		[Obsolete ("Use 'M12' instead.")]
		public float R0C1;
		[Obsolete ("Use 'M22' instead.")]
		public float R1C1;

		public float M11 { get => R0C0; set => R0C0 = value; }
		public float M21 { get => R1C0; set => R1C0 = value; }
		public float M12 { get => R0C1; set => R0C1 = value; }
		public float M22 { get => R1C1; set => R1C1 = value; }
#endif

#if NET
		static readonly NMatrix2 _identity = new NMatrix2
		(
			1, 0,
			0, 1);

		public static NMatrix2 Identity { get => _identity; }
#else
		public readonly static NMatrix2 Identity = new NMatrix2 (
			1, 0,
			0, 1);
#endif

		public NMatrix2 (
			float m11, float m12,
			float m21, float m22)
		{
#if NET
			M11 = m11;
			M21 = m21;
			M12 = m12;
			M22 = m22;
#else
			R0C0 = m11;
			R1C0 = m21;
			R0C1 = m12;
			R1C1 = m22;
#endif
		}

		public readonly float GetDeterminant ()
		{
#if NET
			return M11 * M22 - M21 * M12;
#else
			return R0C0 * R1C1 - R1C0 * R0C1;
#endif
		}

		public float Determinant {
			get {
				return M11 * M22 - M21 * M12;
			}
		}

		public void Transpose ()
		{
			this = Transpose (this);
		}

		public static NMatrix2 Transpose (NMatrix2 mat)
		{
			return new NMatrix2 (mat.M11, mat.M21, mat.M12, mat.M22);
		}

		public static void Transpose (ref NMatrix2 mat, out NMatrix2 result)
		{
#if !NET
			result = new NMatrix2 ();
#endif
			result.M11 = mat.M11;
			result.M12 = mat.M21;
			result.M21 = mat.M12;
			result.M22 = mat.M22;
		}

		public static NMatrix2 Multiply (NMatrix2 left, NMatrix2 right)
		{
			NMatrix2 result;
			Multiply (ref left, ref right, out result);
			return result;
		}

		public static void Multiply (ref NMatrix2 left, ref NMatrix2 right, out NMatrix2 result)
		{
#if !NET
			result = new NMatrix2 ();
#endif
			result.M11 = left.M11 * right.M11 + left.M12 * right.M21;
			result.M12 = left.M11 * right.M12 + left.M12 * right.M22;

			result.M21 = left.M21 * right.M11 + left.M22 * right.M21;
			result.M22 = left.M21 * right.M12 + left.M22 * right.M22;
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
				value.M11, value.M12,
				value.M21, value.M22);
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
			return $"({M11}, {M12})\n({M21}, {M22})";
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (M11, M12, M21, M22);
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
				M11 == other.M11 &&
				M12 == other.M12 &&
				M21 == other.M21 &&
				M22 == other.M22;
		}
	}
}
