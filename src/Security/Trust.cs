// 
// Trust.cs: Implements the managed SecTrust wrapper.
//
// Authors: 
//	Miguel de Icaza
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2010 Novell, Inc
// Copyright 2012-2014 Xamarin Inc.
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

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using ObjCRuntime;
using CoreFoundation;
using Foundation;
#if NET
using System.Runtime.Versioning;
#else
using NativeHandle = System.IntPtr;
#endif

namespace Security {
	public partial class SecTrust : NativeObject {
#if !NET
		public SecTrust (NativeHandle handle) 
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional=true)]
		internal SecTrust (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD

		[DllImport (Constants.SecurityLibrary, EntryPoint="SecTrustGetTypeID")]
		public extern static nint GetTypeID ();

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecTrustCreateWithCertificates (
			/* CFTypeRef */            IntPtr certOrCertArray,
			/* CFTypeRef __nullable */ IntPtr policies,
			/* SecTrustRef *__nonull */ out IntPtr sectrustref);
		

		public SecTrust (X509Certificate certificate, SecPolicy? policy)
		{
			if (certificate is null)
				throw new ArgumentNullException (nameof (certificate));

			using (SecCertificate cert = new SecCertificate (certificate)) {
				Initialize (cert.Handle, policy);
			}
		}

		public SecTrust (X509Certificate2 certificate, SecPolicy? policy)
		{
			if (certificate is null)
				throw new ArgumentNullException (nameof (certificate));

			using (SecCertificate cert = new SecCertificate (certificate)) {
				Initialize (cert.Handle, policy);
			}
		}

		public SecTrust (X509CertificateCollection certificates, SecPolicy? policy)
		{
			if (certificates is null)
				throw new ArgumentNullException (nameof (certificates));

			SecCertificate[] array = new SecCertificate [certificates.Count];
			int i = 0;
			foreach (var certificate in certificates)
				array [i++] = new SecCertificate (certificate);
			Initialize (array, policy);
		}

		public SecTrust (X509Certificate2Collection certificates, SecPolicy? policy)
		{
			if (certificates is null)
				throw new ArgumentNullException (nameof (certificates));

			SecCertificate[] array = new SecCertificate [certificates.Count];
			int i = 0;
			foreach (var certificate in certificates)
				array [i++] = new SecCertificate (certificate);
			Initialize (array, policy);
		}

		void Initialize (SecCertificate[] array, SecPolicy? policy)
		{
			using (var certs = CFArray.FromNativeObjects (array)) {
				Initialize (certs.Handle, policy);
			}
		}

		void Initialize (IntPtr certHandle, SecPolicy? policy)
		{
			SecStatusCode result = SecTrustCreateWithCertificates (certHandle, policy.GetHandle (), out var handle);
			if (result != SecStatusCode.Success)
				throw new ArgumentException (result.ToString ());
			InitializeHandle (handle);
		}

		[Deprecated (PlatformName.iOS, 12,1, message: "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 12,1, message: "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5,1, message: "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,14,1, message: "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustEvaluate (IntPtr /* SecTrustRef */ trust, out /* SecTrustResultType */ SecTrustResult result);

		[Deprecated (PlatformName.iOS, 12, 1, message : "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 1, message : "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 1, message : "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, 1, message : "Use 'SecTrust.Evaluate (out NSError)' instead.")]
		public SecTrustResult Evaluate ()
		{
			SecTrustResult trust;
			SecStatusCode result = SecTrustEvaluate (GetCheckedHandle (), out trust);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
			return trust;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static nint /* CFIndex */ SecTrustGetCertificateCount (IntPtr /* SecTrustRef */ trust);

		public int Count {
			get {
				if (Handle == IntPtr.Zero)
					return 0;
				return (int) SecTrustGetCertificateCount (Handle);
			}
		}

		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* SecCertificateRef */ SecTrustGetCertificateAtIndex (IntPtr /* SecTrustRef */ trust, nint /* CFIndex */ ix);


		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the 'GetCertificateChain' method instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the 'GetCertificateChain' method instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the 'GetCertificateChain' method instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use the 'GetCertificateChain' method instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the 'GetCertificateChain' method instead.")]
		public SecCertificate this [nint index] {
			get {
				if ((index < 0) || (index >= Count))
					throw new ArgumentOutOfRangeException (nameof (index));

				return new SecCertificate (SecTrustGetCertificateAtIndex (GetCheckedHandle (), index), false);
			}
		}

		[Watch (8,0), TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern /* CFArrayRef */ IntPtr SecTrustCopyCertificateChain (/* SecTrustRef */ IntPtr trust);

		[Watch (8,0), TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		public SecCertificate[] GetCertificateChain ()
			=> NSArray.ArrayFromHandle<SecCertificate> (SecTrustCopyCertificateChain (Handle));

		[Deprecated (PlatformName.iOS, 14,0)]
		[Deprecated (PlatformName.MacOSX, 11,0)]
		[Deprecated (PlatformName.TvOS, 14,0)]
		[Deprecated (PlatformName.WatchOS, 7,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* SecKeyRef */ SecTrustCopyPublicKey (IntPtr /* SecTrustRef */ trust);

		[Deprecated (PlatformName.iOS, 14,0, message: "Use 'GetKey' instead.")]
		[Deprecated (PlatformName.MacOSX, 11,0, message: "Use 'GetKey' instead.")]
		[Deprecated (PlatformName.TvOS, 14,0, message: "Use 'GetKey' instead.")]
		[Deprecated (PlatformName.WatchOS, 7,0, message: "Use 'GetKey' instead.")]
		public SecKey GetPublicKey ()
		{
			return new SecKey (SecTrustCopyPublicKey (GetCheckedHandle ()), true);
		}

		[iOS (14,0)]
		[TV (14,0)]
		[Watch (7,0)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* SecKeyRef */ SecTrustCopyKey (IntPtr /* SecTrustRef */ trust);

		[iOS (14,0)]
		[TV (14,0)]
		[Watch (7,0)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
		public SecKey GetKey ()
		{
			return new SecKey (SecTrustCopyKey (GetCheckedHandle ()), true);
		}

		[Mac (10,9)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* CFDataRef */ SecTrustCopyExceptions (IntPtr /* SecTrustRef */ trust);

		[Mac (10,9)]
		public NSData? GetExceptions ()
		{
			return Runtime.GetNSObject<NSData> (SecTrustCopyExceptions (GetCheckedHandle ()), true);
		}

		[Mac (10,9)]
		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool SecTrustSetExceptions (IntPtr /* SecTrustRef */ trust, IntPtr /* __nullable CFDataRef */ exceptions);

		[Mac (10,9)]
		public bool SetExceptions (NSData data)
		{
			return SecTrustSetExceptions (GetCheckedHandle (), data.GetHandle ());
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static double /* CFAbsoluteTime */ SecTrustGetVerifyTime (IntPtr /* SecTrustRef */ trust);

		public double GetVerifyTime ()
		{
			return SecTrustGetVerifyTime (GetCheckedHandle ());
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetVerifyDate (IntPtr /* SecTrustRef */ trust, IntPtr /* CFDateRef */ verifyDate);

		public SecStatusCode SetVerifyDate (DateTime date)
		{
			// CFDateRef amd NSDate are toll-freee bridged
			using (NSDate d = (NSDate) date) {
				return SecTrustSetVerifyDate (GetCheckedHandle (), d.Handle);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetAnchorCertificates (IntPtr /* SecTrustRef */ trust, IntPtr /* CFArrayRef */ anchorCertificates);

		public SecStatusCode SetAnchorCertificates (X509CertificateCollection certificates)
		{
			if (certificates is null)
				return SecTrustSetAnchorCertificates (GetCheckedHandle (), IntPtr.Zero);

			SecCertificate[] array = new SecCertificate [certificates.Count];
			int i = 0;
			foreach (var certificate in certificates)
				array [i++] = new SecCertificate (certificate);
			return SetAnchorCertificates (array);
		}

		public SecStatusCode SetAnchorCertificates (X509Certificate2Collection certificates)
		{
			if (certificates is null)
				return SecTrustSetAnchorCertificates (GetCheckedHandle (), IntPtr.Zero);

			SecCertificate[] array = new SecCertificate [certificates.Count];
			int i = 0;
			foreach (var certificate in certificates)
				array [i++] = new SecCertificate (certificate);
			return SetAnchorCertificates (array);
		}

		public SecStatusCode SetAnchorCertificates (SecCertificate[] array)
		{
			if (array is null)
				return SecTrustSetAnchorCertificates (Handle, IntPtr.Zero);
			using (var certs = CFArray.FromNativeObjects (array)) {
				return SecTrustSetAnchorCertificates (Handle, certs.Handle);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetAnchorCertificatesOnly (IntPtr /* SecTrustRef */ trust, [MarshalAs (UnmanagedType.I1)] bool anchorCertificatesOnly);

		public SecStatusCode SetAnchorCertificatesOnly (bool anchorCertificatesOnly)
		{
			return SecTrustSetAnchorCertificatesOnly (GetCheckedHandle (), anchorCertificatesOnly);
		}
#endif
	}
}
