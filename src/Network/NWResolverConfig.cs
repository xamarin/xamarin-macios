#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using System.Runtime.Versioning;

namespace Network {
	public class NWResolverConfig : NativeObject {

		public NWResolverConfig (IntPtr handle, bool owns) : base (handle, owns) {}
	}
}
