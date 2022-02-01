using ObjCRuntime;

namespace OpenGLES {

	// NSUInteger -> EAGL.h
#if NET
	[UnsupportedOSPlatform ("tvos12.0")]
	[UnsupportedOSPlatform ("ios12.0")]
#if TVOS
	[Obsolete ("Starting with tvos12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12,0, message: "Use 'Metal' instead.")]
#endif
	[Native]
	public enum EAGLRenderingAPI : ulong {
		OpenGLES1 = 1,
		OpenGLES2 = 2,
		OpenGLES3 = 3,
	}
}
