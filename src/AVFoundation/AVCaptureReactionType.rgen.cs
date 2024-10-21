#pragma warning disable APL0003
using System;
using System.Runtime.Versioning;

using Foundation;
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
		[Field ("AVCaptureReactionTypeThumbsUp")]
		ThumbsUp,

		[Field ("AVCaptureReactionTypeThumbsDown")]
		ThumbsDown,

		[Field ("AVCaptureReactionTypeBalloons")]
		Balloons,

		[Field ("AVCaptureReactionTypeHeart")]
		Heart,

		[Field ("AVCaptureReactionTypeFireworks")]
		Fireworks,

		[Field ("AVCaptureReactionTypeRain")]
		Rain,

		[Field ("AVCaptureReactionTypeConfetti")]
		Confetti,

		[Field ("AVCaptureReactionTypeLasers")]
		Lasers,
	}
}
#pragma warning restore APL0003
