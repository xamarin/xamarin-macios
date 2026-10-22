// 
// CVEnums.cs
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
using CoreFoundation;

#nullable enable

namespace CoreVideo {

	// uint32_t -> CVBuffer.h
	/// <summary>An enumeration whose values specify whether a buffer attachment should or should not propagate.</summary>
	[MacCatalyst (13, 1)]
	public enum CVAttachmentMode : uint {
		/// <summary>To be added.</summary>
		ShouldNotPropagate = 0,
		/// <summary>To be added.</summary>
		ShouldPropagate = 1,
	}

	/// <summary>An enumeration that flags whether a <see cref="CoreVideo.CVPixelBuffer" /> is read-only or not.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	[NativeName ("CVPixelBufferLockFlags")]
	public enum CVPixelBufferLock : ulong {
		/// <summary>To be added.</summary>
		None = 0x00000000,
		/// <summary>To be added.</summary>
		ReadOnly = 0x00000001,
	}

	// int32_t -> CVReturn.h
	/// <summary>Possible status codes for CoreVideo functions.</summary>
	[MacCatalyst (13, 1)]
	public enum CVReturn : int {
		/// <summary>The method executed successfully.</summary>
		Success = 0,
		/// <summary>Internal marker</summary>
		First = -6660,
		/// <summary>Generic error message.</summary>
		Error = First,
		/// <summary>Caller passed an invalid argument.</summary>
		InvalidArgument = -6661,
		/// <summary>Allocation failed.</summary>
		AllocationFailed = -6662,
		/// <summary>The operation is not supported.</summary>
		Unsupported = -6663,
		/// <summary>The specified display is invalid</summary>
		InvalidDisplay = -6670,
		/// <summary>The displauy link is already running.</summary>
		DisplayLinkAlreadyRunning = -6671,
		/// <summary>The display link is not running</summary>
		DisplayLinkNotRunning = -6672,
		/// <summary>There are no callbacks registered for the specified display link.</summary>
		DisplayLinkCallbacksNotSet = -6673,
		/// <summary>The specified pixel format is not valid.</summary>
		InvalidPixelFormat = -6680,
		/// <summary>Invalid buffer size.</summary>
		InvalidSize = -6681,
		/// <summary>The specified pixel buffer attributes are not valid.</summary>
		InvalidPixelBufferAttributes = -6682,
		/// <summary>The pixel format is not compatible with OpenGL.</summary>
		PixelBufferNotOpenGLCompatible = -6683,
		/// <summary>The pixel buffer is not compatible with Metal.</summary>
		PixelBufferNotMetalCompatible = -6684,
		/// <summary>Performing this operation would exceed the allocation threshold.</summary>
		WouldExceedAllocationThreshold = -6689,
		/// <summary>Failure to allocate the pool.</summary>
		PoolAllocationFailed = -6690,
		/// <summary>It is not possible to create a buffer pool with the specified attributes.</summary>
		InvalidPoolAttributes = -6691,
		/// <summary>The method will retry.</summary>
		Retry = -6692,
		/// <summary>Internal marker.</summary>
		Last = -6699,
	}


	// uint64_t -> CVBase.h
	/// <summary>A flagging enumeration. Currently only contains a <c>None</c> value of 0.</summary>
	[MacCatalyst (13, 1)]
	public enum CVOptionFlags : long {
		/// <summary>No options are specified.</summary>
		None = 0,
	}

	/// <summary>An enumeration that can flag whether a <see cref="CoreVideo.CVTime" /> is unknown.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	public enum CVTimeFlags : int {
		/// <summary>The time value is indefinite.</summary>
		IsIndefinite = 1 << 0,
	}

	/// <summary>An enumeration whose values can flag validity of a timestamp or field in a <see cref="CoreVideo.CVTimeStamp" />.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	public enum CVTimeStampFlags : ulong {
		/// <summary>To be added.</summary>
		VideoTimeValid = (1 << 0),
		/// <summary>To be added.</summary>
		HostTimeValid = (1 << 1),
		/// <summary>To be added.</summary>
		SMPTETimeValid = (1 << 2),
		/// <summary>To be added.</summary>
		VideoRefreshPeriodValid = (1 << 3),
		/// <summary>To be added.</summary>
		RateScalarValid = (1 << 4),
		/// <summary>To be added.</summary>
		TopField = (1 << 16),
		/// <summary>To be added.</summary>
		BottomField = (1 << 17),
		/// <summary>To be added.</summary>
		VideoHostTimeValid = (VideoTimeValid | HostTimeValid),
		/// <summary>To be added.</summary>
		IsInterlaced = (TopField | BottomField),
	}

	/// <summary>An enumeration whose values specify the SMPTE time state.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	public enum CVSMPTETimeFlags : uint {
		/// <summary>The full time is valid.</summary>
		Valid = (1 << 0),
		/// <summary>Time is running.</summary>
		Running = (1 << 1),
	}

	/// <summary>An enumeration whose values specify SMPTE timecode types.</summary>
	[MacCatalyst (13, 1)]
	public enum CVSMPTETimeType : uint {
		/// <summary>To be added.</summary>
		Type24 = 0,
		/// <summary>To be added.</summary>
		Type25 = 1,
		/// <summary>To be added.</summary>
		Type30Drop = 2,
		/// <summary>To be added.</summary>
		Type30 = 3,
		/// <summary>To be added.</summary>
		Type2997 = 4,
		/// <summary>To be added.</summary>
		Type2997Drop = 5,
		/// <summary>To be added.</summary>
		Type60 = 6,
		/// <summary>To be added.</summary>
		Type5994 = 7,
	}

	// CVOptionFlags (uint64_t) -> CVPixelBufferPool.h
	/// <summary>Defines an option for <see cref="CoreVideo.CVPixelBufferPool.Flush(CoreVideo.CVPixelBufferPoolFlushFlags)" />.</summary>
	[MacCatalyst (13, 1)]
	public enum CVPixelBufferPoolFlushFlags : ulong {
		/// <summary>Releases all unused buffers from the pool.</summary>
		FlushExcessBuffers = 1,
	}

	[MacCatalyst (13, 1)]
	public enum CVImageBufferAlphaChannelMode {
		[Field ("kCVImageBufferAlphaChannelMode_StraightAlpha")]
		Straight,
		[Field ("kCVImageBufferAlphaChannelMode_PremultipliedAlpha")]
		Premultiplied,
	}
}
