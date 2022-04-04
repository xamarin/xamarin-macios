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

namespace ImageIO
{

    public static class CGImageAnimation
    {

        public delegate void CGImageSourceAnimationHandler (nint index, CGImage image, out bool stop);

#if NET
        [SupportedOSPlatform ("macos10.15")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
#else
        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
        [DllImport (Constants.ImageIOLibrary)]
        static extern /* OSStatus */ CGImageAnimationStatus CGAnimateImageAtURLWithBlock ( /* CFURLRef */ IntPtr url, /* CFDictionaryRef _iio_Nullable */ IntPtr options, /* CGImageSourceAnimationHandler */ ref BlockLiteral block);

#if NET
        [SupportedOSPlatform ("macos10.15")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
#else
        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
        [DllImport (Constants.ImageIOLibrary)]
        static extern /* OSStatus */ CGImageAnimationStatus CGAnimateImageDataWithBlock ( /* CFDataRef _Nonnull */ IntPtr data, /* CFDictionaryRef _Nullable */ IntPtr options, /* CGImageSourceAnimationHandler _Nonnull */ ref BlockLiteral block);

#if NET
        [SupportedOSPlatform ("macos10.15")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
#else
        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
        [BindingImpl (BindingImplOptions.Optimizable)]
        public static CGImageAnimationStatus AnimateImage (NSUrl url, CGImageAnimationOptions options, [BlockProxy (typeof (NIDCGImageSourceAnimationBlock))] CGImageSourceAnimationHandler handler)
        {
#if IOS && ARCH_32
            throw new PlatformNotSupportedException ("This API is not supported on this version of iOS");
#else
            if (url == null)
                ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
            if (handler == null)
                ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

            var block = new BlockLiteral ();
            block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, handler);

            try {
                return CGAnimateImageAtURLWithBlock (url.Handle, options.GetHandle (), ref block);
            } finally {
                block.CleanupBlock ();
            }
#endif
        }

#if NET
        [SupportedOSPlatform ("macos10.15")]
        [SupportedOSPlatform ("ios13.0")]
        [SupportedOSPlatform ("tvos13.0")]
#else
        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.TvOS, 13, 0, PlatformArchitecture.All)]
        [Introduced (PlatformName.WatchOS, 6, 0, PlatformArchitecture.All)]
#endif
        [BindingImpl (BindingImplOptions.Optimizable)]
        public static CGImageAnimationStatus AnimateImage (NSData data, CGImageAnimationOptions options, [BlockProxy (typeof (NIDCGImageSourceAnimationBlock))] CGImageSourceAnimationHandler handler)
        {
#if IOS && ARCH_32
            throw new PlatformNotSupportedException ("This API is not supported on this version of iOS");
#else
            if (data == null)
                ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
            if (handler == null)
                ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

            var block = new BlockLiteral ();
            block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, handler);

            try {
                return CGAnimateImageDataWithBlock (data.Handle, options.GetHandle (), ref block);
            } finally {
                block.CleanupBlock ();
            }
#endif
        }

        //
        // This class bridges native block invocations that call into C#
        //
        static internal class SDCGImageSourceAnimationBlock
        {
            static internal readonly DCGImageSourceAnimationBlock Handler = Invoke;

            [MonoPInvokeCallback (typeof (DCGImageSourceAnimationBlock))]
            static void Invoke (IntPtr block, nint index, IntPtr image, [MarshalAs (UnmanagedType.I1)] out bool stop)
            {
                var del = BlockLiteral.GetTarget<CGImageSourceAnimationHandler> (block);
                if (del != null)
                    del (index, new CoreGraphics.CGImage (image, false), out stop);
                else
                    stop = false;
            }
        } /* class SDCGImageSourceAnimationBlock */

        internal sealed class NIDCGImageSourceAnimationBlock : TrampolineBlockBase
        {
            DCGImageSourceAnimationBlock invoker;

            [BindingImpl (BindingImplOptions.Optimizable)]
            public unsafe NIDCGImageSourceAnimationBlock (BlockLiteral * block) : base (block)
            {
                invoker = block->GetDelegateForBlock<DCGImageSourceAnimationBlock> ();
            }

            [Preserve (Conditional = true)]
            [BindingImpl (BindingImplOptions.Optimizable)]
            public unsafe static CGImageSourceAnimationHandler? Create (IntPtr block)
            {
                if (block == IntPtr.Zero)
                    return null;
                var del = (CGImageSourceAnimationHandler) GetExistingManagedDelegate (block);
                return del ?? new NIDCGImageSourceAnimationBlock ( (BlockLiteral *) block).Invoke;
            }

            [BindingImpl (BindingImplOptions.Optimizable)]
            void Invoke (nint index, CGImage image, out bool stop)
            {
                invoker (BlockPointer, index, image.GetHandle (), out stop);
            }
        } /* class NIDCGImageSourceAnimationBlock */

        [UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
        [UserDelegateType (typeof (CGImageSourceAnimationHandler))]
        internal delegate void DCGImageSourceAnimationBlock (IntPtr block, nint index, IntPtr imageHandle, [MarshalAs (UnmanagedType.I1)] out bool stop);
    }

}
