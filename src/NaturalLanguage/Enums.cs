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

	/// <summary>Enumerates preprocessing options for tags.</summary>
	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum NLTaggerOptions : ulong {
		OmitWords = 1uL << 0,
		OmitPunctuation = 1uL << 1,
		OmitWhitespace = 1uL << 2,
		OmitOther = 1uL << 3,
		JoinNames = 1uL << 4,
		JoinContractions = 1uL << 5,
	}

	/// <summary>Enumerates natural language model types.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NLModelType : long {
		/// <summary>Indicates a model that tags text at a level of groups of tokens, such as sentences or paragraphs.</summary>
		Classifier,
		/// <summary>Indicates a model that tags text at the individual token level.</summary>
		Sequence,
	}

	/// <summary>Enumerates linguistic units to which tags can be applied.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NLTokenUnit : long {
		Word,
		Sentence,
		Paragraph,
		Document,
	}


	/// <summary>Enumerates content hints for tokenizers.</summary>
	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum NLTokenizerAttributes : ulong {
		Numeric = 1uL << 0,
		Symbolic = 1uL << 1,
		Emoji = 1uL << 2,
	}

	/// <summary>Enumerates languages for which recognition is supported.</summary>
	[MacCatalyst (13, 1)]
	public enum NLLanguage {
		/// <summary>To be added.</summary>
		[DefaultEnumValue]
		[Field (null)]
		Unevaluated,
		/// <summary>Indicates that the language was not recognized.</summary>
		[Field ("NLLanguageUndetermined")]
		Undetermined,
		/// <summary>Indicates the Amharic language.</summary>
		[Field ("NLLanguageAmharic")]
		Amharic,
		/// <summary>Indicates the Arabic language.</summary>
		[Field ("NLLanguageArabic")]
		Arabic,
		/// <summary>Indicates the Armenian language.</summary>
		[Field ("NLLanguageArmenian")]
		Armenian,
		/// <summary>Indicates the Bengali language.</summary>
		[Field ("NLLanguageBengali")]
		Bengali,
		/// <summary>Indicates the Bulgarian language.</summary>
		[Field ("NLLanguageBulgarian")]
		Bulgarian,
		/// <summary>Indicates the Burmese language.</summary>
		[Field ("NLLanguageBurmese")]
		Burmese,
		/// <summary>Indicates the Catalan language.</summary>
		[Field ("NLLanguageCatalan")]
		Catalan,
		/// <summary>Indicates the Cherokee language.</summary>
		[Field ("NLLanguageCherokee")]
		Cherokee,
		/// <summary>Indicates the Croatian language.</summary>
		[Field ("NLLanguageCroatian")]
		Croatian,
		/// <summary>Indicates the Czech language.</summary>
		[Field ("NLLanguageCzech")]
		Czech,
		/// <summary>Indicates the Danish language.</summary>
		[Field ("NLLanguageDanish")]
		Danish,
		/// <summary>Indicates the Dutch language.</summary>
		[Field ("NLLanguageDutch")]
		Dutch,
		/// <summary>Indicates the English language.</summary>
		[Field ("NLLanguageEnglish")]
		English,
		/// <summary>Indicates the Finnish language.</summary>
		[Field ("NLLanguageFinnish")]
		Finnish,
		/// <summary>Indicates the French language.</summary>
		[Field ("NLLanguageFrench")]
		French,
		/// <summary>Indicates the Georgian language.</summary>
		[Field ("NLLanguageGeorgian")]
		Georgian,
		/// <summary>Indicates the German language.</summary>
		[Field ("NLLanguageGerman")]
		German,
		/// <summary>Indicates the Greek language.</summary>
		[Field ("NLLanguageGreek")]
		Greek,
		/// <summary>Indicates the Gujarati language.</summary>
		[Field ("NLLanguageGujarati")]
		Gujarati,
		/// <summary>Indicates the Hebrew language.</summary>
		[Field ("NLLanguageHebrew")]
		Hebrew,
		/// <summary>Indicates the Hindi language.</summary>
		[Field ("NLLanguageHindi")]
		Hindi,
		/// <summary>Indicates the Hungarian language.</summary>
		[Field ("NLLanguageHungarian")]
		Hungarian,
		/// <summary>Indicates the Icelandic language.</summary>
		[Field ("NLLanguageIcelandic")]
		Icelandic,
		/// <summary>Indicates the Indonesian language.</summary>
		[Field ("NLLanguageIndonesian")]
		Indonesian,
		/// <summary>Indicates the Italian language.</summary>
		[Field ("NLLanguageItalian")]
		Italian,
		/// <summary>Indicates the Japanese language.</summary>
		[Field ("NLLanguageJapanese")]
		Japanese,
		/// <summary>Indicates the Kannada language.</summary>
		[Field ("NLLanguageKannada")]
		Kannada,
		/// <summary>Indicates the Khmer language.</summary>
		[Field ("NLLanguageKhmer")]
		Khmer,
		/// <summary>Indicates the Korean language.</summary>
		[Field ("NLLanguageKorean")]
		Korean,
		/// <summary>Indicates the Lao language.</summary>
		[Field ("NLLanguageLao")]
		Lao,
		/// <summary>Indicates the Malay language.</summary>
		[Field ("NLLanguageMalay")]
		Malay,
		/// <summary>Indicates the Malayalam language.</summary>
		[Field ("NLLanguageMalayalam")]
		Malayalam,
		/// <summary>Indicates the Marathi language.</summary>
		[Field ("NLLanguageMarathi")]
		Marathi,
		/// <summary>Indicates the Mongolian language.</summary>
		[Field ("NLLanguageMongolian")]
		Mongolian,
		/// <summary>Indicates the Norwegian language.</summary>
		[Field ("NLLanguageNorwegian")]
		Norwegian,
		/// <summary>Indicates the Oriya language.</summary>
		[Field ("NLLanguageOriya")]
		Oriya,
		/// <summary>Indicates the Persian language.</summary>
		[Field ("NLLanguagePersian")]
		Persian,
		/// <summary>Indicates the Polish language.</summary>
		[Field ("NLLanguagePolish")]
		Polish,
		/// <summary>Indicates the Portuguese language.</summary>
		[Field ("NLLanguagePortuguese")]
		Portuguese,
		/// <summary>Indicates the Punjabi language.</summary>
		[Field ("NLLanguagePunjabi")]
		Punjabi,
		/// <summary>Indicates the Romanian language.</summary>
		[Field ("NLLanguageRomanian")]
		Romanian,
		/// <summary>Indicates the Russian language.</summary>
		[Field ("NLLanguageRussian")]
		Russian,
		/// <summary>Indicates the Simplified Chinese language character set.</summary>
		[Field ("NLLanguageSimplifiedChinese")]
		SimplifiedChinese,
		/// <summary>Indicates the Sinhalese language.</summary>
		[Field ("NLLanguageSinhalese")]
		Sinhalese,
		/// <summary>Indicates the Slovak language.</summary>
		[Field ("NLLanguageSlovak")]
		Slovak,
		/// <summary>Indicates the Spanish language.</summary>
		[Field ("NLLanguageSpanish")]
		Spanish,
		/// <summary>Indicates the Swedish language.</summary>
		[Field ("NLLanguageSwedish")]
		Swedish,
		/// <summary>Indicates the Tamil language.</summary>
		[Field ("NLLanguageTamil")]
		Tamil,
		/// <summary>Indicates the Telugu language.</summary>
		[Field ("NLLanguageTelugu")]
		Telugu,
		/// <summary>Indicates the Thai language.</summary>
		[Field ("NLLanguageThai")]
		Thai,
		/// <summary>Indicates the Tibetan language.</summary>
		[Field ("NLLanguageTibetan")]
		Tibetan,
		/// <summary>Indicates the Traditional Chinese character set.</summary>
		[Field ("NLLanguageTraditionalChinese")]
		TraditionalChinese,
		/// <summary>Indicates the Turkish language.</summary>
		[Field ("NLLanguageTurkish")]
		Turkish,
		/// <summary>Indicates the Ukrainian language.</summary>
		[Field ("NLLanguageUkrainian")]
		Ukrainian,
		/// <summary>Indicates the Urdu language.</summary>
		[Field ("NLLanguageUrdu")]
		Urdu,
		/// <summary>Indicates the Vietnamese language.</summary>
		[Field ("NLLanguageVietnamese")]
		Vietnamese,

		[iOS (16, 0), Mac (13, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Field ("NLLanguageKazakh")]
		Kazakh,
	}

	/// <summary>Enumerates classes of tags that are returned from a text classifier.</summary>
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
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NLTagSchemeSentimentScore")]
		SentimentScore,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NLContextualEmbeddingAssetsResult : long {
		Available,
		NotAvailable,
		Error,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum NLContextualEmebeddingKey {
		[Field ("NLContextualEmbeddingKeyLanguages")]
		Languages,
		[Field ("NLContextualEmbeddingKeyScripts")]
		Scripts,
		[Field ("NLContextualEmbeddingKeyRevision")]
		Revision,
	}
}
