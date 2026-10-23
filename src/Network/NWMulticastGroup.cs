using CoreFoundation;
using OS_nw_group_descriptor = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;

#nullable enable

namespace Network {
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWMulticastGroup : NativeObject {
		[Preserve (Conditional = true)]
		internal NWMulticastGroup (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_group_descriptor nw_group_descriptor_create_multicast (OS_nw_endpoint multicast_group);

		public NWMulticastGroup (NWEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));

			InitializeHandle (nw_group_descriptor_create_multicast (endpoint.GetCheckedHandle ()));
			GC.KeepAlive (endpoint);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_group_descriptor_add_endpoint (OS_nw_group_descriptor descriptor, OS_nw_endpoint endpoint);

		public void AddEndpoint (NWEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			nw_group_descriptor_add_endpoint (GetCheckedHandle (), endpoint.GetCheckedHandle ());
			GC.KeepAlive (endpoint);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_multicast_group_descriptor_get_disable_unicast_traffic (OS_nw_group_descriptor multicast_descriptor);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_multicast_group_descriptor_set_disable_unicast_traffic (OS_nw_group_descriptor multicast_descriptor, byte disable_unicast_traffic);

		public bool DisabledUnicastTraffic {
			get => nw_multicast_group_descriptor_get_disable_unicast_traffic (GetCheckedHandle ()) != 0;
			set => nw_multicast_group_descriptor_set_disable_unicast_traffic (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_multicast_group_descriptor_set_specific_source (OS_nw_group_descriptor multicast_descriptor, OS_nw_endpoint source);

		public void SetSpecificSource (NWEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			nw_multicast_group_descriptor_set_specific_source (GetCheckedHandle (), endpoint.GetCheckedHandle ());
			GC.KeepAlive (endpoint);
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_group_descriptor_enumerate_endpoints (OS_nw_group_descriptor descriptor, BlockLiteral* enumerate_block);

		[UnmanagedCallersOnly]
		static byte TrampolineEnumerateEndpointsHandler (IntPtr block, OS_nw_endpoint endpoint)
		{
			var del = BlockLiteral.GetTarget<Func<NWEndpoint, bool>> (block);
			if (del is not null) {
				using var nsEndpoint = new NWEndpoint (endpoint, owns: false);
				return del (nsEndpoint) ? (byte) 1 : (byte) 0;
			}
			return 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateEndpoints (Func<NWEndpoint, bool> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, OS_nw_endpoint, byte> trampoline = &TrampolineEnumerateEndpointsHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWMulticastGroup), nameof (TrampolineEnumerateEndpointsHandler));
				nw_group_descriptor_enumerate_endpoints (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}
	}
}
