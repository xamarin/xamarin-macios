// Copyright 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.TVServices {

	[TV (9,0)]
	[Native]
	public enum TVContentItemImageShape : nint {
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
	public enum TVTopShelfContentStyle : nint {
		Inset = 1,
		Sectioned = 2
	}

	static public class TVContentItemImageShapeExtensions {

		[DllImport (Constants.TVServicesLibrary)]
		static extern CGSize TVTopShelfImageSizeForShape (/* TVContentItemImageShape */ nint shape,
			/* TVTopShelfContentStyle */ nint style);

		static public CGSize GetSize (this TVContentItemImageShape self, TVTopShelfContentStyle style)
		{
			return TVTopShelfImageSizeForShape ((nint) (int) self, (nint) (int) style);
		}
	}
}
