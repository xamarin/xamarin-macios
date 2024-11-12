using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
[SupportedOSPlatform ("macos")]
[SupportedOSPlatform ("ios11.0")]
[SupportedOSPlatform ("tvos11.0")]
[SupportedOSPlatform ("maccatalyst13.1")]
enum AVMediaCharacteristics {
	[Field<EnumValue> ("AVMediaCharacteristicVisual")]
	Visual = 0,

	[Field<EnumValue> ("AVMediaCharacteristicAudible")]
	Audible = 1,

	[Field<EnumValue> ("AVMediaCharacteristicLegible")]
	Legible = 2,

	[Field<EnumValue> ("AVMediaCharacteristicFrameBased")]
	FrameBased = 3,

	[Field<EnumValue> ("AVMediaCharacteristicUsesWideGamutColorSpace")]
	UsesWideGamutColorSpace = 4,

	[Field<EnumValue> ("AVMediaCharacteristicIsMainProgramContent")]
	IsMainProgramContent = 5,

	[Field<EnumValue> ("AVMediaCharacteristicIsAuxiliaryContent")]
	IsAuxiliaryContent = 6,

	[Field<EnumValue> ("AVMediaCharacteristicContainsOnlyForcedSubtitles")]
	ContainsOnlyForcedSubtitles = 7,

	[Field<EnumValue> ("AVMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
	TranscribesSpokenDialogForAccessibility = 8,

	[Field<EnumValue> ("AVMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
	DescribesMusicAndSoundForAccessibility = 9,

	[Field<EnumValue> ("AVMediaCharacteristicDescribesVideoForAccessibility")]
	DescribesVideoForAccessibility = 10,

#if !__MACOS__
	[UnsupportedOSPlatform("macos")]
	[Field<EnumValue> ("AVMediaCharacteristicEasyToRead")]
	EasyToRead = 11,
#endif

	[Field<EnumValue> ("AVMediaCharacteristicLanguageTranslation")]
	LanguageTranslation = 12,

	[Field<EnumValue> ("AVMediaCharacteristicDubbedTranslation")]
	DubbedTranslation = 13,

	[Field<EnumValue> ("AVMediaCharacteristicVoiceOverTranslation")]
	VoiceOverTranslation = 14,

	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[Field<EnumValue> ("AVMediaCharacteristicIsOriginalContent")]
	IsOriginalContent = 15,

	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[Field<EnumValue> ("AVMediaCharacteristicContainsHDRVideo")]
	ContainsHdrVideo = 16,

	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[Field<EnumValue> ("AVMediaCharacteristicContainsAlphaChannel")]
	ContainsAlphaChannel = 17,
}
