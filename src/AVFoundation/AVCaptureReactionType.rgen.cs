#pragma warning disable APL0003
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
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

	public static class AVCaptureReactionType_Extensions {
		[DllImport (Constants.AVFoundationLibrary)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		static extern IntPtr AVCaptureReactionSystemImageNameForType (IntPtr reactionType);

		/// <summary>Get the name of the system image that is the recommended iconography for the specified reaction type.</summary>
		/// <param name="reactionType">The reaction type whose system image should be returned.</param>
		/// <returns>The name of the system image that is the recommended iconography for the specified reaction type.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		public static string GetSystemImage (this AVCaptureReactionType reactionType)
		{
			var constant = reactionType.GetConstant ();
			var image = AVCaptureReactionSystemImageNameForType (constant.GetHandle ());
			return CFString.FromHandle (image, false)!;
		}
	}
}
#pragma warning restore APL0003
