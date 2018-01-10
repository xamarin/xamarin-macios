#region --- License ---
/*
  Copyright 2014 Xamarin Inc, All rights reserved
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

using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Matrix3 = global::OpenTK.Matrix3;
using Vector4 = global::OpenTK.Vector4;
using Quaternion = global::OpenTK.Quaternion;
using MathHelper = global::OpenTK.MathHelper;

#if MONOMAC
using pfloat = System.nfloat;
#else
using pfloat = System.Single;
#endif

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SceneKit
{
    /// <summary>
    /// Represents a Quaternion.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SCNQuaternion : IEquatable<SCNQuaternion>
    {
        #region Fields

        SCNVector3 xyz;
        pfloat w;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new SCNQuaternion from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public SCNQuaternion(SCNVector3 v, pfloat w)
        {
            this.xyz = v;
            this.w = w;
        }

        /// <summary>
        /// Construct a new SCNQuaternion
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        /// <param name="z">The z component</param>
        /// <param name="w">The w component</param>
        public SCNQuaternion(pfloat x, pfloat y, pfloat z, pfloat w)
            : this(new SCNVector3(x, y, z), w)
        { }

        public SCNQuaternion (ref Matrix3 matrix)
        {
            double scale = System.Math.Pow(matrix.Determinant, 1.0d / 3.0d);
	    float x, y, z;
	    
            w = (float) (System.Math.Sqrt(System.Math.Max(0, scale + matrix[0, 0] + matrix[1, 1] + matrix[2, 2])) / 2);
            x = (float) (System.Math.Sqrt(System.Math.Max(0, scale + matrix[0, 0] - matrix[1, 1] - matrix[2, 2])) / 2);
            y = (float) (System.Math.Sqrt(System.Math.Max(0, scale - matrix[0, 0] + matrix[1, 1] - matrix[2, 2])) / 2);
            z = (float) (System.Math.Sqrt(System.Math.Max(0, scale - matrix[0, 0] - matrix[1, 1] + matrix[2, 2])) / 2);

	    xyz = new Vector3 (x, y, z);
	    
            if (matrix[2, 1] - matrix[1, 2] < 0) X = -X;
            if (matrix[0, 2] - matrix[2, 0] < 0) Y = -Y;
            if (matrix[1, 0] - matrix[0, 1] < 0) Z = -Z;
        }

	public SCNQuaternion (Quaternion openTkQuaternion) : this (new SCNVector3 (openTkQuaternion.XYZ), openTkQuaternion.W)
	{
		
	}
	
        #endregion

        #region Public Members

        #region Properties

#if !XAMCORE_2_0
        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
        /// </summary>
        [Obsolete("Use Xyz property instead.")]
        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public SCNVector3 XYZ { get { return Xyz; } set { Xyz = value; } }
#endif

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
        /// </summary>
        public SCNVector3 Xyz { get { return xyz; } set { xyz = value; } }

        /// <summary>
        /// Gets or sets the X component of this instance.
        /// </summary>
        [XmlIgnore]
        public pfloat X { get { return xyz.X; } set { xyz.X = value; } }

        /// <summary>
        /// Gets or sets the Y component of this instance.
        /// </summary>
        [XmlIgnore]
        public pfloat Y { get { return xyz.Y; } set { xyz.Y = value; } }

        /// <summary>
        /// Gets or sets the Z component of this instance.
        /// </summary>
        [XmlIgnore]
        public pfloat Z { get { return xyz.Z; } set { xyz.Z = value; } }

        /// <summary>
        /// Gets or sets the W component of this instance.
        /// </summary>
        public pfloat W { get { return w; } set { w = value; } }

        #endregion

        #region Instance

        #region ToAxisAngle

        /// <summary>
        /// Convert the current quaternion to axis angle representation
        /// </summary>
        /// <param name="axis">The resultant axis</param>
        /// <param name="angle">The resultant angle</param>
        public void ToAxisAngle(out SCNVector3 axis, out pfloat angle)
        {
            SCNVector4 result = ToAxisAngle();
            axis = result.Xyz;
            angle = result.W;
        }

        /// <summary>
        /// Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A Vector4 that is the axis-angle representation of this quaternion.</returns>
        public SCNVector4 ToAxisAngle()
        {
            SCNQuaternion q = this;
            if (q.W > 1.0f)
                q.Normalize();

            SCNVector4 result = new SCNVector4();

            result.W = 2.0f * (pfloat)System.Math.Acos(q.W); // angle
            pfloat den = (pfloat)System.Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.0001f)
            {
                result.Xyz = q.Xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                result.Xyz = SCNVector3.UnitX;
            }

            return result;
        }

        #endregion

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(W * W + Xyz.LengthSquared);
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        public float LengthSquared
        {
            get
            {
                return (float)(W * W + Xyz.LengthSquared);
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the Quaternion to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            Xyz *= scale;
            W *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        /// Convert this quaternion to its conjugate
        /// </summary>
        public void Conjugate()
        {
            Xyz = -Xyz;
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines the identity quaternion.
        /// </summary>
#if XAMCORE_2_0
		readonly
#endif
        public static SCNQuaternion Identity = new SCNQuaternion(0, 0, 0, 1);

        #endregion

        #region Add

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static SCNQuaternion Add(SCNQuaternion left, SCNQuaternion right)
        {
            return new SCNQuaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref SCNQuaternion left, ref SCNQuaternion right, out SCNQuaternion result)
        {
            result = new SCNQuaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        #endregion

        #region Sub

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static SCNQuaternion Sub(SCNQuaternion left, SCNQuaternion right)
        {
            return  new SCNQuaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref SCNQuaternion left, ref SCNQuaternion right, out SCNQuaternion result)
        {
            result = new SCNQuaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        #endregion

        #region Mult

#if !XAMCORE_2_0
        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        [Obsolete("Use Multiply instead.")]
        public static SCNQuaternion Mult(SCNQuaternion left, SCNQuaternion right)
        {
            return new SCNQuaternion(
                right.W * left.Xyz + left.W * right.Xyz + SCNVector3.Cross(left.Xyz, right.Xyz),
                left.W * right.W - SCNVector3.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        [Obsolete("Use Multiply instead.")]
        public static void Mult(ref SCNQuaternion left, ref SCNQuaternion right, out SCNQuaternion result)
        {
            result = new SCNQuaternion(
                right.W * left.Xyz + left.W * right.Xyz + SCNVector3.Cross(left.Xyz, right.Xyz),
                left.W * right.W - SCNVector3.Dot(left.Xyz, right.Xyz));
        }
#endif

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static SCNQuaternion Multiply(SCNQuaternion left, SCNQuaternion right)
        {
            SCNQuaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref SCNQuaternion left, ref SCNQuaternion right, out SCNQuaternion result)
        {
            result = new SCNQuaternion(
                right.W * left.Xyz + left.W * right.Xyz + SCNVector3.Cross(left.Xyz, right.Xyz),
                left.W * right.W - SCNVector3.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref SCNQuaternion quaternion, float scale, out SCNQuaternion result)
        {
            result = new SCNQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

#if !XAMCORE_2_0
	[Obsolete ("Use the overload without the 'ref float scale'.")]
        public static void Multiply(ref SCNQuaternion quaternion, ref float scale, out SCNQuaternion result)
	{
            result = new SCNQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);		
	}
#endif
	
        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static SCNQuaternion Multiply(SCNQuaternion quaternion, float scale)
        {
            return new SCNQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        #endregion

        #region Conjugate

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static SCNQuaternion Conjugate(SCNQuaternion q)
        {
            return new SCNQuaternion(-q.Xyz, q.W);
        }

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <param name="result">The conjugate of the given quaternion</param>
        public static void Conjugate(ref SCNQuaternion q, out SCNQuaternion result)
        {
            result = new SCNQuaternion(-q.Xyz, q.W);
        }

        #endregion

        #region Invert

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static SCNQuaternion Invert(SCNQuaternion q)
        {
            SCNQuaternion result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <param name="result">The inverse of the given quaternion</param>
        public static void Invert(ref SCNQuaternion q, out SCNQuaternion result)
        {
            float lengthSq = q.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                result = new SCNQuaternion(q.Xyz * -i, q.W * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static SCNQuaternion Normalize(SCNQuaternion q)
        {
            SCNQuaternion result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref SCNQuaternion q, out SCNQuaternion result)
        {
            float scale = 1.0f / q.Length;
            result = new SCNQuaternion(q.Xyz * scale, q.W * scale);
        }

        #endregion

        #region FromAxisAngle

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public static SCNQuaternion FromAxisAngle(SCNVector3 axis, float angle)
        {
            if (axis.LengthSquared == 0.0f)
                return Identity;

            SCNQuaternion result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.Xyz = axis * (float)System.Math.Sin(angle);
            result.W = (float)System.Math.Cos(angle);

            return Normalize(result);
        }

        #endregion

        #region Slerp

        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static SCNQuaternion Slerp(SCNQuaternion q1, SCNQuaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == 0.0f)
            {
                if (q2.LengthSquared == 0.0f)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared == 0.0f)
            {
                return q1;
            }


            pfloat cosHalfAngle = q1.W * q2.W + SCNVector3.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            SCNQuaternion result = new SCNQuaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return Identity;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNQuaternion operator +(SCNQuaternion left, SCNQuaternion right)
        {
            left.Xyz += right.Xyz;
            left.W += right.W;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNQuaternion operator -(SCNQuaternion left, SCNQuaternion right)
        {
            left.Xyz -= right.Xyz;
            left.W -= right.W;
            return left;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static SCNQuaternion operator *(SCNQuaternion left, SCNQuaternion right)
        {
            Multiply(ref left, ref right, out left);
            return left;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static SCNQuaternion operator *(SCNQuaternion quaternion, float scale)
        {
            Multiply(ref quaternion, scale, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static SCNQuaternion operator *(float scale, SCNQuaternion quaternion)
        {
            return new SCNQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(SCNQuaternion left, SCNQuaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(SCNQuaternion left, SCNQuaternion right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("V: {0}, W: {1}", Xyz, W);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        /// Compares this object instance to another object for equality. 
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is SCNQuaternion == false) return false;
               return this == (SCNQuaternion)other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        /// Provides the hash code for this object. 
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return Xyz.GetHashCode() ^ W.GetHashCode();
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<SCNQuaternion> Members

        /// <summary>
        /// Compares this SCNQuaternion instance to another SCNQuaternion for equality. 
        /// </summary>
        /// <param name="other">The other SCNQuaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(SCNQuaternion other)
        {
            return Xyz == other.Xyz && W == other.W;
        }

        #endregion
    }
}
