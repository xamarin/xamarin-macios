
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

using OpenTK;
using Simd;

using NUnit.Framework;

namespace MonoTouchFixtures.Simd {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VectorDouble4Test {
		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestVector ();
			var actual = new VectorDouble4 (expected.X, expected.Y, expected.Z, expected.W);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (VectorDouble4) inputL;
			var inputSimdR = (VectorDouble4) inputR;

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
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (VectorDouble4) inputL;
			var inputSimdR = (VectorDouble4) inputR;

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
		public void Explicit_Operator_ToVector4 ()
		{
			var expected = GetTestVector ();
			var actual = (Vector4d) expected;

			Asserts.AreEqual (expected, actual, "ToVector4d");
		}

		[Test]
		public void Explicit_Operator_FromVector4 ()
		{
			var expected = new Vector4d (2, 3, 7.7d, 99.12d);
			var actual = (VectorDouble4) expected;

			Asserts.AreEqual (expected, actual, "FromVector4d");
		}

		[Test]
		public void ToStringTest ()
		{
			var vector = new VectorDouble4 (1, 2, 3, 4);

			Assert.AreEqual ("(1, 2, 3, 4)", vector.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (VectorDouble4) expectedA;
			var actualB = (VectorDouble4) expectedB;

			Assert.IsTrue (actualA.Equals ((object) actualA), "self");
			Assert.IsFalse (actualA.Equals ((object) actualB), "other");
			Assert.IsFalse (actualA.Equals (null), "null");
			Assert.IsTrue (actualA.Equals (expectedA), "same type");
		}

		[Test]
		public void Equals_Vector ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (VectorDouble4) expectedA;
			var actualB = (VectorDouble4) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		static VectorDouble4 [] test_vectors = new [] {
			new VectorDouble4 (0.1532144d, 0.5451511d, 0.2004739d, 0.8351463d),
			new VectorDouble4 (0.7717745d, 0.559364d, 0.00918373d, 0.6579159d),
			new VectorDouble4 (0.2023053d, 0.4701468d, 0.6618567d, 0.7685714d),
			new VectorDouble4 (0.4904693d, 0.841727d, 0.2294401d, 0.5736054d),
			new VectorDouble4 (0.1252193d, 0.08986127d, 0.3407605d, 0.9144857d),
			new VectorDouble4 (0.006755914d, 0.07464754d, 0.287938d, 0.3724834d),
			new VectorDouble4 (9.799572E+08d, 1.64794E+09d, 1.117296E+09d, 1.239858E+09d),
			new VectorDouble4 (1.102396E+09d, 3.082477E+08d, 1.126484E+09d, 5.022931E+08d),
			new VectorDouble4 (2.263112E+08d, 8.79644E+08d, 1.303282E+09d, 1.654159E+09d),
			new VectorDouble4 (8.176959E+08d, 1.386156E+09d, 5.956444E+08d, 4.210506E+08d),
		};

		static int counter;
		internal static VectorDouble4 GetTestVector ()
		{
			counter++;
			if (counter == test_vectors.Length)
				counter = 0;
			return test_vectors [counter];
		}
	}
}
