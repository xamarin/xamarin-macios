using System;
using System.Collections.Generic;

using NUnit.Framework.Internal;
using NUnit.Framework.Interfaces;

namespace MonoTouch.NUnit.UI {
	class ExcludeCategoryFilter : TestFilter {
		public HashSet<string> ExcludedCategories { get; private set; }

		public ExcludeCategoryFilter (IEnumerable<string> categories)
		{
			ExcludedCategories = new HashSet<string> (categories);
		}

		public override TNode AddToXml (TNode parentNode, bool recursive)
		{
			throw new NotImplementedException ();
		}

		public override bool Match (ITest test)
		{
			var categories = test.Properties ["Category"];
			if (categories != null) {
				foreach (string cat in categories) {
					if (ExcludedCategories.Contains (cat))
						return false;
				}
			}
			
			if (test.Parent != null)
				return Match (test.Parent);

			return true;
		}

		public override bool Pass (ITest test)
		{
			return Match (test);
		}
	}
}
