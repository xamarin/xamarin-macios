//
// SecProtocolMetadata.cs: Bindings the Security sec_protocol_metadata_t
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using sec_protocol_metadata_t=System.IntPtr;
using dispatch_queue_t=System.IntPtr;

namespace Security {
	
	public class SecProtocolMetadata : NativeObject {
		internal SecProtocolMetadata (IntPtr handle) : base (handle, false) {}
		public SecProtocolMetadata (IntPtr handle, bool owns) : base (handle, owns) {}
	}
}
