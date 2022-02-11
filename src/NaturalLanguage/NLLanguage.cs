using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using System.Runtime.Versioning;

namespace NaturalLanguage {

#if NET
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
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
