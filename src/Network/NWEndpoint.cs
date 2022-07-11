//
// NWEndpoint.cs: Bindings the Netowrk nw_endpoint_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_endpoint=System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif

	public class NWEndpoint : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWEndpoint (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWEndpoint (NativeHandle handle, bool owns) : base (handle, owns) {}
#endif

		[DllImport (Constants.NetworkLibrary)]
		extern static NWEndpointType nw_endpoint_get_type (OS_nw_endpoint handle);

		public NWEndpointType Type => nw_endpoint_get_type (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static OS_nw_endpoint nw_endpoint_create_host (string hostname, string port);

		public static NWEndpoint? Create (string hostname, string port)
		{
			if (hostname is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (hostname));
			if (port is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (port));
			var handle = nw_endpoint_create_host (hostname, port);
			if (handle == IntPtr.Zero)
				return null;
			return new NWEndpoint (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_endpoint_get_hostname (OS_nw_endpoint endpoint);

		public string? Hostname => Marshal.PtrToStringAnsi (nw_endpoint_get_hostname (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_copy_port_string (OS_nw_endpoint endpoint);

		public string Port => nw_endpoint_copy_port_string (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern ushort nw_endpoint_get_port (OS_nw_endpoint endpoint);

		public ushort PortNumber => nw_endpoint_get_port (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_endpoint_create_address (IntPtr sockaddrPtr);

		// TODO: .NET has a SocketAddress type, we could use it for simplicity, but the
		// address family would have to be mapped, and it does not look like a very useful
		// type to begin with.

		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_copy_address_string (OS_nw_endpoint endpoint);

		public string Address => nw_endpoint_copy_address_string (GetCheckedHandle ());

#if false
	// need to sort out sockaddr binding.
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr /* struct sockaddr* */ nw_endpoint_get_address (OS_nw_endpoint endpoint);
#endif

		// TODO: same
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe OS_nw_endpoint nw_endpoint_create_bonjour_service (string name, string type, string domain);

		public static NWEndpoint? CreateBonjourService (string name, string serviceType, string domain)
		{
			if (serviceType is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (serviceType));
			var x = nw_endpoint_create_bonjour_service (name, serviceType, domain);
			if (x == IntPtr.Zero)
				return null;
			return new NWEndpoint (x, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe IntPtr nw_endpoint_get_bonjour_service_name (OS_nw_endpoint endpoint);

		public string? BonjourServiceName => Marshal.PtrToStringAnsi (nw_endpoint_get_bonjour_service_name (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_endpoint_get_bonjour_service_type (OS_nw_endpoint endpoint);

		public string? BonjourServiceType => Marshal.PtrToStringAnsi (nw_endpoint_get_bonjour_service_type (GetCheckedHandle ()));

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_endpoint_get_bonjour_service_domain (OS_nw_endpoint endpoint);

		public string? BonjourServiceDomain => Marshal.PtrToStringAnsi (nw_endpoint_get_bonjour_service_domain (GetCheckedHandle ()));

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern OS_nw_endpoint nw_endpoint_create_url (string url);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public static NWEndpoint? Create (string url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			var handle = nw_endpoint_create_url (url);
			if (handle == IntPtr.Zero)
				return null;
			return new NWEndpoint (handle, owns: true);
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.NetworkLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr nw_endpoint_get_url (OS_nw_endpoint endpoint);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public string? Url => Marshal.PtrToStringAnsi (nw_endpoint_get_url (GetCheckedHandle ()));
	}
}
