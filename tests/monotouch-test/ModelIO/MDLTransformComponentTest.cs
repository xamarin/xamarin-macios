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
using Matrix4 = global::CoreGraphics.NMatrix4;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using OpenTK;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

using Bindings.Test;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLTransformComponentTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void MatrixTest ()
		{
#if NET
			var m4 = new NMatrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
#else
			var m4 = new Matrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
			var m4x4 = (MatrixFloat4x4) m4;
#endif

			using (var obj = new MDLTransform ()) {
				// identity
				Asserts.AreEqual (Matrix4.Identity, obj.Matrix, "Initial identity");
#if NET
				Asserts.AreEqual (MatrixFloat4x4.Identity, obj.Matrix, "Initial identity 4x4");
#else
				Asserts.AreEqual (MatrixFloat4x4.Identity, obj.GetMatrix4x4 (), "Initial identity 4x4");
#endif
				Asserts.AreEqual (MatrixFloat4x4.Identity, CFunctions.GetMatrixFloat4x4 (obj, "matrix"), "Initial identity native");

				// translate the transform somewhere
				obj.SetTranslation (new Vector3 (2, 2, 2), 0);

				// the matrix should now be a translation matrix like this:
				//   1 0 0 0 2
				//   0 1 0 0 2
				//   0 0 1 0 2
				//   0 0 0 1 0
#if !NET
				// but since Matrix4 is transposed when compared to MatrixFloat4x4, we get this:

				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					2, 2, 2, 1
				), obj.Matrix, "Translated");

				// The 4x4 version is correct:
#endif
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 2,
					0, 1, 0, 2,
					0, 0, 1, 2,
					0, 0, 0, 1
#if NET
				), obj.Matrix, "Translated 4x4");
#else
				), obj.GetMatrix4x4 (), "Translated 4x4");
#endif

				// Let's set the matrix to something (different from the identity matrix)
				obj.Matrix = m4;

				// And the matrix resets to the identify matrix.
				Asserts.AreEqual (Matrix4.Identity, obj.Matrix, "After set_Matrix");

				// Translate again
				obj.SetTranslation (new Vector3 (3, 3, 3), 0);

				// Set the matrix using a 4x4 matrix
#if NET
				obj.Matrix = m4;
#else
				obj.SetMatrix4x4 (m4x4);
#endif

				// And we still get the identity matrix back
#if NET
				Asserts.AreEqual (MatrixFloat4x4.Identity, obj.Matrix, "After set_Matrix 2");
#else
				Asserts.AreEqual (MatrixFloat4x4.Identity, obj.GetMatrix4x4 (), "After set_Matrix 2");
#endif
			}
		}

		[Test]
		public void LocalTransformTest ()
		{
#if NET
			var m4 = new NMatrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
#else
			var m4 = new Matrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
			var m4x4 = (MatrixFloat4x4) m4;
#endif

			using (var obj = new MDLTransform ()) {
				var component = (IMDLTransformComponent) obj;
				// identity
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (0), "Initial identity");
#if NET
				Asserts.AreEqual (MatrixFloat4x4.Identity, component.GetLocalTransform (0), "Initial identity 4x4");
#else
				Asserts.AreEqual (MatrixFloat4x4.Identity, component.GetLocalTransform4x4 (0), "Initial identity 4x4");
#endif
				Asserts.AreEqual (MatrixFloat4x4.Identity, CFunctions.MDLTransformComponent_GetLocalTransform (component, 0), "Initial identity native");

				// translate the transform somewhere
				obj.SetTranslation (new Vector3 (2, 2, 2), 0);

				// the local transform should now be a translation matrix like this:
				//   1 0 0 0 2
				//   0 1 0 0 2
				//   0 0 1 0 2
				//   0 0 0 1 0
#if !NET
				// but since Matrix4 is transposed when compared to MatrixFloat4x4, we get this:

				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					2, 2, 2, 1
				), component.GetLocalTransform (0), "Translated");

				// The 4x4 version is correct:
#endif
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 2,
					0, 1, 0, 2,
					0, 0, 1, 2,
					0, 0, 0, 1
#if NET
				), component.GetLocalTransform (0), "Translated 4x4");
#else
				), component.GetLocalTransform4x4 (0), "Translated 4x4");
#endif

				// Let's set the local transform at time 1 to something (different from the identity matrix)
				component.SetLocalTransform (m4, 1);

				// At time 1 the transform matrix is now the identity matrix
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (1), "After SetLocalTransform at 1");

				// At time 0.5 we get a middle ground
#if NET
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 1,
					0, 1, 0, 1,
					0, 0, 1, 1,
					0, 0, 0, 1
#else
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					1, 1, 1, 1
#endif
				), component.GetLocalTransform (0.5), 0.00001f, "AfterSetLocalTransform at 0.5");

				// And at time 0 we still have the translated matrix.
#if NET
				Asserts.AreEqual (new NMatrix4 (
					1, 0, 0, 2,
					0, 1, 0, 2,
					0, 0, 1, 2,
					0, 0, 0, 1
				), component.GetLocalTransform (0), 0.00001f, "AfterSetLocalTransform at 0");
#else
				Asserts.AreEqual (new Matrix4 (
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					2, 2, 2, 1
				), component.GetLocalTransform (0), 0.00001f, "AfterSetLocalTransform at 0");
#endif

				// Let's set the local transform at all times
				component.SetLocalTransform (m4);

				// And we get the identity matrix back at all times
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (0), "Second identity at 0");
				Asserts.AreEqual (Matrix4.Identity, component.GetLocalTransform (1), "Second identity at 1");

				// Translate again
				obj.SetTranslation (new Vector3 (3, 3, 3), 0);

				// Set the local transform using a 4x4 matrix
#if NET
				component.SetLocalTransform (m4, 1);
#else
				component.SetLocalTransform4x4 (m4x4, 1);
#endif

				// And at time 0.5 we still get a middle ground
				// The numbers are different now because the translation matrix was different,
#if !NET
				// and the matrix is correct because we're checking the 4x4 version.
#endif
				Asserts.AreEqual (new MatrixFloat4x4 (
					1, 0, 0, 1.5f,
					0, 1, 0, 1.5f,
					0, 0, 1, 1.5f,
					0, 0, 0, 1
#if NET
				), component.GetLocalTransform (0.5), 0.00001f, "AfterSetLocalTransform4x4 at 0.5");
#else
				), component.GetLocalTransform4x4 (0.5), 0.00001f, "AfterSetLocalTransform4x4 at 0.5");
#endif
			}
		}

		[Test]
		public void CreateGlobalTransformTest ()
		{
#if NET
			NMatrix4 m4;
#else
			Matrix4 m4;
			MatrixFloat4x4 m4x4;
#endif

			using (var obj = new MDLObject ()) {
				m4 = MDLTransform.CreateGlobalTransform (obj, 0);
				Asserts.AreEqual ((Matrix4) MatrixFloat4x4.Transpose (CFunctions.MDLTransform_CreateGlobalTransform (obj, 0)), m4, "CreateGlobalTransform");

#if !NET
				m4x4 = MDLTransform.CreateGlobalTransform4x4 (obj, 0);
				Asserts.AreEqual (CFunctions.MDLTransform_CreateGlobalTransform (obj, 0), m4, "CreateGlobalTransform4x4");
#endif
			}
		}
	}
}

#endif // !__WATCHOS__
