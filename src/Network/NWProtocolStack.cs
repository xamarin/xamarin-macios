//
// NWProtocolStack.cs: Bindings the Netowrk nw_protocol_stack_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using CoreFoundation;

using OS_nw_protocol_definition = System.IntPtr;
using OS_nw_protocol_metadata = System.IntPtr;
using nw_service_class_t = System.IntPtr;
using nw_protocol_stack_t = System.IntPtr;
using nw_protocol_options_t = System.IntPtr;

namespace Network {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWProtocolStack : NativeObject {
		[Preserve (Conditional = true)]
		internal NWProtocolStack (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_prepend_application_protocol (nw_protocol_stack_t stack, nw_protocol_options_t options);

		/// <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void PrependApplicationProtocol (NWProtocolOptions options)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));
			nw_protocol_stack_prepend_application_protocol (GetCheckedHandle (), options.Handle);
			GC.KeepAlive (options);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_protocol_stack_clear_application_protocols (nw_protocol_stack_t stack);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void ClearApplicationProtocols ()
		{
			nw_protocol_stack_clear_application_protocols (GetCheckedHandle ());
		}

		[UnmanagedCallersOnly]
		static void TrampolineIterateHandler (IntPtr block, IntPtr options)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolOptions>> (block);
			if (del is not null) {
				using (var tempOptions = new NWProtocolOptions (options, owns: false))
				using (var definition = tempOptions.ProtocolDefinition) {
					NWProtocolOptions? castedOptions = null;

					if (definition.Equals (NWProtocolDefinition.CreateTcpDefinition ())) {
						castedOptions = new NWProtocolTcpOptions (options, owns: false);
					} else if (definition.Equals (NWProtocolDefinition.CreateUdpDefinition ())) {
						castedOptions = new NWProtocolUdpOptions (options, owns: false);
					} else if (definition.Equals (NWProtocolDefinition.CreateTlsDefinition ())) {
						castedOptions = new NWProtocolTlsOptions (options, owns: false);
#pragma warning disable CA1416 // This call site is reachable on: 'ios' 12.2 and later, 'maccatalyst' 12.2 and later, 'macOS/OSX' 12.0 and later, 'tvos' 12.2 and later. 'NWProtocolIPOptions' is only supported on: 'ios' 13.0 and later, 'maccatalyst' 13.0 and later, 'tvos' 13.0 and later.
					} else if (definition.Equals (NWProtocolDefinition.CreateIPDefinition ())) {
						castedOptions = new NWProtocolIPOptions (options, owns: false);
#pragma warning restore CA1416
#pragma warning disable CA1416 // This call site is reachable on: 'ios' 12.2 and later, 'maccatalyst' 12.2 and later, 'macOS/OSX' 12.0 and later, 'tvos' 12.2 and later. 'NWWebSocketOptions' is only supported on: 'ios' 13.0 and later, 'maccatalyst' 13.0 and later, 'tvos' 13.0 and later.
					} else if (definition.Equals (NWProtocolDefinition.CreateWebSocketDefinition ())) {
						castedOptions = new NWWebSocketOptions (options, owns: false);
#pragma warning restore CA1416
					}

					del (castedOptions ?? tempOptions);
					castedOptions?.Dispose ();
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe extern static void nw_protocol_stack_iterate_application_protocols (nw_protocol_stack_t stack, BlockLiteral* completion);

		/// <param name="callback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProtocols (Action<NWProtocolOptions> callback)
		{
			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineIterateHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWProtocolStack), nameof (TrampolineIterateHandler));
				nw_protocol_stack_iterate_application_protocols (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_transport_protocol (nw_protocol_stack_t stack);

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_protocol_stack_set_transport_protocol (nw_protocol_stack_t stack, IntPtr value);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWProtocolOptions? TransportProtocol {
			get {
				var pHandle = nw_protocol_stack_copy_transport_protocol (GetCheckedHandle ());
				if (pHandle == IntPtr.Zero)
					return null;
				var tempOptions = new NWProtocolOptions (pHandle, owns: true);

				using (var definition = tempOptions.ProtocolDefinition) {
					NWProtocolOptions? castedOptions = null;
					if (definition.Equals (NWProtocolDefinition.CreateTcpDefinition ())) {
						castedOptions = new NWProtocolTcpOptions (pHandle, owns: true);
					}
					if (definition.Equals (NWProtocolDefinition.CreateUdpDefinition ())) {
						castedOptions = new NWProtocolUdpOptions (pHandle, owns: true);
					}
					if (castedOptions is null) {
						return tempOptions;
					} else {
						tempOptions.Dispose ();
						return castedOptions;
					}
				}
			}
			set {
				nw_protocol_stack_set_transport_protocol (GetCheckedHandle (), value.GetHandle ());
				GC.KeepAlive (value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_protocol_stack_copy_internet_protocol (nw_protocol_stack_t stack);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public NWProtocolIPOptions? InternetProtocol {
			get {
				var pHandle = nw_protocol_stack_copy_internet_protocol (GetCheckedHandle ());
				return (pHandle == IntPtr.Zero) ? null : new NWProtocolIPOptions (pHandle, owns: true);
			}
		}
	}
}
