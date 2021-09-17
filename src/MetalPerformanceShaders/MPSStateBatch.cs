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
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;
using Metal;

namespace MetalPerformanceShaders {
#if !NET
	[iOS (11,3), TV (11,3), Mac (10,13,4)]
#else
	[SupportedOSPlatform ("ios11.3")]
	[SupportedOSPlatform ("tvos11.3")]
#endif
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

#if !NET
		[iOS (12,0), TV (12,0), Mac (10,14)]
#else
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
#endif
		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSStateBatchResourceSize (IntPtr batch);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
#if !NET
		[iOS (12,0), TV (12,0), Mac (10,14)]
#else
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
#endif
		public static nuint GetResourceSize (NSArray<MPSState> stateBatch)
		{
			if (stateBatch == null)
				throw new ArgumentNullException (nameof (stateBatch));

			return MPSStateBatchResourceSize (stateBatch.Handle);
		}
	}
}
