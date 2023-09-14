using ObjCRuntime;
using Foundation;

#nullable enable

namespace AVKit {

#if !XAMCORE_3_0 || MONOMAC
	// this enum only exists for OSX (not iOS)
	[Native]
	public enum AVPlayerViewControlsStyle : long {
		None,
		Inline,
		Floating,
		Minimal,
		Default = Inline
	}
#endif

	// The version of the AVError.h header file in the tvOS SDK is much newer than in the iOS SDKs,
	// (copyright 2016 vs 2019), so this is reflecting the tvOS SDK.
	[TV (13, 0)]
#if NET
	[NoMac]
	[NoWatch]
	[MacCatalyst (13, 1)]
#endif
	[Native]
	[ErrorDomain ("AVKitErrorDomain")]
	public enum AVKitError : long {
		None = 0,
		Unknown = -1000,
		PictureInPictureStartFailed = -1001,
		ContentRatingUnknown = -1100,
		ContentDisallowedByPasscode = -1101,
		ContentDisallowedByProfile = -1102,
		RecordingFailed = -1200,
	}

	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioSessionRouteSelection : long {
		None = 0,
		Local = 1,
		External = 2,
	}

	[NoiOS]
	[NoWatch]
	[NoTV]
	[NoMacCatalyst]
	[Native]
	public enum AVRoutePickerViewButtonState : long {
		Normal,
		NormalHighlighted,
		Active,
		ActiveHighlighted,
	}

}
