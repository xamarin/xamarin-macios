// 
// CoreVideo.cs
//
// Authors: Mono Team
//     
// Copyright 2011 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreVideo {

	// CVPixelBuffer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVPlanarComponentInfo {
		public /* int32_t */ int Offset;
		public /* uint32_t */ uint RowBytes;
	}

	// CVPixelBuffer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVPlanarPixelBufferInfo {
		public CVPlanarComponentInfo [] ComponentInfo;
	}

	// CVPixelBuffer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVPlanarPixelBufferInfo_YCbCrPlanar {
		public CVPlanarComponentInfo ComponentInfoY;
		public CVPlanarComponentInfo ComponentInfoCb;
		public CVPlanarComponentInfo ComponentInfoCr;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVPlanarPixelBufferInfo_YCbCrBiPlanar {
		public CVPlanarComponentInfo ComponentInfoY;
		public CVPlanarComponentInfo ComponentInfoCbCr;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVTimeStamp {
		public UInt32 Version;
		public Int32 VideoTimeScale;
		public Int64 VideoTime;
		public UInt64 HostTime;
		public double RateScalar;
		public Int64 VideoRefreshPeriod;
		public CVSMPTETime SMPTETime;
		public UInt64 Flags;
		public UInt64 Reserved;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVSMPTETime {
		public Int16 Subframes;
		public Int16 SubframeDivisor;
		public UInt32 Counter;
		public UInt32 Type;
		public UInt32 Flags;
		public Int16 Hours;
		public Int16 Minutes;
		public Int16 Seconds;
		public Int16 Frames;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct CVFillExtendedPixelsCallBackData {
		public nint /* CFIndex */ Version;
		public CVFillExtendedPixelsCallBack FillCallBack;
		public IntPtr UserInfo;
	}

	public delegate bool CVFillExtendedPixelsCallBack (IntPtr pixelBuffer, IntPtr refCon);
}
