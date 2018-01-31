//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2015, Xamarin Inc
//
using System.ComponentModel;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
#endif
using System;

namespace XamCore.MediaPlayer {
#if XAMCORE_2_0 || !MONOMAC
	[Mac (10,12,2, onlyOn64: true)] // type exists only to expose fields
	[BaseType (typeof (NSObject))]
#if XAMCORE_2_0 && IOS
	// introduced in 4.2
	interface MPMediaEntity : NSSecureCoding {
#else
	interface MPMediaItem : NSSecureCoding {
#endif
		[Static]
		[Export ("canFilterByProperty:")]
		bool CanFilterByProperty (NSString property);

		[Export ("valueForProperty:")]
		NSObject ValueForProperty (NSString property);

		[Since (4,0)]
		[Export ("enumerateValuesForProperties:usingBlock:")]
		void EnumerateValues (NSSet propertiesToEnumerate, MPMediaItemEnumerator enumerator);

		[iOS (8,0)]
		[Export ("objectForKeyedSubscript:")]
		NSObject GetObject (NSObject key);

		[Field ("MPMediaEntityPropertyPersistentID")]
		NSString PropertyPersistentID { get; }

#if XAMCORE_2_0 && IOS
	}
#if MONOMAC || TVOS
	[Mac (10,12,2, onlyOn64: true)]
	[Static]
#else
	[BaseType (typeof (MPMediaEntity))]
#endif
	interface MPMediaItem {
#endif
		[NoMac][NoTV]
		[Since (4,2)]
		[NoMac]
		[NoTV]
		[Export ("persistentIDPropertyForGroupingType:")][Static]
		string GetPersistentIDProperty (MPMediaGrouping groupingType);

		[NoMac][NoTV]
		[Since (4,2)]
		[NoMac]
		[NoTV]
		[Export ("titlePropertyForGroupingType:")][Static]
		string GetTitleProperty (MPMediaGrouping groupingType);

		[Since (3,0)]
		[Field ("MPMediaItemPropertyPersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString PersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyAlbumPersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AlbumPersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyArtistPersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString ArtistPersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyAlbumArtistPersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AlbumArtistPersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyGenrePersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString GenrePersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyComposerPersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString ComposerPersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyPodcastPersistentID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString PodcastPersistentIDProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyMediaType")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString MediaTypeProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyTitle")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString TitleProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyAlbumTitle")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AlbumTitleProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyArtist")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString ArtistProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyAlbumArtist")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AlbumArtistProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyGenre")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString GenreProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyComposer")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString ComposerProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyPlaybackDuration")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString PlaybackDurationProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyAlbumTrackNumber")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AlbumTrackNumberProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyAlbumTrackCount")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AlbumTrackCountProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyDiscNumber")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString DiscNumberProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyDiscCount")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString DiscCountProperty { get; }

		[Since (3,0), Mac (10,13,1, onlyOn64: true)]
		[Field ("MPMediaItemPropertyArtwork")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString ArtworkProperty { get; }

		[iOS (7,0)]
		[Field ("MPMediaItemPropertyIsExplicit")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString IsExplicitProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyLyrics")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString LyricsProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyIsCompilation")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString IsCompilationProperty { get; }

		[Since (4,0)]
		[Field ("MPMediaItemPropertyReleaseDate")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString ReleaseDateProperty { get; }

		[Since (4,0)]
		[Field ("MPMediaItemPropertyBeatsPerMinute")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString BeatsPerMinuteProperty { get; }

		[Since (4,0)]
		[Field ("MPMediaItemPropertyComments")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString CommentsProperty { get; }

		[Since (4,0)]
		[Field ("MPMediaItemPropertyAssetURL")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString AssetURLProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyPlayCount")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString PlayCountProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertySkipCount")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString SkipCountProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyRating")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString RatingProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyLastPlayedDate")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString LastPlayedDateProperty { get; }

		[Since (4,0)]
		[Field ("MPMediaItemPropertyUserGrouping")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString UserGroupingProperty { get; }

		[Since (3,0)]
		[Field ("MPMediaItemPropertyPodcastTitle")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString PodcastTitleProperty { get; }

		[Since (6,0)]
		[Field ("MPMediaItemPropertyBookmarkTime")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString BookmarkTimeProperty { get; }

		[Since (6,0)]
		[Field ("MPMediaItemPropertyIsCloudItem")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString IsCloudItemProperty { get; }

		[iOS (9,2)]
		[Field ("MPMediaItemPropertyHasProtectedAsset")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString HasProtectedAssetProperty { get; }

		[iOS (10, 0)]
		[Field ("MPMediaItemPropertyDateAdded")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString DateAddedProperty { get; }

		[iOS (10,3)]
		[Field ("MPMediaItemPropertyPlaybackStoreID")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString PlaybackStoreIDProperty { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPMediaItemArtwork {
#if !MONOMAC
		[iOS (10,0)]
		[TV (10,0)]
		[Export ("initWithBoundsSize:requestHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGSize boundsSize, Func<CGSize, UIImage> requestHandler);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Export ("initWithImage:")]
		IntPtr Constructor (UIImage image);
		
		[Export ("imageWithSize:")]
		[return: NullAllowed]
		UIImage ImageWithSize (CGSize size);
#else
		[Export ("initWithBoundsSize:requestHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGSize boundsSize, Func<CGSize, NSImage> requestHandler);

		[Export ("imageWithSize:")]
		[return: NullAllowed]
		NSImage ImageWithSize (CGSize size);
#endif

		[Export ("bounds")]
		CGRect Bounds { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Export ("imageCropRect")]
		CGRect ImageCropRectangle { get; }
	}

	[NoMac]
	[NoTV]
	// Objective-C exception thrown.  Name: MPMediaItemCollectionInitException Reason: -init is not supported, use -initWithItems:
	[DisableDefaultCtor]
#if XAMCORE_2_0 && IOS
	// introduced in 4.2 - but the type was never added to classic
	[BaseType (typeof (MPMediaEntity))]
#else
	[BaseType (typeof (NSObject))]
#endif
#if XAMCORE_3_0 || !XAMCORE_2_0 || !IOS
	interface MPMediaItemCollection : NSSecureCoding {
#else
	// part of the bug is that we inlined MPMediaEntity needlessly
	interface MPMediaItemCollection : MPMediaEntity, NSSecureCoding {
#endif
		[Static]
		[Export ("collectionWithItems:")]
		MPMediaItemCollection FromItems (MPMediaItem [] items);

		[DesignatedInitializer]
		[Export ("initWithItems:")]
		IntPtr Constructor (MPMediaItem [] items);

		[Export ("items")]
		MPMediaItem [] Items { get; }

		[Export ("representativeItem")]
		MPMediaItem RepresentativeItem { get; }

		[Export ("count")]
		nint Count { get; }

		[Export ("mediaTypes")]
		MPMediaType MediaTypes { get; }
	}

	[NoMac]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface MPMediaLibrary : NSSecureCoding {
		[Static, Export ("defaultMediaLibrary")]
		MPMediaLibrary DefaultMediaLibrary { get; }

		[Export ("lastModifiedDate")]
		NSDate LastModifiedDate { get; }

		[Export ("beginGeneratingLibraryChangeNotifications")]
		void BeginGeneratingLibraryChangeNotifications ();

		[Export ("endGeneratingLibraryChangeNotifications")]
		void EndGeneratingLibraryChangeNotifications ();

		[Field ("MPMediaLibraryDidChangeNotification")]
		[Notification]
		NSString DidChangeNotification { get; }

		[iOS (9,3)]
		[Static]
		[Export ("authorizationStatus")]
		MPMediaLibraryAuthorizationStatus AuthorizationStatus { get; }

		[iOS (9,3)]
		[Static]
		[Async]
		[Export ("requestAuthorization:")]
		void RequestAuthorization (Action<MPMediaLibraryAuthorizationStatus> handler);

		[iOS (9,3)]
		[Export ("addItemWithProductID:completionHandler:")]
		[Async]
#if XAMCORE_2_0 && IOS
		// MPMediaEntity was not part of classic
		void AddItem (string productID, [NullAllowed] Action<MPMediaEntity[], NSError> completionHandler);
#else
		void AddItem (string productID, [NullAllowed] Action<MPMediaItem[], NSError> completionHandler);
#endif

		[iOS (9,3)]
		[Async]
		[Export ("getPlaylistWithUUID:creationMetadata:completionHandler:")]
		void GetPlaylist (NSUuid uuid, [NullAllowed] MPMediaPlaylistCreationMetadata creationMetadata, Action<MPMediaPlaylist, NSError> completionHandler);
	}

#if !MONOMAC
	[NoTV]
	[BaseType (typeof (UIViewController), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(MPMediaPickerControllerDelegate)})]
	interface MPMediaPickerController {
		[DesignatedInitializer]
		[Export ("initWithMediaTypes:")]
		IntPtr Constructor (MPMediaType mediaTypes);

		[Export ("mediaTypes")]
		MPMediaType MediaTypes { get; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MPMediaPickerControllerDelegate Delegate { get; set; }

		[Export ("allowsPickingMultipleItems")]
		bool AllowsPickingMultipleItems { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("prompt", ArgumentSemantic.Copy)]
		string Prompt { get; set; }

		[Since (6,0)]
		[Export ("showsCloudItems")]
		bool ShowsCloudItems { get; set; }

		[Since (9,2)]
		[Export ("showsItemsWithProtectedAssets")]
		bool ShowsItemsWithProtectedAssets { get; set; }
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MPMediaPickerControllerDelegate {
		[Export ("mediaPicker:didPickMediaItems:"), EventArgs ("ItemsPicked"), EventName ("ItemsPicked")]
		void MediaItemsPicked (MPMediaPickerController sender, MPMediaItemCollection mediaItemCollection);
		
		[Export ("mediaPickerDidCancel:"), EventArgs ("MPMediaPickerController"), EventName ("DidCancel")]
		void MediaPickerDidCancel (MPMediaPickerController sender);
	}
#endif

	[NoMac]
	[NoTV]
	[BaseType (typeof (MPMediaItemCollection))]
	// Objective-C exception thrown.  Name: MPMediaItemCollectionInitException Reason: -init is not supported, use -initWithItems:
	[DisableDefaultCtor]
	interface MPMediaPlaylist : NSSecureCoding {
		[Export ("initWithItems:")]
		IntPtr Constructor (MPMediaItem [] items);

		[Static, Export ("canFilterByProperty:")]
		bool CanFilterByProperty (string property);

		[Export ("valueForProperty:")]
		NSObject ValueForProperty (string property);

		[iOS (7,0)]
		[Export ("persistentID")]
		ulong PersistentID { get; }

		[iOS (7,0)]
		[Export ("name")]
		string Name { get; }

		[iOS (7,0)]
		[Export ("playlistAttributes")]
		MPMediaPlaylistAttribute PlaylistAttributes { get; }

		[iOS (8,0)]
		[Export ("seedItems")]
		MPMediaItem [] SeedItems { get; }		

		[iOS (9,3)]
		[NullAllowed, Export ("descriptionText")]
		string DescriptionText { get; }

		[iOS (9,3)]
		[NullAllowed, Export ("authorDisplayName")]
		string AuthorDisplayName { get; }

		[iOS (9,3)]
		[Async]
		[Export ("addItemWithProductID:completionHandler:")]
		void AddItem (string productID, [NullAllowed] Action<NSError> completionHandler);

		[iOS (9,3)]
		[Async]
		[Export ("addMediaItems:completionHandler:")]
		void AddMediaItems (MPMediaItem[] mediaItems, [NullAllowed] Action<NSError> completionHandler);
	}

	[NoMac]
	[Static]
	interface MPMediaPlaylistProperty {
		[Field ("MPMediaPlaylistPropertyPersistentID")]
		NSString PersistentID { get; }

		[Field ("MPMediaPlaylistPropertyName")]
		NSString Name { get; }

		[Field ("MPMediaPlaylistPropertyPlaylistAttributes")]
		NSString PlaylistAttributes { get; }

		[Field ("MPMediaPlaylistPropertySeedItems")]
		NSString SeedItems { get; }

		[iOS (9, 3)]
		[NoTV] // do not work on AppleTV devices (only in simulator)
		[Field ("MPMediaPlaylistPropertyDescriptionText")]
		NSString DescriptionText { get; }

		[iOS (9, 3)]
		[NoTV] // do not work on AppleTV devices (only in simulator)
		[Field ("MPMediaPlaylistPropertyAuthorDisplayName")]
		NSString AuthorDisplayName { get; }
	}

	[NoMac]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface MPMediaQuery : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithFilterPredicates:")]
		IntPtr Constructor ([NullAllowed] NSSet filterPredicates);

		[NullAllowed] // by default this property is null
		[Export ("filterPredicates", ArgumentSemantic.Retain)]
		NSSet FilterPredicates { get; set; }

		[Export ("addFilterPredicate:")]
		void AddFilterPredicate (MPMediaPredicate predicate);

		[Export ("removeFilterPredicate:")]
		void RemoveFilterPredicate (MPMediaPredicate predicate);

		[Export ("items")]
		MPMediaItem [] Items { get; }

		[Export ("collections")]
		MPMediaItemCollection [] Collections { get; }

		[Export ("groupingType")]
		MPMediaGrouping GroupingType { get; set; }
#if XAMCORE_2_0
		[Export ("albumsQuery")][Static]
		MPMediaQuery AlbumsQuery { get; }

		[Export ("artistsQuery")][Static]
		MPMediaQuery ArtistsQuery { get; }

		[Export ("songsQuery")][Static]
		MPMediaQuery SongsQuery { get; }

		[Export ("playlistsQuery")][Static]
		MPMediaQuery PlaylistsQuery { get; }

		[Export ("podcastsQuery")][Static]
		MPMediaQuery PodcastsQuery { get; }

		[Export ("audiobooksQuery")][Static]
		MPMediaQuery AudiobooksQuery { get; }

		[Export ("compilationsQuery")][Static]
		MPMediaQuery CompilationsQuery { get; }

		[Export ("composersQuery")][Static]
		MPMediaQuery ComposersQuery { get; }

		[Export ("genresQuery")][Static]
		MPMediaQuery GenresQuery { get; }
#else
		[Export ("albumsQuery")][Static]
		MPMediaQuery albumsQuery { get; }
		
		[Export ("artistsQuery")][Static]
		MPMediaQuery artistsQuery { get; }
		
		[Export ("songsQuery")][Static]
		MPMediaQuery songsQuery { get; }
		
		[Export ("playlistsQuery")][Static]
		MPMediaQuery playlistsQuery { get; }
		
		[Export ("podcastsQuery")][Static]
		MPMediaQuery podcastsQuery { get; }
		
		[Export ("audiobooksQuery")][Static]
		MPMediaQuery audiobooksQuery { get; }
		
		[Export ("compilationsQuery")][Static]
		MPMediaQuery compilationsQuery { get; }
		
		[Export ("composersQuery")][Static]
		MPMediaQuery composersQuery { get; }
		
		[Export ("genresQuery")][Static]
		MPMediaQuery genresQuery { get; }
#endif
		[Since (4,2)]
		[Export ("collectionSections")]
		MPMediaQuerySection [] CollectionSections { get; }

		[Since (4,2)]
		[Export ("itemSections")]
		MPMediaQuerySection [] ItemSections { get; }
	}

	[NoMac]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface MPMediaPredicate : NSSecureCoding {
	}

	[NoMac]
	[NoTV]
	[BaseType (typeof (MPMediaPredicate))]
	interface MPMediaPropertyPredicate {
		[Static, Export ("predicateWithValue:forProperty:")]
		MPMediaPropertyPredicate PredicateWithValue ([NullAllowed] NSObject value, string property);

		[Static, Export ("predicateWithValue:forProperty:comparisonType:")]
		MPMediaPropertyPredicate PredicateWithValue ([NullAllowed] NSObject value, string property, MPMediaPredicateComparison comparisonType);

		[Export ("property", ArgumentSemantic.Copy)]
		string Property { get; }

		[Export ("value", ArgumentSemantic.Copy)]
		NSObject Value { get; }

		[Export ("comparisonType")]
		MPMediaPredicateComparison ComparisonType { get; }
	}

	[NoMac]
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[BaseType (typeof (NSObject))]
	interface MPMovieAccessLog : NSCopying {
		[Export ("events")]
		MPMovieAccessLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData")]
		NSData ExtendedLogData { get; }
	}

	[NoMac]
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[BaseType (typeof (NSObject))]
	interface MPMovieErrorLog : NSCopying {
		[Export ("events")]
		MPMovieErrorLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData")]
		NSData ExtendedLogData { get; }
	}

	[NoMac]
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[BaseType (typeof (NSObject))]
	interface MPMovieAccessLogEvent : NSCopying {
		[Export ("numberOfSegmentsDownloaded")]
		nint SegmentedDownloadedCount { get; }

		[Export ("playbackStartDate")]
		NSData PlaybackStartDate { get; }

		[Export ("URI")]
		string Uri { get; }

		[Export ("serverAddress")]
		string ServerAddress { get; }

		[Export ("numberOfServerAddressChanges")]
		nint ServerAddressChangeCount { get; }

		[Export ("playbackSessionID")]
		string PlaybackSessionID { get; }

		[Export ("playbackStartOffset")]
		double PlaybackStartOffset { get; }

		[Export ("segmentsDownloadedDuration")]
		double SegmentsDownloadedDuration { get; }

		[Export ("durationWatched")]
		double DurationWatched { get; }

		[Export ("numberOfStalls")]
		nint StallCount { get; }

		[Export ("numberOfBytesTransferred")]
		long BytesTransferred { get; }

		[Export ("observedBitrate")]
		double ObservedBitrate { get; }

		[Export ("indicatedBitrate")]
		double IndicatedBitrate { get; }

		[Export ("numberOfDroppedVideoFrames")]
		nint DroppedVideoFrameCount { get; }
	}

	[NoMac]
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0)]
	[BaseType (typeof (NSObject))]
	interface MPMovieErrorLogEvent : NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[Export ("URI")]
		string Uri { get; }

		[Export ("serverAddress")]
		string ServerAddress { get; }

		[Export ("playbackSessionID")]
		string PlaybackSessionID { get; }

		[Export ("errorStatusCode")]
		nint ErrorStatusCode { get; }

		[Export ("errorDomain")]
		string ErrorDomain { get; }

		[Export ("errorComment")]
		string ErrorComment { get; }
	}

	[NoMac]
	[Availability (Deprecated = Platform.iOS_9_0)]
	interface MPMoviePlayerFinishedEventArgs {
		[Export ("MPMoviePlayerPlaybackDidFinishReasonUserInfoKey")]
		MPMovieFinishReason FinishReason { get; }
	}

#if !MONOMAC
	[Availability (Deprecated = Platform.iOS_9_0)]
	interface MPMoviePlayerFullScreenEventArgs {
		[Export ("MPMoviePlayerFullscreenAnimationDurationUserInfoKey")]
		double AnimationDuration { get; }

		[Export ("MPMoviePlayerFullscreenAnimationCurveUserInfoKey")]
		UIViewAnimationCurve AnimationCurve { get; }
	}

	[Availability (Deprecated = Platform.iOS_9_0)]
	interface MPMoviePlayerThumbnailEventArgs {
		[Export ("MPMoviePlayerThumbnailImageKey")]
		UIImage Image { get; }

		[Export ("MPMoviePlayerThumbnailTimeKey")]
		double Time { get; }

		[Export ("MPMoviePlayerThumbnailErrorKey")]
		NSError Error { get; }
	}
#endif

	[NoMac]
	[Availability (Deprecated = Platform.iOS_9_0)]
	interface MPMoviePlayerTimedMetadataEventArgs {
		[Export ("MPMoviePlayerTimedMetadataUserInfoKey")]
		MPTimedMetadata [] TimedMetadata { get; }
	}

	// no [Model] yet... it can be easily created in user code (all abstract) if needed
	[NoMac]
	[Protocol]
	interface MPMediaPlayback {
		[Abstract]
		[Export ("play")]
		void Play ();

		[Abstract]
		[Export ("stop")]
		void Stop ();

		[Abstract]
		[Since (3,2)]
		[Export ("pause")]
		void Pause ();
		
		[Abstract]
		[Since (3,2)]
		[Export ("prepareToPlay")]
		void PrepareToPlay ();

		[Abstract]
		[Since (3,2)]
		[Export ("isPreparedToPlay")]
		bool IsPreparedToPlay { get; }

		[Abstract]
		[Since (3,2)]
		[Export ("currentPlaybackTime")]
		double CurrentPlaybackTime { get; set; }

		[Abstract]
		[Since (3,2)]
		[Export ("currentPlaybackRate")]
		float CurrentPlaybackRate { get; set; } // float, not CGFloat

		[Abstract]
		[Since (3,2)]
		[Export ("beginSeekingForward")]
		void BeginSeekingForward ();

		[Abstract]
		[Since (3,2)]
		[Export ("beginSeekingBackward")]
		void BeginSeekingBackward ();

		[Abstract]
		[Since (3,2)]
		[Export ("endSeeking")]
		void EndSeeking ();
	}

#if !MONOMAC
	[NoTV]
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
	[BaseType (typeof (NSObject))]
	interface MPMoviePlayerController : MPMediaPlayback {
		[DesignatedInitializer]
		[Export ("initWithContentURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		// <quote>You should avoid using this property. It is available only when you use the initWithContentURL: method to initialize the movie player controller object.</quote>
		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_3_2, Obsoleted = Platform.iOS_8_0, Message = "Do not use; this API was removed and is not always available.")]
		UIColor BackgroundColor { get; set; }

		[Export ("scalingMode")]
		MPMovieScalingMode ScalingMode { get; set; }

		[Export ("movieControlMode")]
		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_3_2, Obsoleted = Platform.iOS_8_0, Message = "Do not use; this API was removed.")]
		MPMovieControlMode MovieControlMode { get; set; }

		[Since (3,2)]
		[Export ("initialPlaybackTime")]
		double InitialPlaybackTime { get; set; }

		[Since (3,2)]
		[NullAllowed] // by default this property is null
		[Export ("contentURL", ArgumentSemantic.Copy)]
		NSUrl ContentUrl { get; set; }

		[Since (3,2)]
		[Export ("view")]
		UIView View { get; }

		[Since (3,2)]
		[Export ("backgroundView")]
		UIView BackgroundView { get; }
		
		[Since (3,2)]
		[Export ("playbackState")]
		MPMoviePlaybackState PlaybackState { get; }

		[Since (3,2)]
		[Export ("loadState")]
		MPMovieLoadState LoadState { get; }

		[Since (3,2)]
		[Export ("controlStyle")]
		MPMovieControlStyle ControlStyle { get; set; }

		[Since (3,2)]
		[Export ("repeatMode")]
		MPMovieRepeatMode RepeatMode { get; set; }

		[Since (3,2)]
		[Export ("shouldAutoplay")]
		bool ShouldAutoplay { get; set; }

		[Export ("useApplicationAudioSession")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		bool UseApplicationAudioSession { get; set; }

		[Since (3,2)]
		[Export ("fullscreen")]
		bool Fullscreen { [Bind ("isFullscreen")] get; set; }

		[Since (3,2)]
		[Export ("setFullscreen:animated:")]
		void SetFullscreen (bool fullscreen, bool animated);

		[Since (4,3)]
		[Export ("allowsAirPlay")]
		bool AllowsAirPlay { get; set; }

		[iOS (5,0)]
		[Export ("airPlayVideoActive")]
		bool AirPlayVideoActive { [Bind ("isAirPlayVideoActive")] get; }

		[Availability (Deprecated = Platform.iOS_9_0)]
		[Since (4,3)]
		[Export ("accessLog")]
		MPMovieAccessLog AccessLog { get; }

		[Availability (Deprecated = Platform.iOS_9_0)]
		[Since (4,3)]
		[Export ("errorLog")]
		MPMovieErrorLog ErrorLog { get; }

		// Brought it from the MPMediaPlayback.h

		[Export ("thumbnailImageAtTime:timeOption:")]
		[Availability (Introduced = Platform.iOS_3_2, Deprecated = Platform.iOS_7_0, Message = "Use 'RequestThumbnails' instead.")]
		UIImage ThumbnailImageAt (double time, MPMovieTimeOption timeOption);

		[Since (3,2)]
		[Export ("requestThumbnailImagesAtTimes:timeOption:")]
		void RequestThumbnails (NSNumber [] doubleNumbers, MPMovieTimeOption timeOption);

		[Since (3,2)]
		[Export ("cancelAllThumbnailImageRequests")]
		void CancelAllThumbnailImageRequests ();

		//
		// From interface MPMovieProperties
		//
		[Since (3,2)]
		[Export ("movieMediaTypes")]
		MPMovieMediaType MovieMediaTypes { get; }

		[Since (3,2)]
		[Export ("movieSourceType")]
		MPMovieSourceType SourceType { get; set; }

		[Since (3,2)]
		[Export ("duration")]
		double Duration { get; }

		[Since (3,2)]
		[Export ("playableDuration")]
		double PlayableDuration { get; }

		[Since (3,2)]
		[Export ("naturalSize")]
		CGSize NaturalSize { get; }

		[Since (3,2)]
		[Export ("endPlaybackTime")]
		double EndPlaybackTime { get; set; }

		[Since (4,0)]
		[Export ("timedMetadata")]
		MPTimedMetadata [] TimedMetadata { get; }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Field ("MPMoviePlayerScalingModeDidChangeNotification")]
		[Notification]
		NSString ScalingModeDidChangeNotification { get; }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Field ("MPMoviePlayerPlaybackDidFinishNotification")]
		[Notification (typeof (MPMoviePlayerFinishedEventArgs))]
		NSString PlaybackDidFinishNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerPlaybackDidFinishReasonUserInfoKey")] // NSNumber (MPMovieFinishReason)
		NSString PlaybackDidFinishReasonUserInfoKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerPlaybackStateDidChangeNotification")]
		[Notification]
		NSString PlaybackStateDidChangeNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerLoadStateDidChangeNotification")]
		[Notification]
		NSString LoadStateDidChangeNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerNowPlayingMovieDidChangeNotification")]
		[Notification]
		NSString NowPlayingMovieDidChangeNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerWillEnterFullscreenNotification")]
		[Notification (typeof (MPMoviePlayerFullScreenEventArgs))]
		[Notification]
		NSString WillEnterFullscreenNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerDidEnterFullscreenNotification")]
		[Notification]
		NSString DidEnterFullscreenNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerWillExitFullscreenNotification")]
		[Notification (typeof (MPMoviePlayerFullScreenEventArgs))]
		NSString WillExitFullscreenNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerDidExitFullscreenNotification")]
		[Notification]
		NSString DidExitFullscreenNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerFullscreenAnimationDurationUserInfoKey")]
		NSString FullscreenAnimationDurationUserInfoKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerFullscreenAnimationCurveUserInfoKey")]
		NSString FullscreenAnimationCurveUserInfoKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMovieMediaTypesAvailableNotification")]
		[Notification]
		NSString TypesAvailableNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMovieSourceTypeAvailableNotification")]
		[Notification]
		NSString SourceTypeAvailableNotification { get; }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMovieDurationAvailableNotification")]
		[Notification]
		NSString DurationAvailableNotification { get;  }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMovieNaturalSizeAvailableNotification")]
		[Notification]
		NSString NaturalSizeAvailableNotification { get;  }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerThumbnailImageRequestDidFinishNotification")]
		[Notification (typeof (MPMoviePlayerThumbnailEventArgs))]
		NSString ThumbnailImageRequestDidFinishNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerThumbnailImageKey")]
		NSString ThumbnailImageKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerThumbnailTimeKey")]
		NSString ThumbnailTimeKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (3,2)]
		[Field ("MPMoviePlayerThumbnailErrorKey")]
		NSString ThumbnailErrorKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataUpdatedNotification")]
		[Notification (typeof (MPMoviePlayerTimedMetadataEventArgs))]
		NSString TimedMetadataUpdatedNotification { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataUserInfoKey")]
		NSString TimedMetadataUserInfoKey { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataKeyName")]
		NSString TimedMetadataKeyName { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataKeyInfo")]
		NSString TimedMetadataKeyInfo { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataKeyMIMEType")]
		NSString TimedMetadataKeyMIMEType { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataKeyDataType")]
		NSString TimedMetadataKeyDataType { get; }
		
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (4,0)]
		[Field ("MPMoviePlayerTimedMetadataKeyLanguageCode")]
		NSString TimedMetadataKeyLanguageCode { get; }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Field ("MPMediaPlaybackIsPreparedToPlayDidChangeNotification")]
		[Notification]
		NSString MediaPlaybackIsPreparedToPlayDidChangeNotification { get; }

		[Since(6,0)]
		[Export ("readyForDisplay")]
		bool ReadyForDisplay { get;  }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Since (6,0)]
		[Field ("MPMoviePlayerReadyForDisplayDidChangeNotification")]
		[Notification]
		NSString MoviePlayerReadyForDisplayDidChangeNotification { get; }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[iOS (5,0)]
		[Field ("MPMoviePlayerIsAirPlayVideoActiveDidChangeNotification")]
		[Notification]
		NSString MPMoviePlayerIsAirPlayVideoActiveDidChangeNotification { get; }
	}
#endif

	[NoMac]
	[NoTV]
	[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_9_0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: MPTimedMetadata cannot be created directly
	[DisableDefaultCtor]
	interface MPTimedMetadata {
		[Export ("key")]
		string Key { get;  }

		[Export ("keyspace")]
		string Keyspace { get;  }

		[Export ("value")]
#if XAMCORE_3_0
		NSObject Value { get;  }
#else
		NSObject value { get;  }
#endif

		[Export ("timestamp")]
		double Timestamp { get;  }

		[Export ("allMetadata")]
		NSDictionary AllMetadata { get;  }
	}

#if !MONOMAC
	[NoTV]
	[BaseType (typeof (UIViewController))]
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
	interface MPMoviePlayerViewController {
		[DesignatedInitializer]
		[Export ("initWithContentURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("moviePlayer")]
		MPMoviePlayerController MoviePlayer { get; }

		// Directly removed, shows up in iOS 6.1 SDK, but not any later SDKs.
		[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_7_0, Message = "Do not use; this API was removed.")]
		[Export ("shouldAutorotateToInterfaceOrientation:")]
		bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation orientation);
	}
#endif

	[NoMac]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPMusicPlayerController : MPMediaPlayback {

		[Export ("init")]
		[Deprecated (PlatformName.iOS, 11,3)]
		IntPtr Constructor ();

		[Static, Export ("applicationMusicPlayer")]
		MPMusicPlayerController ApplicationMusicPlayer { get; }

		[iOS (10,3)]
		[Static]
		[Export ("applicationQueuePlayer")]
		MPMusicPlayerApplicationController ApplicationQueuePlayer { get; }

		[Static, Export ("iPodMusicPlayer")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'SystemMusicPlayer' instead.")]
		MPMusicPlayerController iPodMusicPlayer { get; }

		[iOS (8,0)]
		[Static, Export ("systemMusicPlayer")]
		MPMusicPlayerController SystemMusicPlayer { get; }

		[Export ("playbackState")]
		MPMusicPlaybackState PlaybackState { get; }

		[Export ("repeatMode")]
		MPMusicRepeatMode RepeatMode { get; set; }

		[Export ("shuffleMode")]
		MPMusicShuffleMode ShuffleMode { get; set; }

		[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_7_0, Message = "Use 'MPVolumeView' for volume control instead.")]
		[Export ("volume")]
		float Volume { get; set; } // nfloat, not CGFloat

		[iOS (5,0)]
		[Export ("indexOfNowPlayingItem")]
		nuint IndexOfNowPlayingItem { get; }

		[Export ("nowPlayingItem", ArgumentSemantic.Copy), NullAllowed]
		MPMediaItem NowPlayingItem { get; set; }

		[Export ("setQueueWithQuery:")]
		void SetQueue (MPMediaQuery query);

		[Export ("setQueueWithItemCollection:")]
		void SetQueue (MPMediaItemCollection collection);

		[iOS (9,3)]
		[Export ("setQueueWithStoreIDs:")]
		void SetQueue (string[] storeIDs);

		[iOS (10,1)]
		[Export ("setQueueWithDescriptor:")]
		void SetQueue (MPMusicPlayerQueueDescriptor descriptor);

		[iOS (10,3)]
		[Export ("prependQueueDescriptor:")]
		void Prepend (MPMusicPlayerQueueDescriptor descriptor);

		[iOS (10,3)]
		[Export ("appendQueueDescriptor:")]
		void Append (MPMusicPlayerQueueDescriptor descriptor);

		[iOS (10,1)]
		[Async]
		[Export ("prepareToPlayWithCompletionHandler:")]
		void PrepareToPlay (Action<NSError> completionHandler);

		[Export ("skipToNextItem")]
		void SkipToNextItem ();

		[Export ("skipToBeginning")]
		void SkipToBeginning ();

		[Export ("skipToPreviousItem")]
		void SkipToPreviousItem ();

		[Export ("beginGeneratingPlaybackNotifications")]
		void BeginGeneratingPlaybackNotifications ();

		[Export ("endGeneratingPlaybackNotifications")]
		void EndGeneratingPlaybackNotifications ();

		[Field ("MPMusicPlayerControllerPlaybackStateDidChangeNotification")]
		[Notification]
		NSString PlaybackStateDidChangeNotification { get; }

		[Field ("MPMusicPlayerControllerNowPlayingItemDidChangeNotification")]
		[Notification]
		NSString NowPlayingItemDidChangeNotification { get; }

		[Field ("MPMusicPlayerControllerVolumeDidChangeNotification")]
		[Notification]
		NSString VolumeDidChangeNotification { get; }
	}

#if !MONOMAC
	[NoTV]
	[BaseType (typeof (UIView))]
	interface MPVolumeView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("showsRouteButton")]
		bool ShowsRouteButton { get; set; }

		[Export ("showsVolumeSlider")]
		bool ShowsVolumeSlider { get; set; }

		[Since (6,0)]
		[Export ("setMinimumVolumeSliderImage:forState:")]
		void SetMinimumVolumeSliderImage ([NullAllowed] UIImage image, UIControlState state);

		[Since (6,0)]
		[Export ("setMaximumVolumeSliderImage:forState:")]
		void SetMaximumVolumeSliderImage ([NullAllowed] UIImage image, UIControlState state);

		[Since (6,0)]
		[Export ("setVolumeThumbImage:forState:")]
		void SetVolumeThumbImage ([NullAllowed] UIImage image, UIControlState state);

		[Since (6,0)]
		[Export ("minimumVolumeSliderImageForState:")]
		UIImage GetMinimumVolumeSliderImage (UIControlState state);

		[Since (6,0)]
		[Export ("maximumVolumeSliderImageForState:")]
		UIImage GetMaximumVolumeSliderImage (UIControlState state);

		[Since (6,0)]
		[Export ("volumeThumbImageForState:")]
		UIImage GetVolumeThumbImage (UIControlState state);

		[Since (6,0)]
		[Export ("volumeSliderRectForBounds:")]
		CGRect GetVolumeSliderRect (CGRect bounds);

		[Since (6,0)]
		[Export ("volumeThumbRectForBounds:volumeSliderRect:value:")]
		CGRect GetVolumeThumbRect (CGRect bounds, CGRect columeSliderRect, float /* float, not CGFloat */ value);

		[Since (6,0)]
		[Export ("setRouteButtonImage:forState:")]
		void SetRouteButtonImage ([NullAllowed] UIImage image, UIControlState state);

		[Since (6,0)]
		[Export ("routeButtonImageForState:")]
		UIImage GetRouteButtonImage (UIControlState state);

		[Since (6,0)]
		[Export ("routeButtonRectForBounds:")]
		CGRect GetRouteButtonRect (CGRect bounds);

		[Since (7,0)]
		[Export ("wirelessRoutesAvailable")]
		bool AreWirelessRoutesAvailable { [Bind ("areWirelessRoutesAvailable")] get; }

		[Since (7,0)]
		[Export ("wirelessRouteActive")]
		bool IsWirelessRouteActive { [Bind ("isWirelessRouteActive")] get; }

		[Since (7,0)]
		[NullAllowed] // by default this property is null
		[Export ("volumeWarningSliderImage", ArgumentSemantic.Retain)]
		UIImage VolumeWarningSliderImage { get; set; }

		[Since (7,0)]
		[Notification]
		[Field ("MPVolumeViewWirelessRoutesAvailableDidChangeNotification")]
		NSString WirelessRoutesAvailableDidChangeNotification { get; }

		[Since (7,0)]
		[Notification]
		[Field ("MPVolumeViewWirelessRouteActiveDidChangeNotification")]
		NSString WirelessRouteActiveDidChangeNotification { get; }
	}	
#endif

	[NoMac]
	[NoTV]
	[Since (4,2)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: MPMediaQuerySection is a read-only object
	[DisableDefaultCtor]
	interface MPMediaQuerySection : NSSecureCoding, NSCopying {
		[Export ("range", ArgumentSemantic.Assign)]	
		NSRange Range { get; }

		[Export ("title", ArgumentSemantic.Copy)]	
		string Title { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -init is not supported, use +defaultCenter
	[DisableDefaultCtor]
	interface MPNowPlayingInfoCenter {
		[Export ("nowPlayingInfo", ArgumentSemantic.Copy), NullAllowed, Internal]
		NSDictionary _NowPlayingInfo { get; set;  }

		[Static]
		[Export ("defaultCenter")]
		MPNowPlayingInfoCenter DefaultCenter { get; }

		[NoiOS]
		[NoTV]
		[Export ("playbackState")]
		MPNowPlayingPlaybackState PlaybackState { get; set; }

		[Internal]
 		[Field ("MPNowPlayingInfoPropertyElapsedPlaybackTime")]
		NSString PropertyElapsedPlaybackTime { get; }
			
		[Internal]
		[Field ("MPNowPlayingInfoPropertyPlaybackRate")]
		NSString PropertyPlaybackRate { get; }
		
		[Internal]
		[Field ("MPNowPlayingInfoPropertyPlaybackQueueIndex")]
		NSString PropertyPlaybackQueueIndex { get; }
		
		[Internal]
		[Field ("MPNowPlayingInfoPropertyPlaybackQueueCount")]
		NSString PropertyPlaybackQueueCount { get; }
		
		[Internal]
		[Field ("MPNowPlayingInfoPropertyChapterNumber")]
		NSString PropertyChapterNumber { get; }
		
		[Internal]
		[Field ("MPNowPlayingInfoPropertyChapterCount")]
		NSString PropertyChapterCount { get; }

		[iOS (8,0)]
		[Internal]
		[Field ("MPNowPlayingInfoPropertyDefaultPlaybackRate")]
		NSString PropertyDefaultPlaybackRate { get; }

		[iOS (9,0)]
		[TV (10,0)]
		[Internal]
		[Field ("MPNowPlayingInfoPropertyAvailableLanguageOptions")]
		NSString PropertyAvailableLanguageOptions { get; }

		[iOS (9,0)]
		[Internal]
		[Field ("MPNowPlayingInfoPropertyCurrentLanguageOptions")]
		NSString PropertyCurrentLanguageOptions { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Field ("MPNowPlayingInfoCollectionIdentifier")]
		NSString PropertyCollectionIdentifier { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Field ("MPNowPlayingInfoPropertyExternalContentIdentifier")]
		NSString PropertyExternalContentIdentifier { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Field ("MPNowPlayingInfoPropertyExternalUserProfileIdentifier")]
		NSString PropertyExternalUserProfileIdentifier { get; }

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13, onlyOn64: true)]
		[Field ("MPNowPlayingInfoPropertyServiceIdentifier")]
		NSString PropertyServiceIdentifier { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Field ("MPNowPlayingInfoPropertyPlaybackProgress")]
		NSString PropertyPlaybackProgress { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Field ("MPNowPlayingInfoPropertyMediaType")]
		NSString PropertyMediaType { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Field ("MPNowPlayingInfoPropertyIsLiveStream")]
		NSString PropertyIsLiveStream { get; }

		[iOS (10,3)]
		[TV (10,2)]
		[Mac (10,12,3, onlyOn64: true)]
		[Field ("MPNowPlayingInfoPropertyAssetURL")]
		NSString PropertyAssetUrl { get; }

		[iOS (11,1), TV (11,1), Mac (10,13,1, onlyOn64: true)]
		[Internal]
		[Field ("MPNowPlayingInfoPropertyCurrentPlaybackDate")]
		NSString PropertyCurrentPlaybackDate { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash if used
	interface MPContentItem {

		[DesignatedInitializer]
		[Export ("initWithIdentifier:")]
		IntPtr Constructor (string identifier);

		[Export ("artwork")]
		MPMediaItemArtwork Artwork { get; set; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("playbackProgress")]
		float PlaybackProgress { get; set; } // float, not CGFloat

		[Export ("subtitle")]
		string Subtitle { get; set; }

		[Export ("title")]
		string Title { get; set; }

		[iOS (10,0)]
		[TV (10,0)]
		[Export ("streamingContent")]
		bool StreamingContent { [Bind ("isStreamingContent")] get; set; }

		[iOS (10,0)]
		[TV (10,0)]
		[Export ("explicitContent")]
		bool ExplicitContent { [Bind ("isExplicitContent")] get; set; }

		[Export ("container")]
		bool Container { [Bind ("isContainer")] get; set; }

		[Export ("playable")]
		bool Playable { [Bind ("isPlayable")] get; set; }
	}

	[NoMac]
	[NoTV]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MPPlayableContentDataSource {

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("contentItemAtIndexPath:")]
#if XAMCORE_4_0
		MPContentItem GetContentItem (NSIndexPath indexPath);
#else
		MPContentItem ContentItem (NSIndexPath indexPath);
#endif

		[Export ("beginLoadingChildItemsAtIndexPath:completionHandler:")]
		void BeginLoadingChildItems (NSIndexPath indexPath, Action<NSError> completionHandler);

		[Export ("childItemsDisplayPlaybackProgressAtIndexPath:")]
		bool ChildItemsDisplayPlaybackProgress (NSIndexPath indexPath);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("numberOfChildItemsAtIndexPath:")]
		nint NumberOfChildItems (NSIndexPath indexPath);

		[NoMac]
		[iOS (10,0)]
		[Async]
		[Export ("contentItemForIdentifier:completionHandler:")]
		void GetContentItem (string identifier, Action<MPContentItem, NSError> completionHandler);
	}

	interface IMPPlayableContentDataSource {
	}

	[NoMac]
	[NoTV]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MPPlayableContentDelegate {
		[Export ("playableContentManager:initiatePlaybackOfContentItemAtIndexPath:completionHandler:")]
		void InitiatePlaybackOfContentItem (MPPlayableContentManager contentManager, NSIndexPath indexPath, Action<NSError> completionHandler);

		[iOS (8,4)]
		[Export ("playableContentManager:didUpdateContext:")]
		void ContextUpdated (MPPlayableContentManager contentManager, MPPlayableContentManagerContext context);

		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 9, 3, message: "Use 'InitializePlaybackQueue (MPPlayableContentManager, MPContentItem[], Action<NSError>)' instead.")]
		[Export ("playableContentManager:initializePlaybackQueueWithCompletionHandler:")]
		void InitializePlaybackQueue (MPPlayableContentManager contentManager, Action<NSError> completionHandler);

		[iOS (9,3)]
		[Export ("playableContentManager:initializePlaybackQueueWithContentItems:completionHandler:")]
		void InitializePlaybackQueue (MPPlayableContentManager contentManager, [NullAllowed] MPContentItem[] contentItems, Action<NSError> completionHandler);
	}

	[NoMac]
	[NoTV]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -init is invalid. Use +sharedManager. <- [sic]
	interface MPPlayableContentManager {

		[Static]
		[Export ("sharedContentManager")]
		MPPlayableContentManager Shared { get; }

		[Export ("dataSource", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		MPPlayableContentDataSource DataSource { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MPPlayableContentDelegate Delegate { get; set; }

		[Export ("beginUpdates")]
		void BeginUpdates ();

		[Export ("endUpdates")]
		void EndUpdates ();

		[Export ("reloadData")]
		void ReloadData ();

		[iOS (8,4)]
		[Export ("context")]
		MPPlayableContentManagerContext Context { get; }

		[iOS (10,0)]
		[Export ("nowPlayingIdentifiers", ArgumentSemantic.Strong)]
		string[] NowPlayingIdentifiers { get; set; }
	}

	[NoMac]
	[NoTV]
	[iOS (8,4)]
	[BaseType (typeof(NSObject))]
	interface MPPlayableContentManagerContext {
		[Export ("enforcedContentItemsCount")]
		nint EnforcedContentItemsCount { get; }

		[Export ("enforcedContentTreeDepth")]
		nint EnforcedContentTreeDepth { get; }

		// iOS 9 beta 2 changed this from contentLimitsEnabled - but the final iOS8.4 release used contentLimitsEnabled
		[iOS (9,0)]
		[Export ("contentLimitsEnforced")]
		bool ContentLimitsEnforced { get; }

		[Availability (Introduced = Platform.iOS_8_4, Deprecated = Platform.iOS_9_0, Message = "Replaced by 'ContentLimitsEnforced'.")]
		[Export ("contentLimitsEnabled")]
		bool ContentLimitsEnabled { get; }

		[Export ("endpointAvailable")]
		bool EndpointAvailable { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Reason: MPRemoteCommands cannot be initialized externally.
	interface MPRemoteCommand {

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("addTarget:action:")]
		void AddTarget (NSObject target, Selector action);

		[Export ("addTargetWithHandler:")]
		NSObject AddTarget (Func<MPRemoteCommandEvent,MPRemoteCommandHandlerStatus> handler);

		[Export ("removeTarget:")]
		void RemoveTarget (NSObject target);

		[Export ("removeTarget:action:")]
		void RemoveTarget ([NullAllowed] NSObject target, Selector action);
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommand))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangePlaybackRateCommands cannot be initialized externally.
	interface MPChangePlaybackRateCommand {

		[Export ("supportedPlaybackRates")]
		NSNumber[] SupportedPlaybackRates { get; set; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (8,0)]
	[BaseType (typeof(MPRemoteCommand))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangeShuffleModeCommand cannot be initialized externally.
	interface MPChangeShuffleModeCommand
	{
		[Export ("currentShuffleType", ArgumentSemantic.Assign)]
		MPShuffleType CurrentShuffleType { get; set; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (8,0)]
	[BaseType (typeof(MPRemoteCommand))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangeRepeatModeCommand cannot be initialized externally.
	interface MPChangeRepeatModeCommand
	{
		[Export ("currentRepeatType", ArgumentSemantic.Assign)]
		MPRepeatType CurrentRepeatType { get; set; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommand))]
	[DisableDefaultCtor] // NSGenericException Reason: MPFeedbackCommands cannot be initialized externally.
	interface MPFeedbackCommand {

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("localizedTitle")]
		string LocalizedTitle { get; set; }

		[iOS (8,2)] // added in 8.2, shown as NS_AVAILABLE_IOS(8_0)
		[Export ("localizedShortTitle")]
		string LocalizedShortTitle { get; set; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommand))]
	[DisableDefaultCtor] // NSGenericException Reason: MPRatingCommands cannot be initialized externally.
	interface MPRatingCommand {

		[Export ("maximumRating")]
		float MaximumRating { get; set; } /* float, not CGFloat */

		[Export ("minimumRating")]
		float MinimumRating { get; set; } /* float, not CGFloat */
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommand))]
	[DisableDefaultCtor] // NSGenericException Reason: MPSkipIntervalCommands cannot be initialized externally.
	interface MPSkipIntervalCommand {

#if XAMCORE_2_0
		[Internal] // -> we can't do double[] for an NSArray of NSTimeInterval
#endif
		[Export ("preferredIntervals")]
		NSArray _PreferredIntervals { get; set; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPRemoteCommandCenter {
		[Mac (10,12,2, onlyOn64: true)]
		[Static]
		[Export ("sharedCommandCenter")]
		MPRemoteCommandCenter Shared { get; }

		[Export ("bookmarkCommand")]
		MPFeedbackCommand BookmarkCommand { get; }

		[Export ("changePlaybackRateCommand")]
		MPChangePlaybackRateCommand ChangePlaybackRateCommand { get; }

		[iOS (8,0)]
		[Export ("changeRepeatModeCommand")]
		MPChangeRepeatModeCommand ChangeRepeatModeCommand { get; }

		[iOS (8,0)]
		[Export ("changeShuffleModeCommand")]
		MPChangeShuffleModeCommand ChangeShuffleModeCommand { get; }

		[Export ("dislikeCommand")]
		MPFeedbackCommand DislikeCommand { get; }

		[Export ("likeCommand")]
		MPFeedbackCommand LikeCommand { get; }

		[Export ("nextTrackCommand")]
		MPRemoteCommand NextTrackCommand { get; }

		[Export ("pauseCommand")]
		MPRemoteCommand PauseCommand { get; }

		[Export ("playCommand")]
		MPRemoteCommand PlayCommand { get; }

		[Export ("previousTrackCommand")]
		MPRemoteCommand PreviousTrackCommand { get; }

		[Export ("ratingCommand")]
		MPRatingCommand RatingCommand { get; }

		[Export ("seekBackwardCommand")]
		MPRemoteCommand SeekBackwardCommand { get; }

		[Export ("seekForwardCommand")]
		MPRemoteCommand SeekForwardCommand { get; }

		[Export ("skipBackwardCommand")]
		MPSkipIntervalCommand SkipBackwardCommand { get; }

		[Export ("skipForwardCommand")]
		MPSkipIntervalCommand SkipForwardCommand { get; }

		[Export ("stopCommand")]
		MPRemoteCommand StopCommand { get; }

		[Export ("togglePlayPauseCommand")]
		MPRemoteCommand TogglePlayPauseCommand { get; }

		[iOS (9,0)]
		[Export ("enableLanguageOptionCommand")]
		MPRemoteCommand EnableLanguageOptionCommand { get; }

		[iOS (9,0)]
		[Export ("disableLanguageOptionCommand")]
		MPRemoteCommand DisableLanguageOptionCommand { get; }

		[iOS (9,1)]
		[Export ("changePlaybackPositionCommand")]
		MPChangePlaybackPositionCommand ChangePlaybackPositionCommand { get; }
	}
	
	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Reason: MPRemoteCommandEvents cannot be initialized externally.
	interface MPRemoteCommandEvent {

		[Export ("command")]
		MPRemoteCommand Command { get; }

		[Export ("timestamp")]
		double /* NSTimeInterval */ Timestamp { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangePlaybackRateCommandEvents cannot be initialized externally.
	interface MPChangePlaybackRateCommandEvent {

		[Export ("playbackRate")]
		float PlaybackRate { get; } // float, not CGFloat
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor] // NSGenericException Reason: MPRatingCommandEvents cannot be initialized externally.
	interface MPRatingCommandEvent {

		[Export ("rating")]
		float Rating { get; } // float, not CGFloat
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor] // Name: NSGenericException Reason: MPSeekCommandEvents cannot be initialized externally.
	interface MPSeekCommandEvent {

		[Export ("type")]
		MPSeekCommandEventType Type { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor] // NSGenericException Reason: MPSkipIntervalCommandEvents cannot be initialized externally.
	interface MPSkipIntervalCommandEvent {

		[Export ("interval")]
		double /* NSTimeInterval */ Interval { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[Since (7,1)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor]
	interface MPFeedbackCommandEvent {

		[Export ("negative")]
		bool Negative { [Bind ("isNegative")] get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (9,0)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangeLanguageOptionCommandEvents cannot be initialized externally.
	interface MPChangeLanguageOptionCommandEvent {
		[Export ("languageOption")]
		MPNowPlayingInfoLanguageOption LanguageOption { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Export ("setting")]
		MPChangeLanguageOptionSetting Setting { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (8,0)]
	[BaseType (typeof(MPRemoteCommandEvent))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangeShuffleModeCommandEvent cannot be initialized externally.
	interface MPChangeShuffleModeCommandEvent
	{
		[Export ("shuffleType")]
		MPShuffleType ShuffleType { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Export ("preservesShuffleMode")]
		bool PreservesShuffleMode { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (8,0)]
	[BaseType (typeof(MPRemoteCommandEvent))]
	[DisableDefaultCtor] // NSGenericException Reason: MPChangeRepeatModeCommandEvent cannot be initialized externally.
	interface MPChangeRepeatModeCommandEvent
	{
		[Export ("repeatType")]
		MPRepeatType RepeatType { get; }

		[iOS (10,0)]
		[TV (10,0)]
		[Export ("preservesRepeatMode")]
		bool PreservesRepeatMode { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // pre-emptive
	interface MPNowPlayingInfoLanguageOption {
		[Export ("initWithType:languageTag:characteristics:displayName:identifier:")]
		IntPtr Constructor (MPNowPlayingInfoLanguageOptionType languageOptionType, string languageTag, [NullAllowed] NSString[] languageOptionCharacteristics, string displayName, string identifier);

		[Export ("languageOptionType")]
		MPNowPlayingInfoLanguageOptionType LanguageOptionType { get; }

		[NullAllowed, Export ("languageTag")]
		string LanguageTag { get; }

		[NullAllowed, Export ("languageOptionCharacteristics")]
		NSString[] LanguageOptionCharacteristics { get; }

		[NullAllowed]
		[Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed]
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("isAutomaticLegibleLanguageOption")]
		bool IsAutomaticLegibleLanguageOption { get; }

		[iOS (9,1)][TV (9,0)]
		[Export ("isAutomaticAudibleLanguageOption")]
		bool IsAutomaticAudibleLanguageOption { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // pre-emptive
	interface MPNowPlayingInfoLanguageOptionGroup {
		[Export ("initWithLanguageOptions:defaultLanguageOption:allowEmptySelection:")]
		IntPtr Constructor (MPNowPlayingInfoLanguageOption[] languageOptions, [NullAllowed] MPNowPlayingInfoLanguageOption defaultLanguageOption, bool allowEmptySelection);

		[Export ("languageOptions")]
		MPNowPlayingInfoLanguageOption[] LanguageOptions { get; }

		[NullAllowed, Export ("defaultLanguageOption")]
		MPNowPlayingInfoLanguageOption DefaultLanguageOption { get; }

		[Export ("allowEmptySelection")]
		bool AllowEmptySelection { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (9,0)]
	[Static]
	// not [Internal] since they are exposed as an NSString[] property in MPNowPlayingInfoLanguageOption
	interface MPLanguageOptionCharacteristics {
		[Field ("MPLanguageOptionCharacteristicIsMainProgramContent")]
		NSString IsMainProgramContent { get; }

		[Field ("MPLanguageOptionCharacteristicIsAuxiliaryContent")]
		NSString IsAuxiliaryContent { get; }

		[Field ("MPLanguageOptionCharacteristicContainsOnlyForcedSubtitles")]
		NSString ContainsOnlyForcedSubtitles { get; }

		[Field ("MPLanguageOptionCharacteristicTranscribesSpokenDialog")]
		NSString TranscribesSpokenDialog { get; }

		[Field ("MPLanguageOptionCharacteristicDescribesMusicAndSound")]
		NSString DescribesMusicAndSound { get; }

		[Field ("MPLanguageOptionCharacteristicEasyToRead")]
		NSString EasyToRead { get; }

		[Field ("MPLanguageOptionCharacteristicDescribesVideo")]
		NSString DescribesVideo { get; }

		[Field ("MPLanguageOptionCharacteristicLanguageTranslation")]
		NSString LanguageTranslation { get; }

		[Field ("MPLanguageOptionCharacteristicDubbedTranslation")]
		NSString DubbedTranslation { get; }

		[Field ("MPLanguageOptionCharacteristicVoiceOverTranslation")]
		NSString VoiceOverTranslation { get; }
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (9,1)]
	[BaseType (typeof (MPRemoteCommand))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSGenericException Reason: MPChangePlaybackPositionCommands cannot be initialized externally.
	interface MPChangePlaybackPositionCommand {
	}

	[Mac (10,12,2, onlyOn64: true)]
	[iOS (9,1)]
	[BaseType (typeof (MPRemoteCommandEvent))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSGenericException Reason: MPChangePlaybackPositionCommandEvents cannot be initialized externally.
	interface MPChangePlaybackPositionCommandEvent {
		[Export ("positionTime")]
		double PositionTime { get; }
	}

	[NoMac]
	[NoTV][iOS (9,3)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPMediaPlaylistCreationMetadata {
		[Export ("initWithName:")]
		[DesignatedInitializer]
		IntPtr Constructor (string name);

		[Export ("name")]
		string Name { get; }

		[NullAllowed] // null_resettable
		[Export ("authorDisplayName")]
		string AuthorDisplayName { get; set; }

		[Export ("descriptionText")]
		string DescriptionText { get; set; }
	}

	[NoMac]
	[NoTV]
	[iOS (10,1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MPMusicPlayerQueueDescriptor : NSSecureCoding {

		[Export ("init")]
		[Deprecated (PlatformName.iOS, 11,3)]
		IntPtr Constructor ();
	}

	[NoMac]
	[NoTV]
	[iOS (10,1)]
	[BaseType (typeof(MPMusicPlayerQueueDescriptor))]
	interface MPMusicPlayerMediaItemQueueDescriptor
	{
		[Export ("initWithQuery:")]
		IntPtr Constructor (MPMediaQuery query);

		[Export ("initWithItemCollection:")]
		IntPtr Constructor (MPMediaItemCollection itemCollection);

		[Export ("query", ArgumentSemantic.Copy)]
		MPMediaQuery Query { get; }

		[Export ("itemCollection", ArgumentSemantic.Strong)]
		MPMediaItemCollection ItemCollection { get; }

		[NullAllowed, Export ("startItem", ArgumentSemantic.Strong)]
		MPMediaItem StartItem { get; set; }

		[Export ("setStartTime:forItem:")]
		void SetStartTime (double startTime, MPMediaItem mediaItem);

		[Export ("setEndTime:forItem:")]
		void SetEndTime (double endTime, MPMediaItem mediaItem);
	}

	[NoMac]
	[NoTV]
	[iOS (10,1)]
	[BaseType (typeof(MPMusicPlayerQueueDescriptor))]
	interface MPMusicPlayerStoreQueueDescriptor
	{
		[Export ("initWithStoreIDs:")]
		IntPtr Constructor (string[] storeIDs);

		[NullAllowed, Export ("storeIDs", ArgumentSemantic.Copy)]
		string[] StoreIDs { get; set; }

		[NullAllowed, Export ("startItemID")]
		string StartItemID { get; set; }

		[Export ("setStartTime:forItemWithStoreID:")]
		void SetStartTime (double startTime, string storeID);

		[Export ("setEndTime:forItemWithStoreID:")]
		void SetEndTime (double endTime, string storeID);
	}

	[NoTV]
	[NoMac]
	[iOS (10,3)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MPMusicPlayerControllerQueue
	{
		[Export ("items", ArgumentSemantic.Copy)]
		MPMediaItem[] Items { get; }

		[Field ("MPMusicPlayerControllerQueueDidChangeNotification")]
		[Notification]
		NSString DidChangeNotification { get; }
	}

	[NoTV]
	[NoMac]
	[iOS (10,3)]
	[BaseType (typeof(MPMusicPlayerControllerQueue))]
	interface MPMusicPlayerControllerMutableQueue
	{
		[Export ("insertQueueDescriptor:afterItem:")]
		void InsertAfter (MPMusicPlayerQueueDescriptor queueDescriptor, [NullAllowed] MPMediaItem item);

		[Export ("removeItem:")]
		void RemoveItem (MPMediaItem item);
	}

	[NoTV]
	[NoMac]
	[iOS (10,3)]
	[BaseType (typeof(MPMusicPlayerController))]
	interface MPMusicPlayerApplicationController
	{
		[Async]
		[Export ("performQueueTransaction:completionHandler:")]
		void Perform (Action<MPMusicPlayerControllerMutableQueue> queueTransaction, Action<MPMusicPlayerControllerQueue, NSError> completionHandler);
	}

	[NoTV][NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPMusicPlayerPlayParameters : NSSecureCoding {
		[Export ("initWithDictionary:")]
		IntPtr Constructor (NSDictionary dictionary);

		[Export ("dictionary", ArgumentSemantic.Copy)]
		NSDictionary Dictionary { get; }
	}

	[NoTV][NoMac]
	[iOS (11,0)]
	[BaseType (typeof (MPMusicPlayerQueueDescriptor))]
	[DisableDefaultCtor]
	interface MPMusicPlayerPlayParametersQueueDescriptor {
		[Export ("initWithPlayParametersQueue:")]
		IntPtr Constructor (MPMusicPlayerPlayParameters[] playParametersQueue);

		[Export ("playParametersQueue", ArgumentSemantic.Copy)]
		MPMusicPlayerPlayParameters[] PlayParametersQueue { get; set; }

		[NullAllowed, Export ("startItemPlayParameters", ArgumentSemantic.Strong)]
		MPMusicPlayerPlayParameters StartItemPlayParameters { get; set; }

		[Export ("setStartTime:forItemWithPlayParameters:")]
		void SetStartTime (/* NSTimeInterval */ double startTime, MPMusicPlayerPlayParameters playParameters);

		[Export ("setEndTime:forItemWithPlayParameters:")]
		void SetEndTime (/* NSTimeInterval */ double endTime, MPMusicPlayerPlayParameters playParameters);
	}

	interface IMPSystemMusicPlayerController {}

	[NoTV][NoMac]
	[iOS (11,0)]
	[Protocol]
	interface MPSystemMusicPlayerController {
		[Abstract]
		[Export ("openToPlayQueueDescriptor:")]
		void OpenToPlay (MPMusicPlayerQueueDescriptor queueDescriptor);
	}

	[Category]
	[BaseType (typeof (NSUserActivity))]
	[TV (10,0,1)][iOS (10,1)]
	[NoWatch][NoMac]
	interface NSUserActivity_MediaPlayerAdditions {
		[NullAllowed, Export ("externalMediaContentIdentifier")]
		NSString GetExternalMediaContentIdentifier ();

		[NullAllowed, Export ("setExternalMediaContentIdentifier:")]
		void SetExternalMediaContentIdentifier (NSString identifier);
	}
#endif
}
