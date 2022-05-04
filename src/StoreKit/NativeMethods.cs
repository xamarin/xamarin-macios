#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace StoreKit {

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	partial class SKReceiptRefreshRequest {
#if NET
		[SupportedOSPlatform ("ios7.1")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7,1)]
		[Mac (10,14)]
#endif
		[DllImport (Constants.StoreKitLibrary, EntryPoint = "SKTerminateForInvalidReceipt")]
		static extern public void TerminateForInvalidReceipt ();
	}
}
