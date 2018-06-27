//
// NWParameter.cs: Bindings the Network nw_parameters_t API.
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

using nw_parameters_t=System.IntPtr;

namespace Network {
	public enum NWMultiPathService {
		Disabled = 0,
		Handover = 1,
		Interactive = 2,
		Aggregate = 3, 
	}
	
	public class NWParameters : INativeObject, IDisposable {
		IntPtr handle;
		
		public IntPtr Handle {
			get { return handle; }
		}

		public NWParameters (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CoreFoundation.CFObject.CFRetain (handle);
		}

		public NWParameters (IntPtr handle) : this (handle, false)
		{
		}

		~NWParameters ()
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

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_create ();

		public NWParameters ()
		{
			handle = nw_parameters_create ();
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_copy (nw_parameters_t handle);

		public NWParameters Clone ()
		{
			return new NWParameters (nw_parameters_copy (handle), owns: true);
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		static extern void nw_parameters_set_multipath_service (nw_parameters_t parameters, NWMultiPathService multipath_service);
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWMultiPathService nw_parameters_get_multipath_service (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWMultiPathService MultipathService {
			get => nw_parameters_get_multipath_service (handle);
			set => nw_parameters_set_multipath_service (handle, value);
		}

		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_default_protocol_stack (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWProtocolStack ProtocolStack => new NWProtocolStack (nw_parameters_copy_default_protocol_stack (handle), owns: true);
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_only (nw_parameters_t parameters, [MarshalAs(UnmanagedType.I1)] bool local_only);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]			
		static extern bool nw_parameters_get_local_only (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool LocalOnly {
			get => nw_parameters_get_local_only (handle);
			set => nw_parameters_set_local_only (handle, value);
		}
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prefer_no_proxy (nw_parameters_t parameters, [MarshalAs(UnmanagedType.I1)] bool prefer_no_proxy);
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_prefer_no_proxy (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool PreferNoProxy {
			get => nw_parameters_get_prefer_no_proxy (handle);
			set => nw_parameters_set_prefer_no_proxy (handle, value);
		}
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		static extern void nw_parameters_set_expired_dns_behavior (nw_parameters_t parameters, NWParametersExpiredDnsBehavior expired_dns_behavior);
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWParametersExpiredDnsBehavior nw_parameters_get_expired_dns_behavior (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWParametersExpiredDnsBehavior ExpiredDnsBehavior {
			get => nw_parameters_get_expired_dns_behavior (handle);
			set => nw_parameters_set_expired_dns_behavior (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_require_interface (nw_parameters_t parameters, IntPtr handleInterface);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void RequireInterface (NWInterface iface)
		{
			nw_parameters_require_interface (handle, iface == null ? IntPtr.Zero : iface.handle);
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_required_interface (nw_parameters_t parameters);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWInterface Interface {
			get {
				var iface = nw_parameters_copy_required_interface (handle);

				if (iface == IntPtr.Zero)
					return null;

				return new NWInterface (handle, owns: true);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_prohibit_interface (nw_parameters_t parameters, IntPtr handleInterface);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void ProhibitInterface (NWInterface iface)
		{
			if (iface == null)
				throw new ArgumentNullException (nameof (iface));

			nw_parameters_prohibit_interface (handle, iface.handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_clear_prohibited_interfaces (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void ClearProhibitedInterfaces ()
		{
			nw_parameters_clear_prohibited_interfaces (handle);
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_required_interface_type (nw_parameters_t parameters, NWInterfaceType ifaceType);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWInterfaceType nw_parameters_get_required_interface_type (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWInterfaceType RequiredIntefaceType {
			get => nw_parameters_get_required_interface_type (handle);
			set => nw_parameters_set_required_interface_type (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_prohibit_interface_type (nw_parameters_t parameters, NWInterfaceType type);
		

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void ProhibitInterfaceType (NWInterfaceType ifaceType)
		{
			nw_parameters_prohibit_interface_type (handle, ifaceType);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_clear_prohibited_interface_types (nw_parameters_t parameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void ClearProhibitedInterfaceTypes ()
		{
			nw_parameters_clear_prohibited_interface_types (handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_prohibit_expensive (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prohibit_expensive (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool prohibit_expensive);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool ProhibitExpensive {
			get => nw_parameters_get_prohibit_expensive (handle);
			set => nw_parameters_set_prohibit_expensive (handle, value);
		}
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_reuse_local_address (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_reuse_local_address (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool reuse_local_address);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool ReuseLocalAddress {
			get => nw_parameters_get_reuse_local_address (handle);
			set => nw_parameters_set_reuse_local_address (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_fast_open_enabled (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_fast_open_enabled (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool fast_open_enabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool FastOpenEnabled {
			get => nw_parameters_get_fast_open_enabled (handle);
			set => nw_parameters_set_fast_open_enabled (handle, value);
		}
	
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWServiceClass nw_parameters_get_service_class (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_service_class (IntPtr handle, NWServiceClass service_class);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWServiceClass ServiceClass {
			get => nw_parameters_get_service_class (handle);
			set => nw_parameters_set_service_class (handle, value);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_local_endpoint (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_endpoint (IntPtr handle, IntPtr endpoint);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWEndpoint Endpoint {
			get {
				var x = nw_parameters_copy_local_endpoint (handle);
				if (x == IntPtr.Zero)
					return null;

				return new NWEndpoint (handle, owns: true);
			}
			
			set {
				nw_parameters_set_local_endpoint (handle, value == null ? IntPtr.Zero : value.handle);
			}
		}
	}

	public enum NWParametersExpiredDnsBehavior {
		Default = 0,
		Allow = 1,
		Prohibit = 2
	}
}
