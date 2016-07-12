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
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.MediaPlayer {

	// NSInteger -> MPMoviePlayerController.h
	[Native]
	[NoTV]
	public enum MPMoviePlaybackState : nint {
		Stopped,
		Playing,
		Paused,
		Interrupted,
		SeekingForward,
		SeekingBackward
	}

	// NSInteger -> MPMoviePlayerController.h
	[Native]
	[NoTV]
	public enum MPMovieLoadState : nint {
		Unknown        = 0,
		Playable       = 1 << 0,
		PlaythroughOK  = 1 << 1,
		Stalled        = 1 << 2,		
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	public enum MPMovieRepeatMode : nint {
		None, One
	}

	// NSInteger -> MPMoviePlayerController.h
	[Native]
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	public enum MPMovieControlStyle : nint {
		None, Embedded, Fullscreen, Default = Embedded
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	public enum MPMovieFinishReason : nint {
		PlaybackEnded, PlaybackError, UserExited
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	[Flags]
	public enum MPMovieMediaType : nint {
		None = 0,
		Video = 1 << 0,
		Audio = 1 << 1
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	public enum MPMovieSourceType : nint {
		Unknown, File, Streaming
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	public enum MPMovieTimeOption : nint {
		NearestKeyFrame,
		Exact
	}

	// NSUInteger -> MPMediaItem.h
	[Native]
	[Flags]
	[NoTV]
	public enum MPMediaType : nuint_compat_int {
#if !XAMCORE_2_0
		[Obsolete ("Use Shorter name Music")]
		MPMediaTypeMusic        = 1 << 0,
		[Obsolete ("Use Shorter name Podcast")]
		MPMediaTypePodcast      = 1 << 1,
		[Obsolete ("Use Shorter name AudioBook")]
		MPMediaTypeAudioBook    = 1 << 2,
		[Obsolete ("Use Shorter name AnyAudio")]
		MPMediaTypeAnyAudio     = 0x00ff,
#endif	
		Music        = 1 << 0,
		Podcast      = 1 << 1,
		AudioBook    = 1 << 2,
		AudioITunesU = 1 << 3,
		AnyAudio     = 0x00ff,
		
		[iOS (5,0)]
		Movie = 1 << 8,
		[iOS (5,0)]
		TVShow = 1 << 9,
		[iOS (5,0)]
		VideoPodcast = 1 << 10,
		[iOS (5,0)]
		MusicVideo = 1 << 11,
		[iOS (5,0)]
		VideoITunesU = 1 << 12,
		[iOS (7,0)]
		HomeVideo = 1 << 13,
		[iOS (5,0)]
		TypeAnyVideo = 0xff00,
#if XAMCORE_2_0
		Any          = 0xFFFFFFFFFFFFFFFF
#else
		Any          = ~0
#endif
	}

	// NSInteger -> MPMediaPlaylist.h
	[Native]
	[Flags]
	[NoTV]
	public enum MPMediaPlaylistAttribute : nint {
		None    = 0,
		OnTheGo = (1 << 0), // if set, the playlist was created on a device rather than synced from iTunes
		Smart   = (1 << 1),
		Genius  = (1 << 2)
	};
			
	// NSInteger -> MPMediaQuery.h
	[Native]
	[NoTV]
	public enum MPMediaGrouping : nint {
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
	[Native]
	[NoTV]
	public enum MPMediaPredicateComparison : nint {
		EqualsTo,
		Contains
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	public enum MPMovieScalingMode : nint {
		None,
		AspectFit,
		AspectFill,
		Fill
	}
	
	// untyped enum -> MPMoviePlayerController.h
	public enum MPMovieControlMode {
		Default, 
		VolumeOnly,
		Hidden   
	}

	// NSInteger -> /MPMusicPlayerController.h
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[Native]
	public enum MPMusicPlaybackState : nint {
		Stopped,
		Playing,
		Paused,
		Interrupted,
		SeekingForward,
		SeekingBackward
	}
	
	// NSInteger -> /MPMusicPlayerController.h
	[Native]
	[NoTV]
	public enum MPMusicRepeatMode : nint {
		Default,
		None,
		One,
		All
	}
	
	// NSInteger -> /MPMusicPlayerController.h
	[Native]
	[NoTV]
	public enum MPMusicShuffleMode : nint {
		Default,
		Off,
		Songs,
		Albums
	}

	public delegate void MPMediaItemEnumerator (string property, NSObject value, ref bool stop);

	// NSInteger -> MPRemoteCommand.h
	[Native]
	[iOS (7,1)]
	public enum MPRemoteCommandHandlerStatus : nint {
		Success = 0,
		NoSuchContent = 100,
		[iOS (9,1)]
		NoActionableNowPlayingItem = 110,
		CommandFailed = 200
	}

	// NSUInteger -> MPRemoteCommandEvent.h
	[Native]
	[iOS (7,1)]
	public enum MPSeekCommandEventType : nuint_compat_int {
		BeginSeeking,
		EndSeeking
	}

	[iOS (9,0)]
	[Native]
	public enum MPNowPlayingInfoLanguageOptionType : nuint {
		Audible,
		Legible
	}

	[iOS (9,3)]
	[Native]
	[ErrorDomain ("MPErrorDomain")]
	public enum MPErrorCode : nint {
		Unknown,
		PermissionDenied,
		CloudServiceCapabilityMissing,
		NetworkConnectionFailed,
		NotFound,
		NotSupported
	}

	[NoTV]
	[iOS (9,3)]
	[Native]
	public enum MPMediaLibraryAuthorizationStatus : nint {
		NotDetermined = 0,
		Denied,
		Restricted,
		Authorized
	}

	[iOS (10,0)]
	[Native]
	public enum MPNowPlayingInfoMediaType : nuint
	{
		None = 0,
		Audio,
		Video
	}

}
