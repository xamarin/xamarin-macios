using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
[SupportedOSPlatform ("maccatalyst13.1")]
enum AVMediaCharacteristics {
	[Field ("AVMediaCharacteristicVisual")]
	Visual = 0,

	[Field ("AVMediaCharacteristicAudible")]
	Audible = 1,

	[Field ("AVMediaCharacteristicLegible")]
	Legible = 2,

	[Field ("AVMediaCharacteristicFrameBased")]
	FrameBased = 3,

	[Field ("AVMediaCharacteristicUsesWideGamutColorSpace")]
	UsesWideGamutColorSpace = 4,

	[Field ("AVMediaCharacteristicIsMainProgramContent")]
	IsMainProgramContent = 5,

	[Field ("AVMediaCharacteristicIsAuxiliaryContent")]
	IsAuxiliaryContent = 6,

	[Field ("AVMediaCharacteristicContainsOnlyForcedSubtitles")]
	ContainsOnlyForcedSubtitles = 7,

	[Field ("AVMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
	TranscribesSpokenDialogForAccessibility = 8,

	[Field ("AVMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
	DescribesMusicAndSoundForAccessibility = 9,

	[Field ("AVMediaCharacteristicDescribesVideoForAccessibility")]
	DescribesVideoForAccessibility = 10,

#if !__MACOS__
	[Field ("AVMediaCharacteristicEasyToRead")]
	EasyToRead = 11,
#endif

	[Field ("AVMediaCharacteristicLanguageTranslation")]
	LanguageTranslation = 12,

	[Field ("AVMediaCharacteristicDubbedTranslation")]
	DubbedTranslation = 13,

	[Field ("AVMediaCharacteristicVoiceOverTranslation")]
	VoiceOverTranslation = 14,

	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[Field ("AVMediaCharacteristicIsOriginalContent")]
	IsOriginalContent = 15,

	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[Field ("AVMediaCharacteristicContainsHDRVideo")]
	ContainsHdrVideo = 16,

	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[Field ("AVMediaCharacteristicContainsAlphaChannel")]
	ContainsAlphaChannel = 17,
}
