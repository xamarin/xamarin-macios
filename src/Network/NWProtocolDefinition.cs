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

	[TV (12,0), Mac (10,14), iOS (12,0)]
	[Watch (6,0)]
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

		[Obsolete ("Use 'CreateIPDefinition' method instead.")]
		public static NWProtocolDefinition IPDefinition => new NWProtocolDefinition (nw_protocol_copy_ip_definition (), owns: true);

		public static NWProtocolDefinition CreateIPDefinition () => new NWProtocolDefinition (nw_protocol_copy_ip_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_tcp_definition ();

		[Obsolete ("Use 'CreateTcpDefinition' method instead.")]
		public static NWProtocolDefinition TcpDefinition => new NWProtocolDefinition (nw_protocol_copy_tcp_definition (), owns: true);

		public static NWProtocolDefinition CreateTcpDefinition () => new NWProtocolDefinition (nw_protocol_copy_tcp_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_udp_definition ();

		[Obsolete ("Use 'CreateUdpDefinition' method instead.")]
		public static NWProtocolDefinition UdpDefinition => new NWProtocolDefinition (nw_protocol_copy_udp_definition (), owns: true);

		public static NWProtocolDefinition CreateUdpDefinition () => new NWProtocolDefinition (nw_protocol_copy_udp_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_tls_definition ();

		[Obsolete ("Use 'CreateTlsDefinition' method instead.")]
		public static NWProtocolDefinition TlsDefinition => new NWProtocolDefinition (nw_protocol_copy_tls_definition (), owns: true);

		public static NWProtocolDefinition CreateTlsDefinition () => new NWProtocolDefinition (nw_protocol_copy_tls_definition (), owns: true);

		[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_ws_definition ();

		[Obsolete ("Use 'CreateWebSocketDefinition' method instead.")]
		[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
		public static NWProtocolDefinition WebSocketDefinition => new NWProtocolDefinition (nw_protocol_copy_ws_definition (), owns: true);

		[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
		public static NWProtocolDefinition CreateWebSocketDefinition () => new NWProtocolDefinition (nw_protocol_copy_ws_definition (), owns: true);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe OS_nw_protocol_definition nw_framer_create_definition (string identifier, NWFramerCreateFlags flags, ref BlockLiteral start_handler);
		delegate NWFramerStartResult nw_framer_create_definition_t (IntPtr block, IntPtr framer);
		static nw_framer_create_definition_t static_CreateFramerHandler = TrampolineCreateFramerHandler;

		[MonoPInvokeCallback (typeof (nw_framer_create_definition_t))]
		static NWFramerStartResult TrampolineCreateFramerHandler (IntPtr block, IntPtr framer)
		{
			// get and call, this is internal and we are trying to do all the magic in the call
			var del = BlockLiteral.GetTarget<Func<NWFramer,NWFramerStartResult>> (block);
			if (del != null) {
				var nwFramer = new NWFramer (framer, owns: true);
				return del (nwFramer);
			}
			return NWFramerStartResult.Unknown;
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static NWProtocolDefinition CreateFramerDefinition (string identifier, NWFramerCreateFlags flags, Func<NWFramer,NWFramerStartResult> startCallback)
		{
			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_CreateFramerHandler, startCallback);
			try {
				return new NWProtocolDefinition (nw_framer_create_definition (identifier, flags, ref block_handler), owns: true);
			} finally {
				block_handler.CleanupBlock ();
			}
		} 
	}
}
