using System;
using ObjCRuntime;
using Foundation;

namespace Photos {
	// NSInteger -> PHImageManager.h
	/// <summary>Enumerates values that control how images are displayed.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHImageContentMode : long {
		AspectFit = 0,
		AspectFill = 1,
		Default = AspectFit
	}

	// NSInteger -> PHImageManager.h
	/// <summary>Enumerates values that control whether to retrieve edited or unedited versions of images.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHImageRequestOptionsVersion : long {
		Current = 0,
		Unadjusted,
		Original
	}

	// NSInteger -> PHImageManager.h
	/// <summary>Enumerates values that control the desired balance between speed and quality when retrieving image data.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHImageRequestOptionsDeliveryMode : long {
		Opportunistic = 0,
		HighQualityFormat = 1,
		FastFormat = 2
	}

	// NSInteger -> PHImageManager.h
	/// <summary>Enumerates values that control the speed and accuracy of image resizing operations.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHImageRequestOptionsResizeMode : long {
		None = 0,
		Fast,
		Exact
	}

	// NSInteger -> PHImageManager.h
	/// <summary>Enumerates values that control whether to return the edited or original version of a video asset.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHVideoRequestOptionsVersion : long {
		Current = 0,
		Original
	}

	// NSInteger -> PHImageManager.h
	/// <summary>Enumerates values that balance the load time and quality of video when requesting video data.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHVideoRequestOptionsDeliveryMode : long {
		Automatic = 0,
		HighQualityFormat = 1,
		MediumQualityFormat = 2,
		FastFormat = 3
	}

	// NSInteger -> PhotosTypes.h
	/// <summary>Enumerates values that indicate whether a collection is a moment list, folder, or smart folder.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHCollectionListType : long {
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		MomentList = 1,
		Folder = 2,
		SmartFolder = 3
	}

	/// <summary>Enumerates values that indicate the subtype of the collection.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHCollectionListSubtype : long {
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		MomentListCluster = 1,

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		MomentListYear = 2,

		RegularFolder = 100,

		SmartFolderEvents = 200,
		SmartFolderFaces = 201,

		Any = Int64.MaxValue,
	}

	// NSUInteger -> PhotosTypes.h
	/// <summary>Enumerates values that describe the editing operations that can be performed on a collection.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHCollectionEditOperation : long {
		None = 0,
		DeleteContent = 1,
		RemoveContent = 2,
		AddContent = 3,
		CreateContent = 4,
		RearrangeContent = 5,
		Delete = 6,
		Rename = 7
	}

	// NSInteger -> PhotosTypes.h
	/// <summary>Enumerates varieties of <see cref="T:Photos.PHAssetCollection" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetCollectionType : long {
		Album = 1,
		SmartAlbum = 2,

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		Moment = 3
	}

	// NSInteger -> PhotosTypes.h
	/// <summary>Enumerates values that describe the particular subtype (For example, time lapses, bursts, shared collections in the cloud, and etc.) of an asset collection.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetCollectionSubtype : long {
		AlbumRegular = 2,
		AlbumSyncedEvent = 3,
		AlbumSyncedFaces = 4,
		AlbumSyncedAlbum = 5,
		AlbumImported = 6,
		AlbumMyPhotoStream = 100,
		AlbumCloudShared = 101,
		SmartAlbumGeneric = 200,
		SmartAlbumPanoramas = 201,
		SmartAlbumVideos = 202,
		SmartAlbumFavorites = 203,
		SmartAlbumTimelapses = 204,
		SmartAlbumAllHidden = 205,
		SmartAlbumRecentlyAdded = 206,
		SmartAlbumBursts = 207,
		SmartAlbumSlomoVideos = 208,
		SmartAlbumUserLibrary = 209,
		[MacCatalyst (13, 1)]
		SmartAlbumSelfPortraits = 210,
		[MacCatalyst (13, 1)]
		SmartAlbumScreenshots = 211,
		[MacCatalyst (13, 1)]
		SmartAlbumDepthEffect = 212,
		[MacCatalyst (13, 1)]
		SmartAlbumLivePhotos = 213,
		[MacCatalyst (13, 1)]
		SmartAlbumAnimated = 214,
		[MacCatalyst (13, 1)]
		SmartAlbumLongExposures = 215,
		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		SmartAlbumUnableToUpload = 216,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		SmartAlbumRAW = 217,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		SmartAlbumCinematic = 218,
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		SmartAlbumSpatial = 219,

		Any = Int64.MaxValue
	}

	// NSUInteger -> PhotosTypes.h
	/// <summary>Enumerates values that indicate whether an operation edits or deletes an asset, changes its properties, or performs no action on the asset.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetEditOperation : long {
		None = 0,
		Delete = 1,
		Content = 2,
		Properties = 3
	}

	// NSInteger -> PhotosTypes.h
	/// <summary>Enumerates the forms of <see cref="T:Photos.PHAsset" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetMediaType : long {
		Unknown = 0,
		Image = 1,
		Video = 2,
		Audio = 3
	}

	// NSUInteger -> PhotosTypes.h
	/// <summary>Enumerates values that describe media subtypes. (HDR, panorama, streaming video, and etc.)</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum PHAssetMediaSubtype : ulong {
		None = 0,
		PhotoPanorama = (1 << 0),
		PhotoHDR = (1 << 1),
		[MacCatalyst (13, 1)]
		Screenshot = (1 << 2),
		[MacCatalyst (13, 1)]
		PhotoLive = (1 << 3),
		[MacCatalyst (13, 1)]
		PhotoDepthEffect = (1 << 4),
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		SmartAlbumSpatial = (1 << 10),
		VideoStreamed = (1 << 16),
		VideoHighFrameRate = (1 << 17),
		VideoTimelapse = (1 << 18),
		VideoCinematic = (1 << 21),
	}

	// NSUInteger -> PhotosTypes.h
	/// <summary>Indicates whether the Photos app or the user selected an asset as a favorite.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum PHAssetBurstSelectionType : ulong {
		None = 0,
		AutoPick = (1 << 0),
		UserPick = (1 << 1)
	}

	/// <summary>Enumerates the current authorization allowed by the application user.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAuthorizationStatus : long {
		NotDetermined,
		Restricted,
		Denied,
		Authorized,
		[iOS (14, 0)]
		[NoTV]
		[NoMac]
		[MacCatalyst (14, 0)]
		Limited,
	}

	/// <summary>Enumerates types of <see cref="T:Photos.PHAssetResource" /> data.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetResourceType : long {
		Photo = 1,
		Video = 2,
		Audio = 3,
		AlternatePhoto = 4,
		FullSizePhoto = 5,
		FullSizeVideo = 6,
		AdjustmentData = 7,
		AdjustmentBasePhoto = 8,
		[MacCatalyst (13, 1)]
		PairedVideo = 9,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		FullSizePairedVideo = 10,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		AdjustmentBasePairedVideo = 11,
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		AdjustmentBaseVideo = 12,
		[iOS (17, 0), TV (17, 0)]
		[MacCatalyst (17, 0), Mac (14, 0)]
		PhotoProxy = 19,
	}

	/// <summary>Enumerates the means by which an asset entered the Photos library.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetSourceType : ulong {
		None = 0,
		UserLibrary = (1 << 0),
		CloudShared = (1 << 1),
		iTunesSynced = (1 << 2)
	}

	/// <summary>Enumerates Live Photo frame types.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHLivePhotoFrameType : long {
		Photo,
		Video
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum PHAssetPlaybackStyle : long {
		Unsupported = 0,
		Image = 1,
		ImageAnimated = 2,
		LivePhoto = 3,
		Video = 4,
		VideoLooping = 5,
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Native]
	public enum PHProjectTextElementType : long {
		Body = 0,
		Title,
		Subtitle,
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
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

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Native]
	public enum PHProjectSectionType : long {
		Undefined = 0,
		Cover = 1,
		Content = 2,
		Auxiliary = 3,
	}

	[NoMacCatalyst]
	[NoiOS]
	[NoTV]
	[Native ("PHLivePhotoEditingErrorCode")]
	[ErrorDomain ("PHLivePhotoEditingErrorDomain")]
	public enum PHLivePhotoEditingError : long {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PHPhotosError.InternalError' instead.")]
		Unknown,
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PHPhotosError.UserCancelled' instead.")]
		Aborted,
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
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

	[ErrorDomain ("PHPhotosErrorDomain")]
	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PHPhotosError : long {
#if !NET
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'InternalError' instead.")]
		Invalid = -1,
#endif
		InternalError = -1,
		UserCancelled = 3072,
		LibraryVolumeOffline = 3114,
		RelinquishingLibraryBundleToWriter = 3142,
		SwitchingSystemPhotoLibrary = 3143,
		NetworkAccessRequired = 3164,
		NetworkError = 3169,
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
		LibraryInFileProviderSyncRoot = 5423,
		PersistentChangeTokenExpired = 3105,
		PersistentChangeDetailsUnavailable = 3210,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum PHAccessLevel : long {
		AddOnly = 1,
		ReadWrite = 2,
	}

	[TV (16, 0), Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[Native]
	public enum PHObjectType : long {
		Asset = 1,
		AssetCollection = 2,
		CollectionList = 3,
	}
}
