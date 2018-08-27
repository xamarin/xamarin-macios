//
// NWEndpoint.cs: Bindings the Netowrk nw_endpoint_t API.
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

using OS_nw_protocol_definition=System.IntPtr;

namespace Network {

	public enum NWIPVersion {
		Any = 0,
		Version4 = 1,
		Version6 = 2,
	}

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class NWProtocolDefinition : NativeObject {
		public NWProtocolDefinition (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_protocol_definition_is_equal (OS_nw_protocol_definition definition1, OS_nw_protocol_definition definition2);

		public bool Equals (object other)
		{
			if (other == null)
				return false;
			if (!(other is NWProtocolDefinition otherDefinition))
				return false;
			return nw_protocol_definition_is_equal (GetCheckedHandle (), otherDefinition.Handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_ip_definition ();

		public static NWProtocolDefinition IPDefinition => new NWProtocolDefinition (nw_protocol_copy_ip_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_tcp_definition ();

		public static NWProtocolDefinition TcpDefinition => new NWProtocolDefinition (nw_protocol_copy_tcp_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_udp_definition ();

		public static NWProtocolDefinition UdpDefinition => new NWProtocolDefinition (nw_protocol_copy_udp_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_tls_definition ();

		public static NWProtocolDefinition TlsDefinition => new NWProtocolDefinition (nw_protocol_copy_tls_definition (), owns: true);

	}
}
