using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	public static class Messaging {
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

		public struct objc_super {
			public IntPtr Handle;
			public IntPtr SuperHandle;
		}

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_bool_bool (IntPtr receiver, IntPtr selector, IntPtr a, bool b, bool c);
	}
}