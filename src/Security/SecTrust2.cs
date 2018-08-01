//
// SecTrust2.cs: Bindings the Security's sec_trust_t
//
// The difference between SecTrust2 and SecTrust is that the
// SecTrust2 is a binding for the new sec_trust_t API that was
// introduced on iOS 12/OSX Mojave, while SecTrust is the older API
// that binds SecTrustRef.
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

namespace Security {

	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class SecTrust2 : NativeObject {
		internal SecTrust2 (IntPtr handle) : base (handle, false) {}
		public SecTrust2 (IntPtr handle, bool owns) : base (handle, owns) {}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_trust_create (IntPtr sectrustHandle);

		public SecTrust2 (SecTrust trust)
		{
			if (trust == null)
				throw new ArgumentNullException (nameof (trust));

			Handle = sec_trust_create (trust.Handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_trust_copy_ref (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public SecTrust Trust => new SecTrust (sec_trust_copy_ref (GetCheckedHandle ()), owns: true);
	}
}
