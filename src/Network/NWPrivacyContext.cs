#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using System.Runtime.Versioning;

using OS_nw_privacy_context=System.IntPtr;
using OS_nw_resolver_config=System.IntPtr;

namespace Network {
	[Watch (8,0), TV (15,0), Mac (12,0), iOS (15,0)]
	public class NWPrivacyContext : NativeObject {

		public NWPrivacyContext (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe OS_nw_privacy_context nw_privacy_context_create (string description);

		public NWPrivacyContext (string description)
			=> InitializeHandle (nw_privacy_context_create (description));

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_privacy_context_flush_cache (OS_nw_privacy_context privacyContext);

		public void FlushCache ()
			=> nw_privacy_context_flush_cache (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_privacy_context_disable_logging (OS_nw_privacy_context privacyContext);

		public void DisableLogging ()
			=> nw_privacy_context_disable_logging (GetCheckedHandle ()); 

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_privacy_context_require_encrypted_name_resolution (OS_nw_privacy_context privacyContext, bool requireEncryptedNameResolution, OS_nw_resolver_config fallbackResolverConfig);

		public void RequireEncryptedNameResolution (bool requireEncryptedNameResolution, NWResolverConfig? fallbackResolverConfig)
			=> nw_privacy_context_require_encrypted_name_resolution (GetCheckedHandle (), requireEncryptedNameResolution, fallbackResolverConfig.GetHandle ());
	}
}
