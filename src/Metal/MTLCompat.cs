#if XAMCORE_2_0 || !MONOMAC

using XamCore.ObjCRuntime;

namespace XamCore.Metal {

#if IOS || TVOS
	public static partial class MTLRenderCommandEncoder_Extensions {

		// Apple removed this in Xcode 8
		[Introduced (PlatformName.iOS, 9,0)]
		[Deprecated (PlatformName.iOS, 10,0, message: "Removed in iOS 10")]
		[Introduced (PlatformName.TvOS, 9,1)]
		[Deprecated (PlatformName.TvOS, 10,0, message: "Removed in tvOS 10")]
		public static void SetDepthClipMode (IMTLRenderCommandEncoder This, MTLDepthClipMode depthClipMode)
		{
		}
	}
#endif
}

#endif
