#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace StoreKit {

	partial class SKReceiptRefreshRequest {
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7, 1)]
		[Mac (10, 14)]
#endif
		[DllImport (Constants.StoreKitLibrary, EntryPoint = "SKTerminateForInvalidReceipt")]
		static extern public void TerminateForInvalidReceipt ();
	}
}
