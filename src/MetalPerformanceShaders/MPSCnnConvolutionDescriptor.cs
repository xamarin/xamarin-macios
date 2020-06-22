using System;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSCnnConvolutionDescriptor {

		[Introduced (PlatformName.TvOS, 11, 0, PlatformArchitecture.All, null)]
		[Introduced (PlatformName.iOS, 11, 0, PlatformArchitecture.All, null)]
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

