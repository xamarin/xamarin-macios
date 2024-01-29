//
// NWProtocolOptions.cs: Bindings the Netowrk nw_protocol_options API.
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
using Security;
using OS_nw_protocol_definition = System.IntPtr;
using IntPtr = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Watch (6, 0)]
#endif
	public class NWProtocolOptions : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWProtocolOptions (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWProtocolOptions (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.NetworkLibrary)]
		internal static extern OS_nw_protocol_definition nw_protocol_options_copy_definition (IntPtr options);

		public NWProtocolDefinition ProtocolDefinition => new NWProtocolDefinition (nw_protocol_options_copy_definition (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		internal static extern IntPtr nw_tls_create_options ();

#if !NET
		[Obsolete ("Use the 'NWProtocolTlsOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateTls ()
		{
			return new NWProtocolTlsOptions (nw_tls_create_options (), owns: true);
		}
#endif

		[DllImport (Constants.NetworkLibrary)]
		internal static extern IntPtr nw_tcp_create_options ();

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateTcp ()
		{
			return new NWProtocolTcpOptions (nw_tcp_create_options (), owns: true);
		}
#endif

		[DllImport (Constants.NetworkLibrary)]
		internal static extern IntPtr nw_udp_create_options ();

#if !NET
		[Obsolete ("Use the 'NWProtocolUdpOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateUdp ()
		{
			return new NWProtocolUdpOptions (nw_udp_create_options (), owns: true);
		}
#endif

		// added to have a consistent API, but obsolete it

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		internal static extern IntPtr nw_quic_create_options ();

#if !NET
		[Obsolete ("Use the 'NWProtocolQuicOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateQuic ()
		{
			return new NWProtocolUdpOptions (nw_quic_create_options (), owns: true);
		}
#endif

		//
		// IP Options
		//
		[DllImport (Constants.NetworkLibrary)]
		internal static extern void nw_ip_options_set_version (IntPtr options, NWIPVersion version);

#if !NET
		[Obsolete ("Use the 'NWProtocolIPOptions' class instead (and the 'SetVersion' method).")]
		public void IPSetVersion (NWIPVersion version)
		{
			nw_ip_options_set_version (GetCheckedHandle (), version);
		}
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal static extern void nw_ip_options_set_hop_limit (IntPtr options, byte hop_limit);

#if !NET
		[Obsolete ("Use the 'NWProtocolIPOptions' class instead.")]
		public void IPSetHopLimit (byte hopLimit)
		{
			nw_ip_options_set_hop_limit (GetCheckedHandle (), hopLimit);
		}
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal static extern void nw_ip_options_set_use_minimum_mtu (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool use_minimum_mtu);

#if !NET
		[Obsolete ("Use the 'NWProtocolIPOptions' class instead.")]
		public void IPSetUseMinimumMtu (bool useMinimumMtu)
		{
			nw_ip_options_set_use_minimum_mtu (GetCheckedHandle (), useMinimumMtu);
		}
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal static extern void nw_ip_options_set_disable_fragmentation (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool disable_fragmentation);

#if !NET
		[Obsolete ("Use the 'NWProtocolIPOptions' class instead.")]
		public void IPSetDisableFragmentation (bool disableFragmentation)
		{
			nw_ip_options_set_disable_fragmentation (GetCheckedHandle (), disableFragmentation);
		}
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal static extern void nw_ip_options_set_calculate_receive_time (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool calculateReceiveTime);

#if !NET
		[Obsolete ("Use the 'NWProtocolIPOptions' class instead.")]
		public void IPSetCalculateReceiveTime (bool calculateReceiveTime)
		{
			nw_ip_options_set_calculate_receive_time (GetCheckedHandle (), calculateReceiveTime);
		}
#endif // !NET


#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		internal static extern void nw_ip_options_set_local_address_preference (IntPtr options, NWIPLocalAddressPreference preference);

#if !NET
		[TV (13, 0)]
		[iOS (13, 0)]
		[Obsolete ("Use the 'NWProtocolIPOptions' class instead.")]
#endif
		public NWIPLocalAddressPreference IPLocalAddressPreference {
			set => nw_ip_options_set_local_address_preference (GetCheckedHandle (), value);
		}
		//
		// TCP Options
		//

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_no_delay (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noDelay);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetNoDelay (bool noDelay) => nw_tcp_options_set_no_delay (GetCheckedHandle (), noDelay);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_no_push (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noPush);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetNoPush (bool noPush) => nw_tcp_options_set_no_push (GetCheckedHandle (), noPush);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_no_options (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noOptions);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetNoOptions (bool noOptions) => nw_tcp_options_set_no_options (GetCheckedHandle (), noOptions);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_enable_keepalive (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableKeepAlive);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetEnableKeepAlive (bool enableKeepAlive) => nw_tcp_options_set_enable_keepalive (GetCheckedHandle (), enableKeepAlive);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_keepalive_count (IntPtr handle, uint keepaliveCount);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetKeepAliveCount (uint keepaliveCount) => nw_tcp_options_set_keepalive_count (GetCheckedHandle (), keepaliveCount);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_keepalive_idle_time (IntPtr handle, uint keepaliveIdleTime);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetKeepAliveIdleTime (uint keepaliveIdleTime) => nw_tcp_options_set_keepalive_idle_time (GetCheckedHandle (), keepaliveIdleTime);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_keepalive_interval (IntPtr handle, uint keepaliveInterval);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetKeepAliveInterval (uint keepaliveInterval) => nw_tcp_options_set_keepalive_interval (GetCheckedHandle (), keepaliveInterval);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_maximum_segment_size (IntPtr handle, uint maximumSegmentSize);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetMaximumSegmentSize (uint maximumSegmentSize) => nw_tcp_options_set_maximum_segment_size (GetCheckedHandle (), maximumSegmentSize);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_connection_timeout (IntPtr handle, uint connectionTimeout);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetConnectionTimeout (uint connectionTimeout) => nw_tcp_options_set_connection_timeout (GetCheckedHandle (), connectionTimeout);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_persist_timeout (IntPtr handle, uint persistTimeout);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetPersistTimeout (uint persistTimeout) => nw_tcp_options_set_persist_timeout (GetCheckedHandle (), persistTimeout);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_retransmit_connection_drop_time (IntPtr handle, uint retransmitConnectionDropTime);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetRetransmitConnectionDropTime (uint retransmitConnectionDropTime) => nw_tcp_options_set_retransmit_connection_drop_time (GetCheckedHandle (), retransmitConnectionDropTime);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_retransmit_fin_drop (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool retransmitFinDrop);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetRetransmitFinDrop (bool retransmitFinDrop) => nw_tcp_options_set_retransmit_fin_drop (GetCheckedHandle (), retransmitFinDrop);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_disable_ack_stretching (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableAckStretching);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetDisableAckStretching (bool disableAckStretching) => nw_tcp_options_set_disable_ack_stretching (GetCheckedHandle (), disableAckStretching);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_enable_fast_open (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableFastOpen);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetEnableFastOpen (bool enableFastOpen) => nw_tcp_options_set_enable_fast_open (GetCheckedHandle (), enableFastOpen);
#endif // !NET

		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_tcp_options_set_disable_ecn (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableEcn);

#if !NET
		[Obsolete ("Use the 'NWProtocolTcpOptions' class instead.")]
		public void TcpSetDisableEcn (bool disableEcn) => nw_tcp_options_set_disable_ecn (GetCheckedHandle (), disableEcn);
#endif // !NET

		//
		// UDP Options
		//
		[DllImport (Constants.NetworkLibrary)]
		internal extern static void nw_udp_options_set_prefer_no_checksum (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool preferNoChecksums);

#if !NET
		[Obsolete ("Use the 'NWProtocolUdpOptions' class instead.")]
		public void UdpSetPreferNoChecksum (bool preferNoChecksums) => nw_udp_options_set_prefer_no_checksum (GetCheckedHandle (), preferNoChecksums);
#endif // !NET

		//
		// TLS options
		//

		[DllImport (Constants.NetworkLibrary)]
		internal extern static IntPtr nw_tls_copy_sec_protocol_options (IntPtr options);

#if !NET
		[Obsolete ("Use the 'NWProtocolTlsOptions' class instead.")]
		public SecProtocolOptions TlsProtocolOptions => new SecProtocolOptions (nw_tls_copy_sec_protocol_options (GetCheckedHandle ()), owns: true);
#endif

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_protocol_options_is_quic (IntPtr options);

		public bool IsQuic => nw_protocol_options_is_quic (GetCheckedHandle ());
	}
}
