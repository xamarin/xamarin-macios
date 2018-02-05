// Copyright 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using CoreMedia;
using ObjCRuntime;

namespace MediaToolbox {

	static public class MTFormatNames {

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.MediaToolboxLibrary)]
		static extern /* CFStringRef CM_NULLABLE */ IntPtr MTCopyLocalizedNameForMediaType (
			CMMediaType mediaType);

		[iOS (9,0)][Mac (10,11)]
		static public string GetLocalizedName (this CMMediaType mediaType)
		{
			return CFString.FetchString (MTCopyLocalizedNameForMediaType (mediaType), releaseHandle: true);
		}

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.MediaToolboxLibrary)]
		static extern /* CFStringRef CM_NULLABLE */ IntPtr MTCopyLocalizedNameForMediaSubType (
			CMMediaType mediaType, uint mediaSubType);

		[iOS (9,0)][Mac (10,11)]
		static public string GetLocalizedName (this CMMediaType mediaType, uint mediaSubType)
		{
			return CFString.FetchString (MTCopyLocalizedNameForMediaSubType (mediaType, mediaSubType), releaseHandle: true);
		}
	}
}
