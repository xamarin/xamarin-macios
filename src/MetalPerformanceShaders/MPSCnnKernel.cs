#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Metal;
using XamCore.Foundation;

namespace XamCore.MetalPerformanceShaders {
	public partial class MPSCnnBinaryConvolution {
		public unsafe MPSCnnBinaryConvolution (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, float [] outputBiasTerms, float [] outputScaleTerms, float [] inputBiasTerms, float [] inputScaleTerms, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags)
			: base (NSObjectFlag.Empty)
		{
			fixed (void *outputBiasTermsHandle = outputBiasTerms) {
				fixed (void* outputScaleTermsHandle = outputScaleTerms) {
					fixed (void* inputBiasTermsHandle = inputBiasTerms) {
						fixed (void* inputScaleTermsHandle = inputScaleTerms)
							InitializeHandle (InitWith (device, convolutionData, (IntPtr) outputBiasTermsHandle, (IntPtr) outputScaleTermsHandle, (IntPtr) inputBiasTermsHandle, (IntPtr) inputScaleTermsHandle, type, flags));
					}
				}
			}
		}
	}

	public partial class MPSCnnBinaryFullyConnected {
		public unsafe MPSCnnBinaryFullyConnected (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, float [] outputBiasTerms, float [] outputScaleTerms, float [] inputBiasTerms, float [] inputScaleTerms, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags)
			: base (NSObjectFlag.Empty)
		{
			fixed (void *outputBiasTermsHandle = outputBiasTerms) {
				fixed (void* outputScaleTermsHandle = outputScaleTerms) {
					fixed (void* inputBiasTermsHandle = inputBiasTerms) {
						fixed (void* inputScaleTermsHandle = inputScaleTerms)
							InitializeHandle (InitWith (device, convolutionData, (IntPtr) outputBiasTermsHandle, (IntPtr) outputScaleTermsHandle, (IntPtr) inputBiasTermsHandle, (IntPtr) inputScaleTermsHandle, type, flags));
					}
				}
			}
		}
	}
}
#endif