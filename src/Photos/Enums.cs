using System;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

namespace Photos
{
	// NSInteger -> PHImageManager.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHImageContentMode : long {
		AspectFit = 0,
		AspectFill = 1,
		Default = AspectFit
	}

	// NSInteger -> PHImageManager.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum PHImageRequestOptionsVersion : long {
		Current = 0,
		Unadjusted,
		Original
	}

	// NSInteger -> PHImageManager.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum PHImageRequestOptionsDeliveryMode : long {
		Opportunistic = 0,
		HighQualityFormat = 1,
		FastFormat = 2
	}

	// NSInteger -> PHImageManager.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum PHImageRequestOptionsResizeMode : long {
		None = 0,
		Fast,
		Exact
	}

	// NSInteger -> PHImageManager.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.15")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,15)]
#endif
	[Native]
	public enum PHVideoRequestOptionsVersion : long {
		Current = 0,
		Original
	}

	// NSInteger -> PHImageManager.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.15")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,15)]
#endif
	[Native]
	public enum PHVideoRequestOptionsDeliveryMode : long {
		Automatic = 0,
		HighQualityFormat = 1,
		MediumQualityFormat = 2,
		FastFormat = 3
	}

	// NSInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHCollectionListType : long {
#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
#endif
		MomentList  = 1,
		Folder      = 2,
		SmartFolder = 3
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHCollectionListSubtype : long {
#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
#endif
		MomentListCluster = 1,

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
#endif
		MomentListYear = 2,

		RegularFolder = 100,

		SmartFolderEvents = 200,
		SmartFolderFaces = 201,

#if !XAMCORE_3_0
		// this was added in the wrong enum type (ref bug #40019)
		[Obsolete ("Incorrect value (exists in 'PHAssetCollectionSubtype').")]
		SmartAlbumSelfPortraits = 210,
		[Obsolete ("Incorrect value (exists in 'PHAssetCollectionSubtype').")]
		SmartAlbumScreenshots = 211,
#endif
		Any = Int64.MaxValue,
	}

	// NSUInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHCollectionEditOperation : long {
		None             = 0,
		DeleteContent    = 1,
		RemoveContent    = 2,
		AddContent       = 3,
		CreateContent    = 4,
		RearrangeContent = 5,
		Delete           = 6,
		Rename           = 7
	}

	// NSInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHAssetCollectionType : long {
		Album      = 1,
		SmartAlbum = 2,

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
#endif
		Moment     = 3
	}

	// NSInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHAssetCollectionSubtype : long {
		AlbumRegular         = 2,
		AlbumSyncedEvent     = 3,
		AlbumSyncedFaces     = 4,
		AlbumSyncedAlbum     = 5,
		AlbumImported        = 6,
		AlbumMyPhotoStream   = 100,
		AlbumCloudShared     = 101,
		SmartAlbumGeneric    = 200,
		SmartAlbumPanoramas  = 201,
		SmartAlbumVideos     = 202,
		SmartAlbumFavorites  = 203,
		SmartAlbumTimelapses = 204,
		SmartAlbumAllHidden  = 205,
		SmartAlbumRecentlyAdded = 206,
		SmartAlbumBursts        = 207,
		SmartAlbumSlomoVideos   = 208,
		SmartAlbumUserLibrary 	= 209,
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.12")]
#else
		[iOS (9,0)]
#endif
		SmartAlbumSelfPortraits = 210,
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.12")]
#else
		[iOS (9,0)]
#endif
		SmartAlbumScreenshots   = 211,
#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("macos10.13")]
#else
		[iOS (10,2)]
		[TV (10,1)]
		[Mac (10,13)]
#endif
		SmartAlbumDepthEffect   = 212,
#if NET
		[SupportedOSPlatform ("ios10.3")]
		[SupportedOSPlatform ("tvos10.2")]
		[SupportedOSPlatform ("macos10.13")]
#else
		[iOS (10,3)]
		[TV (10,2)]
		[Mac (10,13)]
#endif
		SmartAlbumLivePhotos = 213,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.15")]
#else
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,15)]
#endif
		SmartAlbumAnimated = 214,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.15")]
#else
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,15)]
#endif
		SmartAlbumLongExposures = 215,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#else
		[iOS (13,0)]
		[TV (13,0)]
		[Mac (10,15)]
#endif
		SmartAlbumUnableToUpload = 216,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15,0)]
		[TV (15,0)]
		[Mac (12,0)]
		[MacCatalyst (15,0)]
#endif
		SmartAlbumRAW = 217,


		Any           = Int64.MaxValue
	}

	// NSUInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHAssetEditOperation : long {
		None       = 0,
		Delete     = 1,
		Content    = 2,
		Properties = 3
	}

	// NSInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHAssetMediaType : long {
		Unknown = 0,
		Image   = 1,
		Video   = 2,
		Audio   = 3
	}

	// NSUInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	[Flags]
	public enum PHAssetMediaSubtype : ulong {
		None               = 0,
		PhotoPanorama      = (1 << 0),
		PhotoHDR           = (1 << 1),
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.12")]
#else
		[iOS (9,0)]
#endif
		Screenshot         = (1 << 2),
#if NET
		[SupportedOSPlatform ("ios9.1")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.12")]
#else
		[iOS (9,1)]
#endif
		PhotoLive          = (1 << 3),
#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("macos10.15")]
#else
		[iOS (10,2)]
		[TV (10,1)]
		[Mac (10,15)]
#endif
		PhotoDepthEffect   = (1 << 4),
		VideoStreamed      = (1 << 16),
		VideoHighFrameRate = (1 << 17),
		VideoTimelapse     = (1 << 18),
	}

	// NSUInteger -> PhotosTypes.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	[Flags]
	public enum PHAssetBurstSelectionType : ulong {
		None     = 0,
		AutoPick = (1 << 0),
		UserPick = (1 << 1)
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum PHAuthorizationStatus : long {
		NotDetermined,
		Restricted,
		Denied,
		Authorized,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[iOS (14,0)]
		[NoTV]
		[NoMac]
#endif
		Limited,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (9,0)]
	[Mac (10,12)]
	[TV (10,0)]
#endif
	[Native]
	public enum PHAssetResourceType : long
	{
		Photo = 1,
		Video = 2,
		Audio = 3,
		AlternatePhoto = 4,
		FullSizePhoto = 5,
		FullSizeVideo = 6,
		AdjustmentData = 7,
		AdjustmentBasePhoto = 8,
#if NET
		[SupportedOSPlatform ("ios9.1")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[iOS (9,1)]
#endif
		PairedVideo = 9,
#if NET
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		FullSizePairedVideo = 10,
#if NET
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		AdjustmentBasePairedVideo = 11,
#if NET
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
#else
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
#endif
		AdjustmentBaseVideo = 12,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHAssetSourceType : ulong
	{
		None = 0,
		UserLibrary = (1 << 0),
		CloudShared = (1 << 1),
		iTunesSynced = (1 << 2)
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	public enum PHLivePhotoFrameType : long {
		Photo,
		Video
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[TV (11,0)]
	[iOS (11,0)]
	[Mac (10,13)]
#endif
	[Native]
	public enum PHAssetPlaybackStyle : long {
		Unsupported = 0,
		Image = 1,
		ImageAnimated = 2,
		LivePhoto = 3,
		Video = 4,
		VideoLooping = 5,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (10,13)]
	[NoiOS]
	[NoTV]
#endif
	[Native]
	public enum PHProjectTextElementType : long {
		Body = 0,
		Title,
		Subtitle,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (10,13)]
	[NoiOS]
	[NoTV]
#endif
	[Native]
	public enum PHProjectCreationSource : long {
		Undefined = 0,
		UserSelection = 1,
		Album = 2,
		Memory = 3,
		Moment = 4,
		Project = 20,
		ProjectBook = 21,
		ProjectCalendar = 22,
		ProjectCard = 23,
		ProjectPrintOrder = 24,
		ProjectSlideshow = 25,
		ProjectExtension = 26,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (10,13)]
	[NoiOS]
	[NoTV]
#endif
	[Native]
	public enum PHProjectSectionType : long {
		Undefined = 0,
		Cover = 1,
		Content = 2,
		Auxiliary = 3,
	}

#if NET
	[SupportedOSPlatform ("macos10.12")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoMacCatalyst]
	[Mac (10,12)]
	[NoiOS]
	[NoTV]
#endif
	[Native]
	[ErrorDomain ("PHLivePhotoEditingErrorDomain")]
	public enum PHLivePhotoEditingError : long {
#if NET
		[SupportedOSPlatform ("macos10.12")]
		[UnsupportedOSPlatform ("macos10.15")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'PHPhotosError.InternalError' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PHPhotosError.InternalError' instead.")]
#endif
		Unknown,
#if NET
		[SupportedOSPlatform ("macos10.12")]
		[UnsupportedOSPlatform ("macos10.15")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'PHPhotosError.UserCancelled' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PHPhotosError.UserCancelled' instead.")]
#endif
		Aborted,
	}

#if NET
	[SupportedOSPlatform ("macos10.14")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (10,14)]
	[NoiOS]
	[NoTV]
#endif
	public enum FigExifCustomRenderedValue : short {
		NotCustom = 0,
		Custom = 1,
		HdrImage = 2,
		HdrPlusEV0_HdrImage = 3,
		HdrPlusEV0_EV0Image = 4,
		PanoramaImage = 6,
		SdofImage = 7,
		SdofPlusOriginal_SdofImage = 8,
		SdofPlusOriginal_OriginalImage = 9,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	[ErrorDomain ("PHPhotosErrorDomain")]
	[Native]
	public enum PHPhotosError : long {
#if !NET
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("macos12.0")]
#if MONOMAC
		[Obsolete ("Starting with macos12.0 use 'InternalError' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'InternalError' instead.")]
#endif
		Invalid = -1,
#endif
		InternalError = -1,
		UserCancelled = 3072,
		LibraryVolumeOffline = 3114,
		RelinquishingLibraryBundleToWriter = 3142,
		SwitchingSystemPhotoLibrary = 3143,
		NetworkAccessRequired = 3164,
		IdentifierNotFound = 3201,
		MultipleIdentifiersFound = 3202,
		ChangeNotSupported = 3300,
		OperationInterrupted = 3301,
		InvalidResource = 3302,
		MissingResource = 3303,
		NotEnoughSpace = 3305,
		RequestNotSupportedForAsset = 3306,
		AccessRestricted = 3310,
		AccessUserDenied = 3311,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[TV (14,0)]
	[Mac (11,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum PHAccessLevel : long
	{
		AddOnly = 1,
		ReadWrite = 2,
	}
}
