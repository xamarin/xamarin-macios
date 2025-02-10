#region --- License ---
/*
 * Copyright 2014 Xamarin Inc, All Rights Reserved
 
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
using Vector4 = global::System.Numerics.Vector4;
using MathHelper = global::CoreGraphics.MathHelper;
#else
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Quaternion = global::OpenTK.Quaternion;
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
	/// <summary>Represents a 4D vector using four single-precision floating-point numbers.</summary>
	/// <remarks>
	/// The Vector4 structure is suitable for interoperation with unmanaged code requiring four consecutive floats.
	/// </remarks>
#if NET
    [SupportedOSPlatform ("ios")]
    [SupportedOSPlatform ("maccatalyst")]
    [SupportedOSPlatform ("macos")]
    [SupportedOSPlatform ("tvos")]
#endif
	[Serializable]
	[StructLayout (LayoutKind.Sequential)]
	public struct SCNVector4 : IEquatable<SCNVector4> {
		#region Fields

		/// <summary>
		/// The X component of the SCNVector4.
		/// </summary>
		public pfloat X;

		/// <summary>
		/// The Y component of the SCNVector4.
		/// </summary>
		public pfloat Y;

		/// <summary>
		/// The Z component of the SCNVector4.
		/// </summary>
		public pfloat Z;

		/// <summary>
		/// The W component of the SCNVector4.
		/// </summary>
		public pfloat W;

		/// <summary>
		/// Defines a unit-length SCNVector4 that points towards the X-axis.
		/// </summary>
		public readonly static SCNVector4 UnitX = new SCNVector4 (1, 0, 0, 0);

		/// <summary>
		/// Defines a unit-length SCNVector4 that points towards the Y-axis.
		/// </summary>
		public readonly static SCNVector4 UnitY = new SCNVector4 (0, 1, 0, 0);

		/// <summary>
		/// Defines a unit-length SCNVector4 that points towards the Z-axis.
		/// </summary>
		public readonly static SCNVector4 UnitZ = new SCNVector4 (0, 0, 1, 0);

		/// <summary>
		/// Defines a unit-length SCNVector4 that points towards the W-axis.
		/// </summary>
		public readonly static SCNVector4 UnitW = new SCNVector4 (0, 0, 0, 1);

		/// <summary>
		/// Defines a zero-length SCNVector4.
		/// </summary>
		public readonly static SCNVector4 Zero = new SCNVector4 (0, 0, 0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly SCNVector4 One = new SCNVector4 (1, 1, 1, 1);

		/// <summary>
		/// Defines the size of the SCNVector4 struct in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Marshal.SizeOf<SCNVector4> ();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new SCNVector4.
		/// </summary>
		/// <param name="x">The x component of the SCNVector4.</param>
		/// <param name="y">The y component of the SCNVector4.</param>
		/// <param name="z">The z component of the SCNVector4.</param>
		/// <param name="w">The z component of the SCNVector4.</param>
		public SCNVector4 (pfloat x, pfloat y, pfloat z, pfloat w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		/// <summary>
		/// Constructs a new SCNVector4 from the given Vector2.
		/// </summary>
		/// <param name="v">The Vector2 to copy components from.</param>
		public SCNVector4 (Vector2 v)
		{
			X = v.X;
			Y = v.Y;
			Z = 0.0f;
			W = 0.0f;
		}

		/// <summary>
		/// Constructs a new SCNVector4 from the given Vector3.
		/// </summary>
		/// <param name="v">The Vector3 to copy components from.</param>
		public SCNVector4 (SCNVector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = 0.0f;
		}


		public SCNVector4 (Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = 0.0f;
		}

		public SCNVector4 (Vector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = v.W;
		}

		/// <summary>
		/// Constructs a new SCNVector4 from the specified Vector3 and W component.
		/// </summary>
		/// <param name="v">The Vector3 to copy components from.</param>
		/// <param name="w">The W component of the new SCNVector4.</param>
		public SCNVector4 (SCNVector3 v, pfloat w)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = w;
		}

		/// <summary>
		/// Constructs a new SCNVector4 from the given SCNVector4.
		/// </summary>
		/// <param name="v">The SCNVector4 to copy components from.</param>
		public SCNVector4 (SCNVector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = v.W;
		}

		#endregion

		#region Public Members

		#region Instance

		#region public float Length

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <see cref="LengthFast"/>
		/// <seealso cref="LengthSquared"/>
		public pfloat Length {
			get {
				return (pfloat) System.Math.Sqrt (X * X + Y * Y + Z * Z + W * W);
			}
		}

		#endregion

		#region public float LengthFast

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
				return (pfloat) (1.0f / MathHelper.InverseSqrtFast (X * X + Y * Y + Z * Z + W * W));
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
				return X * X + Y * Y + Z * Z + W * W;
			}
		}

		#endregion

		#region public void Normalize()

		/// <summary>
		/// Scales the SCNVector4 to unit length.
		/// </summary>
		public void Normalize ()
		{
			pfloat scale = 1.0f / this.Length;
			X *= scale;
			Y *= scale;
			Z *= scale;
			W *= scale;
		}

		#endregion

		#region public void NormalizeFast()

		/// <summary>
		/// Scales the SCNVector4 to approximately unit length.
		/// </summary>
		public void NormalizeFast ()
		{
			pfloat scale = (pfloat) (MathHelper.InverseSqrtFast (X * X + Y * Y + Z * Z + W * W));
			X *= scale;
			Y *= scale;
			Z *= scale;
			W *= scale;
		}

		#endregion

		#endregion

		#region Static

		#region Obsolete

		#region Sub

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static SCNVector4 Sub (SCNVector4 a, SCNVector4 b)
		{
			a.X -= b.X;
			a.Y -= b.Y;
			a.Z -= b.Z;
			a.W -= b.W;
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Sub (ref SCNVector4 a, ref SCNVector4 b, out SCNVector4 result)
		{
			result.X = a.X - b.X;
			result.Y = a.Y - b.Y;
			result.Z = a.Z - b.Z;
			result.W = a.W - b.W;
		}

		#endregion

		#region Mult

		/// <summary>
		/// Multiply a vector and a scalar
		/// </summary>
		/// <param name="a">Vector operand</param>
		/// <param name="f">Scalar operand</param>
		/// <returns>Result of the multiplication</returns>
		public static SCNVector4 Mult (SCNVector4 a, pfloat f)
		{
			a.X *= f;
			a.Y *= f;
			a.Z *= f;
			a.W *= f;
			return a;
		}

		/// <summary>
		/// Multiply a vector and a scalar
		/// </summary>
		/// <param name="a">Vector operand</param>
		/// <param name="f">Scalar operand</param>
		/// <param name="result">Result of the multiplication</param>
		public static void Mult (ref SCNVector4 a, pfloat f, out SCNVector4 result)
		{
			result.X = a.X * f;
			result.Y = a.Y * f;
			result.Z = a.Z * f;
			result.W = a.W * f;
		}

		#endregion

		#region Div

		/// <summary>
		/// Divide a vector by a scalar
		/// </summary>
		/// <param name="a">Vector operand</param>
		/// <param name="f">Scalar operand</param>
		/// <returns>Result of the division</returns>
		public static SCNVector4 Div (SCNVector4 a, pfloat f)
		{
			pfloat mult = 1.0f / f;
			a.X *= mult;
			a.Y *= mult;
			a.Z *= mult;
			a.W *= mult;
			return a;
		}

		/// <summary>
		/// Divide a vector by a scalar
		/// </summary>
		/// <param name="a">Vector operand</param>
		/// <param name="f">Scalar operand</param>
		/// <param name="result">Result of the division</param>
		public static void Div (ref SCNVector4 a, pfloat f, out SCNVector4 result)
		{
			pfloat mult = 1.0f / f;
			result.X = a.X * mult;
			result.Y = a.Y * mult;
			result.Z = a.Z * mult;
			result.W = a.W * mult;
		}

		#endregion

		#endregion

		#region Add

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static SCNVector4 Add (SCNVector4 a, SCNVector4 b)
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
		public static void Add (ref SCNVector4 a, ref SCNVector4 b, out SCNVector4 result)
		{
			result = new SCNVector4 (a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

		#endregion

		#region Subtract

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static SCNVector4 Subtract (SCNVector4 a, SCNVector4 b)
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
		public static void Subtract (ref SCNVector4 a, ref SCNVector4 b, out SCNVector4 result)
		{
			result = new SCNVector4 (a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}

		#endregion

		#region Multiply

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector4 Multiply (SCNVector4 vector, pfloat scale)
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
		public static void Multiply (ref SCNVector4 vector, pfloat scale, out SCNVector4 result)
		{
			result = new SCNVector4 (vector.X * scale, vector.Y * scale, vector.Z * scale, vector.W * scale);
		}

		/// <summary>
		/// Multiplies a vector by the components a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector4 Multiply (SCNVector4 vector, SCNVector4 scale)
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
		public static void Multiply (ref SCNVector4 vector, ref SCNVector4 scale, out SCNVector4 result)
		{
			result = new SCNVector4 (vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z, vector.W * scale.W);
		}

		#endregion

		#region Divide

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector4 Divide (SCNVector4 vector, pfloat scale)
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
		public static void Divide (ref SCNVector4 vector, pfloat scale, out SCNVector4 result)
		{
			Multiply (ref vector, 1 / scale, out result);
		}

		/// <summary>
		/// Divides a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static SCNVector4 Divide (SCNVector4 vector, SCNVector4 scale)
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
		public static void Divide (ref SCNVector4 vector, ref SCNVector4 scale, out SCNVector4 result)
		{
			result = new SCNVector4 (vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z, vector.W / scale.W);
		}

		#endregion

		#region Min

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static SCNVector4 Min (SCNVector4 a, SCNVector4 b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			a.Z = a.Z < b.Z ? a.Z : b.Z;
			a.W = a.W < b.W ? a.W : b.W;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void Min (ref SCNVector4 a, ref SCNVector4 b, out SCNVector4 result)
		{
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
			result.Z = a.Z < b.Z ? a.Z : b.Z;
			result.W = a.W < b.W ? a.W : b.W;
		}

		#endregion

		#region Max

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static SCNVector4 Max (SCNVector4 a, SCNVector4 b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			a.Z = a.Z > b.Z ? a.Z : b.Z;
			a.W = a.W > b.W ? a.W : b.W;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void Max (ref SCNVector4 a, ref SCNVector4 b, out SCNVector4 result)
		{
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
			result.Z = a.Z > b.Z ? a.Z : b.Z;
			result.W = a.W > b.W ? a.W : b.W;
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
		public static SCNVector4 Clamp (SCNVector4 vec, SCNVector4 min, SCNVector4 max)
		{
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			vec.Z = vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			vec.W = vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp (ref SCNVector4 vec, ref SCNVector4 min, ref SCNVector4 max, out SCNVector4 result)
		{
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			result.Z = vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			result.W = vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W;
		}

		#endregion

		#region Normalize

		/// <summary>
		/// Scale a vector to unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <returns>The normalized vector</returns>
		public static SCNVector4 Normalize (SCNVector4 vec)
		{
			pfloat scale = 1.0f / vec.Length;
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			vec.W *= scale;
			return vec;
		}

		/// <summary>
		/// Scale a vector to unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <param name="result">The normalized vector</param>
		public static void Normalize (ref SCNVector4 vec, out SCNVector4 result)
		{
			pfloat scale = 1.0f / vec.Length;
			result.X = vec.X * scale;
			result.Y = vec.Y * scale;
			result.Z = vec.Z * scale;
			result.W = vec.W * scale;
		}

		#endregion

		#region NormalizeFast

		/// <summary>
		/// Scale a vector to approximately unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <returns>The normalized vector</returns>
		public static SCNVector4 NormalizeFast (SCNVector4 vec)
		{
			pfloat scale = (pfloat) (MathHelper.InverseSqrtFast (vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z + vec.W * vec.W));
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			vec.W *= scale;
			return vec;
		}

		/// <summary>
		/// Scale a vector to approximately unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <param name="result">The normalized vector</param>
		public static void NormalizeFast (ref SCNVector4 vec, out SCNVector4 result)
		{
			pfloat scale = (pfloat) (MathHelper.InverseSqrtFast (vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z + vec.W * vec.W));
			result.X = vec.X * scale;
			result.Y = vec.Y * scale;
			result.Z = vec.Z * scale;
			result.W = vec.W * scale;
		}

		#endregion

		#region Dot

		/// <summary>
		/// Calculate the dot product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static pfloat Dot (SCNVector4 left, SCNVector4 right)
		{
			return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
		}

		/// <summary>
		/// Calculate the dot product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot (ref SCNVector4 left, ref SCNVector4 right, out pfloat result)
		{
			result = left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
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
		public static SCNVector4 Lerp (SCNVector4 a, SCNVector4 b, pfloat blend)
		{
			a.X = blend * (b.X - a.X) + a.X;
			a.Y = blend * (b.Y - a.Y) + a.Y;
			a.Z = blend * (b.Z - a.Z) + a.Z;
			a.W = blend * (b.W - a.W) + a.W;
			return a;
		}

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
		public static void Lerp (ref SCNVector4 a, ref SCNVector4 b, pfloat blend, out SCNVector4 result)
		{
			result.X = blend * (b.X - a.X) + a.X;
			result.Y = blend * (b.Y - a.Y) + a.Y;
			result.Z = blend * (b.Z - a.Z) + a.Z;
			result.W = blend * (b.W - a.W) + a.W;
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
		public static SCNVector4 BaryCentric (SCNVector4 a, SCNVector4 b, SCNVector4 c, pfloat u, pfloat v)
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
		public static void BaryCentric (ref SCNVector4 a, ref SCNVector4 b, ref SCNVector4 c, pfloat u, pfloat v, out SCNVector4 result)
		{
			result = a; // copy

			SCNVector4 temp = b; // copy
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
		public static SCNVector4 Transform (SCNVector4 vec, SCNMatrix4 mat)
		{
			Transform (ref vec, ref mat, out var result);
			return result;
		}

#if NET
		/// <summary>Transform a Vector by the given Matrix.</summary>
        /// <param name="vec">The column vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
#else
		/// <summary>Transform a Vector by the given Matrix.</summary>
		/// <param name="vec">The row vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
#endif
		public static void Transform (ref SCNVector4 vec, ref SCNMatrix4 mat, out SCNVector4 result)
		{
#if NET
            result.X = vec.X * mat.Column0.X +
                       vec.Y * mat.Column1.X +
                       vec.Z * mat.Column2.X +
                       vec.W * mat.Column3.X;

            result.Y = vec.X * mat.Column0.Y +
                       vec.Y * mat.Column1.Y +
                       vec.Z * mat.Column2.Y +
                       vec.W * mat.Column3.Y;

            result.Z = vec.X * mat.Column0.Z +
                       vec.Y * mat.Column1.Z +
                       vec.Z * mat.Column2.Z +
                       vec.W * mat.Column3.Z;

            result.W = vec.X * mat.Column0.W +
                       vec.Y * mat.Column1.W +
                       vec.Z * mat.Column2.W +
                       vec.W * mat.Column3.W;
#else
			result.X = vec.X * mat.Row0.X +
					   vec.Y * mat.Row1.X +
					   vec.Z * mat.Row2.X +
					   vec.W * mat.Row3.X;

			result.Y = vec.X * mat.Row0.Y +
					   vec.Y * mat.Row1.Y +
					   vec.Z * mat.Row2.Y +
					   vec.W * mat.Row3.Y;

			result.Z = vec.X * mat.Row0.Z +
					   vec.Y * mat.Row1.Z +
					   vec.Z * mat.Row2.Z +
					   vec.W * mat.Row3.Z;

			result.W = vec.X * mat.Row0.W +
					   vec.Y * mat.Row1.W +
					   vec.Z * mat.Row2.W +
					   vec.W * mat.Row3.W;
#endif
		}

		#endregion

		#endregion

		#region Swizzle

		/// <summary>
		/// Gets or sets an OpenTK.Vector2 with the X and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2 Xy { get { return new Vector2 ((float) X, (float) Y); } set { X = value.X; Y = value.Y; } }

		/// <summary>
		/// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
		/// </summary>
		[XmlIgnore]
		public SCNVector3 Xyz { get { return new SCNVector3 (X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }

		#endregion

		#region Operators

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector4 operator + (SCNVector4 left, SCNVector4 right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			left.W += right.W;
			return left;
		}

		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector4 operator - (SCNVector4 left, SCNVector4 right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			left.W -= right.W;
			return left;
		}

		/// <summary>
		/// Negates an instance.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector4 operator - (SCNVector4 vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			vec.W = -vec.W;
			return vec;
		}

		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector4 operator * (SCNVector4 vec, pfloat scale)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			vec.W *= scale;
			return vec;
		}

		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="scale">The scalar.</param>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector4 operator * (pfloat scale, SCNVector4 vec)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			vec.W *= scale;
			return vec;
		}

		/// <summary>
		/// Divides an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static SCNVector4 operator / (SCNVector4 vec, pfloat scale)
		{
			pfloat mult = 1.0f / scale;
			vec.X *= mult;
			vec.Y *= mult;
			vec.Z *= mult;
			vec.W *= mult;
			return vec;
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator == (SCNVector4 left, SCNVector4 right)
		{
			return left.Equals (right);
		}

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equa lright; false otherwise.</returns>
		public static bool operator != (SCNVector4 left, SCNVector4 right)
		{
			return !left.Equals (right);
		}

		/// <summary>
		/// Returns a pointer to the first element of the specified instance.
		/// </summary>
		/// <param name="v">The instance.</param>
		/// <returns>A pointer to the first element of v.</returns>
		[CLSCompliant (false)]
		unsafe public static explicit operator pfloat* (SCNVector4 v)
		{
			return &v.X;
		}

		/// <summary>
		/// Returns a pointer to the first element of the specified instance.
		/// </summary>
		/// <param name="v">The instance.</param>
		/// <returns>A pointer to the first element of v.</returns>
		public static explicit operator IntPtr (SCNVector4 v)
		{
			unsafe {
				return (IntPtr) (&v.X);
			}
		}

		#endregion

		#region Overrides

		#region public override string ToString()

		/// <summary>
		/// Returns a System.String that represents the current SCNVector4.
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3})", X, Y, Z, W);
		}

		#endregion

		#region public override int GetHashCode()

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode ()
		{
			return HashCode.Combine (X, Y, Z, W);
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
			if (!(obj is SCNVector4))
				return false;

			return this.Equals ((SCNVector4) obj);
		}

		#endregion

		#endregion

		#endregion

		#region IEquatable<SCNVector4> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals (SCNVector4 other)
		{
			return
				X == other.X &&
				Y == other.Y &&
				Z == other.Z &&
				W == other.W;
		}

		#endregion

		public static implicit operator SCNVector4 (Vector4 vector)
		{
			return new SCNVector4 (vector.X, vector.Y, vector.Z, vector.W);
		}

		public static explicit operator Vector4 (SCNVector4 source)
		{
			return new Vector4 ((float) source.X, (float) source.Y, (float) source.Z, (float) source.W);
		}
	}
}
