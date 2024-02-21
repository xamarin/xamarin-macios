//
// NWPath.cs: Bindings the Netowrk nw_path_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (6, 0)]
#endif
	public class NWPath : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWPath (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWPath (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.NetworkLibrary)]
		extern static NWPathStatus nw_path_get_status (IntPtr handle);

		public NWPathStatus Status => nw_path_get_status (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool nw_path_is_expensive (IntPtr handle);

		public bool IsExpensive => nw_path_is_expensive (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool nw_path_has_ipv4 (IntPtr handle);

		public bool HasIPV4 => nw_path_has_ipv4 (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool nw_path_has_ipv6 (IntPtr handle);

		public bool HasIPV6 => nw_path_has_ipv6 (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool nw_path_has_dns (IntPtr handle);

		public bool HasDns => nw_path_has_dns (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool nw_path_uses_interface_type (IntPtr handle, NWInterfaceType type);

		public bool UsesInterfaceType (NWInterfaceType type) => nw_path_uses_interface_type (GetCheckedHandle (), type);

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_local_endpoint (IntPtr handle);

		public NWEndpoint? EffectiveLocalEndpoint {
			get {
				var x = nw_path_copy_effective_local_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_remote_endpoint (IntPtr handle);

		public NWEndpoint? EffectiveRemoteEndpoint {
			get {
				var x = nw_path_copy_effective_remote_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool nw_path_is_equal (IntPtr p1, IntPtr p2);

		public bool EqualsTo (NWPath other)
		{
			if (other is null)
				return false;

			return nw_path_is_equal (GetCheckedHandle (), other.Handle);
		}

		// Returning 'byte' since 'bool' isn't blittable
#if !NET
		delegate byte nw_path_enumerate_interfaces_block_t (IntPtr block, IntPtr iface);
		static nw_path_enumerate_interfaces_block_t static_Enumerator = TrampolineEnumerator;

		[MonoPInvokeCallback (typeof (nw_path_enumerate_interfaces_block_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static byte TrampolineEnumerator (IntPtr block, IntPtr iface)
		{
			var del = BlockLiteral.GetTarget<Func<NWInterface, bool>> (block);
			if (del is not null)
				return del (new NWInterface (iface, owns: false)) ? (byte) 1 : (byte) 0;
			return 0;
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_path_enumerate_interfaces (IntPtr handle, BlockLiteral* callback);


#if !XAMCORE_5_0
		[Obsolete ("Use the overload that takes a 'Func<NWInterface, bool>' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public void EnumerateInterfaces (Action<NWInterface> callback)
		{
			if (callback is null)
				return;

			Func<NWInterface, bool> func = (v) => {
				callback (v);
				return true;
			};
			EnumerateInterfaces (func);
		}
#endif // !XAMCORE_5_0

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateInterfaces (Func<NWInterface, bool> callback)
		{
			if (callback is null)
				return;

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineEnumerator;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWPath), nameof (TrampolineEnumerator));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_Enumerator, callback);
#endif
				nw_path_enumerate_interfaces (GetCheckedHandle (), &block);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_path_is_constrained (IntPtr path);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public bool IsConstrained => nw_path_is_constrained (GetCheckedHandle ());

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_path_enumerate_gateways (IntPtr path, BlockLiteral* enumerate_block);

		// Returning 'byte' since 'bool' isn't blittable
#if !NET
		delegate byte nw_path_enumerate_gateways_t (IntPtr block, IntPtr endpoint);
		static nw_path_enumerate_gateways_t static_EnumerateGatewaysHandler = TrampolineGatewaysHandler;

		[MonoPInvokeCallback (typeof (nw_path_enumerate_gateways_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static byte TrampolineGatewaysHandler (IntPtr block, IntPtr endpoint)
		{
			var del = BlockLiteral.GetTarget<Func<NWEndpoint, bool>> (block);
			if (del is not null) {
				var nwEndpoint = new NWEndpoint (endpoint, owns: false);
				return del (nwEndpoint) ? (byte) 1 : (byte) 0;
			}
			return 0;
		}

#if !XAMCORE_5_0
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[Obsolete ("Use the overload that takes a 'Func<NWEndpoint, bool>' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public void EnumerateGateways (Action<NWEndpoint> callback)
		{
			Func<NWEndpoint, bool> func = (v) => {
				callback (v);
				return true;
			};
			EnumerateGateways (func);
		}
#endif

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateGateways (Func<NWEndpoint, bool> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineGatewaysHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWPath), nameof (TrampolineGatewaysHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_EnumerateGatewaysHandler, callback);
#endif
				nw_path_enumerate_gateways (GetCheckedHandle (), &block);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.2")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.2")]
#else
		[iOS (14, 2)]
		[TV (14, 2)]
		[Watch (7, 1)]
		[Mac (11, 0)]
		[MacCatalyst (14, 2)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern NWPathUnsatisfiedReason /* nw_path_unsatisfied_reason_t */ nw_path_get_unsatisfied_reason (IntPtr /* OS_nw_path */ path);

#if NET
		[SupportedOSPlatform ("ios14.2")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.2")]
#else
		[iOS (14, 2)]
		[TV (14, 2)]
		[Watch (7, 1)]
		[Mac (11, 0)]
		[MacCatalyst (14, 2)]
#endif
		public NWPathUnsatisfiedReason GetUnsatisfiedReason ()
		{
			return nw_path_get_unsatisfied_reason (GetCheckedHandle ());
		}
	}
}
