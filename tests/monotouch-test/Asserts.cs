using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#if !__WATCHOS__
using ModelIO;
using MetalPerformanceShaders;
#endif
#if HAS_SCENEKIT
using SceneKit;
#endif

#if NET
using System.Numerics;
using CoreGraphics;
using MatrixFloat2x2 = global::CoreGraphics.NMatrix2;
using MatrixFloat3x3 = global::CoreGraphics.NMatrix3;
using MatrixFloat4x3 = global::CoreGraphics.NMatrix4x3;
using Matrix4 = global::System.Numerics.Matrix4x4;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
using VectorFloat3 = global::CoreGraphics.NVector3;
using MatrixDouble4x4 = global::CoreGraphics.NMatrix4d;
using VectorDouble2 = global::CoreGraphics.NVector2d;
using VectorDouble3 = global::CoreGraphics.NVector3d;
using Vector4d = global::CoreGraphics.NVector4d;
using Vector2i = global::CoreGraphics.NVector2i;
using Vector4i = global::CoreGraphics.NVector4i;
using Quaterniond = global::CoreGraphics.NQuaterniond;
#else
using OpenTK;
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x3 = global::OpenTK.NMatrix4x3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
using VectorFloat3 = global::OpenTK.NVector3;
using MatrixDouble4x4 = global::OpenTK.NMatrix4d;
using VectorDouble3 = global::OpenTK.NVector3d;
#endif

#if __MACOS__
#if NET
using pfloat = System.Runtime.InteropServices.NFloat;
#else
using pfloat = System.nfloat;
#endif
#else
using pfloat = System.Single;
#endif

using NUnit.Framework;

public static class Asserts {
	public static void AreEqual (bool expected, bool actual, string message)
	{
		Assert.AreEqual (expected, actual, $"{message} (M) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (float expected, float actual, string message)
	{
		Assert.AreEqual (expected, actual, $"{message} (M) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (float expected, float actual, float delta, string message)
	{
		Assert.AreEqual (expected, actual, delta, message);
	}

	public static void AreEqual (Vector2 expected, Vector2 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector3 expected, Vector3 actual, string message)
	{

		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector3 expected, Vector3 actual, float delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector3 expected, VectorFloat3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (VectorFloat3 expected, Vector3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (VectorFloat3 expected, VectorFloat3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (VectorFloat3 expected, VectorFloat3 actual, float delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector4 expected, Vector4 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, $"{message} (W) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (float expectedX, float expectedY, float expectedZ, float expectedW, Vector4 actual, string message)
	{
		Assert.AreEqual (expectedX, actual.X, $"{message} (X) expected: {new Vector4 (expectedX, expectedY, expectedZ, expectedW)} actual: {actual}");
		Assert.AreEqual (expectedY, actual.Y, $"{message} (Y) expected: {new Vector4 (expectedX, expectedY, expectedZ, expectedW)} actual: {actual}");
		Assert.AreEqual (expectedZ, actual.Z, $"{message} (Z) expected: {new Vector4 (expectedX, expectedY, expectedZ, expectedW)} actual: {actual}");
		Assert.AreEqual (expectedW, actual.W, $"{message} (W) expected: {new Vector4 (expectedX, expectedY, expectedZ, expectedW)} actual: {actual}");
	}

	public static void AreEqual (Vector4 expected, Vector4 actual, float delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, delta, $"{message} (W) expected: {expected} actual: {actual}");
	}

#if !NET
	public static void AreEqual (Matrix2 expected, Matrix2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix3 expected, Matrix3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C2, actual.R0C2, $"{message} (R0C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C2, actual.R1C2, $"{message} (R1C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C0, actual.R2C0, $"{message} (R2C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C1, actual.R2C1, $"{message} (R2C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C2, actual.R2C2, $"{message} (R2C2) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix3 expected, Matrix3 actual, float delta, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, delta, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, delta, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C2, actual.R0C2, delta, $"{message} (R0C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, delta, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, delta, $"{message} (R1C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C2, actual.R1C2, delta, $"{message} (R1C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C0, actual.R2C0, delta, $"{message} (R2C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C1, actual.R2C1, delta, $"{message} (R2C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C2, actual.R2C2, delta, $"{message} (R2C2) expected: {expected} actual: {actual}");
	}
#endif

	public static void AreEqual (Matrix4 expected, Matrix4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix4 expected, Matrix4 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, delta, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, delta, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, delta, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, delta, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, delta, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, delta, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, delta, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, delta, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, delta, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, delta, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, delta, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, delta, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, delta, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, delta, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, delta, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector2i expected, Vector2i actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector4i expected, Vector4i actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, $"{message} (W) expected: {expected} actual: {actual}");
	}

#if !__WATCHOS__
	public static void AreEqual (MDLAxisAlignedBoundingBox expected, MDLAxisAlignedBoundingBox actual, string message)
	{
		AreEqual (expected.MaxBounds, actual.MaxBounds, $"{message} (MaxBounds) expected: {expected} actual: {actual}");
		AreEqual (expected.MinBounds, actual.MinBounds, $"{message} (MinBounds) expected: {expected} actual: {actual}");
	}
#endif // !__WATCHOS__

	public static void AreEqual (Quaternion expected, Quaternion actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, $"{message} (W) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Quaternion [] expected, Quaternion [] actual, string message)
	{
		if (expected is null) {
			if (actual is null)
				return;
			Assert.Fail ($"Expected null, got {actual}. {message}");
		} else if (actual is null) {
			Assert.Fail ($"Expected {expected}, got null. {message}");
		}

		Assert.AreEqual (expected.Length, actual.Length, $"{message} array lengths");
		for (var i = 0; i < expected.Length; i++) {
			AreEqual (expected [i], actual [i], message + $" [{i}]");
		}
	}

	public static void AreEqual (Quaterniond expected, Quaterniond actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, $"{message} (W) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Quaterniond expected, Quaterniond actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, delta, $"{message} (W) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Quaterniond [] expected, Quaterniond [] actual, string message)
	{
		Assert.AreEqual (expected.Length, actual.Length, $"{message} array lengths");
		for (var i = 0; i < expected.Length; i++) {
			AreEqual (expected [i], actual [i], message + $" [{i}]");
		}
	}

#if !__WATCHOS__
	public static void AreEqual (MPSImageHistogramInfo expected, MPSImageHistogramInfo actual, string message)
	{
		Assert.AreEqual (expected.HistogramForAlpha, actual.HistogramForAlpha, $"{message} HistogramForAlpha expected: {expected} actual: {actual}");
		Asserts.AreEqual (expected.MaxPixelValue, actual.MaxPixelValue, $"{message} MaxPixelValue expected: {expected} actual: {actual}");
		Asserts.AreEqual (expected.MinPixelValue, actual.MinPixelValue, $"{message} MinPixelValue expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.NumberOfHistogramEntries, actual.NumberOfHistogramEntries, $"{message} NumberOfHistogramEntries expected: {expected} actual: {actual}");
	}
#endif // !__WATCHOS__

	public static void AreEqual (MatrixFloat2x2 expected, MatrixFloat2x2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixFloat2x2 expected, MatrixFloat2x2 actual, float delta, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, delta, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, delta, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, delta, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, delta, $"{message} (R1C1) expected: {expected} actual: {actual}");
	}

#if !NET
	public static void AreEqual (Matrix2 expected, MatrixFloat2x2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixFloat2x2 expected, Matrix2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
	}
#endif // !NET

	public static void AreEqual (MatrixFloat3x3 expected, MatrixFloat3x3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C0, actual.R2C0, $"{message} (R2C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C1, actual.R2C1, $"{message} (R2C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C2, actual.R0C2, $"{message} (R0C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C2, actual.R1C2, $"{message} (R1C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C2, actual.R2C2, $"{message} (R2C2) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixFloat3x3 expected, MatrixFloat3x3 actual, float delta, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, delta, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, delta, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C0, actual.R2C0, delta, $"{message} (R2C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, delta, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, delta, $"{message} (R1C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C1, actual.R2C1, delta, $"{message} (R2C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C2, actual.R0C2, delta, $"{message} (R0C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C2, actual.R1C2, delta, $"{message} (R1C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C2, actual.R2C2, delta, $"{message} (R2C2) expected: {expected} actual: {actual}");
	}

#if !NET
	public static void AreEqual (Matrix3 expected, MatrixFloat3x3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C2, actual.R0C2, $"{message} (R0C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C2, actual.R1C2, $"{message} (R1C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C0, actual.R2C0, $"{message} (R2C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C1, actual.R2C1, $"{message} (R2C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C2, actual.R2C2, $"{message} (R2C2) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixFloat3x3 expected, Matrix3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, $"{message} (R0C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C1, actual.R0C1, $"{message} (R0C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R0C2, actual.R0C2, $"{message} (R0C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C0, actual.R1C0, $"{message} (R1C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C1, actual.R1C1, $"{message} (R1C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R1C2, actual.R1C2, $"{message} (R1C2) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C0, actual.R2C0, $"{message} (R2C0) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C1, actual.R2C1, $"{message} (R2C1) expected: {expected} actual: {actual}");
		AreEqual (expected.R2C2, actual.R2C2, $"{message} (R2C2) expected: {expected} actual: {actual}");
	}
#endif

	public static void AreEqual (MatrixFloat4x4 expected, MatrixFloat4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixFloat4x4 expected, MatrixFloat4x4 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, delta, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, delta, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, delta, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, delta, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, delta, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, delta, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, delta, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, delta, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, delta, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, delta, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, delta, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, delta, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, delta, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, delta, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, delta, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix4 expected, MatrixFloat4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix4 expected, MatrixFloat4x4 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, delta, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, delta, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, delta, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, delta, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, delta, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, delta, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, delta, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, delta, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, delta, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, delta, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, delta, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, delta, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, delta, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, delta, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, delta, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixFloat4x4 expected, Matrix4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	#region Double Based Types
	public static void AreEqual (double expected, double actual, string message)
	{
		Assert.AreEqual (expected, actual, $"{message} (M) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (double expected, double actual, double delta, string message)
	{
		Assert.AreEqual (expected, actual, delta, message);
	}

#if !NET
	public static void AreEqual (Vector2d expected, Vector2d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
	}
#endif

#if NET
	public static void AreEqual (VectorDouble2 expected, VectorDouble2 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
	}

	public static void AreEqual (VectorDouble2 expected, VectorDouble2 actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
	}
#endif

#if !NET
	public static void AreEqual (Vector3d expected, Vector3d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector3d expected, Vector3d actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector3d expected, VectorDouble3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (VectorDouble3 expected, Vector3d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}
#endif

	public static void AreEqual (VectorDouble3 expected, VectorDouble3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (VectorDouble3 expected, VectorDouble3 actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector4d expected, Vector4d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, $"{message} (W) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Vector4d expected, Vector4d actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, $"{message} (X) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Y, actual.Y, delta, $"{message} (Y) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.Z, actual.Z, delta, $"{message} (Z) expected: {expected} actual: {actual}");
		Assert.AreEqual (expected.W, actual.W, delta, $"{message} (W) expected: {expected} actual: {actual}");
	}

#if !NET
	public static void AreEqual (Matrix4d expected, Matrix4d actual, string message)
	{
		AreEqual (expected.Column0, actual.Column0, $"{message} (Col0) expected: {expected} actual: {actual}");
		AreEqual (expected.Column1, actual.Column1, $"{message} (Col1) expected: {expected} actual: {actual}");
		AreEqual (expected.Column2, actual.Column2, $"{message} (Col2) expected: {expected} actual: {actual}");
		AreEqual (expected.Column3, actual.Column3, $"{message} (Col3) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix4d expected, Matrix4d actual, double delta, string message)
	{
		AreEqual (expected.Column0, actual.Column0, delta, $"{message} (Col0) expected: {expected} actual: {actual}");
		AreEqual (expected.Column1, actual.Column1, delta, $"{message} (Col1) expected: {expected} actual: {actual}");
		AreEqual (expected.Column2, actual.Column2, delta, $"{message} (Col2) expected: {expected} actual: {actual}");
		AreEqual (expected.Column3, actual.Column3, delta, $"{message} (Col3) expected: {expected} actual: {actual}");
	}
#endif //!NET

	public static void AreEqual (MatrixDouble4x4 expected, MatrixDouble4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (MatrixDouble4x4 expected, MatrixDouble4x4 actual, double delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, delta, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, delta, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, delta, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, delta, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, delta, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, delta, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, delta, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, delta, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, delta, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, delta, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, delta, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, delta, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, delta, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, delta, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, delta, $"{message} (M44) expected: {expected} actual: {actual}");
	}

#if !NET
	public static void AreEqual (Matrix4d expected, MatrixDouble4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}

	public static void AreEqual (Matrix4d expected, NMatrix4d actual, double delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, delta, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, delta, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, delta, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, delta, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, delta, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, delta, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, delta, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, delta, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, delta, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, delta, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, delta, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, delta, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, delta, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, delta, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, delta, $"{message} (M44) expected: {expected} actual: {actual}");
	}
#endif // !NET

	public static void AreEqual (NMatrix4x3 expected, NMatrix4x3 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, delta, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, delta, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, delta, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, delta, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, delta, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, delta, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, delta, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, delta, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, delta, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, delta, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, delta, $"{message} (M34) expected: {expected} actual: {actual}");
	}

#if !NET
	public static void AreEqual (NMatrix4d expected, Matrix4d actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M41, actual.M41, $"{message} (M41) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M42, actual.M42, $"{message} (M42) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M43, actual.M43, $"{message} (M43) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
		AreEqual (expected.M44, actual.M44, $"{message} (M44) expected: {expected} actual: {actual}");
	}
#endif

	public static void AreEqual (NMatrix4x3 expected, NMatrix4x3 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, $"{message} (M11) expected: {expected} actual: {actual}");
		AreEqual (expected.M21, actual.M21, $"{message} (M21) expected: {expected} actual: {actual}");
		AreEqual (expected.M31, actual.M31, $"{message} (M31) expected: {expected} actual: {actual}");
		AreEqual (expected.M12, actual.M12, $"{message} (M12) expected: {expected} actual: {actual}");
		AreEqual (expected.M22, actual.M22, $"{message} (M22) expected: {expected} actual: {actual}");
		AreEqual (expected.M32, actual.M32, $"{message} (M32) expected: {expected} actual: {actual}");
		AreEqual (expected.M13, actual.M13, $"{message} (M13) expected: {expected} actual: {actual}");
		AreEqual (expected.M23, actual.M23, $"{message} (M23) expected: {expected} actual: {actual}");
		AreEqual (expected.M33, actual.M33, $"{message} (M33) expected: {expected} actual: {actual}");
		AreEqual (expected.M14, actual.M14, $"{message} (M14) expected: {expected} actual: {actual}");
		AreEqual (expected.M24, actual.M24, $"{message} (M24) expected: {expected} actual: {actual}");
		AreEqual (expected.M34, actual.M34, $"{message} (M34) expected: {expected} actual: {actual}");
	}
	#endregion

#if HAS_SCENEKIT
	public static void AreEqual (SCNVector3 expected, SCNVector3 actual, string message)
	{
		if (AreEqual (expected.X, actual.X, out var dX) &
			AreEqual (expected.Y, actual.Y, out var dY) &
			AreEqual (expected.Z, actual.Z, out var dZ))
			return;

		var diffString = $"({dX}, {dY}, {dZ})";
		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}

	public static void AreEqual (SCNVector3 expected, SCNVector3 actual, pfloat delta, string message)
	{
		if (AreEqual (expected.X, actual.X, delta, out var dX) &
			AreEqual (expected.Y, actual.Y, delta, out var dY) &
			AreEqual (expected.Z, actual.Z, delta, out var dZ))
			return;

		var diffString = $"({dX}, {dY}, {dZ})";
		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}

	public static void AreEqual (SCNVector4 expected, SCNVector4 actual, string message)
	{
		if (AreEqual (expected.X, actual.X, out var dX) &
			AreEqual (expected.Y, actual.Y, out var dY) &
			AreEqual (expected.Z, actual.Z, out var dZ) &
			AreEqual (expected.W, actual.W, out var dW))
			return;

		var diffString = $"({dX}, {dY}, {dZ}, {dW})";
		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}


	public static void AreEqual (SCNVector4 expected, SCNVector4 actual, pfloat delta, string message)
	{
		if (AreEqual (expected.X, actual.X, delta, out var dX) &
			AreEqual (expected.Y, actual.Y, delta, out var dY) &
			AreEqual (expected.Z, actual.Z, delta, out var dZ) &
			AreEqual (expected.W, actual.W, delta, out var dW))
			return;

		var diffString = $"({dX}, {dY}, {dZ}, {dW})";
		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}

	public static void AreEqual (SCNQuaternion expected, SCNQuaternion actual, string message)
	{
		if (AreEqual (expected.X, actual.X, out var dX) &
			AreEqual (expected.Y, actual.Y, out var dY) &
			AreEqual (expected.Z, actual.Z, out var dZ) &
			AreEqual (expected.W, actual.W, out var dW))
			return;

		var diffString = $"[{dX}, {dY}, {dZ}, {dW}]";
		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}

	public static void AreEqual (SCNQuaternion expected, SCNQuaternion actual, pfloat delta, string message)
	{
		if (AreEqual (expected.X, actual.X, delta, out var dX) &
			AreEqual (expected.Y, actual.Y, delta, out var dY) &
			AreEqual (expected.Z, actual.Z, delta, out var dZ) &
			AreEqual (expected.W, actual.W, delta, out var dW))
			return;

		var diffString = $"[{dX}, {dY}, {dZ}, {dW}]";
		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}

	public static void AreEqual (SCNMatrix4 expected, SCNMatrix4 actual, string message)
	{
		if (AreEqual (expected.M11, actual.M11, out var d11) &
			AreEqual (expected.M21, actual.M21, out var d21) &
			AreEqual (expected.M31, actual.M31, out var d31) &
			AreEqual (expected.M41, actual.M41, out var d41) &
			AreEqual (expected.M12, actual.M12, out var d12) &
			AreEqual (expected.M22, actual.M22, out var d22) &
			AreEqual (expected.M32, actual.M32, out var d32) &
			AreEqual (expected.M42, actual.M42, out var d42) &
			AreEqual (expected.M13, actual.M13, out var d13) &
			AreEqual (expected.M23, actual.M23, out var d23) &
			AreEqual (expected.M33, actual.M33, out var d33) &
			AreEqual (expected.M43, actual.M43, out var d43) &
			AreEqual (expected.M14, actual.M14, out var d14) &
			AreEqual (expected.M24, actual.M24, out var d24) &
			AreEqual (expected.M34, actual.M34, out var d34) &
			AreEqual (expected.M44, actual.M44, out var d44)) {

			var size = Marshal.SizeOf<SCNMatrix4> ();
			unsafe {
				byte* e = (byte*) (void*) &expected;
				byte* a = (byte*) (void*) &actual;
				AreEqual (e, a, size, message);
			}
			return;
		}

		var actualString = actual.ToString ();

		var expectedString = expected.ToString ();

		var diffRow1 = $"({d11}, {d12}, {d13}, {d14})";
		var diffRow2 = $"({d21}, {d22}, {d23}, {d24})";
		var diffRow3 = $"({d31}, {d32}, {d33}, {d34})";
		var diffRow4 = $"({d41}, {d42}, {d43}, {d44})";
		var diffString = $"{diffRow1}\n{diffRow2}\n{diffRow3}\n{diffRow4}";

		var msg = $"{message}\nExpected:\n{expected}\nActual:\n{actual}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}

	public static void AreEqual (SCNMatrix4 expected, SCNMatrix4 actual, pfloat delta, string message)
	{
		if (AreEqual (expected.M11, actual.M11, delta, out var d11) &
			AreEqual (expected.M21, actual.M21, delta, out var d21) &
			AreEqual (expected.M31, actual.M31, delta, out var d31) &
			AreEqual (expected.M41, actual.M41, delta, out var d41) &
			AreEqual (expected.M12, actual.M12, delta, out var d12) &
			AreEqual (expected.M22, actual.M22, delta, out var d22) &
			AreEqual (expected.M32, actual.M32, delta, out var d32) &
			AreEqual (expected.M42, actual.M42, delta, out var d42) &
			AreEqual (expected.M13, actual.M13, delta, out var d13) &
			AreEqual (expected.M23, actual.M23, delta, out var d23) &
			AreEqual (expected.M33, actual.M33, delta, out var d33) &
			AreEqual (expected.M43, actual.M43, delta, out var d43) &
			AreEqual (expected.M14, actual.M14, delta, out var d14) &
			AreEqual (expected.M24, actual.M24, delta, out var d24) &
			AreEqual (expected.M34, actual.M34, delta, out var d34) &
			AreEqual (expected.M44, actual.M44, delta, out var d44))
			return;

		var actualString = actual.ToString ();
		var expectedString = expected.ToString ();

		var diffRow1 = $"({d11}, {d12}, {d13}, {d14})";
		var diffRow2 = $"({d21}, {d22}, {d23}, {d24})";
		var diffRow3 = $"({d31}, {d32}, {d33}, {d34})";
		var diffRow4 = $"({d41}, {d42}, {d43}, {d44})";
		var diffString = $"{diffRow1}\n{diffRow2}\n{diffRow3}\n{diffRow4}";

		var msg = $"{message}\nExpected:\n{expectedString}\nActual:\n{actualString}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}


	// The m## arguments correspond with the M## fields in SCNMatrix4
	// For .NET this means the first four values are the first column (and the first row for legacy Xamarin).
	public static void AreEqual (SCNMatrix4 actual, string message,
		pfloat m11, pfloat m12, pfloat m13, pfloat m14,
		pfloat m21, pfloat m22, pfloat m23, pfloat m24,
		pfloat m31, pfloat m32, pfloat m33, pfloat m34,
		pfloat m41, pfloat m42, pfloat m43, pfloat m44)
	{
		AreEqual (actual, message,
			m11, m12, m13, m14,
			m21, m22, m23, m24,
			m31, m32, m33, m34,
			m41, m42, m43, m44,
			delta: 0);
	}

	// The m## arguments correspond with the M## fields in SCNMatrix4
	// For .NET this means the first four values are the first column (and the first row for legacy Xamarin).
	public static void AreEqual (SCNMatrix4 actual, string message,
		pfloat m11, pfloat m12, pfloat m13, pfloat m14,
		pfloat m21, pfloat m22, pfloat m23, pfloat m24,
		pfloat m31, pfloat m32, pfloat m33, pfloat m34,
		pfloat m41, pfloat m42, pfloat m43, pfloat m44,
		pfloat delta
	)
	{
		if (AreEqual (m11, actual.M11, delta, out var d11) &
			AreEqual (m21, actual.M21, delta, out var d21) &
			AreEqual (m31, actual.M31, delta, out var d31) &
			AreEqual (m41, actual.M41, delta, out var d41) &
			AreEqual (m12, actual.M12, delta, out var d12) &
			AreEqual (m22, actual.M22, delta, out var d22) &
			AreEqual (m32, actual.M32, delta, out var d32) &
			AreEqual (m42, actual.M42, delta, out var d42) &
			AreEqual (m13, actual.M13, delta, out var d13) &
			AreEqual (m23, actual.M23, delta, out var d23) &
			AreEqual (m33, actual.M33, delta, out var d33) &
			AreEqual (m43, actual.M43, delta, out var d43) &
			AreEqual (m14, actual.M14, delta, out var d14) &
			AreEqual (m24, actual.M24, delta, out var d24) &
			AreEqual (m34, actual.M34, delta, out var d34) &
			AreEqual (m44, actual.M44, delta, out var d44))
			return;

		var actualString = actual.ToString ();

#if NET
		var row1 = $"({m11}, {m21}, {m31}, {m41})";
		var row2 = $"({m12}, {m22}, {m32}, {m42})";
		var row3 = $"({m13}, {m23}, {m33}, {m43})";
		var row4 = $"({m14}, {m24}, {m34}, {m44})";
#else
		var row1 = $"({m11}, {m12}, {m13}, {m14})";
		var row2 = $"({m21}, {m22}, {m23}, {m24})";
		var row3 = $"({m31}, {m32}, {m33}, {m34})";
		var row4 = $"({m41}, {m42}, {m43}, {m44})";
#endif
		var expectedString = $"{row1}\n{row2}\n{row3}\n{row4}";

		var diffRow1 = $"({d11}, {d12}, {d13}, {d14})";
		var diffRow2 = $"({d21}, {d22}, {d23}, {d24})";
		var diffRow3 = $"({d31}, {d32}, {d33}, {d34})";
		var diffRow4 = $"({d41}, {d42}, {d43}, {d44})";
		var diffString = $"{diffRow1}\n{diffRow2}\n{diffRow3}\n{diffRow4}";

		var msg = $"{message}\nExpected:\n{expectedString}\nActual:\n{actualString}\nDiff:\n{diffString}";
		Assert.Fail (msg);
	}
#endif // HAS_SCENEKIT

	static bool AreEqual (pfloat expected, pfloat actual, out string emojii)
	{
		return AreEqual (expected, actual, 0, out emojii);
	}

	// Use our own implementation to compare two floating point numbers with a tolerance, because
	// the NUnit version doesn't seem to work correctly in legacy Xamarin (older NUnit version?).
	static bool AreEqual (pfloat expected, pfloat actual, pfloat tolerance, out string emojii)
	{
		bool rv;

		if (pfloat.IsNaN (expected) && pfloat.IsNaN (actual)) {
			rv = true;
		} else if (pfloat.IsInfinity (expected) || pfloat.IsNaN (expected) || pfloat.IsNaN (actual)) {
			// Handle infinity specially since subtracting two infinite values gives 
			// NaN and the following test fails. mono also needs NaN to be handled
			// specially although ms.net could use either method. Also, handle
			// situation where no tolerance is used.
			rv = expected.Equals (actual);
		} else {
			rv = Math.Abs (expected - actual) <= tolerance;
		}

		emojii = rv ? "✅" : "❌";

		return rv;
	}

	public unsafe static void AreEqual (byte* expected, byte* actual, int length, string message)
	{
		// Check if the byte arrays are identical
		var equal = true;
		for (var i = 0; i < length; i++) {
			var e = expected [i];
			var a = actual [i];
			equal &= e == a;
		}
		if (equal)
			return;
		// They're not. Create the assertion message and assert.
		var e_sb = new StringBuilder ();
		var a_sb = new StringBuilder ();
		var d_sb = new StringBuilder ();
		for (var i = 0; i < length; i++) {
			var e = expected [i];
			var a = actual [i];
			e_sb.Append ($"0x{e:X2} ");
			a_sb.Append ($"0x{a:X2} ");
			if (e == a) {
				d_sb.Append ("     ");
			} else {
				d_sb.Append ("^^^^ ");
			}
		}
		Assert.Fail ($"{message}\nExpected: {e_sb}\nActual:   {a_sb}\n          {d_sb}");
	}

	public static void AreEqual (DateTime expected, DateTime actual, string message)
	{
		if (expected == actual)
			return;

		var diff = expected - actual;
		Assert.Fail ($"{message}\n\tExpected DateTime: {expected} (Ticks: {expected.Ticks})\n\tActual DateTime: {actual} (Ticks: {actual.Ticks})\n\tDifference is: {diff} = {diff.TotalMilliseconds} ms = {diff.TotalSeconds} s");
	}

	public static void AreEqual (DateTime expected, DateTime actual, TimeSpan tolerance, string message)
	{
		if (expected == actual)
			return;

		var diff = expected - actual;
		if (Math.Abs (diff.Ticks) < tolerance.Ticks)
			return;

		Assert.Fail ($"{message}\n\tExpected DateTime: {expected} (Ticks: {expected.Ticks})\n\tActual DateTime: {actual} (Ticks: {actual.Ticks})\n\tDifference is: {diff} = {diff.TotalMilliseconds} ms = {diff.TotalSeconds} s\n\tTolerance is: {tolerance} = {tolerance.TotalMilliseconds} ms = {tolerance.TotalSeconds} s");
	}
}
