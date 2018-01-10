#if XAMCORE_2_0 || !MONOMAC
using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
	public partial class MPSCnnNeuronPReLU {
		public unsafe MPSCnnNeuronPReLU (IMTLDevice device, float [] a) : this (NSObjectFlag.Empty)
		{
			fixed (void* aHandle = a)
				InitializeHandle (InitWithDevice (device, (IntPtr) aHandle, (nuint)a.Length));
		}
	}
}
#endif