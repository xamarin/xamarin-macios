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
	public class SecIdentity2 : NativeObject {
#if NET
		[Preserve (Conditional = true)]
		internal SecIdentity2 (NativeHandle handle, bool owns) : base (handle, owns) {}
#else
		internal SecIdentity2 (NativeHandle handle) : base (handle, false) { }
		[Preserve (Conditional = true)]
		public SecIdentity2 (NativeHandle handle, bool owns) : base (handle, owns) { }
#endif

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_create (IntPtr secidentityHandle);

		public SecIdentity2 (SecIdentity identity)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));

			InitializeHandle (sec_identity_create (identity.Handle));
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_create_with_certificates (IntPtr secidentityHandle, IntPtr arrayHandle);

		public SecIdentity2 (SecIdentity identity, params SecCertificate [] certificates)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));
			if (certificates is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificates));
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
				try {
					return NSArray.ArrayFromHandle<SecCertificate> (certArray);
				} finally {
					CFObject.CFRelease (certArray);
				}
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool sec_identity_access_certificates (IntPtr identity, BlockLiteral* block);

#if !NET
		internal delegate void AccessCertificatesHandler (IntPtr block, IntPtr cert);
		static readonly AccessCertificatesHandler access = TrampolineAccessCertificates;

		[MonoPInvokeCallback (typeof (AccessCertificatesHandler))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineAccessCertificates (IntPtr block, IntPtr cert)
		{
			var del = BlockLiteral.GetTarget<Action<SecCertificate2>> (block);
			if (del is not null)
				del (new SecCertificate2 (cert, false));
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		// no [Async] as it can be called multiple times
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool AccessCertificates (Action</* sec_identity_t */SecCertificate2> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineAccessCertificates;
				using var block = new BlockLiteral (trampoline, handler, typeof (SecIdentity2), nameof (TrampolineAccessCertificates));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (access, handler);
#endif
				return sec_identity_access_certificates (GetCheckedHandle (), &block);
			}
		}
#endif
	}
}
