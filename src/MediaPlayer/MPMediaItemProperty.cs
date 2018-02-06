//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2015 Xamarin, Inc.
//

#if !XAMCORE_2_0 && !MONOMAC

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace MediaPlayer {

	[Obsolete ("The members of this class are available in MPMediaItem as static properties and are more complete")]
	public static class MPMediaItemProperty {
		public const string PersistentID = "persistentID";
		public const string MediaType = "mediaType"; 
		public const string Title = "title"; 
		public const string AlbumTitle = "albumTitle";
		public const string Artist = "artist"; 
		public const string AlbumArtist = "albumArtist";
		public const string Genre = "genre";
		public const string Composer = "composer";
		public const string PlaybackDuration = "playbackDuration";
		public const string AlbumTrackNumber = "albumTrackNumber";
		public const string AlbumTrackCount = "albumTrackCount";
		public const string DiscNumber = "discNumber";
		public const string DiscCount = "discCount";
		public const string Artwork = "artwork";
		public const string Lyrics = "lyrics";
		public const string IsCompilation = "isCompilation";
		public const string PodcastTitle = "podcastTitle";
		public const string PlayCount = "playCount";
		public const string SkipCount = "skipCount";
		public const string Rating = "rating";
		public const string LastPlayedDate = "lastPlayedDate";

		[iOS (4,0)]
		public const string BeatsPerMinute = "beatsPerMinute";
		[iOS (4,0)]
		public const string Comments = "comments";
		[iOS (4,0)]
		public const string AssetUrl = "assetURL";
		[iOS (4,0)]
		public const string ReleaseDate = "releaseDate";
		[iOS (4,0)]
		public const string UserGrouping = "userGrouping";
	}
}

#endif
