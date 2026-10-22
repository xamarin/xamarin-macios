//
// NWParameter.cs: Bindings the Network nw_parameters_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using CoreFoundation;

using nw_parameters_t = System.IntPtr;
using OS_nw_parameters = System.IntPtr;
using nw_parameters_attribution_t = System.IntPtr;
using OS_nw_privacy_context = System.IntPtr;

namespace Network {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWParameters : NativeObject {
		[Preserve (Conditional = true)]
		internal NWParameters (NativeHandle handle, bool owns) : base (handle, owns) { }

#if !COREBUILD
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_parameters nw_parameters_create_application_service ();

		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public static NWParameters CreateApplicationService () => new NWParameters (nw_parameters_create_application_service (), true);

		static unsafe BlockLiteral* DEFAULT_CONFIGURATION () => (BlockLiteral*) NWParametersConstants._DefaultConfiguration;

		static unsafe BlockLiteral* DISABLE_PROTOCOL () => (BlockLiteral*) NWParametersConstants._ProtocolDisable;

		[UnmanagedCallersOnly]
		static void TrampolineConfigureHandler (IntPtr block, IntPtr iface)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolOptions>> (block);
			if (del is not null) {
				using (var tempOptions = new NWProtocolOptions (iface, owns: false))
				using (var definition = tempOptions.ProtocolDefinition) {
					NWProtocolOptions? castedOptions = null;

					if (definition.Equals (NWProtocolDefinition.CreateTcpDefinition ())) {
						castedOptions = new NWProtocolTcpOptions (iface, owns: false);
					} else if (definition.Equals (NWProtocolDefinition.CreateUdpDefinition ())) {
						castedOptions = new NWProtocolUdpOptions (iface, owns: false);
					} else if (definition.Equals (NWProtocolDefinition.CreateTlsDefinition ())) {
						castedOptions = new NWProtocolTlsOptions (iface, owns: false);
#pragma warning disable CA1416 // This call site is reachable on: 'ios' 12.2 and later, 'maccatalyst' 12.2 and later, 'macOS/OSX' 12.0 and later, 'tvos' 12.2 and later. 'NWProtocolIPOptions' is only supported on: 'ios' 13.0 and later, 'maccatalyst' 13.0 and later, 'tvos' 13.0 and later
					} else if (definition.Equals (NWProtocolDefinition.CreateIPDefinition ())) {
						castedOptions = new NWProtocolIPOptions (iface, owns: false);
#pragma warning restore CA1416
#pragma warning disable CA1416 // This call site is reachable on: 'ios' 12.2 and later, 'maccatalyst' 12.2 and later, 'macOS/OSX' 12.0 and later, 'tvos' 12.2 and later. 'NWWebSocketOptions' is only supported on: 'ios' 13.0 and later, 'maccatalyst' 13.0 and later, 'tvos' 13.0 and later.
					} else if (definition.Equals (NWProtocolDefinition.CreateWebSocketDefinition ())) {
						castedOptions = new NWWebSocketOptions (iface, owns: false);
#pragma warning restore CA1416
					}

					try {
						del (castedOptions ?? tempOptions);
					} finally {
						castedOptions?.Dispose ();
					}
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern nw_parameters_t nw_parameters_create_secure_tcp (BlockLiteral* configure_tls, BlockLiteral* configure_tcp);

		[BindingImpl (BindingImplOptions.Optimizable)]
		unsafe static BlockLiteral* CreateBlock (Action<NWProtocolOptions>? callback, BlockLiteral* callbackBlock, out bool disposeReturnValue)
		{
			if (callback is null) {
				disposeReturnValue = false;
				return DEFAULT_CONFIGURATION ();
			}

			delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineConfigureHandler;
			var block = new BlockLiteral (trampoline, callback, typeof (NWParameters), nameof (TrampolineConfigureHandler));
			*callbackBlock = block;
			disposeReturnValue = true;
			return callbackBlock;
		}

		//
		// If you pass null, to either configureTls, or configureTcp they will use the default options
		//
		/// <param name="configureTls">To be added.</param>
		///         <param name="configureTcp">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateSecureTcp (Action<NWProtocolOptions>? configureTls = null, Action<NWProtocolOptions>? configureTcp = null)
		{
			var tlsHandler = default (BlockLiteral);
			var tcpHandler = default (BlockLiteral); ;

			var tlsPtr = CreateBlock (configureTls, &tlsHandler, out var disposeTlsPtr);
			var tcpPtr = CreateBlock (configureTcp, &tcpHandler, out var disposeTcpPtr);

			var ptr = nw_parameters_create_secure_tcp (tlsPtr, tcpPtr);

			if (disposeTlsPtr)
				tlsPtr->Dispose ();

			if (disposeTcpPtr)
				tcpPtr->Dispose ();

			return new NWParameters (ptr, owns: true);
		}

		/// <param name="configureTcp">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		// If you pass null to configureTcp, it will use the default options
		public unsafe static NWParameters CreateTcp (Action<NWProtocolOptions>? configureTcp = null)
		{
			var tcpHandler = default (BlockLiteral);
			var tcpPtr = CreateBlock (configureTcp, &tcpHandler, out var disposeTcpPtr);

			var ptr = nw_parameters_create_secure_tcp (DISABLE_PROTOCOL (), tcpPtr);

			if (disposeTcpPtr)
				tcpPtr->Dispose ();

			return new NWParameters (ptr, owns: true);
		}


		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe nw_parameters_t nw_parameters_create_secure_udp (void* configure_tls, void* configure_tcp);

		//
		// If you pass null, to either configureTls, or configureTcp they will use the default options
		//
		/// <param name="configureTls">To be added.</param>
		///         <param name="configureUdp">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateSecureUdp (Action<NWProtocolOptions>? configureTls = null, Action<NWProtocolOptions>? configureUdp = null)
		{
			var tlsHandler = default (BlockLiteral);
			var udpHandler = default (BlockLiteral);

			var tlsPtr = CreateBlock (configureTls, &tlsHandler, out var disposeTlsPtr);
			var udpPtr = CreateBlock (configureUdp, &udpHandler, out var disposeUdpPtr);

			var ptr = nw_parameters_create_secure_udp (tlsPtr, udpPtr);

			if (disposeTlsPtr)
				tlsPtr->Dispose ();

			if (disposeUdpPtr)
				udpPtr->Dispose ();

			return new NWParameters (ptr, owns: true);
		}

		// If you pass null to configureTcp, it will use the default options
		/// <param name="configureUdp">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateUdp (Action<NWProtocolOptions>? configureUdp = null)
		{
			var udpHandler = default (BlockLiteral);
			var udpPtr = CreateBlock (configureUdp, &udpHandler, out var disposeUdpPtr);

			var ptr = nw_parameters_create_secure_udp (DISABLE_PROTOCOL (), udpPtr);

			if (disposeUdpPtr)
				udpPtr->Dispose ();

			return new NWParameters (ptr, owns: true);
		}

#if MONOMAC
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern IntPtr nw_parameters_create_custom_ip (byte custom_ip_protocol_number, BlockLiteral* configure_ip);

		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateCustomIP (byte protocolNumber, Action<NWProtocolOptions>? configureCustomIP = null)
		{
			var ipHandler = default (BlockLiteral);
			var ipPtr = CreateBlock (configureCustomIP, &ipHandler, out var disposeIpPtr);

			var ptr = nw_parameters_create_custom_ip (protocolNumber, ipPtr);

			if (disposeIpPtr)
				ipPtr->Dispose ();

			return new NWParameters (ptr, owns: true);
		}
#endif

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_create ();

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public NWParameters ()
		{
			InitializeHandle (nw_parameters_create ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_copy (nw_parameters_t handle);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public NWParameters Clone ()
		{
			return new NWParameters (nw_parameters_copy (GetCheckedHandle ()), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_multipath_service (nw_parameters_t parameters, NWMultiPathService multipath_service);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWMultiPathService nw_parameters_get_multipath_service (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWMultiPathService MultipathService {
			get => nw_parameters_get_multipath_service (GetCheckedHandle ());
			set => nw_parameters_set_multipath_service (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_default_protocol_stack (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWProtocolStack ProtocolStack => new NWProtocolStack (nw_parameters_copy_default_protocol_stack (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_only (nw_parameters_t parameters, byte local_only);

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_local_only (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool LocalOnly {
			get => nw_parameters_get_local_only (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_local_only (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prefer_no_proxy (nw_parameters_t parameters, byte prefer_no_proxy);

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_prefer_no_proxy (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool PreferNoProxy {
			get => nw_parameters_get_prefer_no_proxy (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_prefer_no_proxy (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_expired_dns_behavior (nw_parameters_t parameters, NWParametersExpiredDnsBehavior expired_dns_behavior);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWParametersExpiredDnsBehavior nw_parameters_get_expired_dns_behavior (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWParametersExpiredDnsBehavior ExpiredDnsBehavior {
			get => nw_parameters_get_expired_dns_behavior (GetCheckedHandle ());
			set => nw_parameters_set_expired_dns_behavior (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_require_interface (nw_parameters_t parameters, IntPtr handleInterface);

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_required_interface (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWInterface? RequiredInterface {
			get {
				var iface = nw_parameters_copy_required_interface (GetCheckedHandle ());

				if (iface == IntPtr.Zero)
					return null;

				return new NWInterface (iface, owns: true);
			}
			set {
				nw_parameters_require_interface (GetCheckedHandle (), value.GetHandle ());
				GC.KeepAlive (value);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_prohibit_interface (nw_parameters_t parameters, IntPtr handleInterface);

		/// <param name="iface">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void ProhibitInterface (NWInterface iface)
		{
			if (iface is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (iface));

			nw_parameters_prohibit_interface (GetCheckedHandle (), iface.Handle);
			GC.KeepAlive (iface);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_clear_prohibited_interfaces (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void ClearProhibitedInterfaces ()
		{
			nw_parameters_clear_prohibited_interfaces (GetCheckedHandle ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_required_interface_type (nw_parameters_t parameters, NWInterfaceType ifaceType);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWInterfaceType nw_parameters_get_required_interface_type (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWInterfaceType RequiredInterfaceType {
			get => nw_parameters_get_required_interface_type (GetCheckedHandle ());
			set => nw_parameters_set_required_interface_type (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_prohibit_interface_type (nw_parameters_t parameters, NWInterfaceType type);

		/// <param name="ifaceType">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void ProhibitInterfaceType (NWInterfaceType ifaceType)
		{
			nw_parameters_prohibit_interface_type (GetCheckedHandle (), ifaceType);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_clear_prohibited_interface_types (nw_parameters_t parameters);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void ClearProhibitedInterfaceTypes ()
		{
			nw_parameters_clear_prohibited_interface_types (GetCheckedHandle ());
		}

		[UnmanagedCallersOnly]
		static byte TrampolineIterateProhibitedHandler (IntPtr block, IntPtr iface)
		{
			var del = BlockLiteral.GetTarget<Func<NWInterface, bool>> (block);
			if (del is not null) {
				var x = new NWInterface (iface, owns: false);
				var ret = del (x);
				x.Dispose ();
				return ret ? (byte) 1 : (byte) 0;
			}
			return 0;
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_parameters_iterate_prohibited_interfaces (nw_parameters_t parameters, BlockLiteral* callback);

		/// <param name="iterationCallback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProhibitedInterfaces (Func<NWInterface, bool> iterationCallback)
		{
			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineIterateProhibitedHandler;
				using var block = new BlockLiteral (trampoline, iterationCallback, typeof (NWParameters), nameof (TrampolineIterateProhibitedHandler));
				nw_parameters_iterate_prohibited_interfaces (GetCheckedHandle (), &block);
			}
		}

		[UnmanagedCallersOnly]
		static byte TrampolineIterateProhibitedTypeHandler (IntPtr block, NWInterfaceType type)
		{
			var del = BlockLiteral.GetTarget<Func<NWInterfaceType, bool>> (block);
			if (del is not null)
				return del (type) ? (byte) 1 : (byte) 0;
			return 0;
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_parameters_iterate_prohibited_interface_types (IntPtr handle, BlockLiteral* callback);

		/// <param name="callback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProhibitedInterfaces (Func<NWInterfaceType, bool> callback)
		{
			unsafe {
				delegate* unmanaged<IntPtr, NWInterfaceType, byte> trampoline = &TrampolineIterateProhibitedTypeHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWParameters), nameof (TrampolineIterateProhibitedTypeHandler));
				nw_parameters_iterate_prohibited_interface_types (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_prohibit_expensive (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prohibit_expensive (IntPtr handle, byte prohibit_expensive);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ProhibitExpensive {
			get => nw_parameters_get_prohibit_expensive (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_prohibit_expensive (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_reuse_local_address (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_reuse_local_address (IntPtr handle, byte reuse_local_address);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReuseLocalAddress {
			get => nw_parameters_get_reuse_local_address (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_reuse_local_address (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_fast_open_enabled (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_fast_open_enabled (IntPtr handle, byte fast_open_enabled);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool FastOpenEnabled {
			get => nw_parameters_get_fast_open_enabled (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_fast_open_enabled (GetCheckedHandle (), value.AsByte ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWServiceClass nw_parameters_get_service_class (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_service_class (IntPtr handle, NWServiceClass service_class);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWServiceClass ServiceClass {
			get => nw_parameters_get_service_class (GetCheckedHandle ());
			set => nw_parameters_set_service_class (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_local_endpoint (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_endpoint (IntPtr handle, IntPtr endpoint);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NWEndpoint? LocalEndpoint {
			get {
				var x = nw_parameters_copy_local_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;

				return new NWEndpoint (x, owns: true);
			}

			set {
				nw_parameters_set_local_endpoint (GetCheckedHandle (), value.GetHandle ());
				GC.KeepAlive (value);
			}
		}


		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_include_peer_to_peer (IntPtr handle, byte includePeerToPeer);

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_include_peer_to_peer (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IncludePeerToPeer {
			get => nw_parameters_get_include_peer_to_peer (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_include_peer_to_peer (GetCheckedHandle (), value.AsByte ());
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_prohibit_constrained (IntPtr parameters);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prohibit_constrained (IntPtr parameters, byte prohibit_constrained);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool ProhibitConstrained {
			get => nw_parameters_get_prohibit_constrained (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_prohibit_constrained (GetCheckedHandle (), value.AsByte ());
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_attribution (OS_nw_parameters parameters, byte attribution);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_attribution (OS_nw_parameters parameters);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public NWParametersAttribution Attribution {
			get => (NWParametersAttribution) nw_parameters_get_attribution (GetCheckedHandle ());
			set => nw_parameters_set_attribution (GetCheckedHandle (), (byte) value);
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_privacy_context (OS_nw_parameters parameters, OS_nw_privacy_context privacy_context);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public void SetPrivacyContext (NWPrivacyContext privacyContext)
		{
			nw_parameters_set_privacy_context (GetCheckedHandle (), privacyContext.Handle);
			GC.KeepAlive (privacyContext);
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern OS_nw_parameters nw_parameters_create_quic (void* configureQuic);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateQuic (Action<NWProtocolOptions>? configureQuic = null)
		{
			var quicHandlers = default (BlockLiteral);
			var quicPtr = CreateBlock (configureQuic, &quicHandlers, out var disposeQuicPtr);

			var ptr = nw_parameters_create_quic (quicPtr);

			if (disposeQuicPtr)
				quicPtr->Dispose ();

			return new NWParameters (ptr, owns: true);
		}

		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_requires_dnssec_validation (OS_nw_parameters parameters);

		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_requires_dnssec_validation (OS_nw_parameters parameters, byte requires_dnssec_validation);

		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool RequiresDnssecValidation {
			get => nw_parameters_requires_dnssec_validation (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_requires_dnssec_validation (GetCheckedHandle (), value.AsByte ());
		}

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_parameters_get_allow_ultra_constrained (IntPtr parameters);

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_allow_ultra_constrained (IntPtr parameters, byte prohibit_constrained);

		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		public bool AllowUltraConstrained {
			get => nw_parameters_get_allow_ultra_constrained (GetCheckedHandle ()) != 0;
			set => nw_parameters_set_allow_ultra_constrained (GetCheckedHandle (), value.AsByte ());
		}

#endif // COREBUILD
	}
}
