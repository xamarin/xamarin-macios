//
// coretext.cs: Definitions for CoreText
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.CoreText {

#if XAMCORE_2_0
	[Static]
#else
	[Partial]
#endif
	[Since (3,2)]
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
	[Since (3,2)]
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
	[Since (3,2)]
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
	}
}