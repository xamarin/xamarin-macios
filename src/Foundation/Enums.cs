using ObjCRuntime;

namespace Foundation {

	// Utility enum, ObjC uses NSString
	[NoMac]
	[iOS (7,0)]
	public enum NSDocumentType {
		Unknown = -1,
		PlainText,
		RTF,
		RTFD,
		HTML
	}

	// Utility enum, ObjC uses NSString
	[NoMac]
	[iOS (7,0)]
	public enum NSDocumentViewMode {
		Normal,
		PageLayout
			
	}

	public enum NSRunLoopMode {

		[DefaultEnumValue]
		[Field ("NSDefaultRunLoopMode")]
		Default,

		[Field ("NSRunLoopCommonModes")]
		Common,

#if MONOMAC
		[Field ("NSConnectionReplyMode")]
		ConnectionReply = 2,

		[Field ("NSModalPanelRunLoopMode", "AppKit")]
		ModalPanel,

		[Field ("NSEventTrackingRunLoopMode", "AppKit")]
		EventTracking,
#elif !WATCH
		// iOS-specific Enums start in 100 to avoid conflicting with future extensions to MonoMac
		[Field ("UITrackingRunLoopMode", "UIKit")]
		UITracking = 100,
#endif
		// If it is not part of these enumerations
		[Field (null)]
		Other = 1000
	}

	[Mac (10,9)]
	[iOS (7,0)]
	public enum NSItemDownloadingStatus {
		[Field (null)]
		Unknown = -1,

		[Field ("NSMetadataUbiquitousItemDownloadingStatusCurrent")]
		Current,

		[Field ("NSMetadataUbiquitousItemDownloadingStatusDownloaded")]
		Downloaded,

		[Field ("NSMetadataUbiquitousItemDownloadingStatusNotDownloaded")]
		NotDownloaded,
	}

	[iOS (9,0)][Mac (10,11)]
	public enum NSStringTransform {
		[Field ("NSStringTransformLatinToKatakana")]
		LatinToKatakana,

		[Field ("NSStringTransformLatinToHiragana")]
		LatinToHiragana,

		[Field ("NSStringTransformLatinToHangul")]
		LatinToHangul,

		[Field ("NSStringTransformLatinToArabic")]
		LatinToArabic,

		[Field ("NSStringTransformLatinToHebrew")]
		LatinToHebrew,

		[Field ("NSStringTransformLatinToThai")]
		LatinToThai,

		[Field ("NSStringTransformLatinToCyrillic")]
		LatinToCyrillic,

		[Field ("NSStringTransformLatinToGreek")]
		LatinToGreek,

		[Field ("NSStringTransformToLatin")]
		ToLatin,

		[Field ("NSStringTransformMandarinToLatin")]
		MandarinToLatin,

		[Field ("NSStringTransformHiraganaToKatakana")]
		HiraganaToKatakana,

		[Field ("NSStringTransformFullwidthToHalfwidth")]
		FullwidthToHalfwidth,

		[Field ("NSStringTransformToXMLHex")]
		ToXmlHex,

		[Field ("NSStringTransformToUnicodeName")]
		ToUnicodeName,

		[Field ("NSStringTransformStripCombiningMarks")]
		StripCombiningMarks,

		[Field ("NSStringTransformStripDiacritics")]
		StripDiacritics,
	}

	[NoWatch, NoTV, NoMac, iOS (11, 0)]
	[Native]
	public enum NSUrlSessionMultipathServiceType : long {
		None = 0,
		Handover = 1,
		Interactive = 2,
		Aggregate = 3,
	}

	public enum NSLinguisticTagSchemeType {
		[Field ("NSLinguisticTagSchemeTokenType")]
		Token,

		[Field ("NSLinguisticTagSchemeLexicalClass")]
		LexicalClass,

		[Field ("NSLinguisticTagSchemeNameType")]
		Name,

		[Field ("NSLinguisticTagSchemeNameTypeOrLexicalClass")]
		NameOrLexicalClass,

		[Field ("NSLinguisticTagSchemeLemma")]
		SchemeLemma,

		[Field ("NSLinguisticTagSchemeLanguage")]
		Language,

		[Field ("NSLinguisticTagSchemeScript")]
		SchemeScript,
	}

	public enum NSLinguisticTagScheme {
		[Field ("NSLinguisticTagWord")]
		Word,

		[Field ("NSLinguisticTagPunctuation")]
 		Punctuation,

		[Field ("NSLinguisticTagWhitespace")]
 		Whitespace,

		[Field ("NSLinguisticTagOther")]
 		Other,

		[Field ("NSLinguisticTagNoun")]
		Noun,

		[Field ("NSLinguisticTagVerb")]
		Verb,

		[Field ("NSLinguisticTagAdjective")]
		Adjective,

		[Field ("NSLinguisticTagAdverb")]
		Adverb,

		[Field ("NSLinguisticTagPronoun")]
		Pronoun,

		[Field ("NSLinguisticTagDeterminer")]
		Determiner,

		[Field ("NSLinguisticTagParticle")]
		Particle,

		[Field ("NSLinguisticTagPreposition")]
		Preposition,

		[Field ("NSLinguisticTagNumber")]
		Number,

		[Field ("NSLinguisticTagConjunction")]
		Conjunction,

		[Field ("NSLinguisticTagInterjection")]
		Interjection,

		[Field ("NSLinguisticTagClassifier")]
		Classifier,

		[Field ("NSLinguisticTagIdiom")]
		Idiom,

		[Field ("NSLinguisticTagOtherWord")]
		OtherWord,

		[Field ("NSLinguisticTagSentenceTerminator")]
		Terminator,

		[Field ("NSLinguisticTagOpenQuote")]
		OpenQuote,

		[Field ("NSLinguisticTagCloseQuote")]
		CloseQuote,

		[Field ("NSLinguisticTagOpenParenthesis")]
		OpenParenthesis,

		[Field ("NSLinguisticTagCloseParenthesis")]
		CloseParenthesis,

		[Field ("NSLinguisticTagWordJoiner")]
		WordJoiner,

		[Field ("NSLinguisticTagDash")]
		Dash,

		[Field ("NSLinguisticTagOtherPunctuation")]
		OtherPunctuation,

		[Field ("NSLinguisticTagParagraphBreak")]
		ParagraphBreak,

		[Field ("NSLinguisticTagOtherWhitespace")]
		OtherWhitespace,

		[Field ("NSLinguisticTagPersonalName")]
		PersonalName,

		[Field ("NSLinguisticTagOrganizationName")]
		OrganizationName,

		[Field ("NSLinguisticTagPlaceName")]
		PlaceName,
	}
}
