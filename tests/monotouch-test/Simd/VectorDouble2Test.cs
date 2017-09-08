
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
	public class VectorDouble2Test {

		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestVector ();
			var actual = new VectorDouble2 (expected.X, expected.Y);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (VectorDouble2) inputL;
			var inputSimdR = (VectorDouble2) inputR;

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
			var inputSimdL = (VectorDouble2) inputL;
			var inputSimdR = (VectorDouble2) inputR;

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
		public void Explicit_Operator_ToVector2 ()
		{
			var expected = GetTestVector ();
			var actual = (Vector2d) expected;

			Asserts.AreEqual (expected, actual, "ToVector2d");
		}

		[Test]
		public void Explicit_Operator_FromVector2 ()
		{
			var expected = new Vector2d (2, 3);
			var actual = (VectorDouble2) expected;

			Asserts.AreEqual (expected, actual, "FromVector2d");
		}

		[Test]
		public void ToStringTest ()
		{
			var vector = new VectorDouble2 (1, 2);

			Assert.AreEqual ("(1, 2)", vector.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (VectorDouble2) expectedA;
			var actualB = (VectorDouble2) expectedB;

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
			var actualA = (VectorDouble2) expectedA;
			var actualB = (VectorDouble2) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		static VectorDouble2 [] test_vectors = new [] {
			new VectorDouble2 (0.1532144d, 0.5451511d),
			new VectorDouble2 (0.7717745d, 0.559364d),
			new VectorDouble2 (0.2023053d, 0.4701468d),
			new VectorDouble2 (0.4904693d, 0.841727d),
			new VectorDouble2 (0.1252193d, 0.08986127d),
			new VectorDouble2 (0.006755914d, 0.07464754d),
			new VectorDouble2 (9.799572E+08d, 1.64794E+09d),
			new VectorDouble2 (1.102396E+09d, 3.082477E+08d),
			new VectorDouble2 (2.263112E+08d, 8.79644E+08d),
			new VectorDouble2 (8.176959E+08d, 1.386156E+09d),
		};

		static int counter;
		internal static VectorDouble2 GetTestVector ()
		{
			counter++;
			if (counter == test_vectors.Length)
				counter = 0;
			return test_vectors [counter];
		}
	}
}
