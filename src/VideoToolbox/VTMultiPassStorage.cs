// 
// VTMultiPassStorage.cs: VideoToolbox VTMultiPassStorage class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;

namespace VideoToolbox {

#if NET
	[SupportedOSPlatform ("tvos10.2")]
#else
	[Mac (10,10), iOS (8,0), TV (10,2)]
#endif
	public class VTMultiPassStorage : NativeObject {
		bool closed;
		VTStatus closedStatus;

		protected internal VTMultiPassStorage (IntPtr handle)
			: base (handle, false)
		{
		}

		[Preserve (Conditional=true)]
		internal VTMultiPassStorage (IntPtr handle, bool owns)
			: base (handle, false)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero)
				Close ();
			base.Dispose (disposing);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* OSStatus */ VTStatus VTMultiPassStorageCreate (
			/* CFAllocatorRef */IntPtr allocator, /* can be null */
			/* CFURLRef */ IntPtr fileUrl, /* can be null */
			/* CMTimeRange */ CMTimeRange timeRange, /* can be kCMTimeRangeInvalid */
			/* CFDictionaryRef */ IntPtr options, /* can be null */
			/* VTMultiPassStorageRef */ out IntPtr multiPassStorageOut);

		// Convenience method taking a strong dictionary
		public static VTMultiPassStorage? Create (
			VTMultiPassStorageCreationOptions? options,
			NSUrl? fileUrl = null,
			CMTimeRange? timeRange = null)
		{
			return Create (fileUrl, timeRange, options?.Dictionary);
		}

		public static VTMultiPassStorage? Create (
			NSUrl? fileUrl = null,
			CMTimeRange? timeRange = null,
			NSDictionary? options = null)
		{
			var status = VTMultiPassStorageCreate (
				IntPtr.Zero,
				fileUrl.GetHandle (),
				timeRange ?? CMTimeRange.InvalidRange, 
				options.GetHandle (),
				out var ret);

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
			closedStatus = VTMultiPassStorageClose (Handle);
			closed = true;
			return closedStatus;
		}
	}
}
