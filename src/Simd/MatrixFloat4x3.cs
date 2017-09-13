//
// Authors:
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright (c) 2017 Microsoft Inc
//

//
// This represents the native matrix_float4x3 type (3 rows and 4 columns)
//

using System;
using System.Runtime.InteropServices;

using VectorFloat4=global::OpenTK.Vector4;

namespace Simd
{
	[StructLayout (LayoutKind.Sequential)]
	public struct MatrixFloat4x3 : IEquatable<MatrixFloat4x3>
	{
		public float M11;
		public float M21;
		public float M31;
		float dummy1;

		public float M12;
		public float M22;
		public float M32;
		float dummy2;

		public float M13;
		public float M23;
		public float M33;
		float dummy3;

		public float M14;
		public float M24;
		public float M34;
		float dummy4;

		public MatrixFloat4x3 (
			float m11, float m12, float m13, float m14,
			float m21, float m22, float m23, float m24,
			float m31, float m32, float m33, float m34)
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
			M14 = m14;
			M24 = m24;
			M34 = m34;
			dummy1 = 0;
			dummy2 = 0;
			dummy3 = 0;
			dummy4 = 0;
		}

		public static bool operator == (MatrixFloat4x3 left, MatrixFloat4x3 right)
		{
			return left.Equals (right);
		}

		public static bool operator != (MatrixFloat4x3 left, MatrixFloat4x3 right)
		{
			return !left.Equals (right);
		}

		public override string ToString ()
		{
			return
				$"({M11}, {M12}, {M13}, {M14})\n" +
				$"({M21}, {M22}, {M23}, {M24})\n" +
				$"({M31}, {M32}, {M33}, {M34})";
		}

		public override int GetHashCode ()
		{
			return
				M11.GetHashCode () ^ M12.GetHashCode () ^ M13.GetHashCode () ^ M14.GetHashCode () ^
				M21.GetHashCode () ^ M22.GetHashCode () ^ M23.GetHashCode () ^ M24.GetHashCode () ^
				M31.GetHashCode () ^ M32.GetHashCode () ^ M33.GetHashCode () ^ M34.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (!(obj is MatrixFloat4x3))
				return false;

			return Equals ((MatrixFloat4x3) obj);
		}

		public bool Equals (MatrixFloat4x3 other)
		{
			return
				M11 == other.M11 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14 &&
				M21 == other.M21 && M22 == other.M22 && M23 == other.M23 && M24 == other.M24 &&
				M31 == other.M31 && M32 == other.M32 && M33 == other.M33 && M34 == other.M34;
		}
	}
}
