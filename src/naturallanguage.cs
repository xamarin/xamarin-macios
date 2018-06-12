// Copyright 2018, Microsoft, Corp.
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
using ObjCRuntime;

namespace NaturalLanguage {

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	[DisableDefaultCtor] // designated
	[BaseType (typeof(NSObject))]
	interface NLLanguageRecognizer {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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
		NSString[] _LanguageConstraints { get; set; }

		NLLanguage[] LanguageConstraints {
			[Wrap ("Array.ConvertAll (_LanguageConstraints, e => NLLanguageExtensions.GetValue (e))")]
			get;
			[Wrap ("_LanguageConstraints = Array.ConvertAll (value, e => NLLanguageExtensions.GetConstant (e))")]
			set;
		}
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	[BaseType (typeof(NSObject))]
	interface NLModelConfiguration : NSCopying, NSSecureCoding {
		[Export ("type")]
		NLModelType Type { get; }

		[Internal]
		[NullAllowed, Export ("language")]
		NSString _Language { get; }

		NLLanguage Language { [Wrap ("(_Language.Handle != IntPtr.Zero)? NLLanguageExtensions.GetValue (_Language) : NLLanguage.Undetermined")] get; }

		[Export ("revision")]
		nuint Revision { get; }

		[Static]
		[Export ("supportedRevisionsForType:")]
		NSIndexSet GetSupportedRevisions (NLModelType type);

		[Static]
		[Export ("currentRevisionForType:")]
		nuint GetCurrentRevision (NLModelType type);
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof(NSObject))]
	interface NLModel {
		[Static]
		[Export ("modelWithContentsOfURL:error:")]
		[return: NullAllowed]
		NLModel Create (NSUrl url, [NullAllowed] out NSError error);

		[Export ("configuration", ArgumentSemantic.Copy)]
		NLModelConfiguration Configuration { get; }

		[Export ("predictedLabelForString:")]
		[return: NullAllowed]
		string GetPredictedLabel (string @string);

		[Export ("predictedLabelsForTokens:")]
		string[] GetPredictedLabels (string[] tokens);
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof(NSObject))]
	interface NLTokenizer
	{
		[Export ("initWithUnit:")]
		[DesignatedInitializer]
		IntPtr Constructor (NLTokenUnit unit);

		[Export ("unit")]
		NLTokenUnit Unit { get; }

		[NullAllowed, Export ("string", ArgumentSemantic.Retain)]
		string String { get; set; }

		[Internal]
		[Export ("setLanguage:")]
		void _SetLanguage (string language);

		[Wrap ("_SetLanguage (language.GetConstant ())")]
		void SetLanguage (NLLanguage language);

		[Export ("tokenRangeAtIndex:")]
		NSRange GetTokenRange (nuint characterIndex);

		[Export ("tokensForRange:")]
		NSValue[] GetTokens (NSRange range);

		[Async (ResultTypeName="NLTokenizerEnumerateContinuation")]
		[Export ("enumerateTokensInRange:usingBlock:")]
		void EnumerateTokensInRange (NSRange range, Action<NSRange, NLTokenizerAttributes, bool> block);
	}

	[iOS (12,0), Mac (10,14, onlyOn64: true), TV (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof(NSObject))]
	interface NLTagger
	{
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithTagSchemes:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSString[] tagSchemes);

		[Wrap ("this (Array.ConvertAll (tagSchemes, e => e.GetConstant ()))")]
		IntPtr Constructor (NLTagScheme[] tagSchemes);

		[Internal]
		[Export ("tagSchemes", ArgumentSemantic.Copy)]
		NSString[] _TagSchemes { get; }

		[Wrap ("Array.ConvertAll (_TagSchemes, e => NLTagSchemeExtensions.GetValue (e))")]
		NLTagScheme[] TagSchemes { get; }

		[NullAllowed, Export ("string", ArgumentSemantic.Retain)]
		string String { get; set; }

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("availableTagSchemesForUnit:language:")]
		NSString[] GetAvailableTagSchemes (NLTokenUnit unit, NSString language);

		[Static]
		[Wrap ("Array.ConvertAll (GetAvailableTagSchemes (unit, language.GetConstant()), e => NLTagSchemeExtensions.GetValue (e))")]
		NLTagScheme[] GetAvailableTagSchemes (NLTokenUnit unit, NLLanguage language);

		[Export ("tokenRangeAtIndex:unit:")]
		NSRange GetTokenRange (nuint characterIndex, NSString unit);

		[Internal]
		[NullAllowed, Export ("dominantLanguage")]
		NSString _DominantLanguage { get; }

		[Wrap ("NLLanguageExtensions.GetValue (_DominantLanguage)")]
		NLLanguage DominantLanguage { get; }

		[Async (ResultTypeName="NLTaggerEnumerateTagsContinuation")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("enumerateTagsInRange:unit:scheme:options:usingBlock:")]
		void EnumerateTags (NSRange range, NLTokenUnit unit, NSString scheme, NLTaggerOptions options, Action<NSString, NSRange, bool> block);

		[Wrap ("EnumerateTags (range, unit, scheme.GetConstant (), options, block)")]
		void EnumerateTags (NSRange range, NLTokenUnit unit, NLTagScheme scheme, NLTaggerOptions options, Action<NSString, NSRange, bool> block);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("tagAtIndex:unit:scheme:tokenRange:")]
		[return: NullAllowed]
		NSString GetTag (nuint characterIndex, NLTokenUnit unit, NSString scheme, [NullAllowed] NSRange tokenRange);

		[Wrap ("NLTagExtensions.GetValue (GetTag (characterIndex, unit, scheme.GetConstant (), tokenRange))")]
		NLTag GetTag (nuint characterIndex, NLTokenUnit unit, NLTagScheme scheme, [NullAllowed] NSRange tokenRange);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("tagsInRange:unit:scheme:options:tokenRanges:")]
		NSString[] GetTags (NSRange range, NLTokenUnit unit, NSString scheme, NLTaggerOptions options, [NullAllowed] out NSValue[] tokenRanges);

		[Wrap ("Array.ConvertAll (GetTags (range, unit, scheme.GetConstant (), options, out tokenRanges), e => NLTagExtensions.GetValue (e))")]
		NLTag[] GetTags (NSRange range, NLTokenUnit unit, NLTagScheme scheme, NLTaggerOptions options, [NullAllowed] out NSValue[] tokenRanges);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("setLanguage:range:")]
		void SetLanguage (NSString language, NSRange range);

		[Wrap ("SetLanguage (language.GetConstant (), range)")]
		void SetLanguage (NLLanguage language, NSRange range);

		[Export ("setOrthography:range:")]
		void SetOrthography (NSOrthography orthography, NSRange range);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("setModels:forTagScheme:")]
		void SetModels (NLModel[] models, NSString tagScheme);

		[Wrap ("SetModels (models, tagScheme.GetConstant ())")]
		void SetModels (NLModel[] models, NLTagScheme tagScheme);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("modelsForTagScheme:")]
		NLModel[] GetModels (NSString tagScheme);

		[Wrap ("GetModels (tagScheme.GetConstant ())")]
		NLModel[] GetModels (NLTagScheme tagScheme);
	}
}