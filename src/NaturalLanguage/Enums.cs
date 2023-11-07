// Copyright 2018-2019 Microsoft Corp.
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
	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	public enum NLTaggerOptions : ulong {
		OmitWords = 1uL << 0,
		OmitPunctuation = 1uL << 1,
		OmitWhitespace = 1uL << 2,
		OmitOther = 1uL << 3,
		JoinNames = 1uL << 4,
		JoinContractions = 1uL << 5,
	}

	[Native]
	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	public enum NLModelType : long {
		Classifier,
		Sequence,
	}

	[Native]
	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	public enum NLTokenUnit : long {
		Word,
		Sentence,
		Paragraph,
		Document,
	}


	[Flags]
	[Native]
	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	public enum NLTokenizerAttributes : ulong {
		Numeric = 1uL << 0,
		Symbolic = 1uL << 1,
		Emoji = 1uL << 2,
	}

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	public enum NLLanguage {
		[DefaultEnumValue]
		[Field (null)]
		Unevaluated,
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
		Ukrainian,
		[Field ("NLLanguageUrdu")]
		Urdu,
		[Field ("NLLanguageVietnamese")]
		Vietnamese,

		[iOS (16, 0), Mac (13, 0), Watch (9, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Field ("NLLanguageKazakh")]
		Kazakh,
	}

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
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
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NLTagSchemeSentimentScore")]
		SentimentScore,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum NLScript {
		[Field ("NLScriptUndetermined")]
		Undetermined,
		[Field ("NLScriptArabic")]
		Arabic,
		[Field ("NLScriptArmenian")]
		Armenian,
		[Field ("NLScriptBengali")]
		Bengali,
		[Field ("NLScriptCanadianAboriginalSyllabics")]
		CanadianAboriginalSyllabics,
		[Field ("NLScriptCherokee")]
		Cherokee,
		[Field ("NLScriptCyrillic")]
		Cyrillic,
		[Field ("NLScriptDevanagari")]
		Devanagari,
		[Field ("NLScriptEthiopic")]
		Ethiopic,
		[Field ("NLScriptGeorgian")]
		Georgian,
		[Field ("NLScriptGreek")]
		Greek,
		[Field ("NLScriptGujarati")]
		Gujarati,
		[Field ("NLScriptGurmukhi")]
		Gurmukhi,
		[Field ("NLScriptHebrew")]
		Hebrew,
		[Field ("NLScriptJapanese")]
		Japanese,
		[Field ("NLScriptKannada")]
		Kannada,
		[Field ("NLScriptKhmer")]
		Khmer,
		[Field ("NLScriptKorean")]
		Korean,
		[Field ("NLScriptLao")]
		Lao,
		[Field ("NLScriptLatin")]
		Latin,
		[Field ("NLScriptMalayalam")]
		Malayalam,
		[Field ("NLScriptMongolian")]
		Mongolian,
		[Field ("NLScriptMyanmar")]
		Myanmar,
		[Field ("NLScriptOriya")]
		Oriya,
		[Field ("NLScriptSimplifiedChinese")]
		SimplifiedChinese,
		[Field ("NLScriptSinhala")]
		Sinhala,
		[Field ("NLScriptTamil")]
		Tamil,
		[Field ("NLScriptTelugu")]
		Telugu,
		[Field ("NLScriptThai")]
		Thai,
		[Field ("NLScriptTibetan")]
		Tibetan,
		[Field ("NLScriptTraditionalChinese")]
		TraditionalChinese,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NLContextualEmbeddingAssetsResult : long {
		Available,
		NotAvailable,
		Error,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum NLContextualEmebeddingKey {
		[Field ("NLContextualEmbeddingKeyLanguages")]
		Languages,
		[Field ("NLContextualEmbeddingKeyScripts")]
		Scripts,
		[Field ("NLContextualEmbeddingKeyRevision")]
		Revision,
	}
}
