// Copyright 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace TVServices {

	[TV (9,0)]
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

	[TV (9,0)]
	[Native]
	public enum TVTopShelfContentStyle : long {
		Inset = 1,
		Sectioned = 2
	}

	[TV (11,0)]
	[Native]
	[Flags]
	public enum TVContentItemImageTrait : ulong {
		UserInterfaceStyleLight = (1 << 8),
		UserInterfaceStyleDark = (2 << 8),
		ScreenScale1x = (1 << 12),
		ScreenScale2x = (2 << 12),
	}
}
