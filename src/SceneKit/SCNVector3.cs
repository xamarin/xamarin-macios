#region --- License ---
/*
 * Copyright 2014 Xamarin Inc
 *
 * Altered to use dual types on Mac, and floats on iOS
 
Copyright (c) 2006 - 2008 The Open Toolkit library.

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

using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Runtime.Versioning;

#if NET
using Vector2 = global::System.Numerics.Vector2;
using Vector3 = global::System.Numerics.Vector3;
using MathHelper = global::CoreGraphics.MathHelper;
#else
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using MathHelper = global::OpenTK.MathHelper;
#endif

#if MONOMAC
#if NET
using pfloat = System.Runtime.InteropServices.NFloat;
#else
using pfloat = System.nfloat;
#endif
#else
using pfloat = System.Single;
#endif

#nullable enable

namespace SceneKit {
	/// <summary>
	/// Represents a 3D vector using three single-precision floating-point numbers.
	/// </summary>
	/// <remarks>
	/// The Vector3 structure is suitable for interoperation with unmanaged code requiring three consecutive floats.
	/// </remarks>
#if NET
    [SupportedOSPlatform ("ios")]
    [SupportedOSPlatform ("maccatalyst")]
    [SupportedOSPlatform ("macos")]
    [SupportedOSPlatform ("tvos")]
#endif
	[Serializable]
	[StructLayout (LayoutKind.Sequential)]
	public struct SCNVector3 : IEquatable<SCNVector3> {
		#region Fields

		/// <summary>
		/// The X component of the Vector3.
		/// </summary>
		public pfloat X;

		/// <summary>
		/// The Y component of the Vector3.
		/// </summary>
		public pfloat Y;

		/// <summary>
		/// The Z component of the Vector3.
		/// </summary>
		public pfloat Z;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Vector3.
		/// </summary>
		/// <param name="x">The x component of the Vector3.</param>
		/// <param name="y">The y component of the Vector3.</param>
		/// <param name="z">The z component of the Vector3.</param>
		public SCNVector3 (pfloat x, pfloat y, pfloat z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public SCNVector3 (Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		/// Constructs a new Vector3 from the given Vector3.
		/// </summary>
		/// <param name="v">The Vector3 to copy components from.</param>
		public SCNVector3 (SCNVector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		/// Constructs a new Vector3 from the given Vector4.
		/// </summary>
		/// <param name="v">The Vector4 to copy components from.</param>
		public SCNVector3 (SCNVector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		#endregion

		#region Public Members

		#region Instance

		#region public pfloat Length

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <see cref="LengthFast"/>
		/// <seealso cref="LengthSquared"/>
		public pfloat Length {
			get {
				return (pfloat) System.Math.Sqrt (X * X + Y * Y + Z * Z);
			}
		}

		#endregion

		#region public pfloat LengthFast

		/// <summary>
		/// Gets an approximation of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property uses an approximation of the square root function to calculate vector magnitude, with
		/// an upper error bound of 0.001.
		/// </remarks>
		/// <see cref="Length"/>
		/// <seealso cref="LengthSquared"/>
		public pfloat LengthFast {
			get {
				return (pfloat) (1.0f / MathHelper.InverseSqrtFast (X * X + Y * Y + Z * Z));
			}
		}

		#endregion

		#region public pfloat LengthSquared

		/// <summary>
		/// Gets the square of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property avoids the costly square root operation required by the Length property. This makes it more suitable
		/// for comparisons.
		/// </remarks>
		/// <see cref="Length"/>
		/// <seealso cref="LengthFast"/>
		public pfloat LengthSquared {
			get {
				return X * X + Y * Y + Z * Z;
			}
		}

		#endregion

		#region public void Normalize()

		/// <summary>
		/// Scales the SCNVector3 to unit length.
		/// </summary>
		public void Normalize ()
		{
			pfloat scale = 1.0f / this.Length;
			X *= scale;
			Y *= scale;
			Z *= scale;
		}

		#endregion

		#region public void NormalizeFast()

		/// <summary>
		/// Scales the SCNVector3 to approximately unit length.
		/// </summary>
		public void NormalizeFast ()
		{
			pfloat scale = (pfloat) MathHelper.InverseSqrtFast (X * X + Y * Y + Z * Z);
			X *= scale;
			Y *= scale;
			Z *= scale;
		}

		#endregion

		#endregion

		#region Static

		#region Fields

		/// <summary>
		/// Defines a unit-length SCNVector3 that points towards the X-axis.
		/// </summary>
		public static readonly SCNVector3 UnitX = new SCNVector3 (1, 0, 0);

		/// <summary>
		/// Defines a unit-length SCNVector3 that points towards the Y-axis.
		/// </summary>
		public static readonly SCNVector3 UnitY = new SCNVector3 (0, 1, 0);

		/// <summary>
		/// /// Defines a unit-length SCNVector3 that points towards the Z-axis.
		/// </summary>
		public static readonly SCNVector3 UnitZ = new SCNVector3 (0, 0, 1);

		/// <summary>
		/// Defines a zero-length SCNVector3.
		/// </summary>
		public static readonly SCNVector3 Zero = new SCNVector3 (0, 0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly SCNVector3 One = new SCNVector3 (1, 1, 1);

		/// <summary>
		/// Defines the size of the SCNVector3 struct in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Marshal.SizeOf<SCNVector3> ();

		#endregion

		#region Add

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static SCNVector3 Add (SCNVector3 a, SCNVector3 b)
		{
			Add (ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add (ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
		{
			result = new SCNVector3 (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		#endregion

		#region Subtract

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static SCNVector3 Subtract (SCNVector3 a, SCNVector3 b)
		{
			Subtract (ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract (ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
		{
			result = new SCNVector3 (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		#endregion

		#region Multiply

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector3 Multiply (SCNVector3 vector, pfloat scale)
		{
			Multiply (ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply (ref SCNVector3 vector, pfloat scale, out SCNVector3 result)
		{
			result = new SCNVector3 (vector.X * scale, vector.Y * scale, vector.Z * scale);
		}

		/// <summary>
		/// Multiplies a vector by the components a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector3 Multiply (SCNVector3 vector, SCNVector3 scale)
		{
			Multiply (ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply (ref SCNVector3 vector, ref SCNVector3 scale, out SCNVector3 result)
		{
			result = new SCNVector3 (vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z);
		}

		#endregion

		#region Divide

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector3 Divide (SCNVector3 vector, pfloat scale)
		{
			Divide (ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide (ref SCNVector3 vector, pfloat scale, out SCNVector3 result)
		{
			Multiply (ref vector, 1 / scale, out result);
		}

		/// <summary>
		/// Divides a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector3 Divide (SCNVector3 vector, SCNVector3 scale)
		{
			Divide (ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divide a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide (ref SCNVector3 vector, ref SCNVector3 scale, out SCNVector3 result)
		{
			result = new SCNVector3 (vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z);
		}

		#endregion

		#region ComponentMin

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static SCNVector3 ComponentMin (SCNVector3 a, SCNVector3 b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			a.Z = a.Z < b.Z ? a.Z : b.Z;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void ComponentMin (ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
		{
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
			result.Z = a.Z < b.Z ? a.Z : b.Z;
		}

		#endregion

		#region ComponentMax

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static SCNVector3 ComponentMax (SCNVector3 a, SCNVector3 b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			a.Z = a.Z > b.Z ? a.Z : b.Z;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void ComponentMax (ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
		{
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
			result.Z = a.Z > b.Z ? a.Z : b.Z;
		}

		#endregion

		#region Min

		/// <summary>
		/// Returns the SCNVector3 with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum SCNVector3</returns>
		public static SCNVector3 Min (SCNVector3 left, SCNVector3 right)
		{
			return left.LengthSquared < right.LengthSquared ? left : right;
		}

		#endregion

		#region Max

		/// <summary>
		/// Returns the SCNVector3 with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum SCNVector3</returns>
		public static SCNVector3 Max (SCNVector3 left, SCNVector3 right)
		{
			return left.LengthSquared >= right.LengthSquared ? left : right;
		}

		#endregion

		#region Clamp

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <returns>The clamped vector</returns>
		public static SCNVector3 Clamp (SCNVector3 vec, SCNVector3 min, SCNVector3 max)
		{
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			vec.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp (ref SCNVector3 vec, ref SCNVector3 min, ref SCNVector3 max, out SCNVector3 result)
		{
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			result.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
		}

		#endregion

		#region Normalize

		/// <summary>
		/// Scale a vector to unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <returns>The normalized vector</returns>
		public static SCNVector3 Normalize (SCNVector3 vec)
		{
			pfloat scale = 1.0f / vec.Length;
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		/// <summary>
		/// Scale a vector to unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <param name="result">The normalized vector</param>
		public static void Normalize (ref SCNVector3 vec, out SCNVector3 result)
		{
			pfloat scale = 1.0f / vec.Length;
			result.X = vec.X * scale;
			result.Y = vec.Y * scale;
			result.Z = vec.Z * scale;
		}

		#endregion

		#region NormalizeFast

		/// <summary>
		/// Scale a vector to approximately unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <returns>The normalized vector</returns>
		public static SCNVector3 NormalizeFast (SCNVector3 vec)
		{
			pfloat scale = (pfloat) MathHelper.InverseSqrtFast (vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		/// <summary>
		/// Scale a vector to approximately unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <param name="result">The normalized vector</param>
		public static void NormalizeFast (ref SCNVector3 vec, out SCNVector3 result)
		{
			pfloat scale = (pfloat) MathHelper.InverseSqrtFast (vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
			result.X = vec.X * scale;
			result.Y = vec.Y * scale;
			result.Z = vec.Z * scale;
		}

		#endregion

		#region Dot

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static pfloat Dot (SCNVector3 left, SCNVector3 right)
		{
			return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot (ref SCNVector3 left, ref SCNVector3 right, out pfloat result)
		{
			result = left.X * right.X + left.Y * right.Y + left.Z * right.Z;
		}

		#endregion

		#region Cross

		/// <summary>
		/// Caclulate the cross (vector) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The cross product of the two inputs</returns>
		public static SCNVector3 Cross (SCNVector3 left, SCNVector3 right)
		{
			return new SCNVector3 (left.Y * right.Z - left.Z * right.Y,
							   left.Z * right.X - left.X * right.Z,
							   left.X * right.Y - left.Y * right.X);
		}

		/// <summary>
		/// Caclulate the cross (vector) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The cross product of the two inputs</returns>
		/// <param name="result">The cross product of the two inputs</param>
		public static void Cross (ref SCNVector3 left, ref SCNVector3 right, out SCNVector3 result)
		{
			result.X = left.Y * right.Z - left.Z * right.Y;
			result.Y = left.Z * right.X - left.X * right.Z;
			result.Z = left.X * right.Y - left.Y * right.X;
		}

		#endregion

		#region Lerp

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
		public static SCNVector3 Lerp (SCNVector3 a, SCNVector3 b, pfloat blend)
		{
			a.X = blend * (b.X - a.X) + a.X;
			a.Y = blend * (b.Y - a.Y) + a.Y;
			a.Z = blend * (b.Z - a.Z) + a.Z;
			return a;
		}

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
		public static void Lerp (ref SCNVector3 a, ref SCNVector3 b, pfloat blend, out SCNVector3 result)
		{
			result.X = blend * (b.X - a.X) + a.X;
			result.Y = blend * (b.Y - a.Y) + a.Y;
			result.Z = blend * (b.Z - a.Z) + a.Z;
		}

		#endregion

		#region Barycentric

		/// <summary>
		/// Interpolate 3 Vectors using Barycentric coordinates
		/// </summary>
		/// <param name="a">First input Vector</param>
		/// <param name="b">Second input Vector</param>
		/// <param name="c">Third input Vector</param>
		/// <param name="u">First Barycentric Coordinate</param>
		/// <param name="v">Second Barycentric Coordinate</param>
		/// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
		public static SCNVector3 BaryCentric (SCNVector3 a, SCNVector3 b, SCNVector3 c, pfloat u, pfloat v)
		{
			return a + u * (b - a) + v * (c - a);
		}

		/// <summary>Interpolate 3 Vectors using Barycentric coordinates</summary>
		/// <param name="a">First input Vector.</param>
		/// <param name="b">Second input Vector.</param>
		/// <param name="c">Third input Vector.</param>
		/// <param name="u">First Barycentric Coordinate.</param>
		/// <param name="v">Second Barycentric Coordinate.</param>
		/// <param name="result">Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</param>
		public static void BaryCentric (ref SCNVector3 a, ref SCNVector3 b, ref SCNVector3 c, pfloat u, pfloat v, out SCNVector3 result)
		{
			result = a; // copy

			SCNVector3 temp = b; // copy
			Subtract (ref temp, ref a, out temp);
			Multiply (ref temp, u, out temp);
			Add (ref result, ref temp, out result);

			temp = c; // copy
			Subtract (ref temp, ref a, out temp);
			Multiply (ref temp, v, out temp);
			Add (ref result, ref temp, out result);
		}

		#endregion

		#region Transform

#if NET
        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a right-most column of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The column vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed vector</returns>
#else
		/// <summary>Transform a direction vector by the given Matrix
		/// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
		/// </summary>
		/// <param name="vec">The row vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed vector</returns>
#endif
		public static SCNVector3 TransformVector (SCNVector3 vec, SCNMatrix4 mat)
		{
			TransformVector (ref vec, ref mat, out var v);
			return v;
		}

#if NET
        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a right-most column of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The column vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
#else
		/// <summary>Transform a direction vector by the given Matrix
		/// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
		/// </summary>
		/// <param name="vec">The row vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
#endif
		public static void TransformVector (ref SCNVector3 vec, ref SCNMatrix4 mat, out SCNVector3 result)
		{
#if NET
            result.X = vec.X * mat.Row0.X +
                       vec.Y * mat.Row0.Y +
                       vec.Z * mat.Row0.Z;

            result.Y = vec.X * mat.Row1.X +
                       vec.Y * mat.Row1.Y +
                       vec.Z * mat.Row1.Z;

            result.Z = vec.X * mat.Row2.X +
                       vec.Y * mat.Row2.Y +
                       vec.Z * mat.Row2.Z;
#else
			result.X = vec.X * mat.Row0.X +
					   vec.Y * mat.Row1.X +
					   vec.Z * mat.Row2.X;

			result.Y = vec.X * mat.Row0.Y +
					   vec.Y * mat.Row1.Y +
					   vec.Z * mat.Row2.Y;

			result.Z = vec.X * mat.Row0.Z +
					   vec.Y * mat.Row1.Z +
					   vec.Z * mat.Row2.Z;
#endif
		}

#if NET
		/// <summary>Transform a Normal by the given Matrix</summary>
		/// <remarks>
		/// This calculates the inverse of the given matrix, use TransformNormalInverse if you
		/// already have the inverse to avoid this extra calculation
		/// </remarks>
        /// <param name="norm">The column-based normal to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed normal</returns>
#else
		/// <summary>Transform a Normal by the given Matrix</summary>
		/// <remarks>
		/// This calculates the inverse of the given matrix, use TransformNormalInverse if you
		/// already have the inverse to avoid this extra calculation
		/// </remarks>
		/// <param name="norm">The row-based normal to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed normal</returns>
#endif
		public static SCNVector3 TransformNormal (SCNVector3 norm, SCNMatrix4 mat)
		{
			mat.Invert ();
			return TransformNormalInverse (norm, mat);
		}

#if NET
		/// <summary>Transform a Normal by the given Matrix</summary>
		/// <remarks>
		/// This calculates the inverse of the given matrix, use TransformNormalInverse if you
		/// already have the inverse to avoid this extra calculation
		/// </remarks>
        /// <param name="norm">The column-based normal to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed normal</param>
#else
		/// <summary>Transform a Normal by the given Matrix</summary>
		/// <remarks>
		/// This calculates the inverse of the given matrix, use TransformNormalInverse if you
		/// already have the inverse to avoid this extra calculation
		/// </remarks>
		/// <param name="norm">The row-based normal to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed normal</param>
#endif
		public static void TransformNormal (ref SCNVector3 norm, ref SCNMatrix4 mat, out SCNVector3 result)
		{
			SCNMatrix4 Inverse = SCNMatrix4.Invert (mat);
			SCNVector3.TransformNormalInverse (ref norm, ref Inverse, out result);
		}

#if NET
		/// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
		/// <remarks>
		/// This version doesn't calculate the inverse matrix.
		/// Use this version if you already have the inverse of the desired transform to hand
		/// </remarks>
        /// <param name="norm">The column-based normal to transform</param>
		/// <param name="invMat">The inverse of the desired transformation</param>
		/// <returns>The transformed normal</returns>
#else
		/// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
		/// <remarks>
		/// This version doesn't calculate the inverse matrix.
		/// Use this version if you already have the inverse of the desired transform to hand
		/// </remarks>
		/// <param name="norm">The row-based normal to transform</param>
		/// <param name="invMat">The inverse of the desired transformation</param>
		/// <returns>The transformed normal</returns>
#endif
		public static SCNVector3 TransformNormalInverse (SCNVector3 norm, SCNMatrix4 invMat)
		{
			TransformNormalInverse (ref norm, ref invMat, out var n);
			return n;
		}

#if NET
		/// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
		/// <remarks>
		/// This version doesn't calculate the inverse matrix.
		/// Use this version if you already have the inverse of the desired transform to hand
		/// </remarks>
        /// <param name="norm">The column-based normal to transform</param>
		/// <param name="invMat">The inverse of the desired transformation</param>
		/// <param name="result">The transformed normal</param>
#else
		/// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
		/// <remarks>
		/// This version doesn't calculate the inverse matrix.
		/// Use this version if you already have the inverse of the desired transform to hand
		/// </remarks>
		/// <param name="norm">The row-based normal to transform</param>
		/// <param name="invMat">The inverse of the desired transformation</param>
		/// <param name="result">The transformed normal</param>
#endif
		public static void TransformNormalInverse (ref SCNVector3 norm, ref SCNMatrix4 invMat, out SCNVector3 result)
		{
#if NET
            result.X = norm.X * invMat.Column0.X +
                       norm.Y * invMat.Column0.Y +
                       norm.Z * invMat.Column0.Z;

            result.Y = norm.X * invMat.Column1.X +
                       norm.Y * invMat.Column1.Y +
                       norm.Z * invMat.Column1.Z;

            result.Z = norm.X * invMat.Column2.X +
                       norm.Y * invMat.Column2.Y +
                       norm.Z * invMat.Column2.Z;
#else
			result.X = norm.X * invMat.Row0.X +
					   norm.Y * invMat.Row0.Y +
					   norm.Z * invMat.Row0.Z;

			result.Y = norm.X * invMat.Row1.X +
					   norm.Y * invMat.Row1.Y +
					   norm.Z * invMat.Row1.Z;

			result.Z = norm.X * invMat.Row2.X +
					   norm.Y * invMat.Row2.Y +
					   norm.Z * invMat.Row2.Z;
#endif
		}

#if NET
		/// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The column-based position to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed position</returns>
#else
		/// <summary>Transform a Position by the given Matrix</summary>
		/// <param name="pos">The row-based position to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed position</returns>
#endif
		public static SCNVector3 TransformPosition (SCNVector3 pos, SCNMatrix4 mat)
		{
			TransformPosition (ref pos, ref mat, out var p);
			return p;
		}

#if NET
		/// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The column-based position to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed position</param>
#else
		/// <summary>Transform a Position by the given Matrix</summary>
		/// <param name="pos">The row-based position to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed position</param>
#endif
		public static void TransformPosition (ref SCNVector3 pos, ref SCNMatrix4 mat, out SCNVector3 result)
		{
#if NET
            result.X = mat.Row0.X * pos.X +
                       mat.Row0.Y * pos.Y +
                       mat.Row0.Z * pos.Z +
                       mat.Row0.W;

            result.Y = mat.Row1.X * pos.X +
                       mat.Row1.Y * pos.Y +
                       mat.Row1.Z * pos.Z +
                       mat.Row1.W;

            result.Z = mat.Row2.X * pos.X +
                       mat.Row2.Y * pos.Y +
                       mat.Row2.Z * pos.Z +
                       mat.Row2.W;
#else
			result.X = pos.X * mat.Row0.X +
					   pos.Y * mat.Row1.X +
					   pos.Z * mat.Row2.X +
					   mat.Row3.X;

			result.Y = pos.X * mat.Row0.Y +
					   pos.Y * mat.Row1.Y +
					   pos.Z * mat.Row2.Y +
					   mat.Row3.Y;

			result.Z = pos.X * mat.Row0.Z +
					   pos.Y * mat.Row1.Z +
					   pos.Z * mat.Row2.Z +
					   mat.Row3.Z;
#endif
		}

#if NET
		/// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The column vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed vector</returns>
#else
		/// <summary>Transform a Vector by the given Matrix</summary>
		/// <param name="vec">The row vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed vector</returns>
#endif
		public static SCNVector4 Transform (SCNVector3 vec, SCNMatrix4 mat)
		{
			SCNVector4 v4 = new SCNVector4 (vec.X, vec.Y, vec.Z, 1.0f);
			return SCNVector4.Transform (v4, mat);
		}

#if NET
		/// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The column vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
#else
		/// <summary>Transform a Vector by the given Matrix</summary>
		/// <param name="vec">The row vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
#endif
		public static void Transform (ref SCNVector3 vec, ref SCNMatrix4 mat, out SCNVector4 result)
		{
			SCNVector4 v4 = new SCNVector4 (vec.X, vec.Y, vec.Z, 1.0f);
			SCNVector4.Transform (ref v4, ref mat, out result);
		}

		/// <summary>Transform a SCNVector3 by the given Matrix, and project the resulting Vector4 back to a SCNVector3</summary>
		/// <param name="vec">The vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed vector</returns>
		public static SCNVector3 TransformPerspective (SCNVector3 vec, SCNMatrix4 mat)
		{
			SCNVector4 h = Transform (vec, mat);
			return new SCNVector3 (h.X / h.W, h.Y / h.W, h.Z / h.W);
		}

		/// <summary>Transform a SCNVector3 by the given Matrix, and project the resulting SCNVector4 back to a SCNVector3</summary>
		/// <param name="vec">The vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
		public static void TransformPerspective (ref SCNVector3 vec, ref SCNMatrix4 mat, out SCNVector3 result)
		{
			SCNVector4 h;
			SCNVector3.Transform (ref vec, ref mat, out h);
			result.X = h.X / h.W;
			result.Y = h.Y / h.W;
			result.Z = h.Z / h.W;
		}

		#endregion

		#region CalculateAngle

		/// <summary>
		/// Calculates the angle (in radians) between two vectors.
		/// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
		/// <returns>Angle (in radians) between the vectors.</returns>
		/// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
		public static pfloat CalculateAngle (SCNVector3 first, SCNVector3 second)
		{
			return (pfloat) System.Math.Acos ((SCNVector3.Dot (first, second)) / (first.Length * second.Length));
		}

		/// <summary>Calculates the angle (in radians) between two vectors.</summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
		/// <param name="result">Angle (in radians) between the vectors.</param>
		/// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
		public static void CalculateAngle (ref SCNVector3 first, ref SCNVector3 second, out pfloat result)
		{
			pfloat temp;
			SCNVector3.Dot (ref first, ref second, out temp);
			result = (pfloat) System.Math.Acos (temp / (first.Length * second.Length));
		}

		#endregion

		#endregion

		#region Swizzle

		/// <summary>
		/// Gets or sets an OpenTK.Vector2 with the X and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2 Xy { get { return new Vector2 ((float) X, (float) Y); } set { X = value.X; Y = value.Y; } }

		#endregion

		#region Operators

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector3 operator + (SCNVector3 left, SCNVector3 right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			return left;
		}

		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector3 operator - (SCNVector3 left, SCNVector3 right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			return left;
		}

		/// <summary>
		/// Negates an instance.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector3 operator - (SCNVector3 vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			return vec;
		}

		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector3 operator * (SCNVector3 vec, pfloat scale)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="scale">The scalar.</param>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector3 operator * (pfloat scale, SCNVector3 vec)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		/// <summary>
		/// Divides an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector3 operator / (SCNVector3 vec, pfloat scale)
		{
			pfloat mult = 1.0f / scale;
			vec.X *= mult;
			vec.Y *= mult;
			vec.Z *= mult;
			return vec;
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator == (SCNVector3 left, SCNVector3 right)
		{
			return left.Equals (right);
		}

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equa lright; false otherwise.</returns>
		public static bool operator != (SCNVector3 left, SCNVector3 right)
		{
			return !left.Equals (right);
		}

		#endregion

		#region Overrides

		#region public override string ToString()

		/// <summary>
		/// Returns a System.String that represents the current SCNVector3.
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2})", X, Y, Z);
		}

		#endregion

		#region public override int GetHashCode()

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode ()
		{
			return HashCode.Combine (X, Y, Z);
		}

		#endregion

		#region public override bool Equals(object obj)

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals (object? obj)
		{
			if (!(obj is SCNVector3))
				return false;

			return this.Equals ((SCNVector3) obj);
		}

		#endregion

		#endregion

		#endregion

		#region IEquatable<SCNVector3> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals (SCNVector3 other)
		{
			return
				X == other.X &&
				Y == other.Y &&
				Z == other.Z;
		}

		#endregion
		public static implicit operator SCNVector3 (Vector3 vector)
		{
			return new SCNVector3 (vector.X, vector.Y, vector.Z);
		}

		public static explicit operator Vector3 (SCNVector3 source)
		{
			return new Vector3 ((float) source.X, (float) source.Y, (float) source.Z);
		}
	}
}
