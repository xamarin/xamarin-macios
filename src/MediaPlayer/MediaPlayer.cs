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
	/// <summary>An enumeration of possible states in which the <see cref="T:MediaPlayer.MPMoviePlayerController" /> may be. Used with the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Media%20Player%20MPMovie%20Cotnroller%20Playback%20State&amp;scope=Xamarin" title="P:MediaPlayer.MPMovieCotnroller.PlaybackState">P:MediaPlayer.MPMovieCotnroller.PlaybackState</a></format> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMoviePlaybackState : long {
		/// <summary>To be added.</summary>
		Stopped,
		/// <summary>To be added.</summary>
		Playing,
		/// <summary>To be added.</summary>
		Paused,
		/// <summary>To be added.</summary>
		Interrupted,
		/// <summary>To be added.</summary>
		SeekingForward,
		/// <summary>To be added.</summary>
		SeekingBackward,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration whose values reflect a movie's load state. Used in the <see cref="P:MediaPlayer.MPMoviePlayerController.LoadState" /> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieLoadState : long {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		Playable = 1 << 0,
		/// <summary>To be added.</summary>
		PlaythroughOK = 1 << 1,
		/// <summary>To be added.</summary>
		Stalled = 1 << 2,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration that specifies whether a movie should repeat or not. Used with the <see cref="P:MediaPlayer.MPMoviePlayerController.RepeatMode" /> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieRepeatMode : long {
		None,
		One,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration whose values specify various modes for the <see cref="P:MediaPlayer.MPMoviePlayerController.ControlStyle" /> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieControlStyle : long {
		None,
		Embedded,
		Fullscreen,
		Default = Embedded,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration whose values specify various ways a movie may have finished.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieFinishReason : long {
		PlaybackEnded,
		PlaybackError,
		UserExited,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration that specifies the movie's media types. Used with the <see cref="P:MediaPlayer.MPMoviePlayerController.MovieMediaTypes" /> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native ("MPMovieMediaTypeMask")]
	[Flags]
	public enum MPMovieMediaType : long {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Video = 1 << 0,
		/// <summary>To be added.</summary>
		Audio = 1 << 1,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration that specifies whether a movie's data is provided by a file or streaming. Used with the <see cref="P:MediaPlayer.MPMoviePlayerController.SourceType" /> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieSourceType : long {
		Unknown,
		File,
		Streaming,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration that specifies which frame to use when generating thumbnails.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieTimeOption : long {
		NearestKeyFrame,
		Exact,
	}

	// NSUInteger -> MPMediaItem.h
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
		Any = 0xFFFFFFFFFFFFFFFF,
	}

	// NSInteger -> MPMediaPlaylist.h
	/// <summary>An enumeration whose values specify various types of playlist.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum MPMediaPlaylistAttribute : long {
		None = 0,
		OnTheGo = (1 << 0), // if set, the playlist was created on a device rather than synced from iTunes
		Smart = (1 << 1),
		Genius = (1 << 2),
	};

	// NSInteger -> MPMediaQuery.h
	/// <summary>An enumeration whose values specify various ways in which media can be logically grouped.</summary>
	[NoMac]
	[NoTV]
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
		PodcastTitle,
	}

	// NSInteger -> MPMediaQuery.h
	/// <summary>An enumeration whose values specifies a comparison-type to be used with a <see cref="T:MediaPlayer.MPMediaPredicate" />.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMediaPredicateComparison : long {
		EqualsTo,
		Contains,
	}

	// NSInteger -> MPMoviePlayerController.h
	/// <summary>An enumeration of video scaling modes. Used with the <see cref="P:MediaPlayer.MPMoviePlayerController.ScalingMode" /> property.</summary>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum MPMovieScalingMode : long {
		None,
		AspectFit,
		AspectFill,
		Fill,
	}

	// untyped enum -> MPMoviePlayerController.h
	[NoMac]
	[MacCatalyst (13, 1)]
	public enum MPMovieControlMode {
		Default,
		VolumeOnly,
		Hidden,
	}

	// NSInteger -> /MPMusicPlayerController.h
	[NoMac]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMusicPlaybackState : long {
		/// <summary>To be added.</summary>
		Stopped,
		/// <summary>To be added.</summary>
		Playing,
		/// <summary>To be added.</summary>
		Paused,
		/// <summary>To be added.</summary>
		Interrupted,
		/// <summary>To be added.</summary>
		SeekingForward,
		/// <summary>To be added.</summary>
		SeekingBackward,
	}

	// NSInteger -> /MPMusicPlayerController.h
	[NoMac]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMusicRepeatMode : long {
		Default,
		None,
		One,
		All,
	}

	// NSInteger -> /MPMusicPlayerController.h
	[NoMac]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMusicShuffleMode : long {
		Default,
		Off,
		Songs,
		Albums,
	}

	public delegate void MPMediaItemEnumerator (string property, NSObject value, ref bool stop);

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPShuffleType : long {
		Off,
		Items,
		Collections,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPRepeatType : long {
		Off,
		One,
		All,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPChangeLanguageOptionSetting : long {
		None,
		NowPlayingItemOnly,
		Permanent,
	}

	// NSInteger -> MPRemoteCommand.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPRemoteCommandHandlerStatus : long {
		Success = 0,
		NoSuchContent = 100,
		[MacCatalyst (13, 1)]
		NoActionableNowPlayingItem = 110,
		[MacCatalyst (13, 1)]
		DeviceNotFound = 120,
		CommandFailed = 200,
	}

	// NSUInteger -> MPRemoteCommandEvent.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSeekCommandEventType : ulong {
		BeginSeeking,
		EndSeeking
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPNowPlayingInfoLanguageOptionType : ulong {
		Audible,
		Legible,
	}

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

	/// <summary>Enumerates the status of the application's permission to access the media library.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum MPMediaLibraryAuthorizationStatus : long {
		NotDetermined = 0,
		Denied,
		Restricted,
		Authorized,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPNowPlayingInfoMediaType : ulong {
		None = 0,
		Audio,
		Video,
	}

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
