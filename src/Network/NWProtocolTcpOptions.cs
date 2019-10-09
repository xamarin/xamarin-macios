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
		// default values, same are used in swift
		bool noDelay = false;
		bool noPush = false;
		bool noOptions = false;
		bool enableKeepAlive = false;
		uint keepAliveCount = 0;
		TimeSpan keepAliveIdleTime = TimeSpan.Zero;
		TimeSpan keepAliveInterval = TimeSpan.Zero;
		uint maximumSegmentSize = 0;
		TimeSpan connectionTimeout = TimeSpan.Zero;
		TimeSpan persistTimeout = TimeSpan.Zero;
		TimeSpan connectionDropTime = TimeSpan.Zero;
		bool retransmitFinDrop = false;
		bool disableAckStretching = false;
		bool enableFastOpen = false;
		bool disableEcn = false;

		
		internal NWProtocolTcpOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_tcp_create_options ();
		
		public NWProtocolTcpOptions () : this (nw_tcp_create_options (), owns: true) {}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_delay (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noDelay);

		public bool NoDelay {
			get => noDelay;
			set {
				noDelay = value;
				nw_tcp_options_set_no_delay (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_push (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noPush);

		public bool NoPush {
			get => noPush;
			set {
				noPush = value;
				nw_tcp_options_set_no_push (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_no_options (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool noOptions);

		public bool NoOptions {
			get => noOptions;
			set {
				noOptions = value;
				nw_tcp_options_set_no_options (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_enable_keepalive (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableKeepAlive);

		public bool EnableKeepAlive {
			get => enableKeepAlive;
			set {
				enableKeepAlive = value;
				nw_tcp_options_set_enable_keepalive (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_count (IntPtr handle, uint keepaliveCount);

		public uint KeepAliveCount {
			get => keepAliveCount;
			set {
				keepAliveCount = value;
				nw_tcp_options_set_keepalive_count (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_idle_time (IntPtr handle, uint keepAliveIdleTime);

		public TimeSpan KeepAliveIdleTime {
			get => keepAliveIdleTime;
			set {
				keepAliveIdleTime = value;
				nw_tcp_options_set_keepalive_idle_time (GetCheckedHandle (), (uint) value.Seconds);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_keepalive_interval (IntPtr handle, uint keepaliveInterval);

		public TimeSpan KeepAliveInterval {
			get => keepAliveInterval;
			set {
				keepAliveInterval = value;
				nw_tcp_options_set_keepalive_interval (GetCheckedHandle (), (uint) value.Seconds);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_maximum_segment_size (IntPtr handle, uint maximumSegmentSize);

		public uint MaximumSegmentSize {
			get => maximumSegmentSize;
			set {
				maximumSegmentSize = value;
				nw_tcp_options_set_maximum_segment_size (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_connection_timeout (IntPtr handle, uint connectionTimeout);

		public TimeSpan ConnectionTimeout {
			get => connectionTimeout;
			set {
				connectionTimeout = value;
				nw_tcp_options_set_connection_timeout (GetCheckedHandle (), (uint) value.Seconds);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_persist_timeout (IntPtr handle, uint persistTimeout);

		public TimeSpan PersistTimeout {
			get => persistTimeout;
			set {
				persistTimeout = value;
				nw_tcp_options_set_persist_timeout (GetCheckedHandle (), (uint) value.Seconds);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_retransmit_connection_drop_time (IntPtr handle, uint retransmitConnectionDropTime);

		public TimeSpan RetransmitConnectionDropTime {
			get => connectionDropTime;
			set {
				connectionDropTime = value;
				nw_tcp_options_set_retransmit_connection_drop_time (GetCheckedHandle (), (uint) value.Seconds);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_retransmit_fin_drop (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool retransmitFinDrop);

		public bool RetransmitFinDrop {
			get => retransmitFinDrop;
			set {
				retransmitFinDrop = value;
				nw_tcp_options_set_retransmit_fin_drop (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_disable_ack_stretching (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableAckStretching);

		public bool DisableAckStretching {
			get => disableAckStretching;
			set {
				disableAckStretching = value;
				nw_tcp_options_set_disable_ack_stretching (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_enable_fast_open (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool enableFastOpen);
		public bool EnableFastOpen {
			get => enableFastOpen;
			set {
				enableFastOpen = value;
				nw_tcp_options_set_enable_fast_open (GetCheckedHandle (), enableFastOpen);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_tcp_options_set_disable_ecn (IntPtr handle, [MarshalAs (UnmanagedType.U1)] bool disableEcn);

		public bool DisableEcn {
			get => disableEcn;
			set {
				disableEcn = value;
				nw_tcp_options_set_disable_ecn (GetCheckedHandle (), disableEcn);
			}
		} 
	}
}
