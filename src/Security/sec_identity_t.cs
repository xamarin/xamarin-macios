//
// SecIdentity2.cs: Bindings the Security's sec_identity_t
//
// The difference between SecIdentity2 and SecIdentity is that the
// SecIdentity2 is a binding for the new sec_identity_t API that was
// introduced on iOS 12/OSX Mojave, while SecIdentity is the older API
// that binds SecIdentityRef.
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
	
	public class SecIdentity2 : NativeObject {
		public SecIdentity2 (IntPtr handle, bool owns) : base (handle, owns) {}

		
	}
}
