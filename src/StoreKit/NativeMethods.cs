using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace StoreKit {

	partial class SKReceiptRefreshRequest {
#if !MONOMAC || !XAMCORE_2_0
		[iOS (7,1)]
		[DllImport (Constants.StoreKitLibrary, EntryPoint = "SKTerminateForInvalidReceipt")]
		static extern public void TerminateForInvalidReceipt ();
#endif
	}
}