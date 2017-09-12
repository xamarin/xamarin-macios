#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Metal;

namespace XamCore.MetalPerformanceShaders {
	public partial class MPSCnnNeuronPReLU {
		public MPSCnnNeuronPReLU (IMTLDevice device, float [] a, nuint count) : this (device, MPSKernel.GetPtr (a, true), count)
		{
		}
	}
}
#endif