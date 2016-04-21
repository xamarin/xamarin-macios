#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;

using XamCore.ModelIO;
using XamCore.ObjCRuntime;
using XamCore.Metal;

namespace XamCore.ModelIO {
	public partial class MDLVertexDescriptor {
		[DllImport (Constants.MetalKitLibrary)]
		static extern  /* MDLVertexDescriptor __nonnull */ IntPtr MTKModelIOVertexDescriptorFromMetal (/* MTLVertexDescriptor __nonnull */ IntPtr mtlDescriptor);

		public static MDLVertexDescriptor FromMetal (MTLVertexDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentException ("descriptor");
			return Runtime.GetNSObject<MDLVertexDescriptor> (MTKModelIOVertexDescriptorFromMetal (descriptor.Handle));
		}
	}
}
#endif
