#if !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
using SpriteKit;
using ObjCRuntime;

#if NET
using System.Numerics;
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

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SKTransformNodeTest {
		[SetUp]
		public void VersionCheck ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void EulerAngles ()
		{
			VectorFloat3 V3 = new VectorFloat3 ();

			using (var obj = new SKTransformNode ()) {
				Asserts.AreEqual (V3, obj.EulerAngles, "1 EulerAngles");
				V3 = new VectorFloat3 (1, 2, 3);
				obj.EulerAngles = V3;
				// The values bellow match what the same code in Swift returns.
				Assert.AreEqual (-2.14159298f, obj.EulerAngles.X, 0.000001f, "#x1");
				Assert.AreEqual (1.14159274f, obj.EulerAngles.Y, 0.000001f, "#y1");
				Assert.AreEqual (-0.141592711f, obj.EulerAngles.Z, 0.000001f, "#z1");
			}
		}

		[Test]
		public void RotationMatrix ()
		{
			using (var obj = new SKTransformNode ()) {
				var zero = new MatrixFloat3x3 ();
				obj.RotationMatrix = zero;
				// In Swift, a rotated zero matrice also becomes the identity matrice.
				Asserts.AreEqual (MatrixFloat3x3.Identity, obj.RotationMatrix, "RotationMatrix");
				// Changing XRotation (or YRotation for that matter), makes the RotationMatrix change too
				obj.XRotation = (nfloat) (Math.PI / 2);
				var rotatedMatrix = new MatrixFloat3x3 (
					1, 0, 0,
					0, 0, -1,
					0, 1, 0
				);
				Asserts.AreEqual (rotatedMatrix, obj.RotationMatrix, 0.000001f, "RotationMatrix a");
				Asserts.AreEqual (rotatedMatrix, CFunctions.GetMatrixFloat3x3 (obj, "rotationMatrix"), 0.000001f, "RotationMatrix native a");

				// Got this matrix after setting both XRotation and YRotation to Pi/2
				rotatedMatrix = new MatrixFloat3x3 (
					0, 1, 0,
					0, 0, -1,
					-1, 0, 0
				);
				obj.RotationMatrix = rotatedMatrix;
				Asserts.AreEqual (rotatedMatrix, obj.RotationMatrix, 0.000001f, "RotationMatrix b");
				Assert.AreEqual ((nfloat) (Math.PI / 2), obj.XRotation, 0.000001f, "XRotation b");
				Assert.AreEqual (0, obj.YRotation, 0.000001f, "YRotation b"); // Setting YRotation changes RotationMatrix, but setting RotationMatrix doesn't change YRotation.
			}
		}

		[Test]
		public void QuaternionTest ()
		{
			Quaternion Q;

			using (var obj = new SKTransformNode ()) {
				Asserts.AreEqual (Quaternion.Identity, obj.Quaternion, "1 Quaternion");
				Q = new Quaternion (new Vector3 (1, 2, 3), 4);
				obj.Quaternion = Q;
				Asserts.AreEqual (Q, obj.Quaternion, "2 Quaternion");
			}
		}
	}
}

#endif // !__WATCHOS__
