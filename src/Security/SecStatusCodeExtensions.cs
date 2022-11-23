// 
// SecStatusCodeExtensions.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
// 
// Copyright 2018 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;

namespace Security {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class SecStatusCodeExtensions {

#if NET
		[SupportedOSPlatform ("ios11.3")]
		[SupportedOSPlatform ("tvos11.3")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (11, 3)]
		[TV (11, 3)]
		[Watch (4, 3)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static /* CFStringRef */ IntPtr SecCopyErrorMessageString (
			/* OSStatus */ SecStatusCode status,
			/* void * */ IntPtr reserved); /* always null */

#if NET
		[SupportedOSPlatform ("ios11.3")]
		[SupportedOSPlatform ("tvos11.3")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (11, 3)] // Since Mac 10,3
		[TV (11, 3)]
		[Watch (4, 3)]
#endif
		public static string GetStatusDescription (this SecStatusCode status)
		{
			var ret = SecCopyErrorMessageString (status, IntPtr.Zero);
			return Runtime.GetNSObject<NSString> (ret, owns: true);
		}
	}
}
