// 
// VTMultiPassStorage.cs: VideoToolbox VTMultiPassStorage class
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
	public class VTMultiPassStorage : INativeObject, IDisposable {
		IntPtr handle;
		bool closed;
		VTStatus closedStatus;

		/* invoked by marshallers */
		protected internal VTMultiPassStorage (IntPtr handle)
		{
			this.handle = handle;
			CFObject.CFRetain (this.handle);
		}

		public IntPtr Handle {
			get {return handle; }
		}

		[Preserve (Conditional=true)]
		internal VTMultiPassStorage (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (this.handle);
		}

		~VTMultiPassStorage ()
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
			if (handle != IntPtr.Zero){
				if (!closed)
					Close ();
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTMultiPassStorageCreate (
			/* CFAllocatorRef */IntPtr allocator, /* can be null */
			/* CFURLRef */ IntPtr fileUrl, /* can be null */
			/* CMTimeRange */ CMTimeRange timeRange, /* can be kCMTimeRangeInvalid */
			/* CFDictionaryRef */ IntPtr options, /* can be null */
			/* VTMultiPassStorageRef */ out IntPtr multiPassStorageOut);

		// Convenience method taking a strong dictionary
		public static VTMultiPassStorage Create (
			VTMultiPassStorageCreationOptions options,
			NSUrl fileUrl = null, 
			CMTimeRange? timeRange = null)
		{
			return Create (fileUrl, timeRange, options != null ? options.Dictionary : null); 
		}

		public static VTMultiPassStorage Create (
			NSUrl fileUrl = null, 
			CMTimeRange? timeRange = null, 
			NSDictionary options = null)
		{
			IntPtr ret;
			var status = VTMultiPassStorageCreate (
				IntPtr.Zero,
				fileUrl == null ? IntPtr.Zero : fileUrl.Handle, 
				timeRange ?? CMTimeRange.InvalidRange, 
				options == null ? IntPtr.Zero : options.Handle, 
				out ret);

			if (status != VTStatus.Ok)
				return null;

			return new VTMultiPassStorage (ret, true);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTMultiPassStorageClose (/* VTMultiPassStorage */ IntPtr multiPassStorage);

		public VTStatus Close ()
		{
			if (closed)
				return closedStatus;
			closedStatus = VTMultiPassStorageClose (handle);
			closed = true;
			return closedStatus;
		}
	}
}

