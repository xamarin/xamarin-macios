using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace MonoTests {
	/*
		class CategoryAttribute : Attribute 
		{
			public string Category { get; set; }

			public CategoryAttribute (string category)
			{
				this.Category = category;
			}
		}
	/*	
		static class Assert
		{
			public static void AreEqual (object a, object b, string msg)
			{
				NUnit.Framework.Assert.That (a, Is.EqualTo (b), msg);
			}

			public static void AreEqual (object a, object b)
			{
				NUnit.Framework.Assert.That (a, Is.EqualTo (b));
			}

			public static void IsNotNull (object o, string msg)
			{
				NUnit.Framework.Assert.That (o, Is.Not.Null, msg);
			}

			public static void IsNotNull (object o)
			{
				NUnit.Framework.Assert.That (o, Is.Not.Null);
			}

			public static void IsNull (object o, string msg)
			{
				NUnit.Framework.Assert.That (o, Is.Null, msg);
			}

			public static void IsNull (object o)
			{
				NUnit.Framework.Assert.That (o, Is.Null);
			}

			public static void IsTrue (object o, string msg)
			{
				NUnit.Framework.Assert.That (o, Is.True, msg);
			}

			public static void IsTrue (object o)
			{
				NUnit.Framework.Assert.That (o, Is.True);
			}

			public static void IsFalse (object o, string msg)
			{
				NUnit.Framework.Assert.That (o, Is.False, msg);
			}

			public static void IsFalse (object o)
			{
				NUnit.Framework.Assert.That (o, Is.False);
			}

			public static void AreSame (object a, object b)
			{
				NUnit.Framework.Assert.That (a, Is.SameAs (b));
			}

			public static void AreSame (object a, object b, string msg)
			{
				NUnit.Framework.Assert.That (a, Is.SameAs (b), msg);
			}

			public static void Fail (string msg)
			{
				NUnit.Framework.Assert.Fail (msg);
			}
		}
	*/
	// nunit 1.x compatibility
	public class TestCase {
		protected virtual void SetUp ()
		{
		}

		public static void Assert (string msg, bool condition)
		{
			NUnit.Framework.Assert.True (condition, msg);
		}

		public static void AssertEquals (object a, object b)
		{
			NUnit.Framework.Assert.That (a, Is.EqualTo (b));
		}

		public static void AssertEquals (string msg, object a, object b)
		{
			NUnit.Framework.Assert.That (a, Is.EqualTo (b), msg);
		}

		public static void AssertNull (object a)
		{
			NUnit.Framework.Assert.That (a, Is.Null);
		}

		public static void AssertNull (string msg, object a)
		{
			NUnit.Framework.Assert.That (a, Is.Null, msg);
		}

		public static void AssertNotNull (object a)
		{
			NUnit.Framework.Assert.That (a, Is.Not.Null);
		}

		public static void AssertNotNull (string msg, object a)
		{
			NUnit.Framework.Assert.That (a, Is.Not.Null, msg);
		}

		public static void Fail (string msg)
		{
			NUnit.Framework.Assert.Fail (msg);
		}
	}

	public class Assertion : TestCase {
	}

	public class TestFixtureSetUpAttribute : SetUpAttribute {
	}

	public class StringAssert {
		#region StartsWith
		static public void StartsWith (string expected, string actual, string message, params object [] args)
		{
			Assert.That (actual, new StartsWithConstraint (expected), message, args);
		}

		static public void StartsWith (string expected, string actual, string message)
		{
			StartsWith (expected, actual, message, null);
		}

		static public void StartsWith (string expected, string actual)
		{
			StartsWith (expected, actual, string.Empty, null);
		}
		#endregion

		#region Contains
		static public void Contains (string expected, string actual, string message, params object [] args)
		{
			Assert.That (actual, new SubstringConstraint (expected), message, args);
		}

		static public void Contains (string expected, string actual, string message)
		{
			Contains (expected, actual, message, null);
		}

		static public void Contains (string expected, string actual)
		{
			Contains (expected, actual, string.Empty, null);
		}
		#endregion
	}
}
