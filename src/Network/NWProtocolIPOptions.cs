//
// NWProtocolTls: Bindings the Netowrk nw_protocol_options API focus on TLS options.
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
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (13, 0)]
	[iOS (13, 0)]
	[Watch (6, 0)]
#endif
	public class NWProtocolIPOptions : NWProtocolOptions {
		[Preserve (Conditional = true)]
		internal NWProtocolIPOptions (NativeHandle handle, bool owns) : base (handle, owns) { }

		public void SetVersion (NWIPVersion version)
			=> nw_ip_options_set_version (GetCheckedHandle (), version);

		public void SetHopLimit (nuint hopLimit)
			=> nw_ip_options_set_hop_limit (GetCheckedHandle (), (byte) hopLimit);

		public void SetUseMinimumMtu (bool useMinimumMtu)
			=> nw_ip_options_set_use_minimum_mtu (GetCheckedHandle (), useMinimumMtu);

		public void SetDisableFragmentation (bool disableFragmentation)
			=> nw_ip_options_set_disable_fragmentation (GetCheckedHandle (), disableFragmentation);

		public void SetCalculateReceiveTime (bool shouldCalculateReceiveTime)
			=> nw_ip_options_set_calculate_receive_time (GetCheckedHandle (), shouldCalculateReceiveTime);

		public void SetIPLocalAddressPreference (NWIPLocalAddressPreference localAddressPreference)
			=> nw_ip_options_set_local_address_preference (GetCheckedHandle (), localAddressPreference);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_disable_multicast_loopback (OS_nw_protocol_options options, [MarshalAs (UnmanagedType.I1)] bool disableMulticastLoopback);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public void DisableMulticastLoopback (bool disable)
			=> nw_ip_options_set_disable_multicast_loopback (GetCheckedHandle (), disable);
	}
}
