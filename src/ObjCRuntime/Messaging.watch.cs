#if WATCH
using System;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.CoreGraphics;

namespace XamCore.ObjCRuntime {
#if COREBUILD
	public
#endif
	static partial class Messaging {
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

		// not generated anymore (by other bindings) but needed in NSObject2.cs
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_IntPtr_bool (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, bool arg3);
	}
}
#endif // WATCH
