using System;
using ObjCRuntime;

namespace Foundation {

#if !XAMCORE_5_0
	// Utility enum, ObjC uses NSString
	public enum NSDocumentType {
		Unknown = -1,
		PlainText,
		RTF,
		RTFD,
		HTML,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		MacSimpleText,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		DocFormat,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		WordML,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		OfficeOpenXml,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		WebArchive,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		OpenDocument,
	}
#endif // !XAMCORE_5_0

	// Utility enum, ObjC uses NSString
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

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

	[NoWatch, NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSUrlSessionMultipathServiceType : long {
		None = 0,
		Handover = 1,
		Interactive = 2,
		Aggregate = 3,
	}

	public enum NSLinguisticTagScheme {
		[Field ("NSLinguisticTagSchemeTokenType")]
		Token,

		[Field ("NSLinguisticTagSchemeLexicalClass")]
		LexicalClass,

		[Field ("NSLinguisticTagSchemeNameType")]
		Name,

		[Field ("NSLinguisticTagSchemeNameTypeOrLexicalClass")]
		NameOrLexicalClass,

		[Field ("NSLinguisticTagSchemeLemma")]
		Lemma,

		[Field ("NSLinguisticTagSchemeLanguage")]
		Language,

		[Field ("NSLinguisticTagSchemeScript")]
		Script,
	}

#if !NET
	public enum NSLinguisticTagUnit {
#else
	public enum NSLinguisticTag {
#endif
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

	[Flags]
	[Native]
	public enum NSStringEnumerationOptions : ulong {
		ByLines = 0x0,
		ByParagraphs = 0x1,
		ByComposedCharacterSequences = 0x2,
		ByWords = 0x3,
		BySentences = 0x4,
		ByCaretPositions = 0x5,
		ByDeletionClusters = 0x6,
		Reverse = 1uL << 8,
		SubstringNotRequired = 1uL << 9,
		Localized = 1uL << 10,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0), Watch (8, 0)]
	[Flags]
	[Native]
	public enum NSAttributedStringFormattingOptions : ulong {
		InsertArgumentAttributesWithoutMerging = 1uL << 0,
		ApplyReplacementIndexAttribute = 1uL << 1,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSAttributedStringMarkdownInterpretedSyntax : long {
		Full = 0,
		InlineOnly = 1,
		InlineOnlyPreservingWhitespace = 2,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSAttributedStringMarkdownParsingFailurePolicy : long {
		Error = 0,
		PartiallyParsedIfPossible = 1,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSGrammaticalGender : long {
		NotSet = 0,
		Feminine,
		Masculine,
		Neuter,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSGrammaticalNumber : long {
		NotSet = 0,
		Singular,
		Zero,
		Plural,
		PluralTwo,
		PluralFew,
		PluralMany,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSGrammaticalPartOfSpeech : long {
		NotSet = 0,
		Determiner,
		Pronoun,
		Letter,
		Adverb,
		Particle,
		Adjective,
		Adposition,
		Verb,
		Noun,
		Conjunction,
		Numeral,
		Interjection,
		Preposition,
		Abbreviation,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSInlinePresentationIntent : ulong {
		Emphasized = 1uL << 0,
		StronglyEmphasized = 1uL << 1,
		Code = 1uL << 2,
		Strikethrough = 1uL << 5,
		SoftBreak = 1uL << 6,
		LineBreak = 1uL << 7,
		InlineHTML = 1uL << 8,
		BlockHTML = 1uL << 9,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSPresentationIntentKind : long {
		Paragraph,
		Header,
		OrderedList,
		UnorderedList,
		ListItem,
		CodeBlock,
		BlockQuote,
		ThematicBreak,
		Table,
		TableHeaderRow,
		TableRow,
		TableCell,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSPresentationIntentTableColumnAlignment : long {
		Left,
		Center,
		Right,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSURLRequestAttribution : ulong {
		Developer = 0,
		User = 1,
	}
}
