// Copyright 2018, Microsoft, Corp.
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

using System;
using System.ComponentModel;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace iTunesLibrary {

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface ITLibAlbum {
		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("sortTitle")]
		string SortTitle { get; }

		[Export ("compilation")]
		bool Compilation { [Bind ("isCompilation")] get; }

		[NullAllowed, Export ("artist", ArgumentSemantic.Retain)]
		ITLibArtist Artist { get; }

		[Export ("discCount")]
		nuint DiscCount { get; }

		[Export ("discNumber")]
		nuint DiscNumber { get; }

		[Export ("rating")]
		nint Rating { get; }

		[Export ("ratingComputed")]
		bool RatingComputed { [Bind ("isRatingComputed")] get; }

		[Export ("gapless")]
		bool Gapless { [Bind ("isGapless")] get; }

		[Export ("trackCount")]
		nuint TrackCount { get; }

		[NullAllowed, Export ("albumArtist")]
		string AlbumArtist { get; }

		[NullAllowed, Export ("sortAlbumArtist")]
		string SortAlbumArtist { get; }

		[Export ("persistentID", ArgumentSemantic.Retain)]
		NSNumber PersistentId { get; }
	}

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface ITLibArtist
	{
		[NullAllowed, Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("sortName")]
		string SortName { get; }

		[Export ("persistentID", ArgumentSemantic.Retain)]
		NSNumber PersistentId { get; }
	}

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface ITLibArtwork
	{
		[NullAllowed, Export ("image", ArgumentSemantic.Retain)]
		NSImage Image { get; }

		[NullAllowed, Export ("imageData", ArgumentSemantic.Retain)]
		NSData ImageData { get; }

		[Export ("imageDataFormat", ArgumentSemantic.Assign)]
		ITLibArtworkFormat ImageDataFormat { get; }
	}

	delegate void ITLibMediaEntityEnumerateValuesHandler (NSString property, NSObject value, out bool stop);

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface ITLibMediaEntity
	{
		[Export ("persistentID", ArgumentSemantic.Retain)]
		NSNumber PersistentId { get; }

		[Export ("valueForProperty:")]
		[return: NullAllowed]
		NSObject GetValue (string property);

		[Export ("enumerateValuesForProperties:usingBlock:")]
		void EnumerateValues (NSSet<NSString> properties, ITLibMediaEntityEnumerateValuesHandler handler);

		[Export ("enumerateValuesExceptForProperties:usingBlock:")]
		void EnumerateValuesExcept (NSSet<NSString> properties, ITLibMediaEntityEnumerateValuesHandler handler);
	}

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(ITLibMediaEntity))]
	interface ITLibMediaItem
	{
		[Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("sortTitle")]
		string SortTitle { get; }

		[NullAllowed, Export ("artist", ArgumentSemantic.Retain)]
		ITLibArtist Artist { get; }

		[Export ("composer")]
		string Composer { get; }

		[NullAllowed, Export ("sortComposer")]
		string SortComposer { get; }

		[Export ("rating")]
		nint Rating { get; }

		[Export ("ratingComputed")]
		bool RatingComputed { [Bind ("isRatingComputed")] get; }

		[Export ("startTime")]
		nuint StartTime { get; }

		[Export ("stopTime")]
		nuint StopTime { get; }

		[Export ("album", ArgumentSemantic.Retain)]
		ITLibAlbum Album { get; }

		[Export ("genre")]
		string Genre { get; }

		[NullAllowed, Export ("kind")]
		string Kind { get; }

		[Export ("mediaKind", ArgumentSemantic.Assign)]
		ITLibMediaItemMediaKind MediaKind { get; }

		[Export ("fileSize")]
		ulong FileSize { get; }

		[Export ("size")]
		nuint Size { get; }

		[Export ("totalTime")]
		nuint TotalTime { get; }

		[Export ("trackNumber")]
		nuint TrackNumber { get; }

		[NullAllowed, Export ("category")]
		string Category { get; }

		[NullAllowed, Export ("description")]
		string Description { get; }

		[Export ("lyricsContentRating", ArgumentSemantic.Assign)]
		ITLibMediaItemLyricsContentRating LyricsContentRating { get; }

		[NullAllowed, Export ("contentRating")]
		string ContentRating { get; }

		[NullAllowed, Export ("modifiedDate", ArgumentSemantic.Retain)]
		NSDate ModifiedDate { get; }

		[NullAllowed, Export ("addedDate", ArgumentSemantic.Retain)]
		NSDate AddedDate { get; }

		[Export ("bitrate")]
		nuint Bitrate { get; }

		[Export ("sampleRate")]
		nuint SampleRate { get; }

		[Export ("beatsPerMinute")]
		nuint BeatsPerMinute { get; }

		[Export ("playCount")]
		nuint PlayCount { get; }

		[NullAllowed, Export ("lastPlayedDate", ArgumentSemantic.Retain)]
		NSDate LastPlayedDate { get; }

		[Export ("playStatus", ArgumentSemantic.Assign)]
		ITLibMediaItemPlayStatus PlayStatus { get; }

		[NullAllowed, Export ("location", ArgumentSemantic.Retain)]
		NSUrl Location { get; }

		[Export ("artworkAvailable")]
		bool ArtworkAvailable { [Bind ("hasArtworkAvailable")] get; }

		[NullAllowed, Export ("artwork", ArgumentSemantic.Retain)]
		ITLibArtwork Artwork { get; }

		[NullAllowed, Export ("comments")]
		string Comments { get; }

		[Export ("purchased")]
		bool Purchased { [Bind ("isPurchased")] get; }

		[Export ("cloud")]
		bool Cloud { [Bind ("isCloud")] get; }

		[Export ("drmProtected")]
		bool DrmProtected { [Bind ("isDRMProtected")] get; }

		[Export ("video")]
		bool Video { [Bind ("isVideo")] get; }

		[NullAllowed, Export ("videoInfo", ArgumentSemantic.Retain)]
		ITLibMediaItemVideoInfo VideoInfo { get; }

		[NullAllowed, Export ("releaseDate", ArgumentSemantic.Retain)]
		NSDate ReleaseDate { get; }

		[Export ("year")]
		nuint Year { get; }

		[Export ("fileType")]
		nuint FileType { get; }

		[Export ("skipCount")]
		nuint SkipCount { get; }

		[NullAllowed, Export ("skipDate", ArgumentSemantic.Retain)]
		NSDate SkipDate { get; }

		[NullAllowed, Export ("voiceOverLanguage")]
		string VoiceOverLanguage { get; }

		[Export ("volumeAdjustment")]
		nint VolumeAdjustment { get; }

		[Export ("volumeNormalizationEnergy")]
		nuint VolumeNormalizationEnergy { get; }

		[Export ("userDisabled")]
		bool UserDisabled { [Bind ("isUserDisabled")] get; }

		[NullAllowed, Export ("grouping")]
		string Grouping { get; }

		[Export ("locationType", ArgumentSemantic.Assign)]
		ITLibMediaItemLocationType LocationType { get; }
	}

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface ITLibMediaItemVideoInfo
	{
		[NullAllowed, Export ("series")]
		string Series { get; }

		[NullAllowed, Export ("sortSeries")]
		string SortSeries { get; }

		[Export ("season")]
		nuint Season { get; }

		[NullAllowed, Export ("episode")]
		string Episode { get; }

		[Export ("episodeOrder")]
		nint EpisodeOrder { get; }

		[Export ("hd")]
		bool Hd { [Bind ("isHD")] get; }

		[Export ("videoWidth")]
		nuint VideoWidth { get; }

		[Export ("videoHeight")]
		nuint VideoHeight { get; }
	}

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(ITLibMediaEntity))]
	interface ITLibPlaylist
	{
		[Export ("name")]
		string Name { get; }

		[Export ("master")]
		bool Master { [Bind ("isMaster")] get; }

		[NullAllowed, Export ("parentID", ArgumentSemantic.Retain)]
		NSNumber ParentId { get; }

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }

		[Export ("allItemsPlaylist")]
		bool AllItemsPlaylist { [Bind ("isAllItemsPlaylist")] get; }

		[Export ("items", ArgumentSemantic.Retain)]
		ITLibMediaItem[] Items { get; }

		[Export ("distinguishedKind", ArgumentSemantic.Assign)]
		ITLibDistinguishedPlaylistKind DistinguishedKind { get; }

		[Export ("kind", ArgumentSemantic.Assign)]
		ITLibPlaylistKind Kind { get; }
	}

	[Mac (10,14, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface ITLibrary
	{
		[Export ("applicationVersion")]
		string ApplicationVersion { get; }

		[Export ("features", ArgumentSemantic.Assign)]
		ITLibExportFeature Features { get; }

		[Export ("apiMajorVersion")]
		nuint ApiMajorVersion { get; }

		[Export ("apiMinorVersion")]
		nuint ApiMinorVersion { get; }

		[NullAllowed, Export ("mediaFolderLocation", ArgumentSemantic.Copy)]
		NSUrl MediaFolderLocation { get; }

		[NullAllowed, Export ("musicFolderLocation", ArgumentSemantic.Copy)]
		NSUrl MusicFolderLocation { get; }

		[Export ("showContentRating")]
		bool ShowContentRating { [Bind ("shouldShowContentRating")] get; }

		[Export ("allMediaItems", ArgumentSemantic.Retain)]
		ITLibMediaItem[] AllMediaItems { get; }

		[Export ("allPlaylists", ArgumentSemantic.Retain)]
		ITLibPlaylist[] AllPlaylists { get; }

		[Static]
		[Export ("libraryWithAPIVersion:error:")]
		[return: NullAllowed]
		ITLibrary GetLibrary (string requestedAPIVersion, [NullAllowed] out NSError error);

		[Static]
		[Export ("libraryWithAPIVersion:options:error:")]
		[return: NullAllowed]
		ITLibrary GetLibrary (string requestedAPIVersion, ITLibInitOptions options, [NullAllowed] out NSError error);

		[Export ("initWithAPIVersion:error:")]
		IntPtr Constructor (string requestedAPIVersion, [NullAllowed] out NSError error);

		[Export ("initWithAPIVersion:options:error:")]
		IntPtr Constructor (string requestedAPIVersion, ITLibInitOptions options, [NullAllowed] out NSError error);

		[Export ("artworkForMediaFile:")]
		[return: NullAllowed]
		ITLibArtwork GetArtwork (NSUrl mediaFileUrl);

		[Export ("reloadData")]
		bool ReloadData ();

		[Export ("unloadData")]
		void UnloadData ();
	}

}