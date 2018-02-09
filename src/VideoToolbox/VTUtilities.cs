//
// VTUtilities.cs
//
// Authors: 
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreGraphics;
using CoreVideo;

namespace VideoToolbox {
	[Mac (10,11), iOS (9,0), TV (10,2)]
	public static class VTUtilities {
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCreateCGImageFromCVPixelBuffer (
			/* CM_NONNULL CVPixelBufferRef */ IntPtr pixelBuffer,
			/* CM_NULLABLE CFDictionaryRef */ IntPtr options,
			/* CM_RETURNS_RETAINED_PARAMETER CM_NULLABLE CGImageRef * CM_NONNULL */ out IntPtr imageOut);

		// intentionally not exposing the (NSDictionary options) argument
		// since header docs indicate that there are no options available
		// as of 9.0/10.11 and to always pass NULL
		public static VTStatus ToCGImage (this CVPixelBuffer pixelBuffer, out CGImage image)
		{
			if (pixelBuffer == null)
				throw new ArgumentNullException ("pixelBuffer");
			if (pixelBuffer.Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CVPixelBuffer");

			image = null;

			IntPtr imagePtr;
			var ret = VTCreateCGImageFromCVPixelBuffer (pixelBuffer.Handle,
				IntPtr.Zero, // no options as of 9.0/10.11 - always pass NULL
				out imagePtr);

			if (imagePtr != IntPtr.Zero)
				image = Runtime.GetINativeObject<CGImage> (imagePtr, true); // This is already retained CM_RETURNS_RETAINED_PARAMETER

			return ret;
		}
	}
}
