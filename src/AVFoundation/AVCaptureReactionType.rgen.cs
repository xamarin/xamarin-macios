#pragma warning disable APL0003
using System;
using System.Runtime.Versioning;

using ObjCRuntime;
using ObjCBindings;

#nullable enable

namespace AVFoundation {

	[BindingTypeAttribute]
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("tvos17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
	public enum AVCaptureReactionType {
		[Field<EnumValue> ("AVCaptureReactionTypeThumbsUp")]
		ThumbsUp,

		[Field<EnumValue> ("AVCaptureReactionTypeThumbsDown")]
		ThumbsDown,

		[Field<EnumValue> ("AVCaptureReactionTypeBalloons")]
		Balloons,

		[Field<EnumValue> ("AVCaptureReactionTypeHeart")]
		Heart,

		[Field<EnumValue> ("AVCaptureReactionTypeFireworks")]
		Fireworks,

		[Field<EnumValue> ("AVCaptureReactionTypeRain")]
		Rain,

		[Field<EnumValue> ("AVCaptureReactionTypeConfetti")]
		Confetti,

		[Field<EnumValue> ("AVCaptureReactionTypeLasers")]
		Lasers,
	}
}
#pragma warning restore APL0003
