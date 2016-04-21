#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;

using XamCore.ModelIO;
using XamCore.ObjCRuntime;
using XamCore.Metal;

namespace XamCore.Metal {
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
	}
}
#endif
