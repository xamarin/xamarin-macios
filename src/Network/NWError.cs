//
// NWError.cs: Bindings the Netowrk nw_error API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {
#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (6, 0)]
#endif
	public class NWError : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal NWError (NativeHandle handle, bool owns) : base (handle, owns)
#else
		public NWError (NativeHandle handle, bool owns) : base (handle, owns)
#endif
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
