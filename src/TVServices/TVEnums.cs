// Copyright 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace TVServices {

#if NET
	[SupportedOSPlatform ("tvos9.0")]
#else
	[TV (9,0)]
#endif
	[Native]
	public enum TVContentItemImageShape : long {
		None = 0,
		Poster,
		Square,
		Sdtv,
		Hdtv,
		Wide,
		ExtraWide
	}

#if NET
	[SupportedOSPlatform ("tvos9.0")]
#else
	[TV (9,0)]
#endif
	[Native]
	public enum TVTopShelfContentStyle : long {
		Inset = 1,
		Sectioned = 2
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
#else
	[TV (11,0)]
#endif
	[Native]
	[Flags]
	public enum TVContentItemImageTrait : ulong {
		UserInterfaceStyleLight = (1 << 8),
		UserInterfaceStyleDark = (2 << 8),
		ScreenScale1x = (1 << 12),
		ScreenScale2x = (2 << 12),
	}

	static public class TVContentItemImageShapeExtensions {

#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
#else
		[Deprecated (PlatformName.TvOS, 13,0)]
#endif
		[DllImport (Constants.TVServicesLibrary)]
		static extern CGSize TVTopShelfImageSizeForShape (/* TVContentItemImageShape */ nint shape,
			/* TVTopShelfContentStyle */ nint style);

#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
#else
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'TVTopShelfSectionedContent.GetImageSize' or 'TVTopShelfInsetContent.ImageSize' instead.")]
#endif
		static public CGSize GetSize (this TVContentItemImageShape self, TVTopShelfContentStyle style)
		{
			return TVTopShelfImageSizeForShape ((nint) (int) self, (nint) (int) style);
		}
	}
}
