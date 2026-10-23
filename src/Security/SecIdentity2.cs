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

using System.Runtime.CompilerServices;
using CoreFoundation;

namespace Security {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class SecIdentity2 : NativeObject {
		[Preserve (Conditional = true)]
		internal SecIdentity2 (NativeHandle handle, bool owns) : base (handle, owns) { }

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_create (IntPtr secidentityHandle);

		/// <param name="identity">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public SecIdentity2 (SecIdentity identity)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));

			InitializeHandle (sec_identity_create (identity.Handle));
			GC.KeepAlive (identity);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_create_with_certificates (IntPtr secidentityHandle, IntPtr arrayHandle);

		/// <param name="identity">To be added.</param>
		///         <param name="certificates">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public SecIdentity2 (SecIdentity identity, params SecCertificate [] certificates)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));
			if (certificates is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificates));
			using (var nsarray = NSArray.FromObjects (certificates)) {
				InitializeHandle (sec_identity_create_with_certificates (identity.Handle, nsarray.Handle));
				GC.KeepAlive (identity);
				GC.KeepAlive (nsarray);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* SecIdentityRef */ IntPtr sec_identity_copy_ref (/* OS_sec_identity */ IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public SecIdentity Identity => new SecIdentity (sec_identity_copy_ref (GetCheckedHandle ()), owns: true);

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_identity_copy_certificates_ref (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public SecCertificate []? Certificates {
			get {
				var certArray = sec_identity_copy_certificates_ref (GetCheckedHandle ());
				return NSArray.ArrayFromHandleDropNullElements<SecCertificate> (certArray, releaseHandle: true);
			}
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern byte sec_identity_access_certificates (IntPtr identity, BlockLiteral* block);

		[UnmanagedCallersOnly]
		static void TrampolineAccessCertificates (IntPtr block, IntPtr cert)
		{
			var del = BlockLiteral.GetTarget<Action<SecCertificate2>> (block);
			if (del is not null)
				del (new SecCertificate2 (cert, false));
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		// no [Async] as it can be called multiple times
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool AccessCertificates (Action</* sec_identity_t */SecCertificate2> handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineAccessCertificates;
				using var block = new BlockLiteral (trampoline, handler, typeof (SecIdentity2), nameof (TrampolineAccessCertificates));
				var result = sec_identity_access_certificates (GetCheckedHandle (), &block) != 0;
				GC.KeepAlive (this);
				return result;
			}
		}
#endif
	}
}
