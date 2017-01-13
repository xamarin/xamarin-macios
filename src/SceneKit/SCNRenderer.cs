#if !WATCH

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

#if MONOMAC
using GLContext = global::XamCore.OpenGL.CGLContext;
#else
using GLContext = global::XamCore.OpenGLES.EAGLContext;
#endif

namespace XamCore.SceneKit {

	public partial class SCNRenderer {

		// workaround for generator bug #51514
		public static SCNRenderer FromContext (GLContext context, NSDictionary options)
		{
			return SCNRenderer.FromContext (context.GetHandle (), options);
		}
	}
}

#endif
