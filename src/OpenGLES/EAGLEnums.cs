using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.OpenGLES {

	// NSUInteger -> EAGL.h
	[Native]
	public enum EAGLRenderingAPI : nuint_compat_int {
		OpenGLES1 = 1,
		OpenGLES2 = 2,
		OpenGLES3 = 3,
	}
}
