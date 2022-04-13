//
// NWEndpoint.cs: Bindings the Netowrk nw_endpoint_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_protocol_definition=System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public class NWProtocolDefinition : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWProtocolDefinition (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWProtocolDefinition (NativeHandle handle, bool owns) : base (handle, owns) {}
#endif

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

#if !NET
		[Obsolete ("Use 'CreateIPDefinition' method instead.")]
		public static NWProtocolDefinition IPDefinition => new NWProtocolDefinition (nw_protocol_copy_ip_definition (), owns: true);
#endif

		public static NWProtocolDefinition CreateIPDefinition () => new NWProtocolDefinition (nw_protocol_copy_ip_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_tcp_definition ();

#if !NET
		[Obsolete ("Use 'CreateTcpDefinition' method instead.")]
		public static NWProtocolDefinition TcpDefinition => new NWProtocolDefinition (nw_protocol_copy_tcp_definition (), owns: true);
#endif

		public static NWProtocolDefinition CreateTcpDefinition () => new NWProtocolDefinition (nw_protocol_copy_tcp_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_udp_definition ();

#if !NET
		[Obsolete ("Use 'CreateUdpDefinition' method instead.")]
		public static NWProtocolDefinition UdpDefinition => new NWProtocolDefinition (nw_protocol_copy_udp_definition (), owns: true);
#endif

		public static NWProtocolDefinition CreateUdpDefinition () => new NWProtocolDefinition (nw_protocol_copy_udp_definition (), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_tls_definition ();

#if !NET
		[Obsolete ("Use 'CreateTlsDefinition' method instead.")]
		public static NWProtocolDefinition TlsDefinition => new NWProtocolDefinition (nw_protocol_copy_tls_definition (), owns: true);
#endif

		public static NWProtocolDefinition CreateTlsDefinition () => new NWProtocolDefinition (nw_protocol_copy_tls_definition (), owns: true);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_ws_definition ();

#if !NET
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
		[Obsolete ("Use 'CreateWebSocketDefinition' method instead.")]
		public static NWProtocolDefinition WebSocketDefinition => new NWProtocolDefinition (nw_protocol_copy_ws_definition (), owns: true);
#endif

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public static NWProtocolDefinition CreateWebSocketDefinition () => new NWProtocolDefinition (nw_protocol_copy_ws_definition (), owns: true);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
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

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static NWProtocolDefinition CreateFramerDefinition (string identifier, NWFramerCreateFlags flags, Func<NWFramer, NWFramerStartResult> startCallback)
		{
			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_CreateFramerHandler, startCallback);
			try {
				return new NWProtocolDefinition (nw_framer_create_definition (identifier, flags, ref block_handler), owns: true);
			} finally {
				block_handler.CleanupBlock ();
			}
		} 
		
#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8,0)]
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_copy_quic_definition ();

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8,0)]
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		public static NWProtocolDefinition CreateQuicDefinition () => new NWProtocolDefinition (nw_protocol_copy_quic_definition (), owns: true);
	}
}
