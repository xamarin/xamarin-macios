
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
	public class VectorInt4Test
	{
		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestVector ();
			var actual = new VectorInt4 (expected.X, expected.Y, expected.Z, expected.W);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (VectorInt4) inputL;
			var inputSimdR = (VectorInt4) inputR;

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
			var inputSimdL = (VectorInt4) inputL;
			var inputSimdR = (VectorInt4) inputR;

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
		public void Explicit_Operator_ToVector4i ()
		{
			var expected = GetTestVector ();
			var actual = (Vector4i) expected;

			Asserts.AreEqual (expected, actual, "ToVector4i");
		}

		[Test]
		public void Explicit_Operator_FromVector4i ()
		{
			var expected = new Vector4i (2, 3, 7, 99);
			var actual = (VectorInt4) expected;

			Asserts.AreEqual (expected, actual, "FromVector4i");
		}

		[Test]
		public void ToStringTest ()
		{
			var vector = new VectorInt4 (1, 2, 3, 4);

			Assert.AreEqual ("(1, 2, 3, 4)", vector.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (VectorInt4) expectedA;
			var actualB = (VectorInt4) expectedB;

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
			var actualA = (VectorInt4) expectedA;
			var actualB = (VectorInt4) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		static VectorInt4 [] test_vectors = new [] {
			new VectorInt4 (1532144, 5451511, 2004739, 8351463),
			new VectorInt4 (7717745, 559364, 00918373, 6579159),
			new VectorInt4 (2023053, 4701468, 6618567, 7685714),
			new VectorInt4 (4904693, 841727, 2294401, 5736054),
			new VectorInt4 (1252193, 08986127, 3407605, 9144857),
			new VectorInt4 (006755914, 07464754, 287938, 3724834),
			new VectorInt4 (9, 1, 1, 1),
			new VectorInt4 (1, 3, 1, 5),
			new VectorInt4 (2, 8, 1, 1),
			new VectorInt4 (8, 1, 5, 4),
		};

		static int counter;
		internal static VectorInt4 GetTestVector ()
		{
			counter++;
			if (counter == test_vectors.Length)
				counter = 0;
			return test_vectors [counter];
		}
	}
}
