// Copyright 2016 Xamarin, Inc.
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

using System;

using AppKit;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MediaLibrary {
	[Static]
	[Deprecated (PlatformName.MacOSX, 10, 15)]
	interface MediaLibraryTypeIdentifierKey {
		[Field ("MLFolderRootGroupTypeIdentifier")]
		NSString FolderRootGroupTypeIdentifier { get; }

		[Field ("MLFolderGroupTypeIdentifier")]
		NSString FolderGroupTypeIdentifier { get; }

		[Field ("MLiTunesRootGroupTypeIdentifier")]
		NSString ITunesRootGroupTypeIdentifier { get; }

		[Field ("MLiTunesPlaylistTypeIdentifier")]
		NSString ITunesPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesPurchasedPlaylistTypeIdentifier")]
		NSString ITunesPurchasedPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesPodcastPlaylistTypeIdentifier")]
		NSString ITunesPodcastPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesVideoPlaylistTypeIdentifier")]
		NSString ITunesVideoPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesSmartPlaylistTypeIdentifier")]
		NSString ITunesSmartPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesFolderPlaylistTypeIdentifier")]
		NSString ITunesFolderPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesMoviesPlaylistTypeIdentifier")]
		NSString ITunesMoviesPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesTVShowsPlaylistTypeIdentifier")]
		NSString ITunesTVShowsPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesAudioBooksPlaylistTypeIdentifier")]
		NSString ITunesAudioBooksPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesMusicPlaylistTypeIdentifier")]
		NSString ITunesMusicPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesMusicVideosPlaylistTypeIdentifier")]
		NSString ITunesMusicVideosPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesGeniusPlaylistTypeIdentifier")]
		NSString ITunesGeniusPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesSavedGeniusPlaylistTypeIdentifier")]
		NSString ITunesSavedGeniusPlaylistTypeIdentifier { get; }

		[Field ("MLiTunesiTunesUPlaylistTypeIdentifier")]
		NSString ITunesiTunesUPlaylistTypeIdentifier { get; }

		[Field ("MLPhotosRootGroupTypeIdentifier")]
		NSString PhotosRootGroupTypeIdentifier { get; }

		[Field ("MLPhotosSharedGroupTypeIdentifier")]
		NSString PhotosSharedGroupTypeIdentifier { get; }

		[Field ("MLPhotosAlbumsGroupTypeIdentifier")]
		NSString PhotosAlbumsGroupTypeIdentifier { get; }

		[Field ("MLPhotosAlbumTypeIdentifier")]
		NSString PhotosAlbumTypeIdentifier { get; }

		[Field ("MLPhotosFolderTypeIdentifier")]
		NSString PhotosFolderTypeIdentifier { get; }

		[Field ("MLPhotosSmartAlbumTypeIdentifier")]
		NSString PhotosSmartAlbumTypeIdentifier { get; }

		[Field ("MLPhotosPublishedAlbumTypeIdentifier")]
		NSString PhotosPublishedAlbumTypeIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Field ("MLPhotosAllMomentsGroupTypeIdentifier")]
		NSString PhotosAllMomentsGroupTypeIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Field ("MLPhotosMomentGroupTypeIdentifier")]
		NSString PhotosMomentGroupTypeIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Field ("MLPhotosAllCollectionsGroupTypeIdentifier")]
		NSString PhotosAllCollectionsGroupTypeIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Field ("MLPhotosCollectionGroupTypeIdentifier")]
		NSString PhotosCollectionGroupTypeIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Field ("MLPhotosAllYearsGroupTypeIdentifier")]
		NSString PhotosAllYearsGroupTypeIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Field ("MLPhotosYearGroupTypeIdentifier")]
		NSString PhotosYearGroupTypeIdentifier { get; }

		[Field ("MLPhotosLastImportGroupTypeIdentifier")]
		NSString PhotosLastImportGroupTypeIdentifier { get; }

		[Field ("MLPhotosMyPhotoStreamTypeIdentifier")]
		NSString PhotosMyPhotoStreamTypeIdentifier { get; }

		[Field ("MLPhotosSharedPhotoStreamTypeIdentifier")]
		NSString PhotosSharedPhotoStreamTypeIdentifier { get; }

		[Field ("MLPhotosFavoritesGroupTypeIdentifier")]
		NSString PhotosFavoritesGroupTypeIdentifier { get; }

		[Field ("MLPhotosFrontCameraGroupTypeIdentifier")]
		NSString PhotosFrontCameraGroupTypeIdentifier { get; }

		[Field ("MLPhotosLivePhotosGroupTypeIdentifier")]
		NSString PhotosLivePhotosGroupTypeIdentifier { get; }

		[Field ("MLPhotosLongExposureGroupTypeIdentifier")]
		NSString PhotosLongExposureGroupTypeIdentifier { get; }

		[Field ("MLPhotosAnimatedGroupTypeIdentifier")]
		NSString PhotosAnimatedGroupTypeIdentifier { get; }

		[Field ("MLPhotosPanoramasGroupTypeIdentifier")]
		NSString PhotosPanoramasGroupTypeIdentifier { get; }

		[Field ("MLPhotosVideosGroupTypeIdentifier")]
		NSString PhotosVideosGroupTypeIdentifier { get; }

		[Field ("MLPhotosSloMoGroupTypeIdentifier")]
		NSString PhotosSloMoGroupTypeIdentifier { get; }

		[Field ("MLPhotosTimelapseGroupTypeIdentifier")]
		NSString PhotosTimelapseGroupTypeIdentifier { get; }

		[Field ("MLPhotosBurstGroupTypeIdentifier")]
		NSString PhotosBurstGroupTypeIdentifier { get; }

		[Field ("MLPhotosScreenshotGroupTypeIdentifier")]
		NSString PhotosScreenshotGroupTypeIdentifier { get; }

		[Field ("MLPhotosFacesAlbumTypeIdentifier")]
		NSString PhotosFacesAlbumTypeIdentifier { get; }

		[Field ("MLPhotosAllPhotosAlbumTypeIdentifier")]
		NSString PhotosAllPhotosAlbumTypeIdentifier { get; }

		[Field ("MLPhotosDepthEffectGroupTypeIdentifier")]
		NSString PhotosDepthEffectGroupTypeIdentifier { get; }

		[Field ("MLiPhotoRootGroupTypeIdentifier")]
		NSString IPhotoRootGroupTypeIdentifier { get; }

		[Field ("MLiPhotoAlbumTypeIdentifier")]
		NSString IPhotoAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoLibraryAlbumTypeIdentifier")]
		NSString IPhotoLibraryAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoEventsFolderTypeIdentifier")]
		NSString IPhotoEventsFolderTypeIdentifier { get; }

		[Field ("MLiPhotoSmartAlbumTypeIdentifier")]
		NSString IPhotoSmartAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoEventAlbumTypeIdentifier")]
		NSString IPhotoEventAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoLastImportAlbumTypeIdentifier")]
		NSString IPhotoLastImportAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoLastNMonthsAlbumTypeIdentifier")]
		NSString IPhotoLastNMonthsAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoFlaggedAlbumTypeIdentifier")]
		NSString IPhotoFlaggedAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoFolderAlbumTypeIdentifier")]
		NSString IPhotoFolderAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoSubscribedAlbumTypeIdentifier")]
		NSString IPhotoSubscribedAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoFacesAlbumTypeIdentifier")]
		NSString IPhotoFacesAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoPlacesAlbumTypeIdentifier")]
		NSString IPhotoPlacesAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoPlacesCountryAlbumTypeIdentifier")]
		NSString IPhotoPlacesCountryAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoPlacesProvinceAlbumTypeIdentifier")]
		NSString IPhotoPlacesProvinceAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoPlacesCityAlbumTypeIdentifier")]
		NSString IPhotoPlacesCityAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoPlacesPointOfInterestAlbumTypeIdentifier")]
		NSString IPhotoPlacesPointOfInterestAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoFacebookAlbumTypeIdentifier")]
		NSString IPhotoFacebookAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoFlickrAlbumTypeIdentifier")]
		NSString IPhotoFlickrAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoFacebookGroupTypeIdentifier")]
		NSString IPhotoFacebookGroupTypeIdentifier { get; }

		[Field ("MLiPhotoFlickrGroupTypeIdentifier")]
		NSString IPhotoFlickrGroupTypeIdentifier { get; }

		[Field ("MLiPhotoSlideShowAlbumTypeIdentifier")]
		NSString IPhotoSlideShowAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoLastViewedEventAlbumTypeIdentifier")]
		NSString IPhotoLastViewedEventAlbumTypeIdentifier { get; }

		[Field ("MLiPhotoPhotoStreamAlbumTypeIdentifier")]
		NSString IPhotoPhotoStreamAlbumTypeIdentifier { get; }

		[Field ("MLApertureRootGroupTypeIdentifier")]
		NSString ApertureRootGroupTypeIdentifier { get; }

		[Field ("MLApertureUserAlbumTypeIdentifier")]
		NSString ApertureUserAlbumTypeIdentifier { get; }

		[Field ("MLApertureUserSmartAlbumTypeIdentifier")]
		NSString ApertureUserSmartAlbumTypeIdentifier { get; }

		[Field ("MLApertureProjectAlbumTypeIdentifier")]
		NSString ApertureProjectAlbumTypeIdentifier { get; }

		[Field ("MLApertureFolderAlbumTypeIdentifier")]
		NSString ApertureFolderAlbumTypeIdentifier { get; }

		[Field ("MLApertureProjectFolderAlbumTypeIdentifier")]
		NSString ApertureProjectFolderAlbumTypeIdentifier { get; }

		[Field ("MLApertureLightTableTypeIdentifier")]
		NSString ApertureLightTableTypeIdentifier { get; }

		[Field ("MLApertureFlickrGroupTypeIdentifier")]
		NSString ApertureFlickrGroupTypeIdentifier { get; }

		[Field ("MLApertureFlickrAlbumTypeIdentifier")]
		NSString ApertureFlickrAlbumTypeIdentifier { get; }

		[Field ("MLApertureFacebookGroupTypeIdentifier")]
		NSString ApertureFacebookGroupTypeIdentifier { get; }

		[Field ("MLApertureFacebookAlbumTypeIdentifier")]
		NSString ApertureFacebookAlbumTypeIdentifier { get; }

		[Field ("MLApertureSmugMugGroupTypeIdentifier")]
		NSString ApertureSmugMugGroupTypeIdentifier { get; }

		[Field ("MLApertureSmugMugAlbumTypeIdentifier")]
		NSString ApertureSmugMugAlbumTypeIdentifier { get; }

		[Field ("MLApertureSlideShowTypeIdentifier")]
		NSString ApertureSlideShowTypeIdentifier { get; }

		[Field ("MLApertureAllPhotosTypeIdentifier")]
		NSString ApertureAllPhotosTypeIdentifier { get; }

		[Field ("MLApertureFlaggedTypeIdentifier")]
		NSString ApertureFlaggedTypeIdentifier { get; }

		[Field ("MLApertureAllProjectsTypeIdentifier")]
		NSString ApertureAllProjectsTypeIdentifier { get; }

		[Field ("MLApertureFacesAlbumTypeIdentifier")]
		NSString ApertureFacesAlbumTypeIdentifier { get; }

		[Field ("MLAperturePlacesAlbumTypeIdentifier")]
		NSString AperturePlacesAlbumTypeIdentifier { get; }

		[Field ("MLAperturePlacesCountryAlbumTypeIdentifier")]
		NSString AperturePlacesCountryAlbumTypeIdentifier { get; }

		[Field ("MLAperturePlacesProvinceAlbumTypeIdentifier")]
		NSString AperturePlacesProvinceAlbumTypeIdentifier { get; }

		[Field ("MLAperturePlacesCityAlbumTypeIdentifier")]
		NSString AperturePlacesCityAlbumTypeIdentifier { get; }

		[Field ("MLAperturePlacesPointOfInterestAlbumTypeIdentifier")]
		NSString AperturePlacesPointOfInterestAlbumTypeIdentifier { get; }

		[Field ("MLApertureLastImportAlbumTypeIdentifier")]
		NSString ApertureLastImportAlbumTypeIdentifier { get; }

		[Field ("MLApertureLastNMonthsAlbumTypeIdentifier")]
		NSString ApertureLastNMonthsAlbumTypeIdentifier { get; }

		[Field ("MLApertureLastViewedEventAlbumTypeIdentifier")]
		NSString ApertureLastViewedEventAlbumTypeIdentifier { get; }

		[Field ("MLAperturePhotoStreamAlbumTypeIdentifier")]
		NSString AperturePhotoStreamAlbumTypeIdentifier { get; }

		[Field ("MLGarageBandRootGroupTypeIdentifier")]
		NSString GarageBandRootGroupTypeIdentifier { get; }

		[Field ("MLGarageBandFolderGroupTypeIdentifier")]
		NSString GarageBandFolderGroupTypeIdentifier { get; }

		[Field ("MLLogicRootGroupTypeIdentifier")]
		NSString LogicRootGroupTypeIdentifier { get; }

		[Field ("MLLogicBouncesGroupTypeIdentifier")]
		NSString LogicBouncesGroupTypeIdentifier { get; }

		[Field ("MLLogicProjectsGroupTypeIdentifier")]
		NSString LogicProjectsGroupTypeIdentifier { get; }

		[Field ("MLLogicProjectTypeIdentifier")]
		NSString LogicProjectTypeIdentifier { get; }

		[Field ("MLiMovieRootGroupTypeIdentifier")]
		NSString IMovieRootGroupTypeIdentifier { get; }

		[Field ("MLiMovieEventGroupTypeIdentifier")]
		NSString IMovieEventGroupTypeIdentifier { get; }

		[Field ("MLiMovieProjectGroupTypeIdentifier")]
		NSString IMovieProjectGroupTypeIdentifier { get; }

		[Field ("MLiMovieEventLibraryGroupTypeIdentifier")]
		NSString IMovieEventLibraryGroupTypeIdentifier { get; }

		[Field ("MLiMovieEventCalendarGroupTypeIdentifier")]
		NSString IMovieEventCalendarGroupTypeIdentifier { get; }

		[Field ("MLiMovieFolderGroupTypeIdentifier")]
		NSString IMovieFolderGroupTypeIdentifier { get; }

		[Field ("MLFinalCutRootGroupTypeIdentifier")]
		NSString FinalCutRootGroupTypeIdentifier { get; }

		[Field ("MLFinalCutEventGroupTypeIdentifier")]
		NSString FinalCutEventGroupTypeIdentifier { get; }

		[Field ("MLFinalCutProjectGroupTypeIdentifier")]
		NSString FinalCutProjectGroupTypeIdentifier { get; }

		[Field ("MLFinalCutEventLibraryGroupTypeIdentifier")]
		NSString FinalCutEventLibraryGroupTypeIdentifier { get; }

		[Field ("MLFinalCutEventCalendarGroupTypeIdentifier")]
		NSString FinalCutEventCalendarGroupTypeIdentifier { get; }

		[Field ("MLFinalCutFolderGroupTypeIdentifier")]
		NSString FinalCutFolderGroupTypeIdentifier { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 15)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLMediaLibrary {
		[Export ("initWithOptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDictionary<NSString, NSObject> options);

		[NullAllowed, Export ("mediaSources", ArgumentSemantic.Copy)]
		NSDictionary<NSString, MLMediaSource> MediaSources { get; }

		[Field ("MLMediaLoadSourceTypesKey")]
		NSString MediaLoadSourceTypesKey { get; }

		[Field ("MLMediaLoadIncludeSourcesKey")]
		NSString MediaLoadIncludeSourcesKey { get; }

		[Field ("MLMediaLoadExcludeSourcesKey")]
		NSString MediaLoadExcludeSourcesKey { get; }

		[Field ("MLMediaLoadFoldersKey")]
		NSString MediaLoadFoldersKey { get; }

		[Field ("MLMediaLoadAppleLoops")]
		NSString MediaLoadAppleLoops { get; }

		[Field ("MLMediaLoadMoviesFolder")]
		NSString MediaLoadMoviesFolder { get; }

		[Field ("MLMediaLoadAppFoldersKey")]
		NSString MediaLoadAppFoldersKey { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 15)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLMediaSource {
		[NullAllowed, Export ("mediaLibrary", ArgumentSemantic.Assign)]
		MLMediaLibrary MediaLibrary { get; }

		[Export ("mediaSourceIdentifier")]
		NSString MediaSourceIdentifier { get; }

		[Export ("attributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Attributes { get; }

		[NullAllowed, Export ("rootMediaGroup", ArgumentSemantic.Retain)]
		MLMediaGroup RootMediaGroup { get; }

		[Export ("mediaGroupForIdentifier:")]
		[return: NullAllowed]
		MLMediaGroup MediaGroupForIdentifier (NSString mediaGroupIdentifier);

		[Export ("mediaGroupsForIdentifiers:")]
		NSDictionary<NSString, MLMediaGroup> MediaGroupsForIdentifiers (NSString [] mediaGroupIdentifiers);

		[Export ("mediaObjectForIdentifier:")]
		[return: NullAllowed]
		MLMediaObject MediaObjectForIdentifier (NSString mediaObjectIdentifier);

		[Export ("mediaObjectsForIdentifiers:")]
		NSDictionary<NSString, MLMediaObject> MediaObjectsForIdentifiers (NSString [] mediaObjectIdentifiers);

		[Field ("MLMediaSourcePhotosIdentifier")]
		NSString MediaSourcePhotosIdentifier { get; }

		[Field ("MLMediaSourceiPhotoIdentifier")]
		NSString MediaSourceiPhotoIdentifier { get; }

		[Field ("MLMediaSourceiTunesIdentifier")]
		NSString MediaSourceiTunesIdentifier { get; }

		[Field ("MLMediaSourceApertureIdentifier")]
		NSString MediaSourceApertureIdentifier { get; }

		[Field ("MLMediaSourceiMovieIdentifier")]
		NSString MediaSourceiMovieIdentifier { get; }

		[Field ("MLMediaSourceFinalCutIdentifier")]
		NSString MediaSourceFinalCutIdentifier { get; }

		[Field ("MLMediaSourceGarageBandIdentifier")]
		NSString MediaSourceGarageBandIdentifier { get; }

		[Field ("MLMediaSourceLogicIdentifier")]
		NSString MediaSourceLogicIdentifier { get; }

		[Field ("MLMediaSourcePhotoBoothIdentifier")]
		NSString MediaSourcePhotoBoothIdentifier { get; }

		[Field ("MLMediaSourceCustomFoldersIdentifier")]
		NSString MediaSourceCustomFoldersIdentifier { get; }

		[Field ("MLMediaSourceMoviesFolderIdentifier")]
		NSString MediaSourceMoviesFolderIdentifier { get; }

		[Field ("MLMediaSourceAppDefinedFoldersIdentifier")]
		NSString MediaSourceAppDefinedFoldersIdentifier { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 15)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLMediaGroup {
		[NullAllowed, Export ("mediaLibrary", ArgumentSemantic.Assign)]
		MLMediaLibrary MediaLibrary { get; }

		[NullAllowed, Export ("parent", ArgumentSemantic.Assign)]
		MLMediaGroup Parent { get; }

		[Export ("mediaSourceIdentifier")]
		NSString MediaSourceIdentifier { get; }

		[NullAllowed, Export ("name")]
		string Name { get; }

		[Export ("identifier")]
		NSString Identifier { get; }

		[Export ("typeIdentifier")]
		NSString TypeIdentifier { get; }

		[Export ("attributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Attributes { get; }

		[NullAllowed, Export ("childGroups", ArgumentSemantic.Copy)]
		MLMediaGroup [] ChildGroups { get; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[NullAllowed, Export ("modificationDate", ArgumentSemantic.Copy)]
		NSDate ModificationDate { get; }

		[NullAllowed, Export ("iconImage", ArgumentSemantic.Copy)]
		NSImage IconImage { get; }

		[NullAllowed, Export ("mediaObjects", ArgumentSemantic.Copy)]
		MLMediaObject [] MediaObjects { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 15)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLMediaObject {
		[NullAllowed, Export ("mediaLibrary", ArgumentSemantic.Assign)]
		MLMediaLibrary MediaLibrary { get; }

		[Export ("identifier")]
		NSString Identifier { get; }

		[Export ("mediaSourceIdentifier")]
		NSString MediaSourceIdentifier { get; }

		[Export ("attributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Attributes { get; }

		[Export ("mediaType", ArgumentSemantic.Assign)]
		MLMediaType MediaType { get; }

		[NullAllowed, Export ("contentType")]
		string ContentType { get; }

		[NullAllowed, Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[NullAllowed, Export ("originalURL", ArgumentSemantic.Copy)]
		NSUrl OriginalUrl { get; }

		[Export ("fileSize")]
		nuint FileSize { get; }

		[NullAllowed, Export ("modificationDate", ArgumentSemantic.Copy)]
		NSDate ModificationDate { get; }

		[NullAllowed, Export ("thumbnailURL", ArgumentSemantic.Copy)]
		NSUrl ThumbnailUrl { get; }

		[NullAllowed, Export ("artworkImage", ArgumentSemantic.Copy)]
		NSImage ArtworkImage { get; }

		[Field ("MLMediaObjectDurationKey")]
		NSString MediaObjectDurationKey { get; }

		[Field ("MLMediaObjectArtistKey")]
		NSString MediaObjectArtistKey { get; }

		[Field ("MLMediaObjectAlbumKey")]
		NSString MediaObjectAlbumKey { get; }

		[Field ("MLMediaObjectGenreKey")]
		NSString MediaObjectGenreKey { get; }

		[Field ("MLMediaObjectKindKey")]
		NSString MediaObjectKindKey { get; }

		[Field ("MLMediaObjectTrackNumberKey")]
		NSString MediaObjectTrackNumberKey { get; }

		[Field ("MLMediaObjectBitRateKey")]
		NSString MediaObjectBitRateKey { get; }

		[Field ("MLMediaObjectSampleRateKey")]
		NSString MediaObjectSampleRateKey { get; }

		[Field ("MLMediaObjectChannelCountKey")]
		NSString MediaObjectChannelCountKey { get; }

		[Field ("MLMediaObjectResolutionStringKey")]
		NSString MediaObjectResolutionStringKey { get; }

		[Field ("MLMediaObjectCommentsKey")]
		NSString MediaObjectCommentsKey { get; }

		[Field ("MLMediaObjectKeywordsKey")]
		NSString MediaObjectKeywordsKey { get; }

		[Field ("MLMediaObjectProtectedKey")]
		NSString MediaObjectProtectedKey { get; }
	}
}
