// Copyright 2015 Xamarin Inc.

#if !COREBUILD

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace TVServices {

	static public class TVContentItemImageShapeExtensions {

		[Deprecated (PlatformName.TvOS, 13,0)]
		[DllImport (Constants.TVServicesLibrary)]
		static extern CGSize TVTopShelfImageSizeForShape (/* TVContentItemImageShape */ nint shape,
			/* TVTopShelfContentStyle */ nint style);

		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'TVTopShelfSectionedContent.GetImageSize' or 'TVTopShelfInsetContent.ImageSize' instead.")]
		static public CGSize GetSize (this TVContentItemImageShape self, TVTopShelfContentStyle style)
		{
			return TVTopShelfImageSizeForShape ((nint) (int) self, (nint) (int) style);
		}
	}
}

#endif
