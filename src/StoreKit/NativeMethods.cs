using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace StoreKit {

	partial class SKReceiptRefreshRequest {
		[Watch (6, 2), iOS (7,1), Mac (10,14)]
		[DllImport (Constants.StoreKitLibrary, EntryPoint = "SKTerminateForInvalidReceipt")]
		static extern public void TerminateForInvalidReceipt ();
	}
}