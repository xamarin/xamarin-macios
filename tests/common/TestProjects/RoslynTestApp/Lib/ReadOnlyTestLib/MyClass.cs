using System;
using System.Collections.Generic;

namespace ReadOnlyTestLib {
	public class MyClass {
		static IReadOnlyCollection<string> Test ()
		{
			var dict = new Dictionary<string, string> {
				["foo"] = "bar"
			};

			return (IReadOnlyCollection<string>) dict.Values;
		}
	}
}
