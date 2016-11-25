using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.AVKit {

#if !XAMCORE_3_0 || MONOMAC
	// this enum only exists for OSX (not iOS)
	[Native]
	public enum AVPlayerViewControlsStyle : nint {
		None,
		Inline,
		Floating,
		Minimal,
		Default = Inline 
	}
#endif

#if !TVOS && (!MONOMAC || !XAMCORE_4_0)
	[iOS (9,0)]
	[Native]
	[ErrorDomain ("AVKitErrorDomain")]
	public enum AVKitError : nint {
		None = 0,
		Unknown = -1000,
		PictureInPictureStartFailed = -1001
	}
#endif

#if TVOS
	// this enum only exists for TVOS
	[TV (10,1)]
	public enum AVKitMetadataIdentifier : nint {
		[Field ("AVKitMetadataIdentifierExternalContentIdentifier")]
		ExternalContentIdentifier,
		[Field ("AVKitMetadataIdentifierExternalUserProfileIdentifier")]
		ExternalUserProfileIdentifier,
		[Field ("AVKitMetadataIdentifierPlaybackProgress")]
		PlaybackProgress,
	}
#endif
}	
