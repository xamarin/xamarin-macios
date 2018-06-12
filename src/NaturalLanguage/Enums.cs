// Copyright 2018, Microsoft Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;
using Foundation;
using ObjCRuntime;

namespace NaturalLanguage {

	[Flags]
	[Native]
	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLTaggerOptions : ulong {
		OmitWords = 1uL << 0,
		OmitPunctuation = 1uL << 1,
		OmitWhitespace = 1uL << 2,
		OmitOther = 1uL << 3,
		JoinNames = 1uL << 4,
		JoinContractions = 1uL << 5,
	}

	[Native]
	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLModelType : ulong {
		Classifier,
		Sequence,
	}

	[Native]
	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLTokenUnit : ulong {
		Word,
		Sentence,
		Paragraph,
		Document,
	}


	[Flags]
	[Native]
	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLTokenizerAttributes : ulong {
		Numeric = 1uL << 0,
		Symbolic = 1uL << 1,
		Emoji = 1uL << 2,
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLLanguage {
		[DefaultEnumValue]
		[Field ("NLLanguageUndetermined")]
		Undetermined,
		[Field ("NLLanguageAmharic")]
		Amharic,
		[Field ("NLLanguageArabic")]
		Arabic,
		[Field ("NLLanguageArmenian")]
		Armenian,
		[Field ("NLLanguageBengali")]
		Bengali,
		[Field ("NLLanguageBulgarian")]
		Bulgarian,
		[Field ("NLLanguageBurmese")]
		Burmese,
		[Field ("NLLanguageCatalan")]
		Catalan,
		[Field ("NLLanguageCherokee")]
		Cherokee,
		[Field ("NLLanguageCroatian")]
		Croatian,
		[Field ("NLLanguageCzech")]
		Czech,
		[Field ("NLLanguageDanish")]
		Danish,
		[Field ("NLLanguageDutch")]
		Dutch,
		[Field ("NLLanguageEnglish")]
		English,
		[Field ("NLLanguageFinnish")]
		Finnish,
		[Field ("NLLanguageFrench")]
		French,
		[Field ("NLLanguageGeorgian")]
		Georgian,
		[Field ("NLLanguageGerman")]
		German,
		[Field ("NLLanguageGreek")]
		Greek,
		[Field ("NLLanguageGujarati")]
		Gujarati,
		[Field ("NLLanguageHebrew")]
		Hebrew,
		[Field ("NLLanguageHindi")]
		Hindi,
		[Field ("NLLanguageHungarian")]
		Hungarian,
		[Field ("NLLanguageIcelandic")]
		Icelandic,
		[Field ("NLLanguageIndonesian")]
		Indonesian,
		[Field ("NLLanguageItalian")]
		Italian,
		[Field ("NLLanguageJapanese")]
		Japanese,
		[Field ("NLLanguageKannada")]
		Kannada,
		[Field ("NLLanguageKhmer")]
		Khmer,
		[Field ("NLLanguageKorean")]
		Korean,
		[Field ("NLLanguageLao")]
		Lao,
		[Field ("NLLanguageMalay")]
		Malay,
		[Field ("NLLanguageMalayalam")]
		Malayalam,
		[Field ("NLLanguageMarathi")]
		Marathi,
		[Field ("NLLanguageMongolian")]
		Mongolian,
		[Field ("NLLanguageNorwegian")]
		Norwegian,
		[Field ("NLLanguageOriya")]
		Oriya,
		[Field ("NLLanguagePersian")]
		Persian,
		[Field ("NLLanguagePolish")]
		Polish,
		[Field ("NLLanguagePortuguese")]
		Portuguese,
		[Field ("NLLanguagePunjabi")]
		Punjabi,
		[Field ("NLLanguageRomanian")]
		Romanian,
		[Field ("NLLanguageRussian")]
		Russian,
		[Field ("NLLanguageSimplifiedChinese")]
		SimplifiedChinese,
		[Field ("NLLanguageSinhalese")]
		Sinhalese,
		[Field ("NLLanguageSlovak")]
		Slovak,
		[Field ("NLLanguageSpanish")]
		Spanish,
		[Field ("NLLanguageSwedish")]
		Swedish,
		[Field ("NLLanguageTamil")]
		Tamil,
		[Field ("NLLanguageTelugu")]
		Telugu,
		[Field ("NLLanguageThai")]
		Thai,
		[Field ("NLLanguageTibetan")]
		Tibetan,
		[Field ("NLLanguageTraditionalChinese")]
		TraditionalChinese,
		[Field ("NLLanguageTurkish")]
		Turkish,
		[Field ("NLLanguageUkrainian")]
		NLLanguageUkrainian,
		[Field ("NLLanguageUrdu")]
		Urdu,
		[Field ("NLLanguageVietnamese")]
		Vietnamese,
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLTagScheme {
		[Field ("NLTagSchemeTokenType")]
		TokenType,
		[Field ("NLTagSchemeLexicalClass")]
		LexicalClass,
		[Field ("NLTagSchemeNameType")]
		NameType,
		[Field ("NLTagSchemeNameTypeOrLexicalClass")]
		NameTypeOrLexicalClass,
		[Field ("NLTagSchemeLemma")]
		Lemma,
		[Field ("NLTagSchemeLanguage")]
		Language,
		[Field ("NLTagSchemeScript")]
		Script,
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	public enum NLTag {
		[Field ("NLTagWord")]
		Word,
		[Field ("NLTagPunctuation")]
		Punctuation,
		[Field ("NLTagWhitespace")]
		Whitespace,
		[Field ("NLTagOther")]
		Other, 
		[Field ("NLTagNoun")]
		Noun,
		[Field ("NLTagVerb")]
		Verb,
		[Field ("NLTagAdjective")]
		Adjective,
		[Field ("NLTagAdverb")]
		Adverb,
		[Field ("NLTagPronoun")]
		Pronoun,
		[Field ("NLTagDeterminer")]
		Determiner,
		[Field ("NLTagParticle")]
		Particle,
		[Field ("NLTagPreposition")]
		Preposition,
		[Field ("NLTagNumber")]
		Number,
		[Field ("NLTagConjunction")]
		Conjunction,
		[Field ("NLTagInterjection")]
		Interjection,
		[Field ("NLTagClassifier")]
		Classifier,
		[Field ("NLTagIdiom")]
		Idiom,
		[Field ("NLTagOtherWord")]
		OtherWord,
		[Field ("NLTagSentenceTerminator")]
		SentenceTerminator,
		[Field ("NLTagOpenQuote")]
		OpenQuote,
		[Field ("NLTagCloseQuote")]
		CloseQuote,
		[Field ("NLTagOpenParenthesis")]
		OpenParenthesis, 
		[Field ("NLTagCloseParenthesis")]
		CloseParenthesis,
		[Field ("NLTagWordJoiner")]
		WordJoiner,
		[Field ("NLTagDash")]
		Dash,
		[Field ("NLTagOtherPunctuation")]
		OtherPunctuation,
		[Field ("NLTagParagraphBreak")]
		ParagraphBreak,
		[Field ("NLTagOtherWhitespace")]
		OtherWhitespace,
		[Field ("NLTagPersonalName")]
		PersonalName,
		[Field ("NLTagPlaceName")]
		PlaceName,
		[Field ("NLTagOrganizationName")]
		OrganizationName, 
	}

}
