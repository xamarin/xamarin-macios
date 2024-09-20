//
// coretext.cs: Definitions for CoreText
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace CoreText {

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTFontFeatures" />.</summary>
	[Static]
	interface CTFontFeatureKey {
		[Field ("kCTFontFeatureTypeIdentifierKey")]
		NSString Identifier { get; }

		[Field ("kCTFontFeatureTypeNameKey")]
		NSString Name { get; }

		[Field ("kCTFontFeatureTypeExclusiveKey")]
		NSString Exclusive { get; }

		[Field ("kCTFontFeatureTypeSelectorsKey")]
		NSString Selectors { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTFontFeatureSelectors" />.</summary>
	[Static]
	interface CTFontFeatureSelectorKey {
		[Field ("kCTFontFeatureSelectorIdentifierKey")]
		NSString Identifier { get; }

		[Field ("kCTFontFeatureSelectorNameKey")]
		NSString Name { get; }

		[Field ("kCTFontFeatureSelectorDefaultKey")]
		NSString Default { get; }

		[Field ("kCTFontFeatureSelectorSettingKey")]
		NSString Setting { get; }

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCTFontFeatureSampleTextKey")]
		NSString SampleText { get; }

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCTFontFeatureTooltipTextKey")]
		NSString TooltipText { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTFontVariationAxes" />.</summary>
	[Static]
	interface CTFontVariationAxisKey {

		[Field ("kCTFontVariationAxisIdentifierKey")]
		NSString Identifier { get; }

		[Field ("kCTFontVariationAxisMinimumValueKey")]
		NSString MinimumValue { get; }

		[Field ("kCTFontVariationAxisMaximumValueKey")]
		NSString MaximumValue { get; }

		[Field ("kCTFontVariationAxisDefaultValueKey")]
		NSString DefaultValue { get; }

		[Field ("kCTFontVariationAxisNameKey")]
		NSString Name { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCTFontVariationAxisHiddenKey")]
		NSString Hidden { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTTypesetterOptions" />.</summary>
	[Static]
	interface CTTypesetterOptionKey {

		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kCTTypesetterOptionDisableBidiProcessing")]
#if !NET
		[Internal]
		NSString _DisableBidiProcessing { get; }
#else
		NSString DisableBidiProcessing { get; }
#endif

		[Field ("kCTTypesetterOptionForcedEmbeddingLevel")]
#if !NET
		[Internal]
		NSString _ForceEmbeddingLevel { get; }
#else
		NSString ForceEmbeddingLevel { get; }
#endif

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCTTypesetterOptionAllowUnboundedLayout")]
		NSString AllowUnboundedLayout { get; }
	}

	[Static]
	interface CTFontManagerErrorKeys {
		[Field ("kCTFontManagerErrorFontURLsKey")]
		NSString FontUrlsKey { get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCTFontManagerErrorFontDescriptorsKey")]
		NSString FontDescriptorsKey { get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCTFontManagerErrorFontAssetNameKey")]
		NSString FontAssetNameKey { get; }
	}

#if NET
	[Internal]
	[Static][Partial]
	interface CTBaselineClassID {
		[Field ("kCTBaselineClassRoman")]
		NSString Roman { get; }

		[Field ("kCTBaselineClassIdeographicCentered")]
		NSString IdeographicCentered { get; }

		[Field ("kCTBaselineClassIdeographicLow")]
		NSString IdeographicLow { get; }

		[Field ("kCTBaselineClassIdeographicHigh")]
		NSString IdeographicHigh { get; }

		[Field ("kCTBaselineClassHanging")]
		NSString Hanging { get; }

		[Field ("kCTBaselineClassMath")]
		NSString Math { get; }
	}

	[Internal]
	[Static][Partial]
	interface CTBaselineFontID {
		[Field ("kCTBaselineReferenceFont")]
		NSString Reference { get; }

		[Field ("kCTBaselineOriginalFont")]
		NSString Original { get; }
	}

	/// <summary>A valid key for use with <see cref="T:CoreText.CTFontDescriptor" /> attribute properties.</summary>
	[Static]
	interface CTFontDescriptorAttributeKey {
		[Field ("kCTFontURLAttribute")]
		NSString Url { get; }

		[Field ("kCTFontNameAttribute")]
		NSString Name { get; }

		[Field ("kCTFontDisplayNameAttribute")]
		NSString DisplayName { get; }

		[Field ("kCTFontFamilyNameAttribute")]
		NSString FamilyName { get; }

		[Field ("kCTFontStyleNameAttribute")]
		NSString StyleName { get; }

		[Field ("kCTFontTraitsAttribute")]
		NSString Traits { get; }

		[Field ("kCTFontVariationAttribute")]
		NSString Variation { get; }

		[Field ("kCTFontSizeAttribute")]
		NSString Size { get; }

		[Field ("kCTFontMatrixAttribute")]
		NSString Matrix { get; }

		[Field ("kCTFontCascadeListAttribute")]
		NSString CascadeList { get; }

		[Field ("kCTFontCharacterSetAttribute")]
		NSString CharacterSet { get; }

		[Field ("kCTFontLanguagesAttribute")]
		NSString Languages { get; }

		[Field ("kCTFontBaselineAdjustAttribute")]
		NSString BaselineAdjust { get; }

		[Field ("kCTFontMacintoshEncodingsAttribute")]
		NSString MacintoshEncodings { get; }

		[Field ("kCTFontFeaturesAttribute")]
		NSString Features { get; }

		[Field ("kCTFontFeatureSettingsAttribute")]
		NSString FeatureSettings { get; }

		[Field ("kCTFontFixedAdvanceAttribute")]
		NSString FixedAdvance { get; }

		[Field ("kCTFontOrientationAttribute")]
		NSString FontOrientation { get; }

		[Field ("kCTFontFormatAttribute")]
		NSString FontFormat { get; }

		[Field ("kCTFontRegistrationScopeAttribute")]
		NSString RegistrationScope { get; }

		[Field ("kCTFontPriorityAttribute")]
		NSString Priority { get; }

		[Field ("kCTFontEnabledAttribute")]
		NSString Enabled { get; }

		[iOS (13, 0), NoTV, NoWatch, MacCatalyst (13, 1), NoMac]
		[Field ("kCTFontRegistrationUserInfoAttribute")]
		NSString RegistrationUserInfo { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTTextTabOptions" />.</summary>
	[Static]
	interface CTTextTabOptionKey {
		[Field ("kCTTabColumnTerminatorsAttributeName")]
		NSString ColumnTerminators { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTFrameAttributes" />.</summary>
	[Static]
	interface CTFrameAttributeKey {
		[Field ("kCTFrameProgressionAttributeName")]
		NSString Progression { get; }

		[Field ("kCTFramePathFillRuleAttributeName")]
		NSString PathFillRule { get; }

		[Field ("kCTFramePathWidthAttributeName")]
		NSString PathWidth { get; }

		[Field ("kCTFrameClippingPathsAttributeName")]
		NSString ClippingPaths { get; }

		[Field ("kCTFramePathClippingPathAttributeName")]
		NSString PathClippingPath { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTFontTraits" />.</summary>
	[Static]
	interface CTFontTraitKey {
		[Field ("kCTFontSymbolicTrait")]
		NSString Symbolic { get; }

		[Field ("kCTFontWeightTrait")]
		NSString Weight { get; }

		[Field ("kCTFontWidthTrait")]
		NSString Width { get; }

		[Field ("kCTFontSlantTrait")]
		NSString Slant { get; }
	}

	[Internal]
	[Static][Partial]
	interface CTFontNameKeyId {
		[Field ("kCTFontCopyrightNameKey")]
		NSString Copyright { get; }

		[Field ("kCTFontFamilyNameKey")]
		NSString Family { get; }

		[Field ("kCTFontSubFamilyNameKey")]
		NSString SubFamily { get; }

		[Field ("kCTFontStyleNameKey")]
		NSString Style { get; }

		[Field ("kCTFontUniqueNameKey")]
		NSString Unique { get; }

		[Field ("kCTFontFullNameKey")]
		NSString Full { get; }

		[Field ("kCTFontVersionNameKey")]
		NSString Version { get; }

		[Field ("kCTFontPostScriptNameKey")]
		NSString PostScript { get; }

		[Field ("kCTFontTrademarkNameKey")]
		NSString Trademark { get; }

		[Field ("kCTFontManufacturerNameKey")]
		NSString Manufacturer { get; }

		[Field ("kCTFontDesignerNameKey")]
		NSString Designer { get; }

		[Field ("kCTFontDescriptionNameKey")]
		NSString Description { get; }

		[Field ("kCTFontVendorURLNameKey")]
		NSString VendorUrl { get; }

		[Field ("kCTFontDesignerURLNameKey")]
		NSString DesignerUrl { get; }

		[Field ("kCTFontLicenseNameKey")]
		NSString License { get; }

		[Field ("kCTFontLicenseURLNameKey")]
		NSString LicenseUrl { get; }

		[Field ("kCTFontSampleTextNameKey")]
		NSString SampleText { get; }

		[Field ("kCTFontPostScriptCIDNameKey")]
		NSString PostscriptCid { get; }
	}

	/// <summary>A class whose static property can be used as a key for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTFontCollectionOptions" />.</summary>
	[Static]
	interface CTFontCollectionOptionKey {
		[Field ("kCTFontCollectionRemoveDuplicatesOption")]
		NSString RemoveDuplicates { get; }
	}
#endif

	[Internal]
	[Static]
	interface CTFontDescriptorMatchingKeys {
		[Field ("kCTFontDescriptorMatchingSourceDescriptor")]
		NSString SourceDescriptorKey { get; }

		[Field ("kCTFontDescriptorMatchingDescriptors")]
		NSString DescriptorsKey { get; }

		[Field ("kCTFontDescriptorMatchingResult")]
		NSString ResultKey { get; }

		[Field ("kCTFontDescriptorMatchingPercentage")]
		NSString PercentageKey { get; }

		[Field ("kCTFontDescriptorMatchingCurrentAssetSize")]
		NSString CurrentAssetSizeKey { get; }

		[Field ("kCTFontDescriptorMatchingTotalDownloadedSize")]
		NSString TotalDownloadedSizeKey { get; }

		[Field ("kCTFontDescriptorMatchingTotalAssetSize")]
		NSString TotalAssetSizeKey { get; }

		[Field ("kCTFontDescriptorMatchingError")]
		NSString ErrorKey { get; }
	}

	[StrongDictionary ("CTFontDescriptorMatchingKeys")]
	interface CTFontDescriptorMatchingProgress {
		CTFontDescriptor SourceDescriptor { get; }
		CTFontDescriptor [] Descriptors { get; }
		CTFontDescriptor [] Result { get; }
		double Percentage { get; }
		long CurrentAssetSize { get; }
		long TotalDownloadedSize { get; }
		long TotalAssetSize { get; }
		NSError Error { get; }
	}

	/// <summary>A class whose static properties can be used as keys for the <see cref="T:Foundation.NSDictionary" /> used by <see cref="T:CoreText.CTStringAttributes" />.</summary>
	[Static]
	[Partial]
	interface CTStringAttributeKey {
#if NET
		[Field ("kCTFontAttributeName")]
		NSString Font { get; }

		[Field ("kCTForegroundColorFromContextAttributeName")]
		NSString ForegroundColorFromContext { get; }

		[Field ("kCTKernAttributeName")]
		NSString KerningAdjustment { get; }

		[Field ("kCTLigatureAttributeName")]
		NSString LigatureFormation { get; }

		[Field ("kCTForegroundColorAttributeName")]
		NSString ForegroundColor { get; }

		[Field ("kCTBackgroundColorAttributeName")]
		NSString BackgroundColor { get; }

		[Field ("kCTParagraphStyleAttributeName")]
		NSString ParagraphStyle { get; }

		[Field ("kCTStrokeWidthAttributeName")]
		NSString StrokeWidth { get; }

		[Field ("kCTStrokeColorAttributeName")]
		NSString StrokeColor { get; }

		[Field ("kCTUnderlineStyleAttributeName")]
		NSString UnderlineStyle { get; }

		[Field ("kCTSuperscriptAttributeName")]
		NSString Superscript { get; }

		[Field ("kCTUnderlineColorAttributeName")]
		NSString UnderlineColor { get; }

		[Field ("kCTVerticalFormsAttributeName")]
		NSString VerticalForms { get; }

		[Field ("kCTHorizontalInVerticalFormsAttributeName")]
		NSString HorizontalInVerticalForms { get; }

		[Field ("kCTGlyphInfoAttributeName")]
		NSString GlyphInfo { get; }

		[Field ("kCTCharacterShapeAttributeName")]
		NSString CharacterShape { get; }

		[Field ("kCTRunDelegateAttributeName")]
		NSString RunDelegate { get; }

		[Field ("kCTBaselineOffsetAttributeName")]
		NSString BaselineOffset { get; }

		[Field ("kCTBaselineClassAttributeName")]
		NSString BaselineClass { get; }

		[Field ("kCTBaselineInfoAttributeName")]
		NSString BaselineInfo { get; }

		[Field ("kCTBaselineReferenceInfoAttributeName")]
		NSString BaselineReferenceInfo { get; }

		[Field ("kCTWritingDirectionAttributeName")]
		NSString WritingDirection { get; }

		[Field ("kCTRubyAnnotationAttributeName")]
		NSString RubyAnnotation { get; }

		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kCTAdaptiveImageProviderAttributeName")]
		NSString AdaptiveImageProvider { get; }
#endif

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCTTrackingAttributeName")]
		NSString TrackingAttributeName { get; }
	}

	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface CTAdaptiveImageProviding {
		[Abstract]
		[Export ("imageForProposedSize:scaleFactor:imageOffset:imageSize:")]
		[return: NullAllowed]
		CGImage GetImage (CGSize proposedSize, nfloat scaleFactor, out CGPoint imageOffset, out CGSize imageSize);
	}
}
