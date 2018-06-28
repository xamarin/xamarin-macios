//
// NWPath.cs: Bindings the Netowrk nw_path_t API.
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

namespace Network {
	public enum NWPathStatus {
		Invalid = 0,
		Satisfied = 1,
		Unsatisfied = 2,
		Satisfiable = 3
	}
	
	public class NWPath : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWPath (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWPath (IntPtr handle) : this (handle, false)
		{
		}

		~NWPath ()
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
		extern static NWPathStatus nw_path_get_status (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWPathStatus Status => nw_path_get_status (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_is_expensive (IntPtr handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool IsExpensive => nw_path_is_expensive (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_has_ipv4 (IntPtr handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool HasIpV4 => nw_path_has_ipv4 (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_has_ipv6 (IntPtr handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool HasIpV6 => nw_path_has_ipv6 (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_has_dns (IntPtr handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool HasDns => nw_path_has_dns (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_uses_interface_type (IntPtr handle, NWInterfaceType type);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool UsesInterfaceType (NWInterfaceType type) => nw_path_uses_interface_type (handle, type);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_local_endpoint (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWEndpoint EffectiveLocalEndpoint {
			get {
				var x = nw_path_copy_effective_local_endpoint (handle);
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_remote_endpoint (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWEndpoint EffectiveRemoteEndpoint {
			get {
				var x = nw_path_copy_effective_remote_endpoint (handle);
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_is_equal (IntPtr p1, IntPtr p2);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public bool EqualsTo (NWPath other)
		{
			if (other == null)
				return false;

			return nw_path_is_equal (handle, other.handle);
		}

		delegate void nw_path_enumerate_interfaces_block_t (IntPtr block, IntPtr iface);
		static nw_path_enumerate_interfaces_block_t static_Enumerator = TrampolineEnumerator;
		
		[MonoPInvokeCallback (typeof (nw_path_enumerate_interfaces_block_t))]
		static unsafe void TrampolineEnumerator (IntPtr block, IntPtr iface)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<NWInterface>) (descriptor->Target);
			if (del != null){
				del (new NWInterface (iface, owns: false));
			}
		}
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_path_enumerate_interfaces (IntPtr handle, void *callback);
		
		public void EnumerateInterfaces (Action<NWInterface> callback)
		{
			if (callback == null)
				return;
			
			unsafe {
			        BlockLiteral *block_ptr_handler;
			        BlockLiteral block_handler;
			        block_handler = new BlockLiteral ();
			        block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (static_Enumerator, callback);
			
			        nw_path_enumerate_interfaces (handle, (void*) block_ptr_handler);
			        block_ptr_handler->CleanupBlock ();
			}
		}
				
	}
}
