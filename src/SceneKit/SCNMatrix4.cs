/*
 * This keeps the code of OpenTK's Matrix4 almost intact, except we replace the
 * Vector4 with a SCNVector4
 
Copyright (c) 2006 - 2008 The Open Toolkit library.
Copyright (c) 2014 Xamarin Inc.  All rights reserved

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

using System;
using System.Runtime.InteropServices;

using Vector3 = global::OpenTK.Vector3;
using Vector3d = global::OpenTK.Vector3d;
using Vector4 = global::OpenTK.Vector4;
using Quaternion = global::OpenTK.Quaternion;
using Quaterniond = global::OpenTK.Quaterniond;
#if MONOMAC
using pfloat = System.nfloat;
#else
using pfloat = System.Single;
#endif


namespace SceneKit 
{
    /// <summary>
    /// Represents a 4x4 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SCNMatrix4 : IEquatable<SCNMatrix4>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public SCNVector4 Row0;
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public SCNVector4 Row1;
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public SCNVector4 Row2;
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public SCNVector4 Row3;
 
        /// <summary>
        /// The identity matrix
        /// </summary>
#if XAMCORE_2_0
		readonly
#endif
        public static SCNMatrix4 Identity = new SCNMatrix4(SCNVector4.UnitX, SCNVector4.UnitY, SCNVector4.UnitZ, SCNVector4.UnitW);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public SCNMatrix4(SCNVector4 row0, SCNVector4 row1, SCNVector4 row2, SCNVector4 row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m03">Fourth item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m13">Fourth item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        /// <param name="m23">First item of the third row of the matrix.</param>
        /// <param name="m30">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m31">Second item of the fourth row of the matrix.</param>
        /// <param name="m32">Third item of the fourth row of the matrix.</param>
        /// <param name="m33">Fourth item of the fourth row of the matrix.</param>
        public SCNMatrix4(
            pfloat m00, pfloat m01, pfloat m02, pfloat m03,
            pfloat m10, pfloat m11, pfloat m12, pfloat m13,
            pfloat m20, pfloat m21, pfloat m22, pfloat m23,
            pfloat m30, pfloat m31, pfloat m32, pfloat m33)
        {
            Row0 = new SCNVector4(m00, m01, m02, m03);
            Row1 = new SCNVector4(m10, m11, m12, m13);
            Row2 = new SCNVector4(m20, m21, m22, m23);
            Row3 = new SCNVector4(m30, m31, m32, m33);
        }

#if !WATCH
		public SCNMatrix4 (CoreAnimation.CATransform3D transform)
		{
			Row0 = new SCNVector4 ((pfloat)transform.m11, (pfloat)transform.m12, (pfloat)transform.m13, (pfloat)transform.m14);
			Row1 = new SCNVector4 ((pfloat)transform.m21, (pfloat)transform.m22, (pfloat)transform.m23, (pfloat)transform.m24);
			Row2 = new SCNVector4 ((pfloat)transform.m31, (pfloat)transform.m32, (pfloat)transform.m33, (pfloat)transform.m34);
			Row3 = new SCNVector4 ((pfloat)transform.m41, (pfloat)transform.m42, (pfloat)transform.m43, (pfloat)transform.m44);
		}
#endif
	
        #endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public pfloat Determinant
        {
            get
            {
                return
                    Row0.X * Row1.Y * Row2.Z * Row3.W - Row0.X * Row1.Y * Row2.W * Row3.Z + Row0.X * Row1.Z * Row2.W * Row3.Y - Row0.X * Row1.Z * Row2.Y * Row3.W
                  + Row0.X * Row1.W * Row2.Y * Row3.Z - Row0.X * Row1.W * Row2.Z * Row3.Y - Row0.Y * Row1.Z * Row2.W * Row3.X + Row0.Y * Row1.Z * Row2.X * Row3.W
                  - Row0.Y * Row1.W * Row2.X * Row3.Z + Row0.Y * Row1.W * Row2.Z * Row3.X - Row0.Y * Row1.X * Row2.Z * Row3.W + Row0.Y * Row1.X * Row2.W * Row3.Z
                  + Row0.Z * Row1.W * Row2.X * Row3.Y - Row0.Z * Row1.W * Row2.Y * Row3.X + Row0.Z * Row1.X * Row2.Y * Row3.W - Row0.Z * Row1.X * Row2.W * Row3.Y
                  + Row0.Z * Row1.Y * Row2.W * Row3.X - Row0.Z * Row1.Y * Row2.X * Row3.W - Row0.W * Row1.X * Row2.Y * Row3.Z + Row0.W * Row1.X * Row2.Z * Row3.Y
                  - Row0.W * Row1.Y * Row2.Z * Row3.X + Row0.W * Row1.Y * Row2.X * Row3.Z - Row0.W * Row1.Z * Row2.X * Row3.Y + Row0.W * Row1.Z * Row2.Y * Row3.X;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public SCNVector4 Column0
        {
            get { return new SCNVector4(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public SCNVector4 Column1
        {
            get { return new SCNVector4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public SCNVector4 Column2
        {
            get { return new SCNVector4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public SCNVector4 Column3
        {
            get { return new SCNVector4(Row0.W, Row1.W, Row2.W, Row3.W); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public pfloat M11 { get { return Row0.X; } set { Row0.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public pfloat M12 { get { return Row0.Y; } set { Row0.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public pfloat M13 { get { return Row0.Z; } set { Row0.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public pfloat M14 { get { return Row0.W; } set { Row0.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public pfloat M21 { get { return Row1.X; } set { Row1.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public pfloat M22 { get { return Row1.Y; } set { Row1.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public pfloat M23 { get { return Row1.Z; } set { Row1.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public pfloat M24 { get { return Row1.W; } set { Row1.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public pfloat M31 { get { return Row2.X; } set { Row2.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public pfloat M32 { get { return Row2.Y; } set { Row2.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public pfloat M33 { get { return Row2.Z; } set { Row2.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public pfloat M34 { get { return Row2.W; } set { Row2.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public pfloat M41 { get { return Row3.X; } set { Row3.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public pfloat M42 { get { return Row3.Y; } set { Row3.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public pfloat M43 { get { return Row3.Z; } set { Row3.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public pfloat M44 { get { return Row3.W; } set { Row3.W = value; } }

        #endregion

        #region Instance

        #region public void Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public void Invert()
        {
            this = SCNMatrix4.Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = SCNMatrix4.Transpose(this);
        }

        #endregion

        #endregion

        #region Static
        
        #region CreateFromAxisAngle
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static void CreateFromAxisAngle(SCNVector3 axis, pfloat angle, out SCNMatrix4 result)
        {
            pfloat cos = (float)System.Math.Cos(-angle);
            pfloat sin = (float)System.Math.Sin(-angle);
            pfloat t = 1.0f - cos;

            axis.Normalize();

            result = new SCNMatrix4(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0.0f,
                                 t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0.0f,
                                 t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0.0f,
                                 0, 0, 0, 1);
        }

        public static void CreateFromAxisAngle(Vector3 axis, float angle, out SCNMatrix4 result)
        {
            pfloat cos = (float)System.Math.Cos(-angle);
            pfloat sin = (float)System.Math.Sin(-angle);
            pfloat t = 1.0f - cos;

            axis.Normalize();

            result = new SCNMatrix4(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0.0f,
                                 t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0.0f,
                                 t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0.0f,
                                 0, 0, 0, 1);
        }

        public static void CreateFromAxisAngle(Vector3d axis, double angle, out SCNMatrix4 result)
        {
            double cos = System.Math.Cos(-angle);
            double sin = System.Math.Sin(-angle);
            double t = 1.0f - cos;

            axis.Normalize();

            result = new SCNMatrix4((pfloat)(t * axis.X * axis.X + cos), (pfloat)(t * axis.X * axis.Y - sin * axis.Z), (pfloat)(t * axis.X * axis.Z + sin * axis.Y), (pfloat)(0.0f),
				    (pfloat)( t * axis.X * axis.Y + sin * axis.Z), (pfloat)(t * axis.Y * axis.Y + cos), (pfloat)(t * axis.Y * axis.Z - sin * axis.X), (pfloat)0.0f,
				    (pfloat)(t * axis.X * axis.Z - sin * axis.Y), (pfloat)(t * axis.Y * axis.Z + sin * axis.X), (pfloat)(t * axis.Z * axis.Z + cos), (pfloat)0.0f,
				    0, 0, 0, 1);
        }
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static SCNMatrix4 CreateFromAxisAngle(SCNVector3 axis, pfloat angle)
        {
            SCNMatrix4 result;
            CreateFromAxisAngle(axis, angle, out result);
            return result;
        }
        
        #endregion

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateRotationX(pfloat angle, out SCNMatrix4 result)
        {
            pfloat cos = (pfloat)System.Math.Cos(angle);
            pfloat sin = (pfloat)System.Math.Sin(angle);

            result.Row0 = SCNVector4.UnitX;
            result.Row1 = new SCNVector4(0.0f, cos, sin, 0.0f);
            result.Row2 = new SCNVector4(0.0f, -sin, cos, 0.0f);
            result.Row3 = SCNVector4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting SCNMatrix4 instance.</returns>
        public static SCNMatrix4 CreateRotationX(pfloat angle)
        {
            SCNMatrix4 result;
            CreateRotationX(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateRotationY(pfloat angle, out SCNMatrix4 result)
        {
            pfloat cos = (pfloat)System.Math.Cos(angle);
            pfloat sin = (pfloat)System.Math.Sin(angle);

            result.Row0 = new SCNVector4(cos, 0.0f, -sin, 0.0f);
            result.Row1 = SCNVector4.UnitY;
            result.Row2 = new SCNVector4(sin, 0.0f, cos, 0.0f);
            result.Row3 = SCNVector4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting SCNMatrix4 instance.</returns>
        public static SCNMatrix4 CreateRotationY(pfloat angle)
        {
            SCNMatrix4 result;
            CreateRotationY(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateRotationZ(pfloat angle, out SCNMatrix4 result)
        {
            pfloat cos = (pfloat)System.Math.Cos(angle);
            pfloat sin = (pfloat)System.Math.Sin(angle);

            result.Row0 = new SCNVector4(cos, sin, 0.0f, 0.0f);
            result.Row1 = new SCNVector4(-sin, cos, 0.0f, 0.0f);
            result.Row2 = SCNVector4.UnitZ;
            result.Row3 = SCNVector4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting SCNMatrix4 instance.</returns>
        public static SCNMatrix4 CreateRotationZ(pfloat angle)
        {
            SCNMatrix4 result;
            CreateRotationZ(angle, out result);
            return result;
        }

        #endregion

        #region CreateTranslation

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateTranslation(pfloat x, pfloat y, pfloat z, out SCNMatrix4 result)
        {
            result = Identity;
            result.Row3 = new SCNVector4(x, y, z, 1);
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateTranslation(ref SCNVector3 vector, out SCNMatrix4 result)
        {
            result = Identity;
            result.Row3 = new SCNVector4(vector.X, vector.Y, vector.Z, 1);
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting SCNMatrix4 instance.</returns>
        public static SCNMatrix4 CreateTranslation(pfloat x, pfloat y, pfloat z)
        {
            SCNMatrix4 result;
            CreateTranslation(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting SCNMatrix4 instance.</returns>
        public static SCNMatrix4 CreateTranslation(SCNVector3 vector)
        {
            SCNMatrix4 result;
            CreateTranslation(vector.X, vector.Y, vector.Z, out result);
            return result;
        }

        #endregion

        #region CreateOrthographic

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateOrthographic(pfloat width, pfloat height, pfloat zNear, pfloat zFar, out SCNMatrix4 result)
        {
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <rereturns>The resulting SCNMatrix4 instance.</rereturns>
        public static SCNMatrix4 CreateOrthographic(pfloat width, pfloat height, pfloat zNear, pfloat zFar)
        {
            SCNMatrix4 result;
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
            return result;
        }

        #endregion

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting SCNMatrix4 instance.</param>
        public static void CreateOrthographicOffCenter(pfloat left, pfloat right, pfloat bottom, pfloat top, pfloat zNear, pfloat zFar, out SCNMatrix4 result)
        {
            result = new SCNMatrix4();

            pfloat invRL = 1 / (right - left);
            pfloat invTB = 1 / (top - bottom);
            pfloat invFN = 1 / (zFar - zNear);

            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = -2 * invFN;

            result.M41 = -(right + left) * invRL;
            result.M42 = -(top + bottom) * invTB;
            result.M43 = -(zFar + zNear) * invFN;
            result.M44 = 1;
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting SCNMatrix4 instance.</returns>
        public static SCNMatrix4 CreateOrthographicOffCenter(pfloat left, pfloat right, pfloat bottom, pfloat top, pfloat zNear, pfloat zFar)
        {
            SCNMatrix4 result;
            CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }

        #endregion
        
        #region CreatePerspectiveFieldOfView
        
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveFieldOfView(pfloat fovy, pfloat aspect, pfloat zNear, pfloat zFar, out SCNMatrix4 result)
        {
            if (fovy <= 0 || fovy > Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            pfloat yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            pfloat yMin = -yMax;
            pfloat xMin = yMin * aspect;
            pfloat xMax = yMax * aspect;

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
        }
        
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static SCNMatrix4 CreatePerspectiveFieldOfView(pfloat fovy, pfloat aspect, pfloat zNear, pfloat zFar)
        {
            SCNMatrix4 result;
            CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
            return result;
        }
        
        #endregion
        
        #region CreatePerspectiveOffCenter
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveOffCenter(pfloat left, pfloat right, pfloat bottom, pfloat top, pfloat zNear, pfloat zFar, out SCNMatrix4 result)
        {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            pfloat x = (2.0f * zNear) / (right - left);
            pfloat y = (2.0f * zNear) / (top - bottom);
            pfloat a = (right + left) / (right - left);
            pfloat b = (top + bottom) / (top - bottom);
            pfloat c = -(zFar + zNear) / (zFar - zNear);
            pfloat d = -(2.0f * zFar * zNear) / (zFar - zNear);
            
            result = new SCNMatrix4(x, 0, 0,  0,
                                 0, y, 0,  0,
                                 a, b, c, -1,
                                 0, 0, d,  0);
        }
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static SCNMatrix4 CreatePerspectiveOffCenter(pfloat left, pfloat right, pfloat bottom, pfloat top, pfloat zNear, pfloat zFar)
        {
            SCNMatrix4 result;
            CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
        
        #endregion

        #region Obsolete Functions

#if !XAMCORE_2_0
        #region Translation Functions

        /// <summary>
        /// Builds a translation matrix.
        /// </summary>
        /// <param name="trans">The translation vector.</param>
        /// <returns>A new SCNMatrix4 instance.</returns>
        [Obsolete("Use CreateTranslation instead.")]
        public static SCNMatrix4 Translation(SCNVector3 trans)
        {
            return Translation(trans.X, trans.Y, trans.Z);
        }

        /// <summary>
        /// Build a translation matrix with the given translation
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        /// <returns>A Translation matrix</returns>
        [Obsolete("Use CreateTranslation instead.")]
        public static SCNMatrix4 Translation(pfloat x, pfloat y, pfloat z)
        {
            SCNMatrix4 result = Identity;
            result.Row3 = new SCNVector4(x, y, z, 1.0f);
            return result;
        }

		#endregion
#endif // !XAMCORE_2_0

        #endregion

        #region Scale Functions

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="scale">Single scale factor for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static SCNMatrix4 Scale(pfloat scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="scale">Scale factors for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static SCNMatrix4 Scale(SCNVector3 scale)
        {
            return Scale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>A scaling matrix</returns>
        public static SCNMatrix4 Scale(pfloat x, pfloat y, pfloat z)
        {
            SCNMatrix4 result;
            result.Row0 = SCNVector4.UnitX * x;
            result.Row1 = SCNVector4.UnitY * y;
            result.Row2 = SCNVector4.UnitZ * z;
            result.Row3 = SCNVector4.UnitW;
            return result;
        }

        #endregion

        #region Rotation Functions

#if !XAMCORE_2_0
        /// <summary>
        /// Build a rotation matrix that rotates about the x-axis
        /// </summary>
        /// <param name="angle">angle in radians to rotate counter-clockwise around the x-axis</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateRotationX instead.")]
        public static SCNMatrix4 RotateX(pfloat angle)
        {
            pfloat cos = (pfloat)System.Math.Cos(angle);
            pfloat sin = (pfloat)System.Math.Sin(angle);

            SCNMatrix4 result;
            result.Row0 = SCNVector4.UnitX;
            result.Row1 = new SCNVector4(0.0f, cos, sin, 0.0f);
            result.Row2 = new SCNVector4(0.0f, -sin, cos, 0.0f);
            result.Row3 = SCNVector4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix that rotates about the y-axis
        /// </summary>
        /// <param name="angle">angle in radians to rotate counter-clockwise around the y-axis</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateRotationY instead.")]
        public static SCNMatrix4 RotateY(pfloat angle)
        {
            pfloat cos = (pfloat)System.Math.Cos(angle);
            pfloat sin = (pfloat)System.Math.Sin(angle);

            SCNMatrix4 result;
            result.Row0 = new SCNVector4(cos, 0.0f, -sin, 0.0f);
            result.Row1 = SCNVector4.UnitY;
            result.Row2 = new SCNVector4(sin, 0.0f, cos, 0.0f);
            result.Row3 = SCNVector4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix that rotates about the z-axis
        /// </summary>
        /// <param name="angle">angle in radians to rotate counter-clockwise around the z-axis</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateRotationZ instead.")]
        public static SCNMatrix4 RotateZ(pfloat angle)
        {
            pfloat cos = (pfloat)System.Math.Cos(angle);
            pfloat sin = (pfloat)System.Math.Sin(angle);

            SCNMatrix4 result;
            result.Row0 = new SCNVector4(cos, sin, 0.0f, 0.0f);
            result.Row1 = new SCNVector4(-sin, cos, 0.0f, 0.0f);
            result.Row2 = SCNVector4.UnitZ;
            result.Row3 = SCNVector4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix to rotate about the given axis
        /// </summary>
        /// <param name="axis">the axis to rotate about</param>
        /// <param name="angle">angle in radians to rotate counter-clockwise (looking in the direction of the given axis)</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateFromAxisAngle instead.")]
        public static SCNMatrix4 Rotate(SCNVector3 axis, pfloat angle)
        {
            pfloat cos = (pfloat)System.Math.Cos(-angle);
            pfloat sin = (pfloat)System.Math.Sin(-angle);
            pfloat t = 1.0f - cos;

            axis.Normalize();

            SCNMatrix4 result;
            result.Row0 = new SCNVector4(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0.0f);
            result.Row1 = new SCNVector4(t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0.0f);
            result.Row2 = new SCNVector4(t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0.0f);
            result.Row3 = SCNVector4.UnitW;
            return result;
        }
#endif

        /// <summary>
        /// Build a rotation matrix from a quaternion
        /// </summary>
        /// <param name="q">the quaternion</param>
        /// <returns>A rotation matrix</returns>
        public static SCNMatrix4 Rotate(Quaternion q)
        {
	    SCNMatrix4 result;
            Vector3 axis;
            float angle;
            q.ToAxisAngle(out axis, out angle);
            CreateFromAxisAngle(axis, angle, out result);
	    return result;
        }

        /// <summary>
        /// Build a rotation matrix from a quaternion
        /// </summary>
        /// <param name="q">the quaternion</param>
        /// <returns>A rotation matrix</returns>
        public static SCNMatrix4 Rotate(Quaterniond q)
        {
            Vector3d axis;
            double angle;
	    SCNMatrix4 result;
            q.ToAxisAngle(out axis, out angle);
	    CreateFromAxisAngle(axis, angle, out result);
            return result;
        }
        #endregion

        #region Camera Helper Functions

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A SCNMatrix4 that transforms world space to camera space</returns>
        public static SCNMatrix4 LookAt(SCNVector3 eye, SCNVector3 target, SCNVector3 up)
        {
            SCNVector3 z = SCNVector3.Normalize(eye - target);
            SCNVector3 x = SCNVector3.Normalize(SCNVector3.Cross(up, z));
            SCNVector3 y = SCNVector3.Normalize(SCNVector3.Cross(z, x));

            SCNMatrix4 rot = new SCNMatrix4(new SCNVector4(x.X, y.X, z.X, 0.0f),
                                        new SCNVector4(x.Y, y.Y, z.Y, 0.0f),
                                        new SCNVector4(x.Z, y.Z, z.Z, 0.0f),
                                        SCNVector4.UnitW);

            SCNMatrix4 trans = SCNMatrix4.CreateTranslation(-eye);

            return trans * rot;
        }

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eyeX">Eye (camera) position in world space</param>
        /// <param name="eyeY">Eye (camera) position in world space</param>
        /// <param name="eyeZ">Eye (camera) position in world space</param>
        /// <param name="targetX">Target position in world space</param>
        /// <param name="targetY">Target position in world space</param>
        /// <param name="targetZ">Target position in world space</param>
        /// <param name="upX">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upY">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upZ">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A SCNMatrix4 that transforms world space to camera space</returns>
        public static SCNMatrix4 LookAt(pfloat eyeX, pfloat eyeY, pfloat eyeZ, pfloat targetX, pfloat targetY, pfloat targetZ, pfloat upX, pfloat upY, pfloat upZ)
        {
            return LookAt(new SCNVector3(eyeX, eyeY, eyeZ), new SCNVector3(targetX, targetY, targetZ), new SCNVector3(upX, upY, upZ));
        }

#if !XAMCORE_2_0
        /// <summary>
        /// Build a projection matrix
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="near">Distance to the near clip plane</param>
        /// <param name="far">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        [Obsolete("Use CreatePerspectiveOffCenter instead.")]
        public static SCNMatrix4 Frustum(pfloat left, pfloat right, pfloat bottom, pfloat top, pfloat near, pfloat far)
        {
            pfloat invRL = 1.0f / (right - left);
            pfloat invTB = 1.0f / (top - bottom);
            pfloat invFN = 1.0f / (far - near);
            return new SCNMatrix4(new SCNVector4(2.0f * near * invRL, 0.0f, 0.0f, 0.0f),
                               new SCNVector4(0.0f, 2.0f * near * invTB, 0.0f, 0.0f),
                               new SCNVector4((right + left) * invRL, (top + bottom) * invTB, -(far + near) * invFN, -1.0f),
                               new SCNVector4(0.0f, 0.0f, -2.0f * far * near * invFN, 0.0f));
        }

        /// <summary>
        /// Build a projection matrix
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="near">Distance to the near clip plane</param>
        /// <param name="far">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        [Obsolete("Use CreatePerspectiveFieldOfView instead.")]
        public static SCNMatrix4 Perspective(pfloat fovy, pfloat aspect, pfloat near, pfloat far)
        {
            pfloat yMax = near * (float)System.Math.Tan(0.5f * fovy);
            pfloat yMin = -yMax;
            pfloat xMin = yMin * aspect;
            pfloat xMax = yMax * aspect;

            return Frustum(xMin, xMax, yMin, yMax, near, far);
        }
#endif

        #endregion

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static SCNMatrix4 Mult(SCNMatrix4 left, SCNMatrix4 right)
        {
            SCNMatrix4 result;
            Mult(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref SCNMatrix4 left, ref SCNMatrix4 right, out SCNMatrix4 result)
        {
            result = new SCNMatrix4(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43,
                left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44,
                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43,
                left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44,
                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43,
                left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44,
                left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41,
                left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42,
                left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43,
                left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44);
        }

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the SCNMatrix4 is singular.</exception>
        public static SCNMatrix4 Invert(SCNMatrix4 mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            pfloat[,] inverse = {{mat.Row0.X, mat.Row0.Y, mat.Row0.Z, mat.Row0.W}, 
                                {mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W}, 
                                {mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W}, 
                                {mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                pfloat maxPivot = 0.0f;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                pfloat absVal = (pfloat)System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        pfloat f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                pfloat pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0f)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                pfloat oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        pfloat f = inverse[j, icol];
                        inverse[j, icol] = 0.0f;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    pfloat f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new SCNVector4(inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[0, 3]);
            mat.Row1 = new SCNVector4(inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[1, 3]);
            mat.Row2 = new SCNVector4(inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[2, 3]);
            mat.Row3 = new SCNVector4(inverse[3, 0], inverse[3, 1], inverse[3, 2], inverse[3, 3]);
            return mat;
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static SCNMatrix4 Transpose(SCNMatrix4 mat)
        {
            return new SCNMatrix4(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref SCNMatrix4 mat, out SCNMatrix4 result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new SCNMatrix44 which holds the result of the multiplication</returns>
        public static SCNMatrix4 operator *(SCNMatrix4 left, SCNMatrix4 right)
        {
            return SCNMatrix4.Mult(left, right);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(SCNMatrix4 left, SCNMatrix4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(SCNMatrix4 left, SCNMatrix4 right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current SCNMatrix44.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SCNMatrix4))
                return false;

            return this.Equals((SCNMatrix4)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<SCNMatrix4> Members

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">An matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(SCNMatrix4 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion

#if false

	// The following code compiles, btu I want to think about
	// making this so easily accessible with an implicit operator,
	// after all, this can be up to 1k of data being moved.
	
	// We can do implicit, because our storage is always >= CATransform3D which uses nfloats
	public static implicit operator CoreAnimation.CATransform3D (SCNMatrix4 source)
	{
		return new CoreAnimation.CATransform3D (){
			m11 = source.Row0.X,
			m12 = source.Row0.Y,
			m13 = source.Row0.Z,
			m14 = source.Row0.W,

			m21 = source.Row1.X,
			m22 = source.Row1.Y,
			m23 = source.Row1.Z,
			m24 = source.Row1.W,

			m31 = source.Row2.X,
			m32 = source.Row2.Y,
			m33 = source.Row2.Z,
			m34 = source.Row2.W,

			m41 = source.Row3.X,
			m42 = source.Row3.Y,
			m43 = source.Row3.Z,
			m44 = source.Row3.W,
		};
	}
#endif
    }
}
