#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Metal;

namespace XamCore.MetalPerformanceShaders {
	public partial class MPSCnnBinaryConvolution {
		public MPSCnnBinaryConvolution (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, float [] outputBiasTerms, float [] outputScaleTerms, float [] inputBiasTerms, float [] inputScaleTerms, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags)
			: this (device, convolutionData, MPSKernel.GetPtr (outputBiasTerms, false), MPSKernel.GetPtr (outputScaleTerms, false), MPSKernel.GetPtr (inputBiasTerms, false), MPSKernel.GetPtr (inputScaleTerms, false), type, flags)
		{
		}
	}

	public partial class MPSCnnBinaryFullyConnected {
		public MPSCnnBinaryFullyConnected (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, float [] outputBiasTerms, float [] outputScaleTerms, float [] inputBiasTerms, float [] inputScaleTerms, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags)
			: this (device, convolutionData, MPSKernel.GetPtr (outputBiasTerms, false), MPSKernel.GetPtr (outputScaleTerms, false), MPSKernel.GetPtr (inputBiasTerms, false), MPSKernel.GetPtr (inputScaleTerms, false), type, flags)
		{
		}
	}
}
#endif