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

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_create ();

		public NWParameters ()
		{
			handle = nw_parameters_create ();
		}

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_copy (nw_parameters_t handle);

		public NWParameters Clone ()
		{
			return new NWParameters (nw_parameters_copy (handle), owns: true);
		}
		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		static extern void nw_parameters_set_multipath_service (nw_parameters_t parameters, NWMultiPathService multipath_service);
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWMultiPathService nw_parameters_get_multipath_service (nw_parameters_t parameters);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public NWMultiPathService MultipathService {
			get => nw_parameters_get_multipath_service (handle);
			set => nw_parameters_set_multipath_service (handle, value);
		}

		
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_default_protocol_stack (nw_parameters_t parameters);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public NWProtocolStack ProtocolStack => new NWProtocolStack (nw_parameters_copy_default_protocol_stack (handle), owns: true);
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_only (nw_parameters_t parameters, bool local_only);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern bool nw_parameters_get_local_only (nw_parameters_t parameters);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public bool LocalOnly {
			get => nw_parameters_get_local_only (handle);
			set => nw_parameters_set_local_only (handle, value);
		}
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prefer_no_proxy (nw_parameters_t parameters, bool prefer_no_proxy);
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern bool nw_parameters_get_prefer_no_proxy (nw_parameters_t parameters);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		public bool PreferNoProxy {
			get => nw_parameters_get_prefer_no_proxy (handle);
			set => nw_parameters_set_prefer_no_proxy (handle, value);
		}
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		static extern void nw_parameters_set_expired_dns_behavior (nw_parameters_t parameters, NWParametersExpiredDnsBehavior expired_dns_behavior);
	
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWParametersExpiredDnsBehavior nw_parameters_get_expired_dns_behavior (nw_parameters_t parameters);

		public NWParametersExpiredDnsBehavior ExpiredDnsBehavior {
			get => nw_parameters_get_expired_dns_behavior (handle);
			set => nw_parameters_set_expired_dns_behavior (handle, value);
		}
			
	}

	public enum NWParametersExpiredDnsBehavior {
		Default = 0,
		Allow = 1,
		Prohibit = 2
	}
}
