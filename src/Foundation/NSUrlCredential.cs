// Copyright 2013 Xamarin Inc.

using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Security;

namespace Foundation {

	public partial class NSUrlCredential {
		public NSUrlCredential (SecIdentity identity, SecCertificate [] certificates, NSUrlCredentialPersistence persistence)
			: this (identity.Handle, NSArray.FromNativeObjects (certificates).Handle, persistence)
		{
		}

		public static NSUrlCredential FromIdentityCertificatesPersistance (SecIdentity identity, SecCertificate [] certificates, NSUrlCredentialPersistence persistence)
		{
			if (identity is null)
				throw new ArgumentNullException ("identity");

			if (certificates is null)
				throw new ArgumentNullException ("certificates");

			using (var certs = NSArray.FromNativeObjects (certificates))
				return FromIdentityCertificatesPersistanceInternal (identity.Handle, certs.Handle, persistence);
		}

		public SecIdentity SecIdentity {
			get {
				IntPtr handle = Identity;
				return (handle == IntPtr.Zero) ? null : new SecIdentity (handle, false);
			}
		}

		public static NSUrlCredential FromTrust (SecTrust trust)
		{
			if (trust is null)
				throw new ArgumentNullException ("trust");

			return FromTrust (trust.Handle);
		}
	}
}
