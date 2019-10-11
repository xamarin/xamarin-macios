//
// NWProtocolTcp: Bindings the Netowrk nw_protocol_options API focus on Tcp options.
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
	public class NWProtocolTcpOptions : NWProtocolOptions {
		
		internal NWProtocolTcpOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_tcp_create_options ();
		
		public NWProtocolTcpOptions () : this (nw_tcp_create_options (), owns: true) {}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_delay (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noDelay);

		public void SetNoDelay (bool noDelay) => nw_tcp_options_set_no_delay (GetCheckedHandle (), noDelay);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_push (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noPush);

		public void SetNoPush (bool noPush) => nw_tcp_options_set_no_push (GetCheckedHandle (), noPush);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_options (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noOptions);

		public void SetNoOptions (bool noOptions) => nw_tcp_options_set_no_options (GetCheckedHandle (), noOptions);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_enable_keepalive (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableKeepAlive);

		public void SetEnableKeepAlive (bool enableKeepAlive) =>  nw_tcp_options_set_enable_keepalive (GetCheckedHandle (), enableKeepAlive);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_count (IntPtr handle, uint keepaliveCount);

		public void SetKeepAliveCount (uint keepAliveCount) => nw_tcp_options_set_keepalive_count (GetCheckedHandle (), keepAliveCount);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_idle_time (IntPtr handle, uint keepAliveIdleTime);

		public void SetKeepAliveIdleTime (TimeSpan keepAliveIdleTime)
			=> nw_tcp_options_set_keepalive_idle_time (GetCheckedHandle (), (uint) keepAliveIdleTime.Seconds);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_interval (IntPtr handle, uint keepaliveInterval);

		public void SetKeepAliveInterval (TimeSpan keepAliveInterval)
			=> nw_tcp_options_set_keepalive_interval (GetCheckedHandle (), (uint) keepAliveInterval.Seconds);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_maximum_segment_size (IntPtr handle, uint maximumSegmentSize);

		public void SetMaximumSegmentSize (uint maximumSegmentSize)
			=> nw_tcp_options_set_maximum_segment_size (GetCheckedHandle (), maximumSegmentSize);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_connection_timeout (IntPtr handle, uint connectionTimeout);

		public void SetConnectionTimeout (TimeSpan connectionTimeout)
			=> nw_tcp_options_set_connection_timeout (GetCheckedHandle (), (uint) connectionTimeout.Seconds);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_persist_timeout (IntPtr handle, uint persistTimeout);

		public void SetPersistTimeout (TimeSpan persistTimeout)
			=> nw_tcp_options_set_persist_timeout (GetCheckedHandle (), (uint) persistTimeout.Seconds);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_retransmit_connection_drop_time (IntPtr handle, uint retransmitConnectionDropTime);

		public void SetRetransmitConnectionDropTime (TimeSpan connectionDropTime)
			=> nw_tcp_options_set_retransmit_connection_drop_time (GetCheckedHandle (), (uint) connectionDropTime.Seconds);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_retransmit_fin_drop (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool retransmitFinDrop);

		public void SetRetransmitFinDrop (bool retransmitFinDrop) => nw_tcp_options_set_retransmit_fin_drop (GetCheckedHandle (), retransmitFinDrop);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_disable_ack_stretching (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableAckStretching);

		public void SetDisableAckStretching (bool disableAckStretching)
			=> nw_tcp_options_set_disable_ack_stretching (GetCheckedHandle (), disableAckStretching);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_enable_fast_open (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableFastOpen);

		public void SetEnableFastOpen (bool enableFastOpen) => nw_tcp_options_set_enable_fast_open (GetCheckedHandle (), enableFastOpen);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_disable_ecn (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableEcn);

		public void SetDisableEcn (bool disableEcn) => nw_tcp_options_set_disable_ecn (GetCheckedHandle (), disableEcn);
	}
}
