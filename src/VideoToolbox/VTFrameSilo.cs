// 
// VTFrameSilo.cs: VideoToolbox VTFrameSilo class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;

namespace VideoToolbox {
	[Mac (10,10), iOS (8,0), TV (10,2)]
	public class VTFrameSilo : INativeObject, IDisposable {
		IntPtr handle;
		GCHandle callbackHandle;

		/* invoked by marshallers */
		protected internal VTFrameSilo (IntPtr handle)
		{
			this.handle = handle;
			CFObject.CFRetain (this.handle);
		}

		public IntPtr Handle {
			get {return handle; }
		}

		[Preserve (Conditional=true)]
		internal VTFrameSilo (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (this.handle);
		}

		~VTFrameSilo ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (callbackHandle.IsAllocated)
				callbackHandle.Free();

			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloCreate (
			/* CFAllocatorRef */ IntPtr allocator, /* can be null */
			/* CFURLRef */ IntPtr fileUrl, /* can be null */
			/* CMTimeRange */ CMTimeRange timeRange, /* can be kCMTimeRangeInvalid */
			/* CFDictionaryRef */ IntPtr options, /* Reserved, always null */
			/* VTFrameSiloRef */ out IntPtr siloOut);

		public static VTFrameSilo Create (NSUrl fileUrl = null, CMTimeRange? timeRange = null)
		{
			IntPtr ret;
			var status = VTFrameSiloCreate (
				IntPtr.Zero,
				fileUrl == null ? IntPtr.Zero : fileUrl.Handle, 
				timeRange ?? CMTimeRange.InvalidRange, 
				IntPtr.Zero, 
				out ret);

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
			if (sampleBuffer == null)
				throw new ArgumentNullException ("sampleBuffer");

			var status = VTFrameSiloAddSampleBuffer (handle, sampleBuffer.Handle);
			return status;
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloSetTimeRangesForNextPass (
			/* VTFrameSiloRef */ IntPtr silo,
			/* CMItemCount */ nint timeRangeCount,
			/* const CMTimeRange * */ IntPtr timeRangeArray);

		public unsafe VTStatus SetTimeRangesForNextPass (CMTimeRange[] ranges)
		{
			if (ranges == null)
				throw new ArgumentNullException ("ranges");

			if (ranges.Length > 0)
				fixed (CMTimeRange *first = &ranges [0]) {
					return VTFrameSiloSetTimeRangesForNextPass (handle, ranges.Length, (IntPtr)first);
				}
			else
				return VTFrameSiloSetTimeRangesForNextPass (handle, ranges.Length, IntPtr.Zero);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloGetProgressOfCurrentPass (
			/* VTFrameSiloRef */ IntPtr silo,
			/* Float32* */ out float progressOut);

		public VTStatus GetProgressOfCurrentPass (out float progress)
		{
			return VTFrameSiloGetProgressOfCurrentPass (handle, out progress);
		}

		delegate VTStatus EachSampleBufferCallback (/* void* */ IntPtr callbackInfo, /* CMSampleBufferRef */ IntPtr sampleBufferPtr);

		static EachSampleBufferCallback static_EachSampleBufferCallback = new EachSampleBufferCallback (BufferCallback);

#if !MONOMAC
		[MonoPInvokeCallback (typeof (EachSampleBufferCallback))]
#endif
		static VTStatus BufferCallback (IntPtr callbackInfo, IntPtr sampleBufferPtr)
		{
			var gch = GCHandle.FromIntPtr (callbackInfo);
			var func = (Func<CMSampleBuffer, VTStatus>) gch.Target;
			var sampleBuffer = new CMSampleBuffer (sampleBufferPtr, false);
			return func (sampleBuffer);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTFrameSiloCallFunctionForEachSampleBuffer (
			/* VTFrameSiloRef */ IntPtr silo,
			/* CMTimeRange */ CMTimeRange timeRange, // CMTimeRange.Invalid retrieves all sample buffers
			/* void* */ IntPtr callbackInfo,
			/* */ EachSampleBufferCallback callback);

		public VTStatus ForEach (Func<CMSampleBuffer, VTStatus> callback, CMTimeRange? range = null)
		{
			callbackHandle = GCHandle.Alloc (callback);
			var foreachResult = VTFrameSiloCallFunctionForEachSampleBuffer (handle, range ?? CMTimeRange.InvalidRange, GCHandle.ToIntPtr (callbackHandle), static_EachSampleBufferCallback);
			callbackHandle.Free ();
			return foreachResult;
		}
	}
}

