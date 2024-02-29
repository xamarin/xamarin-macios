//
// NWParameter.cs: Bindings the Network nw_parameters_t API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using CoreFoundation;

using nw_parameters_t = System.IntPtr;
using OS_nw_parameters = System.IntPtr;
using nw_parameters_attribution_t = System.IntPtr;
using OS_nw_privacy_context = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Watch (6, 0)]
#endif
	public class NWParameters : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWParameters (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWParameters (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_parameters nw_parameters_create_application_service ();

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		public static NWParameters CreateApplicationService () => new NWParameters (nw_parameters_create_application_service (), true);

		static unsafe BlockLiteral* DEFAULT_CONFIGURATION () => (BlockLiteral*) NWParametersConstants._DefaultConfiguration;

		static unsafe BlockLiteral* DISABLE_PROTOCOL () => (BlockLiteral*) NWParametersConstants._ProtocolDisable;

#if !NET
		delegate void nw_parameters_configure_protocol_block_t (IntPtr block, IntPtr iface);
		static nw_parameters_configure_protocol_block_t static_ConfigureHandler = TrampolineConfigureHandler;

		[MonoPInvokeCallback (typeof (nw_parameters_configure_protocol_block_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
					} else if (definition.Equals (NWProtocolDefinition.CreateIPDefinition ())) {
						castedOptions = new NWProtocolIPOptions (iface, owns: false);
					} else if (definition.Equals (NWProtocolDefinition.CreateWebSocketDefinition ())) {
						castedOptions = new NWWebSocketOptions (iface, owns: false);
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

#if NET
			delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineConfigureHandler;
			var block = new BlockLiteral (trampoline, callback, typeof (NWParameters), nameof (TrampolineConfigureHandler));
#else
			var block = new BlockLiteral ();
			block.SetupBlockUnsafe (static_ConfigureHandler, callback);
#endif
			*callbackBlock = block;
			disposeReturnValue = true;
			return callbackBlock;
		}

		//
		// If you pass null, to either configureTls, or configureTcp they will use the default options
		//
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
#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern IntPtr nw_parameters_create_custom_ip (byte custom_ip_protocol_number, BlockLiteral *configure_ip);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
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

		public NWParameters ()
		{
			InitializeHandle (nw_parameters_create ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern nw_parameters_t nw_parameters_copy (nw_parameters_t handle);

		public NWParameters Clone ()
		{
			return new NWParameters (nw_parameters_copy (GetCheckedHandle ()), owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_multipath_service (nw_parameters_t parameters, NWMultiPathService multipath_service);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWMultiPathService nw_parameters_get_multipath_service (nw_parameters_t parameters);

		public NWMultiPathService MultipathService {
			get => nw_parameters_get_multipath_service (GetCheckedHandle ());
			set => nw_parameters_set_multipath_service (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_default_protocol_stack (nw_parameters_t parameters);

		public NWProtocolStack ProtocolStack => new NWProtocolStack (nw_parameters_copy_default_protocol_stack (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_only (nw_parameters_t parameters, [MarshalAs (UnmanagedType.I1)] bool local_only);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_local_only (nw_parameters_t parameters);

		public bool LocalOnly {
			get => nw_parameters_get_local_only (GetCheckedHandle ());
			set => nw_parameters_set_local_only (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prefer_no_proxy (nw_parameters_t parameters, [MarshalAs (UnmanagedType.I1)] bool prefer_no_proxy);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_prefer_no_proxy (nw_parameters_t parameters);

		public bool PreferNoProxy {
			get => nw_parameters_get_prefer_no_proxy (GetCheckedHandle ());
			set => nw_parameters_set_prefer_no_proxy (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_expired_dns_behavior (nw_parameters_t parameters, NWParametersExpiredDnsBehavior expired_dns_behavior);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWParametersExpiredDnsBehavior nw_parameters_get_expired_dns_behavior (nw_parameters_t parameters);

		public NWParametersExpiredDnsBehavior ExpiredDnsBehavior {
			get => nw_parameters_get_expired_dns_behavior (GetCheckedHandle ());
			set => nw_parameters_set_expired_dns_behavior (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_require_interface (nw_parameters_t parameters, IntPtr handleInterface);

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_required_interface (nw_parameters_t parameters);

		public NWInterface? RequiredInterface {
			get {
				var iface = nw_parameters_copy_required_interface (GetCheckedHandle ());

				if (iface == IntPtr.Zero)
					return null;

				return new NWInterface (iface, owns: true);
			}
			set {
				nw_parameters_require_interface (GetCheckedHandle (), value.GetHandle ());
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_prohibit_interface (nw_parameters_t parameters, IntPtr handleInterface);

		public void ProhibitInterface (NWInterface iface)
		{
			if (iface is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (iface));

			nw_parameters_prohibit_interface (GetCheckedHandle (), iface.Handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_clear_prohibited_interfaces (nw_parameters_t parameters);

		public void ClearProhibitedInterfaces ()
		{
			nw_parameters_clear_prohibited_interfaces (GetCheckedHandle ());
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_required_interface_type (nw_parameters_t parameters, NWInterfaceType ifaceType);

		[DllImport (Constants.NetworkLibrary)]
		static extern NWInterfaceType nw_parameters_get_required_interface_type (nw_parameters_t parameters);

		public NWInterfaceType RequiredInterfaceType {
			get => nw_parameters_get_required_interface_type (GetCheckedHandle ());
			set => nw_parameters_set_required_interface_type (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_prohibit_interface_type (nw_parameters_t parameters, NWInterfaceType type);

		public void ProhibitInterfaceType (NWInterfaceType ifaceType)
		{
			nw_parameters_prohibit_interface_type (GetCheckedHandle (), ifaceType);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_clear_prohibited_interface_types (nw_parameters_t parameters);

		public void ClearProhibitedInterfaceTypes ()
		{
			nw_parameters_clear_prohibited_interface_types (GetCheckedHandle ());
		}

#if !NET
		delegate byte nw_parameters_iterate_interfaces_block_t (IntPtr block, IntPtr iface);
		static nw_parameters_iterate_interfaces_block_t static_iterateProhibitedHandler = TrampolineIterateProhibitedHandler;

		[MonoPInvokeCallback (typeof (nw_parameters_iterate_interfaces_block_t))]
#else
		[UnmanagedCallersOnly]
#endif
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

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProhibitedInterfaces (Func<NWInterface, bool> iterationCallback)
		{
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, byte> trampoline = &TrampolineIterateProhibitedHandler;
				using var block = new BlockLiteral (trampoline, iterationCallback, typeof (NWParameters), nameof (TrampolineIterateProhibitedHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_iterateProhibitedHandler, iterationCallback);
#endif
				nw_parameters_iterate_prohibited_interfaces (GetCheckedHandle (), &block);
			}
		}

#if !NET
		delegate byte nw_parameters_iterate_interface_types_block_t (IntPtr block, NWInterfaceType type);
		static nw_parameters_iterate_interface_types_block_t static_IterateProhibitedTypeHandler = TrampolineIterateProhibitedTypeHandler;

		[MonoPInvokeCallback (typeof (nw_parameters_iterate_interface_types_block_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static byte TrampolineIterateProhibitedTypeHandler (IntPtr block, NWInterfaceType type)
		{
			var del = BlockLiteral.GetTarget<Func<NWInterfaceType, bool>> (block);
			if (del is not null)
				return del (type) ? (byte) 1 : (byte) 0;
			return 0;
		}

		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_parameters_iterate_prohibited_interface_types (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProhibitedInterfaces (Func<NWInterfaceType, bool> callback)
		{
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, NWInterfaceType, byte> trampoline = &TrampolineIterateProhibitedTypeHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWParameters), nameof (TrampolineIterateProhibitedTypeHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_IterateProhibitedTypeHandler, callback);
#endif
				nw_parameters_iterate_prohibited_interface_types (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_prohibit_expensive (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_prohibit_expensive (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool prohibit_expensive);

		public bool ProhibitExpensive {
			get => nw_parameters_get_prohibit_expensive (GetCheckedHandle ());
			set => nw_parameters_set_prohibit_expensive (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_reuse_local_address (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_reuse_local_address (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool reuse_local_address);

		public bool ReuseLocalAddress {
			get => nw_parameters_get_reuse_local_address (GetCheckedHandle ());
			set => nw_parameters_set_reuse_local_address (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_fast_open_enabled (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_fast_open_enabled (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool fast_open_enabled);

		public bool FastOpenEnabled {
			get => nw_parameters_get_fast_open_enabled (GetCheckedHandle ());
			set => nw_parameters_set_fast_open_enabled (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWServiceClass nw_parameters_get_service_class (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_service_class (IntPtr handle, NWServiceClass service_class);

		public NWServiceClass ServiceClass {
			get => nw_parameters_get_service_class (GetCheckedHandle ());
			set => nw_parameters_set_service_class (GetCheckedHandle (), value);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_parameters_copy_local_endpoint (IntPtr handle);

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_local_endpoint (IntPtr handle, IntPtr endpoint);

		public NWEndpoint? LocalEndpoint {
			get {
				var x = nw_parameters_copy_local_endpoint (GetCheckedHandle ());
				if (x == IntPtr.Zero)
					return null;

				return new NWEndpoint (x, owns: true);
			}

			set {
				nw_parameters_set_local_endpoint (GetCheckedHandle (), value.GetHandle ());
			}
		}


		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_include_peer_to_peer (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool includePeerToPeer);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_include_peer_to_peer (IntPtr handle);

		public bool IncludePeerToPeer {
			get => nw_parameters_get_include_peer_to_peer (GetCheckedHandle ());
			set => nw_parameters_set_include_peer_to_peer (GetCheckedHandle (), value);
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
		static extern bool nw_parameters_get_prohibit_constrained (IntPtr parameters);

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
		static extern void nw_parameters_set_prohibit_constrained (IntPtr parameters, [MarshalAs (UnmanagedType.I1)] bool prohibit_constrained);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public bool ProhibitConstrained {
			get => nw_parameters_get_prohibit_constrained (GetCheckedHandle ());
			set => nw_parameters_set_prohibit_constrained (GetCheckedHandle (), value);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_attribution (OS_nw_parameters parameters, NWParametersAttribution attribution);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern NWParametersAttribution nw_parameters_get_attribution (OS_nw_parameters parameters);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public NWParametersAttribution Attribution {
			get => nw_parameters_get_attribution (GetCheckedHandle ());
			set => nw_parameters_set_attribution (GetCheckedHandle (), value);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_privacy_context (OS_nw_parameters parameters, OS_nw_privacy_context privacy_context);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public void SetPrivacyContext (NWPrivacyContext privacyContext)
			=> nw_parameters_set_privacy_context (GetCheckedHandle (), privacyContext.Handle);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern OS_nw_parameters nw_parameters_create_quic (void* configureQuic);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
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

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		[return: MarshalAs (UnmanagedType.I1)]
		[DllImport (Constants.NetworkLibrary)]
		static extern bool nw_parameters_requires_dnssec_validation (OS_nw_parameters parameters);

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_set_requires_dnssec_validation (OS_nw_parameters parameters, [MarshalAs (UnmanagedType.I1)] bool requires_dnssec_validation);

#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[TV (16, 0)]
		[Mac (13, 0)]
		[iOS (16, 0)]
		[Watch (9, 0)]
#endif
		public bool RequiresDnssecValidation {
			get => nw_parameters_requires_dnssec_validation (GetCheckedHandle ());
			set => nw_parameters_set_requires_dnssec_validation (GetCheckedHandle (), value);
		}
	}
}
