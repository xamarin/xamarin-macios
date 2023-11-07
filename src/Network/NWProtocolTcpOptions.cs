//
// NWProtocolTcp: Bindings the Netowrk nw_protocol_options API focus on Tcp options.
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
//
// Copyrigh 2019 Microsoft Inc
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
using OS_nw_protocol_options = System.IntPtr;
using IntPtr = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (6, 0)]
#endif
	public class NWProtocolTcpOptions : NWProtocolOptions {

		[Preserve (Conditional = true)]
		internal NWProtocolTcpOptions (NativeHandle handle, bool owns) : base (handle, owns) { }

		public NWProtocolTcpOptions () : this (nw_tcp_create_options (), owns: true) { }

		public void SetNoDelay (bool noDelay) => nw_tcp_options_set_no_delay (GetCheckedHandle (), noDelay);

		public void SetNoPush (bool noPush) => nw_tcp_options_set_no_push (GetCheckedHandle (), noPush);

		public void SetNoOptions (bool noOptions) => nw_tcp_options_set_no_options (GetCheckedHandle (), noOptions);

		public void SetEnableKeepAlive (bool enableKeepAlive) => nw_tcp_options_set_enable_keepalive (GetCheckedHandle (), enableKeepAlive);

		public void SetKeepAliveCount (uint keepAliveCount) => nw_tcp_options_set_keepalive_count (GetCheckedHandle (), keepAliveCount);

		public void SetKeepAliveIdleTime (TimeSpan keepAliveIdleTime)
			=> nw_tcp_options_set_keepalive_idle_time (GetCheckedHandle (), (uint) keepAliveIdleTime.Seconds);

		public void SetKeepAliveInterval (TimeSpan keepAliveInterval)
			=> nw_tcp_options_set_keepalive_interval (GetCheckedHandle (), (uint) keepAliveInterval.Seconds);

		public void SetMaximumSegmentSize (uint maximumSegmentSize)
			=> nw_tcp_options_set_maximum_segment_size (GetCheckedHandle (), maximumSegmentSize);

		public void SetConnectionTimeout (TimeSpan connectionTimeout)
			=> nw_tcp_options_set_connection_timeout (GetCheckedHandle (), (uint) connectionTimeout.Seconds);

		public void SetPersistTimeout (TimeSpan persistTimeout)
			=> nw_tcp_options_set_persist_timeout (GetCheckedHandle (), (uint) persistTimeout.Seconds);

		public void SetRetransmitConnectionDropTime (TimeSpan connectionDropTime)
			=> nw_tcp_options_set_retransmit_connection_drop_time (GetCheckedHandle (), (uint) connectionDropTime.Seconds);

		public void SetRetransmitFinDrop (bool retransmitFinDrop) => nw_tcp_options_set_retransmit_fin_drop (GetCheckedHandle (), retransmitFinDrop);

		public void SetDisableAckStretching (bool disableAckStretching)
			=> nw_tcp_options_set_disable_ack_stretching (GetCheckedHandle (), disableAckStretching);

		public void SetEnableFastOpen (bool enableFastOpen) => nw_tcp_options_set_enable_fast_open (GetCheckedHandle (), enableFastOpen);

		public void SetDisableEcn (bool disableEcn) => nw_tcp_options_set_disable_ecn (GetCheckedHandle (), disableEcn);

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
		static extern void nw_tcp_options_set_multipath_force_version (OS_nw_protocol_options options, NWMultipathVersion multipath_force_version);

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
		public void ForceMultipathVersion (NWMultipathVersion version)
			=> nw_tcp_options_set_multipath_force_version (GetCheckedHandle (), version);
	}
}
