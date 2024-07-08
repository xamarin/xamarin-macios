//
// MPSImageBatch.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using Metal;

namespace MetalPerformanceShaders {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public static partial class MPSImageBatch {

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSImageBatchIncrementReadCount (IntPtr batch, nint amount);

		// Using 'NSArray<MPSImage>' instead of `MPSImage[]` because image array 'Handle' matters.
		public static nuint IncrementReadCount (NSArray<MPSImage> imageBatch, nint amount)
		{
			if (imageBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBatch));

			return MPSImageBatchIncrementReadCount (imageBatch.Handle, amount);
		}

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern void MPSImageBatchSynchronize (IntPtr batch, IntPtr /* id<MTLCommandBuffer> */ cmdBuf);

		// Using 'NSArray<MPSImage>' instead of `MPSImage[]` because image array 'Handle' matters.
		public static void Synchronize (NSArray<MPSImage> imageBatch, IMTLCommandBuffer commandBuffer)
		{
			if (imageBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBatch));
			if (commandBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (commandBuffer));

			MPSImageBatchSynchronize (imageBatch.Handle, commandBuffer.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSImageBatchResourceSize (IntPtr batch);

		// Using 'NSArray<MPSImage>' instead of `MPSImage[]` because image array 'Handle' matters.
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static nuint GetResourceSize (NSArray<MPSImage> imageBatch)
		{
			if (imageBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBatch));

			return MPSImageBatchResourceSize (imageBatch.Handle);
		}

		// TODO: Disabled due to 'MPSImageBatchIterate' is not in the native library rdar://47282304.
		//[DllImport (Constants.MetalPerformanceShadersLibrary)]
		//static extern nint MPSImageBatchIterate (IntPtr batch, IntPtr iterator);

		//public delegate nint MPSImageBatchIterator (MPSImage image, nuint index);

		//[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		//internal delegate nint DMPSImageBatchIterator (IntPtr block, IntPtr image, nuint index);

		//// This class bridges native block invocations that call into C#
		//static internal class MPSImageBatchIteratorTrampoline {
		//	static internal readonly DMPSImageBatchIterator Handler = Invoke;

		//	[MonoPInvokeCallback (typeof (DMPSImageBatchIterator))]
		//	static unsafe nint Invoke (IntPtr block, IntPtr image, nuint index)
		//	{
		//		var descriptor = (BlockLiteral *) block;
		//		var del = (MPSImageBatchIterator) descriptor->Target;
		//		nint retval;
		//		using (var img = Runtime.GetNSObject<MPSImage> (image)) {
		//			retval = del (img, index);
		//		}
		//		return retval;
		//	}
		//}

		//[BindingImpl (BindingImplOptions.Optimizable)]
		//public static nint Iterate (NSArray<MPSImage> imageBatch, MPSImageBatchIterator iterator)
		//{
		//	if (imageBatch is null)
		//		ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBatch));
		//	if (iterator is null)
		//		ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (iterator));
		//	unsafe {
		//		BlockLiteral* block_ptr_iterator;
		//		BlockLiteral block_iterator;
		//		block_iterator = new BlockLiteral ();
		//		block_ptr_iterator = &block_iterator;
		//		block_iterator.SetupBlockUnsafe (MPSImageBatchIteratorTrampoline.Handler, iterator);

		//		nint ret = MPSImageBatchIterate (imageBatch.Handle, (IntPtr) block_ptr_iterator);

		//		block_ptr_iterator->CleanupBlock ();

		//		return ret;
		//	}
		//}
	}
}
