using XamCore.ObjCRuntime;

namespace XamCore.OpenGLES {

	// NSUInteger -> EAGL.h
	[Native]
	public enum EAGLRenderingAPI : nuint_compat_int {
		OpenGLES1 = 1,
		OpenGLES2 = 2,
		OpenGLES3 = 3,
	}
}
