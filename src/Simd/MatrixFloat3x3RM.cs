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
// One day we might be able to remove this type in favor of a System.Numerics.Matrix3x3 type (which doesn't exist yet),
// so the only API available here is the one that I _think_ System.Numerics.Matrix3x3 will have (based on the existing
// System.Numerics.Matrix4x4 type). In particular it must not have any API that *doesn't* exist in a potential
// System.Numerics.Matrix3x3 type (🔮).
//

#nullable enable

using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;

using ObjCRuntime;

// This type does not come from the CoreGraphics framework
#if NET
namespace CoreGraphics
{
	[NativeName ("GLKMatrix3")]
	[StructLayout (LayoutKind.Sequential)]
	public struct RMatrix3 : IEquatable<RMatrix3>
	{
		public float M11;
		public float M12;
		public float M13;
		public float M21;
		public float M22;
		public float M23;
		public float M31;
		public float M32;
		public float M33;

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

		public RMatrix3 (
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
		}

		static readonly RMatrix3 _identity = new RMatrix3
		(
			1, 0, 0,
			0, 1, 0,
			0, 0, 1
		);

		public static RMatrix3 Identity { get => _identity; }

		public float this [int row, int column] {
			get {
				switch (row) {
				case 0:
					switch (column) {
					case 0: return M11;
					case 1: return M12;
					case 2: return M13;
					}
					break;
				case 1:
					switch (column) {
					case 0: return M21;
					case 1: return M22;
					case 2: return M23;
					}
					break;
				case 2:
					switch (column) {
					case 0: return M31;
					case 1: return M32;
					case 2: return M33;
					}
					break;
				}

				throw new IndexOutOfRangeException();
			}
			set {
				switch (row) {
				case 0:
					switch (column) {
					case 0: M11 = value; return;
					case 1: M12 = value; return;
					case 2: M13 = value; return;
					}
					break;
				case 1:
					switch (column) {
					case 0: M21 = value; return;
					case 1: M22 = value; return;
					case 2: M23 = value; return;
					}
					break;
				case 2:
					switch (column) {
					case 0: M31 = value; return;
					case 1: M32 = value; return;
					case 2: M33 = value; return;
					}
					break;
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static RMatrix3 CreateFromQuaternion (Quaternion quaternion)
		{
			quaternion = Quaternion.Normalize (quaternion);

			float xx = (float) (quaternion.X * quaternion.X);
			float yy = (float) (quaternion.Y * quaternion.Y);
			float zz = (float) (quaternion.Z * quaternion.Z);
			float xy = (float) (quaternion.X * quaternion.Y);
			float xz = (float) (quaternion.X * quaternion.Z);
			float yz = (float) (quaternion.Y * quaternion.Z);
			float wx = (float) (quaternion.W * quaternion.X);
			float wy = (float) (quaternion.W * quaternion.Y);
			float wz = (float) (quaternion.W * quaternion.Z);

			var result = new RMatrix3 ();
			result.M11 = 1 - 2 * (yy + zz);
			result.M12 = 2 * (xy - wz);
			result.M13 = 2 * (xz + wy);

			result.M21 = 2 * (xy + wz);
			result.M22 = 1 - 2 * (xx + zz);
			result.M23 = 2 * (yz - wx);

			result.M31 = 2 * (xz - wy);
			result.M32 = 2 * (yz + wx);
			result.M33 = 1 - 2 * (xx + yy);

			return result;
		}

		public readonly float GetDeterminant ()
		{
			return
				M11 * (M22 * M33 - M23 * M32) -
				M12 * (M21 * M33 - M23 * M31) +
				M13 * (M21 * M32 - M22 * M31);
		}

		public static RMatrix3 Transpose (RMatrix3 matrix)
		{
			RMatrix3 result = new RMatrix3 ();
			result.M11 = matrix.M11;
			result.M21 = matrix.M12;
			result.M31 = matrix.M13;
			result.M12 = matrix.M21;
			result.M22 = matrix.M22;
			result.M32 = matrix.M23;
			result.M13 = matrix.M31;
			result.M23 = matrix.M32;
			result.M33 = matrix.M33;
			return result;
		}

		public static RMatrix3 Multiply (RMatrix3 value1, RMatrix3 value2)
		{
			RMatrix3 result;
			result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31;
			result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32;
			result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33;

			result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31;
			result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32;
			result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33;

			result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31;
			result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32;
			result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33;
			return result;
		}

		public static RMatrix3 Multiply (RMatrix3 value1, float value2)
		{
			var result = new RMatrix3 ();
			result.M11 = value2 * value1.M11;
			result.M12 = value2 * value1.M12;
			result.M13 = value2 * value1.M13;
			result.M21 = value2 * value1.M21;
			result.M22 = value2 * value1.M22;
			result.M23 = value2 * value1.M23;
			result.M31 = value2 * value1.M31;
			result.M32 = value2 * value1.M32;
			result.M33 = value2 * value1.M33;
			return result;
		}

		public static RMatrix3 Add (RMatrix3 value1, RMatrix3 value2)
		{
			var result = new RMatrix3 ();
			result.M11 = value1.M11 + value2.M11;
			result.M12 = value1.M12 + value2.M12;
			result.M13 = value1.M13 + value2.M13;
			result.M21 = value1.M21 + value2.M21;
			result.M22 = value1.M22 + value2.M22;
			result.M23 = value1.M23 + value2.M23;
			result.M31 = value1.M31 + value2.M31;
			result.M32 = value1.M32 + value2.M32;
			result.M33 = value1.M33 + value2.M33;
			return result;
		}

		public static RMatrix3 Subtract (RMatrix3 value1, RMatrix3 value2)
		{
			var result = new RMatrix3 ();
			result.M11 = value1.M11 - value2.M11;
			result.M12 = value1.M12 - value2.M12;
			result.M13 = value1.M13 - value2.M13;
			result.M21 = value1.M21 - value2.M21;
			result.M22 = value1.M22 - value2.M22;
			result.M23 = value1.M23 - value2.M23;
			result.M31 = value1.M31 - value2.M31;
			result.M32 = value1.M32 - value2.M32;
			result.M33 = value1.M33 - value2.M33;
			return result;
		}

		public static RMatrix3 operator * (RMatrix3 value1, RMatrix3 value2)
		{
			return Multiply (value1, value2);
		}

		public static bool operator == (RMatrix3 value1, RMatrix3 value2)
		{
			return value1.Equals (value2);
		}

		public static bool operator != (RMatrix3 value1, RMatrix3 value2)
		{
			return !value1.Equals (value2);
		}

		public override string ToString ()
		{
			return
				$"{{ {{M11:{M11} M12:{M12} M13:{M13}}} {{M21:{M21} M22:{M22} M23:{M23}}} {{M31:{M31} M32:{M32} M33:{M33}}} }}";
		}

		public override int GetHashCode ()
		{
			var hash = new HashCode();
			hash.Add(M11);
			hash.Add(M12);
			hash.Add(M13);
			hash.Add(M21);
			hash.Add(M22);
			hash.Add(M23);
			hash.Add(M31);
			hash.Add(M32);
			hash.Add(M33);
			return hash.ToHashCode();
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
				M11 == other.M11 && M12 == other.M12 && M13 == other.M13 &&
				M21 == other.M21 && M22 == other.M22 && M23 == other.M23 &&
				M31 == other.M31 && M32 == other.M32 && M33 == other.M33;
		}
	}
}
#endif // NET
