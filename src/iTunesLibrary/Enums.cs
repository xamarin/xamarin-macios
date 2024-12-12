// Copyright 2018, Microsoft Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;
using Foundation;
using ObjCRuntime;

namespace iTunesLibrary {

	[Native]
	public enum ITLibArtworkFormat : ulong {
		None = 0,
		Bitmap = 1,
		Jpeg = 2,
		Jpeg2000 = 3,
		Gif = 4,
		Png = 5,
		Bmp = 6,
		Tiff = 7,
		Pict = 8,
	}

	[Native]
	public enum ITLibMediaItemMediaKind : ulong {
		Unknown = 1,
		Song = 2,
		Movie = 3,
		Podcast = 4,
		Audiobook = 5,
		PdfBooklet = 6,
		MusicVideo = 7,
		TVShow = 8,
		InteractiveBooklet = 9,
		HomeVideo = 12,
		Ringtone = 14,
		DigitalBooklet = 15,
		iOSApplication = 16,
		VoiceMemo = 17,
		iTunesU = 18,
		Book = 19,
		PdfBook = 20,
		AlertTone = 21,
	}

	[Native]
	public enum ITLibMediaItemLyricsContentRating : ulong {
		None = 0,
		Explicit = 1,
		Clean = 2,
	}

	[Native]
	public enum ITLibMediaItemLocationType : ulong {
		Unknown = 0,
		File = 1,
		Url = 2,
		Remote = 3,
	}

	[Native]
	public enum ITLibMediaItemPlayStatus : ulong {
		None = 0,
		PartiallyPlayed = 1,
		Unplayed = 2,
	}

	[Native]
	public enum ITLibDistinguishedPlaylistKind : ulong {
		None = 0,
		Movies = 1,
		TVShows = 2,
		Music = 3,
		Audiobooks = 4,
		Books = Audiobooks,
		Ringtones = 5,
		Podcasts = 7,
		VoiceMemos = 14,
		Purchases = 16,
		iTunesU = 26,
		NightiesMusic = 42,
		MyTopRated = 43,
		Top25MostPlayed = 44,
		RecentlyPlayed = 45,
		RecentlyAdded = 46,
		MusicVideos = 47,
		ClassicalMusic = 48,
		LibraryMusicVideos = 49,
		HomeVideos = 50,
		Applications = 51,
		LovedSongs = 52,
		MusicShowsAndMovies = 53,
	}

	[Native]
	public enum ITLibPlaylistKind : ulong {
		Regular,
		Smart,
		Genius,
		Folder,
		GeniusMix,
	}

	[Native]
	public enum ITLibExportFeature : ulong {
		ITLibExportFeatureNone = 0,
	}

	[Native]
	public enum ITLibInitOptions : ulong {
		None = 0,
		LazyLoadData = 1,
	}

	public enum MediaItemProperty {
		[Field ("ITLibMediaItemPropertyAlbumTitle")]
		AlbumTitle,
		[Field ("ITLibMediaItemPropertySortAlbumTitle")]
		SortAlbumTitle,
		[Field ("ITLibMediaItemPropertyAlbumArtist")]
		AlbumArtist,
		[Field ("ITLibMediaItemPropertyAlbumRating")]
		AlbumRating,
		[Field ("ITLibMediaItemPropertyAlbumRatingComputed")]
		AlbumRatingComputed,
		[Field ("ITLibMediaItemPropertySortAlbumArtist")]
		SortAlbumArtist,
		[Field ("ITLibMediaItemPropertyAlbumIsGapless")]
		AlbumIsGapless,
		[Field ("ITLibMediaItemPropertyAlbumIsCompilation")]
		AlbumIsCompilation,
		[Field ("ITLibMediaItemPropertyAlbumDiscCount")]
		AlbumDiscCount,
		[Field ("ITLibMediaItemPropertyAlbumDiscNumber")]
		AlbumDiscNumber,
		[Field ("ITLibMediaItemPropertyAlbumTrackCount")]
		AlbumTrackCount,
		[Field ("ITLibMediaItemPropertyArtistName")]
		ArtistName,
		[Field ("ITLibMediaItemPropertySortArtistName")]
		SortArtistName,
		[Field ("ITLibMediaItemPropertyVideoIsHD")]
		VideoIsHD,
		[Field ("ITLibMediaItemPropertyVideoWidth")]
		VideoWidth,
		[Field ("ITLibMediaItemPropertyVideoHeight")]
		VideoHeight,
		[Field ("ITLibMediaItemPropertyVideoSeries")]
		VideoSeries,
		[Field ("ITLibMediaItemPropertyVideoSortSeries")]
		VideoSortSeries,
		[Field ("ITLibMediaItemPropertyVideoSeason")]
		VideoSeason,
		[Field ("ITLibMediaItemPropertyVideoEpisode")]
		VideoEpisode,
		[Field ("ITLibMediaItemPropertyVideoEpisodeOrder")]
		VideoEpisodeOrder,
		[Field ("ITLibMediaItemPropertyHasArtwork")]
		HasArtwork,
		[Field ("ITLibMediaItemPropertyBitRate")]
		BitRate,
		[Field ("ITLibMediaItemPropertyBeatsPerMinute")]
		BeatsPerMinute,
		[Field ("ITLibMediaItemPropertyCategory")]
		Category,
		[Field ("ITLibMediaItemPropertyComments")]
		Comments,
		[Field ("ITLibMediaItemPropertyComposer")]
		Composer,
		[Field ("ITLibMediaItemPropertySortComposer")]
		SortComposer,
		[Field ("ITLibMediaItemPropertyContentRating")]
		ContentRating,
		[Field ("ITLibMediaItemPropertyLyricsContentRating")]
		LyricsContentRating,
		[Field ("ITLibMediaItemPropertyAddedDate")]
		AddedDate,
		[Field ("ITLibMediaItemPropertyModifiedDate")]
		ModifiedDate,
		[Field ("ITLibMediaItemPropertyDescription")]
		Description,
		[Field ("ITLibMediaItemPropertyIsUserDisabled")]
		IsUserDisabled,
		[Field ("ITLibMediaItemPropertyFileType")]
		FileType,
		[Field ("ITLibMediaItemPropertyGenre")]
		Genre,
		[Field ("ITLibMediaItemPropertyGrouping")]
		Grouping,
		[Field ("ITLibMediaItemPropertyIsVideo")]
		IsVideo,
		[Field ("ITLibMediaItemPropertyKind")]
		Kind,
		[Field ("ITLibMediaItemPropertyTitle")]
		Title,
		[Field ("ITLibMediaItemPropertySortTitle")]
		SortTitle,
		[Field ("ITLibMediaItemPropertyVolumeNormalizationEnergy")]
		VolumeNormalizationEnergy,
		[Field ("ITLibMediaItemPropertyPlayCount")]
		PlayCount,
		[Field ("ITLibMediaItemPropertyLastPlayDate")]
		LastPlayDate,
		[Field ("ITLibMediaItemPropertyPlayStatus")]
		PlayStatus,
		[Field ("ITLibMediaItemPropertyIsDRMProtected")]
		IsDrmProtected,
		[Field ("ITLibMediaItemPropertyIsPurchased")]
		IsPurchased,
		[Field ("ITLibMediaItemPropertyMovementCount")]
		MovementCount,
		[Field ("ITLibMediaItemPropertyMovementName")]
		MovementName,
		[Field ("ITLibMediaItemPropertyMovementNumber")]
		MovementNumber,
		[Field ("ITLibMediaItemPropertyRating")]
		Rating,
		[Field ("ITLibMediaItemPropertyRatingComputed")]
		RatingComputed,
		[Field ("ITLibMediaItemPropertyReleaseDate")]
		ReleaseDate,
		[Field ("ITLibMediaItemPropertySampleRate")]
		SampleRate,
		[Field ("ITLibMediaItemPropertySize")]
		Size,
		[Field ("ITLibMediaItemPropertyFileSize")]
		FileSize,
		[Field ("ITLibMediaItemPropertyUserSkipCount")]
		UserSkipCount,
		[Field ("ITLibMediaItemPropertySkipDate")]
		SkipDate,
		[Field ("ITLibMediaItemPropertyStartTime")]
		StartTime,
		[Field ("ITLibMediaItemPropertyStopTime")]
		StopTime,
		[Field ("ITLibMediaItemPropertyTotalTime")]
		TotalTime,
		[Field ("ITLibMediaItemPropertyTrackNumber")]
		TrackNumber,
		[Field ("ITLibMediaItemPropertyLocationType")]
		LocationType,
		[Field ("ITLibMediaItemPropertyVoiceOverLanguage")]
		VoiceOverLanguage,
		[Field ("ITLibMediaItemPropertyVolumeAdjustment")]
		VolumeAdjustment,
		[Field ("ITLibMediaItemPropertyWork")]
		Work,
		[Field ("ITLibMediaItemPropertyYear")]
		Year,
		[Field ("ITLibMediaItemPropertyMediaKind")]
		MediaKind,
		[Field ("ITLibMediaItemPropertyLocation")]
		Location,
		[Field ("ITLibMediaItemPropertyArtwork")]
		Artwork,
	}

	public enum ITLibPlaylistProperty {
		[Field ("ITLibPlaylistPropertyName")]
		Name,
		[Field ("ITLibPlaylistPropertyAllItemsPlaylist")]
		AllItemsPlaylist,
		[Field ("ITLibPlaylistPropertyDistinguisedKind")]
		DistinguisedKind,
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'Primary' instead.")]
		[Field ("ITLibPlaylistPropertyMaster")]
		Master,
		[Field ("ITLibPlaylistPropertyParentPersistentID")]
		ParentPersistentId,
		[Field ("ITLibPlaylistPropertyPrimary")]
		Primary,
		[Field ("ITLibPlaylistPropertyVisible")]
		Visible,
		[Field ("ITLibPlaylistPropertyItems")]
		Items,
		[Field ("ITLibPlaylistPropertyKind")]
		Kind,
	}

	public enum ITLibMediaEntityProperty {
		[Field ("ITLibMediaEntityPropertyPersistentID")]
		PersistentId,
	}
}
