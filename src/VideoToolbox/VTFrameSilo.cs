// 
// VTFrameSilo.cs: VideoToolbox VTFrameSilo class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace VideoToolbox {

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public class VTFrameSilo : NativeObject {
#if !NET
		protected internal VTFrameSilo (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal VTFrameSilo (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloCreate (
			/* CFAllocatorRef */ IntPtr allocator, /* can be null */
			/* CFURLRef */ IntPtr fileUrl, /* can be null */
			/* CMTimeRange */ CMTimeRange timeRange, /* can be kCMTimeRangeInvalid */
			/* CFDictionaryRef */ IntPtr options, /* Reserved, always null */
			/* VTFrameSiloRef */ out IntPtr siloOut);

		public static VTFrameSilo? Create (NSUrl? fileUrl = null, CMTimeRange? timeRange = null)
		{
			var status = VTFrameSiloCreate (
				IntPtr.Zero,
				fileUrl.GetHandle (),
				timeRange ?? CMTimeRange.InvalidRange,
				IntPtr.Zero,
				out var ret);

			if (status != VTStatus.Ok)
				return null;

			return new VTFrameSilo (ret, true);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloAddSampleBuffer (
			/* VTFrameSiloRef */ IntPtr silo,
			/* CMSampleBufferRef */ IntPtr sampleBuffer);

		public VTStatus AddSampleBuffer (CMSampleBuffer sampleBuffer)
		{
			if (sampleBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sampleBuffer));

			return VTFrameSiloAddSampleBuffer (Handle, sampleBuffer.Handle);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloSetTimeRangesForNextPass (
			/* VTFrameSiloRef */ IntPtr silo,
			/* CMItemCount */ nint timeRangeCount,
			/* const CMTimeRange * */ IntPtr timeRangeArray);

		public unsafe VTStatus SetTimeRangesForNextPass (CMTimeRange [] ranges)
		{
			if (ranges is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ranges));

			if (ranges.Length > 0)
				fixed (CMTimeRange* first = &ranges [0]) {
					return VTFrameSiloSetTimeRangesForNextPass (Handle, ranges.Length, (IntPtr) first);
				}
			else
				return VTFrameSiloSetTimeRangesForNextPass (Handle, ranges.Length, IntPtr.Zero);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloGetProgressOfCurrentPass (
			/* VTFrameSiloRef */ IntPtr silo,
			/* Float32* */ out float progressOut);

		public VTStatus GetProgressOfCurrentPass (out float progress)
		{
			return VTFrameSiloGetProgressOfCurrentPass (Handle, out progress);
		}

#if !NET
		delegate VTStatus EachSampleBufferCallback (/* void* */ IntPtr callbackInfo, /* CMSampleBufferRef */ IntPtr sampleBufferPtr);

		static EachSampleBufferCallback static_EachSampleBufferCallback = new EachSampleBufferCallback (BufferCallback);
#endif

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (EachSampleBufferCallback))]
#endif
#endif
		static VTStatus BufferCallback (IntPtr callbackInfo, IntPtr sampleBufferPtr)
		{
			var gch = GCHandle.FromIntPtr (callbackInfo);
			var func = gch.Target as Func<CMSampleBuffer, VTStatus>;
			if (func is null)
				return (VTStatus) 1; // return non-zero to abort iteration early.
			var sampleBuffer = new CMSampleBuffer (sampleBufferPtr, false);
			return func (sampleBuffer);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe extern static /* OSStatus */ VTStatus VTFrameSiloCallFunctionForEachSampleBuffer (
			/* VTFrameSiloRef */ IntPtr silo,
			/* CMTimeRange */ CMTimeRange timeRange, // CMTimeRange.Invalid retrieves all sample buffers
			/* void* */ IntPtr callbackInfo,
#if NET
			/* */ delegate* unmanaged<IntPtr, IntPtr, VTStatus> callback);
#else
			/* */ EachSampleBufferCallback callback);
#endif

		public unsafe VTStatus ForEach (Func<CMSampleBuffer, VTStatus> callback, CMTimeRange? range = null)
		{
			var callbackHandle = GCHandle.Alloc (callback);
#if NET
			var foreachResult = VTFrameSiloCallFunctionForEachSampleBuffer (Handle, range ?? CMTimeRange.InvalidRange, GCHandle.ToIntPtr (callbackHandle), &BufferCallback);
#else
			var foreachResult = VTFrameSiloCallFunctionForEachSampleBuffer (Handle, range ?? CMTimeRange.InvalidRange, GCHandle.ToIntPtr (callbackHandle), static_EachSampleBufferCallback);
#endif
			callbackHandle.Free ();
			return foreachResult;
		}
	}
}
