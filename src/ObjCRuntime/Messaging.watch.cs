#if WATCH
using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreGraphics;

namespace ObjCRuntime {
#if COREBUILD
	public
#endif
	static partial class Messaging {
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";
	}
}
#endif // WATCH
