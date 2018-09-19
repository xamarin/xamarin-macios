//
// NWError.cs: Bindings the Netowrk nw_error API.
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

namespace Network {
	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class NWError : NativeObject {
		public NWError (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern NWErrorDomain nw_error_get_error_domain (IntPtr error);

		public NWErrorDomain ErrorDomain => nw_error_get_error_domain (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern /* int */ int nw_error_get_error_code (IntPtr handle);

		public int ErrorCode => nw_error_get_error_code (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_error_copy_cf_error (IntPtr error);

		public CFException CFError {
			get {
				return CFException.FromCFError (nw_error_copy_cf_error (GetCheckedHandle ()), true);
			}
		}
	}
}
