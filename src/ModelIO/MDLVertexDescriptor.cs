using System;
using System.Runtime.InteropServices;

using Foundation;
using ModelIO;
using ObjCRuntime;
using Metal;

#nullable enable

namespace ModelIO {
	public partial class MDLVertexDescriptor {
		[DllImport (Constants.MetalKitLibrary)]
		static extern  /* MDLVertexDescriptor __nonnull */ IntPtr MTKModelIOVertexDescriptorFromMetal (/* MTLVertexDescriptor __nonnull */ IntPtr mtlDescriptor);

		public static MDLVertexDescriptor? FromMetal (MTLVertexDescriptor descriptor)
		{
			if (descriptor is null)
				throw new ArgumentException (nameof (descriptor));
			return Runtime.GetNSObject<MDLVertexDescriptor> (MTKModelIOVertexDescriptorFromMetal (descriptor.Handle));
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.MetalKitLibrary)]
		unsafe static extern /* MDLVertexDescriptor __nonnull */ IntPtr MTKModelIOVertexDescriptorFromMetalWithError (/* MTLVertexDescriptor __nonnull */ IntPtr metalDescriptor, /* NSError */ IntPtr* error);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static MDLVertexDescriptor? FromMetal (MTLVertexDescriptor descriptor, out NSError? error)
		{
			if (descriptor is null)
				throw new ArgumentException (nameof (descriptor));
			IntPtr err;
			MDLVertexDescriptor? vd;
			unsafe {
				vd = Runtime.GetNSObject<MDLVertexDescriptor> (MTKModelIOVertexDescriptorFromMetalWithError (descriptor.Handle, &err));
			}
			error = Runtime.GetNSObject<NSError> (err);
			return vd;
		}
	}
}
