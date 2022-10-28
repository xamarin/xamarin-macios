using System;
using Foundation;
using ObjCRuntime;

namespace PhotosUI {

	[Mac (10,12)]
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
	[NoiOS][NoTV][NoMacCatalyst]
	[Mac (10,12)]
	[Native]
	public enum PHLivePhotoViewContentMode : long {
		AspectFit,
		AspectFill,
	}
#else
	[NoMac]
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
	[NoiOS][NoTV][NoWatch]
	[Mac (10,14)]
	public enum PHProjectCategory {
		[Field ("PHProjectCategoryBook")]
		Book,
		[Field ("PHProjectCategoryCalendar")]
		Calendar,
		[Field ("PHProjectCategoryCard")]
		Card,
		[Field ("PHProjectCategoryPrints")]
		Prints,
		[Field ("PHProjectCategorySlideshow")]
		Slideshow,
		[Field ("PHProjectCategoryWallDecor")]
		WallDecor,
		[Field ("PHProjectCategoryOther")]
		Other,
		[Mac (10,14,2)]
		[Field ("PHProjectCategoryUndefined")]
		Undefined,
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14,0)]
	[Native]
	public enum PHPickerConfigurationAssetRepresentationMode : long
	{
		Automatic = 0,
		Current = 1,
		Compatible = 2,
	}
}
