//
// SKUniform.cs: SKUniform class
//
// Authors:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2016 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Matrix2 = global::OpenTK.Matrix2;
using Matrix3 = global::OpenTK.Matrix3;
using Matrix4 = global::OpenTK.Matrix4;
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;

#if (XAMCORE_2_0 || !MONOMAC) && !WATCH
namespace SpriteKit {
	public partial class SKUniform {

		static bool? versionCheck = null;

		static bool CheckSystemVersion ()
		{
			if (!versionCheck.HasValue) {
#if MONOMAC
				versionCheck = PlatformHelper.CheckSystemVersion (10, 12);
#elif TVOS || IOS
				versionCheck = UIKit.UIDevice.CurrentDevice.CheckSystemVersion (10, 0);
#else
				#error Unknown platform
#endif
			}
			return versionCheck.Value;
		}

		// Apple deprecated initWithName:floatVector2: in macOS10.12/iOS10.0
		// and made available initWithName:vectorFloat2: so we invoke
		// the right one at runtime depending on which OS version we are running
		public SKUniform (string name, Vector2 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameVectorFloat2 (name, value), "initWithName:vectorFloat2:");
			else
				InitializeHandle (InitWithNameFloatVector2 (name, value), "initWithName:floatVector2:");
		}

		// Apple deprecated initWithName:floatVector3: in macOS10.12/iOS10.0
		// and made available initWithName:vectorFloat3: so we invoke
		// the right one at runtime depending on which OS version we are running
		public SKUniform (string name, Vector3 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameVectorFloat3 (name, value), "initWithName:vectorFloat3:");
			else
				InitializeHandle (InitWithNameFloatVector3 (name, value), "initWithName:floatVector3:");
		}

		// Apple deprecated initWithName:floatVector4: in macOS10.12/iOS10.0
		// and made available initWithName:vectorFloat4: so we invoke
		// the right one at runtime depending on which OS version we are running
		public SKUniform (string name, Vector4 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameVectorFloat4 (name, value), "initWithName:vectorFloat4:");
			else
				InitializeHandle (InitWithNameFloatVector4 (name, value), "initWithName:floatVector4:");
		}

#if !XAMCORE_4_0
		// Apple deprecated initWithName:floatMatrix2: in macOS10.12/iOS10.0
		// and made available initWithName:matrixFloat2x2: so we invoke
		// the right one at runtime depending on which OS version we are running
		//
		// Unfortunately our 'initWithName:matrixFloat2x2:' implementation is
		// incorrect (the matrix is transposed), but changing it would be a
		// breaking change, so we obsolete this constructor and provide a new
		// one which implements (only) 'initWithName:matrixFloat2x2:'
		// correctly. However, this constructor still does the right thing for
		// < macOS 10.12 / iOS 10.0
		[Obsolete ("Use the '(string, MatrixFloat2x2)' overload instead.")]
		public SKUniform (string name, Matrix2 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameMatrixFloat2x2 (name, value), "initWithName:matrixFloat2x2:");
			else
				InitializeHandle (InitWithNameFloatMatrix2 (name, value), "initWithName:floatMatrix2:");
		}

		// Apple deprecated initWithName:floatMatrix3: in macOS10.12/iOS10.0
		// and made available initWithName:matrixFloat3x3: so we invoke
		// the right one at runtime depending on which OS version we are running
		//
		// Unfortunately our 'initWithName:matrixFloat3x3:' implementation is
		// incorrect (the matrix is transposed), but changing it would be a
		// breaking change, so we obsolete this constructor and provide a new
		// one which implements (only) 'initWithName:matrixFloat3x3:'
		// correctly. However, this constructor still does the right thing for
		// < macOS 10.12 / iOS 10.0
		[Obsolete ("Use the '(string, MatrixFloat3x3)' overload instead.")]
		public SKUniform (string name, Matrix3 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameMatrixFloat3x3 (name, value), "initWithName:matrixFloat3x3:");
			else
				InitializeHandle (InitWithNameFloatMatrix3 (name, value), "initWithName:floatMatrix3:");
		}

		// Apple deprecated initWithName:floatMatrix4: in macOS10.12/iOS10.0
		// and made available initWithName:matrixFloat4x4: so we invoke
		// the right one at runtime depending on which OS version we are running
		//
		// Unfortunately our 'initWithName:matrixFloat4x4:' implementation is
		// incorrect (the matrix is transposed), but changing it would be a
		// breaking change, so we obsolete this constructor and provide a new
		// one which implements (only) 'initWithName:matrixFloat4x4:'
		// correctly. However, this constructor still does the right thing for
		// < macOS 10.12 / iOS 10.0
		[Obsolete ("Use the '(string, MatrixFloat4x4)' overload instead.")]
		public SKUniform (string name, Matrix4 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameMatrixFloat4x4 (name, value), "initWithName:matrixFloat4x4:");
			else
				InitializeHandle (InitWithNameFloatMatrix4 (name, value), "initWithName:floatMatrix4:");
		}
#endif // !XAMCORE_4_0

		// Apple deprecated floatVector2Value in macOS10.12/iOS10.0
		// and made available vectorFloat2Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public virtual Vector2 FloatVector2Value
		{
			get {
				if (CheckSystemVersion ())
					return _VectorFloat2Value;
				else
					return _FloatVector2Value;
			}
			set {
				if (CheckSystemVersion ())
					_VectorFloat2Value = value;
				else
					_FloatVector2Value = value;
			}
		}

		// Apple deprecated floatVector3Value in macOS10.12/iOS10.0
		// and made available vectorFloat3Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public virtual Vector3 FloatVector3Value
		{
			get {
				if (CheckSystemVersion ())
					return _VectorFloat3Value;
				else
					return _FloatVector3Value;
			}
			set {
				if (CheckSystemVersion ())
					_VectorFloat3Value = value;
				else
					_FloatVector3Value = value;
			}
		}

		// Apple deprecated floatVector4Value in macOS10.12/iOS10.0
		// and made available vectorFloat4Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public virtual Vector4 FloatVector4Value
		{
			get {
				if (CheckSystemVersion ())
					return _VectorFloat4Value;
				else
					return _FloatVector4Value;
			}
			set {
				if (CheckSystemVersion ())
					_VectorFloat4Value = value;
				else
					_FloatVector4Value = value;
			}
		}

#if !XAMCORE_4_0
		// Apple deprecated floatMatrix2Value in macOS10.12/iOS10.0
		// and made available matrixFloat2x2Value so we invoke
		// the right one at runtime depending on which OS version we are running
		//
		// Unfortunately our 'matrixFloat2x2Value' implementation is incorrect
		// (we return a transposed matrix), but changing it would be a
		// breaking change, so we obsolete this property and provide a new one
		// which implements (only) 'matrixFloat4x4Value' correctly. However,
		// this property still returns the correct matrix for < macOS 10.12 /
		// iOS 10.0
		[Obsolete ("Use 'MatrixFloat2x2Value' instead.")]
		public virtual Matrix2 FloatMatrix2Value
		{
			get {
				if (CheckSystemVersion ())
					return (Matrix2) MatrixFloat2x2.Transpose (MatrixFloat2x2Value);
				else
					return _FloatMatrix2Value;
			}
			set {
				if (CheckSystemVersion ())
					MatrixFloat2x2Value = MatrixFloat2x2.Transpose ((MatrixFloat2x2) value);
				else
					_FloatMatrix2Value = value;
			}
		}

		// Apple deprecated floatMatrix3Value in macOS10.12/iOS10.0
		// and made available matrixFloat3x3Value so we invoke
		// the right one at runtime depending on which OS version we are running
		//
		// Unfortunately our 'matrixFloat3x3Value' implementation is incorrect
		// (we return a transposed matrix), but changing it would be a
		// breaking change, so we obsolete this property and provide a new one
		// which implements (only) 'matrixFloat3x3Value' correctly. However,
		// this property still returns the correct matrix for < macOS 10.12 /
		// iOS 10.0
		[Obsolete ("Use 'MatrixFloat3x3Value' instead.")]
		public virtual Matrix3 FloatMatrix3Value
		{
			get {
				if (CheckSystemVersion ())
					return (Matrix3) MatrixFloat3x3.Transpose (MatrixFloat3x3Value);
				else
					return _FloatMatrix3Value;
			}
			set {
				if (CheckSystemVersion ())
					MatrixFloat3x3Value = MatrixFloat3x3.Transpose ((MatrixFloat3x3) value);
				else
					_FloatMatrix3Value = value;
			}
		}

		// Apple deprecated floatMatrix4Value in macOS10.12/iOS10.0
		// and made available matrixFloat4x4Value so we invoke
		// the right one at runtime depending on which OS version we are running
		//
		// Unfortunately our 'matrixFloat4x4Value' implementation is incorrect
		// (we return a transposed matrix), but changing it would be a
		// breaking change, so we obsolete this property and provide a new one
		// which implements (only) 'matrixFloat4x4Value' correctly. However,
		// this property still returns the correct matrix for < macOS 10.12 /
		// iOS 10.0
		[Obsolete ("Use 'MatrixFloat4x4Value' instead.")]
		public virtual Matrix4 FloatMatrix4Value
		{
			get {
				if (CheckSystemVersion ())
					return (Matrix4) MatrixFloat4x4.Transpose (MatrixFloat4x4Value);
				else
					return _FloatMatrix4Value;
			}
			set {
				if (CheckSystemVersion ())
					MatrixFloat4x4Value = MatrixFloat4x4.Transpose ((MatrixFloat4x4) value);
				else
					_FloatMatrix4Value = value;
			}
		}
#endif // !XAMCORE_4_0
	}
}
#endif // XAMCORE_2_0
