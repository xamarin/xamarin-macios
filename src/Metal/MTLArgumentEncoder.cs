using System;
namespace XamCore.Metal {
	public partial class MTLArgumentEncoder_Extensions {
		public unsafe static void SetBuffers (this IMTLArgumentEncoder This, IMTLBuffer [] buffers, nint [] offsets, Foundation.NSRange range)
		{
			unsafe {
				fixed (void* handle = offsets)
					SetBuffers (This, buffers, (IntPtr)handle, range);
			}
		}
	}
}
