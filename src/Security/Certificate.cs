// 
// Certificate.cs: Implements the managed SecCertificate wrapper.
//
// Authors: 
//	Miguel de Icaza
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2010 Novell, Inc
// Copyright 2012-2013 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

#if !NET
#define NATIVE_APPLE_CERTIFICATE
#endif

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Security {

	public partial class SecCertificate : NativeObject {
#if !NET
		public SecCertificate (NativeHandle handle)
			: base (handle, false, verify: true)
		{
		}
#endif // !NET

		[Preserve (Conditional = true)]
		internal SecCertificate (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}
#if !COREBUILD
		[DllImport (Constants.SecurityLibrary, EntryPoint = "SecCertificateGetTypeID")]
		public extern static nint GetTypeID ();

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr SecCertificateCreateWithData (IntPtr allocator, IntPtr cfData);

		public SecCertificate (NSData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			Initialize (data);
		}

		public SecCertificate (byte [] data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			using (NSData cert = NSData.FromArray (data)) {
				Initialize (cert);
			}
		}

		public SecCertificate (X509Certificate certificate)
		{
			if (certificate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificate));

#if NATIVE_APPLE_CERTIFICATE
			var handle = certificate.Impl.GetNativeAppleCertificate ();
			if (handle != IntPtr.Zero) {
				CFObject.CFRetain (handle);
				InitializeHandle (handle);
				return;
			}
#endif

			using (NSData cert = NSData.FromArray (certificate.GetRawCertData ())) {
				Initialize (cert);
			}
		}

#if NATIVE_APPLE_CERTIFICATE
		internal SecCertificate (X509CertificateImpl impl)
		{
			var handle = impl.GetNativeAppleCertificate ();
			if (handle != IntPtr.Zero) {
				CFObject.CFRetain (handle);
				InitializeHandle (handle);
				return;
			}

			using (NSData cert = NSData.FromArray (impl.RawData)) {
				Initialize (cert);
			}
		}
#endif

		public SecCertificate (X509Certificate2 certificate)
		{
			if (certificate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificate));

#if NATIVE_APPLE_CERTIFICATE
			var handle = certificate.Impl.GetNativeAppleCertificate ();
			if (handle != IntPtr.Zero) {
				CFObject.CFRetain (handle);
				InitializeHandle (handle);
				return;
			}
#endif

			using (NSData cert = NSData.FromArray (certificate.RawData)) {
				Initialize (cert);
			}
		}

		void Initialize (NSData data)
		{
			var handle = SecCertificateCreateWithData (IntPtr.Zero, data.Handle);
			if (handle == IntPtr.Zero)
				throw new ArgumentException ("Not a valid DER-encoded X.509 certificate");
			InitializeHandle (handle);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr SecCertificateCopySubjectSummary (IntPtr cert);

		public string? SubjectSummary {
			get {
				return CFString.FromHandle (SecCertificateCopySubjectSummary (GetCheckedHandle ()), releaseHandle: true);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* CFDataRef */ IntPtr SecCertificateCopyData (/* SecCertificateRef */ IntPtr cert);

		public NSData DerData {
			get {
				IntPtr data = SecCertificateCopyData (GetCheckedHandle ());
				if (data == IntPtr.Zero)
					throw new ArgumentException ("Not a valid certificate");
				return Runtime.GetNSObject<NSData> (data, true)!;
			}
		}

		byte [] GetRawData ()
		{
			using (NSData data = DerData)
				return data.ToArray ();
		}

		public X509Certificate ToX509Certificate ()
		{
#if NET
			return X509CertificateLoader.LoadCertificate (GetRawData ());
#else
#if NATIVE_APPLE_CERTIFICATE
			var impl = new Mono.AppleTls.X509CertificateImplApple (GetCheckedHandle (), false);
			return new X509Certificate (impl);
#else
			return new X509Certificate (GetRawData ());
#endif
#endif
		}

		public X509Certificate2 ToX509Certificate2 ()
		{
#if NET
			return X509CertificateLoader.LoadCertificate (GetRawData ());
#else
			return new X509Certificate2 (GetRawData ());
#endif
		}

		internal static bool Equals (SecCertificate first, SecCertificate second)
		{
			/*
			 * This is a little bit expensive, but unfortunately there is no better API to compare two
			 * SecCertificateRef's for equality.
			 */
			if (first is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (first));
			if (second is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (second));
			if (first.Handle == second.Handle)
				return true;

			using (var firstData = first.DerData)
			using (var secondData = second.DerData) {
				if (firstData.Handle == secondData.Handle)
					return true;

				if (firstData.Length != secondData.Length)
					return false;
				nint length = (nint) firstData.Length;
				for (nint i = 0; i < length; i++) {
					if (firstData [i] != secondData [i])
						return false;
				}

				return true;
			}
		}

#if !__MACCATALYST__ // Neither the macOS nor the non-MacOS one works on Mac Catalyst
#if MONOMAC
		/* Only available on OS X v10.7 or later */
		[DllImport (Constants.SecurityLibrary)]
		extern static /* CFDictionaryRef */ IntPtr SecCertificateCopyValues (/* SecCertificateRef */ IntPtr certificate, /* CFArrayRef */ IntPtr keys, /* CFErrorRef _Nullable * */ IntPtr error);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.14", "Use 'GetKey' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10,14, message: "Use 'GetKey' instead.")]
#endif
		public NSData? GetPublicKey ()
		{
			IntPtr result;
			using (var oids = NSArray.FromIntPtrs (new NativeHandle [] { SecCertificateOIDs.SubjectPublicKey })) {
				result = SecCertificateCopyValues (GetCheckedHandle (), oids.Handle, IntPtr.Zero);
				if (result == IntPtr.Zero)
					throw new ArgumentException ("Not a valid certificate");
			}

			using (var dict = new NSDictionary (result, true)) {
				var ptr = dict.LowlevelObjectForKey (SecCertificateOIDs.SubjectPublicKey);
				if (ptr == IntPtr.Zero)
					return null;

				using var publicKeyDict = new NSDictionary (ptr, false);
				var dataPtr = publicKeyDict.LowlevelObjectForKey (SecPropertyKey.Value);
				return Runtime.GetNSObject<NSData> (dataPtr);
			}
		}
#else
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos12.0")]
		[ObsoletedOSPlatform ("ios12.0")]
#else
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.WatchOS, 5, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* __nullable SecKeyRef */ IntPtr SecCertificateCopyPublicKey (IntPtr /* SecCertificateRef */ certificate);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos12.0", "Use 'GetKey' instead.")]
		[ObsoletedOSPlatform ("ios12.0", "Use 'GetKey' instead.")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetKey' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetKey' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetKey' instead.")]
#endif
		public SecKey? GetPublicKey ()
		{
			IntPtr data = SecCertificateCopyPublicKey (Handle);
			return (data == IntPtr.Zero) ? null : new SecKey (data, true);
		}
#endif
#endif // !__MACCATALYST__

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern IntPtr /* SecKeyRef* */ SecCertificateCopyKey (IntPtr /* SecKeyRef* */ key);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
#endif
		public SecKey? GetKey ()
		{
			var key = SecCertificateCopyKey (Handle);
			return key == IntPtr.Zero ? null : new SecKey (key, true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* OSStatus */ int SecCertificateCopyCommonName (IntPtr /* SecCertificateRef */ certificate, IntPtr* /* CFStringRef * __nonnull CF_RETURNS_RETAINED */ commonName);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
		public string? GetCommonName ()
		{
			IntPtr cn;
			unsafe {
				if (SecCertificateCopyCommonName (Handle, &cn) == 0)
					return CFString.FromHandle (cn, releaseHandle: true);
			}
			return null;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* OSStatus */ int SecCertificateCopyEmailAddresses (IntPtr /* SecCertificateRef */ certificate, IntPtr* /* CFArrayRef * __nonnull CF_RETURNS_RETAINED */ emailAddresses);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
		public string? []? GetEmailAddresses ()
		{
			IntPtr emails;
			unsafe {
				if (SecCertificateCopyEmailAddresses (Handle, &emails) == 0)
					return CFArray.StringArrayFromHandle (emails, true);
			}
			return null;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* __nullable CFDataRef */ IntPtr SecCertificateCopyNormalizedIssuerSequence (IntPtr /* SecCertificateRef */ certificate);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetNormalizedIssuerSequence ()
		{
			IntPtr data = SecCertificateCopyNormalizedIssuerSequence (Handle);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* __nullable CFDataRef */ IntPtr SecCertificateCopyNormalizedSubjectSequence (IntPtr /* SecCertificateRef */ certificate);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetNormalizedSubjectSequence ()
		{
			IntPtr data = SecCertificateCopyNormalizedSubjectSequence (Handle);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if MONOMAC
#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'GetSerialNumber' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'GetSerialNumber' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* __nullable CFDataRef */ IntPtr SecCertificateCopySerialNumber (IntPtr /* SecCertificateRef */ certificate, IntPtr /* CFErrorRef * */ error);

#else // !MONOMAC
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'GetSerialNumber' instead.")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'GetSerialNumber' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'GetSerialNumber' instead.")]
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetSerialNumber' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetSerialNumber' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GetSerialNumber' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetSerialNumber' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* __nullable CFDataRef */ IntPtr SecCertificateCopySerialNumber (IntPtr /* SecCertificateRef */ certificate);
#endif
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'GetSerialNumber(out NSError)' instead.")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'GetSerialNumber(out NSError)' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'GetSerialNumber(out NSError)' instead.")]
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetSerialNumber(out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetSerialNumber(out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GetSerialNumber(out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetSerialNumber(out NSError)' instead.")]
#endif
		public NSData? GetSerialNumber ()
		{
#if MONOMAC
			IntPtr data = SecCertificateCopySerialNumber (Handle, IntPtr.Zero);
#else
			IntPtr data = SecCertificateCopySerialNumber (Handle);
#endif
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* __nullable CFDataRef */ IntPtr SecCertificateCopySerialNumberData (IntPtr /* SecCertificateRef */ certificate, IntPtr* /* CFErrorRef * */ error);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetSerialNumber (out NSError? error)
		{
			IntPtr err = IntPtr.Zero;
			IntPtr data;
			unsafe {
				data = SecCertificateCopySerialNumberData (Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* CFDateRef */ IntPtr SecCertificateCopyNotValidBeforeDate (/* SecCertificateRef */ IntPtr certificate);

		/// <summary>Get the date when this certificate becomes valid.</summary>
		/// <returns>The date when this certificate becomes valid, or null if the date could not be obtained.</returns>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public NSDate? NotValidBeforeDate {
			get {
				var ptr = SecCertificateCopyNotValidBeforeDate (Handle);
				return Runtime.GetNSObject<NSDate> (ptr, owns: true);
			}
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* CFDateRef */ IntPtr SecCertificateCopyNotValidAfterDate (/* SecCertificateRef */ IntPtr certificate);

		/// <summary>Get the date when this certificate is no longer valid.</summary>
		/// <returns>The date when this certificate is no longer valid, or null if the date could not be obtained.</returns>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public NSDate? NotValidAfterDate {
			get {
				var ptr = SecCertificateCopyNotValidAfterDate (Handle);
				return Runtime.GetNSObject<NSDate> (ptr, owns: true);
			}
		}

#endif // COREBUILD
	}

	public partial class SecIdentity : NativeObject {
#if !NET
		public SecIdentity (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal SecIdentity (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary, EntryPoint = "SecIdentityGetTypeID")]
		public extern static nint GetTypeID ();

		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static /* OSStatus */ SecStatusCode SecIdentityCopyCertificate (/* SecIdentityRef */ IntPtr identityRef,  /* SecCertificateRef* */ IntPtr* certificateRef);

		public SecCertificate Certificate {
			get {
				SecStatusCode result;
				IntPtr cert;
				unsafe {
					result = SecIdentityCopyCertificate (GetCheckedHandle (), &cert);
				}
				if (result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
				return new SecCertificate (cert, true);
			}
		}

		/// <summary>Create a <see cref="SecIdentity" /> from PKCS #12 data.</summary>
		/// <param name="data">The PKCS #12 blob data as a byte array.</param>
		/// <param name="password">The password for the private key in the PKCS #12 data. An empty password is not supported.</param>
		/// <remarks>
		///    <para>On macOS 14 or earlier this method requires access to the default keychain,where the private key + certificate will be stored.</para>
		///    <para>On macOS 15 or later, as well as all other platforms (iOS, tvOS, Mac Catalyst), calling this method will not affect any keychains (a temporary, in-memory keychain is created for the duration of the import).</para>
		/// </remarks>
		public static SecIdentity Import (byte [] data, string password)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			if (string.IsNullOrEmpty (password)) // SecPKCS12Import() doesn't allow empty passwords.
				throw new ArgumentException (nameof (password));
			using (var pwstring = new NSString (password))
			using (var options = NSMutableDictionary.FromObjectAndKey (pwstring, SecImportExport.Passphrase)) {
				NSDictionary [] array;
#if __MACOS__
				/* There are unfortunate platform differences for SecPKCS12Import:
				 *
				 * `SecPKCS12Import` will, _on macOS only, not other platforms_, add the imported certificate + private key into the default keychain.
				 *
				 * Apple's documentation for [`SecPKCS12Import`](https://developer.apple.com/documentation/security/1396915-secpkcs12import?language=objc)
				 * implies importing into the keychain is optional ("[...] You can then use the Keychain Services API
				 * (see [Keychain services](https://developer.apple.com/documentation/security/keychain_services?language=objc))
				 * to put the identities and associated certificates in the keychain."), but that doesn't match the behavior we're seeing,
				 * either on the bots nor locally (if I lock the keychain before running the unit test on macOS, I get a dialog asking for
				 * my password to unlock the keychain when this method is called). Other people on StackOverflow has run into the same issue
				 * (https://stackoverflow.com/q/33181127), where one of the answers points to the source code (https://stackoverflow.com/a/66846609),
				 * confirming this behavior.
				 *
				 * StackOverflow also suggests using [`SecItemImport`](https://developer.apple.com/documentation/security/1395728-secitemimport)
				 * instead, which works, with a few caveats:
				 *
				 * 1. Importing a PKCS#12 blob only returns the certificate, not the private key. This is a bug, as [confirmed](https://forums.developer.apple.com/forums/thread/31711) by Quinn "The Eskimo!":
				 *
				 *    > `SecItemImport` really does support importing private keys without putting them in the keychain,
				 *    > and that code all runs and works in the PKCS#12 case; internally I see both the certificate and
				 *    > the private key extracted from the PKCS#12. The problem arises when the code tries to match up
				 *    > the certificate and private key to form an identity. That code is failing in the no-keychain case,
				 *    > so you end up getting back just the certificate. Notably, in the PEM case no matching occurs and
				 *    > thus you get back both the certificate and the private key.
				 *    >
				 *    > This is clearly a bug and I’ve filed it as such (r. 25,140,029).
				 *
				 *    That was 8 years ago, and 6 years later it still hasn't been fixed (as confirmed by Quinn in the same thread), so it's unlikely it'll ever be fixed.
				 *
				 * 2. So I tried exporting the X509Certificate into a PEM string instead, and that works, I successfully
				 *    get back a `SecKey` instance and a `SecCertificate` instance! Success?
				 *
				 * 3. Nope, because there's no way to create a `SecIdentity` from `SecKey`+`SecCertificate`. You have to put
				 *    the `SecKey` into a keychain, and then pass the `SecCertificate` to [`SecIdentityCreateWithCertificate`](https://developer.apple.com/documentation/security/1401160-secidentitycreatewithcertificate?language=objc),
				 *    and we're back to where we started.
				 *
				 * 4. OK, what about creating a temporary `SecKeychain`, add the `SecKey` there, create the `SecIdentity`, then delete the `SecKeychain`?
				 *
				 *      * [`SecKeyChain`](https://developer.apple.com/documentation/security/1401214-seckeychaincreate) was deprecated in macOS 10.10 :/
				 *      * curl had the same problem:
				 *          * https://github.com/curl/curl/issues/10038
				 *          * https://github.com/curl/curl/issues/5403)
				 *
				 *          There was PR to use a temporary keychain (https://github.com/curl/curl/pull/10059), with a number of good reasons why it was a bad idea, so it was eventually rejected:
				 *
				 *          * The temporary keychain is stored on disk, which isn't particularly fast.
				 *          * The ownership/rights of the file must be considered to ensure there are no security issues.
				 *          * Concurrency would have to be considered with regards to cleanup - what if there are multiple threads trying to use the same location on disk.
				 *
				 *
				 * In [this Apple forum thread](https://forums.developer.apple.com/forums/thread/31711) a user gripes about this exact problem:
				 *
				 *     > I've resorted to using a private API to created a SecIdentity from a SecCertificate and a SecKey that I already have in memory.
				 *
				 * Quinn answers:
				 *
				 * > On the macOS front, there’s nothing stopping a command-line tool running on a CI machine using the keychain.
				 *   There are some pain points but no showstoppers.
				 *
				 * FWIW Quinn “The Eskimo!” wrote a document explaining how to find and fix problems with regards to the keychain on CI machines
				 * (https://developer.apple.com/forums/thread/712005). The last sentence is a gem: "Resetting trust settings is more of a challenge.
				 * It’s probably possible to do this with the security tool but, honestly, if you think that your CI system has messed up trust settings
				 * it’s easiest to throw it away and start again from scratch." - in other words if something goes wrong, the easiest is to wipe the
				 * machine and start over again.
				 *
				 *     "some pain points" is somewhat of an understatement...
				 *
				 * The good news is that on macOS 15+, Apple added an option to use a temporary, in-memory only keychain, avoiding the whole problem,
				 * so let's use that!
				 */
				if (OperatingSystem.IsMacOSVersionAtLeast (15, 0))
					options.Add (SecImportExport.ToMemoryOnly, NSNumber.FromBoolean (true));
#endif
				SecStatusCode result = SecImportExport.ImportPkcs12 (data, options, out array);
				if (result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());

				return new SecIdentity (array [0].LowlevelObjectForKey (SecImportExport.Identity.Handle), false);
			}
		}

		public static SecIdentity Import (X509Certificate2 certificate)
		{
			if (certificate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificate));
			if (!certificate.HasPrivateKey)
				throw new InvalidOperationException ("Need X509Certificate2 with a private key.");

			/*
			 * SecPSK12Import does not allow any empty passwords, so let's generate
			 * a semi-random one here.
			 */
			var password = Guid.NewGuid ().ToString ();
			var pkcs12 = certificate.Export (X509ContentType.Pfx, password);
			return Import (pkcs12, password);
		}
#endif
	}

	public partial class SecKey : NativeObject {
#if !NET
		public SecKey (IntPtr handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
#if NET
		internal SecKey (NativeHandle handle, bool owns)
#else
		public SecKey (NativeHandle handle, bool owns)
#endif
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary, EntryPoint = "SecKeyGetTypeID")]
		public extern static nint GetTypeID ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos12.0", "Use 'SecKeyCreateRandomKey' instead.")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'SecKeyCreateRandomKey' instead.")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'SecKeyCreateRandomKey' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'SecKeyCreateRandomKey' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'SecKeyCreateRandomKey' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'SecKeyCreateRandomKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'SecKeyCreateRandomKey' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'SecKeyCreateRandomKey' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'SecKeyCreateRandomKey' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static SecStatusCode SecKeyGeneratePair (IntPtr dictHandle, IntPtr* pubKey, IntPtr* privKey);

		// TODO: pull all the TypeRefs needed for the NSDictionary

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos12.0", "Use 'CreateRandomKey' instead.")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'CreateRandomKey' instead.")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'CreateRandomKey' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'CreateRandomKey' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CreateRandomKey' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CreateRandomKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CreateRandomKey' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CreateRandomKey' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'CreateRandomKey' instead.")]
#endif
		public static SecStatusCode GenerateKeyPair (NSDictionary parameters, out SecKey? publicKey, out SecKey? privateKey)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			IntPtr pub, priv;

			SecStatusCode res;
			unsafe {
				res = SecKeyGeneratePair (parameters.Handle, &pub, &priv);
			}
			if (res == SecStatusCode.Success) {
				publicKey = new SecKey (pub, true);
				privateKey = new SecKey (priv, true);
			} else
				publicKey = privateKey = null;
			return res;
		}

		[Advice ("On iOS this method applies the attributes to both public and private key. To apply different attributes to each key, use 'GenerateKeyPair (SecKeyType, int, SecPublicPrivateKeyAttrs, SecPublicPrivateKeyAttrs, out SecKey, out SecKey)' instead.")]
		public static SecStatusCode GenerateKeyPair (SecKeyType type, int keySizeInBits, SecPublicPrivateKeyAttrs publicAndPrivateKeyAttrs, out SecKey? publicKey, out SecKey? privateKey)
		{
#if !MONOMAC
			// iOS (+friends) need to pass the strong dictionary for public and private key attributes to specific keys
			// instead of merging them with other attributes.
			return GenerateKeyPair (type, keySizeInBits, publicAndPrivateKeyAttrs, publicAndPrivateKeyAttrs, out publicKey, out privateKey);
#else
			if (type == SecKeyType.Invalid)
				throw new ArgumentException ("invalid 'SecKeyType'", nameof (type));

			NSMutableDictionary dic;
			if (publicAndPrivateKeyAttrs is not null)
				dic = new NSMutableDictionary (publicAndPrivateKeyAttrs.GetDictionary ()!);
			else
				dic = new NSMutableDictionary ();
			dic.LowlevelSetObject ((NSObject) type.GetConstant ()!, SecAttributeKey.Type);
			dic.LowlevelSetObject (new NSNumber (keySizeInBits), SecKeyGenerationAttributeKeys.KeySizeInBitsKey.Handle);
			return GenerateKeyPair (dic, out publicKey, out privateKey);
#endif
		}
#if !MONOMAC
		public static SecStatusCode GenerateKeyPair (SecKeyType type, int keySizeInBits, SecPublicPrivateKeyAttrs publicKeyAttrs, SecPublicPrivateKeyAttrs privateKeyAttrs, out SecKey? publicKey, out SecKey? privateKey)
		{
			if (type == SecKeyType.Invalid)
				throw new ArgumentException ("invalid 'SecKeyType'", nameof (type));

			using (var dic = new NSMutableDictionary ()) {
				dic.LowlevelSetObject ((NSObject) type.GetConstant ()!, SecAttributeKey.Type);
				using (var ksib = new NSNumber (keySizeInBits)) {
					dic.LowlevelSetObject (ksib, SecKeyGenerationAttributeKeys.KeySizeInBitsKey.Handle);
					if (publicKeyAttrs is not null)
						dic.LowlevelSetObject (publicKeyAttrs.GetDictionary (), SecKeyGenerationAttributeKeys.PublicKeyAttrsKey.Handle);
					if (privateKeyAttrs is not null)
						dic.LowlevelSetObject (privateKeyAttrs.GetDictionary (), SecKeyGenerationAttributeKeys.PrivateKeyAttrsKey.Handle);
					return GenerateKeyPair (dic, out publicKey, out privateKey);
				}
			}
		}
#endif

		[DllImport (Constants.SecurityLibrary)]
		extern static /* size_t */ nint SecKeyGetBlockSize (IntPtr handle);

		public int BlockSize {
			get {
				return (int) SecKeyGetBlockSize (GetCheckedHandle ());
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'SecKeyCreateSignature' instead.")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'SecKeyCreateSignature' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'SecKeyCreateSignature' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'SecKeyCreateSignature' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'SecKeyCreateSignature' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'SecKeyCreateSignature' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'SecKeyCreateSignature' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static SecStatusCode SecKeyRawSign (IntPtr handle, SecPadding padding, IntPtr dataToSign, nint dataToSignLen, IntPtr sig, nint* sigLen);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'CreateSignature' instead.")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'CreateSignature' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'CreateSignature' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CreateSignature' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CreateSignature' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CreateSignature' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'CreateSignature' instead.")]
#endif
		public SecStatusCode RawSign (SecPadding padding, IntPtr dataToSign, int dataToSignLen, out byte [] result)
		{
			if (dataToSign == IntPtr.Zero)
				throw new ArgumentException (nameof (dataToSign));

			return _RawSign (padding, dataToSign, dataToSignLen, out result);
		}

		public unsafe SecStatusCode RawSign (SecPadding padding, byte [] dataToSign, out byte [] result)
		{
			if (dataToSign is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dataToSign));

			fixed (byte* bp = dataToSign)
				return _RawSign (padding, (IntPtr) bp, dataToSign.Length, out result);
		}

		unsafe SecStatusCode _RawSign (SecPadding padding, IntPtr dataToSign, int dataToSignLen, out byte [] result)
		{
			SecStatusCode status;
			nint len = 1024;
			result = new byte [len];
			fixed (byte* p = result) {
				status = SecKeyRawSign (GetCheckedHandle (), padding, dataToSign, dataToSignLen, (IntPtr) p, &len);
				Array.Resize (ref result, (int) len);
			}
			return status;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'SecKeyVerifySignature' instead.")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'SecKeyVerifySignature' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'SecKeyVerifySignature' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'SecKeyVerifySignature' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'SecKeyVerifySignature' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'SecKeyVerifySignature' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'SecKeyVerifySignature' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeyRawVerify (IntPtr handle, SecPadding padding, IntPtr signedData, nint signedLen, IntPtr sign, nint signLen);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'VerifySignature' instead.")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'VerifySignature' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'VerifySignature' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'VerifySignature' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'VerifySignature' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'VerifySignature' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'VerifySignature' instead.")]
#endif
		public unsafe SecStatusCode RawVerify (SecPadding padding, IntPtr signedData, int signedDataLen, IntPtr signature, int signatureLen)
		{
			return SecKeyRawVerify (GetCheckedHandle (), padding, signedData, (nint) signedDataLen, signature, (nint) signatureLen);
		}

		public SecStatusCode RawVerify (SecPadding padding, byte [] signedData, byte [] signature)
		{
			if (signature is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (signature));
			if (signedData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (signedData));
			unsafe {
				// SecKeyRawVerify will try to read from the signedData/signature pointers even if
				// the corresponding length is 0, which may crash (happens in Xcode 11 beta 1)
				// so if length is 0, then pass an array with one element.
				var signatureArray = signature.Length == 0 ? new byte [] { 0 } : signature;
				var signedDataArray = signedData.Length == 0 ? new byte [] { 0 } : signedData;
				fixed (byte* sp = signatureArray)
				fixed (byte* dp = signedDataArray) {
					return SecKeyRawVerify (GetCheckedHandle (), padding, (IntPtr) dp, (nint) signedData.Length, (IntPtr) sp, (nint) signature.Length);
				}
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'SecKeyCreateEncryptedData' instead.")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'SecKeyCreateEncryptedData' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'SecKeyCreateEncryptedData' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'SecKeyCreateEncryptedData' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'SecKeyCreateEncryptedData' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'SecKeyCreateEncryptedData' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'SecKeyCreateEncryptedData' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static SecStatusCode SecKeyEncrypt (IntPtr handle, SecPadding padding, IntPtr plainText, nint plainTextLen, IntPtr cipherText, nint* cipherTextLengh);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'CreateEncryptedData' instead.")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'CreateEncryptedData' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'CreateEncryptedData' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CreateEncryptedData' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CreateEncryptedData' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CreateEncryptedData' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'CreateEncryptedData' instead.")]
#endif
		public unsafe SecStatusCode Encrypt (SecPadding padding, IntPtr plainText, nint plainTextLen, IntPtr cipherText, ref nint cipherTextLen)
		{
			return SecKeyEncrypt (GetCheckedHandle (), padding, plainText, plainTextLen, cipherText, (nint*) Unsafe.AsPointer<nint> (ref cipherTextLen));
		}

		public SecStatusCode Encrypt (SecPadding padding, byte [] plainText, byte [] cipherText)
		{
			if (cipherText is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (cipherText));
			if (plainText is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (plainText));
			unsafe {
				fixed (byte* cp = cipherText)
				fixed (byte* pp = plainText) {
					nint len = (nint) cipherText.Length;
					return SecKeyEncrypt (GetCheckedHandle (), padding, (IntPtr) pp, (nint) plainText.Length, (IntPtr) cp, &len);
				}
			}
		}

		public SecStatusCode Encrypt (SecPadding padding, byte [] plainText, out byte [] cipherText)
		{
			cipherText = new byte [BlockSize];
			return Encrypt (padding, plainText, cipherText);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'SecKeyCreateDecryptedData' instead.")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'SecKeyCreateDecryptedData' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'SecKeyCreateDecryptedData' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'SecKeyCreateDecryptedData' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'SecKeyCreateDecryptedData' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'SecKeyCreateDecryptedData' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'SecKeyCreateDecryptedData' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static SecStatusCode SecKeyDecrypt (IntPtr handle, SecPadding padding, IntPtr cipherTextLen, nint cipherLen, IntPtr plainText, nint* plainTextLen);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos15.0", "Use 'CreateDecryptedData' instead.")]
		[ObsoletedOSPlatform ("maccatalyst15.0", "Use 'CreateDecryptedData' instead.")]
		[ObsoletedOSPlatform ("ios15.0", "Use 'CreateDecryptedData' instead.")]
#else
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CreateDecryptedData' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CreateDecryptedData' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CreateDecryptedData' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'CreateDecryptedData' instead.")]
#endif
		public unsafe SecStatusCode Decrypt (SecPadding padding, IntPtr cipherText, nint cipherTextLen, IntPtr plainText, ref nint plainTextLen)
		{
			return SecKeyDecrypt (GetCheckedHandle (), padding, cipherText, cipherTextLen, plainText, (nint*) Unsafe.AsPointer<nint> (ref plainTextLen));
		}

		SecStatusCode _Decrypt (SecPadding padding, byte [] cipherText, ref byte []? plainText)
		{
			if (cipherText is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (cipherText));

			unsafe {
				fixed (byte* cp = cipherText) {
					if (plainText is null)
						plainText = new byte [cipherText.Length];
					nint len = plainText.Length;
					SecStatusCode status;
					fixed (byte* pp = plainText)
						status = SecKeyDecrypt (GetCheckedHandle (), padding, (IntPtr) cp, (nint) cipherText.Length, (IntPtr) pp, &len);
					if (len < plainText.Length)
						Array.Resize<byte> (ref plainText, (int) len);
					return status;
				}
			}
		}

		public SecStatusCode Decrypt (SecPadding padding, byte [] cipherText, out byte []? plainText)
		{
			plainText = null;
			return _Decrypt (padding, cipherText, ref plainText);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern IntPtr /* SecKeyRef _Nullable */ SecKeyCreateRandomKey (IntPtr /* CFDictionaryRef* */ parameters, IntPtr* /* CFErrorRef** */ error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		static public SecKey? CreateRandomKey (NSDictionary parameters, out NSError? error)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			IntPtr err;
			IntPtr key;
			unsafe {
				key = SecKeyCreateRandomKey (parameters.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return key == IntPtr.Zero ? null : new SecKey (key, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		static public SecKey? CreateRandomKey (SecKeyType keyType, int keySizeInBits, NSDictionary? parameters, out NSError? error)
		{
			using (var ks = new NSNumber (keySizeInBits))
			using (var md = parameters is null ? new NSMutableDictionary () : new NSMutableDictionary (parameters)) {
				md.LowlevelSetObject ((NSObject) keyType.GetConstant ()!, SecKeyGenerationAttributeKeys.KeyTypeKey.Handle);
				md.LowlevelSetObject (ks, SecKeyGenerationAttributeKeys.KeySizeInBitsKey.Handle);
				return CreateRandomKey (md, out error);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		static public SecKey? CreateRandomKey (SecKeyGenerationParameters parameters, out NSError? error)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));
			if (parameters.KeyType == SecKeyType.Invalid)
				throw new ArgumentException ("invalid 'SecKeyType'", "SecKeyGeneration.KeyType");

			using (var dictionary = parameters.GetDictionary ()!) {
				return CreateRandomKey (dictionary, out error);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern IntPtr /* SecKeyRef _Nullable */ SecKeyCreateWithData (IntPtr /* CFDataRef* */ keyData, IntPtr /* CFDictionaryRef* */ attributes, IntPtr* /* CFErrorRef** */ error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		static public SecKey? Create (NSData keyData, NSDictionary parameters, out NSError? error)
		{
			if (keyData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (keyData));
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			IntPtr err;
			IntPtr key;
			unsafe {
				key = SecKeyCreateWithData (keyData.Handle, parameters.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return key == IntPtr.Zero ? null : new SecKey (key, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		static public SecKey? Create (NSData keyData, SecKeyType keyType, SecKeyClass keyClass, int keySizeInBits, NSDictionary parameters, out NSError? error)
		{
			using (var ks = new NSNumber (keySizeInBits))
			using (var md = parameters is null ? new NSMutableDictionary () : new NSMutableDictionary (parameters)) {
				md.LowlevelSetObject ((NSObject) keyType.GetConstant ()!, SecKeyGenerationAttributeKeys.KeyTypeKey.Handle);
				md.LowlevelSetObject ((NSObject) keyClass.GetConstant ()!, SecAttributeKey.KeyClass);
				md.LowlevelSetObject (ks, SecKeyGenerationAttributeKeys.KeySizeInBitsKey.Handle);
				return Create (keyData, md, out error);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern IntPtr /* CFDataRef _Nullable */ SecKeyCopyExternalRepresentation (IntPtr /* SecKeyRef* */ key, IntPtr* /* CFErrorRef** */ error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetExternalRepresentation (out NSError? error)
		{
			IntPtr data;
			IntPtr err;
			unsafe {
				data = SecKeyCopyExternalRepresentation (Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetExternalRepresentation ()
		{
			IntPtr data;
			IntPtr err;
			unsafe {
				data = SecKeyCopyExternalRepresentation (Handle, &err);
			}
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern IntPtr /* CFDictionaryRef _Nullable */ SecKeyCopyAttributes (IntPtr /* SecKeyRef* */ key);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSDictionary? GetAttributes ()
		{
			var dict = SecKeyCopyAttributes (Handle);
			return Runtime.GetNSObject<NSDictionary> (dict, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern IntPtr /* SecKeyRef* */ SecKeyCopyPublicKey (IntPtr /* SecKeyRef* */ key);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public SecKey? GetPublicKey ()
		{
			var key = SecKeyCopyPublicKey (Handle);
			return key == IntPtr.Zero ? null : new SecKey (key, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern byte /* Boolean */ SecKeyIsAlgorithmSupported (IntPtr /* SecKeyRef* */ key, /* SecKeyOperationType */ nint operation, IntPtr /* SecKeyAlgorithm* */ algorithm);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public bool IsAlgorithmSupported (SecKeyOperationType operation, SecKeyAlgorithm algorithm)
		{
			return SecKeyIsAlgorithmSupported (Handle, (int) operation, algorithm.GetConstant ().GetHandle ()) != 0;
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* CFDataRef _Nullable */ IntPtr SecKeyCreateSignature (/* SecKeyRef */ IntPtr key, /* SecKeyAlgorithm */ IntPtr algorithm, /* CFDataRef */ IntPtr dataToSign, /* CFErrorRef* */ IntPtr* error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? CreateSignature (SecKeyAlgorithm algorithm, NSData dataToSign, out NSError? error)
		{
			if (dataToSign is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dataToSign));

			IntPtr data;
			IntPtr err;
			unsafe {
				data = SecKeyCreateSignature (Handle, algorithm.GetConstant ().GetHandle (), dataToSign.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* Boolean */ byte SecKeyVerifySignature (/* SecKeyRef */ IntPtr key, /* SecKeyAlgorithm */ IntPtr algorithm, /* CFDataRef */ IntPtr signedData, /* CFDataRef */ IntPtr signature, /* CFErrorRef* */ IntPtr* error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public bool VerifySignature (SecKeyAlgorithm algorithm, NSData signedData, NSData signature, out NSError? error)
		{
			if (signedData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (signedData));
			if (signature is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (signature));

			bool result;
			IntPtr err;
			unsafe {
				result = SecKeyVerifySignature (Handle, algorithm.GetConstant ().GetHandle (), signedData.Handle, signature.Handle, &err) != 0;
			}
			error = Runtime.GetNSObject<NSError> (err);
			return result;
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* CFDataRef _Nullable */ IntPtr SecKeyCreateEncryptedData (/* SecKeyRef */ IntPtr key, /* SecKeyAlgorithm */ IntPtr algorithm, /* CFDataRef */ IntPtr plaintext, /* CFErrorRef* */ IntPtr* error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? CreateEncryptedData (SecKeyAlgorithm algorithm, NSData plaintext, out NSError? error)
		{
			if (plaintext is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (plaintext));

			IntPtr data;
			IntPtr err;
			unsafe {
				data = SecKeyCreateEncryptedData (Handle, algorithm.GetConstant ().GetHandle (), plaintext.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* CFDataRef _Nullable */ IntPtr SecKeyCreateDecryptedData (/* SecKeyRef */ IntPtr key, /* SecKeyAlgorithm */ IntPtr algorithm, /* CFDataRef */ IntPtr ciphertext, /* CFErrorRef* */ IntPtr* error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? CreateDecryptedData (SecKeyAlgorithm algorithm, NSData ciphertext, out NSError? error)
		{
			if (ciphertext is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ciphertext));

			IntPtr data;
			IntPtr err;
			unsafe {
				data = SecKeyCreateDecryptedData (Handle, algorithm.GetConstant ().GetHandle (), ciphertext.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern /* CFDataRef _Nullable */ IntPtr SecKeyCopyKeyExchangeResult (/* SecKeyRef */ IntPtr privateKey, /* SecKeyAlgorithm */ IntPtr algorithm, /* SecKeyRef */ IntPtr publicKey, /* CFDictionaryRef */ IntPtr parameters, /* CFErrorRef* */ IntPtr* error);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetKeyExchangeResult (SecKeyAlgorithm algorithm, SecKey publicKey, NSDictionary parameters, out NSError? error)
		{
			if (publicKey is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (publicKey));
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			IntPtr data;
			IntPtr err;
			unsafe {
				data = SecKeyCopyKeyExchangeResult (Handle, algorithm.GetConstant ().GetHandle (), publicKey.Handle, parameters.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return Runtime.GetNSObject<NSData> (data, true);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSData? GetKeyExchangeResult (SecKeyAlgorithm algorithm, SecKey publicKey, SecKeyKeyExchangeParameter parameters, out NSError? error)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			return GetKeyExchangeResult (algorithm, publicKey, parameters.Dictionary!, out error);
		}

#endif
	}
}
