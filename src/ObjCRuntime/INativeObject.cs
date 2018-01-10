using System;
using Foundation;

namespace ObjCRuntime {

	public interface INativeObject {
#if !COREBUILD
		IntPtr Handle { 
			get;
		}
#endif
	}

#if !COREBUILD
	static class NativeObjectHelper {

		// help to avoid the (too common pattern)
		// 	var p = x == null ? IntPtr.Zero : x.Handle;
		static public IntPtr GetHandle (this INativeObject self)
		{
			return self == null ? IntPtr.Zero : self.Handle;
		}
	}
#endif
}
