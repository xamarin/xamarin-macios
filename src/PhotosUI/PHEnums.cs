using System;
using XamCore.ObjCRuntime;

namespace XamCore.PhotosUI {

	[Mac (10,12)]
	[TV (10,0)]
	[iOS (9,1)]
	[Native]
	public enum PHLivePhotoViewPlaybackStyle : nint
	{
		Undefined = 0,
		Full,
		Hint
	}

#if MONOMAC
	[Mac (10,12)]
	[Native]
	public enum PHLivePhotoViewContentMode : nint {
		AspectFit,
		AspectFill,
	}
#else
	[TV (10,0)]
	[iOS (9,1)]
	[Native]
	[Flags] // NS_OPTIONS
	public enum PHLivePhotoBadgeOptions : nuint {
		None = 0,
		OverContent = 1 << 0,
		LiveOff = 1 << 1,
	}
#endif
}
