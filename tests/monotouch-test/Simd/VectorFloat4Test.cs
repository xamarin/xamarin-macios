
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
	public class VectorFloat4Test
	{
		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestVector ();
			var actual = new VectorFloat4 (expected.X, expected.Y, expected.Z, expected.W);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (VectorFloat4) inputL;
			var inputSimdR = (VectorFloat4) inputR;

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
			var inputSimdL = (VectorFloat4) inputL;
			var inputSimdR = (VectorFloat4) inputR;

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
			var actual = (Vector4) expected;

			Asserts.AreEqual (expected, actual, "ToVector4");
		}

		[Test]
		public void Explicit_Operator_FromVector4 ()
		{
			var expected = new Vector4 (2, 3, 7.7f, 99.12f);
			var actual = (VectorFloat4) expected;

			Asserts.AreEqual (expected, actual, "FromVector4");
		}

		[Test]
		public void ToStringTest ()
		{
			var vector = new VectorFloat4 (1, 2, 3, 4);

			Assert.AreEqual ("(1, 2, 3, 4)", vector.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (VectorFloat4) expectedA;
			var actualB = (VectorFloat4) expectedB;

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
			var actualA = (VectorFloat4) expectedA;
			var actualB = (VectorFloat4) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		static VectorFloat4 [] test_vectors = new [] {
			new VectorFloat4 (0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f),
			new VectorFloat4 (0.7717745f, 0.559364f, 0.00918373f, 0.6579159f),
			new VectorFloat4 (0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f),
			new VectorFloat4 (0.4904693f, 0.841727f, 0.2294401f, 0.5736054f),
			new VectorFloat4 (0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f),
			new VectorFloat4 (0.006755914f, 0.07464754f, 0.287938f, 0.3724834f),
			new VectorFloat4 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f),
			new VectorFloat4 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f),
			new VectorFloat4 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f),
			new VectorFloat4 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f),
		};

		static int counter;
		internal static VectorFloat4 GetTestVector ()
		{
			counter++;
			if (counter == test_vectors.Length)
				counter = 0;
			return test_vectors [counter];
		}
	}
}
