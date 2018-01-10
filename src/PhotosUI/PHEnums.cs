using System;
using ObjCRuntime;

namespace PhotosUI {

	[Mac (10,12, onlyOn64: true)]
	[TV (10,0)]
	[iOS (9,1)]
	[Native]
	public enum PHLivePhotoViewPlaybackStyle : long
	{
		Undefined = 0,
		Full,
		Hint
	}

#if MONOMAC
	[Mac (10,12, onlyOn64: true)]
	[Native]
	public enum PHLivePhotoViewContentMode : long {
		AspectFit,
		AspectFill,
	}
#else
	[TV (10,0)]
	[iOS (9,1)]
	[Native]
	[Flags] // NS_OPTIONS
	public enum PHLivePhotoBadgeOptions : ulong {
		None = 0,
		OverContent = 1 << 0,
		LiveOff = 1 << 1,
	}
#endif
}
