//
// ImageIO - CGImageAnimation.cs
//
// Authors:
//	Whitney Schmidt  <whschm@microsoft.com>
//
// Copyright 2020, Microsoft Corp.
//

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace ImageIO
{

    public partial class CGImageAnimation
    {

        public delegate void CGImageSourceAnimationBlock (nint index, CGImage image, out bool stop);

        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [DllImport (Constants.ImageIOLibrary)]
        static extern int CGAnimateImageAtURLWithBlock ( /* CFURLRef */ IntPtr url, /* CFDictionaryRef _iio_Nullable */ IntPtr options, /* CGImageSourceAnimationBlock */ ref BlockLiteral block);

        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [DllImport (Constants.ImageIOLibrary)]
        static extern int CGAnimateImageDataWithBlock ( /* CFDataRef _Nonnull */ IntPtr data, /* CFDictionaryRef _Nullable */ IntPtr options, /* CGImageSourceAnimationBlock _Nonnull */ ref BlockLiteral block);

        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [BindingImpl (BindingImplOptions.Optimizable)]
        public int AnimateImage (NSUrl url, NSDictionary options, [BlockProxy (typeof (NIDCGImageSourceAnimationBlock))] CGImageSourceAnimationBlock block)
        {
#if IOS && ARCH_32
            throw new PlatformNotSupportedException ();
#else
            if (url == null)
                throw new ArgumentNullException (nameof (url));
            if (block == null)
                throw new ArgumentNullException (nameof (block));

            int ret;
            BlockLiteral block_block;
            block_block = new BlockLiteral ();
            block_block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, block);

            try {
                ret = CGAnimateImageAtURLWithBlock (url.Handle, options.GetHandle (), ref block_block);
            } finally {
                block_block.CleanupBlock ();
            }

            return ret;
#endif
        }

        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [BindingImpl (BindingImplOptions.Optimizable)]
        public int AnimateImage (NSData data, NSDictionary options, [BlockProxy (typeof (NIDCGImageSourceAnimationBlock))] CGImageSourceAnimationBlock block)
        {
#if IOS && ARCH_32
            throw new PlatformNotSupportedException ();
#else
            if (data == null)
                throw new ArgumentNullException (nameof (data));
            if (block == null)
                throw new ArgumentNullException (nameof (block));

            int ret;
            BlockLiteral block_block;
            block_block = new BlockLiteral ();
            block_block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, block);

            try {
                ret = CGAnimateImageDataWithBlock (data.Handle, options.GetHandle (), ref block_block);
            } finally {
                block_block.CleanupBlock ();
            }

            return ret;
#endif
        }

        //
        // This class bridges native block invocations that call into C#
        //
        static internal class SDCGImageSourceAnimationBlock
        {
            static internal readonly DCGImageSourceAnimationBlock Handler = Invoke;

            [MonoPInvokeCallback (typeof (DCGImageSourceAnimationBlock))]
            static void Invoke (IntPtr block, nint index, IntPtr image, [System.Runtime.InteropServices.MarshalAs (System.Runtime.InteropServices.UnmanagedType.I1)] out bool stop)
            {
                var del = BlockLiteral.GetTarget<CGImageSourceAnimationBlock> (block);
                if (del != null)
                    del (index, new CoreGraphics.CGImage (image), out stop);
                else
                    stop = default (bool);
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
            public unsafe static CGImageSourceAnimationBlock Create (IntPtr block)
            {
                if (block == IntPtr.Zero)
                    return null;
                var del = (CGImageSourceAnimationBlock) GetExistingManagedDelegate (block);
                return del ?? new NIDCGImageSourceAnimationBlock ( (BlockLiteral *) block).Invoke;
            }

            [BindingImpl (BindingImplOptions.Optimizable)]
            void Invoke (nint index, CGImage image, out bool stop)
            {
                invoker (BlockPointer, index, image == null ? IntPtr.Zero : image.Handle, out stop);
            }
        } /* class NIDCGImageSourceAnimationBlock */

        [UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
        [UserDelegateType (typeof (CGImageSourceAnimationBlock))]
        internal delegate void DCGImageSourceAnimationBlock (IntPtr block, nint index, IntPtr imageHandle, [System.Runtime.InteropServices.MarshalAs (System.Runtime.InteropServices.UnmanagedType.I1)] out bool stop);
    }

}
