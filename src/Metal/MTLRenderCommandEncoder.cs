using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace Metal {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class IMTLRenderCommandEncoder_Extensions {
#if !WATCH
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos14.5")]
#else
		[TV (14, 5)]
		[NoWatch]
#endif
		public unsafe static void SetViewports (this IMTLRenderCommandEncoder This, MTLViewport [] viewports)
		{
			fixed (void* handle = viewports)
				This.SetViewports ((IntPtr) handle, (nuint) (viewports?.Length ?? 0));
		}
#endif // !WATCH

#if !WATCH
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos14.5")]
#else
		[TV (14, 5)]
		[NoWatch]
#endif
		public unsafe static void SetScissorRects (this IMTLRenderCommandEncoder This, MTLScissorRect [] scissorRects)
		{
			fixed (void* handle = scissorRects)
				This.SetScissorRects ((IntPtr) handle, (nuint) (scissorRects?.Length ?? 0));
		}
#endif // !WATCH

#if !WATCH
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos14.5")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[TV (14, 5)]
		[Mac (11, 0)]
		[NoWatch]
#endif
		public unsafe static void SetTileBuffers (this IMTLRenderCommandEncoder This, IMTLBuffer [] buffers, nuint [] offsets, NSRange range)
		{
			fixed (void* handle = offsets)
				This.SetTileBuffers (buffers, (IntPtr) handle, range);
		}
#endif // !WATCH

#if !WATCH
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos14.5")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[TV (14, 5)]
		[Mac (11, 0)]
		[NoWatch]
#endif
		public unsafe static void SetTileSamplerStates (this IMTLRenderCommandEncoder This, IMTLSamplerState [] samplers, float [] lodMinClamps, float [] lodMaxClamps, NSRange range)
		{
			fixed (void* minHandle = lodMinClamps) {
				fixed (void* maxHandle = lodMaxClamps) {
					This.SetTileSamplerStates (samplers, (IntPtr) minHandle, (IntPtr) maxHandle, range);
				}
			}
		}
#endif // !WATCH
	}
}
