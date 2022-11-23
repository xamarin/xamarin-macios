//
// Unit tests for SCNMatrix4
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if HAS_SCENEKIT

#nullable enable

using System;
using Foundation;
using SceneKit;

using NUnit.Framework;

#if __MACOS__
#if NET
using pfloat = System.Runtime.InteropServices.NFloat;
#else
using pfloat = System.nfloat;
#endif
#else
using pfloat = System.Single;
#endif

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SCNVector4Test {
		[Test]
		public void Transform ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector4 (10, 20, 30, 40);
			var transformed = SCNVector4.Transform (pos, matrix);
			Asserts.AreEqual (new SCNVector4 (1300, 2300, 3300, 4300), transformed, "Transformed");
		}

		[Test]
		public void Transform_out ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector4 (10, 20, 30, 40);
			SCNVector4.Transform (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector4 (1300, 2300, 3300, 4300), transformed, "Transformed");
		}

	}
}

#endif // HAS_SCENEKIT
