
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

#if NET
using NMatrix4d = global::CoreGraphics.NMatrix4d;
#else
using OpenTK;
using NMatrix4d = global::OpenTK.NMatrix4d;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Simd {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NMatrix4dTest {

		[Test]
		public void Identity ()
		{
			var identity = new NMatrix4d {
				M11 = 1d,
				M22 = 1d,
				M33 = 1d,
				M44 = 1d,
			};
			Asserts.AreEqual (identity, NMatrix4d.Identity, "identity");
#if !NET
			Asserts.AreEqual (Matrix4d.Identity, NMatrix4d.Identity, "opentk identity");
#endif
		}

#if !NET
		[Test]
		public void RowConstructor ()
		{
			var expected = GetTestMatrix ();
			var actual = new NMatrix4d (expected.Row0, expected.Row1, expected.Row2, expected.Row3);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestMatrix ();
			var actual = new NMatrix4d (expected.M11, expected.M12, expected.M13, expected.M14,
											 expected.M21, expected.M22, expected.M23, expected.M24,
											 expected.M31, expected.M32, expected.M33, expected.M34,
											 expected.M41, expected.M42, expected.M43, expected.M44);
			Asserts.AreEqual (expected, actual, "ctor 1");

		}

		[Test]
		public void Determinant ()
		{
			var expected = GetTestMatrix ();
			var actual = (NMatrix4d) expected;
			Assert.AreEqual (expected.Determinant, actual.Determinant, 0.000001d, "determinant\n" + actual);

		}

		[Test]
		public void Elements ()
		{
			var expected = GetTestMatrix ();
			var actual = (NMatrix4d) expected;

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
			var actual = (NMatrix4d) expected;

			expected.Transpose ();
			actual.Transpose ();

			Asserts.AreEqual (expected, actual, "transpose");
		}

		[Test]
		public void TransposeStatic ()
		{
			var input = GetTestMatrix ();
			var inputSimd = (NMatrix4d) input;

			var expected = Matrix4d.Transpose (input);
			var actual = NMatrix4d.Transpose (inputSimd);

			Asserts.AreEqual (expected, actual, "transpose");

			input = GetTestMatrix ();
			inputSimd = (NMatrix4d) input;
			Matrix4d.Transpose (ref input, out expected);
			NMatrix4d.Transpose (ref inputSimd, out actual);
			Asserts.AreEqual (expected, actual, "transpose out/ref");
		}

		[Test]
		public void TransposeStatic_ByRef ()
		{
			var input = GetTestMatrix ();
			var inputSimd = (NMatrix4d) input;

			Matrix4d expected;
			NMatrix4d actual;

			Matrix4d.Transpose (ref input, out expected);
			NMatrix4d.Transpose (ref inputSimd, out actual);
			Asserts.AreEqual (expected, actual, "transpose out/ref");
		}

		[Test]
		public void Multiply ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4d) inputL;
			var inputSimdR = (NMatrix4d) inputR;
			var expected = Matrix4d.Mult (inputL, inputR);
			var actual = NMatrix4d.Multiply (inputSimdL, inputSimdR);

			Asserts.AreEqual (expected, actual, "multiply");
		}

		[Test]
		public void Multiply_ByRef ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4d) inputL;
			var inputSimdR = (NMatrix4d) inputR;
			Matrix4d expected;
			NMatrix4d actual;

			Matrix4d.Mult (ref inputL, ref inputR, out expected);
			NMatrix4d.Multiply (ref inputSimdL, ref inputSimdR, out actual);

			Asserts.AreEqual (expected, actual, "multiply");
		}


		[Test]
		public void Multiply_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4d) inputL;
			var inputSimdR = (NMatrix4d) inputR;
			var expected = inputL * inputR;
			var actual = inputSimdL * inputSimdR;

			Asserts.AreEqual (expected, actual, "multiply");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4d) inputL;
			var inputSimdR = (NMatrix4d) inputR;

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

			Assert.IsTrue (NMatrix4d.Identity == (NMatrix4d) Matrix4d.Identity, "identity equality");
		}

		[Test]
		public void Inequality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4d) inputL;
			var inputSimdR = (NMatrix4d) inputR;

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

			Assert.IsFalse (NMatrix4d.Identity != (NMatrix4d) Matrix4d.Identity, "identity equality");
		}

		[Test]
		public void Explicit_Operator_ToMatrix4d ()
		{
			var expected = (NMatrix4d) GetTestMatrix ();
			var actual = (Matrix4d) expected;

			Asserts.AreEqual (expected, actual, "tomatrix4");

			actual = (Matrix4d) NMatrix4d.Identity;
			Asserts.AreEqual (NMatrix4d.Identity, actual, "tomatrix4 identity");
			Asserts.AreEqual (Matrix4d.Identity, actual, "tomatrix4 identity2");
		}

		[Test]
		public void Explicit_Operator_FromMatrix4d ()
		{
			var expected = GetTestMatrix ();
			var actual = (NMatrix4d) expected;

			Asserts.AreEqual (expected, actual, "frommatrix4");

			actual = (NMatrix4d) Matrix4d.Identity;
			Asserts.AreEqual (NMatrix4d.Identity, actual, "tomatrix4 identity");
			Asserts.AreEqual (Matrix4d.Identity, actual, "tomatrix4 identity2");
		}

		[Test]
		public void ToStringTest ()
		{
			var expected = GetTestMatrix ();
			var actual = (NMatrix4d) expected;

			Assert.AreEqual (expected.ToString (), actual.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestMatrix ();
			var expectedB = GetTestMatrix ();
			var actualA = (NMatrix4d) expectedA;
			var actualB = (NMatrix4d) expectedB;

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
			var actualA = (NMatrix4d) expectedA;
			var actualB = (NMatrix4d) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		// A collection of test matrices.
		//
		// I initially tried randomly generating test matrices, but it turns out
		// there are accumulative computational differences in the different algorithms
		// between Matrix4d and NMatrix4d. Since the differences are accumulative,
		// I couldn't find a minimal sensible delta values when comparing
		// matrices.
		//
		// So I just serialized a few matrices that were randomly generated, and
		// these have been tested to not produce accumulative computational differences.
		//
		static Matrix4d [] test_matrices = new [] {
			new Matrix4d (0.1532144d, 0.5451511d, 0.2004739d, 0.8351463d, 0.9884372d, 0.1313103d, 0.3327205d, 0.01164342d, 0.6563147d, 0.7923161d, 0.6764754d, 0.07481737d, 0.03239552d, 0.7156482d, 0.6136858d, 0.1864168f),
			new Matrix4d (0.7717745d, 0.559364d, 0.00918373d, 0.6579159d, 0.123461d, 0.9993145d, 0.5487496d, 0.2823398d, 0.9710717d, 0.8750508d, 0.472472d, 0.2608089d, 0.5771761d, 0.5617125d, 0.176998d, 0.1271691f),
			new Matrix4d (0.2023053d, 0.4701468d, 0.6618567d, 0.7685714d, 0.8561344d, 0.009231919d, 0.6150167d, 0.7542298d, 0.550727d, 0.3625788d, 0.6639862d, 0.5763468d, 0.9717328d, 0.003812184d, 0.985266d, 0.7540002f),
			new Matrix4d (9.799572E+08d, 1.64794E+09d, 1.117296E+09d, 1.239858E+09d, 6.389504E+07d, 1.172175E+09d, 1.399567E+09d, 1.187143E+09d, 3.729208E+07d, 5.50313E+08d, 1.847369E+09d, 1.612405E+09d, 1.699488E+08d, 4.952176E+08d, 1.07262E+09d, 2.035059E+09f),
			new Matrix4d (1.102396E+09d, 3.082477E+08d, 1.126484E+09d, 5.022931E+08d, 1.966322E+09d, 1.1814E+09d, 8.464673E+08d, 1.940651E+09d, 1.229937E+09d, 1.367379E+09d, 1.900015E+09d, 1.516109E+09d, 2.146064E+09d, 1.870971E+09d, 1.046267E+09d, 1.088363E+09f),
			new Matrix4d (2.263112E+08d, 8.79644E+08d, 1.303282E+09d, 1.654159E+09d, 3.705524E+08d, 1.984941E+09d, 2.175935E+07d, 4.633518E+08d, 1.801243E+09d, 1.616996E+09d, 1.620852E+09d, 7291498d, 1.012728E+09d, 2.834145E+08d, 3.5328E+08d, 1.35012E+09f),
			new Matrix4d (0.4904693d, 0.841727d, 0.2294401d, 0.5736054d, 0.5406881d, 0.2172498d, 0.1261143d, 0.6736677d, 0.4570194d, 0.9091009d, 0.7669608d, 0.8468134d, 0.01802658d, 0.3850208d, 0.3730424d, 0.2440258f),
			new Matrix4d (0.1252193d, 0.08986127d, 0.3407605d, 0.9144857d, 0.340791d, 0.2192288d, 0.5144276d, 0.01813344d, 0.07687104d, 0.7971596d, 0.6393988d, 0.9002907d, 0.1011457d, 0.5047605d, 0.7202546d, 0.07729452f),
			new Matrix4d (8.176959E+08d, 1.386156E+09d, 5.956444E+08d, 4.210506E+08d, 1.212676E+09d, 4.131035E+08d, 1.032453E+09d, 2.074689E+08d, 1.536594E+09d, 3.266183E+07d, 5.222072E+08d, 7.923175E+08d, 1.762531E+09d, 7.901702E+08d, 8.1975E+08d, 1.630734E+09f),
			new Matrix4d (0.006755914d, 0.07464754d, 0.287938d, 0.3724834d, 0.1496783d, 0.6224982d, 0.7150125d, 0.5554719d, 0.4638171d, 0.4200902d, 0.4867154d, 0.773377d, 0.3558737d, 0.4043404d, 0.04670618d, 0.7695189d),
		};

		static int counter;
		internal static Matrix4d GetTestMatrix ()
		{
			counter++;
			if (counter == test_matrices.Length)
				counter = 0;
			return test_matrices [counter];
		}
#endif
	}
}
