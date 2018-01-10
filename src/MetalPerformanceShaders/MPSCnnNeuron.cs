#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Metal;
using XamCore.Foundation;

namespace XamCore.MetalPerformanceShaders {
	public partial class MPSCnnNeuronPReLU {
		public unsafe MPSCnnNeuronPReLU (IMTLDevice device, float [] a) : this (NSObjectFlag.Empty)
		{
			fixed (void* aHandle = a)
				InitializeHandle (InitWithDevice (device, (IntPtr) aHandle, (nuint)a.Length));
		}
	}
}
#endif