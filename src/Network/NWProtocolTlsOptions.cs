//
// NWProtocolTls: Bindings the Netowrk nw_protocol_options API focus on Tls options.
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using Security;
using OS_nw_protocol_definition=System.IntPtr;
using IntPtr=System.IntPtr;

namespace Network {

	[TV (12,0), Mac (10,14), iOS (12,0), Watch (6,0)]
	public class NWProtocolTlsOptions : NWProtocolOptions {
		internal NWProtocolTlsOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_tls_create_options ();
		
		public NWProtocolTlsOptions () : this (nw_tls_create_options (), owns: true) {}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_tls_copy_sec_protocol_options (IntPtr options);

		public SecProtocolOptions ProtocolOptions => new SecProtocolOptions (nw_tls_copy_sec_protocol_options (GetCheckedHandle ()), owns: true);
	}
}
