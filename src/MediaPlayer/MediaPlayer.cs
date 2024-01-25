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

#nullable enable

namespace MediaPlayer {
	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
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
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieLoadState : long {
		Unknown = 0,
		Playable = 1 << 0,
		PlaythroughOK = 1 << 1,
		Stalled = 1 << 2,
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieRepeatMode : long {
		None, One
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieControlStyle : long {
		None, Embedded, Fullscreen, Default = Embedded
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieFinishReason : long {
		PlaybackEnded, PlaybackError, UserExited
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native ("MPMovieMediaTypeMask")]
	[Flags]
	public enum MPMovieMediaType : long {
		None = 0,
		Video = 1 << 0,
		Audio = 1 << 1
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieSourceType : long {
		Unknown, File, Streaming
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieTimeOption : long {
		NearestKeyFrame,
		Exact
	}

	// NSUInteger -> MPMediaItem.h
	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum MPMediaType : ulong {
		Music = 1 << 0,
		Podcast = 1 << 1,
		AudioBook = 1 << 2,
		AudioITunesU = 1 << 3,
		AnyAudio = 0x00ff,

		[MacCatalyst (13, 1)]
		Movie = 1 << 8,
		[MacCatalyst (13, 1)]
		TVShow = 1 << 9,
		[MacCatalyst (13, 1)]
		VideoPodcast = 1 << 10,
		[MacCatalyst (13, 1)]
		MusicVideo = 1 << 11,
		[MacCatalyst (13, 1)]
		VideoITunesU = 1 << 12,
		[MacCatalyst (13, 1)]
		HomeVideo = 1 << 13,
		[MacCatalyst (13, 1)]
		TypeAnyVideo = 0xff00,
		Any = 0xFFFFFFFFFFFFFFFF
	}

	// NSInteger -> MPMediaPlaylist.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum MPMediaPlaylistAttribute : long {
		None = 0,
		OnTheGo = (1 << 0), // if set, the playlist was created on a device rather than synced from iTunes
		Smart = (1 << 1),
		Genius = (1 << 2)
	};

	// NSInteger -> MPMediaQuery.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
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
	[NoMac]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMediaPredicateComparison : long {
		EqualsTo,
		Contains
	}

	// NSInteger -> MPMoviePlayerController.h
	[NoMac]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieScalingMode : long {
		None,
		AspectFit,
		AspectFill,
		Fill
	}

	// untyped enum -> MPMoviePlayerController.h
	[NoMac]
	[MacCatalyst (13, 1)]
	public enum MPMovieControlMode {
		Default,
		VolumeOnly,
		Hidden
	}

	// NSInteger -> /MPMusicPlayerController.h
	[NoMac]
	[NoWatch]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
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
	[NoMac]
	[NoWatch]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMusicRepeatMode : long {
		Default,
		None,
		One,
		All
	}

	// NSInteger -> /MPMusicPlayerController.h
	[NoMac]
	[NoWatch]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMusicShuffleMode : long {
		Default,
		Off,
		Songs,
		Albums
	}

	public delegate void MPMediaItemEnumerator (string property, NSObject value, ref bool stop);

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPShuffleType : long {
		Off,
		Items,
		Collections
	}

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPRepeatType : long {
		Off,
		One,
		All
	}

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPChangeLanguageOptionSetting : long {
		None,
		NowPlayingItemOnly,
		Permanent
	}

	// NSInteger -> MPRemoteCommand.h
	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPRemoteCommandHandlerStatus : long {
		Success = 0,
		NoSuchContent = 100,
		[MacCatalyst (13, 1)]
		NoActionableNowPlayingItem = 110,
		[MacCatalyst (13, 1)]
		DeviceNotFound = 120,
		CommandFailed = 200
	}

	// NSUInteger -> MPRemoteCommandEvent.h
	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSeekCommandEventType : ulong {
		BeginSeeking,
		EndSeeking
	}

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPNowPlayingInfoLanguageOptionType : ulong {
		Audible,
		Legible
	}

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
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

	[NoMac]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMediaLibraryAuthorizationStatus : long {
		NotDetermined = 0,
		Denied,
		Restricted,
		Authorized
	}

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPNowPlayingInfoMediaType : ulong {
		None = 0,
		Audio,
		Video
	}

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPNowPlayingPlaybackState : ulong {
		Unknown = 0,
		Playing,
		Paused,
		Stopped,
		Interrupted,
	}
}
