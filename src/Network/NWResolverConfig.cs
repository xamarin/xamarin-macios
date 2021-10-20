#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using System.Runtime.Versioning;

using OS_nw_resolver_config=System.IntPtr;
using OS_nw_endpoint=System.IntPtr; 

namespace Network {
	
	[Watch (8,0), TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	public class NWResolverConfig : NativeObject {

		public NWResolverConfig (IntPtr handle, bool owns) : base (handle, owns) {}
		
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_resolver_config nw_resolver_config_create_https (OS_nw_endpoint urlEndpoint);
		
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_resolver_config nw_resolver_config_create_tls (OS_nw_endpoint serverEndpoint);

		public NWResolverConfig (NWEndpoint urlEndpoint, NWResolverConfigEndpointType endpointType)
		{
			if (urlEndpoint is null)
				throw new ArgumentNullException (nameof (urlEndpoint));
			switch (endpointType) {
			case NWResolverConfigEndpointType.Https:
				InitializeHandle ( nw_resolver_config_create_https (urlEndpoint.Handle));
				break;
			case NWResolverConfigEndpointType.Tls:
				InitializeHandle ( nw_resolver_config_create_tls (urlEndpoint.Handle));
				break;
			default:
				throw new ArgumentOutOfRangeException ("Unknown endpoint type.");
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_resolver_config_add_server_address (OS_nw_resolver_config config, OS_nw_endpoint serverAddress);

		public void AddServerAddress (NWEndpoint serverAddress)
			=> nw_resolver_config_add_server_address (GetCheckedHandle (), serverAddress.Handle);
	}
}
