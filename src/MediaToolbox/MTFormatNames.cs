// Copyright 2015 Xamarin Inc.

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using CoreMedia;
using ObjCRuntime;

namespace MediaToolbox {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	static public class MTFormatNames {

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		[DllImport (Constants.MediaToolboxLibrary)]
		static extern /* CFStringRef CM_NULLABLE */ IntPtr MTCopyLocalizedNameForMediaType (
			CMMediaType mediaType);

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		static public string? GetLocalizedName (this CMMediaType mediaType)
		{
			return CFString.FromHandle (MTCopyLocalizedNameForMediaType (mediaType), releaseHandle: true);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		[DllImport (Constants.MediaToolboxLibrary)]
		static extern /* CFStringRef CM_NULLABLE */ IntPtr MTCopyLocalizedNameForMediaSubType (
			CMMediaType mediaType, uint mediaSubType);

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		static public string? GetLocalizedName (this CMMediaType mediaType, uint mediaSubType)
		{
			return CFString.FromHandle (MTCopyLocalizedNameForMediaSubType (mediaType, mediaSubType), releaseHandle: true);
		}
	}
}
