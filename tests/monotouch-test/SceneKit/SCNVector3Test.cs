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
	public class SCNVector3Test {
		static pfloat delta = (pfloat) 0.000001;

		[Test]
		public void TransformVector ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformVector (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (740, 1340, 1940), transformed, "Transformed");
		}

		[Test]
		public void TransformVector_out ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			SCNVector3.TransformVector (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector3 (740, 1340, 1940), transformed, "Transformed");
		}

		[Test]
		public void TransformNormal ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, -22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, -44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			pos.Normalize ();
			var transformed = SCNVector3.TransformNormal (pos, matrix);
			Asserts.AreEqual (new SCNVector3 ((pfloat) 0.406966, 0, (pfloat) (-0.151853)), transformed, delta, "Transformed");
		}

		[Test]
		public void TransformNormal_out ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, -22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, -44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			pos.Normalize ();
			SCNVector3.TransformNormal (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector3 ((pfloat) 0.406966, 0, (pfloat) (-0.151853)), transformed, delta, "Transformed");
		}

		[Test]
		public void TransformNormalInverse ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			pos.Normalize ();
			var transformed = SCNVector3.TransformNormalInverse (pos, matrix);
			Asserts.AreEqual (new SCNVector3 ((pfloat) 39.0201413, (pfloat) 40.62370877, (pfloat) 42.2272762), transformed, delta, "Transformed");
		}

		[Test]
		public void TransformNormalInverse_out ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			pos.Normalize ();
			SCNVector3.TransformNormalInverse (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector3 ((pfloat) 39.0201413, (pfloat) 40.62370877, (pfloat) 42.2272762), transformed, delta, "Transformed");
		}

		[Test]
		public void TransformPosition ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
		}

		[Test]
		public void TransformPosition_out ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			SCNVector3.TransformPosition (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
		}

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

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.Transform (pos, matrix);
			Asserts.AreEqual (new SCNVector4 (754, 1364, 1974, 2584), transformed, "Transformed");
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

			var pos = new SCNVector3 (10, 20, 30);
			SCNVector3.Transform (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector4 (754, 1364, 1974, 2584), transformed, "Transformed");
		}

		[Test]
		public void TransformPerspective ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPerspective (pos, matrix);
			Asserts.AreEqual (new SCNVector3 ((pfloat) 0.291795, (pfloat) 0.5278637, (pfloat) 0.76393188), transformed, delta, "Transformed");
		}

		[Test]
		public void TransformPerspective_out ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#if !NET
			matrix.Transpose ();
#endif

			var pos = new SCNVector3 (10, 20, 30);
			SCNVector3.TransformPerspective (ref pos, ref matrix, out var transformed);
			Asserts.AreEqual (new SCNVector3 ((pfloat) 0.291795, (pfloat) 0.5278637, (pfloat) 0.76393188), transformed, delta, "Transformed");
		}
	}
}
#endif // HAS_SCENEKIT
