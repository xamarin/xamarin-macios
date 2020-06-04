//
// coretext.cs: Definitions for CoreText
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

namespace CoreText {

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

		[iOS (13,0), Mac (10,15), TV (13,0), Watch (6,0)]
		[Field ("kCTFontFeatureSampleTextKey")]
		NSString SampleText { get; }

		[iOS (13,0), Mac (10,15), TV (13,0), Watch (6,0)]
		[Field ("kCTFontFeatureTooltipTextKey")]
		NSString TooltipText { get; }
	}

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

		[iOS (11,0), Mac (10,13), Watch (4,0), TV (11,0)]
		[Field ("kCTFontVariationAxisHiddenKey")]
		NSString Hidden { get; }
	}

	[Static]
	interface CTTypesetterOptionKey {

		[Deprecated (PlatformName.iOS, 6, 0)]
		[Field ("kCTTypesetterOptionDisableBidiProcessing")]
#if !XAMCORE_4_0
		[Internal]
		NSString _DisableBidiProcessing { get; }
#else
		NSString DisableBidiProcessing { get; }
#endif

		[Field ("kCTTypesetterOptionForcedEmbeddingLevel")]
#if !XAMCORE_4_0
		[Internal]
		NSString _ForceEmbeddingLevel { get; }
#else
		NSString ForceEmbeddingLevel { get; }
#endif

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Field ("kCTTypesetterOptionAllowUnboundedLayout")]
		NSString AllowUnboundedLayout { get; }
	}

	[Static]
	interface CTFontManagerErrorKeys {
		[Field ("kCTFontManagerErrorFontURLsKey")]
		NSString FontUrlsKey { get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Field ("kCTFontManagerErrorFontDescriptorsKey")]
		NSString FontDescriptorsKey { get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Field ("kCTFontManagerErrorFontAssetNameKey")]
		NSString FontAssetNameKey { get; }
	}

	[Static][Partial]
	interface CTStringAttributeKey {
		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Field ("kCTTrackingAttributeName")]
		NSString TrackingAttributeName { get; }
	}
}