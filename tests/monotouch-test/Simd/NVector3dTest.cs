
using System;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

#if NET
using NVector3d = global::CoreGraphics.NVector3d;
#else
using OpenTK;
using NVector3d = global::OpenTK.NVector3d;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Simd {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NVector3dTest {

		[Test]
		public void ElementConstructor ()
		{
			var expected = GetTestVector ();
			var actual = new NVector3d (expected.X, expected.Y, expected.Z);
			Asserts.AreEqual (expected, actual, "ctor 1");
		}

		[Test]
		public void Equality_Operator ()
		{
			var inputL = GetTestVector ();
			var inputR = GetTestVector ();
			var inputSimdL = (NVector3d) inputL;
			var inputSimdR = (NVector3d) inputR;

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
			var inputSimdL = (NVector3d) inputL;
			var inputSimdR = (NVector3d) inputR;

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

#if !NET
		[Test]
		public void Explicit_Operator_ToVector3 ()
		{
			var expected = GetTestVector ();
			var actual = (Vector3d) expected;

			Asserts.AreEqual (expected, actual, "ToVector3d");
		}

		[Test]
		public void Explicit_Operator_FromVector3 ()
		{
			var expected = new Vector3d (2, 3, 7.7d);
			var actual = (NVector3d) expected;

			Asserts.AreEqual (expected, actual, "FromVector3d");
		}
#endif // !NET

		[Test]
		public void ToStringTest ()
		{
			var vector = new NVector3d (1, 2, 3);

			Assert.AreEqual ("(1, 2, 3)", vector.ToString (), "tostring");
		}

		// GetHashCode doesn't have to be identical, so no need to test

		[Test]
		public void Equals_Object ()
		{
			var expectedA = GetTestVector ();
			var expectedB = GetTestVector ();
			var actualA = (NVector3d) expectedA;
			var actualB = (NVector3d) expectedB;

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
			var actualA = (NVector3d) expectedA;
			var actualB = (NVector3d) expectedB;

			Assert.IsTrue (actualA.Equals (actualA), "self");
			Assert.IsFalse (actualA.Equals (actualB), "other");
		}

		static NVector3d [] test_vectors = new [] {
			new NVector3d (0.1532144d, 0.5451511d, 0.2004739d),
			new NVector3d (0.7717745d, 0.559364d, 0.00918373d),
			new NVector3d (0.2023053d, 0.4701468d, 0.6618567d),
			new NVector3d (0.4904693d, 0.841727d, 0.2294401d),
			new NVector3d (0.1252193d, 0.08986127d, 0.3407605d),
			new NVector3d (0.006755914d, 0.07464754d, 0.287938d),
			new NVector3d (9.799572E+08d, 1.64794E+09d, 1.117296E+09d),
			new NVector3d (1.102396E+09d, 3.082477E+08d, 1.126484E+09d),
			new NVector3d (2.263112E+08d, 8.79644E+08d, 1.303282E+09d),
			new NVector3d (8.176959E+08d, 1.386156E+09d, 5.956444E+08d),
		};

		static int counter;
		internal static NVector3d GetTestVector ()
		{
			counter++;
			if (counter == test_vectors.Length)
				counter = 0;
			return test_vectors [counter];
		}
	}
}
