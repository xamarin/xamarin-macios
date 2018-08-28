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

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public enum NWPathStatus : long {
		Invalid = 0,
		Satisfied = 1,
		Unsatisfied = 2,
		Satisfiable = 3,
	}

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class NWPath : NativeObject {
		public NWPath (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		extern static NWPathStatus nw_path_get_status (IntPtr handle);

		public NWPathStatus Status => nw_path_get_status (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_is_expensive (IntPtr handle);

		public bool IsExpensive => nw_path_is_expensive (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_has_ipv4 (IntPtr handle);

		public bool HasIPV4 => nw_path_has_ipv4 (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_has_ipv6 (IntPtr handle);

		public bool HasIPV6 => nw_path_has_ipv6 (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_has_dns (IntPtr handle);

		public bool HasDns => nw_path_has_dns (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_uses_interface_type (IntPtr handle, NWInterfaceType type);

		public bool UsesInterfaceType (NWInterfaceType type) => nw_path_uses_interface_type (GetCheckedHandle (), type);

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_local_endpoint (IntPtr handle);

		public NWEndpoint EffectiveLocalEndpoint {
			get {
				var x = nw_path_copy_effective_local_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_remote_endpoint (IntPtr handle);

		public NWEndpoint EffectiveRemoteEndpoint {
			get {
				var x = nw_path_copy_effective_remote_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs(UnmanagedType.U1)]
		extern static bool nw_path_is_equal (IntPtr p1, IntPtr p2);

		public bool EqualsTo (NWPath other)
		{
			if (other == null)
				return false;

			return nw_path_is_equal (GetCheckedHandle (), other.Handle);
		}

		delegate void nw_path_enumerate_interfaces_block_t (IntPtr block, IntPtr iface);
		static nw_path_enumerate_interfaces_block_t static_Enumerator = TrampolineEnumerator;

		[MonoPInvokeCallback (typeof (nw_path_enumerate_interfaces_block_t))]
		static void TrampolineEnumerator (IntPtr block, IntPtr iface)
		{
			var del = BlockLiteral.GetTarget<Action<NWInterface>> (block);
			if (del != null)
				del (new NWInterface (iface, owns: false));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_path_enumerate_interfaces (IntPtr handle, ref BlockLiteral callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateInterfaces (Action<NWInterface> callback)
		{
			if (callback == null)
				return;

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_Enumerator, callback);

			try {
				nw_path_enumerate_interfaces (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}
	}
}
