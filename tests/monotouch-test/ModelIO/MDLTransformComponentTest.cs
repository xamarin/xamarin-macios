//
// MDLTransformComponent Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft inc
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
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
	public class MDLTransformComponentTest
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void MatrixTest ()
		{
			var m4 = new Matrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
			var m4x4 = (MatrixFloat4x4) m4;

			using (var obj = new MDLTransform ()) {
				// identity
				Asserts.AreEqual (Matrix4.Identity, obj.Matrix, "Initial identity");
				Asserts.AreEqual (MatrixFloat4x4.Identity, obj.GetMatrix4x4 (), "Initial identity 4x4");
				Asserts.AreEqual (MatrixFloat4x4.Identity, CFunctions.GetMatrixFloat4x4 (obj, "matrix"), "Initial identity native");

				// translate the transform somewhere
				obj.SetTranslation (new Vector3 (2, 2, 2), 0);

				// the matrix should now be a translation matrix like this:
				//   1 0 0 0 2
				//   0 1 0 0 2
				//   0 0 1 0 2
				//   0 0 0 1 0
				// but since Matrix4 is transposed when compared to MatrixFloat4x4, we get this:

				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					2, 2, 2, 1
				), obj.Matrix, "Translated");

				// The 4x4 version is correct:
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 2,
					0, 1, 0, 2,
					0, 0, 1, 2,
					0, 0, 0, 1
				), obj.GetMatrix4x4 (), "Translated 4x4");

				// Let's set the matrix to something (different from the identity matrix)
				obj.Matrix = m4;

				// And the matrix resets to the identify matrix.
				Asserts.AreEqual (Matrix4.Identity, obj.Matrix, "After set_Matrix");

				// Translate again
				obj.SetTranslation (new Vector3 (3, 3, 3), 0);

				// Set the matrix using a 4x4 matrix
				obj.SetMatrix4x4 (m4x4);

				// And we still get the identity matrix back
				Asserts.AreEqual (MatrixFloat4x4.Identity, obj.GetMatrix4x4 (), "After set_Matrix 2");
			}
		}

		[Test]
		public void LocalTransformTest ()
		{
			var m4 = new Matrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
			var m4x4 = (MatrixFloat4x4) m4;

			using (var obj = new MDLTransform ()) {
				var component = (IMDLTransformComponent) obj;
				// identity
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (0), "Initial identity");
				Asserts.AreEqual (MatrixFloat4x4.Identity, component.GetLocalTransform4x4 (0), "Initial identity 4x4");
				Asserts.AreEqual (MatrixFloat4x4.Identity, CFunctions.MDLTransformComponent_GetLocalTransform  (component, 0), "Initial identity native");

				// translate the transform somewhere
				obj.SetTranslation (new Vector3 (2, 2, 2), 0);

				// the local transform should now be a translation matrix like this:
				//   1 0 0 0 2
				//   0 1 0 0 2
				//   0 0 1 0 2
				//   0 0 0 1 0
				// but since Matrix4 is transposed when compared to MatrixFloat4x4, we get this:

				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					2, 2, 2, 1
				), component.GetLocalTransform (0), "Translated");

				// The 4x4 version is correct:
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 2,
					0, 1, 0, 2,
					0, 0, 1, 2,
					0, 0, 0, 1
				), component.GetLocalTransform4x4 (0), "Translated 4x4");

				// Let's set the local transform at time 1 to something (different from the identity matrix)
				component.SetLocalTransform (m4, 1);

				// At time 1 the transform matrix is now the identity matrix
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (1), "After SetLocalTransform at 1");

				// At time 0.5 we get a middle ground
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					1, 1, 1, 1
				), component.GetLocalTransform (0.5), 0.00001f, "AfterSetLocalTransform at 0.5");

				// And at time 0 we still have the translated matrix.
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					2, 2, 2, 1
				), component.GetLocalTransform (0), 0.00001f, "AfterSetLocalTransform at 0");

				// Let's set the local transform at all times
				component.SetLocalTransform (m4);

				// And we get the identity matrix back at all times
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (0), "Second identity at 0");
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (1), "Second identity at 1");

				// Translate again
				obj.SetTranslation (new Vector3 (3, 3, 3), 0);

				// Set the local transform using a 4x4 matrix
				component.SetLocalTransform4x4 (m4x4, 1);

				// And at time 0.5 we still get a middle ground
				// The numbers are different now because the translation matrix was different,
				// and the matrix is correct because we're checking the 4x4 version.
				Asserts.AreEqual (new MatrixFloat4x4 (
					1, 0, 0, 1.5f,
					0, 1, 0, 1.5f,
					0, 0, 1, 1.5f,
					0, 0, 0, 1
				), component.GetLocalTransform4x4 (0.5), 0.00001f, "AfterSetLocalTransform4x4 at 0.5");
			}
		}

		[Test]
		public void CreateGlobalTransformTest ()
		{
			Matrix4 m4;
			MatrixFloat4x4 m4x4;

			using (var obj = new MDLObject ()) {
				m4 = MDLTransform.CreateGlobalTransform (obj, 0);
				Asserts.AreEqual ((Matrix4) MatrixFloat4x4.Transpose (CFunctions.MDLTransform_CreateGlobalTransform (obj, 0)), m4, "CreateGlobalTransform");

				m4x4 = MDLTransform.CreateGlobalTransform4x4 (obj, 0);
				Asserts.AreEqual (CFunctions.MDLTransform_CreateGlobalTransform (obj, 0), m4, "CreateGlobalTransform4x4");
			}
		}
	}
}

#endif // !__WATCHOS__
