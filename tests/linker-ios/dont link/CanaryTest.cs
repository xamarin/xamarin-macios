using System;
using System.Reflection;
using NUnit.Framework;

namespace DontLink {

	[TestFixture]
	public class CanaryTest {

		static void AssertCallStaticReturnBool (Type t, string method, bool expected)
		{
			AssertCallStaticReturnBool (t, method, null, expected);
		}

		static void AssertCallStaticReturnBool (Type t, string method, object [] parameters, bool expected)
		{
			var m = t.GetMethod (method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.NotNull (m, $"{t.Name}::{method} is missing");
			var result = (bool)m.Invoke (null, parameters);
			Assert.That (result, Is.EqualTo (expected), $"{t.Name}::{method} returned `{result}` not the expected `{expected}`");
		}

		[Test]
		public void Mscorlib ()
		{
			// type is internal - but we need to ensure it does not change the value it returns (without a corresponding update to InlinerSubStep.cs)
			var bc = Type.GetType ("System.Runtime.Versioning.BinaryCompatibility, mscorlib");
			Assert.IsNotNull (bc, "BinaryCompatibility");
			AssertCallStaticReturnBool (bc, "get_TargetsAtLeast_Desktop_V4_5", true);
			AssertCallStaticReturnBool (bc, "get_TargetsAtLeast_Desktop_V4_5_1", false);
		}
	}
}
