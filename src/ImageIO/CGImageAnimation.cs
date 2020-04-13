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

namespace ImageIO {

    public partial class CGImageAnimation {

        public delegate void CGImageSourceAnimationBlock (nint arg0, CGImage arg1, out bool done /*check*/ );

        [DllImport (Constants.ImageIOLibrary)]
        static extern unsafe int CGAnimateImageAtURLWithBlock (/* CFURLRef */ IntPtr url, /* CFDictionaryRef _iio_Nullable */ IntPtr options, /* CGImageSourceAnimationBlock */ IntPtr block);

        [DllImport (Constants.ImageIOLibrary)]
        static extern int CGAnimateImageDataWithBlock ( /* CFDataRef _Nonnull */ IntPtr data, /* CFDictionaryRef _Nullable */ IntPtr options, /* CGImageSourceAnimationBlock _Nonnull */ IntPtr block);

        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual int CGAnimateImage (NSUrl url, NSDictionary options, [BlockProxy (typeof (NIDCGImageSourceAnimationBlock))] CGImageSourceAnimationBlock block) {
            if (url == null)
                throw new ArgumentNullException ("url");
            if (block == null)
                throw new ArgumentNullException ("block");

            int ret;
            unsafe {
                BlockLiteral * block_ptr_block;
                BlockLiteral block_block;
                block_block = new BlockLiteral ();
                block_ptr_block = & block_block;
                block_block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, block);

                try {
                    ret = CGAnimateImageAtURLWithBlock (url.Handle, options == null ? IntPtr.Zero : options.Handle, (IntPtr) block_ptr_block);
                } finally {
                    block_ptr_block -> CleanupBlock ();
                }
            }

            return ret;
        }

        [Introduced (PlatformName.MacOSX, 10, 15, PlatformArchitecture.All)]
        [Introduced (PlatformName.iOS, 13, 0, PlatformArchitecture.All)]
        [BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual int CGAnimateImage (NSData data, NSDictionary options, [BlockProxy (typeof (NIDCGImageSourceAnimationBlock))] CGImageSourceAnimationBlock block) {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (block == null)
                throw new ArgumentNullException ("block");

            int ret;
            unsafe {
                BlockLiteral * block_ptr_block;
                BlockLiteral block_block;
                block_block = new BlockLiteral ();
                block_ptr_block = & block_block;
                block_block.SetupBlockUnsafe (SDCGImageSourceAnimationBlock.Handler, block);

                try {
                    ret = CGAnimateImageDataWithBlock(data.Handle, options == null ? IntPtr.Zero : options.Handle, (IntPtr) block_ptr_block);
                } finally {
                    block_ptr_block -> CleanupBlock ();
                }
            }
 
            return ret;
        }

        //
        // This class bridges native block invocations that call into C#
        //
        static internal class SDCGImageSourceAnimationBlock {
            static internal readonly DCGImageSourceAnimationBlock Handler = Invoke;

            [MonoPInvokeCallback (typeof (DCGImageSourceAnimationBlock))]
            static unsafe void Invoke (IntPtr block, nint arg0, IntPtr arg1, [System.Runtime.InteropServices.MarshalAs (System.Runtime.InteropServices.UnmanagedType.I1)] out bool done) {
                var descriptor = (BlockLiteral * ) block;
                var del = (CGImageSourceAnimationBlock) (descriptor -> Target);
                if (del != null)
                    del (arg0, new CoreGraphics.CGImage (arg1), out done);
                else
                    done = default (bool);
            }
        } /* class SDCGImageSourceAnimationBlock */

        internal sealed class NIDCGImageSourceAnimationBlock : TrampolineBlockBase {
            DCGImageSourceAnimationBlock invoker;

            [BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
            public unsafe NIDCGImageSourceAnimationBlock (BlockLiteral * block) : base (block) {
                invoker = block -> GetDelegateForBlock<DCGImageSourceAnimationBlock> ();
            }

            [Preserve (Conditional = true)]
            [BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
            public unsafe static CGImageSourceAnimationBlock Create (IntPtr block) {
                if (block == IntPtr.Zero)
                    return null;
                var del = (CGImageSourceAnimationBlock) GetExistingManagedDelegate (block);
                return del ?? new NIDCGImageSourceAnimationBlock ((BlockLiteral * ) block).Invoke;
            }

            [BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
            unsafe void Invoke (nint arg0, CGImage arg1, out bool done) {
                invoker (BlockPointer, arg0, arg1 == null ? IntPtr.Zero : arg1.Handle, out done);
            }
        } /* class NIDCGImageSourceAnimationBlock */

        [UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
        [UserDelegateType (typeof (CGImageSourceAnimationBlock))]
        internal delegate void DCGImageSourceAnimationBlock (IntPtr block, nint arg0, IntPtr arg1, [System.Runtime.InteropServices.MarshalAs (System.Runtime.InteropServices.UnmanagedType.I1)] out bool done);
    }

}
