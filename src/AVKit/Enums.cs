using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

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
	[iOS (9,0)]
	[TV (13,0)]
#if NET
	[NoMac]
	[NoWatch]
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
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (13,0)]
#endif
	[Native]
	public enum AVAudioSessionRouteSelection : long {
		None = 0,
		Local = 1,
		External = 2,
	}

#if NET
	[SupportedOSPlatform ("macos10.15")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS]
	[NoWatch]
	[NoTV]
	[Mac (10,15)]
#endif
	[Native]
	public enum AVRoutePickerViewButtonState : long {
		Normal,
		NormalHighlighted,
		Active,
		ActiveHighlighted,
	}

}	
