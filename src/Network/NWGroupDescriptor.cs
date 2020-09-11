using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

#nullable enable

namespace Network {

	[TV (14,0), Mac (11,0), iOS (14,0), Watch (7,0)]
	public class NWGroupDescriptor : NativeObject {
		public NWGroupDescriptor (IntPtr handle, bool owns) : base (handle, owns) {}
	}
}
