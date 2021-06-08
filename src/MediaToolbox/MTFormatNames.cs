// Copyright 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using CoreMedia;
using ObjCRuntime;

namespace MediaToolbox {

	static public class MTFormatNames {

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		[DllImport (Constants.MediaToolboxLibrary)]
		static extern /* CFStringRef CM_NULLABLE */ IntPtr MTCopyLocalizedNameForMediaType (
			CMMediaType mediaType);

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		static public string GetLocalizedName (this CMMediaType mediaType)
		{
			return CFString.FromHandle (MTCopyLocalizedNameForMediaType (mediaType), releaseHandle: true);
		}

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		[DllImport (Constants.MediaToolboxLibrary)]
		static extern /* CFStringRef CM_NULLABLE */ IntPtr MTCopyLocalizedNameForMediaSubType (
			CMMediaType mediaType, uint mediaSubType);

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		static public string GetLocalizedName (this CMMediaType mediaType, uint mediaSubType)
		{
			return CFString.FromHandle (MTCopyLocalizedNameForMediaSubType (mediaType, mediaSubType), releaseHandle: true);
		}
	}
}
