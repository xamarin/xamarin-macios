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

#if XAMCORE_2_0
	[Static]
#else
	[Partial]
#endif
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

#if XAMCORE_2_0
	[Static]
#else
	[Partial]
#endif
	interface CTFontFeatureSelectorKey {
		[Field ("kCTFontFeatureSelectorIdentifierKey")]
		NSString Identifier { get; }

		[Field ("kCTFontFeatureSelectorNameKey")]
		NSString Name { get; }

		[Field ("kCTFontFeatureSelectorDefaultKey")]
		NSString Default { get; }

		[Field ("kCTFontFeatureSelectorSettingKey")]
		NSString Setting { get; }
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

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Field ("kCTTypesetterOptionAllowUnboundedLayout")]
		NSString AllowUnboundedLayout { get; }
	}
}