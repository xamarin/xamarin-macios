using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace StoreKit {

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
