using System;
using Metal;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace MetalPerformanceShaders {
	public partial class MPSCnnNeuronPReLU {
#if !NET
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.")]
#else
		[UnsupportedOSPlatform ("ios12.0")]
		[UnsupportedOSPlatform ("tvos12.0")]
		[UnsupportedOSPlatform ("macos10.14")]
#if IOS
		[Obsolete ("Starting with ios12.0 please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos12.0 please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.14 please use the '.ctor (IMTLDevice, MPSNNNeuronDescriptor)' overload instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public unsafe MPSCnnNeuronPReLU (IMTLDevice device, float [] a) : this (NSObjectFlag.Empty)
		{
			fixed (void* aHandle = a)
				InitializeHandle (InitWithDevice (device, (IntPtr) aHandle, (nuint)a.Length));
		}
	}
}
