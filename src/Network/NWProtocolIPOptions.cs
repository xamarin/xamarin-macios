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
using OS_nw_protocol_definition=System.IntPtr;
using IntPtr=System.IntPtr;

namespace Network {

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	public enum NWIPLocalAddressPreference {
		Default = 0,
		Temporary = 1,
		Stable = 2,
	}

	[Watch (6,0), TV (12,0), Mac (10,14), iOS (12,0)]
	public enum NWIPVersion {
		Any = 0,
		Version4 = 1,
		Version6 = 2,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWProtocolIPOptions : NWProtocolOptions {
		internal NWProtocolIPOptions (IntPtr handle, bool owns) : base (handle, owns) {}

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
	}
}