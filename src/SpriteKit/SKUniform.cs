//
// SKUniform.cs: SKUniform class
//
// Authors:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2016 Xamarin Inc.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Matrix2 = global::OpenTK.Matrix2;
using Matrix3 = global::OpenTK.Matrix3;
using Matrix4 = global::OpenTK.Matrix4;

#if XAMCORE_2_0 || !MONOMAC
namespace XamCore.SpriteKit {
	public partial class SKUniform {

		static bool? versionCheck = null;

		static bool CheckSystemVersion ()
		{
			if (!versionCheck.HasValue) {
#if MONOMAC
				versionCheck = PlatformHelper.CheckSystemVersion (10, 12);
#elif TVOS || IOS
				versionCheck = XamCore.UIKit.UIDevice.CurrentDevice.CheckSystemVersion (10, 0);
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

		// Apple deprecated initWithName:floatMatrix2: in macOS10.12/iOS10.0
		// and made available initWithName:matrixFloat2x2: so we invoke
		// the right one at runtime depending on which OS version we are running
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
		public SKUniform (string name, Matrix4 value)
		{
			if (CheckSystemVersion ())
				InitializeHandle (InitWithNameMatrixFloat4x4 (name, value), "initWithName:matrixFloat4x4:");
			else
				InitializeHandle (InitWithNameFloatMatrix4 (name, value), "initWithName:floatMatrix4:");
		}

		// Apple deprecated floatVector2Value in macOS10.12/iOS10.0
		// and made available vectorFloat2Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public Vector2 FloatVector2Value
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
		public Vector3 FloatVector3Value
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
		public Vector4 FloatVector4Value
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

		// Apple deprecated floatMatrix2Value in macOS10.12/iOS10.0
		// and made available matrixFloat2x2Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public Matrix2 FloatMatrix2Value
		{
			get {
				if (CheckSystemVersion ())
					return _MatrixFloat2x2Value;
				else
					return _FloatMatrix2Value;
			}
			set {
				if (CheckSystemVersion ())
					_MatrixFloat2x2Value = value;
				else
					_FloatMatrix2Value = value;
			}
		}

		// Apple deprecated floatMatrix3Value in macOS10.12/iOS10.0
		// and made available matrixFloat3x3Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public Matrix3 FloatMatrix3Value
		{
			get {
				if (CheckSystemVersion ())
					return _MatrixFloat3x3Value;
				else
					return _FloatMatrix3Value;
			}
			set {
				if (CheckSystemVersion ())
					_MatrixFloat3x3Value = value;
				else
					_FloatMatrix3Value = value;
			}
		}

		// Apple deprecated floatMatrix4Value in macOS10.12/iOS10.0
		// and made available matrixFloat4x4Value so we invoke
		// the right one at runtime depending on which OS version we are running
		public Matrix4 FloatMatrix4Value
		{
			get {
				if (CheckSystemVersion ())
					return _MatrixFloat4x4Value;
				else
					return _FloatMatrix4Value;
			}
			set {
				if (CheckSystemVersion ())
					_MatrixFloat4x4Value = value;
				else
					_FloatMatrix4Value = value;
			}
		}
	}
}
#endif // XAMCORE_2_0
