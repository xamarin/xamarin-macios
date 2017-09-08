
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

using OpenTK;
using Simd;

using NUnit.Framework;

namespace MonoTouchFixtures.Simd
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VectorFloat2Test
	{
		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestVector ();
			var actual = new VectorFloat2 (expected.X, expected.Y);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (VectorFloat2) inputL;
			var inputSimdR = (VectorFloat2) inputR;

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
			var inputSimdL = (VectorFloat2) inputL;
			var inputSimdR = (VectorFloat2) inputR;

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
			var actual = (Vector2) expected;

			Asserts.AreEqual (expected, actual, "ToVector2");
		}

		[Test]
		public void Explicit_Operator_FromVector2 ()
		{
			var expected = new Vector2 (2, 3);
			var actual = (VectorFloat2) expected;

			Asserts.AreEqual (expected, actual, "FromVector2");
		}

		[Test]
		public void ToStringTest ()
		{
			var vector = new VectorFloat2 (1, 2);

			Assert.AreEqual ("(1, 2)", vector.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (VectorFloat2) expectedA;
			var actualB = (VectorFloat2) expectedB;

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
			var actualA = (VectorFloat2) expectedA;
			var actualB = (VectorFloat2) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		static VectorFloat2 [] test_vectors = new [] {
			new VectorFloat2 (0.1532144f, 0.5451511f),
			new VectorFloat2 (0.7717745f, 0.559364f),
			new VectorFloat2 (0.2023053f, 0.4701468f),
			new VectorFloat2 (0.4904693f, 0.841727f),
			new VectorFloat2 (0.1252193f, 0.08986127f),
			new VectorFloat2 (0.006755914f, 0.07464754f),
			new VectorFloat2 (9.799572E+08f, 1.64794E+09f),
			new VectorFloat2 (1.102396E+09f, 3.082477E+08f),
			new VectorFloat2 (2.263112E+08f, 8.79644E+08f),
			new VectorFloat2 (8.176959E+08f, 1.386156E+09f),
		};

		static int counter;
		internal static VectorFloat2 GetTestVector ()
		{
			counter++;
			if (counter == test_vectors.Length)
				counter = 0;
			return test_vectors [counter];
		}
	}
}
