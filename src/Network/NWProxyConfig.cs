#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_proxy_config = System.IntPtr;
using OS_nw_relay_hop = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_protocol_options = System.IntPtr;

namespace Network {
	[SupportedOSPlatform ("tvos17.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst")]
	public class NWProxyConfig : NativeObject {
		[Preserve (Conditional = true)]
		internal NWProxyConfig (NativeHandle handle, bool owns) : base (handle, owns) { }

#if !COREBUILD
		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_proxy_config nw_proxy_config_create_relay (OS_nw_relay_hop first_hop, /* [NullAllowed] */ OS_nw_relay_hop second_hop);

		public static NWProxyConfig? CreateRelay (NWRelayHop firstHop, NWRelayHop? secondHop)
		{
			if (firstHop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (firstHop));
			var handle = nw_proxy_config_create_relay (firstHop.GetCheckedHandle (), secondHop.GetHandle ());
			GC.KeepAlive (firstHop);
			GC.KeepAlive (secondHop);
			if (handle == NativeHandle.Zero)
				return default;
			return new NWProxyConfig (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern OS_nw_proxy_config nw_proxy_config_create_oblivious_http (OS_nw_relay_hop relay, /* sbyte* */IntPtr relay_resource_path, byte* gateway_key_config, nuint gateway_key_config_length);

		public static NWProxyConfig? CreateObliviousHttp (NWRelayHop hop, string relaysResourcePath, byte [] gatewayKeyConfig)
		{
			if (hop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (hop));
			if (relaysResourcePath is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (relaysResourcePath));
			if (gatewayKeyConfig is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (gatewayKeyConfig));

			using var resourcePathPtr = new TransientString (relaysResourcePath);

			unsafe {
				fixed (byte* gatewayKeyConfigPointer = gatewayKeyConfig) {
					var handle = nw_proxy_config_create_oblivious_http (hop.GetCheckedHandle (), resourcePathPtr, gatewayKeyConfigPointer, (nuint) gatewayKeyConfig.Length);
					GC.KeepAlive (hop);
					if (handle == NativeHandle.Zero)
						return default;
					return new NWProxyConfig (handle, owns: true);
				}
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_proxy_config nw_proxy_config_create_http_connect (OS_nw_endpoint proxy_endpoint, /* [NullAllowed] */ OS_nw_protocol_options proxy_tls_options);

		public static NWProxyConfig? CreateHttpConnect (NWEndpoint proxyEndpoint, NWProtocolTlsOptions? options)
		{
			if (proxyEndpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxyEndpoint));

			var handle = nw_proxy_config_create_http_connect (proxyEndpoint.GetCheckedHandle (), options.GetHandle ());
			GC.KeepAlive (proxyEndpoint);
			GC.KeepAlive (options);
			if (handle == NativeHandle.Zero)
				return default;
			return new NWProxyConfig (handle, true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_proxy_config nw_proxy_config_create_socksv5 (OS_nw_endpoint proxy_endpoint);

		public static NWProxyConfig? CreateSocksV5 (NWEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			var handle = nw_proxy_config_create_socksv5 (endpoint.GetCheckedHandle ());
			GC.KeepAlive (endpoint);
			if (handle == NativeHandle.Zero)
				return default;
			return new NWProxyConfig (handle, true);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_proxy_config_set_username_and_password (OS_nw_proxy_config proxy_config, /* sbyte* */ IntPtr username, /* sbyte* */ IntPtr password);

		public void SetUsernameAndPassword (string username, string password)
		{
			if (username is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (username));
			if (password is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (password));

			using var usernamePtr = new TransientString (username);
			using var passwordPtr = new TransientString (password);
			nw_proxy_config_set_username_and_password (GetCheckedHandle (), usernamePtr, passwordPtr);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_proxy_config_set_failover_allowed (OS_nw_proxy_config proxy_config, /* bool */ byte failover_allowed);

		[DllImport (Constants.NetworkLibrary)]
		static extern byte nw_proxy_config_get_failover_allowed (OS_nw_proxy_config proxy_config);

		public bool FailoverAllowed {
			get => nw_proxy_config_get_failover_allowed (GetCheckedHandle ()) != 0;
			set => nw_proxy_config_set_failover_allowed (GetCheckedHandle (), (byte) (value ? 1 : 0));
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_proxy_config_add_match_domain (OS_nw_proxy_config config, /* sbyte* */ IntPtr match_domain);

		public void AddMatchDomain (string domain)
		{
			if (domain is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (domain));
			using var domainPtr = new TransientString (domain);
			nw_proxy_config_add_match_domain (GetCheckedHandle (), domainPtr);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_proxy_config_clear_match_domains (OS_nw_proxy_config config);

		public void ClearMatchDomains () => nw_proxy_config_clear_match_domains (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_proxy_config_add_excluded_domain (OS_nw_proxy_config config, /* sbyte* */ IntPtr excluded_domain);

		public void AddExcludedDomain (string domain)
		{
			if (domain is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (domain));
			using var domainPtr = new TransientString (domain);
			nw_proxy_config_add_excluded_domain (GetCheckedHandle (), domainPtr);
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_proxy_config_clear_excluded_domains (OS_nw_proxy_config config);

		public void ClearExcludedDomains () => nw_proxy_config_clear_excluded_domains (GetCheckedHandle ());


		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern void nw_proxy_config_enumerate_match_domains (OS_nw_proxy_config config, /* nw_proxy_domain_enumerator_t */ BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static void TrampolineEnumerateMatchDomainHandler (IntPtr block, IntPtr domainPtr)
		{
			var del = BlockLiteral.GetTarget<Action<string?>> (block);
			if (del is not null) {
				var domain = Marshal.PtrToStringAuto (domainPtr);
				del (domain);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateMatchDomains (Action<string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateMatchDomainHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWProxyConfig), nameof (TrampolineEnumerateMatchDomainHandler));
				nw_proxy_config_enumerate_match_domains (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern void nw_proxy_config_enumerate_excluded_domains (OS_nw_proxy_config config, BlockLiteral* enumerator);

		[UnmanagedCallersOnly]
		static void TrampolineEnumerateExcludeDomainHandler (IntPtr block, IntPtr domainPtr)
		{
			var del = BlockLiteral.GetTarget<Action<string?>> (block);
			if (del is not null) {
				var domain = Marshal.PtrToStringAuto (domainPtr);
				del (domain);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateExcludedDomains (Action<string?> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateExcludeDomainHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWProxyConfig), nameof (TrampolineEnumerateExcludeDomainHandler));
				nw_proxy_config_enumerate_excluded_domains (GetCheckedHandle (), &block);
				GC.KeepAlive (this);
			}
		}
#endif // COREBUILD

	}
}
