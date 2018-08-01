//
// NWProtocolOptions.cs: Bindings the Netowrk nw_protocol_options API.
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
using Security;
using OS_nw_protocol_definition=System.IntPtr;
using IntPtr=System.IntPtr;

namespace Network {

	public class NWProtocolOptions : NativeObject {
		public NWProtocolOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_options_copy_definition (IntPtr options);

		public NWProtocolDefinition ProtocolDefinition => new NWProtocolDefinition (nw_protocol_options_copy_definition (GetCheckedHandle ()), owns: true);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_tls_create_options ();

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWProtocolOptions CreateTls ()
		{
			return new NWProtocolOptions (nw_tls_create_options (), owns: true);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_tcp_create_options ();

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWProtocolOptions CreateTcp ()
		{
			return new NWProtocolOptions (nw_tcp_create_options (), owns: true);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_udp_create_options ();

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWProtocolOptions CreateUdp ()
		{
			return new NWProtocolOptions (nw_udp_create_options (), owns: true);
		}

//
// IP Options
//
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_version (IntPtr options, NWIPVersion version);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void IPSetVersion (NWIPVersion version)
		{
			nw_ip_options_set_version (GetCheckedHandle (), version);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_hop_limit (IntPtr options, byte hop_limit);

		public void IPSetHopLimit (byte hopLimit)
		{
			nw_ip_options_set_hop_limit (GetCheckedHandle (), hopLimit);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_use_minimum_mtu (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool use_minimum_mtu);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void IPSetUseMinimumMtu (bool useMinimumMtu)
		{
			nw_ip_options_set_use_minimum_mtu (GetCheckedHandle (), useMinimumMtu);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_disable_fragmentation (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool disable_fragmentation);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void IPSetDisableFragmentation (bool disableFragmentation)
		{
			nw_ip_options_set_disable_fragmentation (GetCheckedHandle (), disableFragmentation);
		}

//
// TCP Options
//
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_delay (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noDelay);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetNoDelay (bool noDelay) => nw_tcp_options_set_no_delay (GetCheckedHandle (), noDelay);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_push (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noPush);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetNoPush (bool noPush) => nw_tcp_options_set_no_push (GetCheckedHandle (), noPush);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_options (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noOptions);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetNoOptions (bool noOptions) => nw_tcp_options_set_no_options (GetCheckedHandle (), noOptions);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_enable_keepalive (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableKeepAlive);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetEnableKeepAlive (bool enableKeepAlive) => nw_tcp_options_set_enable_keepalive (GetCheckedHandle (), enableKeepAlive);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_count (IntPtr handle, uint keepaliveCount);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetKeepAliveCount (uint keepaliveCount) => nw_tcp_options_set_keepalive_count (GetCheckedHandle (), keepaliveCount);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_idle_time (IntPtr handle, uint keepaliveIdleTime);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetKeepAliveIdleTime (uint keepaliveIdleTime) => nw_tcp_options_set_keepalive_idle_time (GetCheckedHandle (), keepaliveIdleTime);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_interval (IntPtr handle, uint keepaliveInterval);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetKeepAliveInterval (uint keepaliveInterval) => nw_tcp_options_set_keepalive_interval (GetCheckedHandle (), keepaliveInterval);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_maximum_segment_size (IntPtr handle, uint maximumSegmentSize);
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetMaximumSegmentSize (uint maximumSegmentSize) => nw_tcp_options_set_maximum_segment_size (GetCheckedHandle (), maximumSegmentSize);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_connection_timeout (IntPtr handle, uint connectionTimeout);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetConnectionTimeout (uint connectionTimeout) => nw_tcp_options_set_connection_timeout (GetCheckedHandle (), connectionTimeout);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_persist_timeout (IntPtr handle, uint persistTimeout);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetPersistTimeout (uint persistTimeout) => nw_tcp_options_set_persist_timeout (GetCheckedHandle (), persistTimeout);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_retransmit_connection_drop_time (IntPtr handle, uint retransmitConnectionDropTime);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetRetransmitConnectionDropTime (uint retransmitConnectionDropTime) => nw_tcp_options_set_retransmit_connection_drop_time (GetCheckedHandle (), retransmitConnectionDropTime);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_retransmit_fin_drop (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool retransmitFinDrop);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetRetransmitFinDrop (bool retransmitFinDrop) => nw_tcp_options_set_retransmit_fin_drop (GetCheckedHandle (), retransmitFinDrop);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_disable_ack_stretching (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableAckStretching);
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetDisableAckStretching (bool disableAckStretching) => nw_tcp_options_set_disable_ack_stretching (GetCheckedHandle (), disableAckStretching);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_enable_fast_open (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableFastOpen);
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetEnableFastOpen (bool enableFastOpen) => nw_tcp_options_set_enable_fast_open (GetCheckedHandle (), enableFastOpen);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_disable_ecn (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableEcn);
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void TcpSetDisableEcn (bool disableEcn) => nw_tcp_options_set_disable_ecn (GetCheckedHandle (), disableEcn);

//
// UDP Options
//
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_udp_options_set_prefer_no_checksum (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool preferNoChecksums);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void UdpSetPreferNoChecksum (bool preferNoChecksums) => nw_udp_options_set_prefer_no_checksum (GetCheckedHandle (), preferNoChecksums);

//
// TLS options
//

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_tls_copy_sec_protocol_options (IntPtr options);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public SecProtocolOptions TlsProtocolOptions => new SecProtocolOptions (nw_tls_copy_sec_protocol_options (GetCheckedHandle ()), owns: true);
	}
}
