//
// Unit tests for SCNMatrix4
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreAnimation;
using Foundation;
using SceneKit;

using NUnit.Framework;

#if NET
using Vector3 = global::System.Numerics.Vector3;
using Vector3d = global::CoreGraphics.NVector3d;
using Vector4 = global::System.Numerics.Vector4;
using Quaternion = global::System.Numerics.Quaternion;
using Quaterniond = global::CoreGraphics.NQuaterniond;
#else
using OpenTK;
#endif

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
	public class SCNMatrix4Test {
		static pfloat OneThird = (pfloat) (1.0 / 3.0);
		static pfloat OneFifteenth = (pfloat) (1.0 / 15.0);
		static pfloat TwoThirds = (pfloat) (2.0 / 3.0);
		static pfloat TwoFifteenths = (pfloat) (2.0 / 15.0); // 0.1333333333..
		static pfloat SqrtTwo = (pfloat) (Math.Sqrt (2));
		static pfloat SqrtTwoHalved = (pfloat) (Math.Sqrt (2) / 2);
		static pfloat SqrtThree = (pfloat) (Math.Sqrt (3));
		static pfloat SqrtThreeHalved = (pfloat) (Math.Sqrt (3) / 2);
		static pfloat SqrtThreeInverted = (pfloat) (1 / Math.Sqrt (3));
		static pfloat SqrtSix = (pfloat) (Math.Sqrt (6));
		static pfloat SqrtSixInverted = (pfloat) (1 / Math.Sqrt (6));
		static pfloat SqrtTwelve = (pfloat) (Math.Sqrt (12)); // 3.464102
		static pfloat OhPointFive = (pfloat) 0.5;

		[Test]
		public void Identity ()
		{
			var matrix = SCNMatrix4.Identity;
			var expected = new SCNMatrix4 (
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1);

			Asserts.AreEqual (expected, matrix, "Identity");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (pos, transformed, "Transformed");
		}

		[Test]
		public void Constructor_RowVectors ()
		{
			var matrix = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var expected = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
			Asserts.AreEqual (expected, matrix, "Constructor");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#endif
		}

		[Test]
		public void Constructor_Elements ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);

			Asserts.AreEqual (matrix, "Constructor",
#if NET
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
#else
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#endif

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#endif
		}

#if !WATCH
		[Test]
		public void Constructor_CATransform3d ()
		{
			var transform = new CATransform3D () {
				M11 = 11,
				M12 = 12,
				M13 = 13,
				M14 = 14,
				M21 = 21,
				M22 = 22,
				M23 = 23,
				M24 = 24,
				M31 = 31,
				M32 = 32,
				M33 = 33,
				M34 = 34,
				M41 = 41,
				M42 = 42,
				M43 = 43,
				M44 = 44,
			};
			var matrix = new SCNMatrix4 (transform);
			var expected = new SCNMatrix4 (
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "Constructor");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
		}
#endif

		[Test]
		public void Determinant ()
		{
			var matrix = new SCNMatrix4 (
				3, 5, 8, 9,
				5, 3, 5, 8,
				9, 6, 4, 2,
				4, 6, 9, 8);
			Assert.AreEqual ((pfloat) (-165), matrix.Determinant, "Determinant");
		}


		[Test]
		public void Rows ()
		{
			var matrix = new SCNMatrix4 (
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
			Asserts.AreEqual (matrix.Row0, new SCNVector4 (11, 12, 13, 14), "Row0");
			Asserts.AreEqual (matrix.Row1, new SCNVector4 (21, 22, 23, 24), "Row1");
			Asserts.AreEqual (matrix.Row2, new SCNVector4 (31, 32, 33, 34), "Row2");
			Asserts.AreEqual (matrix.Row3, new SCNVector4 (41, 42, 43, 44), "Row3");
		}

		[Test]
		public void Elements ()
		{
			// We're column-major in .NET, which means the first number (M#.) is the column,
			// and the second number (M.#) is the row. That's the reverse of how it's in legacy
			// Xamarin.
			var matrix = new SCNMatrix4 (
#if NET
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
#else
				11, 12, 13, 14,
				21, 22, 23, 24,
				31, 32, 33, 34,
				41, 42, 43, 44);
#endif
			Assert.AreEqual ((pfloat) 11, matrix.M11, "M11");
			Assert.AreEqual ((pfloat) 12, matrix.M12, "M12");
			Assert.AreEqual ((pfloat) 13, matrix.M13, "M13");
			Assert.AreEqual ((pfloat) 14, matrix.M14, "M14");
			Assert.AreEqual ((pfloat) 21, matrix.M21, "M21");
			Assert.AreEqual ((pfloat) 22, matrix.M22, "M22");
			Assert.AreEqual ((pfloat) 23, matrix.M23, "M23");
			Assert.AreEqual ((pfloat) 24, matrix.M24, "M24");
			Assert.AreEqual ((pfloat) 31, matrix.M31, "M31");
			Assert.AreEqual ((pfloat) 32, matrix.M32, "M32");
			Assert.AreEqual ((pfloat) 33, matrix.M33, "M33");
			Assert.AreEqual ((pfloat) 34, matrix.M34, "M34");
			Assert.AreEqual ((pfloat) 41, matrix.M41, "M41");
			Assert.AreEqual ((pfloat) 42, matrix.M42, "M42");
			Assert.AreEqual ((pfloat) 43, matrix.M43, "M43");
			Assert.AreEqual ((pfloat) 44, matrix.M44, "M44");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
		}

#if NET // The legacy Invert implementation seems very wrong, so only verify .NET behavior
		[Test]
		public void Invert ()
		{
			var matrix = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var ex = Assert.Throws<InvalidOperationException> (() => matrix.Invert (), "Singular matrix");
			Assert.That (ex.Message, Does.Contain ("Matrix is singular and cannot be inverted"), "Singular matrix message");

			matrix = new SCNMatrix4 (
				3, 5, 8, 9,
				5, 3, 5, 8,
				9, 6, 4, 2,
				4, 6, 9, 8);
			var originalMatrix = matrix;
			matrix.Invert ();

			Asserts.AreEqual (SCNMatrix4Invert (originalMatrix), matrix, (pfloat) 0.00001, "Native");

			var expected = new SCNMatrix4 (
				(pfloat) (-0.6181818181818182), (pfloat) (0.3151515151515151), (pfloat) (-0.030303030303030304), (pfloat) (0.3878787878787879),
				(pfloat) (1.6363636363636365), (pfloat) (-0.696969696969697), (pfloat) (0.3939393939393939), (pfloat) (-1.2424242424242424),
				(pfloat) (-1.3818181818181818), (pfloat) (0.3515151515151515), (pfloat) (-0.30303030303030304), (pfloat) (1.2787878787878788),
				(pfloat) (0.6363636363636364), (pfloat) (-0.030303030303030304), (pfloat) (0.06060606060606061), (pfloat) (-0.5757575757575758));

			Asserts.AreEqual (expected, matrix, (pfloat) 0.00001, "Invert");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (-0.4f, 13, -14.6f), transformed, 0.00001f, "Transformed");
		}
#endif

		[Test]
		public void Transpose ()
		{
			var matrix = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			matrix.Transpose ();
			var expected = new SCNMatrix4 (
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
			Asserts.AreEqual (expected, matrix, "Transpose");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#endif
		}

		[Test]
		public void CreateFromColumns ()
		{
			var matrix = SCNMatrix4.CreateFromColumns (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var expected = new SCNMatrix4 (
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
			Asserts.AreEqual (expected, matrix, "CreateFromColumns");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#endif
		}

		[Test]
		public void CreateFromColumns_Out ()
		{
			SCNMatrix4.CreateFromColumns (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44),
			out var matrix);
			var expected = new SCNMatrix4 (
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
			Asserts.AreEqual (expected, matrix, "CreateFromColumns");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#endif
		}

		[Test]
		public void CreateFromAxisAngle_pfloat_Out ()
		{
			SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (2, 2, 2), (pfloat) (Math.PI / 3), out var matrix);
			var expected = new SCNMatrix4 (
				TwoThirds, -OneThird, TwoThirds, 0,
				TwoThirds, TwoThirds, -OneThird, 0,
				-OneThird, TwoThirds, TwoThirds, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateFromAxisAngle");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 10, 30), transformed, (pfloat) 0.00001, "Transformed");
		}

		[Test]
		public void CreateFromAxisAngle_float_Out ()
		{
			SCNMatrix4.CreateFromAxisAngle (new Vector3 (2, 2, 2), (float) (Math.PI / 3), out var matrix);
			var expected = new SCNMatrix4 (
				TwoThirds, -OneThird, TwoThirds, 0,
				TwoThirds, TwoThirds, -OneThird, 0,
				-OneThird, TwoThirds, TwoThirds, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateFromAxisAngle");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 10, 30), transformed, (pfloat) 0.00001, "Transformed");
		}

		[Test]
		public void CreateFromAxisAngle_double_Out ()
		{
			SCNMatrix4.CreateFromAxisAngle (new Vector3d (2, 2, 2), (double) (Math.PI / 3), out var matrix);
			var expected = new SCNMatrix4 (
				TwoThirds, -OneThird, TwoThirds, 0,
				TwoThirds, TwoThirds, -OneThird, 0,
				-OneThird, TwoThirds, TwoThirds, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateFromAxisAngle");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 10, 30), transformed, (pfloat) 0.000001, "Transformed");
		}

		[Test]
		public void CreateFromAxisAngle ()
		{
			var matrix = SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (2, 2, 2), (pfloat) (Math.PI / 3));
			var expected = new SCNMatrix4 (
				TwoThirds, -OneThird, TwoThirds, 0,
				TwoThirds, TwoThirds, -OneThird, 0,
				-OneThird, TwoThirds, TwoThirds, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateFromAxisAngle");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 10, 30), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateRotationX_Out ()
		{
			var angle = (pfloat) (Math.PI / 3);
			SCNMatrix4.CreateRotationX (angle, out var matrix);
			var expected = new SCNMatrix4 (
				1, 0, 0, 0,
				0, OhPointFive, -SqrtThreeHalved, 0,
				0, SqrtThreeHalved, OhPointFive, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateRotationX");

			Asserts.AreEqual (SCNMatrix4MakeRotation (angle, 1, 0, 0), matrix, 0.00001f, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (10, -15.980762f, 32.320508f), transformed, 0.0001f, "Transformed");
		}

		[Test]
		public void CreateRotationX ()
		{
			var angle = (pfloat) (Math.PI / 3);
			var matrix = SCNMatrix4.CreateRotationX (angle);
			var expected = new SCNMatrix4 (
				1, 0, 0, 0,
				0, OhPointFive, -SqrtThreeHalved, 0,
				0, SqrtThreeHalved, OhPointFive, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateRotationX");

			Asserts.AreEqual (SCNMatrix4MakeRotation (angle, 1, 0, 0), matrix, 0.00001f, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (10, -15.980762f, 32.320508f), transformed, 0.0001f, "Transformed");
		}

		[Test]
		public void CreateRotationY_Out ()
		{
			var angle = (pfloat) (Math.PI / 3);
			SCNMatrix4.CreateRotationY ((pfloat) (Math.PI / 3), out var matrix);
			var expected = new SCNMatrix4 (
				OhPointFive, 0, SqrtThreeHalved, 0,
				0, 1, 0, 0,
				-SqrtThreeHalved, 0, OhPointFive, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateRotationY");

			Asserts.AreEqual (SCNMatrix4MakeRotation (angle, 0, 1, 0), matrix, 0.00001f, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (30.98076f, 20, 6.33974f), transformed, 0.0001f, "Transformed");
		}

		[Test]
		public void CreateRotationY ()
		{
			var angle = (pfloat) (Math.PI / 3);
			var matrix = SCNMatrix4.CreateRotationY (angle);
			var expected = new SCNMatrix4 (
				OhPointFive, 0, SqrtThreeHalved, 0,
				0, 1, 0, 0,
				-SqrtThreeHalved, 0, OhPointFive, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateRotationY");

			Asserts.AreEqual (SCNMatrix4MakeRotation (angle, 0, 1, 0), matrix, 0.00001f, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (30.98076f, 20, 6.33974f), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateRotationZ_Out ()
		{
			var angle = (pfloat) (Math.PI / 3);
			SCNMatrix4.CreateRotationZ (angle, out var matrix);
			var expected = new SCNMatrix4 (
				OhPointFive, -SqrtThreeHalved, 0, 0,
				SqrtThreeHalved, OhPointFive, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateRotationZ");

			Asserts.AreEqual (SCNMatrix4MakeRotation (angle, 0, 0, 1), matrix, 0.00001f, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (-12.320508f, 18.66025f, 30), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateRotationZ ()
		{
			var angle = (pfloat) (Math.PI / 3);
			var matrix = SCNMatrix4.CreateRotationZ (angle);
			var expected = new SCNMatrix4 (
				OhPointFive, -SqrtThreeHalved, 0, 0,
				SqrtThreeHalved, OhPointFive, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreateRotationZ");

			Asserts.AreEqual (SCNMatrix4MakeRotation (angle, 0, 0, 1), matrix, 0.00001f, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (-12.320508f, 18.66025f, 30), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateTranslation_Out ()
		{
			SCNMatrix4.CreateTranslation (1, 2, 3, out var matrix);
			var expected = new SCNMatrix4 (
				1, 0, 0, 1,
				0, 1, 0, 2,
				0, 0, 1, 3,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreateTranslation");

			Asserts.AreEqual (SCNMatrix4MakeTranslation (new SCNVector3 (1, 2, 3)), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (11, 22, 33), transformed, "Transformed");
		}

		[Test]
		public void CreateTranslation_Vector_Out ()
		{
			var translation = new SCNVector3 (1, 2, 3);
			SCNMatrix4.CreateTranslation (ref translation, out var matrix);
			var expected = new SCNMatrix4 (
				1, 0, 0, 1,
				0, 1, 0, 2,
				0, 0, 1, 3,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreateTranslation");

			Asserts.AreEqual (SCNMatrix4MakeTranslation (new SCNVector3 (1, 2, 3)), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (11, 22, 33), transformed, "Transformed");
		}

		[Test]
		public void CreateTranslation ()
		{
			var matrix = SCNMatrix4.CreateTranslation (1, 2, 3);
			var expected = new SCNMatrix4 (
				1, 0, 0, 1,
				0, 1, 0, 2,
				0, 0, 1, 3,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreateTranslation");

			Asserts.AreEqual (SCNMatrix4MakeTranslation (new SCNVector3 (1, 2, 3)), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (11, 22, 33), transformed, "Transformed");
		}

		[Test]
		public void CreateTranslation_Vector ()
		{
			var translation = new SCNVector3 (1, 2, 3);
			var matrix = SCNMatrix4.CreateTranslation (translation);
			var expected = new SCNMatrix4 (
				1, 0, 0, 1,
				0, 1, 0, 2,
				0, 0, 1, 3,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreateTranslation");

			Asserts.AreEqual (SCNMatrix4MakeTranslation (translation), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (11, 22, 33), transformed, "Transformed");
		}

		[Test]
		public void CreateOrthographic_Out ()
		{
			SCNMatrix4.CreateOrthographic (1, 2, 3, 4, out var matrix);
			var expected = new SCNMatrix4 (
				2, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, -2, -7,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, 0, "CreateOrthographic");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 20, -67), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateOrthographic ()
		{
			var matrix = SCNMatrix4.CreateOrthographic (1, 2, 3, 4);
			var expected = new SCNMatrix4 (
				2, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, -2, -7,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, 0, "CreateOrthographic");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 20, -67), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateOrthographicOffCenter_Out ()
		{
			SCNMatrix4.CreateOrthographicOffCenter (1, 2, 3, 4, 5, 6, out var matrix);
			var expected = new SCNMatrix4 (
				2, 0, 0, -3,
				0, 2, 0, -7,
				0, 0, -2, -11,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreateOrthographicOffCenter");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (17, 33, -71), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreateOrthographicOffCenter ()
		{
			var matrix = SCNMatrix4.CreateOrthographicOffCenter (1, 2, 3, 4, 5, 6);
			var expected = new SCNMatrix4 (
				2, 0, 0, -3,
				0, 2, 0, -7,
				0, 0, -2, -11,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreateOrthographicOffCenter");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (17, 33, -71), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreatePerspectiveFieldOfView_Out ()
		{
			SCNMatrix4.CreatePerspectiveFieldOfView ((pfloat) (Math.PI / 3), 2, 3, 4, out var matrix);
			var expected = new SCNMatrix4 (
				SqrtThreeHalved, 0, 0, 0,
				0, SqrtThree, 0, 0,
				0, 0, -7, -24,
				0, 0, -1, 0);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreatePerspectiveFieldOfView");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (8.660254f, 34.641016f, -234), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreatePerspectiveFieldOfView ()
		{
			var matrix = SCNMatrix4.CreatePerspectiveFieldOfView ((pfloat) (Math.PI / 3), 2, 3, 4);
			var expected = new SCNMatrix4 (
				SqrtThreeHalved, 0, 0, 0,
				0, SqrtThree, 0, 0,
				0, 0, -7, -24,
				0, 0, -1, 0);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.000001f, "CreatePerspectiveFieldOfView");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (8.660254f, 34.641016f, -234), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void CreatePerspectiveOffCenter_Out ()
		{
			SCNMatrix4.CreatePerspectiveOffCenter (1, 2, 3, 4, 5, 6, out var matrix);
			var expected = new SCNMatrix4 (
				10, 0, 3, 0,
				 0, 10, 7, 0,
				 0, 0, -11, -60,
				 0, 0, -1, 0);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreatePerspectiveOffCenter");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (190, 410, -390), transformed, "Transformed");
		}

		[Test]
		public void CreatePerspectiveOffCenter ()
		{
			var matrix = SCNMatrix4.CreatePerspectiveOffCenter (1, 2, 3, 4, 5, 6);
			var expected = new SCNMatrix4 (
				10, 0, 3, 0,
				 0, 10, 7, 0,
				 0, 0, -11, -60,
				 0, 0, -1, 0);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, "CreatePerspectiveOffCenter");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (190, 410, -390), transformed, "Transformed");
		}

		[Test]
		public void Scale ()
		{
			var matrix = SCNMatrix4.Scale (2);
			var expected = new SCNMatrix4 (
				2, 0, 0, 0,
				0, 2, 0, 0,
				0, 0, 2, 0,
				0, 0, 0, 1);
			Asserts.AreEqual (expected, matrix, "CreateScale");

			Asserts.AreEqual (SCNMatrix4MakeScale (new SCNVector3 (2, 2, 2)), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (20, 40, 60), transformed, "Transformed");
		}

		[Test]
		public void Scale_Vector ()
		{
			var matrix = SCNMatrix4.Scale (new SCNVector3 (1, 2, 3));
			var expected = new SCNMatrix4 (
				1, 0, 0, 0,
				0, 2, 0, 0,
				0, 0, 3, 0,
				0, 0, 0, 1);
			Asserts.AreEqual (expected, matrix, "CreateScale");

			Asserts.AreEqual (SCNMatrix4MakeScale (new SCNVector3 (1, 2, 3)), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (10, 40, 90), transformed, "Transformed");
		}

		[Test]
		public void Scale_3 ()
		{
			var matrix = SCNMatrix4.Scale (1, 2, 3);
			var expected = new SCNMatrix4 (
				1, 0, 0, 0,
				0, 2, 0, 0,
				0, 0, 3, 0,
				0, 0, 0, 1);
			Asserts.AreEqual (expected, matrix, "CreateScale");

			Asserts.AreEqual (SCNMatrix4MakeScale (new SCNVector3 (1, 2, 3)), matrix, "Native");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (10, 40, 90), transformed, "Transformed");
		}

		[Test]
		public void Rotate ()
		{
			var quaternion = new Quaternion (1, 2, 3, 4);
			var matrix = SCNMatrix4.Rotate (quaternion);
			var expected = new SCNMatrix4 (
				TwoFifteenths, -TwoThirds, 11 * OneFifteenth, 0,
				7 * TwoFifteenths, OneThird, TwoFifteenths, 0,
				-OneThird, TwoThirds, TwoThirds, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.00001, "Rotate");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (pos, transformed, 0.0001f, "Transformed");
		}

		[Test]
		public void Rotate_d ()
		{
			var quaternion = new Quaterniond (1, 2, 3, 4);
			var matrix = SCNMatrix4.Rotate (quaternion);
			var expected = new SCNMatrix4 (
				TwoFifteenths, -TwoThirds, 11 * OneFifteenth, 0,
				7 * TwoFifteenths, OneThird, TwoFifteenths, 0,
				-OneThird, TwoThirds, TwoThirds, 0,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.00001, "Rotate");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (pos, transformed, 0.0001f, "Transformed");
		}

		[Test]
		public void LookAt_Vectors ()
		{
			var matrix = SCNMatrix4.LookAt (new SCNVector3 (1, 2, 3), new SCNVector3 (4, 5, 6), new SCNVector3 (7, 8, 9));
			var expected = new SCNMatrix4 (
				SqrtSixInverted, -2 * SqrtSixInverted, SqrtSixInverted, 0,
				-SqrtTwoHalved, 0, SqrtTwoHalved, -SqrtTwo,
				-SqrtThreeInverted, -SqrtThreeInverted, -SqrtThreeInverted, SqrtTwelve,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.00001, "LookAt");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (0, 12.7279220f, -31.1769145f), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void LookAt_Elements ()
		{
			var matrix = SCNMatrix4.LookAt (1, 2, 3, 4, 5, 6, 7, 8, 9);
			var expected = new SCNMatrix4 (
				SqrtSixInverted, -2 * SqrtSixInverted, SqrtSixInverted, 0,
				-SqrtTwoHalved, 0, SqrtTwoHalved, -SqrtTwo,
				-SqrtThreeInverted, -SqrtThreeInverted, -SqrtThreeInverted, SqrtTwelve,
				0, 0, 0, 1);
#if !NET
			expected.Transpose ();
#endif
			Asserts.AreEqual (expected, matrix, (pfloat) 0.00001, "LookAt");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (0, 12.7279220f, -31.1769145f), transformed, 0.00001f, "Transformed");
		}

		[Test]
		public void Mult ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			var matrix = SCNMatrix4.Mult (a, b);

			Asserts.AreEqual (SCNMatrix4Mult (a, b), matrix, "Native");
			var expected = new SCNMatrix4 (
#if NET
				94950, 98600, 102250, 105900,
				95990, 99680, 103370, 107060,
				97030, 100760, 104490, 108220,
				98070, 101840, 105610, 109380);
#else
				46350, 46400, 46450, 46500,
				83390, 83480, 83570, 83660,
				120430, 120560, 120690, 120820,
				157470, 157640, 157810, 157980);
#endif
			Asserts.AreEqual (expected, matrix, "Mult");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (6094900, 6161660, 6228420), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (5901670, 5908040, 5914410), transformed, "Transformed");
#endif
		}

		[Test]
		public void Mult_ByRef ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			SCNMatrix4.Mult (ref a, ref b, out var matrix);

			Asserts.AreEqual (SCNMatrix4Mult (a, b), matrix, "Native");
			var expected = new SCNMatrix4 (
#if NET
				94950, 98600, 102250, 105900,
				95990, 99680, 103370, 107060,
				97030, 100760, 104490, 108220,
				98070, 101840, 105610, 109380);
#else
				46350, 46400, 46450, 46500,
				83390, 83480, 83570, 83660,
				120430, 120560, 120690, 120820,
				157470, 157640, 157810, 157980);
#endif
			Asserts.AreEqual (expected, matrix, "Mult");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (6094900, 6161660, 6228420), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (5901670, 5908040, 5914410), transformed, "Transformed");
#endif
		}

#if NET // The legacy Invert implementation seems very wrong, so only verify .NET behavior
		[Test]
		public void Static_Invert ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var ex = Assert.Throws<InvalidOperationException> (() => SCNMatrix4.Invert (a), "Singular matrix");
			Assert.That (ex.Message, Does.Contain ("Matrix is singular and cannot be inverted"), "Singular matrix message");

			a = new SCNMatrix4 (
				3, 5, 8, 9,
				5, 3, 5, 8,
				9, 6, 4, 2,
				4, 6, 9, 8);

			var matrix = SCNMatrix4.Invert (a);

			var expected = new SCNMatrix4 (
				(pfloat) (-0.6181818181818182), (pfloat) (0.3151515151515151), (pfloat) (-0.030303030303030304), (pfloat) (0.3878787878787879),
				(pfloat) (1.6363636363636365), (pfloat) (-0.696969696969697), (pfloat) (0.3939393939393939), (pfloat) (-1.2424242424242424),
				(pfloat) (-1.3818181818181818), (pfloat) (0.3515151515151515), (pfloat) (-0.30303030303030304), (pfloat) (1.2787878787878788),
				(pfloat) (0.6363636363636364), (pfloat) (-0.030303030303030304), (pfloat) (0.06060606060606061), (pfloat) (-0.5757575757575758));
			Asserts.AreEqual (expected, matrix, (pfloat) 0.00001f, "Invert");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
			Asserts.AreEqual (new SCNVector3 (-0.4f, 13, -14.6f), transformed, 0.00001f, "Transformed");
		}
#endif

		[Test]
		public void Static_Transpose ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var matrix = SCNMatrix4.Transpose (a);
			var expected = new SCNMatrix4 (
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
			Asserts.AreEqual (expected, matrix, "Transpose");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#endif
		}

		[Test]
		public void Static_Transpose_ByRef ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			SCNMatrix4.Transpose (ref a, out var matrix);
			var expected = new SCNMatrix4 (
				11, 21, 31, 41,
				12, 22, 32, 42,
				13, 23, 33, 43,
				14, 24, 34, 44);
			Asserts.AreEqual (expected, matrix, "Transpose");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (1501, 1562, 1623), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (754, 1364, 1974), transformed, "Transformed");
#endif
		}

		[Test]
		public void Operator_Multiply ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			var matrix = a * b;
			Asserts.AreEqual (SCNMatrix4Mult (a, b), matrix, "Native");
			var expected = new SCNMatrix4 (
#if NET
				94950, 98600, 102250, 105900,
				95990, 99680, 103370, 107060,
				97030, 100760, 104490, 108220,
				98070, 101840, 105610, 109380);
#else
				46350, 46400, 46450, 46500,
				83390, 83480, 83570, 83660,
				120430, 120560, 120690, 120820,
				157470, 157640, 157810, 157980);
#endif
			Asserts.AreEqual (expected, matrix, "*");

			var pos = new SCNVector3 (10, 20, 30);
			var transformed = SCNVector3.TransformPosition (pos, matrix);
#if NET
			Asserts.AreEqual (new SCNVector3 (6094900, 6161660, 6228420), transformed, "Transformed");
#else
			Asserts.AreEqual (new SCNVector3 (5901670, 5908040, 5914410), transformed, "Transformed");
#endif
		}

		[Test]
		public void Operator_Equals ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			Assert.IsFalse (a == b, "Equals");
		}

		[Test]
		public void Operator_NotEquals ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			Assert.IsTrue (a != b, "NotEquals");
		}

		[Test]
		public void ToStringTest ()
		{
			var matrix = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			Assert.AreEqual ("(11, 12, 13, 14)\n(21, 22, 23, 24)\n(31, 32, 33, 34)\n(41, 42, 43, 44)", matrix.ToString (), "ToString");
		}

		[Test]
		public void Object_Equals ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			Assert.IsFalse (((object) a).Equals (b), "object.Equals");
		}

		[Test]
		public void IEquatable_Equals ()
		{
			var a = new SCNMatrix4 (
				new SCNVector4 (11, 12, 13, 14),
				new SCNVector4 (21, 22, 23, 24),
				new SCNVector4 (31, 32, 33, 34),
				new SCNVector4 (41, 42, 43, 44));
			var b = new SCNMatrix4 (
				new SCNVector4 (911, 912, 913, 914),
				new SCNVector4 (921, 922, 923, 924),
				new SCNVector4 (931, 932, 933, 934),
				new SCNVector4 (941, 942, 943, 944));
			Assert.IsFalse (((IEquatable<SCNMatrix4>) a).Equals (b), "object.Equals");
		}

		[Test]
		public void CreateRotationX_NodeComparison ()
		{
			// Create a test node (it defaults to position 0,0,0)
			var node = SCNNode.Create ();
			// Create a translation matrix
			var angle = (pfloat) (Math.PI / 2);
			// Use that matrix to transform the node
			node.Transform = SCNMatrix4.CreateRotationX (angle);
			Asserts.AreEqual (new SCNVector3 (angle, 0, 0), node.EulerAngles, 0.000001f, "EulerAngles");
			Asserts.AreEqual (new SCNQuaternion (SqrtTwoHalved, 0, 0, SqrtTwoHalved), node.Orientation, 0.000001f, "Orientation");
			Asserts.AreEqual (new SCNVector3 (0, 0, 0), node.Position, "Position");
			Asserts.AreEqual (new SCNVector4 (1, 0, 0, angle), node.Rotation, 0.000001f, "Rotation");
			Asserts.AreEqual (new SCNVector3 (1, 1, 1), node.Scale, "Scale");
		}

		[Test]
		public void CreateTranslationAndTransformPosition ()
		{
			// Create test point
			var point = new SCNVector3 (1, 2, 3);
			// Create translation
			var matrix = SCNMatrix4.CreateTranslation (10, 0, 0);
			// Transform the point
			var newPoint = SCNVector3.TransformPosition (point, matrix);
			Asserts.AreEqual (new SCNVector3 (11, 2, 3), newPoint, "A");
		}

		[Test]
		public void TranslationPosition_ret ()
		{
			// Create test point
			var point = new SCNVector3 (1, 2, 3);
			// Create translation
			var matrix =
				SCNMatrix4.CreateTranslation (-1, 0, 0) *
				SCNMatrix4.Scale (10, 1, 1);
			// Transform the point
			var newPoint = SCNVector3.TransformPosition (point, matrix);
			Asserts.AreEqual (new SCNVector3 (0, 2, 3), newPoint, "A");
		}

		[Test]
		public void TranslationPosition_out ()
		{
			// Create test point
			var point = new SCNVector3 (1, 2, 3);
			// Create translation
			var matrix =
				SCNMatrix4.CreateTranslation (-1, 0, 0) *
				SCNMatrix4.Scale (10, 1, 1);
			// Transform the point
			SCNVector3.TransformPosition (ref point, ref matrix, out var newPoint);
			Asserts.AreEqual (new SCNVector3 (0, 2, 3), newPoint, "A");
		}

		[Test]
		public void CreateTranslations_ret_floats ()
		{
			// Create a test node (it defaults to position 0,0,0)
			var node = SCNNode.Create ();
			// Create a translation matrix
			var matrix = SCNMatrix4.CreateTranslation (1, 2, 3);
			// Use that matrix to transform the node
			node.Transform = matrix;
			// Ask the node to extract just the translation part of the matrix
			var newPoint = node.Position;
			// Verify that it is now positioned at (1,2,3)
			Asserts.AreEqual (new SCNVector3 (1, 2, 3), newPoint, "A");
		}

		[Test]
		public void CreateTranslations_ret_SCNVector3 ()
		{
			// Create a test node (it defaults to position 0,0,0)
			var node = SCNNode.Create ();
			// Create a translation matrix
			var matrix = SCNMatrix4.CreateTranslation (new SCNVector3 (1, 2, 3));
			// Use that matrix to transform the node
			node.Transform = matrix;
			// Ask the node to extract just the translation part of the matrix
			var newPoint = node.Position;
			// Verify that it is now positioned at (1,2,3)
			Asserts.AreEqual (new SCNVector3 (1, 2, 3), newPoint, "A");
		}

		[Test]
		public void CreateTranslations_out_floats ()
		{
			// Create a test node (it defaults to position 0,0,0)
			var node = SCNNode.Create ();
			// Create a translation matrix
			SCNMatrix4.CreateTranslation (1, 2, 3, out var matrix);
			// Use that matrix to transform the node
			node.Transform = matrix;
			// Ask the node to extract just the translation part of the matrix
			var newPoint = node.Position;
			// Verify that it is now positioned at (1,2,3)
			Asserts.AreEqual (new SCNVector3 (1, 2, 3), newPoint, "A");
		}

		[Test]
		public void CreateTranslations_out_SCNVector3 ()
		{
			// Create a test node (it defaults to position 0,0,0)
			var node = SCNNode.Create ();
			// Create a translation matrix
			var vector = new SCNVector3 (1, 2, 3);
			SCNMatrix4.CreateTranslation (ref vector, out var matrix);
			// Use that matrix to transform the node
			node.Transform = matrix;
			// Ask the node to extract just the translation part of the matrix
			var newPoint = node.Position;
			// Verify that it is now positioned at (1,2,3)
			Asserts.AreEqual (new SCNVector3 (1, 2, 3), newPoint, "A");
		}

		[Test]
		public void SCNMatrix4Translate ()
		{
			var translationVector = new SCNVector3 (1, 2, 3);
			var managedTranslation = SCNMatrix4.CreateTranslation (translationVector);
			var nativeTranslation = SCNMatrix4MakeTranslation (translationVector);
			Asserts.AreEqual (nativeTranslation, managedTranslation, "A");
		}

		static SCNMatrix4 SCNMatrix4MakeTranslation (SCNVector3 v)
		{
			return global::Bindings.Test.CFunctions.x_SCNMatrix4MakeTranslation (v.X, v.Y, v.Z);
		}

		static SCNMatrix4 SCNMatrix4MakeScale (SCNVector3 v)
		{
			return global::Bindings.Test.CFunctions.x_SCNMatrix4MakeScale (v.X, v.Y, v.Z);
		}

		[DllImport (global::ObjCRuntime.Constants.SceneKitLibrary)]
		static extern SCNMatrix4 SCNMatrix4MakeRotation (pfloat angle, pfloat x, pfloat y, pfloat z);

		[DllImport (global::ObjCRuntime.Constants.SceneKitLibrary)]
		static extern SCNMatrix4 SCNMatrix4Mult (SCNMatrix4 a, SCNMatrix4 b);

		[DllImport (global::ObjCRuntime.Constants.SceneKitLibrary)]
		static extern SCNMatrix4 SCNMatrix4Invert (SCNMatrix4 a);
	}
}
#endif // !__WATCHOS__
