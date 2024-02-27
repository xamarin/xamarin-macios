
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

#if NET
using NMatrix4x3 = global::CoreGraphics.NMatrix4x3;
#else
using NMatrix4x3 = global::OpenTK.NMatrix4x3;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Simd {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NMatrix4x3Test {
		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestMatrix ();
			var actual = new NMatrix4x3 (
				expected.M11, expected.M12, expected.M13, expected.M14,
				expected.M21, expected.M22, expected.M23, expected.M24,
				expected.M31, expected.M32, expected.M33, expected.M34);
			Asserts.AreEqual (expected, actual, "ctor 1");

		}

		[Test]
		public void Elements ()
		{
			var expected = GetTestMatrix ();
			var actual = (NMatrix4x3) expected;

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
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4x3) inputL;
			var inputSimdR = (NMatrix4x3) inputR;

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
		}

		[Test]
		public void Inequality_Operator ()
		{
			var inputL = GetTestMatrix ();
			var inputR = GetTestMatrix ();
			var inputSimdL = (NMatrix4x3) inputL;
			var inputSimdR = (NMatrix4x3) inputR;

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
		}

		[Test]
		public void ToStringTest ()
		{
			var expected = GetTestMatrix ();
			var actual = (NMatrix4x3) expected;

			Assert.AreEqual (expected.ToString (), actual.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestMatrix ();
			var expectedB = GetTestMatrix ();
			var actualA = (NMatrix4x3) expectedA;
			var actualB = (NMatrix4x3) expectedB;

			Assert.IsTrue (actualA.Equals ((object) actualA), "self");
			Assert.IsFalse (actualA.Equals ((object) actualB), "other");
			Assert.IsFalse (actualA.Equals (null), "null");
			Assert.IsTrue (actualA.Equals (expectedA), "other type");
		}

		[Test]
		public void Equals_Matrix ()
		{
			var expectedA = GetTestMatrix ();
			var expectedB = GetTestMatrix ();
			var actualA = (NMatrix4x3) expectedA;
			var actualB = (NMatrix4x3) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		// A collection of test matrices.
		//
		// I initially tried randomly generating test matrices, but it turns out
		// there are accumulative computational differences in the different algorithms
		// between Matrix4 and NMatrix4x3. Since the differences are accumulative,
		// I couldn't find a minimal sensible delta values when comparing 
		// matrices.
		//
		// So I just serialized a few matrices that were randomly generated, and
		// these have been tested to not produce accumulative computational differences.
		// 
		static NMatrix4x3 [] test_matrices = new [] {
			new NMatrix4x3 (0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f, 0.9884372f, 0.1313103f, 0.3327205f, 0.01164342f, 0.6563147f, 0.7923161f, 0.6764754f, 0.07481737f),
			new NMatrix4x3 (0.7717745f, 0.559364f, 0.00918373f, 0.6579159f, 0.123461f, 0.9993145f, 0.5487496f, 0.2823398f, 0.9710717f, 0.8750508f, 0.472472f, 0.2608089f),
			new NMatrix4x3 (0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f, 0.8561344f, 0.009231919f, 0.6150167f, 0.7542298f, 0.550727f, 0.3625788f, 0.6639862f, 0.5763468f),
			new NMatrix4x3 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f, 6.389504E+07f, 1.172175E+09f, 1.399567E+09f, 1.187143E+09f, 3.729208E+07f, 5.50313E+08f, 1.847369E+09f, 1.612405E+09f),
			new NMatrix4x3 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f, 1.966322E+09f, 1.1814E+09f, 8.464673E+08f, 1.940651E+09f, 1.229937E+09f, 1.367379E+09f, 1.900015E+09f, 1.516109E+09f),
			new NMatrix4x3 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f, 3.705524E+08f, 1.984941E+09f, 2.175935E+07f, 4.633518E+08f, 1.801243E+09f, 1.616996E+09f, 1.620852E+09f, 7291498f),
			new NMatrix4x3 (0.4904693f, 0.841727f, 0.2294401f, 0.5736054f, 0.5406881f, 0.2172498f, 0.1261143f, 0.6736677f, 0.4570194f, 0.9091009f, 0.7669608f, 0.8468134f),
			new NMatrix4x3 (0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f, 0.340791f, 0.2192288f, 0.5144276f, 0.01813344f, 0.07687104f, 0.7971596f, 0.6393988f, 0.9002907f),
			new NMatrix4x3 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f, 1.212676E+09f, 4.131035E+08f, 1.032453E+09f, 2.074689E+08f, 1.536594E+09f, 3.266183E+07f, 5.222072E+08f, 7.923175E+08f),
			new NMatrix4x3 (0.006755914f, 0.07464754f, 0.287938f, 0.3724834f, 0.1496783f, 0.6224982f, 0.7150125f, 0.5554719f, 0.4638171f, 0.4200902f, 0.4867154f, 0.773377f),
		};

		static int counter;
		internal static NMatrix4x3 GetTestMatrix ()
		{
			counter++;
			if (counter == test_matrices.Length)
				counter = 0;
			return test_matrices [counter];
		}
	}
}
