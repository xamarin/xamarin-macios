
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

#if NET
using MatrixFloat2x2 = global::CoreGraphics.NMatrix2;
#else
using OpenTK;
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Simd {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MatrixFloat2x2Test {
		[Test]
		public void Identity ()
		{
			var identity = new MatrixFloat2x2 {
				R0C0 = 1f,
				R1C1 = 1f,
			};
			Asserts.AreEqual (identity, MatrixFloat2x2.Identity, "identity");
#if !NET
			Asserts.AreEqual (Matrix2.Identity, MatrixFloat2x2.Identity, "opentk identity");
#endif
		}

#if !NET //we no longer have OpenTK.Matrix2 to compare against
		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestMatrix ();
			var actual = new MatrixFloat2x2 (expected.R0C0, expected.R0C1,
											 expected.R1C0, expected.R1C1);
			Asserts.AreEqual (expected, actual, "ctor 1");

		}

		[Test]
		public void Determinant ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat2x2) expected;
			Assert.AreEqual (expected.Determinant, actual.Determinant, 0.000001f, "determinant\n" + actual);
		}

		[Test]
		public void Elements ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat2x2) expected;

			Assert.AreEqual (expected.R0C0, actual.R0C0, "R0C0 getter");
			Assert.AreEqual (expected.R0C1, actual.R0C1, "R0C1 getter");
			Assert.AreEqual (expected.R1C0, actual.R1C0, "R1C0 getter");
			Assert.AreEqual (expected.R1C1, actual.R1C1, "R1C1 getter");

			var newExpected = GetTestMatrix ();
			actual.R0C0 = newExpected.R0C0;
			actual.R0C1 = newExpected.R0C1;
			actual.R1C0 = newExpected.R1C0;
			actual.R1C1 = newExpected.R1C1;
			Assert.AreEqual (newExpected.R0C0, actual.R0C0, "R0C0 setter");
			Assert.AreEqual (newExpected.R0C1, actual.R0C1, "R0C1 setter");
			Assert.AreEqual (newExpected.R1C0, actual.R1C0, "R1C0 setter");
			Assert.AreEqual (newExpected.R1C1, actual.R1C1, "R1C1 setter");
		}

		[Test]
		public void TransposeInstance ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat2x2) expected;

			expected.Transpose ();
			actual.Transpose ();

			Asserts.AreEqual (expected, actual, "transpose");
		}

		[Test]
		public void TransposeStatic ()
		{
			var input = GetTestMatrix ();
			var inputSimd = (MatrixFloat2x2) input;

			Matrix2 expected;
			Matrix2.Transpose (ref input, out expected);
			var actual = MatrixFloat2x2.Transpose (inputSimd);

			Asserts.AreEqual (expected, actual, "transpose");

			input = GetTestMatrix ();
			inputSimd = (MatrixFloat2x2) input;
			Matrix2.Transpose (ref input, out expected);
			MatrixFloat2x2.Transpose (ref inputSimd, out actual);
			Asserts.AreEqual (expected, actual, "transpose out/ref");
		}

		[Test]
		public void TransposeStatic_ByRef ()
		{
			var input = GetTestMatrix ();
			var inputSimd = (MatrixFloat2x2) input;

			Matrix2 expected;
			MatrixFloat2x2 actual;

			Matrix2.Transpose (ref input, out expected);
			MatrixFloat2x2.Transpose (ref inputSimd, out actual);
			Asserts.AreEqual (expected, actual, "transpose out/ref");
		}

		[Test]
		public void Multiply ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat2x2) inputL;
			var inputSimdR = (MatrixFloat2x2) inputR;
			Matrix2 expected;
			Matrix2.Multiply (ref inputR, ref inputL, out expected); // OpenTK.Matrix2 got left/right mixed up...
			var actual = MatrixFloat2x2.Multiply (inputSimdL, inputSimdR);

			Asserts.AreEqual (expected, actual, "multiply");
		}

		[Test]
		public void Multiply_ByRef ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat2x2) inputL;
			var inputSimdR = (MatrixFloat2x2) inputR;
			Matrix2 expected;
			MatrixFloat2x2 actual;

			Matrix2.Multiply (ref inputR, ref inputL, out expected); // OpenTK.Matrix2 got left/right mixed up...
			MatrixFloat2x2.Multiply (ref inputSimdL, ref inputSimdR, out actual);

			Asserts.AreEqual (expected, actual, "multiply");
		}


		[Test]
		public void Multiply_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat2x2) inputL;
			var inputSimdR = (MatrixFloat2x2) inputR;
			Matrix2 expected;
			Matrix2.Multiply (ref inputR, ref inputL, out expected); // OpenTK.Matrix2 got left/right mixed up...
			var actual = inputSimdL * inputSimdR;

			Asserts.AreEqual (expected, actual, "multiply");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat2x2) inputL;
			var inputSimdR = (MatrixFloat2x2) inputR;

			// matrices are different
			Assert.AreEqual (inputL.Equals (inputR), inputSimdL == inputSimdR, "inequality");
			Assert.IsFalse (inputL.Equals (inputR), "inequality 2 expected");
			Assert.IsFalse (inputSimdL == inputSimdR, "inequality 2 actual");

			inputL = inputR;
			inputSimdL = inputSimdR;
			// matrices are identical
			Assert.AreEqual (inputL.Equals (inputR), inputSimdL == inputSimdR, "equality");
			Assert.IsTrue (inputL.Equals (inputR), "equality 2 expected");
			Assert.IsTrue (inputSimdL == inputSimdR, "equality 2 actual");

			Assert.IsTrue (MatrixFloat2x2.Identity == (MatrixFloat2x2) Matrix2.Identity, "identity equality");
		}

		[Test]
		public void Inequality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (MatrixFloat2x2) inputL;
			var inputSimdR = (MatrixFloat2x2) inputR;

			// matrices are different
			Assert.AreEqual (!inputL.Equals (inputR), inputSimdL != inputSimdR, "inequality");
			Assert.IsTrue (!inputL.Equals (inputR), "inequality 2 expected");
			Assert.IsTrue (inputSimdL != inputSimdR, "inequality 2 actual");

			inputL = inputR;
			inputSimdL = inputSimdR;
			// matrices are identical
			Assert.AreEqual (!inputL.Equals (inputR), inputSimdL != inputSimdR, "equality");
			Assert.IsFalse (!inputL.Equals (inputR), "equality 2 expected");
			Assert.IsFalse (inputSimdL != inputSimdR, "equality 2 actual");

			Assert.IsFalse (MatrixFloat2x2.Identity != (MatrixFloat2x2) Matrix2.Identity, "identity equality");
		}

		[Test]
		public void Explicit_Operator_ToMatrix2 ()
		{
			var expected = (MatrixFloat2x2) GetTestMatrix ();
			var actual = (Matrix2) expected;

			Asserts.AreEqual (expected, actual, "tomatrix2");

			actual = (Matrix2) MatrixFloat2x2.Identity;
			Asserts.AreEqual (MatrixFloat2x2.Identity, actual, "tomatrix2 identity");
			Asserts.AreEqual (Matrix2.Identity, actual, "tomatrix2 identity2");
		}

		[Test]
		public void Explicit_Operator_FromMatrix2 ()
		{
			var expected = GetTestMatrix ();
			var actual = (MatrixFloat2x2) expected;

			Asserts.AreEqual (expected, actual, "frommatrix2");

			actual = (MatrixFloat2x2) Matrix2.Identity;
			Asserts.AreEqual (MatrixFloat2x2.Identity, actual, "tomatrix2 identity");
			Asserts.AreEqual (Matrix2.Identity, actual, "tomatrix2 identity2");
		}
#endif // !NET

		[Test]
		public void ToStringTest ()
		{
			var actual = new MatrixFloat2x2 (1, 2, 3, 4);

			Assert.AreEqual ("(1, 2)\n(3, 4)", actual.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

#if !NET
		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestMatrix ();
			var expectedB = GetTestMatrix ();
			var actualA = (MatrixFloat2x2) expectedA;
			var actualB = (MatrixFloat2x2) expectedB;

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
			var actualA = (MatrixFloat2x2) expectedA;
			var actualB = (MatrixFloat2x2) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		// A collection of test matrices.
		//
		// I initially tried randomly generating test matrices, but it turns out
		// there are accumulative computational differences in the different algorithms
		// between Matrix2 and MatrixFloat2x2. Since the differences are accumulative,
		// I couldn't find a minimal sensible delta values when comparing 
		// matrices.
		//
		// So I just serialized a few matrices that were randomly generated, and
		// these have been tested to not produce accumulative computational differences.
		// 
		static Matrix2 [] test_matrices = new [] {
			new Matrix2 (3, 5, 7, 11),
			new Matrix2 (5, 7, 11, 13),
			new Matrix2 (7, 11, 13, 17),
			new Matrix2 (0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f),
			new Matrix2 (0.7717745f, 0.559364f, 0.00918373f, 0.6579159f),
			new Matrix2 (0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f),
			new Matrix2 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f),
			new Matrix2 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f),
			new Matrix2 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f),
			new Matrix2 (0.4904693f, 0.841727f, 0.2294401f, 0.5736054f),
			new Matrix2 (0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f),
			new Matrix2 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f),
			new Matrix2 (0.006755914f, 0.07464754f, 0.287938f, 0.3724834f),
		};

		static int counter;
		internal static Matrix2 GetTestMatrix ()
		{
			counter++;
			if (counter == test_matrices.Length)
				counter = 0;
			return test_matrices [counter];
		}
#endif // !NET
	}
}
