// Copyright 2018-2019 Microsoft, Corp.
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

using System;
using System.ComponentModel;
using Foundation;
using CoreML;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace NaturalLanguage {

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // designated
	[BaseType (typeof (NSObject))]
	interface NLLanguageRecognizer {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Static]
		[Internal]
		[Export ("dominantLanguageForString:")]
		[return: NullAllowed]
		NSString _GetDominantLanguage (IntPtr @string);

		[Export ("processString:")]
		void Process (string @string);

		[Export ("reset")]
		void Reset ();

		[Internal]
		[NullAllowed, Export ("dominantLanguage")]
		NSString _DominantLanguage { get; }

		[Wrap ("NLLanguageExtensions.GetValue (_DominantLanguage)")]
		NLLanguage DominantLanguage { get; }

		// left in case the user does not want to get a c# dict
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("languageHypothesesWithMaximum:")]
		NSDictionary<NSString, NSNumber> GetNativeLanguageHypotheses (nuint maxHypotheses);

		// left in case the user does not want to get a c# dict
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("languageHints", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSNumber> NativeLanguageHints { get; set; }

		[Internal]
		[Export ("languageConstraints", ArgumentSemantic.Copy)]
		NSString [] _LanguageConstraints { get; set; }

		NLLanguage [] LanguageConstraints {
			[Wrap ("Array.ConvertAll (_LanguageConstraints, e => NLLanguageExtensions.GetValue (e))")]
			get;
			[Wrap ("_LanguageConstraints = Array.ConvertAll (value, e => NLLanguageExtensions.GetConstant (e)!)")]
			set;
		}
	}

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NLModelConfiguration : NSCopying, NSSecureCoding {
		[Export ("type")]
		NLModelType Type { get; }

		[Internal]
		[NullAllowed, Export ("language")]
		NSString _Language { get; }

		NLLanguage Language { [Wrap ("(_Language is not null)? NLLanguageExtensions.GetValue (_Language) : NLLanguage.Undetermined")] get; }

		[Export ("revision")]
		nuint Revision { get; }

		[Static]
		[Export ("supportedRevisionsForType:")]
		NSIndexSet GetSupportedRevisions (NLModelType type);

		[Static]
		[Export ("currentRevisionForType:")]
		nuint GetCurrentRevision (NLModelType type);
	}

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NLModel {
		[Static]
		[Export ("modelWithContentsOfURL:error:")]
		[return: NullAllowed]
		NLModel Create (NSUrl url, [NullAllowed] out NSError error);

		[Static]
		[Export ("modelWithMLModel:error:")]
		[return: NullAllowed]
		NLModel Create (MLModel mlModel, [NullAllowed] out NSError error);

		[Export ("configuration", ArgumentSemantic.Copy)]
		NLModelConfiguration Configuration { get; }

		[Export ("predictedLabelForString:")]
		[return: NullAllowed]
		string GetPredictedLabel (string @string);

		[Export ("predictedLabelsForTokens:")]
		string [] GetPredictedLabels (string [] tokens);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("predictedLabelHypothesesForString:maximumCount:")]
		// `Native` added (like existing API) because we provide a better API with manual bindings (to avoid NSNumber)
		NSDictionary<NSString, NSNumber> GetNativePredictedLabelHypotheses (string @string, nuint maximumCount);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("predictedLabelHypothesesForTokens:maximumCount:")]
		// `Native` added (like existing API) because we provide a better API with manual bindings (to avoid NSNumber)
		NSDictionary<NSString, NSNumber> [] GetNativePredictedLabelHypotheses (string [] tokens, nuint maximumCount);
	}

	delegate void NLTokenizerEnumerateContinuationHandler (NSRange tokenRange, NLTokenizerAttributes flags, out bool stop);

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NLTokenizer {
		[Export ("initWithUnit:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NLTokenUnit unit);

		[Export ("unit")]
		NLTokenUnit Unit { get; }

		[NullAllowed, Export ("string", ArgumentSemantic.Retain)]
		string String { get; set; }

		[Internal]
		[Export ("setLanguage:")]
		void _SetLanguage (NSString language);

		[Wrap ("_SetLanguage (language.GetConstant ()!)")]
		void SetLanguage (NLLanguage language);

		[Export ("tokenRangeAtIndex:")]
		NSRange GetTokenRange (nuint characterIndex);

		[Export ("tokensForRange:")]
		NSValue [] GetTokens (NSRange range);

		[Export ("enumerateTokensInRange:usingBlock:")]
		void EnumerateTokens (NSRange range, NLTokenizerEnumerateContinuationHandler handler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tokenRangeForRange:")]
		NSRange GetTokenRange (NSRange range);
	}

	delegate void NLTaggerEnumerateTagsContinuationHandler (NSString tag, NSRange tokenRange, out bool stop);

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NLTagger {
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithTagSchemes:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([Params] NSString [] tagSchemes);

		[Wrap ("this (Array.ConvertAll (tagSchemes, e => e.GetConstant ()!))")]
		NativeHandle Constructor ([Params] NLTagScheme [] tagSchemes);

		[Internal]
		[Export ("tagSchemes", ArgumentSemantic.Copy)]
		NSString [] _TagSchemes { get; }

		[Wrap ("Array.ConvertAll (_TagSchemes, e => NLTagSchemeExtensions.GetValue (e))")]
		NLTagScheme [] TagSchemes { get; }

		[NullAllowed, Export ("string", ArgumentSemantic.Retain)]
		string String { get; set; }

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("availableTagSchemesForUnit:language:")]
		NSString [] GetAvailableTagSchemes (NLTokenUnit unit, NSString language);

		[Static]
		[Wrap ("Array.ConvertAll (GetAvailableTagSchemes (unit, language.GetConstant()!), e => NLTagSchemeExtensions.GetValue (e))")]
		NLTagScheme [] GetAvailableTagSchemes (NLTokenUnit unit, NLLanguage language);

		[Export ("tokenRangeAtIndex:unit:")]
		NSRange GetTokenRange (nuint characterIndex, NSString unit);

		[Internal]
		[NullAllowed, Export ("dominantLanguage")]
		NSString _DominantLanguage { get; }

		[Wrap ("NLLanguageExtensions.GetValue (_DominantLanguage)")]
		NLLanguage DominantLanguage { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("enumerateTagsInRange:unit:scheme:options:usingBlock:")]
		void EnumerateTags (NSRange range, NLTokenUnit unit, NSString scheme, NLTaggerOptions options, NLTaggerEnumerateTagsContinuationHandler handler);

		[Wrap ("EnumerateTags (range, unit, scheme.GetConstant ()!, options, handler)")]
		void EnumerateTags (NSRange range, NLTokenUnit unit, NLTagScheme scheme, NLTaggerOptions options, NLTaggerEnumerateTagsContinuationHandler handler);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("tagAtIndex:unit:scheme:tokenRange:")]
		[return: NullAllowed]
		NSString GetTag (nuint characterIndex, NLTokenUnit unit, NSString scheme, out NSRange tokenRange);

		[return: NullAllowed]
		[Wrap ("GetTag (characterIndex, unit, scheme.GetConstant ()!, out tokenRange)")]
		NSString GetTag (nuint characterIndex, NLTokenUnit unit, NLTagScheme scheme, out NSRange tokenRange);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("tagsInRange:unit:scheme:options:tokenRanges:")]
		NSString [] GetTags (NSRange range, NLTokenUnit unit, NSString scheme, NLTaggerOptions options, [NullAllowed] out NSValue [] tokenRanges);

		[Wrap ("GetTags (range, unit, scheme.GetConstant ()!, options, out tokenRanges)")]
		NSString [] GetTags (NSRange range, NLTokenUnit unit, NLTagScheme scheme, NLTaggerOptions options, [NullAllowed] out NSValue [] tokenRanges);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("setLanguage:range:")]
		void SetLanguage (NSString language, NSRange range);

		[Wrap ("SetLanguage (language.GetConstant ()!, range)")]
		void SetLanguage (NLLanguage language, NSRange range);

		[Export ("setOrthography:range:")]
		void SetOrthography (NSOrthography orthography, NSRange range);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("setModels:forTagScheme:")]
		void SetModels (NLModel [] models, NSString tagScheme);

		[Wrap ("SetModels (models, tagScheme.GetConstant ()!)")]
		void SetModels (NLModel [] models, NLTagScheme tagScheme);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("modelsForTagScheme:")]
		NLModel [] GetModels (NSString tagScheme);

		[Wrap ("GetModels (tagScheme.GetConstant ()!)")]
		NLModel [] GetModels (NLTagScheme tagScheme);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("setGazetteers:forTagScheme:")]
		void SetGazetteers (NLGazetteer [] gazetteers, NSString tagScheme);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("SetGazetteers (gazetteers, tagScheme.GetConstant ()!)")]
		void SetGazetteers (NLGazetteer [] gazetteers, NLTagScheme tagScheme);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("gazetteersForTagScheme:")]
		NLGazetteer [] GetGazetteers (NSString tagScheme);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("GetGazetteers (tagScheme.GetConstant ()!)")]
		NLGazetteer [] GetGazetteers (NLTagScheme tagScheme);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Async]
		[Export ("requestAssetsForLanguage:tagScheme:completionHandler:")]
		void RequestAssets (NSString language, NSString tagScheme, Action<NLTaggerAssetsResult, NSError> completionHandler);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async]
		[Wrap ("RequestAssets (language.GetConstant ()!, tagScheme.GetConstant ()!, completionHandler)")]
		void RequestAssets (NLLanguage language, NLTagScheme tagScheme, Action<NLTaggerAssetsResult, NSError> completionHandler);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tagHypothesesAtIndex:unit:scheme:maximumCount:tokenRange:")]
		// `Native` added (like existing API) because we provide a better API with manual bindings (to avoid NSNumber)
		NSDictionary<NSString, NSNumber> GetNativeTagHypotheses (nuint characterIndex, NLTokenUnit unit, NSString scheme, nuint maximumCount, out NSRange tokenRange);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Internal]
		[Sealed]
		[Export ("tagHypothesesAtIndex:unit:scheme:maximumCount:tokenRange:")]
		NSDictionary<NSString, NSNumber> GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NSString scheme, nuint maximumCount, IntPtr tokenRange);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("GetTagHypotheses (characterIndex, unit, scheme, maximumCount, IntPtr.Zero)")]
		// `Native` added (like existing API) because we provide a better API with manual bindings (to avoid NSNumber)
		NSDictionary<NSString, NSNumber> GetNativeTagHypotheses (nuint characterIndex, NLTokenUnit unit, NSString scheme, nuint maximumCount);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tokenRangeForRange:unit:")]
		NSRange GetTokenRange (NSRange range, NLTokenUnit unit);
	}

	[iOS (12, 0), TV (12, 0), Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Static] // only used to compare with NSString not as input/output
	interface NLTag {
		[Field ("NLTagWord")]
		NSString Word { get; }
		[Field ("NLTagPunctuation")]
		NSString Punctuation { get; }
		[Field ("NLTagWhitespace")]
		NSString Whitespace { get; }
		[Field ("NLTagOther")]
		NSString Other { get; }
		[Field ("NLTagNoun")]
		NSString Noun { get; }
		[Field ("NLTagVerb")]
		NSString Verb { get; }
		[Field ("NLTagAdjective")]
		NSString Adjective { get; }
		[Field ("NLTagAdverb")]
		NSString Adverb { get; }
		[Field ("NLTagPronoun")]
		NSString Pronoun { get; }
		[Field ("NLTagDeterminer")]
		NSString Determiner { get; }
		[Field ("NLTagParticle")]
		NSString Particle { get; }
		[Field ("NLTagPreposition")]
		NSString Preposition { get; }
		[Field ("NLTagNumber")]
		NSString Number { get; }
		[Field ("NLTagConjunction")]
		NSString Conjunction { get; }
		[Field ("NLTagInterjection")]
		NSString Interjection { get; }
		[Field ("NLTagClassifier")]
		NSString Classifier { get; }
		[Field ("NLTagIdiom")]
		NSString Idiom { get; }
		[Field ("NLTagOtherWord")]
		NSString OtherWord { get; }
		[Field ("NLTagSentenceTerminator")]
		NSString SentenceTerminator { get; }
		[Field ("NLTagOpenQuote")]
		NSString OpenQuote { get; }
		[Field ("NLTagCloseQuote")]
		NSString CloseQuote { get; }
		[Field ("NLTagOpenParenthesis")]
		NSString OpenParenthesis { get; }
		[Field ("NLTagCloseParenthesis")]
		NSString CloseParenthesis { get; }
		[Field ("NLTagWordJoiner")]
		NSString WordJoiner { get; }
		[Field ("NLTagDash")]
		NSString Dash { get; }
		[Field ("NLTagOtherPunctuation")]
		NSString OtherPunctuation { get; }
		[Field ("NLTagParagraphBreak")]
		NSString ParagraphBreak { get; }
		[Field ("NLTagOtherWhitespace")]
		NSString OtherWhitespace { get; }
		[Field ("NLTagPersonalName")]
		NSString PersonalName { get; }
		[Field ("NLTagPlaceName")]
		NSString PlaceName { get; }
		[Field ("NLTagOrganizationName")]
		NSString OrganizationName { get; }
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NLDistanceType : long {
		Cosine,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NLTaggerAssetsResult : long {
		Available,
		NotAvailable,
		Error,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate void NLEnumerateNeighborsHandler (string neighbor, /* NLDistance */ double distance, ref bool stop);

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NLEmbedding {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("wordEmbeddingForLanguage:")]
		[return: NullAllowed]
		NLEmbedding GetWordEmbedding (NSString language);

		[Static]
		[Wrap ("GetWordEmbedding (language.GetConstant ()!)")]
		[return: NullAllowed]
		NLEmbedding GetWordEmbedding (NLLanguage language);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("wordEmbeddingForLanguage:revision:")]
		[return: NullAllowed]
		NLEmbedding GetWordEmbedding (NSString language, nuint revision);

		[Static]
		[Wrap ("GetWordEmbedding (language.GetConstant ()!)")]
		[return: NullAllowed]
		NLEmbedding GetWordEmbedding (NLLanguage language, nuint revision);

		[Static]
		[Export ("embeddingWithContentsOfURL:error:")]
		[return: NullAllowed]
		NLEmbedding GetEmbedding (NSUrl url, [NullAllowed] out NSError error);

		[Export ("containsString:")]
		bool Contains (string @string);

		[Export ("distanceBetweenString:andString:distanceType:")]
		double GetDistance (string firstString, string secondString, NLDistanceType distanceType);

		[Export ("enumerateNeighborsForString:maximumCount:distanceType:usingBlock:")]
		void EnumerateNeighbors (string @string, nuint maxCount, NLDistanceType distanceType, NLEnumerateNeighborsHandler handler);

		[Export ("enumerateNeighborsForString:maximumCount:maximumDistance:distanceType:usingBlock:")]
		void EnumerateNeighbors (string @string, nuint maxCount, double maxDistance, NLDistanceType distanceType, NLEnumerateNeighborsHandler handler);

		[Export ("neighborsForString:maximumCount:distanceType:")]
		[return: NullAllowed]
		string [] GetNeighbors (string @string, nuint maxCount, NLDistanceType distanceType);

		[Export ("neighborsForString:maximumCount:maximumDistance:distanceType:")]
		[return: NullAllowed]
		string [] GetNeighbors (string @string, nuint maxCount, double maxDistance, NLDistanceType distanceType);

		[Export ("vectorForString:")]
		[return: NullAllowed]
		[return: BindAs (typeof (float []))]
		// doc says "array of double" but other API uses float ?!?
		NSNumber [] GetVector (string @string);

		[Internal] // can't bind float[] without NSArray but it will be better bound using .net pattern `bool TryGetVector (string, out float[] vector)`
		[Export ("getVector:forString:")]
		bool GetVector (IntPtr /* float[] */ vector, string @string);

		[Export ("enumerateNeighborsForVector:maximumCount:distanceType:usingBlock:")]
		void EnumerateNeighbors ([BindAs (typeof (float []))] NSNumber [] vector, nuint maxCount, NLDistanceType distanceType, NLEnumerateNeighborsHandler handler);

		[Export ("enumerateNeighborsForVector:maximumCount:maximumDistance:distanceType:usingBlock:")]
		void EnumerateNeighbors ([BindAs (typeof (float []))] NSNumber [] vector, nuint maxCount, double maxDistance, NLDistanceType distanceType, NLEnumerateNeighborsHandler handler);

		[Export ("neighborsForVector:maximumCount:distanceType:")]
		string [] GetNeighbors ([BindAs (typeof (float []))] NSNumber [] vector, nuint maxCount, NLDistanceType distanceType);

		[Export ("neighborsForVector:maximumCount:maximumDistance:distanceType:")]
		string [] GetNeighbors ([BindAs (typeof (float []))] NSNumber [] vector, nuint maxCount, double maxDistance, NLDistanceType distanceType);

		[Export ("dimension")]
		nuint Dimension { get; }

		[Export ("vocabularySize")]
		nuint VocabularySize { get; }

		[NullAllowed, Export ("language")]
		[BindAs (typeof (NLLanguage?))]
		NSString Language { get; }

		[Export ("revision")]
		nuint Revision { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("supportedRevisionsForLanguage:")]
		NSIndexSet GetSupportedRevisions (NSString language);

		[Static]
		[Wrap ("GetSupportedRevisions (language.GetConstant ()!)")]
		NSIndexSet GetSupportedRevisions (NLLanguage language);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("currentRevisionForLanguage:")]
		nuint GetCurrentRevision (NSString language);

		[Static]
		[Wrap ("GetCurrentRevision (language.GetConstant ()!)")]
		nuint GetCurrentRevision (NLLanguage language);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("writeEmbeddingForDictionary:language:revision:toURL:error:")]
		bool Write (NSDictionary dictionary, [NullAllowed] NSString language, nuint revision, NSUrl url, [NullAllowed] out NSError error);

		[Static]
		[Wrap ("Write (dictionary.GetDictionary ()!, language.HasValue ? language.Value.GetConstant () : null, revision, url, out error)")]
		bool Write (NLVectorDictionary dictionary, NLLanguage? language, nuint revision, NSUrl url, [NullAllowed] out NSError error);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("currentSentenceEmbeddingRevisionForLanguage:")]
		nuint GetCurrentSentenceEmbeddingRevision (NSString language);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Wrap ("GetCurrentSentenceEmbeddingRevision (language.GetConstant ()!)")]
		nuint GetCurrentSentenceEmbeddingRevision (NLLanguage language);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("sentenceEmbeddingForLanguage:")]
		[return: NullAllowed]
		NLEmbedding GetSentenceEmbedding (NSString language);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Wrap ("GetSentenceEmbedding (language.GetConstant ()!)")]
		[return: NullAllowed]
		NLEmbedding GetSentenceEmbedding (NLLanguage language);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("sentenceEmbeddingForLanguage:revision:")]
		[return: NullAllowed]
		NLEmbedding GetSentenceEmbedding (NSString language, nuint revision);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Wrap ("GetSentenceEmbedding (language.GetConstant ()!, revision)")]
		[return: NullAllowed]
		NLEmbedding GetSentenceEmbedding (NLLanguage language, nuint revision);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("supportedSentenceEmbeddingRevisionsForLanguage:")]
		NSIndexSet GetSupportedSentenceEmbeddingRevisions (NSString language);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Wrap ("GetSupportedSentenceEmbeddingRevisions (language.GetConstant ()!)")]
		NSIndexSet GetSupportedSentenceEmbeddingRevisions (NLLanguage language);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NLGazetteer {

		[Static]
		[Export ("gazetteerWithContentsOfURL:error:")]
		[return: NullAllowed]
		NLGazetteer Create (NSUrl url, [NullAllowed] out NSError error);

		[Export ("initWithContentsOfURL:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url, [NullAllowed] out NSError error);

		[Export ("initWithData:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData data, [NullAllowed] out NSError error);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithDictionary:language:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDictionary dictionary, [NullAllowed] NSString language, [NullAllowed] out NSError error);

		// sadly `language?.GetConstant ()` does not cut it :(
		// error CS1929: 'NLLanguage?' does not contain a definition for 'GetConstant' and the best extension method overload 'NLLanguageExtensions.GetConstant(NLLanguage)' requires a receiver of type 'NLLanguage'
		[Wrap ("this (dictionary.GetDictionary ()!, language.HasValue ? language.Value.GetConstant () : null, out error)")]
		NativeHandle Constructor (NLStrongDictionary dictionary, NLLanguage? language, [NullAllowed] out NSError error);

		[Export ("labelForString:")]
		[return: NullAllowed]
		string GetLabel (string @string);

		[NullAllowed, Export ("language")]
		[BindAs (typeof (NLLanguage?))]
		NSString Language { get; }

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("writeGazetteerForDictionary:language:toURL:error:")]
		bool Write (NSDictionary dictionary, [NullAllowed] NSString language, NSUrl url, [NullAllowed] out NSError error);

		[Static]
		[Wrap ("Write (dictionary.GetDictionary ()!, language.HasValue ? language.Value.GetConstant () : null, url, out error)")]
		bool Write (NLStrongDictionary dictionary, NLLanguage? language, NSUrl url, [NullAllowed] out NSError error);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NLContextualEmbedding {
		[Static]
		[Export ("contextualEmbeddingWithModelIdentifier:")]
		[return: NullAllowed]
		NLContextualEmbedding CreateWithModelIdentifier (string modelIdentifier);

		[Static]
		[Export ("contextualEmbeddingsForValues:")]
		NLContextualEmbedding [] Create (NSDictionary<NSString, NSObject> values);

		[Static]
		[Export ("contextualEmbeddingWithLanguage:")]
		[return: NullAllowed]
		NLContextualEmbedding CreateWithLanguage (string language);

		[Static]
		[Export ("contextualEmbeddingWithScript:")]
		[return: NullAllowed]
		NLContextualEmbedding CreateWithScript (string script);

		[Export ("modelIdentifier")]
		string ModelIdentifier { get; }

		[Export ("languages", ArgumentSemantic.Copy)]
		string [] Languages { get; }

		[Export ("scripts", ArgumentSemantic.Copy)]
		string [] Scripts { get; }

		[Export ("revision")]
		nuint Revision { get; }

		[Export ("dimension")]
		nuint Dimension { get; }

		[Export ("maximumSequenceLength")]
		nuint MaximumSequenceLength { get; }

		[Export ("loadWithError:")]
		bool Load ([NullAllowed] out NSError error);

		[Export ("unload")]
		void Unload ();

		[Export ("embeddingResultForString:language:error:")]
		[return: NullAllowed]
		NLContextualEmbeddingResult GetEmbeddingResult (string @string, [NullAllowed] string language, [NullAllowed] out NSError error);

		[Export ("hasAvailableAssets")]
		bool HasAvailableAssets { get; }

		[Export ("requestEmbeddingAssetsWithCompletionHandler:")]
		[Async]
		void RequestAssets (Action<NLContextualEmbeddingAssetsResult, NSError> completionHandler);
	}

	delegate void TokenVectorEnumeratorHandler (NSArray<NSNumber> tokenVector, NSRange tokenRange, out bool stop);

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NLContextualEmbeddingResult {
		[Export ("string")]
		string String { get; }

		[Export ("language")]
		string Language { get; }

		[Export ("sequenceLength")]
		nuint SequenceLength { get; }

		[Export ("enumerateTokenVectorsInRange:usingBlock:")]
		void EnumerateTokenVectors (NSRange range, TokenVectorEnumeratorHandler enumerationHandler);

#if WATCHOS
		[return: BindAs (typeof(int[]))]
#else
		[return: BindAs (typeof (long []))]
#endif
		[Export ("tokenVectorAtIndex:tokenRange:")]
		[return: NullAllowed]
		NSNumber [] GetVector (nuint characterIndex, ref NSRange tokenRange);
	}
}
