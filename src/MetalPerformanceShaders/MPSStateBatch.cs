//
// MPSStateBatch.cs
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
	[SupportedOSPlatform ("ios11.3")]
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[iOS (11, 3)]
	[TV (11, 3)]
#endif
	public static partial class MPSStateBatch {

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSStateBatchIncrementReadCount (IntPtr batch, nint amount);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
		public static nuint IncrementReadCount (NSArray<MPSState> stateBatch, nint amount)
		{
			if (stateBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (stateBatch));

			return MPSStateBatchIncrementReadCount (stateBatch.Handle, amount);
		}

		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern void MPSStateBatchSynchronize (IntPtr batch, IntPtr /* id<MTLCommandBuffer> */ cmdBuf);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
		public static void Synchronize (NSArray<MPSState> stateBatch, IMTLCommandBuffer commandBuffer)
		{
			if (stateBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (stateBatch));
			if (commandBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (commandBuffer));

			MPSStateBatchSynchronize (stateBatch.Handle, commandBuffer.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
#endif
		[DllImport (Constants.MetalPerformanceShadersLibrary)]
		static extern nuint MPSStateBatchResourceSize (IntPtr batch);

		// Using 'NSArray<MPSState>' instead of `MPSState[]` because array 'Handle' matters.
#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
#endif
		public static nuint GetResourceSize (NSArray<MPSState> stateBatch)
		{
			if (stateBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (stateBatch));

			return MPSStateBatchResourceSize (stateBatch.Handle);
		}
	}
}
