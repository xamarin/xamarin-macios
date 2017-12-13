// 
// CVPixelBufferIOSurface.cs
//
// Authors: Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2017 Xamarin Inc.
//

#if XAMCORE_2_0 && !WATCH
using System;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.CoreVideo {
	public partial class CVPixelBuffer : CVImageBuffer {

		[iOS (11,0), Mac (10,13, onlyOn64:true), TV (11,0), NoWatch]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static IntPtr /* IOSurfaceRef */ CVPixelBufferGetIOSurface (
			/* CVPixelBufferRef CV_NULLABLE */ IntPtr pixelBuffer
		);

		[iOS (11,0), Mac (10,13, onlyOn64:true), TV (11,0), NoWatch]
		public XamCore.IOSurface.IOSurface GetIOSurface ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CVPixelBuffer");

			var ret = CVPixelBufferGetIOSurface (Handle);
			if (ret == IntPtr.Zero)
				return null;

			return Runtime.GetINativeObject <XamCore.IOSurface.IOSurface> (ret, false);
		}

		[iOS (11,0), Mac (10,13, onlyOn64:true), TV (11,0), NoWatch]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn /* IOSurfaceRef */ CVPixelBufferCreateWithIOSurface (
			/* CFAllocatorRef CV_NULLABLE */ IntPtr allocator,
			/* IOSurfaceRef CV_NONNULL */ IntPtr surface,
			/* CFDictionaryRef CV_NULLABLE */ IntPtr pixelBufferAttributes,
			/* CVPixelBufferRef CV_NULLABLE * CV_NONNULL */ out IntPtr pixelBufferOut
		);

		[iOS (11,0), Mac (10,13, onlyOn64:true), TV (11,0), NoWatch]
		public static CVPixelBuffer Create (XamCore.IOSurface.IOSurface surface, out CVReturn result, CVPixelBufferAttributes pixelBufferAttributes = null)
		{
			if (surface == null)
				throw new ArgumentNullException (nameof (surface));

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

		[iOS (11,0), Mac (10,13, onlyOn64:true), TV (11,0), NoWatch]
		public static CVPixelBuffer Create (XamCore.IOSurface.IOSurface surface, CVPixelBufferAttributes pixelBufferAttributes = null)
		{
			CVReturn result;
			return Create (surface, out result, pixelBufferAttributes);
		}
	}
}
#endif
