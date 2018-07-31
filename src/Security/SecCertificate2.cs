//
// SecCertificate2.cs: Bindings the Security's sec_trust_t
//
// The difference between SecCertificate2 and SecCertificate is that the
// SecCertificate2 is a binding for the new sec_trust_t API that was
// introduced on iOS 12/OSX Mojave, while SecCertificate is the older API
// that binds SecCertificateRef.
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
	public class SecCertificate2 : NativeObject {
		public SecCertificate2 (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_certificate_create (IntPtr seccertificateHandle);

		public SecCertificate2 (SecCertificate certificate)
		{
			if (certificate == null)
				throw new ArgumentNullException (nameof (certificate));
			handle = sec_certificate_create (certificate.Handle);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_copy_ref (IntPtr handle);

		public SecCertificate Certificate => new SecCertificate (sec_identity_copy_ref (GetHandle ()), owns: true);
	}
}
