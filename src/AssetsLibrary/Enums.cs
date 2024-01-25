using System;

using ObjCRuntime;

#nullable enable

namespace AssetsLibrary {

	// NSInteger -> ALAssetsLibrary.h
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[Native]
	public enum ALAssetOrientation : long {
		Up,
		Down,
		Left,
		Right,
		UpMirrored,
		DownMirrored,
		LeftMirrored,
		RightMirrored,
	}

	// NSUInteger -> ALAssetsLibrary.h
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[Native]
	[Flags]
	public enum ALAssetsGroupType : ulong {
		Library = (1 << 0),
		Album = (1 << 1),
		Event = (1 << 2),
		Faces = (1 << 3),
		SavedPhotos = (1 << 4),
		GroupPhotoStream = (1 << 5),
		All = 0xFFFFFFFF,
	}

	// untyped enum -> ALAssetsLibrary.h
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[ErrorDomain ("ALAssetsLibraryErrorDomain")]
	public enum ALAssetsError {
		UnknownError = -1,
		WriteFailedError = -3300,
		WriteBusyError = -3301,
		WriteInvalidDataError = -3302,
		WriteIncompatibleDataError = -3303,
		WriteDataEncodingError = -3304,
		WriteDiskSpaceError = -3305,
		DataUnavailableError = -3310,
		AccessUserDeniedError = -3311,
		AccessGloballyDeniedError = -3312,
	}

	// NSInteger -> ALAssetsLibrary.h
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[Native]
	public enum ALAuthorizationStatus : long {
		NotDetermined,
		Restricted,
		Denied,
		Authorized
	}

	// internally used (not exposed by ObjC)
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	public enum ALAssetType {
		Video,
		Photo,
		Unknown,
	}
}
