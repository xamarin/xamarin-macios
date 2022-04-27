#nullable enable

using System;
using System.Collections.Generic;

using Foundation;

namespace NaturalLanguage {

	public partial class NLTagger {

		public Dictionary<NLLanguage, double> GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NLTagScheme scheme, nuint maximumCount)
		{
			var constant = scheme.GetConstant ();
			if (constant is null)
				throw new ArgumentOutOfRangeException (nameof (scheme));
			using (var hypo = GetNativeTagHypotheses (characterIndex, unit, constant, maximumCount))
				return NLLanguageExtensions.Convert (hypo);
		}

		public Dictionary<NLLanguage, double> GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NLTagScheme scheme, nuint maximumCount, out NSRange tokenRange)
		{
			var constant = scheme.GetConstant ();
			if (constant is null)
				throw new ArgumentOutOfRangeException (nameof (scheme));
			using (var hypo = GetNativeTagHypotheses (characterIndex, unit, constant, maximumCount, out tokenRange))
				return NLLanguageExtensions.Convert (hypo);
		}
	}
}
