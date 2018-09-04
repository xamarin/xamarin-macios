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

	static partial class Libraries  {
		static public class Network {
			static public readonly IntPtr Handle = Dlfcn.dlopen (Constants.NetworkLibrary, 0);
		}
	}

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class NWParameters : NativeObject {
		public NWParameters (IntPtr handle, bool owns) : base (handle, owns) {}

		static unsafe BlockLiteral *DEFAULT_CONFIGURATION () => (BlockLiteral *) NWParametersConstants._DefaultConfiguration;

		static unsafe BlockLiteral *DISABLE_PROTOCOL () => (BlockLiteral *) NWParametersConstants._ProtocolDisable;

		delegate void nw_parameters_configure_protocol_block_t (IntPtr block, IntPtr iface);
		static nw_parameters_configure_protocol_block_t static_ConfigureHandler = TrampolineConfigureHandler;

		[MonoPInvokeCallback(typeof (nw_parameters_configure_protocol_block_t))]
		static void TrampolineConfigureHandler (IntPtr block, IntPtr iface)
		{
			var del = BlockLiteral.GetTarget<Action<NWProtocolOptions>> (block);
			if (del != null) {
				var x = new NWProtocolOptions (iface, owns: false);
				del (x);
				x.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern nw_parameters_t nw_parameters_create_secure_tcp (void *configure_tls, void *configure_tcp);

		//
		// If you pass null, to either configureTls, or configureTcp they will use the default options
		//
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateSecureTcp (Action<NWProtocolOptions> configureTls = null, Action<NWProtocolOptions> configureTcp = null)
		{
			var tlsHandler = new BlockLiteral ();
			var tcpHandler = new BlockLiteral ();

			var tlsPtr = &tlsHandler;
			var tcpPtr = &tcpHandler;
			if (configureTls == null)
				tlsPtr = DEFAULT_CONFIGURATION ();
			else
				tlsHandler.SetupBlockUnsafe (static_ConfigureHandler, configureTls);

			if (configureTcp == null)
				tcpPtr = DEFAULT_CONFIGURATION ();
			else
				tcpHandler.SetupBlockUnsafe (static_ConfigureHandler, configureTcp);

			var ptr = nw_parameters_create_secure_tcp (tlsPtr, tcpPtr);

			if (configureTls != null)
				tlsPtr->CleanupBlock ();

			if (configureTcp != null)
				tcpPtr->CleanupBlock ();

			return new NWParameters (ptr, owns: true);
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		// If you pass null to configureTcp, it will use the default options
		public unsafe static NWParameters CreateTcp (Action<NWProtocolOptions> configureTcp = null)
		{
			var tcpHandler = new BlockLiteral ();

			var tcpPtr = &tcpHandler;

			if (configureTcp == null)
				tcpPtr = DEFAULT_CONFIGURATION ();
			else
				tcpHandler.SetupBlockUnsafe (static_ConfigureHandler, configureTcp);

			var ptr = nw_parameters_create_secure_tcp (DISABLE_PROTOCOL (), tcpPtr);

			if (configureTcp != null)
				tcpPtr->CleanupBlock ();

			return new NWParameters (ptr, owns: true);
		}


		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe nw_parameters_t nw_parameters_create_secure_udp (void *configure_tls, void *configure_tcp);

		//
		// If you pass null, to either configureTls, or configureTcp they will use the default options
		//
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateSecureUdp (Action<NWProtocolOptions> configureTls = null, Action<NWProtocolOptions> configureUdp = null)
		{
			var tlsHandler = new BlockLiteral ();
			var udpHandler = new BlockLiteral ();

			BlockLiteral *tlsPtr = &tlsHandler;
			BlockLiteral *udpPtr = &udpHandler;

			if (configureTls == null)
				tlsPtr = DEFAULT_CONFIGURATION ();
			else
				tlsHandler.SetupBlockUnsafe (static_ConfigureHandler, configureTls);

			if (configureUdp == null)
				udpPtr = DEFAULT_CONFIGURATION ();
			else
				udpHandler.SetupBlockUnsafe (static_ConfigureHandler, configureUdp);

			var ptr = nw_parameters_create_secure_udp (tlsPtr, udpPtr);

			if (configureTls != null)
				tlsPtr->CleanupBlock ();

			if (configureUdp != null)
				udpPtr->CleanupBlock ();

			return new NWParameters (ptr, owns: true);
		}

		// If you pass null to configureTcp, it will use the default options
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static NWParameters CreateUdp (Action<NWProtocolOptions> configureUdp = null)
		{
			var udpHandler = new BlockLiteral ();

			var udpPtr = &udpHandler;
			if (configureUdp == null)
				udpPtr = DEFAULT_CONFIGURATION ();
			else
				udpHandler.SetupBlockUnsafe (static_ConfigureHandler, configureUdp);

			var ptr = nw_parameters_create_secure_udp (DISABLE_PROTOCOL (), udpPtr);

			if (configureUdp != null)
				udpPtr->CleanupBlock ();

			return new NWParameters (ptr, owns: true);
		}

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

		public NWInterface RequireInterface {
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
			if (iface == null)
				throw new ArgumentNullException (nameof (iface));

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

		delegate bool nw_parameters_iterate_interfaces_block_t (IntPtr block, IntPtr iface);
		static nw_parameters_iterate_interfaces_block_t static_iterateProhibitedHandler = TrampolineIterateProhibitedHandler;

		[MonoPInvokeCallback (typeof (nw_parameters_iterate_interfaces_block_t))]
		[return: MarshalAs (UnmanagedType.I1)]
		static bool TrampolineIterateProhibitedHandler (IntPtr block, IntPtr iface)
		{
			var del = BlockLiteral.GetTarget<Func<NWInterface,bool>> (block);
			if (del != null) {
				var x = new NWInterface (iface, owns: false);
				var ret = del (x);
				x.Dispose ();
				return ret;
			}
			return false;
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_iterate_prohibited_interfaces (nw_parameters_t parameters, ref BlockLiteral callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProhibitedInterfaces (Func<NWInterface,bool> iterationCallback)
		{
			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_iterateProhibitedHandler, iterationCallback);

			try {
				nw_parameters_iterate_prohibited_interfaces (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		delegate bool nw_parameters_iterate_interface_types_block_t (IntPtr block, NWInterfaceType type);
		static nw_parameters_iterate_interface_types_block_t static_IterateProhibitedTypeHandler = TrampolineIterateProhibitedTypeHandler;

		[MonoPInvokeCallback (typeof (nw_parameters_iterate_interface_types_block_t))]
		[return: MarshalAs (UnmanagedType.I1)]
		static bool TrampolineIterateProhibitedTypeHandler (IntPtr block, NWInterfaceType type)
		{
			var del = BlockLiteral.GetTarget<Func<NWInterfaceType,bool>> (block);
			if (del != null)
				return del (type);
			return false;
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_parameters_iterate_prohibited_interface_types (IntPtr handle, ref BlockLiteral callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void IterateProhibitedInterfaces (Func<NWInterfaceType,bool> callback)
		{
			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_IterateProhibitedTypeHandler, callback);

			try {
				nw_parameters_iterate_prohibited_interface_types (GetCheckedHandle (), ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
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

		public NWEndpoint LocalEndpoint {
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
		static extern void nw_parameters_set_include_peer_to_peer (IntPtr handle, bool includePeerToPeer);

		[DllImport (Constants.NetworkLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool nw_parameters_get_include_peer_to_peer (IntPtr handle);

		public bool IncludePeerToPeer {
			get => nw_parameters_get_include_peer_to_peer (GetCheckedHandle ());
			set => nw_parameters_set_include_peer_to_peer (GetCheckedHandle (), value);
		}
	}

	public enum NWParametersExpiredDnsBehavior {
		Default = 0,
		Allow = 1,
		Prohibit = 2,
	}
}
