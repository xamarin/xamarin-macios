#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;

namespace NaturalLanguage {

	public partial class NLLanguageExtensions {

		static internal Dictionary<NLLanguage, double> Convert (NSDictionary<NSString, NSNumber> dict)
		{
			var result = new Dictionary<NLLanguage, double> ((int) dict.Count);
			foreach (var k in dict.Keys) {
				result [NLLanguageExtensions.GetValue (k)] = dict [k].DoubleValue;
			}
			return result;
		}
	}
}
