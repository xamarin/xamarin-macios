#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;

using Foundation;
using ModelIO;
using ObjCRuntime;
using Metal;

namespace Metal {
	public partial class MTLVertexDescriptor {

		[iOS (9,0)]
		[DllImport (Constants.MetalKitLibrary)]
		static extern  /* MTLVertexDescriptor __nonnull */ IntPtr MTKMetalVertexDescriptorFromModelIO (/* MDLVertexDescriptor __nonnull */ IntPtr modelIODescriptor);

		[iOS (9,0)]
		public static MTLVertexDescriptor FromModelIO (MDLVertexDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentException ("descriptor");
			return Runtime.GetNSObject<MTLVertexDescriptor> (MTKMetalVertexDescriptorFromModelIO (descriptor.Handle));
		}

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[TV (10,0)]
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MTLVertexDescriptor __nonnull */ IntPtr MTKMetalVertexDescriptorFromModelIOWithError (/* MDLVertexDescriptor __nonnull */ IntPtr modelIODescriptor, out IntPtr error);

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[TV (10,0)]
		public static MTLVertexDescriptor FromModelIO (MDLVertexDescriptor descriptor, out NSError error)
		{
			if (descriptor == null)
				throw new ArgumentException ("descriptor");
			IntPtr err;
			var vd = Runtime.GetNSObject<MTLVertexDescriptor> (MTKMetalVertexDescriptorFromModelIOWithError (descriptor.Handle, out err));
			error = Runtime.GetNSObject<NSError> (err);
			return vd;
		}
	}
}
#endif
