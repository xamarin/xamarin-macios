//
// NWProtocolTls: Bindings the Netowrk nw_protocol_options API focus on Udp options.
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
//
// Copyrigh 2019 Microsoft Inc
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
	public class NWProtocolUdpOptions : NWProtocolOptions {
		internal NWProtocolUdpOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		public NWProtocolUdpOptions () : this (nw_udp_create_options (), owns: true) {}

		public void SetPreferNoChecksum (bool preferNoChecksum) => nw_udp_options_set_prefer_no_checksum (GetCheckedHandle (), preferNoChecksum);
	}
}
