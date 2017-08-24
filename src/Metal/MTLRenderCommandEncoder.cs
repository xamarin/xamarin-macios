#if XAMCORE_2_0
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Metal {
	public static class IMTLRenderCommandEncoder_Extensions {
#if !COREBUILD
		//public unsafe static void SetViewports (this IMTLRenderCommandEncoder This, MTLViewPort [] viewports)
		//{
		//	fixed (void* handle = viewports)
		//		SetViewports (This, (IntPtr)handle, viewports?.Length ?? 0);
		//}

		//public unsafe static void SetScissorRects (this IMTLRenderCommandEncoder This, MTLScissorRect [] scissorRects)
		//{
		//	fixed (void* handle = scissorRects)
		//		SetScissorRects (This, (IntPtr)handle, scissorRects?.Length ?? 0);
		//}
	}
#endif
}
#endif
