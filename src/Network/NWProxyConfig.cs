#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_proxy_config = System.IntPtr;
using OS_nw_relay_hop = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_protocol_options = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
#else
	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
	public class NWProxyConfig : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWProxyConfig (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWProxyConfig (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.NetworkLibrary)]
		static extern OS_nw_proxy_config nw_proxy_config_create_relay (OS_nw_relay_hop first_hop, /* [NullAllowed] */ OS_nw_relay_hop second_hop);

		public static NWProxyConfig? CreateRelay (NWRelayHop firstHop, NWRelayHop? secondHop)
		{
			if (firstHop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (firstHop));
			var handle = nw_proxy_config_create_relay (firstHop.GetCheckedHandle (), secondHop.GetHandle ());
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
			var pinned = GCHandle.Alloc (gatewayKeyConfig, GCHandleType.Pinned);

			unsafe {
				fixed (byte* gatewayKeyConfigPointer = gatewayKeyConfig) {
					var handle = nw_proxy_config_create_oblivious_http (hop.GetCheckedHandle (), resourcePathPtr, gatewayKeyConfigPointer, (nuint) gatewayKeyConfig.Length);
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

#if !NET
		delegate void nw_proxy_config_enumerate_match_domains_t (IntPtr block, IntPtr domain);
		static nw_proxy_config_enumerate_match_domains_t static_EnumerateMatchDomainHandler = TrampolineEnumerateMatchDomainHandler;

		[MonoPInvokeCallback (typeof (nw_proxy_config_enumerate_match_domains_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateMatchDomainHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWProxyConfig), nameof (TrampolineEnumerateMatchDomainHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_EnumerateMatchDomainHandler, handler);
#endif
				nw_proxy_config_enumerate_match_domains (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static unsafe extern void nw_proxy_config_enumerate_excluded_domains (OS_nw_proxy_config config, BlockLiteral* enumerator);

#if !NET
		delegate void nw_proxy_config_enumerate_exclude_domains_t (IntPtr block, IntPtr domain);
		static nw_proxy_config_enumerate_exclude_domains_t static_EnumerateExcludeDomainHandler = TrampolineEnumerateExcludeDomainHandler;

		[MonoPInvokeCallback (typeof (nw_proxy_config_enumerate_match_domains_t))]
#else
		[UnmanagedCallersOnly]
#endif
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
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineEnumerateExcludeDomainHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWProxyConfig), nameof (TrampolineEnumerateExcludeDomainHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_EnumerateExcludeDomainHandler, handler);
#endif
				nw_proxy_config_enumerate_excluded_domains (GetCheckedHandle (), &block);
			}
		}

	}
}
