using System;
using System.Collections.Generic;

using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace Xamarin.iOS.UnitTests.NUnit {
	public class ClassOrNamespaceFilter : TestFilter {
		bool isClassFilter;
		List<string> names;

		public ClassOrNamespaceFilter (string name, bool isClassFilter)
		{
			AddName (name);
			this.isClassFilter = isClassFilter;
		}

		public ClassOrNamespaceFilter (IEnumerable<string> names, bool isClassFilter)
		{
			if (names is null)
				throw new ArgumentNullException (nameof (names));

			this.isClassFilter = isClassFilter;
			foreach (string n in names) {
				string name = n?.Trim ();
				if (String.IsNullOrEmpty (name))
					continue;

				AddName (name);
			}
		}

		public void AddName (string name)
		{
			if (String.IsNullOrEmpty (name))
				throw new ArgumentException ("must not be null or empty", nameof (name));

			if (names is null)
				names = new List<string> ();
			if (names.Contains (name))
				return;

			names.Add (name);
		}

		public override bool Match (ITest test)
		{
			if (test is null || names is null || names.Count == 0)
				return false;

			if (test.FixtureType is null)
				return false; // It's probably an assembly name, all tests will have a fixture

			if (isClassFilter)
				return NameMatches (test.FixtureType.FullName);

			int dot = test.FixtureType.FullName.LastIndexOf ('.');
			if (dot < 1)
				return false;

			return NameMatches (test.FixtureType.FullName.Substring (0, dot));
		}

		bool NameMatches (string name)
		{
			foreach (string n in names) {
				if (n is null)
					continue;
				if (String.Compare (name, n, StringComparison.Ordinal) == 0)
					return true;
			}

			return false;
		}
	}
}
