#if XAMCORE_2_0 && MONOMAC
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Metal {
	public static class IMTLRenderCommandEncoder_Extensions {
		[Mac (10,13, onlyOn64: true), NoiOS, NoTV, NoWatch]
		public unsafe static void SetViewports (this IMTLRenderCommandEncoder This, MTLViewport [] viewports)
		{
			fixed (void* handle = viewports)
				This.SetViewports ((IntPtr)handle, (nuint)(viewports?.Length ?? 0));
		}

		[Mac (10,13, onlyOn64: true), NoiOS, NoTV, NoWatch]
		public unsafe static void SetScissorRects (this IMTLRenderCommandEncoder This, MTLScissorRect [] scissorRects)
		{
			fixed (void* handle = scissorRects)
				This.SetScissorRects ((IntPtr)handle, (nuint)(scissorRects?.Length ?? 0));
		}
	}
}
#endif
