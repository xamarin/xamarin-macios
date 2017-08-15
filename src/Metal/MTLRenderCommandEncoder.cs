using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Metal {
	public static class MTLRenderCommandEncoder_Extensions {
		public unsafe static void SetViewports (this IMTLRenderCommandEncoder This, MTLViewPort [] viewports)
		{
			unsafe
			{
				fixed (void* handle = viewports)
					SetViewports (This, (IntPtr) handle, viewports.Length);
			}
		}

		public unsafe static void SetScissorRects (this IMTLRenderCommandEncoder This, MTLScissorRect [] scissorRects)
		{
			unsafe {
				fixed (void* handle = scissorRects)
					SetScissorRects (This, (IntPtr)handle, scissorRects.Length);
			}
		}
	}
}
 