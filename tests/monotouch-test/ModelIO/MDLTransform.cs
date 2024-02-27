//
// MDLTransform Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;

using CoreGraphics;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;

#if NET
using System.Numerics;
using Matrix4 = global::System.Numerics.Matrix4x4;
using MatrixFloat2x2 = global::CoreGraphics.NMatrix2;
using MatrixFloat3x3 = global::CoreGraphics.NMatrix3;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
using VectorFloat3 = global::CoreGraphics.NVector3;
#else
using OpenTK;
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
using VectorFloat3 = global::OpenTK.NVector3;
#endif

using Bindings.Test;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLTransformTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0+ or macOS 10.11+");
		}

		[Test]
		public void Ctors ()
		{
#if NET
			var id = NMatrix4.Identity;
#else
			Matrix4 id = Matrix4.Identity;
#endif
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (id)) {
				Asserts.AreEqual (Vector3.Zero, obj.Translation, "Translation");
				Asserts.AreEqual (Vector3.One, obj.Scale, "Scale");
				Asserts.AreEqual (Vector3.Zero, obj.Rotation, "Rotation");
				Asserts.AreEqual (id, obj.Matrix, "Matrix");
				if (TestRuntime.CheckXcodeVersion (8, 0))
					Asserts.AreEqual (false, obj.ResetsTransform, "ResetsTransform");

				obj.Translation = V3;
				Asserts.AreEqual (V3, obj.Translation, "Translation 2");
			}

			if (TestRuntime.CheckXcodeVersion (8, 0)) {
				using (var obj = new MDLTransform (id, true)) {
					Asserts.AreEqual (Vector3.Zero, obj.Translation, "Translation");
					Asserts.AreEqual (Vector3.One, obj.Scale, "Scale");
					Asserts.AreEqual (Vector3.Zero, obj.Rotation, "Rotation");
					Asserts.AreEqual (id, obj.Matrix, "Matrix");
					Asserts.AreEqual (true, obj.ResetsTransform, "ResetsTransform");

					obj.Translation = V3;
					Asserts.AreEqual (V3, obj.Translation, "Translation 2");
				}
			}

			using (var obj = new MDLTransform (id)) {
				V3 *= 2;
				obj.Scale = V3;
				Asserts.AreEqual (V3, obj.Scale, "Scale 2");
			}

			using (var obj = new MDLTransform (id)) {
				V3 *= 2;
				obj.Rotation = V3;
				Asserts.AreEqual (V3, obj.Rotation, "Rotation 2");
			}

			using (var obj = new MDLTransform (id)) {
				V3 *= 2;
				obj.Rotation = V3;
				Asserts.AreEqual (V3, obj.Rotation, "Rotation 2");
			}

#if NET
			var m4 = new NMatrix4 (
				4, 0, 0, 2,
				0, 3, 0, 3,
				0, 0, 2, 4,
				0, 0, 0, 1);
#else
			var m4 = new Matrix4 (
				4, 0, 0, 0,
				0, 3, 0, 0,
				0, 0, 2, 0,
				2, 3, 4, 1);
#endif

			using (var obj = new MDLTransform (m4)) {
				Asserts.AreEqual (Vector3.Zero, obj.Rotation, "Rotation 3");
				Asserts.AreEqual (new Vector3 (4, 3, 2), obj.Scale, "Scale 3");
				Asserts.AreEqual (new Vector3 (2, 3, 4), obj.Translation, "Translation 3");
				Asserts.AreEqual (m4, obj.Matrix, 0.0001f, "Matrix 3");
			}

#if !NET
			var m4x4 = new MatrixFloat4x4 (
				4, 0, 0, 2,
				0, 3, 0, 3,
				0, 0, 2, 4,
				0, 0, 0, 1);
			using (var obj = new MDLTransform (m4x4)) {
				Asserts.AreEqual (Vector3.Zero, obj.Rotation, "Rotation 4");
				Asserts.AreEqual (new Vector3 (4, 3, 2), obj.Scale, "Scale 4");
				Asserts.AreEqual (new Vector3 (2, 3, 4), obj.Translation, "Translation 4");
#if NET
				Asserts.AreEqual (m4x4, obj.Matrix, 0.0001f, "Matrix4x4 4");
#else
				Asserts.AreEqual (m4x4, obj.GetMatrix4x4 (), 0.0001f, "Matrix4x4 4");
#endif
				Asserts.AreEqual (m4x4, CFunctions.GetMatrixFloat4x4 (obj, "matrix"), 0.0001f, "Matrix4x4-native 4");
			}
#endif
		}

		[Test]
		public void ScaleAtTimeTest ()
		{
#if NET
			var matrix = NMatrix4.Identity;
#else
			var matrix = Matrix4.Identity;
#endif
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetScale (V3, 0);
				Asserts.AreEqual (V3, obj.GetScale (0), "ScaleAtTime");
			}
		}

		[Test]
		public void TranslationAtTimeTest ()
		{
#if NET
			var matrix = NMatrix4.Identity;
#else
			var matrix = Matrix4.Identity;
#endif
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetTranslation (V3, 0);
				Asserts.AreEqual (V3, obj.GetTranslation (0), "TranslationAtTime");
			}
		}

		[Test]
		public void RotationAtTimeTest ()
		{
#if NET
			var matrix = NMatrix4.Identity;
#else
			var matrix = Matrix4.Identity;
#endif
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetRotation (V3, 0);
				Asserts.AreEqual (V3, obj.GetRotation (0), "RotationAtTime");
			}
		}

		[Test]
		public void GetRotationMatrixTest ()
		{
#if NET
			var matrix = NMatrix4.Identity;
#else
			var matrix = Matrix4.Identity;
#endif
			var V3 = new Vector3 (1, 0, 0);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetRotation (V3, 0);
				var expected = new MatrixFloat4x4 (
					1, 0, 0, 0,
					0, (float) Math.Cos (1.0f), (float) -Math.Sin (1.0f), 0,
					0, (float) Math.Sin (1.0f), (float) Math.Cos (1.0f), 0,
					0, 0, 0, 1
				);
#if NET
				Asserts.AreEqual (expected, obj.GetRotationMatrix (0), 0.00001f, "GetRotationMatrix");
#else
				Asserts.AreEqual ((Matrix4) MatrixFloat4x4.Transpose (expected), obj.GetRotationMatrix (0), 0.00001f, "GetRotationMatrix");
				Asserts.AreEqual (expected, obj.GetRotationMatrix4x4 (0), 0.00001f, "GetRotationMatrix4x4");
#endif
				Asserts.AreEqual (expected, CFunctions.MDLTransform_GetRotationMatrix (obj, 0), 0.00001f, "GetRotationMatrix4x4 native");
			}
		}
	}
}

#endif // !__WATCHOS__
