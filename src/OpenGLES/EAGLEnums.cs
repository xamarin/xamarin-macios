using ObjCRuntime;

namespace OpenGLES {

	// NSUInteger -> EAGL.h
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Native]
	public enum EAGLRenderingAPI : ulong {
		OpenGLES1 = 1,
		OpenGLES2 = 2,
		OpenGLES3 = 3,
	}
}
