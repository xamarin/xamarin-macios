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
		static extern /* MDLVertexDescriptor __nonnull */ IntPtr MTKModelIOVertexDescriptorFromMetalWithError (/* MTLVertexDescriptor __nonnull */ IntPtr metalDescriptor, out /* NSError */ IntPtr error);

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
		public static MDLVertexDescriptor? FromMetal (MTLVertexDescriptor descriptor, out NSError? error)
		{
			if (descriptor is null)
				throw new ArgumentException (nameof (descriptor));
			IntPtr err;
			var vd = Runtime.GetNSObject<MDLVertexDescriptor> (MTKModelIOVertexDescriptorFromMetalWithError (descriptor.Handle, out err));
			error = Runtime.GetNSObject<NSError> (err);
			return vd;
		}
	}
}
