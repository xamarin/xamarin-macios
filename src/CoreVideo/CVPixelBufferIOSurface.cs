// 
// CVPixelBufferIOSurface.cs
//
// Authors: Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2017 Xamarin Inc.
//

#if !WATCH
using System;
using System.Runtime.InteropServices;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace CoreVideo {
	public partial class CVPixelBuffer : CVImageBuffer {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static IntPtr /* IOSurfaceRef */ CVPixelBufferGetIOSurface (
			/* CVPixelBufferRef CV_NULLABLE */ IntPtr pixelBuffer
		);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		public IOSurface.IOSurface? GetIOSurface ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CVPixelBuffer");

			var ret = CVPixelBufferGetIOSurface (Handle);
			if (ret == IntPtr.Zero)
				return null;

			return Runtime.GetINativeObject<IOSurface.IOSurface> (ret, false);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn /* IOSurfaceRef */ CVPixelBufferCreateWithIOSurface (
			/* CFAllocatorRef CV_NULLABLE */ IntPtr allocator,
			/* IOSurfaceRef CV_NONNULL */ IntPtr surface,
			/* CFDictionaryRef CV_NULLABLE */ IntPtr pixelBufferAttributes,
			/* CVPixelBufferRef CV_NULLABLE * CV_NONNULL */ out IntPtr pixelBufferOut
		);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		public static CVPixelBuffer? Create (IOSurface.IOSurface surface, out CVReturn result, CVPixelBufferAttributes? pixelBufferAttributes = null)
		{
			if (surface is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (surface));

			IntPtr pixelBufferPtr;
			result = CVPixelBufferCreateWithIOSurface (
				allocator: IntPtr.Zero,
				surface: surface.Handle,
				pixelBufferAttributes: pixelBufferAttributes?.Dictionary.Handle ?? IntPtr.Zero,
				pixelBufferOut: out pixelBufferPtr
			);

			if (result != CVReturn.Success)
				return null;

			return new CVPixelBuffer (pixelBufferPtr, true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		public static CVPixelBuffer? Create (IOSurface.IOSurface surface, CVPixelBufferAttributes? pixelBufferAttributes = null)
		{
			CVReturn result;
			return Create (surface, out result, pixelBufferAttributes);
		}
	}
}
#endif
