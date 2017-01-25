using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.Photos
{
	// NSInteger -> PHImageManager.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHImageContentMode : nint {
		AspectFit = 0,
		AspectFill = 1,
		Default = AspectFit
	}

	// NSInteger -> PHImageManager.h
	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Native]
	public enum PHImageRequestOptionsVersion : nint {
		Current = 0,
		Unadjusted,
		Original
	}

	// NSInteger -> PHImageManager.h
	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Native]
	public enum PHImageRequestOptionsDeliveryMode : nint {
		Opportunistic = 0,
		HighQualityFormat = 1,
		FastFormat = 2
	}

	// NSInteger -> PHImageManager.h
	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Native]
	public enum PHImageRequestOptionsResizeMode : nint {
		None = 0,
		Fast,
		Exact
	}

	// NSInteger -> PHImageManager.h
	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Native]
	public enum PHVideoRequestOptionsVersion : nint {
		Current = 0,
		Original
	}

	// NSInteger -> PHImageManager.h
	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Native]
	public enum PHVideoRequestOptionsDeliveryMode : nint {
		Automatic = 0,
		HighQualityFormat = 1,
		MediumQualityFormat = 2,
		FastFormat = 3
	}

	// NSInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHCollectionListType : nint {
		MomentList  = 1,
		Folder      = 2,
		SmartFolder = 3
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHCollectionListSubtype : nint {
		MomentListCluster = 1,
		MomentListYear = 2,

		RegularFolder = 100,

		SmartFolderEvents = 200,
		SmartFolderFaces = 201,

#if !XAMCORE_3_0
		// this was added in the wrong enum type (ref bug #40019)
		[Obsolete ("Incorrect value (exists in PHAssetCollectionSubtype)")]
		SmartAlbumSelfPortraits = 210,
		[Obsolete ("Incorrect value (exists in PHAssetCollectionSubtype)")]
		SmartAlbumScreenshots = 211,
#endif
#if XAMCORE_2_0
		Any = Int64.MaxValue,
#else
		Any = Int32.MaxValue,
#endif
	}

	// NSUInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHCollectionEditOperation : nint {
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
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHAssetCollectionType : nint {
		Album      = 1,
		SmartAlbum = 2,
		Moment     = 3
	}

	// NSInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHAssetCollectionSubtype : nint {
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
		[iOS (9,0)]
		SmartAlbumSelfPortraits = 210,
		[iOS (9,0)]
		SmartAlbumScreenshots   = 211,
		[iOS (10,2), TV (10,1)]
		SmartAlbumDepthEffect   = 212,
		[iOS (10,3), TV (10,2)]
		SmartAlbumLivePhotos = 213,
#if XAMCORE_2_0
		Any           = Int64.MaxValue
#else
		Any           = Int32.MaxValue
#endif
	}

	// NSUInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHAssetEditOperation : nint {
		None       = 0,
		Delete     = 1,
		Content    = 2,
		Properties = 3
	}

	// NSInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHAssetMediaType : nint {
		Unknown = 0,
		Image   = 1,
		Video   = 2,
		Audio   = 3
	}

	// NSUInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	[Flags]
	public enum PHAssetMediaSubtype : nuint {
		None               = 0,
		PhotoPanorama      = (1 << 0),
		PhotoHDR           = (1 << 1),
		Screenshot         = (1 << 2),
		PhotoLive          = (1 << 3),
		[iOS (10,2), TV (10,1)]
		PhotoDepthEffect   = (1 << 4),
		VideoStreamed      = (1 << 16),
		VideoHighFrameRate = (1 << 17),
		VideoTimelapse     = (1 << 18),
			
	}

	// NSUInteger -> PhotosTypes.h
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	[Flags]
	public enum PHAssetBurstSelectionType : nuint {
		None     = 0,
		AutoPick = (1 << 0),
		UserPick = (1 << 1)
	}

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Native]
	public enum PHAuthorizationStatus : nint {
		NotDetermined, Restricted, Denied, Authorized
	}

	[iOS (9,0)]
	[Mac (10,12, onlyOn64 : true)]
	[TV (10,0)]
	[Native]
	public enum PHAssetResourceType : nint
	{
		Photo = 1,
		Video = 2,
		Audio = 3,
		AlternatePhoto = 4,
		FullSizePhoto = 5,
		FullSizeVideo = 6,
		AdjustmentData = 7,
		AdjustmentBasePhoto = 8,
		PairedVideo = 9
	}

	[iOS (9,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHAssetSourceType : nuint
	{
		None = 0,
		UserLibrary = (1 << 0),
		CloudShared = (1 << 1),
		iTunesSynced = (1 << 2)
	}

	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Native]
	public enum PHLivePhotoFrameType : nint {
		Photo,
		Video
	}
}
