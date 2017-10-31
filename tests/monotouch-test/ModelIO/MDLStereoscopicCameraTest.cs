//
// MDLStereoscopicCamera Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft Inc.
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
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
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
#if !__TVOS__
using MonoTouch.MultipeerConnectivity;
#endif
using MonoTouch.UIKit;
using MonoTouch.ModelIO;
using MonoTouch.ObjCRuntime;
#endif
using OpenTK;

using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
using VectorFloat3 = global::OpenTK.NVector3;

using Bindings.Test;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO
{
	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLStereoscopicCameraTest
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void Properties ()
		{
			using (var obj = new MDLStereoscopicCamera ()) {
				Assert.AreEqual (63f, obj.InterPupillaryDistance, "InterPupillaryDistance");
				Assert.AreEqual (0f, obj.LeftVergence, "LeftVergence");
				Assert.AreEqual (0f, obj.RightVergence, "RightVergence");
				Assert.AreEqual (0f, obj.Overlap, "Overlap");

				var mat1 = new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					-0.63f, 0, 0, 1);
				Asserts.AreEqual (mat1, obj.LeftViewMatrix, "LeftViewMatrix");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat1), obj.LeftViewMatrix4x4, "LeftViewMatrix4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat1), CFunctions.GetMatrixFloat4x4 (obj, "leftViewMatrix"), "LeftViewMatrix4x4 native");

				var mat2 = new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					0.63f, 0, 0, 1);
				Asserts.AreEqual (mat2, obj.RightViewMatrix, "RightViewMatrix");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat2), obj.RightViewMatrix4x4, "RightViewMatrix4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat2), CFunctions.GetMatrixFloat4x4 (obj, "rightViewMatrix"), "RightViewMatrix4x4 native");

				var mat3 = new Matrix4 (
					1.308407f, 0, 0, 0,
					0, 1.962611f, 0, 0,
					0, 0, -1.0002f, -1,
					0, 0, -0.20002f, 0);
				Asserts.AreEqual (mat3, obj.LeftProjectionMatrix, 0.0001f, "LeftProjectionMatrix");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat3), obj.LeftProjectionMatrix4x4, 0.0001f, "LeftProjectionMatrix4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat3), CFunctions.GetMatrixFloat4x4 (obj, "leftProjectionMatrix"), 0.0001f, "LeftProjectionMatrix4x4 native");

				Asserts.AreEqual (mat3, obj.RightProjectionMatrix, 0.0001f, "RightProjectionMatrix");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat3), obj.RightProjectionMatrix4x4, 0.0001f, "RightProjectionMatrix4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) mat3), CFunctions.GetMatrixFloat4x4 (obj, "rightProjectionMatrix"), 0.0001f, "RightProjectionMatrix4x4 native");
			}
		}
	}
}

#endif // !__WATCHOS__
