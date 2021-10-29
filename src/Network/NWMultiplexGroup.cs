using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using OS_nw_group_descriptor=System.IntPtr;
using OS_nw_endpoint=System.IntPtr;

#nullable enable

namespace Network {

#if !NET
	[Watch (8,0), TV (15,0), Mac (12,0), iOS (15,0)]
#else
	[SupportedOSPlatform ("ios15.0"), SupportedOSPlatform ("tvos15.0"), SupportedOSPlatform ("macos12.0"), SupportedOSPlatform ("maccatalyst")]
#endif
	public class NWMultiplexGroup : NWMulticastGroup {

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_group_descriptor nw_group_descriptor_create_multiplex (OS_nw_endpoint remoteEndpoint);
		
		internal NWMultiplexGroup (IntPtr handle, bool owns) : base (handle, owns) {}

		public NWMultiplexGroup (NWEndpoint endpoint) 
			: base (nw_group_descriptor_create_multiplex (endpoint.GetCheckedHandle ()), true)  { }
	}
}
