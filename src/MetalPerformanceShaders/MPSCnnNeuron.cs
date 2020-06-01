using System;
using Metal;
using Foundation;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSCnnNeuronPReLU {
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Please use '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Please use '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Please use '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		public unsafe MPSCnnNeuronPReLU (IMTLDevice device, float [] a) : this (NSObjectFlag.Empty)
		{
			fixed (void* aHandle = a)
				InitializeHandle (InitWithDevice (device, (IntPtr) aHandle, (nuint)a.Length));
		}
	}
}

