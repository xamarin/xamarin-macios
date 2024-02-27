//
// Unit tests for GKAgent3D
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//	
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using Foundation;
using GameplayKit;

#if NET
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

namespace MonoTouchFixtures.GamePlayKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKAgent3DTest {
		[SetUp]
		public void Setup ()
		{
			// Headers and documentation say this was introduced in iOS 9.
			// My iOS 9.3.5 device doesn't agree:
			// > dyld: Symbol not found: _OBJC_CLASS_$_GKAgent3D
			// Apple's iOS 9 -> iOS 10 diff also says this class was introduced in iOS 10.
			TestRuntime.AssertXcodeVersion (8, 0);
		}

#if !NET
		[Test]
		public void RotationTest ()
		{
			using (var obj = new GKAgent3D ()) {
#if NET
				var initial = new MatrixFloat3x3 (0, 0, 1,
												  0, 1, 0,
												  1, 0, 0);
#else
				var initial = new Matrix3 (0, 0, 1,
										   0, 1, 0,
										   1, 0, 0);
#endif
				Asserts.AreEqual (initial, obj.Rotation, "Rotation");
#if !NET
				Asserts.AreEqual ((MatrixFloat3x3) initial, obj.Rotation3x3, "Rotation3x3");
#endif

#if !NET
				var mat = new Matrix3 (1, 2, 3,
									   4, 5, 6,
									   7, 8, 9);
#endif
				var mat3x3 = new MatrixFloat3x3 (1, 2, 3,
												 4, 5, 6,
												 7, 8, 9); ;

#if !NET
				obj.Rotation = mat;
				Asserts.AreEqual (mat, obj.Rotation, "Rotation after setter");
				var transposed3x3 = MatrixFloat3x3.Transpose ((MatrixFloat3x3) mat);
				Asserts.AreEqual (transposed3x3, obj.Rotation3x3, "Rotation3x3 after setter");
				Asserts.AreEqual (transposed3x3, CFunctions.GetMatrixFloat3x3 (obj, "rotation"), "Rotation3x3 after setter native");
#endif
#if NET
				obj.Rotation = mat3x3;
				Asserts.AreEqual (mat3x3, obj.Rotation, "Rotation3x3 after setter 3x3");
#else
				obj.Rotation3x3 = mat3x3;
				Asserts.AreEqual (mat3x3, obj.Rotation3x3, "Rotation3x3 after setter 3x3");
#endif
				Asserts.AreEqual (mat3x3, CFunctions.GetMatrixFloat3x3 (obj, "rotation"), "Rotation3x3 after setter native 3x3");
			}
		}
#endif // !NET
	}
}

#endif // __WATCHOS__
