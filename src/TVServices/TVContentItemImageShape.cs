// Copyright 2015 Xamarin Inc.

#nullable enable

#if !COREBUILD

using System;
using System.Runtime.InteropServices;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

namespace TVServices {

	static public class TVContentItemImageShapeExtensions {

#if NET
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos13.0")]
#else
		[Deprecated (PlatformName.TvOS, 13, 0)]
#endif
		[DllImport (Constants.TVServicesLibrary)]
		static extern CGSize TVTopShelfImageSizeForShape (/* TVContentItemImageShape */ nint shape,
			/* TVTopShelfContentStyle */ nint style);

#if NET
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos13.0")]
#else
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TVTopShelfSectionedContent.GetImageSize' or 'TVTopShelfInsetContent.ImageSize' instead.")]
#endif
		static public CGSize GetSize (this TVContentItemImageShape self, TVTopShelfContentStyle style)
		{
			return TVTopShelfImageSizeForShape ((nint) (int) self, (nint) (int) style);
		}
	}
}

#endif
