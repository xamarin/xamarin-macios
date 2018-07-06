//
// NWEndpoint.cs: Bindings the Netowrk nw_endpoint_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_endpoint=System.IntPtr;

namespace Network {

	public enum NWEndpointType {
		Invalid = 0,
		Address = 1,
		Host = 2,
		BonjourService = 3
	}
	
	public class NWEndpoint : NativeObject {
		public NWEndpoint (IntPtr handle, bool owns) : base (handle, owns) {}

		[Mac(10,14)][iOS(12,0)][TV(12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static NWEndpointType nw_endpoint_get_type (OS_nw_endpoint handle);

		[Mac(10,14)][iOS(12,0)][TV(12,0)]
		public NWEndpointType Type => nw_endpoint_get_type (handle);

		[Mac(10,14)][iOS(12,0)][TV(12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static OS_nw_endpoint nw_endpoint_create_host (string hostname, string port);

		[Mac(10,14)][iOS(12,0)][TV(12,0)]
		public static NWEndpoint Create (string hostname, string port)
		{
			if (hostname == null)
				throw new ArgumentNullException (nameof (hostname));
			if (port == null)
				throw new ArgumentNullException (nameof (port));
			var handle = nw_endpoint_create_host (hostname, port);
			if (handle == IntPtr.Zero)
				return null;
			return new NWEndpoint (handle, owns: true);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_copy_port_string (OS_nw_endpoint endpoint);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public string Port => nw_endpoint_copy_port_string (handle);

		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern ushort nw_endpoint_get_port (OS_nw_endpoint endpoint);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public ushort PortNumber => nw_endpoint_get_port (handle);
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_endpoint_create_address (IntPtr sockaddrPtr);

		// TODO: .NET has a SocketAddress type, we could use it for simplicity, but the
		// address family would have to be mapped, and it does not look like a very useful
		// type to begin with.

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_copy_address_string (OS_nw_endpoint endpoint);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public string Address => nw_endpoint_copy_address_string (handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr /* struct sockaddr* */ nw_endpoint_get_address (OS_nw_endpoint endpoint);

		// TODO: same
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe OS_nw_endpoint nw_endpoint_create_bonjour_service (string name, string type, string domain);

		public static NWEndpoint CreateBonjourService (string name, string serviceType, string domain)
		{
			if (serviceType == null)
				throw new ArgumentNullException (nameof (serviceType));
			var x = nw_endpoint_create_bonjour_service (name, serviceType, domain);
			if (x == IntPtr.Zero)
				return null;
			return new NWEndpoint (x, owns: true);
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe IntPtr nw_endpoint_get_bonjour_service_name (OS_nw_endpoint endpoint);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public string BonjourServiceName => Marshal.PtrToStringAnsi (nw_endpoint_get_bonjour_service_name (handle));
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_endpoint_get_bonjour_service_type (OS_nw_endpoint endpoint);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public string BonjourServiceType => Marshal.PtrToStringAnsi (nw_endpoint_get_bonjour_service_type (handle));
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_endpoint_get_bonjour_service_domain (OS_nw_endpoint endpoint);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public string BonjourServiceDomain => Marshal.PtrToStringAnsi (nw_endpoint_get_bonjour_service_domain (handle));
		
	}
}
