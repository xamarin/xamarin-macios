//
// MDLCamera Unit Tests
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
	public class MDCameraTest
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void ProjectionMatrix ()
		{
			using (var obj = new MDLCamera ()) {
				Assert.AreEqual (0.1f, obj.NearVisibilityDistance, 0.0001f, "NearVisibilityDistance");
				Assert.AreEqual (1000f, obj.FarVisibilityDistance, 0.0001f, "FarVisibilityDistance");
				Assert.AreEqual (54f, obj.FieldOfView, 0.0001f, "FieldOfView");
				var initialProjectionMatrix = new Matrix4 (
					1.308407f, 0, 0, 0,
					0, 1.962611f, 0, 0,
					0, 0, -1.0002f, -1,
					0, 0, -0.20002f, 0
				);
				Asserts.AreEqual (initialProjectionMatrix, obj.ProjectionMatrix, 0.0001f, "Initial");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) initialProjectionMatrix), obj.ProjectionMatrix4x4, 0.0001f, "Initial 4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) initialProjectionMatrix), CFunctions.GetMatrixFloat4x4 (obj, "projectionMatrix"), 0.0001f, "Initial native");

				obj.NearVisibilityDistance = 1.0f;
				var modifiedProjectionMatrix = new Matrix4 (
					1.308407f, 0, 0, 0,
					0, 1.962611f, 0, 0,
					0, 0, -1.002002f, -1,
					0, 0, -2.002002f, 0
				);
				Asserts.AreEqual (modifiedProjectionMatrix, obj.ProjectionMatrix, 0.0001f, "Second");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) modifiedProjectionMatrix), obj.ProjectionMatrix4x4, 0.0001f, "Second 4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) modifiedProjectionMatrix), CFunctions.GetMatrixFloat4x4 (obj, "projectionMatrix"), 0.0001f, "Second native");
			}
		}
	}
}

#endif // !__WATCHOS__
