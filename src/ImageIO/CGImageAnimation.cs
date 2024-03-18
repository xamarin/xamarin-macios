//
// ImageIO - CGImageAnimation.cs
//
// Authors:
//	Whitney Schmidt  <whschm@microsoft.com>
//
// Copyright 2020, Microsoft Corp.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace ImageIO {

#if NET
    [SupportedOSPlatform ("ios")]
    [SupportedOSPlatform ("maccatalyst")]
    [SupportedOSPlatform ("macos")]
    [SupportedOSPlatform ("tvos")]
#endif
	public static class CGImageAnimation {

		public delegate void CGImageSourceAnimationHandler (nint index, CGImage image, out bool stop);

#if NET
        [SupportedOSPlatform ("macos")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
        [SupportedOSPlatform ("maccatalyst")]
#else
		[Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		unsafe static extern /* OSStatus */ CGImageAnimationStatus CGAnimateImageAtURLWithBlock ( /* CFURLRef */ IntPtr url, /* CFDictionaryRef _iio_Nullable */ IntPtr options, /* CGImageSourceAnimationHandler */ BlockLiteral* block);

#if NET
        [SupportedOSPlatform ("macos")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
        [SupportedOSPlatform ("maccatalyst")]
#else
		[Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		unsafe static extern /* OSStatus */ CGImageAnimationStatus CGAnimateImageDataWithBlock ( /* CFDataRef _Nonnull */ IntPtr data, /* CFDictionaryRef _Nullable */ IntPtr options, /* CGImageSourceAnimationHandler _Nonnull */ BlockLiteral* block);

#if NET
        [SupportedOSPlatform ("macos")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
        [SupportedOSPlatform ("maccatalyst")]
#else
		[Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static CGImageAnimationStatus AnimateImage (NSUrl url, CGImageAnimationOptions options, CGImageSourceAnimationHandler handler)
		{
#if IOS && ARCH_32
            throw new PlatformNotSupportedException ("This API is not supported on this version of iOS");
#else
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, nint, IntPtr, byte*, void> trampoline = &SDCGImageSourceAnimationBlock.Invoke;
				using var block = new BlockLiteral (trampoline, handler, typeof (SDCGImageSourceAnimationBlock), nameof (SDCGImageSourceAnimationBlock.Invoke));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, handler);
#endif
				return CGAnimateImageAtURLWithBlock (url.Handle, options.GetHandle (), &block);
			}
#endif
		}

#if NET
        [SupportedOSPlatform ("macos")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
        [SupportedOSPlatform ("maccatalyst")]
#else
		[Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static CGImageAnimationStatus AnimateImage (NSData data, CGImageAnimationOptions options, CGImageSourceAnimationHandler handler)
		{
#if IOS && ARCH_32
            throw new PlatformNotSupportedException ("This API is not supported on this version of iOS");
#else
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, nint, IntPtr, byte*, void> trampoline = &SDCGImageSourceAnimationBlock.Invoke;
				using var block = new BlockLiteral (trampoline, handler, typeof (SDCGImageSourceAnimationBlock), nameof (SDCGImageSourceAnimationBlock.Invoke));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, handler);
#endif
				return CGAnimateImageDataWithBlock (data.Handle, options.GetHandle (), &block);
			}
#endif
		}

		//
		// This class bridges native block invocations that call into C#
		//
		static internal class SDCGImageSourceAnimationBlock {
#if !NET
			unsafe static internal readonly DCGImageSourceAnimationBlock Handler = Invoke;

			[MonoPInvokeCallback (typeof (DCGImageSourceAnimationBlock))]
#else
			[UnmanagedCallersOnly]
#endif
			internal unsafe static void Invoke (IntPtr block, nint index, IntPtr image, byte* stop)
			{
				var del = BlockLiteral.GetTarget<CGImageSourceAnimationHandler> (block);
				if (del is not null) {
					del (index, new CoreGraphics.CGImage (image, false), out var stopValue);
					*stop = stopValue ? (byte) 1 : (byte) 0;
				} else
					*stop = 0;
			}
		} /* class SDCGImageSourceAnimationBlock */

#if !NET
		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		[UserDelegateType (typeof (CGImageSourceAnimationHandler))]
		unsafe internal delegate void DCGImageSourceAnimationBlock (IntPtr block, nint index, IntPtr imageHandle, byte* stop);
#endif
	}

}
