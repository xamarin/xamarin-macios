using System;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSCnnConvolutionDescriptor {

#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Introduced (PlatformName.TvOS, 11, 0, PlatformArchitecture.All, null)]
		[Introduced (PlatformName.iOS, 11, 0, PlatformArchitecture.All, null)]
#endif
		public unsafe void SetBatchNormalizationParameters (float [] mean, float [] variance, float [] gamma, float [] beta, float epsilon)
		{
			fixed (void* meanHandle = mean)
			fixed (void* varianceHandle = variance)
			fixed (void* gammaHandle = gamma)
			fixed (void* betaHandle = beta)
				SetBatchNormalizationParameters ((IntPtr) meanHandle, (IntPtr) varianceHandle, (IntPtr) gammaHandle, (IntPtr) betaHandle, epsilon);
		}

	}
}
