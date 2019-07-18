using System;
using System.Collections.Generic;

using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace Xamarin.iOS.UnitTests.NUnit
{
	public class TestMethodFilter : TestFilter
	{
		HashSet <string> methods = new HashSet<string> ();

		public TestMethodFilter (string method)
		{
			if (string.IsNullOrEmpty (method))
				throw new ArgumentException (nameof (method));
			Add (method);
		}

		public TestMethodFilter (IEnumerable<string> methods)
		{
			if (methods == null)
				throw new ArgumentNullException (nameof (methods));

			AddRange (methods);
		}

		public void Add (string method)
		{
			if (string.IsNullOrEmpty (method))
				throw new ArgumentException ("must not be null or empty", nameof (method));

			methods.Add (method);
		}
		
		public void AddRange (IEnumerable<string> ignoredMethods)
		{
			if (methods == null)
				throw new ArgumentNullException (nameof (ignoredMethods));
			foreach (var m in ignoredMethods)
				methods.Add (m);
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