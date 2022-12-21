// 
// VTPixelRotationSession.cs: VideoTools Pixel Rotation Session class 
//
// Authors: 
//    Israel Soto (issoto@microsoft.com)
// 
// Copyright 2022 Microsoft Corporation.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;
using CoreVideo;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace VideoToolbox {

#if NET
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
	[SupportedOSPlatform ("watchos9.0")]
	[SupportedOSPlatform ("tvos16.0")]
#else
	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
#endif
	public class VTPixelRotationSession : VTSession {

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* CFTypeID */ nint VTPixelRotationSessionGetTypeID ();
		public static nint GetTypeID () => VTPixelRotationSessionGetTypeID ();

#if !NET
		/* invoked by marshallers */
		protected internal VTPixelRotationSession (NativeHandle handle) : base (handle)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal VTPixelRotationSession (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static void VTPixelRotationSessionInvalidate (/* VTPixelRotationSessionRef */ IntPtr session);

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero)
				VTPixelRotationSessionInvalidate (Handle);

			base.Dispose (disposing);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe extern static VTStatus VTPixelRotationSessionCreate (
			/* CFAllocatorRef */ IntPtr allocator, /* can be null */
			/* VTPixelRotationSessionRef* */ out IntPtr pixelRotationSessionOut);

		public static VTPixelRotationSession? Create () => Create (null);

		public static VTPixelRotationSession? Create (CFAllocator? allocator)
		{
			var result = VTPixelRotationSessionCreate (allocator.GetHandle (), out var ret);

			if (result == VTStatus.Ok && ret != IntPtr.Zero)
				return new VTPixelRotationSession (ret, true);

			return null;
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTPixelRotationSessionRotateImage (
			/* VTPixelRotationSessionRef */ IntPtr session,
			/* CVPixelBuffer */ IntPtr sourceBuffer,
			/* CVPixelBuffer */ IntPtr destinationBuffer);

		public VTStatus RotateImage (CVPixelBuffer sourceBuffer, CVPixelBuffer destinationBuffer)
		{
			if (sourceBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceBuffer));

			if (destinationBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destinationBuffer));

			return VTPixelRotationSessionRotateImage (GetCheckedHandle (), sourceBuffer.Handle, destinationBuffer.Handle);
		}

		public VTStatus SetRotationProperties (VTPixelRotationProperties options)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));

			return VTSessionSetProperties (Handle, options.Dictionary.Handle);
		}
	}
}
