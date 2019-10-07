//
// NWProtocolTls: Bindings the Netowrk nw_protocol_options API focus on TLS options.
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
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

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	public enum NWIPLocalAddressPreference {
		Default = 0,
		Temporary = 1,
		Stable = 2,
	}

	public enum NWIPVersion {
		Any = 0,
		Version4 = 1,
		Version6 = 2,
	}

	[TV (13,0), Mac (10,15), iOS (13,0), Watch (6,0)]
	public class NWProtocolIPOptions : NWProtocolOptions {
		// default values, same are uset ins swift
		NWIPVersion version = NWIPVersion.Any;
		nuint hopLimit = (nuint) 0;
		bool useMinimumMtu = false;
		bool disableFragmentation = false;
		bool shouldCalculateReceiveTime = false;
		NWIPLocalAddressPreference localAddressPreference = NWIPLocalAddressPreference.Default;


		internal NWProtocolIPOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_version (IntPtr options, NWIPVersion version);

		public NWIPVersion Version {
			get => version;
			set {
				version = value;
				nw_ip_options_set_version (GetCheckedHandle (), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_hop_limit (IntPtr options, nuint hop_limit);

		public nuint HopLimit {
			get => hopLimit;
			set {
				hopLimit = value;
				nw_ip_options_set_hop_limit (GetCheckedHandle (), value);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_use_minimum_mtu (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool use_minimum_mtu);

		public bool UseMinimumMtu {
			get => useMinimumMtu;
			set {
				useMinimumMtu = value;
				nw_ip_options_set_use_minimum_mtu (GetCheckedHandle (), value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_disable_fragmentation (IntPtr options, [MarshalAs (UnmanagedType.I1)] bool disable_fragmentation);

		public bool DisableFragmentation {
			get => disableFragmentation;
			set {
				disableFragmentation = value;
				nw_ip_options_set_disable_fragmentation (GetCheckedHandle (), disableFragmentation);
			}
		} 

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_calculate_receive_time (IntPtr options, bool calculateReceiveTime);

		public bool CalculateReceiveTime {
			get => shouldCalculateReceiveTime;
			set {
				shouldCalculateReceiveTime = value;
				nw_ip_options_set_calculate_receive_time (GetCheckedHandle (), value);
			}
		} 

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_local_address_preference (IntPtr options, NWIPLocalAddressPreference preference);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		public NWIPLocalAddressPreference IPLocalAddressPreference {
			get => localAddressPreference;
			set {
				localAddressPreference = value;
				nw_ip_options_set_local_address_preference (GetCheckedHandle (), value);
			}
		}
	}
}