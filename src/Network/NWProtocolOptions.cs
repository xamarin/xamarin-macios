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

	[TV (12,0), Mac (10,14), iOS (12,0)]
	[Watch (6,0)]
	public class NWProtocolOptions : NativeObject {
		public NWProtocolOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern OS_nw_protocol_definition nw_protocol_options_copy_definition (IntPtr options);

		public NWProtocolDefinition ProtocolDefinition => new NWProtocolDefinition (nw_protocol_options_copy_definition (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		protected static extern IntPtr nw_tls_create_options ();

		[Obsolete ("Use the 'NWProtocolTlsOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateTls ()
		{
			return new NWProtocolTlsOptions (nw_tls_create_options (), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern IntPtr nw_tcp_create_options ();

		[Obsolete ("Use the 'NWProptocolTcpOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateTcp ()
		{
			return new NWProtocolTcpOptions (nw_tcp_create_options (), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern IntPtr nw_udp_create_options ();

		[Obsolete ("Use the 'NWProptocolUdpOptions' class methods and constructors instead.")]
		public static NWProtocolOptions CreateUdp ()
		{
			return new NWProtocolUdpOptions (nw_udp_create_options (), owns: true);
		}

//
// IP Options
//
		[DllImport (Constants.NetworkLibrary)]
		protected static extern void nw_ip_options_set_version (IntPtr options, NWIPVersion version);

		[Obsolete ("Use the 'NWProptocolIPOptions' class instead.")]
		public void IPSetVersion (NWIPVersion version)
		{
			nw_ip_options_set_version (GetCheckedHandle (), version);
		}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern void nw_ip_options_set_hop_limit (IntPtr options, byte hop_limit);

		[Obsolete ("Use the 'NWProptocolIPOptions' class instead.")]
		public void IPSetHopLimit (byte hopLimit)
		{
			nw_ip_options_set_hop_limit (GetCheckedHandle (), hopLimit);
		}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern void nw_ip_options_set_use_minimum_mtu (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool use_minimum_mtu);

		[Obsolete ("Use the 'NWProptocolIPOptions' class instead.")]
		public void IPSetUseMinimumMtu (bool useMinimumMtu)
		{
			nw_ip_options_set_use_minimum_mtu (GetCheckedHandle (), useMinimumMtu);
		}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern void nw_ip_options_set_disable_fragmentation (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool disable_fragmentation);

		[Obsolete ("Use the 'NWProptocolIPOptions' class instead.")]
		public void IPSetDisableFragmentation (bool disableFragmentation)
		{
			nw_ip_options_set_disable_fragmentation (GetCheckedHandle (), disableFragmentation);
		}

		[DllImport (Constants.NetworkLibrary)]
		protected static extern void nw_ip_options_set_calculate_receive_time (IntPtr options, bool calculateReceiveTime);

		[Obsolete ("Use the 'NWProptocolIPOptions' class instead.")]
		public void IPSetCalculateReceiveTime (bool calculateReceiveTime)
		{
			nw_ip_options_set_calculate_receive_time (GetCheckedHandle (), calculateReceiveTime);
		}


		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.NetworkLibrary)]
		protected static extern void nw_ip_options_set_local_address_preference (IntPtr options, NWIPLocalAddressPreference preference);

		[Obsolete ("Use the 'NWProptocolIPOptions' class instead.")]
		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		public NWIPLocalAddressPreference IPLocalAddressPreference {
			set => nw_ip_options_set_local_address_preference (GetCheckedHandle (), value);
		}
//
// TCP Options
//

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_no_delay (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noDelay);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetNoDelay (bool noDelay) => nw_tcp_options_set_no_delay (GetCheckedHandle (), noDelay);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_no_push (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noPush);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetNoPush (bool noPush) => nw_tcp_options_set_no_push (GetCheckedHandle (), noPush);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_no_options (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noOptions);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetNoOptions (bool noOptions) => nw_tcp_options_set_no_options (GetCheckedHandle (), noOptions);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_enable_keepalive (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableKeepAlive);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetEnableKeepAlive (bool enableKeepAlive) => nw_tcp_options_set_enable_keepalive (GetCheckedHandle (), enableKeepAlive);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_keepalive_count (IntPtr handle, uint keepaliveCount);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetKeepAliveCount (uint keepaliveCount) => nw_tcp_options_set_keepalive_count (GetCheckedHandle (), keepaliveCount);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_keepalive_idle_time (IntPtr handle, uint keepaliveIdleTime);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetKeepAliveIdleTime (uint keepaliveIdleTime) => nw_tcp_options_set_keepalive_idle_time (GetCheckedHandle (), keepaliveIdleTime);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_keepalive_interval (IntPtr handle, uint keepaliveInterval);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetKeepAliveInterval (uint keepaliveInterval) => nw_tcp_options_set_keepalive_interval (GetCheckedHandle (), keepaliveInterval);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_maximum_segment_size (IntPtr handle, uint maximumSegmentSize);
		
		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetMaximumSegmentSize (uint maximumSegmentSize) => nw_tcp_options_set_maximum_segment_size (GetCheckedHandle (), maximumSegmentSize);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_connection_timeout (IntPtr handle, uint connectionTimeout);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetConnectionTimeout (uint connectionTimeout) => nw_tcp_options_set_connection_timeout (GetCheckedHandle (), connectionTimeout);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_persist_timeout (IntPtr handle, uint persistTimeout);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetPersistTimeout (uint persistTimeout) => nw_tcp_options_set_persist_timeout (GetCheckedHandle (), persistTimeout);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_retransmit_connection_drop_time (IntPtr handle, uint retransmitConnectionDropTime);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetRetransmitConnectionDropTime (uint retransmitConnectionDropTime) => nw_tcp_options_set_retransmit_connection_drop_time (GetCheckedHandle (), retransmitConnectionDropTime);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_retransmit_fin_drop (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool retransmitFinDrop);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetRetransmitFinDrop (bool retransmitFinDrop) => nw_tcp_options_set_retransmit_fin_drop (GetCheckedHandle (), retransmitFinDrop);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_disable_ack_stretching (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableAckStretching);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetDisableAckStretching (bool disableAckStretching) => nw_tcp_options_set_disable_ack_stretching (GetCheckedHandle (), disableAckStretching);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_enable_fast_open (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableFastOpen);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetEnableFastOpen (bool enableFastOpen) => nw_tcp_options_set_enable_fast_open (GetCheckedHandle (), enableFastOpen);

		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_tcp_options_set_disable_ecn (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableEcn);

		[Obsolete ("Use the 'NWProptocolTcpOptions' class instead.")]
		public void TcpSetDisableEcn (bool disableEcn) => nw_tcp_options_set_disable_ecn (GetCheckedHandle (), disableEcn);

//
// UDP Options
//
		[DllImport (Constants.NetworkLibrary)]
		protected extern static void nw_udp_options_set_prefer_no_checksum (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool preferNoChecksums);

		[Obsolete ("Use the 'NWProptocolUdpOptions' class instead.")]
		public void UdpSetPreferNoChecksum (bool preferNoChecksums) => nw_udp_options_set_prefer_no_checksum (GetCheckedHandle (), preferNoChecksums);

//
// TLS options
//

		[DllImport (Constants.NetworkLibrary)]
		protected extern static IntPtr nw_tls_copy_sec_protocol_options (IntPtr options);

		[Obsolete ("Use the 'NWProtocolTlsOptions' class instead.")]
		public SecProtocolOptions TlsProtocolOptions => new SecProtocolOptions (nw_tls_copy_sec_protocol_options (GetCheckedHandle ()), owns: true);
	}
}
