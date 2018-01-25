// Copyright 2013 Xamarin Inc.

using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Security;

namespace Foundation {

	public partial class NSUrlCredential {
#if !XAMCORE_2_0
		[Obsolete ("Use 'NSUrlCredential(SecTrust)' constructor.")]
		public NSUrlCredential (IntPtr trust, bool ignored) : base (NSObjectFlag.Empty)
		{
			if (IsDirectBinding) {
				Handle = Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("initWithTrust:"), trust);
			} else {
				Handle = Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.GetHandle ("initWithTrust:"), trust);
			}
		}
#endif

		public NSUrlCredential (SecIdentity identity, SecCertificate [] certificates, NSUrlCredentialPersistence persistence)
			: this (identity.Handle, NSArray.FromNativeObjects (certificates).Handle, persistence)
		{
		}

		public static NSUrlCredential FromIdentityCertificatesPersistance (SecIdentity identity, SecCertificate [] certificates, NSUrlCredentialPersistence persistence)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");

			if (certificates == null)
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
			if (trust == null)
				throw new ArgumentNullException ("trust");

			return FromTrust (trust.Handle);
		}
	}
}
