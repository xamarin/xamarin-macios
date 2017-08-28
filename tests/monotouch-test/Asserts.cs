using System;
#if XAMCORE_2_0
#if !__WATCHOS__
using ModelIO;
using MetalPerformanceShaders;
#endif
#else
using MonoTouch.ModelIO;
using MonoTouch.MetalPerformanceShaders;
#endif
using OpenTK;
using NUnit.Framework;

public static class Asserts
{
#if !__WATCHOS__
	public static void AreEqual (bool expected, bool actual, string message)
	{
		Assert.AreEqual (expected, actual, message + " (M)");
	}

	public static void AreEqual (float expected, float actual, string message)
	{
		Assert.AreEqual (expected, actual, message + " (M)");
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

	public static void AreEqual (Vector4 expected, Vector4 actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
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

	public static void AreEqual (Matrix4 expected, Matrix4 actual, string message)
	{
		AreEqual (expected.Column0, actual.Column0, message + " (Col0)");
		AreEqual (expected.Column1, actual.Column1, message + " (Col1)");
		AreEqual (expected.Column2, actual.Column2, message + " (Col2)");
		AreEqual (expected.Column3, actual.Column3, message + " (Col3)");
	}

	public static void AreEqual (Vector2i expected, Vector2i actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
	}

	public static void AreEqual (MDLAxisAlignedBoundingBox expected, MDLAxisAlignedBoundingBox actual, string message)
	{
		AreEqual (expected.MaxBounds, actual.MaxBounds, message + " (MaxBounds)");
		AreEqual (expected.MinBounds, actual.MinBounds, message + " (MinBounds)");
	}

	public static void AreEqual (Quaternion expected, Quaternion actual, string message)
	{
		Assert.AreEqual (expected.X, actual.X, message + " (X)");
		Assert.AreEqual (expected.Y, actual.Y, message + " (Y)");
		Assert.AreEqual (expected.Z, actual.Z, message + " (Z)");
		Assert.AreEqual (expected.W, actual.W, message + " (W)");
	}

	public static void AreEqual (MPSImageHistogramInfo expected, MPSImageHistogramInfo actual, string message)
	{
		Assert.AreEqual (expected.HistogramForAlpha, actual.HistogramForAlpha, message + " HistogramForAlpha");
		Asserts.AreEqual (expected.MaxPixelValue, actual.MaxPixelValue, message + " MaxPixelValue");
		Asserts.AreEqual (expected.MinPixelValue, actual.MinPixelValue, message + " MinPixelValue");
		Assert.AreEqual (expected.NumberOfHistogramEntries, actual.NumberOfHistogramEntries, message + " NumberOfHistogramEntries");

	}
#endif // !__WATCHOS__
}

