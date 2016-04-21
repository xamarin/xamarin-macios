// 
// SecIdentity.cs: Implements the managed SecIdentity wrapper.
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;
using XamCore.CoreFoundation;
using XamCore.Foundation;

namespace XamCore.Security {

	public partial class SecIdentity {

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecIdentityCopyPrivateKey (IntPtr /* SecIdentityRef */ identity, out IntPtr /* SecKeyRef* */ privatekey);

		public SecKey PrivateKey {
			get {
				IntPtr p;
				SecStatusCode result = SecIdentityCopyPrivateKey (handle, out p);
				if (result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
				return new SecKey (p, true);
			}
		}
	}
}