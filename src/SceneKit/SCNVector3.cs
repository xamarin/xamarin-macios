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

#if !MONOMAC
#define PFLOAT_SINGLE
#endif

using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using MathHelper = global::OpenTK.MathHelper;
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
       
namespace SceneKit
{
    /// <summary>
    /// Represents a 3D vector using three single-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The Vector3 structure is suitable for interoperation with unmanaged code requiring three consecutive floats.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SCNVector3 : IEquatable<SCNVector3>
    {
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
        public SCNVector3(pfloat x, pfloat y, pfloat z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs a new Vector3.
        /// </summary>
        /// <param name="x">The x component of the Vector3.</param>
        /// <param name="y">The y component of the Vector3.</param>
        /// <param name="z">The z component of the Vector3.</param>
        public SCNVector3(double x, double y, double z)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            X = new NFloat (x);
            Y = new NFloat (y);
            Z = new NFloat (z);
#else
            X = (pfloat) x;
            Y = (pfloat) y;
            Z = (pfloat) z;
#endif
        }

        public SCNVector3(Vector3 v)
	{
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            X = new NFloat (v.X);
            Y = new NFloat (v.Y);
            Z = new NFloat (v.Z);
#else
            X = v.X;
            Y = v.Y;
            Z = v.Z;
#endif
	}
	
        /// <summary>
        /// Constructs a new Vector3 from the given Vector3.
        /// </summary>
        /// <param name="v">The Vector3 to copy components from.</param>
        public SCNVector3(SCNVector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Constructs a new Vector3 from the given Vector4.
        /// </summary>
        /// <param name="v">The Vector4 to copy components from.</param>
        public SCNVector3(SCNVector4 v)
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
        public pfloat Length
        {
            get
            {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
                return new NFloat (System.Math.Sqrt(X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value));
#else
                return (pfloat)System.Math.Sqrt(X * X + Y * Y + Z * Z);
#endif
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
        public pfloat LengthFast
        {
            get
            {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
		    return new NFloat (1.0f / MathHelper.InverseSqrtFast(X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value));
#else
		    return (pfloat)(1.0f / MathHelper.InverseSqrtFast(X * X + Y * Y + Z * Z));
#endif
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
        public pfloat LengthSquared
        {
            get
            {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
                return new NFloat (X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value);
#else
                return X * X + Y * Y + Z * Z;
#endif
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the SCNVector3 to unit length.
        /// </summary>
        public void Normalize()
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            pfloat scale = new NFloat (1.0f / this.Length.Value);
            X = new NFloat (X.Value * scale.Value);
            Y = new NFloat (Y.Value * scale.Value);
            Z = new NFloat (Z.Value * scale.Value);
#else
            pfloat scale = 1.0f / this.Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
#endif
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the SCNVector3 to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
	    pfloat scale = new NFloat (MathHelper.InverseSqrtFast(X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value));
            X = new NFloat (X.Value * scale.Value);
            Y = new NFloat (Y.Value * scale.Value);
            Z = new NFloat (Z.Value * scale.Value);
#else
	    pfloat scale = (pfloat)MathHelper.InverseSqrtFast(X * X + Y * Y + Z * Z);
            X *= scale;
            Y *= scale;
            Z *= scale;
#endif
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length SCNVector3 that points towards the X-axis.
        /// </summary>
        public static readonly SCNVector3 UnitX = new SCNVector3(1, 0, 0);

        /// <summary>
        /// Defines a unit-length SCNVector3 that points towards the Y-axis.
        /// </summary>
        public static readonly SCNVector3 UnitY = new SCNVector3(0, 1, 0);

        /// <summary>
        /// /// Defines a unit-length SCNVector3 that points towards the Z-axis.
        /// </summary>
        public static readonly SCNVector3 UnitZ = new SCNVector3(0, 0, 1);

        /// <summary>
        /// Defines a zero-length SCNVector3.
        /// </summary>
        public static readonly SCNVector3 Zero = new SCNVector3(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly SCNVector3 One = new SCNVector3(1, 1, 1);

        /// <summary>
        /// Defines the size of the SCNVector3 struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new SCNVector3());

        #endregion

        #region Add

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        public static SCNVector3 Add(SCNVector3 a, SCNVector3 b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <param name="result">Result of operation.</param>
        public static void Add(ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new SCNVector3(a.X.Value + b.X.Value, a.Y.Value + b.Y.Value, a.Z.Value + b.Z.Value);
#else
            result = new SCNVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
#endif
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        public static SCNVector3 Subtract(SCNVector3 a, SCNVector3 b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        public static void Subtract(ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new SCNVector3(a.X.Value - b.X.Value, a.Y.Value - b.Y.Value, a.Z.Value - b.Z.Value);
#else
            result = new SCNVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
#endif
        }

        #endregion

        #region Multiply

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static SCNVector3 Multiply(SCNVector3 vector, pfloat scale)
        {
            Multiply(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref SCNVector3 vector, pfloat scale, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new SCNVector3(vector.X.Value * scale.Value, vector.Y.Value * scale.Value, vector.Z.Value * scale.Value);
#else
            result = new SCNVector3(vector.X * scale, vector.Y * scale, vector.Z * scale);
#endif
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static SCNVector3 Multiply(SCNVector3 vector, SCNVector3 scale)
        {
            Multiply(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref SCNVector3 vector, ref SCNVector3 scale, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new SCNVector3(vector.X.Value * scale.X.Value, vector.Y.Value * scale.Y.Value, vector.Z.Value * scale.Z.Value);
#else
            result = new SCNVector3(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z);
#endif
        }

        #endregion

        #region Divide

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static SCNVector3 Divide(SCNVector3 vector, pfloat scale)
        {
            Divide(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref SCNVector3 vector, pfloat scale, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            Multiply(ref vector, new NFloat (1 / scale.Value), out result);
#else
            Multiply(ref vector, 1 / scale, out result);
#endif
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static SCNVector3 Divide(SCNVector3 vector, SCNVector3 scale)
        {
            Divide(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divide a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref SCNVector3 vector, ref SCNVector3 scale, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new SCNVector3(vector.X.Value / scale.X.Value, vector.Y.Value / scale.Y.Value, vector.Z.Value / scale.Z.Value);
#else
            result = new SCNVector3(vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z);
#endif
        }

        #endregion

        #region ComponentMin

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>The component-wise minimum</returns>
        public static SCNVector3 ComponentMin(SCNVector3 a, SCNVector3 b)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            a.X = a.X.Value < b.X.Value ? a.X : b.X;
            a.Y = a.Y.Value < b.Y.Value ? a.Y : b.Y;
            a.Z = a.Z.Value < b.Z.Value ? a.Z : b.Z;
#else
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
#endif
            return a;
        }

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise minimum</param>
        public static void ComponentMin(ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = a.X.Value < b.X.Value ? a.X : b.X;
            result.Y = a.Y.Value < b.Y.Value ? a.Y : b.Y;
            result.Z = a.Z.Value < b.Z.Value ? a.Z : b.Z;
#else
            result.X = a.X < b.X ? a.X : b.X;
            result.Y = a.Y < b.Y ? a.Y : b.Y;
            result.Z = a.Z < b.Z ? a.Z : b.Z;
#endif
        }

        #endregion

        #region ComponentMax

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>The component-wise maximum</returns>
        public static SCNVector3 ComponentMax(SCNVector3 a, SCNVector3 b)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            a.X = a.X.Value > b.X.Value ? a.X : b.X;
            a.Y = a.Y.Value > b.Y.Value ? a.Y : b.Y;
            a.Z = a.Z.Value > b.Z.Value ? a.Z : b.Z;
#else
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
#endif
            return a;
        }

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise maximum</param>
        public static void ComponentMax(ref SCNVector3 a, ref SCNVector3 b, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = a.X.Value > b.X.Value ? a.X : b.X;
            result.Y = a.Y.Value > b.Y.Value ? a.Y : b.Y;
            result.Z = a.Z.Value > b.Z.Value ? a.Z : b.Z;
#else
            result.X = a.X > b.X ? a.X : b.X;
            result.Y = a.Y > b.Y ? a.Y : b.Y;
            result.Z = a.Z > b.Z ? a.Z : b.Z;
#endif
        }

        #endregion

        #region Min

        /// <summary>
        /// Returns the SCNVector3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>The minimum SCNVector3</returns>
        public static SCNVector3 Min(SCNVector3 left, SCNVector3 right)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            return left.LengthSquared.Value < right.LengthSquared.Value ? left : right;
#else
            return left.LengthSquared < right.LengthSquared ? left : right;
#endif
        }

        #endregion

        #region Max

        /// <summary>
        /// Returns the SCNVector3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>The minimum SCNVector3</returns>
        public static SCNVector3 Max(SCNVector3 left, SCNVector3 right)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            return left.LengthSquared.Value >= right.LengthSquared.Value ? left : right;
#else
            return left.LengthSquared >= right.LengthSquared ? left : right;
#endif
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
        public static SCNVector3 Clamp(SCNVector3 vec, SCNVector3 min, SCNVector3 max)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            vec.X = vec.X.Value < min.X.Value ? min.X : vec.X.Value > max.X.Value ? max.X : vec.X;
            vec.Y = vec.Y.Value < min.Y.Value ? min.Y : vec.Y.Value > max.Y.Value ? max.Y : vec.Y;
            vec.Z = vec.Z.Value < min.Z.Value ? min.Z : vec.Z.Value > max.Z.Value ? max.Z : vec.Z;
#else
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            vec.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
#endif
            return vec;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(ref SCNVector3 vec, ref SCNVector3 min, ref SCNVector3 max, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = vec.X.Value < min.X.Value ? min.X : vec.X.Value > max.X.Value ? max.X : vec.X;
            result.Y = vec.Y.Value < min.Y.Value ? min.Y : vec.Y.Value > max.Y.Value ? max.Y : vec.Y;
            result.Z = vec.Z.Value < min.Z.Value ? min.Z : vec.Z.Value > max.Z.Value ? max.Z : vec.Z;
#else
            result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            result.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
#endif
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static SCNVector3 Normalize(SCNVector3 vec)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            pfloat scale = new NFloat (1.0f / vec.Length.Value);
            vec.X = new NFloat (vec.X.Value * scale.Value);
            vec.Y = new NFloat (vec.Y.Value * scale.Value);
            vec.Z = new NFloat (vec.Z.Value * scale.Value);
#else
            pfloat scale = 1.0f / vec.Length;
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
#endif
            return vec;
        }

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(ref SCNVector3 vec, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            pfloat scale = new NFloat (1.0f / vec.Length.Value);
            result.X = new NFloat (vec.X.Value * scale.Value);
            result.Y = new NFloat (vec.Y.Value * scale.Value);
            result.Z = new NFloat (vec.Z.Value * scale.Value);
#else
            pfloat scale = 1.0f / vec.Length;
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
            result.Z = vec.Z * scale;
#endif
        }

        #endregion

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static SCNVector3 NormalizeFast(SCNVector3 vec)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            pfloat scale = new NFloat (MathHelper.InverseSqrtFast(vec.X.Value * vec.X.Value + vec.Y.Value * vec.Y.Value + vec.Z.Value * vec.Z.Value));
            vec.X = new NFloat (vec.X.Value * scale.Value);
            vec.Y = new NFloat (vec.Y.Value * scale.Value);
            vec.Z = new NFloat (vec.Z.Value * scale.Value);
#else
            pfloat scale = (pfloat)MathHelper.InverseSqrtFast(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
#endif
            return vec;
        }

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void NormalizeFast(ref SCNVector3 vec, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            pfloat scale = new NFloat (MathHelper.InverseSqrtFast(vec.X.Value * vec.X.Value + vec.Y.Value * vec.Y.Value + vec.Z.Value * vec.Z.Value));
            result.X = new NFloat (vec.X.Value * scale.Value);
            result.Y = new NFloat (vec.Y.Value * scale.Value);
            result.Z = new NFloat (vec.Z.Value * scale.Value);
#else
            pfloat scale = (pfloat)MathHelper.InverseSqrtFast(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
            result.Z = vec.Z * scale;
#endif
        }

        #endregion

        #region Dot

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The dot product of the two inputs</returns>
        public static pfloat Dot(SCNVector3 left, SCNVector3 right)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            return new NFloat (left.X.Value * right.X.Value + left.Y.Value * right.Y.Value + left.Z.Value * right.Z.Value);
#else
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
#endif
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The dot product of the two inputs</param>
        public static void Dot(ref SCNVector3 left, ref SCNVector3 right, out pfloat result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new NFloat (left.X.Value * right.X.Value + left.Y.Value * right.Y.Value + left.Z.Value * right.Z.Value);
#else
            result = left.X * right.X + left.Y * right.Y + left.Z * right.Z;
#endif
        }

        #endregion

        #region Cross

        /// <summary>
        /// Caclulate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The cross product of the two inputs</returns>
        public static SCNVector3 Cross(SCNVector3 left, SCNVector3 right)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            return new SCNVector3(left.Y.Value * right.Z.Value - left.Z.Value * right.Y.Value,
                               left.Z.Value * right.X.Value - left.X.Value * right.Z.Value,
                               left.X.Value * right.Y.Value - left.Y.Value * right.X.Value);
#else
            return new SCNVector3(left.Y * right.Z - left.Z * right.Y,
                               left.Z * right.X - left.X * right.Z,
                               left.X * right.Y - left.Y * right.X);
#endif
        }

        /// <summary>
        /// Caclulate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The cross product of the two inputs</returns>
        /// <param name="result">The cross product of the two inputs</param>
        public static void Cross(ref SCNVector3 left, ref SCNVector3 right, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = new NFloat (left.Y.Value * right.Z.Value - left.Z.Value * right.Y.Value);
            result.Y = new NFloat (left.Z.Value * right.X.Value - left.X.Value * right.Z.Value);
            result.Z = new NFloat (left.X.Value * right.Y.Value - left.Y.Value * right.X.Value);
#else
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
#endif
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
        public static SCNVector3 Lerp(SCNVector3 a, SCNVector3 b, pfloat blend)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            a.X = new NFloat (blend.Value * (b.X.Value - a.X.Value) + a.X.Value);
            a.Y = new NFloat (blend.Value * (b.Y.Value - a.Y.Value) + a.Y.Value);
            a.Z = new NFloat (blend.Value * (b.Z.Value - a.Z.Value) + a.Z.Value);
#else
            a.X = blend * (b.X - a.X) + a.X;
            a.Y = blend * (b.Y - a.Y) + a.Y;
            a.Z = blend * (b.Z - a.Z) + a.Z;
#endif
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref SCNVector3 a, ref SCNVector3 b, pfloat blend, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = new NFloat (blend.Value * (b.X.Value - a.X.Value) + a.X.Value);
            result.Y = new NFloat (blend.Value * (b.Y.Value - a.Y.Value) + a.Y.Value);
            result.Z = new NFloat (blend.Value * (b.Z.Value - a.Z.Value) + a.Z.Value);
#else
            result.X = blend * (b.X - a.X) + a.X;
            result.Y = blend * (b.Y - a.Y) + a.Y;
            result.Z = blend * (b.Z - a.Z) + a.Z;
#endif
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
        public static SCNVector3 BaryCentric(SCNVector3 a, SCNVector3 b, SCNVector3 c, pfloat u, pfloat v)
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
        public static void BaryCentric(ref SCNVector3 a, ref SCNVector3 b, ref SCNVector3 c, pfloat u, pfloat v, out SCNVector3 result)
        {
            result = a; // copy

            SCNVector3 temp = b; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, u, out temp);
            Add(ref result, ref temp, out result);

            temp = c; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, v, out temp);
            Add(ref result, ref temp, out result);
        }

        #endregion

        #region Transform

        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static SCNVector3 TransformVector(SCNVector3 vec, SCNMatrix4 mat)
        {
            SCNVector3 v;
            v.X = SCNVector3.Dot(vec, new SCNVector3(mat.Column0));
            v.Y = SCNVector3.Dot(vec, new SCNVector3(mat.Column1));
            v.Z = SCNVector3.Dot(vec, new SCNVector3(mat.Column2));
            return v;
        }

        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void TransformVector(ref SCNVector3 vec, ref SCNMatrix4 mat, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = new NFloat (vec.X.Value * mat.Row0.X.Value +
                       vec.Y.Value * mat.Row1.X.Value +
                       vec.Z.Value * mat.Row2.X.Value);

            result.Y = new NFloat (vec.X.Value * mat.Row0.Y.Value +
                       vec.Y.Value * mat.Row1.Y.Value +
                       vec.Z.Value * mat.Row2.Y.Value);

            result.Z = new NFloat (vec.X.Value * mat.Row0.Z.Value +
                       vec.Y.Value * mat.Row1.Z.Value +
                       vec.Z.Value * mat.Row2.Z.Value);
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

        /// <summary>Transform a Normal by the given Matrix</summary>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static SCNVector3 TransformNormal(SCNVector3 norm, SCNMatrix4 mat)
        {
            mat.Invert();
            return TransformNormalInverse(norm, mat);
        }

        /// <summary>Transform a Normal by the given Matrix</summary>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed normal</param>
        public static void TransformNormal(ref SCNVector3 norm, ref SCNMatrix4 mat, out SCNVector3 result)
        {
            SCNMatrix4 Inverse = SCNMatrix4.Invert(mat);
            SCNVector3.TransformNormalInverse(ref norm, ref Inverse, out result);
        }

        /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static SCNVector3 TransformNormalInverse(SCNVector3 norm, SCNMatrix4 invMat)
        {
            SCNVector3 n;
            n.X = SCNVector3.Dot(norm, new SCNVector3(invMat.Row0));
            n.Y = SCNVector3.Dot(norm, new SCNVector3(invMat.Row1));
            n.Z = SCNVector3.Dot(norm, new SCNVector3(invMat.Row2));
            return n;
        }

        /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <param name="result">The transformed normal</param>
        public static void TransformNormalInverse(ref SCNVector3 norm, ref SCNMatrix4 invMat, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = new NFloat (norm.X.Value * invMat.Row0.X.Value +
                       norm.Y.Value * invMat.Row0.Y.Value +
                       norm.Z.Value * invMat.Row0.Z.Value);

            result.Y = new NFloat (norm.X.Value * invMat.Row1.X.Value +
                       norm.Y.Value * invMat.Row1.Y.Value +
                       norm.Z.Value * invMat.Row1.Z.Value);

            result.Z = new NFloat (norm.X.Value * invMat.Row2.X.Value +
                       norm.Y.Value * invMat.Row2.Y.Value +
                       norm.Z.Value * invMat.Row2.Z.Value);
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

        /// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed position</returns>
        public static SCNVector3 TransformPosition(SCNVector3 pos, SCNMatrix4 mat)
        {
            SCNVector3 p;
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            p.X = new NFloat (SCNVector3.Dot(pos, new SCNVector3(mat.Column0)).Value + mat.Row3.X.Value);
            p.Y = new NFloat (SCNVector3.Dot(pos, new SCNVector3(mat.Column1)).Value + mat.Row3.Y.Value);
            p.Z = new NFloat (SCNVector3.Dot(pos, new SCNVector3(mat.Column2)).Value + mat.Row3.Z.Value);
#else
            p.X = SCNVector3.Dot(pos, new SCNVector3(mat.Column0)) + mat.Row3.X;
            p.Y = SCNVector3.Dot(pos, new SCNVector3(mat.Column1)) + mat.Row3.Y;
            p.Z = SCNVector3.Dot(pos, new SCNVector3(mat.Column2)) + mat.Row3.Z;
#endif
            return p;
        }

        /// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed position</param>
        public static void TransformPosition(ref SCNVector3 pos, ref SCNMatrix4 mat, out SCNVector3 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = new NFloat (pos.X.Value * mat.Row0.X.Value +
                       pos.Y.Value * mat.Row1.X.Value +
                       pos.Z.Value * mat.Row2.X.Value +
                       mat.Row3.X.Value);

            result.Y = new NFloat (pos.X.Value * mat.Row0.Y.Value +
                       pos.Y.Value * mat.Row1.Y.Value +
                       pos.Z.Value * mat.Row2.Y.Value +
                       mat.Row3.Y.Value);

            result.Z = new NFloat (pos.X.Value * mat.Row0.Z.Value +
                       pos.Y.Value * mat.Row1.Z.Value +
                       pos.Z.Value * mat.Row2.Z.Value +
                       mat.Row3.Z.Value);
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

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static SCNVector4 Transform(SCNVector3 vec, SCNMatrix4 mat)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            SCNVector4 v4 = new SCNVector4(vec.X, vec.Y, vec.Z, new NFloat (1.0f));
#else
            SCNVector4 v4 = new SCNVector4(vec.X, vec.Y, vec.Z, 1.0f);
#endif
            SCNVector4 result;
            result.X = SCNVector4.Dot(v4, mat.Column0);
            result.Y = SCNVector4.Dot(v4, mat.Column1);
            result.Z = SCNVector4.Dot(v4, mat.Column2);
            result.W = SCNVector4.Dot(v4, mat.Column3);
            return result;
        }

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void Transform(ref SCNVector3 vec, ref SCNMatrix4 mat, out SCNVector4 result)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            SCNVector4 v4 = new SCNVector4(vec.X, vec.Y, vec.Z, new NFloat (1.0f));
            SCNVector4.Transform(ref v4, ref mat, out result);
#else
            SCNVector4 v4 = new SCNVector4(vec.X, vec.Y, vec.Z, 1.0f);
            SCNVector4.Transform(ref v4, ref mat, out result);
#endif
        }

        /// <summary>Transform a SCNVector3 by the given Matrix, and project the resulting Vector4 back to a SCNVector3</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static SCNVector3 TransformPerspective(SCNVector3 vec, SCNMatrix4 mat)
        {
            SCNVector4 h = Transform(vec, mat);
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            return new SCNVector3(h.X.Value / h.W.Value, h.Y.Value / h.W.Value, h.Z.Value / h.W.Value);
#else
            return new SCNVector3(h.X / h.W, h.Y / h.W, h.Z / h.W);
#endif
        }

        /// <summary>Transform a SCNVector3 by the given Matrix, and project the resulting SCNVector4 back to a SCNVector3</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void TransformPerspective(ref SCNVector3 vec, ref SCNMatrix4 mat, out SCNVector3 result)
        {
            SCNVector4 h;
            SCNVector3.Transform(ref vec, ref mat, out h);
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result.X = new NFloat (h.X.Value / h.W.Value);
            result.Y = new NFloat (h.Y.Value / h.W.Value);
            result.Z = new NFloat (h.Z.Value / h.W.Value);
#else
            result.X = h.X / h.W;
            result.Y = h.Y / h.W;
            result.Z = h.Z / h.W;
#endif
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
        public static pfloat CalculateAngle(SCNVector3 first, SCNVector3 second)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            return new NFloat (System.Math.Acos((SCNVector3.Dot(first, second).Value) / (first.Length.Value * second.Length.Value)));
#else
            return (pfloat)System.Math.Acos((SCNVector3.Dot(first, second)) / (first.Length * second.Length));
#endif
        }

        /// <summary>Calculates the angle (in radians) between two vectors.</summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <param name="result">Angle (in radians) between the vectors.</param>
        /// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
        public static void CalculateAngle(ref SCNVector3 first, ref SCNVector3 second, out pfloat result)
        {
            pfloat temp;
            SCNVector3.Dot(ref first, ref second, out temp);
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            result = new NFloat (System.Math.Acos(temp.Value / (first.Length.Value * second.Length.Value)));
#else
            result = (pfloat)System.Math.Acos(temp / (first.Length * second.Length));
#endif
        }

        #endregion

        #endregion

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the X and Y components of this instance.
        /// </summary>
        [XmlIgnore]
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
	public Vector2 Xy { get { return new Vector2((float)X.Value, (float)Y.Value); } set { X = new NFloat (value.X); Y = new NFloat (value.Y); } }
#else
	public Vector2 Xy { get { return new Vector2((float)X, (float)Y); } set { X = value.X; Y = value.Y; } }
#endif

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNVector3 operator +(SCNVector3 left, SCNVector3 right)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            left.X = new pfloat (left.X.Value + right.X.Value);
            left.Y = new pfloat (left.Y.Value + right.Y.Value);
            left.Z = new pfloat (left.Z.Value + right.Z.Value);
#else
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
#endif
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNVector3 operator -(SCNVector3 left, SCNVector3 right)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
        	left.X = new pfloat (left.X.Value - right.X.Value);
			left.Y = new pfloat (left.Y.Value - right.Y.Value);
			left.Z = new pfloat (left.Z.Value - right.Z.Value);
#else
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
#endif
            return left;
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNVector3 operator -(SCNVector3 vec)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            vec.X = new pfloat (-vec.X.Value);
            vec.Y = new pfloat (-vec.Y.Value);
            vec.Z = new pfloat (-vec.Z.Value);
#else
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
#endif
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNVector3 operator *(SCNVector3 vec, pfloat scale)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            vec.X = new pfloat (vec.X.Value * scale.Value);
            vec.Y = new pfloat (vec.Y.Value * scale.Value);
            vec.Z = new pfloat (vec.Z.Value * scale.Value);
#else
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
#endif
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNVector3 operator *(pfloat scale, SCNVector3 vec)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            vec.X = new pfloat (vec.X.Value * scale.Value);
            vec.Y = new pfloat (vec.Y.Value * scale.Value);
            vec.Z = new pfloat (vec.Z.Value * scale.Value);
#else
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
#endif
            return vec;
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNVector3 operator /(SCNVector3 vec, pfloat scale)
        {
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
            pfloat mult = new pfloat (1.0f / scale.Value);
            vec.X = new pfloat (vec.X.Value * mult.Value);
            vec.Y = new pfloat (vec.X.Value * mult.Value);
            vec.Z = new pfloat (vec.X.Value * mult.Value);
#else
            pfloat mult = 1.0f / scale;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
#endif
            return vec;
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(SCNVector3 left, SCNVector3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equa lright; false otherwise.</returns>
        public static bool operator !=(SCNVector3 left, SCNVector3 right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current SCNVector3.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
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

            return this.Equals((SCNVector3)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<SCNVector3> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(SCNVector3 other)
        {
            return
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
                X.Value == other.X.Value &&
                Y.Value == other.Y.Value &&
                Z.Value == other.Z.Value;
#else
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
#endif
        }

        #endregion
	public static implicit operator SCNVector3 (Vector3 vector)
	{
		return new SCNVector3 (vector.X, vector.Y, vector.Z);
	}

	public static explicit operator Vector3 (SCNVector3 source)
	{
#if NO_NFLOAT_OPERATORS && !PFLOAT_SINGLE
		return new Vector3 ((float)source.X.Value, (float)source.Y.Value, (float)source.Z.Value);
#else
		return new Vector3 ((float)source.X, (float)source.Y, (float)source.Z);
#endif
	}
    }
}
