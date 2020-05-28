using System;
#if !__WATCHOS__
using ModelIO;
using MetalPerformanceShaders;
#endif
using OpenTK;
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x3 = global::OpenTK.NMatrix4x3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
using VectorFloat3 = global::OpenTK.NVector3;
using MatrixDouble4x4 = global::OpenTK.NMatrix4d;
using VectorDouble3 = global::OpenTK.NVector3d;
using NUnit.Framework;

public static class Asserts
{
	public static void AreEqual (bool expected, bool actual, string message)
	{
		Assert.AreEqual (expected, actual, message + " (M)");
	}

	public static void AreEqual (float expected, float actual, string message)
	{
		Assert.AreEqual (expected, actual, message + " (M)");
	}

	public static void AreEqual (float expected, float actual, float delta, string message)
	{
		Assert.AreEqual (expected, actual, delta, message);
	}

	public static void AreEqual (Vector2 expected, Vector2 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
	}

	public static void AreEqual (Vector3 expected, Vector3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (Vector3 expected, Vector3 actual, float delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
	}

	public static void AreEqual (Vector3 expected, VectorFloat3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (VectorFloat3 expected, Vector3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (VectorFloat3 expected, VectorFloat3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (VectorFloat3 expected, VectorFloat3 actual, float delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
	}

	public static void AreEqual (Vector4 expected, Vector4 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
	}

	public static void AreEqual (Vector4 expected, Vector4 actual, float delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, delta, message + " (W)");
	}

	public static void AreEqual (Matrix2 expected, Matrix2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
	}

	public static void AreEqual (Matrix3 expected, Matrix3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R0C2, actual.R0C2, message + " (R0C2)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
		AreEqual (expected.R1C2, actual.R1C2, message + " (R1C2)");
		AreEqual (expected.R2C0, actual.R2C0, message + " (R2C0)");
		AreEqual (expected.R2C1, actual.R2C1, message + " (R2C1)");
		AreEqual (expected.R2C2, actual.R2C2, message + " (R2C2)");
	}

	public static void AreEqual (Matrix3 expected, Matrix3 actual, float delta, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, delta, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, delta, message + " (R0C1)");
		AreEqual (expected.R0C2, actual.R0C2, delta, message + " (R0C2)");
		AreEqual (expected.R1C0, actual.R1C0, delta, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, delta, message + " (R1C1)");
		AreEqual (expected.R1C2, actual.R1C2, delta, message + " (R1C2)");
		AreEqual (expected.R2C0, actual.R2C0, delta, message + " (R2C0)");
		AreEqual (expected.R2C1, actual.R2C1, delta, message + " (R2C1)");
		AreEqual (expected.R2C2, actual.R2C2, delta, message + " (R2C2)");
	}

	public static void AreEqual (Matrix4 expected, Matrix4 actual, string message)
	{
		AreEqual (expected.Column0, actual.Column0, message + " (Col0)");
		AreEqual (expected.Column1, actual.Column1, message + " (Col1)");
		AreEqual (expected.Column2, actual.Column2, message + " (Col2)");
		AreEqual (expected.Column3, actual.Column3, message + " (Col3)");
	}

	public static void AreEqual (Matrix4 expected, Matrix4 actual, float delta, string message)
	{
		AreEqual (expected.Column0, actual.Column0, delta, message + " (Col0)");
		AreEqual (expected.Column1, actual.Column1, delta, message + " (Col1)");
		AreEqual (expected.Column2, actual.Column2, delta, message + " (Col2)");
		AreEqual (expected.Column3, actual.Column3, delta, message + " (Col3)");
	}

	public static void AreEqual (Vector2i expected, Vector2i actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
	}

	public static void AreEqual (Vector4i expected, Vector4i actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
	}

#if !__WATCHOS__
	public static void AreEqual (MDLAxisAlignedBoundingBox expected, MDLAxisAlignedBoundingBox actual, string message)
	{
		AreEqual (expected.MaxBounds, actual.MaxBounds, message + " (MaxBounds)");
		AreEqual (expected.MinBounds, actual.MinBounds, message + " (MinBounds)");
	}
#endif // !__WATCHOS__

	public static void AreEqual (Quaternion expected, Quaternion actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
	}

	public static void AreEqual (Quaternion [] expected, Quaternion [] actual, string message)
	{
		if (expected == null) {
			if (actual == null)
				return;
			Assert.Fail ($"Expected null, got {actual}. {message}");
		} else if (actual == null) {
			Assert.Fail ($"Expected {expected}, got null. {message}");
		}

		Assert.AreEqual (expected.Length, actual.Length, message + " array lengths");
		for (var i = 0; i < expected.Length; i++) {
			AreEqual (expected [i], actual [i], message + $" [{i}]");
		}
	}

	public static void AreEqual (Quaterniond expected, Quaterniond actual, string message)
	{
		if (expected == null) {
			if (actual == null)
				return;
			Assert.Fail ($"Expected null, got {actual}. {message}");
		} else if (actual == null) {
			Assert.Fail ($"Expected {expected}, got null. {message}");
		}

		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
	}

	public static void AreEqual (Quaterniond expected, Quaterniond actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, delta, message + " (W)");
	}

	public static void AreEqual (Quaterniond [] expected, Quaterniond [] actual, string message)
	{
		Assert.AreEqual (expected.Length, actual.Length, message + " array lengths");
		for (var i = 0; i < expected.Length; i++) {
			AreEqual (expected [i], actual [i], message + $" [{i}]");
		}
	}

#if !__WATCHOS__
	public static void AreEqual (MPSImageHistogramInfo expected, MPSImageHistogramInfo actual, string message)
	{
		Assert.AreEqual (expected.HistogramForAlpha, actual.HistogramForAlpha, message + " HistogramForAlpha");
		Asserts.AreEqual (expected.MaxPixelValue, actual.MaxPixelValue, message + " MaxPixelValue");
		Asserts.AreEqual (expected.MinPixelValue, actual.MinPixelValue, message + " MinPixelValue");
		Assert.AreEqual (expected.NumberOfHistogramEntries, actual.NumberOfHistogramEntries, message + " NumberOfHistogramEntries");
	}
#endif // !__WATCHOS__

	public static void AreEqual (MatrixFloat2x2 expected, MatrixFloat2x2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
	}

	public static void AreEqual (MatrixFloat2x2 expected, MatrixFloat2x2 actual, float delta, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, delta, message + " (R0C0)");
		AreEqual (expected.R1C0, actual.R1C0, delta, message + " (R1C0)");
		AreEqual (expected.R0C1, actual.R0C1, delta, message + " (R0C1)");
		AreEqual (expected.R1C1, actual.R1C1, delta, message + " (R1C1)");
	}

	public static void AreEqual (Matrix2 expected, MatrixFloat2x2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
	}

	public static void AreEqual (MatrixFloat2x2 expected, Matrix2 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
	}

	public static void AreEqual (MatrixFloat3x3 expected, MatrixFloat3x3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R2C0, actual.R2C0, message + " (R2C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
		AreEqual (expected.R2C1, actual.R2C1, message + " (R2C1)");
		AreEqual (expected.R0C2, actual.R0C2, message + " (R0C2)");
		AreEqual (expected.R1C2, actual.R1C2, message + " (R1C2)");
		AreEqual (expected.R2C2, actual.R2C2, message + " (R2C2)");
	}

	public static void AreEqual (MatrixFloat3x3 expected, MatrixFloat3x3 actual, float delta, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, delta, message + " (R0C0)");
		AreEqual (expected.R1C0, actual.R1C0, delta, message + " (R1C0)");
		AreEqual (expected.R2C0, actual.R2C0, delta, message + " (R2C0)");
		AreEqual (expected.R0C1, actual.R0C1, delta, message + " (R0C1)");
		AreEqual (expected.R1C1, actual.R1C1, delta, message + " (R1C1)");
		AreEqual (expected.R2C1, actual.R2C1, delta, message + " (R2C1)");
		AreEqual (expected.R0C2, actual.R0C2, delta, message + " (R0C2)");
		AreEqual (expected.R1C2, actual.R1C2, delta, message + " (R1C2)");
		AreEqual (expected.R2C2, actual.R2C2, delta, message + " (R2C2)");
	}

	public static void AreEqual (Matrix3 expected, MatrixFloat3x3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R0C2, actual.R0C2, message + " (R0C2)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
		AreEqual (expected.R1C2, actual.R1C2, message + " (R1C2)");
		AreEqual (expected.R2C0, actual.R2C0, message + " (R2C0)");
		AreEqual (expected.R2C1, actual.R2C1, message + " (R2C1)");
		AreEqual (expected.R2C2, actual.R2C2, message + " (R2C2)");
	}

	public static void AreEqual (MatrixFloat3x3 expected, Matrix3 actual, string message)
	{
		AreEqual (expected.R0C0, actual.R0C0, message + " (R0C0)");
		AreEqual (expected.R0C1, actual.R0C1, message + " (R0C1)");
		AreEqual (expected.R0C2, actual.R0C2, message + " (R0C2)");
		AreEqual (expected.R1C0, actual.R1C0, message + " (R1C0)");
		AreEqual (expected.R1C1, actual.R1C1, message + " (R1C1)");
		AreEqual (expected.R1C2, actual.R1C2, message + " (R1C2)");
		AreEqual (expected.R2C0, actual.R2C0, message + " (R2C0)");
		AreEqual (expected.R2C1, actual.R2C1, message + " (R2C1)");
		AreEqual (expected.R2C2, actual.R2C2, message + " (R2C2)");
	}

	public static void AreEqual (MatrixFloat4x4 expected, MatrixFloat4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M41, actual.M41, message + " (M41)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M42, actual.M42, message + " (M42)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M43, actual.M43, message + " (M43)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
		AreEqual (expected.M44, actual.M44, message + " (M44)");
	}

	public static void AreEqual (MatrixFloat4x4 expected, MatrixFloat4x4 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, message + " (M11)");
		AreEqual (expected.M21, actual.M21, delta, message + " (M21)");
		AreEqual (expected.M31, actual.M31, delta, message + " (M31)");
		AreEqual (expected.M41, actual.M41, delta, message + " (M41)");
		AreEqual (expected.M12, actual.M12, delta, message + " (M12)");
		AreEqual (expected.M22, actual.M22, delta, message + " (M22)");
		AreEqual (expected.M32, actual.M32, delta, message + " (M32)");
		AreEqual (expected.M42, actual.M42, delta, message + " (M42)");
		AreEqual (expected.M13, actual.M13, delta, message + " (M13)");
		AreEqual (expected.M23, actual.M23, delta, message + " (M23)");
		AreEqual (expected.M33, actual.M33, delta, message + " (M33)");
		AreEqual (expected.M43, actual.M43, delta, message + " (M43)");
		AreEqual (expected.M14, actual.M14, delta, message + " (M14)");
		AreEqual (expected.M24, actual.M24, delta, message + " (M24)");
		AreEqual (expected.M34, actual.M34, delta, message + " (M34)");
		AreEqual (expected.M44, actual.M44, delta, message + " (M44)");
	}

	public static void AreEqual (Matrix4 expected, MatrixFloat4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M41, actual.M41, message + " (M41)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M42, actual.M42, message + " (M42)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M43, actual.M43, message + " (M43)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
		AreEqual (expected.M44, actual.M44, message + " (M44)");
	}

	public static void AreEqual (Matrix4 expected, MatrixFloat4x4 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, message + " (M11)");
		AreEqual (expected.M21, actual.M21, delta, message + " (M21)");
		AreEqual (expected.M31, actual.M31, delta, message + " (M31)");
		AreEqual (expected.M41, actual.M41, delta, message + " (M41)");
		AreEqual (expected.M12, actual.M12, delta, message + " (M12)");
		AreEqual (expected.M22, actual.M22, delta, message + " (M22)");
		AreEqual (expected.M32, actual.M32, delta, message + " (M32)");
		AreEqual (expected.M42, actual.M42, delta, message + " (M42)");
		AreEqual (expected.M13, actual.M13, delta, message + " (M13)");
		AreEqual (expected.M23, actual.M23, delta, message + " (M23)");
		AreEqual (expected.M33, actual.M33, delta, message + " (M33)");
		AreEqual (expected.M43, actual.M43, delta, message + " (M43)");
		AreEqual (expected.M14, actual.M14, delta, message + " (M14)");
		AreEqual (expected.M24, actual.M24, delta, message + " (M24)");
		AreEqual (expected.M34, actual.M34, delta, message + " (M34)");
		AreEqual (expected.M44, actual.M44, delta, message + " (M44)");
	}

	public static void AreEqual (MatrixFloat4x4 expected, Matrix4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M41, actual.M41, message + " (M41)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M42, actual.M42, message + " (M42)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M43, actual.M43, message + " (M43)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
		AreEqual (expected.M44, actual.M44, message + " (M44)");
	}

#region Double Based Types
	public static void AreEqual (double expected, double actual, string message)
	{
		Assert.AreEqual (expected, actual, message + " (M)");
	}

	public static void AreEqual (double expected, double actual, double delta, string message)
	{
		Assert.AreEqual (expected, actual, delta, message);
	}

	public static void AreEqual (Vector2d expected, Vector2d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
	}

	public static void AreEqual (Vector3d expected, Vector3d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (Vector3d expected, Vector3d actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
	}

	public static void AreEqual (Vector3d expected, VectorDouble3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (VectorDouble3 expected, Vector3d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (VectorDouble3 expected, VectorDouble3 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, 0.001, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, 0.001, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, 0.001, message + " (Z)");
	}

	public static void AreEqual (VectorDouble3 expected, VectorDouble3 actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
	}

	public static void AreEqual (Vector4d expected, Vector4d actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
	}

	public static void AreEqual (Vector4d expected, Vector4d actual, double delta, string message)
	{
		Assert.AreEqual (expected.X, actual.X, delta, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, delta, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, delta, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, delta, message + " (W)");
	}

	public static void AreEqual (Matrix4d expected, Matrix4d actual, string message)
	{
		AreEqual (expected.Column0, actual.Column0, message + " (Col0)");
		AreEqual (expected.Column1, actual.Column1, message + " (Col1)");
		AreEqual (expected.Column2, actual.Column2, message + " (Col2)");
		AreEqual (expected.Column3, actual.Column3, message + " (Col3)");
	}

	public static void AreEqual (Matrix4d expected, Matrix4d actual, double delta, string message)
	{
		AreEqual (expected.Column0, actual.Column0, delta, message + " (Col0)");
		AreEqual (expected.Column1, actual.Column1, delta, message + " (Col1)");
		AreEqual (expected.Column2, actual.Column2, delta, message + " (Col2)");
		AreEqual (expected.Column3, actual.Column3, delta, message + " (Col3)");
	}

	public static void AreEqual (MatrixDouble4x4 expected, MatrixDouble4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M41, actual.M41, message + " (M41)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M42, actual.M42, message + " (M42)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M43, actual.M43, message + " (M43)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
		AreEqual (expected.M44, actual.M44, message + " (M44)");
	}

	public static void AreEqual (MatrixDouble4x4 expected, MatrixDouble4x4 actual, double delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, message + " (M11)");
		AreEqual (expected.M21, actual.M21, delta, message + " (M21)");
		AreEqual (expected.M31, actual.M31, delta, message + " (M31)");
		AreEqual (expected.M41, actual.M41, delta, message + " (M41)");
		AreEqual (expected.M12, actual.M12, delta, message + " (M12)");
		AreEqual (expected.M22, actual.M22, delta, message + " (M22)");
		AreEqual (expected.M32, actual.M32, delta, message + " (M32)");
		AreEqual (expected.M42, actual.M42, delta, message + " (M42)");
		AreEqual (expected.M13, actual.M13, delta, message + " (M13)");
		AreEqual (expected.M23, actual.M23, delta, message + " (M23)");
		AreEqual (expected.M33, actual.M33, delta, message + " (M33)");
		AreEqual (expected.M43, actual.M43, delta, message + " (M43)");
		AreEqual (expected.M14, actual.M14, delta, message + " (M14)");
		AreEqual (expected.M24, actual.M24, delta, message + " (M24)");
		AreEqual (expected.M34, actual.M34, delta, message + " (M34)");
		AreEqual (expected.M44, actual.M44, delta, message + " (M44)");
	}

	public static void AreEqual (Matrix4d expected, MatrixDouble4x4 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M41, actual.M41, message + " (M41)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M42, actual.M42, message + " (M42)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M43, actual.M43, message + " (M43)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
		AreEqual (expected.M44, actual.M44, message + " (M44)");
	}

	public static void AreEqual (Matrix4d expected, NMatrix4d actual, double delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, message + " (M11)");
		AreEqual (expected.M21, actual.M21, delta, message + " (M21)");
		AreEqual (expected.M31, actual.M31, delta, message + " (M31)");
		AreEqual (expected.M41, actual.M41, delta, message + " (M41)");
		AreEqual (expected.M12, actual.M12, delta, message + " (M12)");
		AreEqual (expected.M22, actual.M22, delta, message + " (M22)");
		AreEqual (expected.M32, actual.M32, delta, message + " (M32)");
		AreEqual (expected.M42, actual.M42, delta, message + " (M42)");
		AreEqual (expected.M13, actual.M13, delta, message + " (M13)");
		AreEqual (expected.M23, actual.M23, delta, message + " (M23)");
		AreEqual (expected.M33, actual.M33, delta, message + " (M33)");
		AreEqual (expected.M43, actual.M43, delta, message + " (M43)");
		AreEqual (expected.M14, actual.M14, delta, message + " (M14)");
		AreEqual (expected.M24, actual.M24, delta, message + " (M24)");
		AreEqual (expected.M34, actual.M34, delta, message + " (M34)");
		AreEqual (expected.M44, actual.M44, delta, message + " (M44)");
	}

	public static void AreEqual (NMatrix4x3 expected, NMatrix4x3 actual, float delta, string message)
	{
		AreEqual (expected.M11, actual.M11, delta, message + " (M11)");
		AreEqual (expected.M21, actual.M21, delta, message + " (M21)");
		AreEqual (expected.M31, actual.M31, delta, message + " (M31)");
		AreEqual (expected.M12, actual.M12, delta, message + " (M12)");
		AreEqual (expected.M22, actual.M22, delta, message + " (M22)");
		AreEqual (expected.M32, actual.M32, delta, message + " (M32)");
		AreEqual (expected.M13, actual.M13, delta, message + " (M13)");
		AreEqual (expected.M23, actual.M23, delta, message + " (M23)");
		AreEqual (expected.M33, actual.M33, delta, message + " (M33)");
		AreEqual (expected.M14, actual.M14, delta, message + " (M14)");
		AreEqual (expected.M24, actual.M24, delta, message + " (M24)");
		AreEqual (expected.M34, actual.M34, delta, message + " (M34)");
	}

	public static void AreEqual (NMatrix4d expected, Matrix4d actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M41, actual.M41, message + " (M41)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M42, actual.M42, message + " (M42)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M43, actual.M43, message + " (M43)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
		AreEqual (expected.M44, actual.M44, message + " (M44)");
	}

	public static void AreEqual (NMatrix4x3 expected, NMatrix4x3 actual, string message)
	{
		AreEqual (expected.M11, actual.M11, message + " (M11)");
		AreEqual (expected.M21, actual.M21, message + " (M21)");
		AreEqual (expected.M31, actual.M31, message + " (M31)");
		AreEqual (expected.M12, actual.M12, message + " (M12)");
		AreEqual (expected.M22, actual.M22, message + " (M22)");
		AreEqual (expected.M32, actual.M32, message + " (M32)");
		AreEqual (expected.M13, actual.M13, message + " (M13)");
		AreEqual (expected.M23, actual.M23, message + " (M23)");
		AreEqual (expected.M33, actual.M33, message + " (M33)");
		AreEqual (expected.M14, actual.M14, message + " (M14)");
		AreEqual (expected.M24, actual.M24, message + " (M24)");
		AreEqual (expected.M34, actual.M34, message + " (M34)");
	}
#endregion
}

