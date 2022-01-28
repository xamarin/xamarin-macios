//
// This file contains definitions used in the MediaPlayer namespace
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2015 Xamarin, Inc.
//

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

#nullable enable

namespace MediaPlayer {
	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMoviePlaybackState : long {
		Stopped,
		Playing,
		Paused,
		Interrupted,
		SeekingForward,
		SeekingBackward
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieLoadState : long {
		Unknown        = 0,
		Playable       = 1 << 0,
		PlaythroughOK  = 1 << 1,
		Stalled        = 1 << 2,		
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieRepeatMode : long {
		None, One
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieControlStyle : long {
		None, Embedded, Fullscreen, Default = Embedded
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieFinishReason : long {
		PlaybackEnded, PlaybackError, UserExited
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	[Flags]
	public enum MPMovieMediaType : long {
		None = 0,
		Video = 1 << 0,
		Audio = 1 << 1
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieSourceType : long {
		Unknown, File, Streaming
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieTimeOption : long {
		NearestKeyFrame,
		Exact
	}

	// NSUInteger -> MPMediaItem.h
#if !NET
	[Watch (7,0)]
#endif
	[Native]
	[Flags]
	public enum MPMediaType : ulong {
		Music        = 1 << 0,
		Podcast      = 1 << 1,
		AudioBook    = 1 << 2,
		AudioITunesU = 1 << 3,
		AnyAudio     = 0x00ff,
		
#if NET
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[Mac (10,12,2)]
#endif
		Movie = 1 << 8,
#if NET
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[Mac (10,12,2)]
#endif
		TVShow = 1 << 9,
#if NET
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[Mac (10,12,2)]
#endif
		VideoPodcast = 1 << 10,
#if NET
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[Mac (10,12,2)]
#endif
		MusicVideo = 1 << 11,
#if NET
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[Mac (10,12,2)]
#endif
		VideoITunesU = 1 << 12,
#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[iOS (7,0)]
		[Mac (10,12,2)]
#endif
		HomeVideo = 1 << 13,
#if NET
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[Mac (10,12,2)]
#endif
		TypeAnyVideo = 0xff00,
		Any          = 0xFFFFFFFFFFFFFFFF
	}

	// NSInteger -> MPMediaPlaylist.h
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
#endif
	[Native]
	[Flags]
	public enum MPMediaPlaylistAttribute : long {
		None    = 0,
		OnTheGo = (1 << 0), // if set, the playlist was created on a device rather than synced from iTunes
		Smart   = (1 << 1),
		Genius  = (1 << 2)
	};
			
	// NSInteger -> MPMediaQuery.h
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum MPMediaGrouping : long {
		Title,
		Album,
		Artist,
		AlbumArtist,
		Composer,
		Genre,
		Playlist,
		PodcastTitle
	}

	// NSInteger -> MPMediaQuery.h
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
#endif
	[Native]
	public enum MPMediaPredicateComparison : long {
		EqualsTo,
		Contains
	}

	// NSInteger -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
#endif
	[Native]
	public enum MPMovieScalingMode : long {
		None,
		AspectFit,
		AspectFill,
		Fill
	}
	
	// untyped enum -> MPMoviePlayerController.h
#if NET
	[UnsupportedOSPlatform ("macos")]
#else
	[NoMac]
#endif
	public enum MPMovieControlMode {
		Default, 
		VolumeOnly,
		Hidden   
	}

	// NSInteger -> /MPMusicPlayerController.h
#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoMac]
	[NoWatch]
	[TV (14,0)]
#endif
	[Native]
	public enum MPMusicPlaybackState : long {
		Stopped,
		Playing,
		Paused,
		Interrupted,
		SeekingForward,
		SeekingBackward
	}
	
	// NSInteger -> /MPMusicPlayerController.h
#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoMac]
	[NoWatch]
	[TV (14,0)]
#endif
	[Native]
	public enum MPMusicRepeatMode : long {
		Default,
		None,
		One,
		All
	}
	
	// NSInteger -> /MPMusicPlayerController.h
#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoMac]
	[NoWatch]
	[TV (14,0)]
#endif
	[Native]
	public enum MPMusicShuffleMode : long {
		Default,
		Off,
		Songs,
		Albums
	}

	public delegate void MPMediaItemEnumerator (string property, NSObject value, ref bool stop);

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
#else
	[Mac (10,12,2)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPShuffleType : long
	{
		Off,
		Items,
		Collections
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
#else
	[Mac (10,12,2)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPRepeatType : long
	{
		Off,
		One,
		All
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[SupportedOSPlatform ("ios10.0")]
#else
	[Mac (10,12,2)]
	[iOS (10,0)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPChangeLanguageOptionSetting : long
	{
		None,
		NowPlayingItemOnly,
		Permanent
	}

	// NSInteger -> MPRemoteCommand.h
#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[SupportedOSPlatform ("ios7.1")]
#else
	[Mac (10,12,2)]
	[iOS (7,1)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPRemoteCommandHandlerStatus : long {
		Success = 0,
		NoSuchContent = 100,
#if NET
		[SupportedOSPlatform ("ios9.1")]
		[SupportedOSPlatform ("macos10.12.2")]
#else
		[iOS (9,1)]
#endif
		NoActionableNowPlayingItem = 110,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
#else
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
#endif
		DeviceNotFound = 120,
		CommandFailed = 200
	}

	// NSUInteger -> MPRemoteCommandEvent.h
#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[SupportedOSPlatform ("ios7.1")]
#else
	[Mac (10,12,2)]
	[iOS (7,1)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPSeekCommandEventType : ulong {
		BeginSeeking,
		EndSeeking
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[SupportedOSPlatform ("ios9.0")]
#else
	[Mac (10,12,2)]
	[iOS (9,0)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPNowPlayingInfoLanguageOptionType : ulong {
		Audible,
		Legible
	}

#if NET
	[SupportedOSPlatform ("macos10.14.2")]
	[SupportedOSPlatform ("ios9.3")]
#else
	[Mac (10,14,2)]
	[Watch (7,0)]
	[iOS (9,3)]
#endif
	[Native]
	[ErrorDomain ("MPErrorDomain")]
	public enum MPErrorCode : long {
		Unknown,
		PermissionDenied,
		CloudServiceCapabilityMissing,
		NetworkConnectionFailed,
		NotFound,
		NotSupported,
		Cancelled,
		RequestTimedOut,
	}

#if NET
	[SupportedOSPlatform ("ios9.3")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMac]
	[NoTV]
	[NoWatch]
	[iOS (9,3)]
#endif
	[Native]
	public enum MPMediaLibraryAuthorizationStatus : long {
		NotDetermined = 0,
		Denied,
		Restricted,
		Authorized
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[Mac (10,12,2)]
	[iOS (10,0)]
	[TV (10,0)]
	[Watch (5,0)]
#endif
	[Native]
	public enum MPNowPlayingInfoMediaType : ulong
	{
		None = 0,
		Audio,
		Video
	}

#if NET
	[SupportedOSPlatform ("macos10.12.2")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[Mac (10,12,2)]
	[Watch (5,0)]
	[iOS (11, 0)]
	[TV (11, 0)]
#endif
	[Native]
	public enum MPNowPlayingPlaybackState : ulong
	{
		Unknown = 0,
		Playing,
		Paused,
		Stopped,
		Interrupted,
	}
}
