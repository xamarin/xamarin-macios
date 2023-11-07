#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;
using OS_nw_privacy_context = System.IntPtr;
using OS_nw_resolver_config = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {
#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[Watch (8, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
#endif
	public class NWPrivacyContext : NativeObject {

		public static NWPrivacyContext Default =>
			new NWPrivacyContext (NWPrivacyContextConstants._DefaultContext, false);

		[Preserve (Conditional = true)]
#if NET
		internal NWPrivacyContext (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public NWPrivacyContext (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe OS_nw_privacy_context nw_privacy_context_create (IntPtr description);

		static unsafe OS_nw_privacy_context nw_privacy_context_create (string description)
		{
			using var descriptionPtr = new TransientString (description);
			return nw_privacy_context_create (descriptionPtr);
		}

		public NWPrivacyContext (string description)
			: base (nw_privacy_context_create (description), true) { }

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_privacy_context_flush_cache (OS_nw_privacy_context privacyContext);

		public void FlushCache ()
			=> nw_privacy_context_flush_cache (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_privacy_context_disable_logging (OS_nw_privacy_context privacyContext);

		public void DisableLogging ()
			=> nw_privacy_context_disable_logging (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_privacy_context_require_encrypted_name_resolution (OS_nw_privacy_context privacyContext, [MarshalAs (UnmanagedType.I1)] bool requireEncryptedNameResolution, OS_nw_resolver_config fallbackResolverConfig);

		public void RequireEncryptedNameResolution (bool requireEncryptedNameResolution, NWResolverConfig? fallbackResolverConfig)
			=> nw_privacy_context_require_encrypted_name_resolution (GetCheckedHandle (), requireEncryptedNameResolution, fallbackResolverConfig.GetHandle ());
	}
}
