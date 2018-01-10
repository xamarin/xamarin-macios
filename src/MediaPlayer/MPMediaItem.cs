// 
// MPMediaItem.cs: 
//
// Authors:
//   Geoff Norton.
//   Miguel de Icaza
//     
// Copyright 2011 Novell, Inc
// Copyright 2011-2012 Xamarin, Inc
//

#if !TVOS && !MONOMAC

using System;
using System.Collections;
using Foundation; 
using ObjCRuntime;
using CoreGraphics;

namespace MediaPlayer {
	public partial class MPMediaItem {

#if !XAMCORE_2_0
		[Obsolete ("Use 'CanFilterByProperty (NSString)' instead.")]
		public static bool CanFilterByProperty (string property)
		{
			using (NSString ns = new NSString (property))
				return CanFilterByProperty (ns);
		}

		[Obsolete ("Use 'ValueForProperty (NSString)' instead.")]
		public virtual NSObject ValueForProperty (string property)
		{
			using (NSString ns = new NSString (property))
				return ValueForProperty (ns);
		}
#endif

		ulong UInt64ForProperty (NSString property)
		{
			var prop = ValueForProperty (property) as NSNumber;
			if (prop == null)
				return 0;
			return prop.UInt64Value;
		}

		uint UInt32ForProperty (NSString property)
		{
			var prop = ValueForProperty (property) as NSNumber;
			if (prop == null)
				return 0;
			return prop.UInt32Value;
		}

		int Int32ForProperty (NSString property)
		{
			var prop = ValueForProperty (property) as NSNumber;
			if (prop == null)
				return 0;
			return prop.Int32Value;
		}

		double DoubleForProperty (NSString property)
		{
			var prop = ValueForProperty (property) as NSNumber;
			if (prop == null)
				return 0;
			return prop.DoubleValue;
		}

		public ulong PersistentID {
			get {
				return UInt64ForProperty (PersistentIDProperty);
			}
		}

		public ulong AlbumPersistentID {
			get {
				return UInt64ForProperty (AlbumPersistentIDProperty);
			}
		}
		
		public ulong ArtistPersistentID {
			get {
				return UInt64ForProperty (ArtistPersistentIDProperty);
			}
		}

		public ulong AlbumArtistPersistentID {
			get {
				return UInt64ForProperty (AlbumArtistPersistentIDProperty);
			}
		}

		public ulong GenrePersistentID {
			get {
				return UInt64ForProperty (GenrePersistentIDProperty);
			}
		}

		public ulong ComposerPersistentID {
			get {
				return UInt64ForProperty (ComposerPersistentIDProperty);
			}
		}

		public ulong PodcastPersistentID {
			get {
				return UInt64ForProperty (PodcastPersistentIDProperty);
			}
		}

		public MPMediaType MediaType {
			get {
				return (MPMediaType) Int32ForProperty (MediaTypeProperty);
			}
		}

		public NSString Title {
			get {
				return ValueForProperty (TitleProperty) as NSString;
			}
		}

		public NSString AlbumTitle {
			get {
				return ValueForProperty (AlbumTitleProperty) as NSString;
			}
		}

		public NSString Artist {
			get {
				return ValueForProperty (ArtistProperty) as NSString;
			}
		}

		public NSString AlbumArtist {
			get {
				return ValueForProperty (AlbumArtistProperty) as NSString;
			}
		}

		public NSString Genre {
			get {
				return ValueForProperty (GenreProperty) as NSString;
			}
		}

		public NSString Composer {
			get {
				return ValueForProperty (ComposerProperty) as NSString;
			}
		}

		public double PlaybackDuration {
			get {
				return DoubleForProperty (PlaybackDurationProperty);
			}
		}

		public int AlbumTrackNumber {
			get {
				return Int32ForProperty (AlbumTrackNumberProperty);
			}
		}

		public int AlbumTrackCount {
			get {
				return Int32ForProperty (AlbumTrackCountProperty);
			}
		}

		public int DiscNumber {
			get {
				return Int32ForProperty (DiscNumberProperty);
			}
		}

		public int DiscCount {
			get {
				return Int32ForProperty (DiscCountProperty);
			}
		}

		public MPMediaItemArtwork Artwork {
			get {
				return (ValueForProperty (ArtworkProperty) as MPMediaItemArtwork);
			}
		}

		public NSString Lyrics {
			get {
				return ValueForProperty (LyricsProperty) as NSString;
			}
		}

		public bool IsCompilation {
			get {
				return Int32ForProperty (IsCompilationProperty) != 0;
			}
		}

		public NSDate ReleaseDate {
			get {
				return (ValueForProperty (ReleaseDateProperty) as NSDate);
			}
		}

		public uint BeatsPerMinute {
			get {
				return UInt32ForProperty (BeatsPerMinuteProperty);
			}
		}

		public NSString Comments {
			get {
				return ValueForProperty (CommentsProperty) as NSString;
			}
		}

		public NSUrl AssetURL {
			get {
				return ValueForProperty (AssetURLProperty) as NSUrl;
			}
		}

		public int PlayCount {
			get {
				return Int32ForProperty (PlayCountProperty);
			}
		}

		public int SkipCount {
			get {
				return Int32ForProperty (SkipCountProperty);
			}
		}

		public uint Rating {
			get {
				return UInt32ForProperty (RatingProperty);
			}
		}

		public NSDate LastPlayedDate {
			get {
				return (ValueForProperty (LastPlayedDateProperty) as NSDate);
			}
		}

		public NSString UserGrouping {
			get {
				return ValueForProperty (UserGroupingProperty) as NSString;
			}
		}

		public NSString PodcastTitle {
			get {
				return ValueForProperty (PodcastTitleProperty) as NSString;
			}
		}

		public double BookmarkTime {
			get {
				return DoubleForProperty (BookmarkTimeProperty);
			}
		}

		public bool IsCloudItem {
			get {
				return Int32ForProperty (IsCloudItemProperty) != 0;
			}
		}
		
		[iOS (9,2)]
		public bool HasProtectedAsset {
			get {
				return Int32ForProperty (HasProtectedAssetProperty) != 0;
			}
		}

		[iOS (10,0)]
		public bool IsExplicitItem {
			get {
				return Int32ForProperty (IsExplicitProperty) != 0;
			}
		}

		[iOS (10,0)]
		public NSDate DateAdded {
			get {
				return (ValueForProperty (DateAddedProperty) as NSDate);
			}
		}

		[iOS (10,3)]
		public NSString PlaybackStoreID {
			get {
				return (ValueForProperty (PlaybackStoreIDProperty) as NSString);
			}
		}
	}
}

#endif // !TVOS
