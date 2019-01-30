//
// MPSImageBatch.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using Metal;

namespace MetalPerformanceShaders {
	[iOS (11,3), TV (11,3), Mac (10,13,4, onlyOn64: true)]
	public static partial class MPSImageBatch {

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSImageBatchIncrementReadCount (IntPtr batch, nint amount);

		// Using 'NSArray<MPSImage>' instead of `MPSImage[]` because image array 'Handle' matters.
		public static nuint IncrementReadCount (NSArray<MPSImage> imageBatch, nint amount)
		{
			if (imageBatch == null)
				throw new ArgumentNullException (nameof (imageBatch));

			return MPSImageBatchIncrementReadCount (imageBatch.Handle, amount);
		}

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern void MPSImageBatchSynchronize (IntPtr batch, IntPtr /* id<MTLCommandBuffer> */ cmdBuf);

		// Using 'NSArray<MPSImage>' instead of `MPSImage[]` because image array 'Handle' matters.
		public static void Synchronize (NSArray<MPSImage> imageBatch, IMTLCommandBuffer commandBuffer)
		{
			if (imageBatch == null)
				throw new ArgumentNullException (nameof (imageBatch));
			if (commandBuffer == null)
				throw new ArgumentNullException (nameof (commandBuffer));

			MPSImageBatchSynchronize (imageBatch.Handle, commandBuffer.Handle);
		}

		[iOS (12,0), TV (12,0), Mac (10,14, onlyOn64: true)]
		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSImageBatchResourceSize (IntPtr batch);

		// Using 'NSArray<MPSImage>' instead of `MPSImage[]` because image array 'Handle' matters.
		[iOS (12,0), TV (12,0), Mac (10,14, onlyOn64: true)]
		public static nuint GetResourceSize (NSArray<MPSImage> imageBatch)
		{
			if (imageBatch == null)
				throw new ArgumentNullException (nameof (imageBatch));

			return MPSImageBatchResourceSize (imageBatch.Handle);
		}

		// TODO: Disabled due to 'MPSImageBatchIterate' is not in the native library rdar://47282304.
		//[iOS (12,0), TV (12,0), Mac (10,14, onlyOn64: true)]
		//[DllImport (Constants.MetalPerformanceShadersLibrary)]
		//static extern nint MPSImageBatchIterate (IntPtr batch, IntPtr iterator);

		//[iOS (12,0), TV (12,0), Mac (10,14, onlyOn64: true)]
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

		//[iOS (12,0), TV (12,0), Mac (10,14, onlyOn64: true)]
		//[BindingImpl (BindingImplOptions.Optimizable)]
		//public static nint Iterate (NSArray<MPSImage> imageBatch, MPSImageBatchIterator iterator)
		//{
		//	if (imageBatch == null)
		//		throw new ArgumentNullException (nameof (imageBatch));
		//	if (iterator == null)
		//		throw new ArgumentNullException (nameof (iterator));
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
#endif