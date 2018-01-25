using ObjCRuntime;

namespace OpenGLES {

	// NSUInteger -> EAGL.h
	[Native]
	public enum EAGLRenderingAPI : ulong {
		OpenGLES1 = 1,
		OpenGLES2 = 2,
		OpenGLES3 = 3,
	}
}
