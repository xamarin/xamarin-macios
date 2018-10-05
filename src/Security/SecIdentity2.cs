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

namespace Security {

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), Watch (5,0)]
	public class SecIdentity2 : NativeObject {
		internal SecIdentity2 (IntPtr handle) : base (handle, false) {}
		public SecIdentity2 (IntPtr handle, bool owns) : base (handle, owns) {}

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_create (IntPtr secidentityHandle);

		public SecIdentity2 (SecIdentity identity)
		{
			if (identity == null)
				throw new ArgumentNullException (nameof (identity));

			InitializeHandle (sec_identity_create (identity.Handle));
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_create_with_certificates (IntPtr secidentityHandle, IntPtr arrayHandle);

		public SecIdentity2 (SecIdentity identity, SecCertificate [] certificates)
		{
			if (identity == null)
				throw new ArgumentNullException (nameof (identity));
			if (certificates == null)
				throw new ArgumentNullException (nameof (certificates));
			using (var nsarray = NSArray.FromObjects (certificates))
				InitializeHandle (sec_identity_create_with_certificates (identity.Handle, nsarray.Handle));
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* SecIdentityRef */ IntPtr sec_identity_copy_ref (/* OS_sec_identity */ IntPtr handle);

		public SecIdentity Identity => new SecIdentity (sec_identity_copy_ref (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_copy_certificates_ref (IntPtr handle);

		public SecCertificate [] Certificates {
			get {
				var certArray = sec_identity_copy_certificates_ref (GetCheckedHandle ());
				var n = (int) NSArray.GetCount (certArray);
				var ret = new SecCertificate [n];
				for (int i = 0; i < n; i++)
					ret [i] = new SecCertificate (NSArray.GetAtIndex (certArray, (nuint) i), owns: false);
				CFObject.CFRelease (certArray);
				return ret;
			}
		}
#endif
	}
}
