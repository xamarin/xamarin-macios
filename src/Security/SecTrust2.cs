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

namespace Security {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (5, 0)]
#endif
	public class SecTrust2 : NativeObject {
		[Preserve (Conditional = true)]
#if NET
		internal SecTrust2 (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		public SecTrust2 (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_trust_create (IntPtr sectrustHandle);

		public SecTrust2 (SecTrust trust)
		{
			if (trust is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (trust));

			Handle = sec_trust_create (trust.Handle);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_trust_copy_ref (IntPtr handle);

		public SecTrust Trust => new SecTrust (sec_trust_copy_ref (GetCheckedHandle ()), owns: true);
	}
}
