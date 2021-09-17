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

#if MONOMAC
	[Mac (10,10)]
	[Native]
	public enum AVCaptureViewControlsStyle : long {
		Inline,
		Floating,
		InlineDeviceSelection,
		Default = Inline,
	}

	[Mac (10,9)]
	[Native]
	public enum AVPlayerViewTrimResult : long {
		OKButton,
		CancelButton
	}
#endif

#if !MONOMAC || !XAMCORE_4_0
	[iOS (9,0)]
	[TV (13,0)]
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
#endif

	[NoWatch, NoTV, NoMac]
	[iOS (13,0)]
	[Native]
	public enum AVAudioSessionRouteSelection : long {
		None = 0,
		Local = 1,
		External = 2,
	}

	[NoiOS, NoWatch, NoTV]
	[Mac (10,15)]
	[Native]
	public enum AVRoutePickerViewButtonState : long {
		Normal,
		NormalHighlighted,
		Active,
		ActiveHighlighted,
	}

}	
