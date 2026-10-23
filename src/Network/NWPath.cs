//
// NWPath.cs: Bindings the Netowrk nw_path_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoreFoundation;

namespace Network {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWPath : NativeObject {
		[Preserve (Conditional = true)]
		internal NWPath (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		extern static NWPathStatus nw_path_get_status (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWPathStatus Status => nw_path_get_status (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_path_is_expensive (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsExpensive => nw_path_is_expensive (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_path_has_ipv4 (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool HasIPV4 => nw_path_has_ipv4 (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_path_has_ipv6 (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool HasIPV6 => nw_path_has_ipv6 (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_path_has_dns (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool HasDns => nw_path_has_dns (GetCheckedHandle ()) != 0;

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_path_uses_interface_type (IntPtr handle, NWInterfaceType type);

		/// <param name="type">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool UsesInterfaceType (NWInterfaceType type) => nw_path_uses_interface_type (GetCheckedHandle (), type) != 0;

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_path_copy_effective_local_endpoint (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWEndpoint? EffectiveRemoteEndpoint {
			get {
				var x = nw_path_copy_effective_remote_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;
				return new NWEndpoint (x, owns: true);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static byte nw_path_is_equal (IntPtr p1, IntPtr p2);

		/// <param name="other">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool EqualsTo (NWPath other)
		{
			if (other is null)
				return false;

			bool result = nw_path_is_equal (GetCheckedHandle (), other.Handle) != 0;
			GC.KeepAlive (other);
			return result;
		}

		// Returning 'byte' since 'bool' isn't blittable
		[UnmanagedCallersOnly]
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
		/// <param name="callback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineEnumerator;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWPath), nameof (TrampolineEnumerator));
				nw_path_enumerate_interfaces (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_path_is_constrained (IntPtr path);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool IsConstrained => nw_path_is_constrained (GetCheckedHandle ()) != 0;

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_path_enumerate_gateways (IntPtr path, BlockLiteral* enumerate_block);

		// Returning 'byte' since 'bool' isn't blittable
		[UnmanagedCallersOnly]
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
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
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

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateGateways (Func<NWEndpoint, bool> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineGatewaysHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWPath), nameof (TrampolineGatewaysHandler));
				nw_path_enumerate_gateways (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWPathUnsatisfiedReason /* nw_path_unsatisfied_reason_t */ nw_path_get_unsatisfied_reason (IntPtr /* OS_nw_path */ path);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public NWPathUnsatisfiedReason GetUnsatisfiedReason ()
		{
			return nw_path_get_unsatisfied_reason (GetCheckedHandle ());
		}

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_path_is_ultra_constrained (IntPtr path);

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		public bool IsUltraConstrained => nw_path_is_ultra_constrained (GetCheckedHandle ()) != 0;

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_path_get_link_quality (IntPtr path);

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		public NWLinkQuality LinkQuality => (NWLinkQuality) nw_path_get_link_quality (GetCheckedHandle ());

	}
}
