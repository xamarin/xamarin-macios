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
}
