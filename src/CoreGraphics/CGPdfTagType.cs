//
// CGPdfTagType.cs
//
// Author:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2018-2019 Microsoft Corporation
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace CoreGraphics {

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[iOS (13, 0)]
	[TV (13, 0)]
	[Watch (6, 0)]
#endif
	public static class CGPdfTagType_Extensions {

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* const char * _Nullable */ IntPtr CGPDFTagTypeGetName (CGPdfTagType tagType);

		public static string? GetName (this CGPdfTagType self)
		{
			return Marshal.PtrToStringAnsi (CGPDFTagTypeGetName (self));
		}
	}
}
