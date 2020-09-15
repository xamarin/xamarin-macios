//
// NWUdpMetadata.cs: Bindings the Netowrk nw_protocol_metadata_t API that is an Udp.
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
using Security;
using CoreFoundation;

namespace Network {

	[TV (12,0), Mac (10,14), iOS (12,0), Watch (6,0)]
	public class NWUdpMetadata : NWProtocolMetadata {

		internal NWUdpMetadata (IntPtr handle, bool owns) : base (handle, owns) {}

		public NWUdpMetadata () : this (nw_udp_create_metadata (), owns: true) {}
	}
}

