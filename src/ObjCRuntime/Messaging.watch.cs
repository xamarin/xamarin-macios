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
	}
}
#endif // WATCH
