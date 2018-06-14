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
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_protocol_definition=System.IntPtr;
using OS_nw_protocol_options=System.IntPtr;

namespace Network {
	
	public class NWProtocolOptions : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWProtocolOptions (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWProtocolOptions (IntPtr handle) : this (handle, false)
		{
		}

		~NWProtocolOptions ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_protocol_definition nw_protocol_options_copy_definition (OS_nw_protocol_options options);

		public NWProtocolDefinition ProtocolDefinition => new NWProtocolDefinition (nw_protocol_options_copy_definition (handle), owns: true);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_version (OS_nw_protocol_options options, NWIPVersion version);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetVersion (NWIPVersion version)
		{
			nw_ip_options_set_version (handle, version);
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_hop_limit (OS_nw_protocol_options options, byte hop_limit);
	
		public void SetHopLimit (byte hopLimit)
		{
			nw_ip_options_set_hop_limit (handle, hopLimit);
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_use_minimum_mtu (OS_nw_protocol_options options, bool use_minimum_mtu);
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetUseMinimumMtu (bool useMinimumMtu)
		{
			nw_ip_options_set_use_minimum_mtu (handle, useMinimumMtu);
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_ip_options_set_disable_fragmentation (OS_nw_protocol_options options, bool disable_fragmentation);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetDisableFragmentation (bool disableFragmentation)
		{
			nw_ip_options_set_disable_fragmentation (handle, disableFragmentation);
		}

		
	}
}
