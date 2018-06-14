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
	
	public class NWEndpoint : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWEndpoint (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWEndpoint (IntPtr handle) : this (handle, false)
		{
		}

		~NWEndpoint ()
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

		[Mac(10,14)][iOS(12,0)][Watch(5,0)][TV(12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static NWEndpointType nw_endpoint_get_type (OS_nw_endpoint handle);

		[Mac(10,14)][iOS(12,0)][Watch(5,0)][TV(12,0)]
		public NWEndpointType Type => nw_endpoint_get_type (handle);

		[Mac(10,14)][iOS(12,0)][Watch(5,0)][TV(12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static OS_nw_endpoint nw_endpoint_create_host (string hostname, string port);

		[Mac(10,14)][iOS(12,0)][Watch(5,0)][TV(12,0)]
		public NWEndpoint Create (string hostname, string port)
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

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_copy_port_string (OS_nw_endpoint endpoint);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public string Port => nw_endpoint_copy_port_string (handle);

		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern ushort nw_endpoint_get_port (OS_nw_endpoint endpoint);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public ushort PortNumber => nw_endpoint_get_port (handle);
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_endpoint nw_endpoint_create_address (IntPtr sockaddrPtr);

		// TODO: .NET has a SocketAddress type, we could use it for simplicity, but the
		// address family would have to be mapped, and it does not look like a very useful
		// type to begin with.

		// extern char * _Nonnull nw_endpoint_copy_address_string (nw_endpoint_t _Nonnull endpoint) __attribute__((availability(tvos, introduced=12.0))) __attribute__((availability(watchos, introduced=5.0))) __attribute__((availability(ios, introduced=12.0))) __attribute__((availability(macos, introduced=10.14)));
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_copy_address_string (OS_nw_endpoint endpoint);
		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public string Address => nw_endpoint_copy_address_string (handle);
		
		// extern const struct sockaddr * _Nonnull nw_endpoint_get_address (nw_endpoint_t _Nonnull endpoint) __attribute__((availability(tvos, introduced=12.0))) __attribute__((availability(watchos, introduced=5.0))) __attribute__((availability(ios, introduced=12.0))) __attribute__((availability(macos, introduced=10.14)));
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr /* struct sockaddr* */ nw_endpoint_get_address (OS_nw_endpoint endpoint);

		// TODO: same
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe OS_nw_endpoint nw_endpoint_create_bonjour_service (string name, string type, string domain);

		public NWEndpoint CreateBonjourService (string name, string serviceType, string domain)
		{
			if (serviceType == null)
				throw new ArgumentNullException (nameof (serviceType));
			var x = nw_endpoint_create_bonjour_service (name, serviceType, domain);
			return new NWEndpoint (x, owns: true);
		}
		
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe string nw_endpoint_get_bonjour_service_name (OS_nw_endpoint endpoint);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public string BonjourServiceName => nw_endpoint_get_bonjour_service_name (handle);


		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_get_bonjour_service_type (OS_nw_endpoint endpoint);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public string BonjourSericeType => nw_endpoint_get_bonjour_service_type (handle);


		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern string nw_endpoint_get_bonjour_service_domain (OS_nw_endpoint endpoint);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public string BonjourServiceDomain => nw_endpoint_get_bonjour_service_domain (handle);
		
	}
}
