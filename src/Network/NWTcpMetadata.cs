//
// NWTcpMetadata.cs: Bindings the Netowrk nw_protocol_metadata_t API that is an Tcp.
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
//
// Copyrigh 2019 Microsoft
//

#nullable enable

using System;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {

	[TV (12,0), Mac (10,14), iOS (12,0), Watch (6,0)]
	public class NWTcpMetadata : NWProtocolMetadata {

		internal NWTcpMetadata (IntPtr handle, bool owns) : base (handle, owns) {}

		public uint AvailableReceiveBuffer => nw_tcp_get_available_receive_buffer (GetCheckedHandle ());

		public uint AvailableSendBuffer => nw_tcp_get_available_send_buffer (GetCheckedHandle ());
	}
}
