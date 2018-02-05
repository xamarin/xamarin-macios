#if XAMCORE_2_0
using System;

namespace Metal {
	public static partial class MTLArgumentEncoder_Extensions {
		public unsafe static void SetBuffers (this IMTLArgumentEncoder This, IMTLBuffer [] buffers, nint [] offsets, Foundation.NSRange range)
		{
			fixed (void* handle = offsets)
				This.SetBuffers (buffers, (IntPtr)handle, range);
		}
	}
}
#endif