using System;
using System.Collections.Generic;

using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace Xamarin.iOS.UnitTests.NUnit
{
	public class TestMethodFilter : TestFilter
	{
		List <string> methods;

		public TestMethodFilter (string method)
		{
			AddMethod (method);
		}

		public TestMethodFilter (IEnumerable<string> methods)
		{
			if (methods == null)
				throw new ArgumentNullException (nameof (methods));

			foreach (string m in methods) {
				AddMethod (m);
			}
		}

		public void AddMethod (string method)
		{
			if (string.IsNullOrEmpty (method))
				throw new ArgumentException ("must not be null or empty", nameof (method));

			if (methods == null)
				methods = new List <string> ();
			if (methods.Contains (method))
				return;

			methods.Add (method);
		}

		public override bool Match (ITest test)
		{
			if (test == null || methods == null || methods.Count == 0)
				return false;

			if (test.FixtureType == null)
				return false; // It's probably an assembly name, all tests will have a fixture

			return true;
		}

		public override bool Pass (ITest test)
		{
			if (test == null || methods == null || methods.Count == 0)
				return false;

			if (test.FixtureType == null)
				return false; // It's probably an assembly name, all tests will have a fixture

			// if the  method is not present, execute the test
			return !methods.Contains (test.FullName);
		}

	}
}