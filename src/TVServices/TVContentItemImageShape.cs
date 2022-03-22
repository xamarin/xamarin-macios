// Copyright 2015 Xamarin Inc.

#if !COREBUILD

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace TVServices {

	static public class TVContentItemImageShapeExtensions {

#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.TvOS, 13,0)]
#endif
		[DllImport (Constants.TVServicesLibrary)]
		static extern CGSize TVTopShelfImageSizeForShape (/* TVContentItemImageShape */ nint shape,
			/* TVTopShelfContentStyle */ nint style);

#if NET
		[UnsupportedOSPlatform ("tvos13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0 use 'TVTopShelfSectionedContent.GetImageSize' or 'TVTopShelfInsetContent.ImageSize' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'TVTopShelfSectionedContent.GetImageSize' or 'TVTopShelfInsetContent.ImageSize' instead.")]
#endif
		static public CGSize GetSize (this TVContentItemImageShape self, TVTopShelfContentStyle style)
		{
			return TVTopShelfImageSizeForShape ((nint) (int) self, (nint) (int) style);
		}
	}
}

#endif
