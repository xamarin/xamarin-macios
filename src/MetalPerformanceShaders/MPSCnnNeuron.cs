#nullable enable

using System;
using Metal;
using Foundation;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSCnnNeuronPReLU {
#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("tvos12.0", "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[ObsoletedOSPlatform ("macos10.14", "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[ObsoletedOSPlatform ("ios12.0", "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
#else
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
#endif
		public unsafe MPSCnnNeuronPReLU (IMTLDevice device, float [] a) : this (NSObjectFlag.Empty)
		{
			fixed (void* aHandle = a)
				InitializeHandle (InitWithDevice (device, (IntPtr) aHandle, (nuint) a.Length));
		}
	}
}
