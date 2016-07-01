using System;
#if XAMCORE_2_0
#if !__WATCHOS__
using ModelIO;
#endif
#else
using MonoTouch.ModelIO;
#endif
using OpenTK;
using NUnit.Framework;

public static class Asserts
{
#if !__WATCHOS__
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
#endif // !__WATCHOS__
}

