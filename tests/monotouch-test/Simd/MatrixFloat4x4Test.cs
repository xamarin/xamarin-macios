
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

#if NET
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using OpenTK;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Simd {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MatrixFloat4x4Test {
		[Test]
		public void Identity ()
		{
			var identity = new MatrixFloat4x4 {
				M11 = 1f,
				M22 = 1f,
				M33 = 1f,
				M44 = 1f,
			};
			Asserts.AreEqual (identity, MatrixFloat4x4.Identity, "identity");
#if !NET
			Asserts.AreEqual (Matrix4.Identity, MatrixFloat4x4.Identity, "opentk identity");
#endif
		}

#if !NET
		[Test]
		public void RowConstructor ()
		{
			var expected = GetTestMatrix ();
			var actual = new MatrixFloat4x4 (expected.Row0, expected.Row1, expected.Row2, expected.Row3);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestMatrix ();
			var actual = new MatrixFloat4x4 (expected.M11, expected.M12, expected.M13, expected.M14,
											 expected.M21, expected.M22, expected.M23, expected.M24,
											 expected.M31, expected.M32, expected.M33, expected.M34,
											 expected.M41, expected.M42, expected.M43, expected.M44);
			Asserts.AreEqual (expected, actual, "ctor 1");

		}

		[Test]
		public void Determinant ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat4x4) expected;
			Assert.AreEqual (expected.Determinant, actual.Determinant, 0.000001f, "determinant\n" + actual);

		}

		[Test]
		public void Elements ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat4x4) expected;

			Assert.AreEqual (expected.M11, actual.M11, "m11 getter");
			Assert.AreEqual (expected.M12, actual.M12, "m12 getter");
			Assert.AreEqual (expected.M13, actual.M13, "m13 getter");
			Assert.AreEqual (expected.M14, actual.M14, "m14 getter");
			Assert.AreEqual (expected.M21, actual.M21, "m21 getter");
			Assert.AreEqual (expected.M22, actual.M22, "m22 getter");
			Assert.AreEqual (expected.M23, actual.M23, "m23 getter");
			Assert.AreEqual (expected.M24, actual.M24, "m24 getter");
			Assert.AreEqual (expected.M31, actual.M31, "m31 getter");
			Assert.AreEqual (expected.M32, actual.M32, "m32 getter");
			Assert.AreEqual (expected.M33, actual.M33, "m33 getter");
			Assert.AreEqual (expected.M34, actual.M34, "m34 getter");
			Assert.AreEqual (expected.M41, actual.M41, "m41 getter");
			Assert.AreEqual (expected.M42, actual.M42, "m42 getter");
			Assert.AreEqual (expected.M43, actual.M43, "m43 getter");
			Assert.AreEqual (expected.M44, actual.M44, "m44 getter");

			var newExpected = GetTestMatrix ();
			actual.M11 = newExpected.M11;
			actual.M12 = newExpected.M12;
			actual.M13 = newExpected.M13;
			actual.M14 = newExpected.M14;
			actual.M21 = newExpected.M21;
			actual.M22 = newExpected.M22;
			actual.M23 = newExpected.M23;
			actual.M24 = newExpected.M24;
			actual.M31 = newExpected.M31;
			actual.M32 = newExpected.M32;
			actual.M33 = newExpected.M33;
			actual.M34 = newExpected.M34;
			actual.M41 = newExpected.M41;
			actual.M42 = newExpected.M42;
			actual.M43 = newExpected.M43;
			actual.M44 = newExpected.M44;
			Assert.AreEqual (newExpected.M11, actual.M11, "m11 setter");
			Assert.AreEqual (newExpected.M12, actual.M12, "m12 setter");
			Assert.AreEqual (newExpected.M13, actual.M13, "m13 setter");
			Assert.AreEqual (newExpected.M14, actual.M14, "m14 setter");
			Assert.AreEqual (newExpected.M21, actual.M21, "m21 setter");
			Assert.AreEqual (newExpected.M22, actual.M22, "m22 setter");
			Assert.AreEqual (newExpected.M23, actual.M23, "m23 setter");
			Assert.AreEqual (newExpected.M24, actual.M24, "m24 setter");
			Assert.AreEqual (newExpected.M31, actual.M31, "m31 setter");
			Assert.AreEqual (newExpected.M32, actual.M32, "m32 setter");
			Assert.AreEqual (newExpected.M33, actual.M33, "m33 setter");
			Assert.AreEqual (newExpected.M34, actual.M34, "m34 setter");
			Assert.AreEqual (newExpected.M41, actual.M41, "m41 setter");
			Assert.AreEqual (newExpected.M42, actual.M42, "m42 setter");
			Assert.AreEqual (newExpected.M43, actual.M43, "m43 setter");
			Assert.AreEqual (newExpected.M44, actual.M44, "m44 setter");
		}

		[Test]
		public void TransposeInstance ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat4x4) expected;

			expected.Transpose ();
			actual.Transpose ();

			Asserts.AreEqual (expected, actual, "transpose");
		}

		[Test]
		public void TransposeStatic ()
		{
			var input = GetTestMatrix ();
			var inputSimd = (MatrixFloat4x4) input;

			var expected = Matrix4.Transpose (input);
			var actual = MatrixFloat4x4.Transpose (inputSimd);

			Asserts.AreEqual (expected, actual, "transpose");

			input = GetTestMatrix ();
			inputSimd = (MatrixFloat4x4) input;
			Matrix4.Transpose (ref input, out expected);
			MatrixFloat4x4.Transpose (ref inputSimd, out actual);
			Asserts.AreEqual (expected, actual, "transpose out/ref");
		}

		[Test]
		public void TransposeStatic_ByRef ()
		{
			var input = GetTestMatrix ();
			var inputSimd = (MatrixFloat4x4) input;

			Matrix4 expected;
			MatrixFloat4x4 actual;

			Matrix4.Transpose (ref input, out expected);
			MatrixFloat4x4.Transpose (ref inputSimd, out actual);
			Asserts.AreEqual (expected, actual, "transpose out/ref");
		}

		[Test]
		public void Multiply ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat4x4) inputL;
			var inputSimdR = (MatrixFloat4x4) inputR;
			var expected = Matrix4.Mult (inputL, inputR);
			var actual = MatrixFloat4x4.Multiply (inputSimdL, inputSimdR);

			Asserts.AreEqual (expected, actual, "multiply");
		}

		[Test]
		public void Multiply_ByRef ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat4x4) inputL;
			var inputSimdR = (MatrixFloat4x4) inputR;
			Matrix4 expected;
			MatrixFloat4x4 actual;

			Matrix4.Mult (ref inputL, ref inputR, out expected);
			MatrixFloat4x4.Multiply (ref inputSimdL, ref inputSimdR, out actual);

			Asserts.AreEqual (expected, actual, "multiply");
		}


		[Test]
		public void Multiply_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat4x4) inputL;
			var inputSimdR = (MatrixFloat4x4) inputR;
			var expected = inputL * inputR;
			var actual = inputSimdL * inputSimdR;

			Asserts.AreEqual (expected, actual, "multiply");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat4x4) inputL;
			var inputSimdR = (MatrixFloat4x4) inputR;

			// matrices are different
			Assert.AreEqual (inputL == inputR, inputSimdL == inputSimdR, "inequality");
			Assert.IsFalse (inputL == inputR, "inequality 2 expected");
			Assert.IsFalse (inputSimdL == inputSimdR, "inequality 2 actual");

			inputL = inputR;
			inputSimdL = inputSimdR;
			// matrices are identical
			Assert.AreEqual (inputL == inputR, inputSimdL == inputSimdR, "equality");
			Assert.IsTrue (inputL == inputR, "equality 2 expected");
			Assert.IsTrue (inputSimdL == inputSimdR, "equality 2 actual");

			Assert.IsTrue (MatrixFloat4x4.Identity == (MatrixFloat4x4) Matrix4.Identity, "identity equality");
		}

		[Test]
		public void Inequality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat4x4) inputL;
			var inputSimdR = (MatrixFloat4x4) inputR;

			// matrices are different
			Assert.AreEqual (inputL != inputR, inputSimdL != inputSimdR, "inequality");
			Assert.IsTrue (inputL != inputR, "inequality 2 expected");
			Assert.IsTrue (inputSimdL != inputSimdR, "inequality 2 actual");

			inputL = inputR;
			inputSimdL = inputSimdR;
			// matrices are identical
			Assert.AreEqual (inputL != inputR, inputSimdL != inputSimdR, "equality");
			Assert.IsFalse (inputL != inputR, "equality 2 expected");
			Assert.IsFalse (inputSimdL != inputSimdR, "equality 2 actual");

			Assert.IsFalse (MatrixFloat4x4.Identity != (MatrixFloat4x4) Matrix4.Identity, "identity equality");
		}

		[Test]
		public void Explicit_Operator_ToMatrix4 ()
		{
			var expected = (MatrixFloat4x4) GetTestMatrix ();
			var actual = (Matrix4) expected;

			Asserts.AreEqual (expected, actual, "tomatrix4");

			actual = (Matrix4) MatrixFloat4x4.Identity;
			Asserts.AreEqual (MatrixFloat4x4.Identity, actual, "tomatrix4 identity");
			Asserts.AreEqual (Matrix4.Identity, actual, "tomatrix4 identity2");
		}

		[Test]
		public void Explicit_Operator_FromMatrix4 ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat4x4) expected;

			Asserts.AreEqual (expected, actual, "frommatrix4");

			actual = (MatrixFloat4x4) Matrix4.Identity;
			Asserts.AreEqual (MatrixFloat4x4.Identity, actual, "tomatrix4 identity");
			Asserts.AreEqual (Matrix4.Identity, actual, "tomatrix4 identity2");
		}

		[Test]
		public void ToStringTest ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat4x4) expected;

			Assert.AreEqual (expected.ToString (), actual.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestMatrix ();
			var expectedB = GetTestMatrix ();
			var actualA = (MatrixFloat4x4) expectedA;
			var actualB = (MatrixFloat4x4) expectedB;

			Assert.IsTrue (actualA.Equals ((object) actualA), "self");
			Assert.IsFalse (actualA.Equals ((object) actualB), "other");
			Assert.IsFalse (actualA.Equals (null), "null");
			Assert.IsFalse (actualA.Equals (expectedA), "other type");
		}

		[Test]
		public void Equals_Matrix ()
		{
			var expectedA = GetTestMatrix ();
			var expectedB = GetTestMatrix ();
			var actualA = (MatrixFloat4x4) expectedA;
			var actualB = (MatrixFloat4x4) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		// A collection of test matrices.
		//
		// I initially tried randomly generating test matrices, but it turns out
		// there are accumulative computational differences in the different algorithms
		// between Matrix4 and MatrixFloat4x4. Since the differences are accumulative,
		// I couldn't find a minimal sensible delta values when comparing 
		// matrices.
		//
		// So I just serialized a few matrices that were randomly generated, and
		// these have been tested to not produce accumulative computational differences.
		// 
		static Matrix4 [] test_matrices = new [] {
			new Matrix4 (0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f, 0.9884372f, 0.1313103f, 0.3327205f, 0.01164342f, 0.6563147f, 0.7923161f, 0.6764754f, 0.07481737f, 0.03239552f, 0.7156482f, 0.6136858f, 0.1864168f),
			new Matrix4 (0.7717745f, 0.559364f, 0.00918373f, 0.6579159f, 0.123461f, 0.9993145f, 0.5487496f, 0.2823398f, 0.9710717f, 0.8750508f, 0.472472f, 0.2608089f, 0.5771761f, 0.5617125f, 0.176998f, 0.1271691f),
			new Matrix4 (0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f, 0.8561344f, 0.009231919f, 0.6150167f, 0.7542298f, 0.550727f, 0.3625788f, 0.6639862f, 0.5763468f, 0.9717328f, 0.003812184f, 0.985266f, 0.7540002f),
			new Matrix4 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f, 6.389504E+07f, 1.172175E+09f, 1.399567E+09f, 1.187143E+09f, 3.729208E+07f, 5.50313E+08f, 1.847369E+09f, 1.612405E+09f, 1.699488E+08f, 4.952176E+08f, 1.07262E+09f, 2.035059E+09f),
			new Matrix4 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f, 1.966322E+09f, 1.1814E+09f, 8.464673E+08f, 1.940651E+09f, 1.229937E+09f, 1.367379E+09f, 1.900015E+09f, 1.516109E+09f, 2.146064E+09f, 1.870971E+09f, 1.046267E+09f, 1.088363E+09f),
			new Matrix4 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f, 3.705524E+08f, 1.984941E+09f, 2.175935E+07f, 4.633518E+08f, 1.801243E+09f, 1.616996E+09f, 1.620852E+09f, 7291498f, 1.012728E+09f, 2.834145E+08f, 3.5328E+08f, 1.35012E+09f),
			new Matrix4 (0.4904693f, 0.841727f, 0.2294401f, 0.5736054f, 0.5406881f, 0.2172498f, 0.1261143f, 0.6736677f, 0.4570194f, 0.9091009f, 0.7669608f, 0.8468134f, 0.01802658f, 0.3850208f, 0.3730424f, 0.2440258f),
			new Matrix4 (0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f, 0.340791f, 0.2192288f, 0.5144276f, 0.01813344f, 0.07687104f, 0.7971596f, 0.6393988f, 0.9002907f, 0.1011457f, 0.5047605f, 0.7202546f, 0.07729452f),
			new Matrix4 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f, 1.212676E+09f, 4.131035E+08f, 1.032453E+09f, 2.074689E+08f, 1.536594E+09f, 3.266183E+07f, 5.222072E+08f, 7.923175E+08f, 1.762531E+09f, 7.901702E+08f, 8.1975E+08f, 1.630734E+09f),
			new Matrix4 (0.006755914f, 0.07464754f, 0.287938f, 0.3724834f, 0.1496783f, 0.6224982f, 0.7150125f, 0.5554719f, 0.4638171f, 0.4200902f, 0.4867154f, 0.773377f, 0.3558737f, 0.4043404f, 0.04670618f, 0.7695189f),
		};

		static int counter;
		internal static Matrix4 GetTestMatrix ()
		{
			counter++;
			if (counter == test_matrices.Length)
				counter = 0;
			return test_matrices [counter];
		}
#endif // !NET
	}
}
