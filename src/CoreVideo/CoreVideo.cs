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
using ObjCRuntime;

namespace CoreVideo {

	// uint32_t -> CVBuffer.h
	[Watch (4,0)]
	public enum CVAttachmentMode : uint {
		ShouldNotPropagate    = 0,
		ShouldPropagate       = 1,
	}

	[Flags]
	[Watch (4,0)]
#if XAMCORE_4_0
	public enum CVPixelBufferLock : ulong {
#else
	// before iOS10 beta 2 this was an untyped enum -> CVPixelBuffer.h
	// note: used as a CVOptionFlags uint64_t (CVBase.h) in the API
	public enum CVPixelBufferLock : uint {
#endif
		None = 0x00000000,
		ReadOnly = 0x00000001,
	}	

	// CVPixelBuffer.h
	[Watch (4,0)]
	public struct CVPlanarComponentInfo {
		public /* int32_t */ int Offset;
		public /* uint32_t */ uint RowBytes;
	}

	// CVPixelBuffer.h
	[Watch (4,0)]
	public struct CVPlanarPixelBufferInfo {
		public CVPlanarComponentInfo[] ComponentInfo;
	}

	// CVPixelBuffer.h
	[Watch (4,0)]
	public struct CVPlanarPixelBufferInfo_YCbCrPlanar {
		public CVPlanarComponentInfo ComponentInfoY;
		public CVPlanarComponentInfo ComponentInfoCb;
		public CVPlanarComponentInfo ComponentInfoCr;
	}

	[Watch (4,0)]
	public struct CVPlanarPixelBufferInfo_YCbCrBiPlanar {
		public CVPlanarComponentInfo ComponentInfoY;
		public CVPlanarComponentInfo ComponentInfoCbCr;
	}

	// int32_t -> CVReturn.h
	[Watch (4,0)]
	public enum CVReturn : int {
		Success = 0,
		First = -6660,
		Error = First,
		InvalidArgument = -6661,
		AllocationFailed = -6662,
		Unsupported = -6663,
		InvalidDisplay = -6670,
		DisplayLinkAlreadyRunning = -6671,
		DisplayLinkNotRunning = -6672,
		DisplayLinkCallbacksNotSet = -6673,
		InvalidPixelFormat = -6680,
		InvalidSize = -6681,
		InvalidPixelBufferAttributes = -6682,
		PixelBufferNotOpenGLCompatible = -6683,
		PixelBufferNotMetalCompatible = -6684,
		WouldExceedAllocationThreshold = -6689,
		PoolAllocationFailed = -6690,
		InvalidPoolAttributes = -6691,
		Retry = -6692,
		Last = -6699,
	}


	// uint64_t -> CVBase.h
	[Watch (4,0)]
	public enum CVOptionFlags : long {
		None = 0,
	}

	[Watch (4,0)]
	public struct CVTimeStamp {
		public UInt32		Version;
		public Int32 		VideoTimeScale;
		public Int64 		VideoTime;
		public UInt64 		HostTime;
		public double 		RateScalar;
		public Int64 		VideoRefreshPeriod;
		public CVSMPTETime 	SMPTETime;
		public UInt64 		Flags;
		public UInt64 		Reserved;
	}
        
	[Watch (4,0)]
	public struct CVSMPTETime {
		public Int16	Subframes;
		public Int16	SubframeDivisor;
		public UInt32	Counter;
		public UInt32	Type;
		public UInt32	Flags;
		public Int16	Hours;
		public Int16	Minutes;
		public Int16	Seconds;
		public Int16	Frames;
	}

	[Flags]
	[Watch (4,0)]
	public enum CVTimeFlags : int {
		IsIndefinite = 1 << 0
	}

	[Flags]
	[Watch (4,0)]
#if XAMCORE_2_0
	public enum CVTimeStampFlags : ulong {
#else
	public enum CVTimeStampFlags {
#endif
		VideoTimeValid              = (1 << 0),
		HostTimeValid               = (1 << 1),
		SMPTETimeValid              = (1 << 2),
		VideoRefreshPeriodValid     = (1 << 3),
		RateScalarValid             = (1 << 4),
		TopField                    = (1 << 16),
		BottomField                 = (1 << 17),
		VideoHostTimeValid          = (VideoTimeValid | HostTimeValid),
		IsInterlaced                = (TopField | BottomField)
	}	

	[Flags]
	[Watch (4,0)]
	public enum CVSMPTETimeFlags : uint {
		Valid     = (1 << 0),
		Running   = (1 << 1)
	}

	[Watch (4,0)]
	public enum CVSMPTETimeType : uint {
		Type24        = 0,
		Type25        = 1,
		Type30Drop    = 2,
		Type30        = 3,
		Type2997      = 4,
		Type2997Drop  = 5,
		Type60        = 6,
		Type5994      = 7
	}

	[Watch (4,0)]
	public struct CVFillExtendedPixelsCallBackData {
		public nint /* CFIndex */ Version;
		public CVFillExtendedPixelsCallBack FillCallBack;
		public IntPtr UserInfo;
	} 

	[Watch (4,0)]
	public delegate bool CVFillExtendedPixelsCallBack (IntPtr pixelBuffer, IntPtr refCon);

	// CVOptionFlags (uint64_t) -> CVPixelBufferPool.h
	[Watch (4,0)]
	[iOS (9,0)][Mac (10,11)]
	public enum CVPixelBufferPoolFlushFlags : ulong {
		FlushExcessBuffers = 1,
	}
}
