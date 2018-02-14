//
// searchkit.cs: Definitions for AppKit
//

using System;
using Foundation;
using ObjCRuntime;

namespace SearchKit {
	[Static]
	interface SKTextAnalysisKeys {
		[Field ("kSKMinTermLength")]
		NSString MinTermLengthKey { get; }
		
		[Field ("kSKStopWords")]
		NSString StopWordsKey { get; }
		
		[Field ("kSKSubstitutions")]
		NSString SubstitutionsKey { get; }
		
		[Field ("kSKMaximumTerms")]
		NSString MaximumTermsKey { get; }
		
		[Field ("kSKProximityIndexing")]
		NSString ProximityIndexingKey { get; }
		
		[Field ("kSKTermChars")]
		NSString TermCharsKey { get; }
		
		[Field ("kSKStartTermChars")]
		NSString StartTermCharsKey { get; }
		
		[Field ("kSKEndTermChars")]
		NSString EndTermCharsKey { get; }
	}

	[StrongDictionary ("SKTextAnalysisKeys")]
	interface SKTextAnalysis {
		int MinTermLength { get; set; }
		NSSet    StopWords { get; set; }
		NSDictionary Substitutions { get; set; }
		NSNumber MaximumTerms { get; set; }
		bool ProximityIndexing { get; set; }
		string TermChars { get; set; }
		string StartTermChars { get; set; }
		string EndTermChars { get; set; }
	}
}
