// 
// SecIdentity.cs: Implements the managed SecIdentity wrapper.
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace Security {

	public partial class SecIdentity {

		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static SecStatusCode /* OSStatus */ SecIdentityCopyPrivateKey (IntPtr /* SecIdentityRef */ identity, IntPtr* /* SecKeyRef* */ privatekey);

		public SecKey PrivateKey {
			get {
				IntPtr p;
				SecStatusCode result;
				unsafe {
					result = SecIdentityCopyPrivateKey (Handle, &p);
				}
				if (result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
				return new SecKey (p, true);
			}
		}
	}
}
