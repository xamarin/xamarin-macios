using System;
using System.Runtime.InteropServices;

using Foundation;
using ModelIO;
using ObjCRuntime;
using Metal;

#nullable enable

namespace Metal {
	public partial class MTLVertexDescriptor {

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
#endif
		[DllImport (Constants.MetalKitLibrary)]
		static extern  /* MTLVertexDescriptor __nonnull */ IntPtr MTKMetalVertexDescriptorFromModelIO (/* MDLVertexDescriptor __nonnull */ IntPtr modelIODescriptor);

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
#endif
		public static MTLVertexDescriptor? FromModelIO (MDLVertexDescriptor descriptor)
		{
			if (descriptor is null)
				throw new ArgumentException ("descriptor");
			return Runtime.GetNSObject<MTLVertexDescriptor> (MTKMetalVertexDescriptorFromModelIO (descriptor.Handle));
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
#endif
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MTLVertexDescriptor __nonnull */ IntPtr MTKMetalVertexDescriptorFromModelIOWithError (/* MDLVertexDescriptor __nonnull */ IntPtr modelIODescriptor, out IntPtr error);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
#endif
		public static MTLVertexDescriptor? FromModelIO (MDLVertexDescriptor descriptor, out NSError? error)
		{
			if (descriptor is null)
				throw new ArgumentException ("descriptor");
			IntPtr err;
			var vd = Runtime.GetNSObject<MTLVertexDescriptor> (MTKMetalVertexDescriptorFromModelIOWithError (descriptor.Handle, out err));
			error = Runtime.GetNSObject<NSError> (err);
			return vd;
		}
	}
}
