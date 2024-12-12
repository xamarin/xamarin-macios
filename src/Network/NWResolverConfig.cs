#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using CoreFoundation;

using OS_nw_resolver_config = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

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
	public class NWResolverConfig : NativeObject {

		[Preserve (Conditional = true)]
#if NET
		internal NWResolverConfig (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWResolverConfig (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_resolver_config nw_resolver_config_create_https (OS_nw_endpoint urlEndpoint);

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_resolver_config nw_resolver_config_create_tls (OS_nw_endpoint serverEndpoint);

		public NWResolverConfig (NWEndpoint urlEndpoint, NWResolverConfigEndpointType endpointType)
		{
			if (urlEndpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (urlEndpoint));
			switch (endpointType) {
			case NWResolverConfigEndpointType.Https:
				InitializeHandle (nw_resolver_config_create_https (urlEndpoint.Handle));
				break;
			case NWResolverConfigEndpointType.Tls:
				InitializeHandle (nw_resolver_config_create_tls (urlEndpoint.Handle));
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown endpoint type: {endpointType}");
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_resolver_config_add_server_address (OS_nw_resolver_config config, OS_nw_endpoint serverAddress);

		public void AddServerAddress (NWEndpoint serverAddress)
			=> nw_resolver_config_add_server_address (GetCheckedHandle (), serverAddress.Handle);
	}
}
