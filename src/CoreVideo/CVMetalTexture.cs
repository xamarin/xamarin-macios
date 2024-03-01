//
// CVMetalTexture.cs: Implementation of the CVMetalTexture class
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2015 Xamarin Inc
//
//

#if !__WATCHOS__

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;
using Metal;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreVideo {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#else
	[MacCatalyst (15, 0)]
#endif
	public class CVMetalTexture : NativeObject {
		[Preserve (Conditional = true)]
		internal CVMetalTexture (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* id<MTLTexture> __nullable */ IntPtr CVMetalTextureGetTexture (
			/* CVMetalTextureRef __nonnull */ IntPtr image);

		[DllImport (Constants.CoreVideoLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CVMetalTextureIsFlipped (/* CVMetalTextureRef __nonnull */ IntPtr image);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVMetalTextureGetCleanTexCoords (/* CVMetalTextureRef __nonnull */ IntPtr image,
			/* float[2] */ IntPtr lowerLeft, /* float[2] */ IntPtr lowerRight, /* float[2] */ IntPtr upperRight,
			/* float[2] */ IntPtr upperLeft);

		public IMTLTexture? Texture {
			get {
				return Runtime.GetINativeObject<IMTLTexture> (CVMetalTextureGetTexture (Handle), owns: false);
			}
		}

		public bool IsFlipped {
			get {
				return CVMetalTextureIsFlipped (Handle);
			}
		}

		public void GetCleanTexCoords (out float [] lowerLeft, out float [] lowerRight, out float [] upperRight, out float [] upperLeft)
		{
			lowerLeft = new float [2];
			lowerRight = new float [2];
			upperRight = new float [2];
			upperLeft = new float [2];

			unsafe {
				fixed (float* ll = lowerLeft, lr = lowerRight, ur = upperRight, ul = upperLeft) {
					CVMetalTextureGetCleanTexCoords (Handle, (IntPtr) ll, (IntPtr) lr, (IntPtr) ur, (IntPtr) ul);
				}
			}
		}
	}
}

#endif // IOS || TVOS
