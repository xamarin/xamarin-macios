//
// MPSStateBatch.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.
//

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using Metal;

namespace MetalPerformanceShaders {
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[iOS (11,3), TV (11,3), Mac (10,13,4)]
	public static partial class MPSStateBatch {

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSStateBatchIncrementReadCount (IntPtr batch, nint amount);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
		public static nuint IncrementReadCount (NSArray<MPSState> stateBatch, nint amount)
		{
			if (stateBatch == null)
				throw new ArgumentNullException (nameof (stateBatch));

			return MPSStateBatchIncrementReadCount (stateBatch.Handle, amount);
		}

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern void MPSStateBatchSynchronize (IntPtr batch, IntPtr /* id<MTLCommandBuffer> */ cmdBuf);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
		public static void Synchronize (NSArray<MPSState> stateBatch, IMTLCommandBuffer commandBuffer)
		{
			if (stateBatch == null)
				throw new ArgumentNullException (nameof (stateBatch));
			if (commandBuffer == null)
				throw new ArgumentNullException (nameof (commandBuffer));

			MPSStateBatchSynchronize (stateBatch.Handle, commandBuffer.Handle);
		}

		[Introduced (PlatformName.MacCatalyst, 13, 0)]
		[iOS (12,0), TV (12,0), Mac (10,14)]
		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSStateBatchResourceSize (IntPtr batch);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
		[Introduced (PlatformName.MacCatalyst, 13, 0)]
		[iOS (12,0), TV (12,0), Mac (10,14)]
		public static nuint GetResourceSize (NSArray<MPSState> stateBatch)
		{
			if (stateBatch == null)
				throw new ArgumentNullException (nameof (stateBatch));

			return MPSStateBatchResourceSize (stateBatch.Handle);
		}
	}
}
