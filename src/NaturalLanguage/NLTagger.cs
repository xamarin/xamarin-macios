using System;
using System.Collections.Generic;

using Foundation;

namespace NaturalLanguage {

	public partial class NLTagger {

		public Dictionary<NLLanguage, double> GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NLTagScheme scheme, nuint maximumCount)
		{
			using (var hypo = GetNativeTagHypotheses (characterIndex, unit, scheme.GetConstant (), maximumCount))
				return NLLanguageExtensions.Convert (hypo);
		}

		public Dictionary<NLLanguage, double> GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NLTagScheme scheme, nuint maximumCount, out NSRange tokenRange)
		{
			using (var hypo = GetNativeTagHypotheses (characterIndex, unit, scheme.GetConstant (), maximumCount, out tokenRange))
				return NLLanguageExtensions.Convert (hypo);
		}
	}
}
