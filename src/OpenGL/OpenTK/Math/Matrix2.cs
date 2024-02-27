#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.
Copyright 2014 Xamarin Inc

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion

#nullable enable
#pragma warning disable CS3021 // Type or member does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute

using System;
using System.Runtime.InteropServices;

#if !NET
namespace OpenTK {
	// Todo: Remove this warning when the code goes public.
#pragma warning disable 3019
	[Serializable]
	[StructLayout (LayoutKind.Sequential)]
	public struct Matrix2 : IEquatable<Matrix2> {
		#region Fields & Access

		/// <summary>Row 0, Column 0</summary>
		public float R0C0;

		/// <summary>Row 0, Column 1</summary>
		public float R0C1;

		/// <summary>Row 1, Column 0</summary>
		public float R1C0;

		/// <summary>Row 1, Column 1</summary>
		public float R1C1;

		/// <summary>Gets the component at the given row and column in the matrix.</summary>
		/// <param name="row">The row of the matrix.</param>
		/// <param name="column">The column of the matrix.</param>
		/// <returns>The component at the given row and column in the matrix.</returns>
		public float this [int row, int column] {
			get {
				switch (row) {
				case 0:
					switch (column) {
					case 0: return R0C0;
					case 1: return R0C1;
					}
					break;

				case 1:
					switch (column) {
					case 0: return R1C0;
					case 1: return R1C1;
					}
					break;
				}

				throw new IndexOutOfRangeException ();
			}
			set {
				switch (row) {
				case 0:
					switch (column) {
					case 0: R0C0 = value; return;
					case 1: R0C1 = value; return;
					}
					break;

				case 1:
					switch (column) {
					case 0: R1C0 = value; return;
					case 1: R1C1 = value; return;
					}
					break;
				}

				throw new IndexOutOfRangeException ();
			}
		}

		/// <summary>Gets the component at the index into the matrix.</summary>
		/// <param name="index">The index into the components of the matrix.</param>
		/// <returns>The component at the given index into the matrix.</returns>
		public float this [int index] {
			get {
				switch (index) {
				case 0: return R0C0;
				case 1: return R0C1;
				case 2: return R1C0;
				case 3: return R1C1;
				default: throw new IndexOutOfRangeException ();
				}
			}
			set {
				switch (index) {
				case 0: R0C0 = value; return;
				case 1: R0C1 = value; return;
				case 2: R1C0 = value; return;
				case 3: R1C1 = value; return;
				default: throw new IndexOutOfRangeException ();
				}
			}
		}

		/// <summary>Converts the matrix into an IntPtr.</summary>
		/// <param name="matrix">The matrix to convert.</param>
		/// <returns>An IntPtr for the matrix.</returns>
		public static explicit operator IntPtr (Matrix2 matrix)
		{
			unsafe {
				return (IntPtr) (&matrix.R0C0);
			}
		}

		/// <summary>Converts the matrix into left float*.</summary>
		/// <param name="matrix">The matrix to convert.</param>
		/// <returns>A float* for the matrix.</returns>
		[CLSCompliant (false)]
		unsafe public static explicit operator float* (Matrix2 matrix)
		{
			return &matrix.R0C0;
		}

		/// <summary>Converts the matrix into an array of floats.</summary>
		/// <param name="matrix">The matrix to convert.</param>
		/// <returns>An array of floats for the matrix.</returns>
		public static explicit operator float [] (Matrix2 matrix)
		{
			return new float [4]
			{
				matrix.R0C0,
				matrix.R0C1,
				matrix.R1C0,
				matrix.R1C1,
			};
		}

		#endregion

		#region Constructors

		/// <summary>Constructs left matrix with the same components as the given matrix.</summary>
		/// <param name="vector">The matrix whose components to copy.</param>
		public Matrix2 (ref Matrix2 matrix)
		{
			this.R0C0 = matrix.R0C0;
			this.R0C1 = matrix.R0C1;
			this.R1C0 = matrix.R1C0;
			this.R1C1 = matrix.R1C1;
		}

		/// <summary>Constructs left matrix with the given values.</summary>
		/// <param name="r0c0">The value for row 0 column 0.</param>
		/// <param name="r0c1">The value for row 0 column 1.</param>
		/// <param name="r1c0">The value for row 1 column 0.</param>
		/// <param name="r1c1">The value for row 1 column 1.</param>
		public Matrix2
		(
			float r0c0,
			float r0c1,
			float r1c0,
			float r1c1
		)
		{
			this.R0C0 = r0c0;
			this.R0C1 = r0c1;
			this.R1C0 = r1c0;
			this.R1C1 = r1c1;
		}

		/// <summary>Constructs left matrix from the given array of float-precision floating-point numbers.</summary>
		/// <param name="floatArray">The array of floats for the components of the matrix.</param>
		public Matrix2 (float [] floatArray)
		{
			if (floatArray is null || floatArray.GetLength (0) < 9)
				throw new MissingFieldException ();

			this.R0C0 = floatArray [0];
			this.R0C1 = floatArray [1];
			this.R1C0 = floatArray [2];
			this.R1C1 = floatArray [3];
		}

		#endregion

		#region Equality

		/// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
		/// <param name="matrix">The OpenTK.Matrix2 structure to compare with.</param>
		/// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
		[CLSCompliant (false)]
		public bool Equals (Matrix2 matrix)
		{
			return
				R0C0 == matrix.R0C0 &&
				R0C1 == matrix.R0C1 &&
				R1C0 == matrix.R1C0 &&
				R1C1 == matrix.R1C1;
		}

		/// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
		/// <param name="matrix">The OpenTK.Matrix2 structure to compare to.</param>
		/// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
		public bool Equals (ref Matrix2 matrix)
		{
			return
				R0C0 == matrix.R0C0 &&
				R0C1 == matrix.R0C1 &&
				R1C0 == matrix.R1C0 &&
				R1C1 == matrix.R1C1;
		}

		/// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
		/// <param name="left">The left-hand operand.</param>
		/// <param name="right">The right-hand operand.</param>
		/// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
		public static bool Equals (ref Matrix2 left, ref Matrix2 right)
		{
			return
				left.R0C0 == right.R0C0 &&
				left.R0C1 == right.R0C1 &&
				left.R1C0 == right.R1C0 &&
				left.R1C1 == right.R1C1;
		}

		/// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
		/// <param name="matrix">The OpenTK.Matrix2 structure to compare with.</param>
		/// <param name="tolerance">The limit below which the matrices are considered equal.</param>
		/// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
		public bool EqualsApprox (ref Matrix2 matrix, float tolerance)
		{
			return
				System.Math.Abs (R0C0 - matrix.R0C0) <= tolerance &&
				System.Math.Abs (R0C1 - matrix.R0C1) <= tolerance &&
				System.Math.Abs (R1C0 - matrix.R1C0) <= tolerance &&
				System.Math.Abs (R1C1 - matrix.R1C1) <= tolerance;
		}

		/// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
		/// <param name="left">The left-hand operand.</param>
		/// <param name="right">The right-hand operand.</param>
		/// <param name="tolerance">The limit below which the matrices are considered equal.</param>
		/// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
		public static bool EqualsApprox (ref Matrix2 left, ref Matrix2 right, float tolerance)
		{
			return
				System.Math.Abs (left.R0C0 - right.R0C0) <= tolerance &&
				System.Math.Abs (left.R0C1 - right.R0C1) <= tolerance &&
				System.Math.Abs (left.R1C0 - right.R1C0) <= tolerance &&
				System.Math.Abs (left.R1C1 - right.R1C1) <= tolerance;
		}

		#endregion

		#region Arithmetic Operators


		/// <summary>Add left matrix to this matrix.</summary>
		/// <param name="matrix">The matrix to add.</param>
		public void Add (ref Matrix2 matrix)
		{
			R0C0 = R0C0 + matrix.R0C0;
			R0C1 = R0C1 + matrix.R0C1;
			R1C0 = R1C0 + matrix.R1C0;
			R1C1 = R1C1 + matrix.R1C1;
		}

		/// <summary>Add left matrix to this matrix.</summary>
		/// <param name="matrix">The matrix to add.</param>
		/// <param name="result">The resulting matrix of the addition.</param>
		public void Add (ref Matrix2 matrix, out Matrix2 result)
		{
			result.R0C0 = R0C0 + matrix.R0C0;
			result.R0C1 = R0C1 + matrix.R0C1;
			result.R1C0 = R1C0 + matrix.R1C0;
			result.R1C1 = R1C1 + matrix.R1C1;
		}

		/// <summary>Add left matrix to left matrix.</summary>
		/// <param name="matrix">The matrix on the matrix side of the equation.</param>
		/// <param name="right">The matrix on the right side of the equation</param>
		/// <param name="result">The resulting matrix of the addition.</param>
		public static void Add (ref Matrix2 left, ref Matrix2 right, out Matrix2 result)
		{
			result.R0C0 = left.R0C0 + right.R0C0;
			result.R0C1 = left.R0C1 + right.R0C1;
			result.R1C0 = left.R1C0 + right.R1C0;
			result.R1C1 = left.R1C1 + right.R1C1;
		}


		/// <summary>Subtract left matrix from this matrix.</summary>
		/// <param name="matrix">The matrix to subtract.</param>
		public void Subtract (ref Matrix2 matrix)
		{
			R0C0 = R0C0 - matrix.R0C0;
			R0C1 = R0C1 - matrix.R0C1;
			R1C0 = R1C0 - matrix.R1C0;
			R1C1 = R1C1 - matrix.R1C1;
		}

		/// <summary>Subtract matrix from this matrix.</summary>
		/// <param name="matrix">The matrix to subtract.</param>
		/// <param name="result">The resulting matrix of the subtraction.</param>
		public void Subtract (ref Matrix2 matrix, out Matrix2 result)
		{
			result.R0C0 = R0C0 - matrix.R0C0;
			result.R0C1 = R0C1 - matrix.R0C1;
			result.R1C0 = R1C0 - matrix.R1C0;
			result.R1C1 = R1C1 - matrix.R1C1;
		}

		/// <summary>Subtract right matrix from left matrix.</summary>
		/// <param name="matrix">The matrix on the matrix side of the equation.</param>
		/// <param name="right">The matrix on the right side of the equation</param>
		/// <param name="result">The resulting matrix of the subtraction.</param>
		public static void Subtract (ref Matrix2 left, ref Matrix2 right, out Matrix2 result)
		{
			result.R0C0 = left.R0C0 - right.R0C0;
			result.R0C1 = left.R0C1 - right.R0C1;
			result.R1C0 = left.R1C0 - right.R1C0;
			result.R1C1 = left.R1C1 - right.R1C1;
		}


		/// <summary>Multiply left martix times this matrix.</summary>
		/// <param name="matrix">The matrix to multiply.</param>
		public void Multiply (ref Matrix2 matrix)
		{
			float r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
			float r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;

			float r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
			float r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;

			R0C0 = r0c0;
			R0C1 = r0c1;

			R1C0 = r1c0;
			R1C1 = r1c1;
		}

		/// <summary>Multiply matrix times this matrix.</summary>
		/// <param name="matrix">The matrix to multiply.</param>
		/// <param name="result">The resulting matrix of the multiplication.</param>
		public void Multiply (ref Matrix2 matrix, out Matrix2 result)
		{
			result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
			result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;
			result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
			result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;
		}

		/// <summary>Multiply left matrix times left matrix.</summary>
		/// <param name="matrix">The matrix on the matrix side of the equation.</param>
		/// <param name="right">The matrix on the right side of the equation</param>
		/// <param name="result">The resulting matrix of the multiplication.</param>
		public static void Multiply (ref Matrix2 left, ref Matrix2 right, out Matrix2 result)
		{
			result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0;
			result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1;
			result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0;
			result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1;
		}


		/// <summary>Multiply matrix times this matrix.</summary>
		/// <param name="matrix">The matrix to multiply.</param>
		public void Multiply (float scalar)
		{
			R0C0 = scalar * R0C0;
			R0C1 = scalar * R0C1;
			R1C0 = scalar * R1C0;
			R1C1 = scalar * R1C1;
		}

		/// <summary>Multiply matrix times this matrix.</summary>
		/// <param name="matrix">The matrix to multiply.</param>
		/// <param name="result">The resulting matrix of the multiplication.</param>
		public void Multiply (float scalar, out Matrix2 result)
		{
			result.R0C0 = scalar * R0C0;
			result.R0C1 = scalar * R0C1;
			result.R1C0 = scalar * R1C0;
			result.R1C1 = scalar * R1C1;
		}

		/// <summary>Multiply left matrix times left matrix.</summary>
		/// <param name="matrix">The matrix on the matrix side of the equation.</param>
		/// <param name="right">The matrix on the right side of the equation</param>
		/// <param name="result">The resulting matrix of the multiplication.</param>
		public static void Multiply (ref Matrix2 matrix, float scalar, out Matrix2 result)
		{
			result.R0C0 = scalar * matrix.R0C0;
			result.R0C1 = scalar * matrix.R0C1;
			result.R1C0 = scalar * matrix.R1C0;
			result.R1C1 = scalar * matrix.R1C1;
		}


		#endregion

		#region Functions

		public float Determinant {
			get {
				return R0C0 * R1C1 - R0C1 * R1C0;
			}
		}

		public void Transpose ()
		{
			MathHelper.Swap (ref R0C1, ref R1C0);
		}
		public void Transpose (out Matrix2 result)
		{
			result.R0C0 = R0C0;
			result.R0C1 = R1C0;
			result.R1C0 = R0C1;
			result.R1C1 = R1C1;
		}
		public static void Transpose (ref Matrix2 matrix, out Matrix2 result)
		{
			result.R0C0 = matrix.R0C0;
			result.R0C1 = matrix.R1C0;
			result.R1C0 = matrix.R0C1;
			result.R1C1 = matrix.R1C1;
		}

		#endregion

		#region Transformation Functions

		public void Transform (ref Vector2 vector)
		{
			float x = R0C0 * vector.X + R0C1 * vector.Y;
			vector.Y = R1C0 * vector.X + R1C1 * vector.Y;
			vector.X = x;
		}
		public static void Transform (ref Matrix2 matrix, ref Vector2 vector)
		{
			float x = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
			vector.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
			vector.X = x;
		}
		public void Transform (ref Vector2 vector, out Vector2 result)
		{
			result.X = R0C0 * vector.X + R0C1 * vector.Y;
			result.Y = R1C0 * vector.X + R1C1 * vector.Y;
		}
		public static void Transform (ref Matrix2 matrix, ref Vector2 vector, out Vector2 result)
		{
			result.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
			result.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
		}

		public void Rotate (float angle)
		{
			float angleRadians = MathHelper.DegreesToRadians (angle);
			float sin = (float) System.Math.Sin (angleRadians);
			float cos = (float) System.Math.Cos (angleRadians);

			float r0c0 = cos * R0C0 - sin * R1C0;
			float r0c1 = cos * R0C1 - sin * R1C1;

			R1C0 = cos * R1C0 + sin * R0C0;
			R1C1 = cos * R1C1 + sin * R0C1;

			R0C0 = r0c0;
			R0C1 = r0c1;
		}
		public void Rotate (float angle, out Matrix2 result)
		{
			float angleRadians = MathHelper.DegreesToRadians (angle);
			float sin = (float) System.Math.Sin (angleRadians);
			float cos = (float) System.Math.Cos (angleRadians);

			result.R0C0 = cos * R0C0 - sin * R1C0;
			result.R0C1 = cos * R0C1 - sin * R1C1;
			result.R1C0 = cos * R1C0 + sin * R0C0;
			result.R1C1 = cos * R1C1 + sin * R0C1;
		}
		public static void Rotate (ref Matrix2 matrix, float angle, out Matrix2 result)
		{
			float angleRadians = MathHelper.DegreesToRadians (angle);
			float sin = (float) System.Math.Sin (angleRadians);
			float cos = (float) System.Math.Cos (angleRadians);

			result.R0C0 = cos * matrix.R0C0 - sin * matrix.R1C0;
			result.R0C1 = cos * matrix.R0C1 - sin * matrix.R1C1;
			result.R1C0 = cos * matrix.R1C0 + sin * matrix.R0C0;
			result.R1C1 = cos * matrix.R1C1 + sin * matrix.R0C1;
		}
		public static void RotateMatrix (float angle, out Matrix2 result)
		{
			float angleRadians = MathHelper.DegreesToRadians (angle);
			float sin = (float) System.Math.Sin (angleRadians);
			float cos = (float) System.Math.Cos (angleRadians);

			result.R0C0 = cos;
			result.R0C1 = -sin;
			result.R1C0 = sin;
			result.R1C1 = cos;
		}

		#endregion

		#region Constants

		/// <summary>The identity matrix.</summary>
		public static readonly Matrix2 Identity = new Matrix2
		(
			1, 0,
			0, 1
		);

		/// <summary>A matrix of all zeros.</summary>
		public static readonly Matrix2 Zero = new Matrix2
		(
			0, 0,
			0, 0
		);

		#endregion

		#region HashCode

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode ()
		{
			return
				R0C0.GetHashCode () ^ R0C1.GetHashCode () ^
				R1C0.GetHashCode () ^ R1C1.GetHashCode ();
		}

		#endregion

		#region String

		/// <summary>Returns the fully qualified type name of this instance.</summary>
		/// <returns>A System.String containing left fully qualified type name.</returns>
		public override string ToString ()
		{
			return String.Format (
				"|{00}, {01}|\n" +
				"|{02}, {03}|\n",
				R0C0, R0C1,
				R1C0, R1C1);
		}

		#endregion
	}
#pragma warning restore 3019
}
#endif // !NET
