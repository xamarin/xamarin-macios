// 
// SecTrust.cs: Implements the managed SecTrust wrapper.
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace Security {

	public delegate void SecTrustCallback (SecTrust trust, SecTrustResult trustResult);
	public delegate void SecTrustWithErrorCallback (SecTrust trust, bool result, NSError /* CFErrorRef _Nullable */ error);

	public partial class SecTrust {

		public SecTrust (SecCertificate certificate, SecPolicy policy)
		{
			if (certificate == null)
				throw new ArgumentNullException ("certificate");

			Initialize (certificate.Handle, policy);
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustCopyPolicies (IntPtr /* SecTrustRef */ trust, ref IntPtr /* CFArrayRef* */ policies);

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public SecPolicy[] GetPolicies ()
		{
			IntPtr p = IntPtr.Zero;
			SecStatusCode result = SecTrustCopyPolicies (Handle, ref p);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
			return NSArray.ArrayFromHandle<SecPolicy> (p);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetPolicies (IntPtr /* SecTrustRef */ trust, IntPtr /* CFTypeRef */ policies);

		// the API accept the handle for a single policy or an array of them
		void SetPolicies (IntPtr policy)
		{
			SecStatusCode result = SecTrustSetPolicies (Handle, policy);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
		}

		public void SetPolicy (SecPolicy policy)
		{
			if (policy == null)
				throw new ArgumentNullException ("policy");

			SetPolicies (policy.Handle);
		}

		public void SetPolicies (IEnumerable<SecPolicy> policies)
		{
			if (policies == null)
				throw new ArgumentNullException ("policies");

			using (var array = NSArray.FromNSObjects (policies.ToArray ()))
				SetPolicies (array.Handle);
		}

		public void SetPolicies (NSArray policies)
		{
			if (policies == null)
				throw new ArgumentNullException ("policies");

			SetPolicies (policies.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.9")]
#else
		[iOS (7,0)]
		[Mac (10,9)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustGetNetworkFetchAllowed (IntPtr /* SecTrustRef */ trust, [MarshalAs (UnmanagedType.I1)] out bool /* Boolean* */ allowFetch);

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.9")]
#else
		[iOS (7,0)]
		[Mac (10,9)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetNetworkFetchAllowed (IntPtr /* SecTrustRef */ trust, [MarshalAs (UnmanagedType.I1)] bool /* Boolean */ allowFetch);

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.9")]
#else
		[iOS (7,0)]
		[Mac (10,9)]
#endif
		public bool NetworkFetchAllowed {
			get {
				bool value;
				SecStatusCode result = SecTrustGetNetworkFetchAllowed (Handle, out value);
				if (result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
				return value;
			}
			set {
				SecStatusCode result = SecTrustSetNetworkFetchAllowed (Handle, value);
				if (result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
			}
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustCopyCustomAnchorCertificates (IntPtr /* SecTrustRef */ trust, out IntPtr /* CFArrayRef* */ anchors);

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public SecCertificate[] GetCustomAnchorCertificates  ()
		{
			IntPtr p;
			SecStatusCode result = SecTrustCopyCustomAnchorCertificates (Handle, out p);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
			return NSArray.ArrayFromHandle<SecCertificate> (p);
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (7,0)]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustEvaluateAsync (IntPtr /* SecTrustRef */ trust, IntPtr /* dispatch_queue_t */ queue, ref BlockLiteral block);

		internal delegate void TrustEvaluateHandler (IntPtr block, IntPtr trust, SecTrustResult trustResult);
		static readonly TrustEvaluateHandler evaluate = TrampolineEvaluate;

		[MonoPInvokeCallback (typeof (TrustEvaluateHandler))]
		static void TrampolineEvaluate (IntPtr block, IntPtr trust, SecTrustResult trustResult)
		{
			var del = BlockLiteral.GetTarget<SecTrustCallback> (block);
			if (del != null) {
				var t = trust == IntPtr.Zero ? null : new SecTrust (trust, false);
				del (t, trustResult);
			}
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (7,0)]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public SecStatusCode Evaluate (DispatchQueue queue, SecTrustCallback handler)
		{
			// headers have `dispatch_queue_t _Nullable queue` but it crashes... don't trust headers, even for SecTrust
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			if (handler == null)
				throw new ArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			try {
				block_handler.SetupBlockUnsafe (evaluate, handler);
				return SecTrustEvaluateAsync (Handle, queue.Handle, ref block_handler);
			}
			finally {
				block_handler.CleanupBlock ();
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern SecStatusCode SecTrustEvaluateAsyncWithError (IntPtr /* SecTrustRef */ trust, IntPtr /* dispatch_queue_t */ queue, ref BlockLiteral block);

		internal delegate void TrustEvaluateErrorHandler (IntPtr block, IntPtr trust, bool result, IntPtr /* CFErrorRef _Nullable */  error);
		static readonly TrustEvaluateErrorHandler evaluate_error = TrampolineEvaluateError;

		[MonoPInvokeCallback (typeof (TrustEvaluateErrorHandler))]
		static void TrampolineEvaluateError (IntPtr block, IntPtr trust, bool result, IntPtr /* CFErrorRef _Nullable */  error)
		{
			var del = BlockLiteral.GetTarget<SecTrustWithErrorCallback> (block);
			if (del != null) {
				var t = trust == IntPtr.Zero ? null : new SecTrust (trust, false);
				var e = error == IntPtr.Zero ? null : new NSError (error);
				del (t, result, e);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public SecStatusCode Evaluate (DispatchQueue queue, SecTrustWithErrorCallback handler)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			if (handler == null)
				throw new ArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			try {
				block_handler.SetupBlockUnsafe (evaluate_error, handler);
				return SecTrustEvaluateAsyncWithError (Handle, queue.Handle, ref block_handler);
			}
			finally {
				block_handler.CleanupBlock ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustGetTrustResult (IntPtr /* SecTrustRef */ trust, out SecTrustResult /* SecTrustResultType */ result);

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public SecTrustResult GetTrustResult ()
		{
			SecTrustResult trust_result;
			SecStatusCode result = SecTrustGetTrustResult (Handle, out trust_result);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
			return trust_result;
		}

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
#else
		[Watch (5,0)]
		[TV (12,0)]
		[Mac (10,14)]
		[iOS (12,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		static extern bool SecTrustEvaluateWithError (/* SecTrustRef */ IntPtr trust, out /* CFErrorRef** */ IntPtr error);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
#else
		[Watch (5,0)]
		[TV (12,0)]
		[Mac (10,14)]
		[iOS (12,0)]
#endif
		public bool Evaluate (out NSError error)
		{
			var result = SecTrustEvaluateWithError (Handle, out var err);
			error = err == IntPtr.Zero ? null : new NSError (err);
			return result;
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.9")]
#else
		[iOS (7,0)]
		[Mac (10,9)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* CFDictionaryRef */ SecTrustCopyResult (IntPtr /* SecTrustRef */ trust);

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.9")]
#else
		[iOS (7,0)]
		[Mac (10,9)]
#endif
		public NSDictionary GetResult ()
		{
			return new NSDictionary (SecTrustCopyResult (Handle), true);
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("macos10.9")]
#else
		[iOS (7,0)]
		[Mac (10,9)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetOCSPResponse (IntPtr /* SecTrustRef */ trust, IntPtr /* CFTypeRef */ responseData);

		// the API accept the handle for a single policy or an array of them
#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		void SetOCSPResponse (IntPtr ocsp)
		{
			SecStatusCode result = SecTrustSetOCSPResponse (Handle, ocsp);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public void SetOCSPResponse (NSData ocspResponse)
		{
			if (ocspResponse == null)
				throw new ArgumentNullException ("ocspResponse");

			SetOCSPResponse (ocspResponse.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public void SetOCSPResponse (IEnumerable<NSData> ocspResponses)
		{
			if (ocspResponses == null)
				throw new ArgumentNullException ("ocspResponses");

			using (var array = NSArray.FromNSObjects (ocspResponses.ToArray ()))
				SetOCSPResponse (array.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public void SetOCSPResponse (NSArray ocspResponses)
		{
			if (ocspResponses == null)
				throw new ArgumentNullException ("ocspResponses");

			SetOCSPResponse (ocspResponses.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios12.1.1")]
		[SupportedOSPlatform ("tvos12.1.1")]
		[SupportedOSPlatform ("macos10.14.2")]
#else
		[iOS (12,1,1)]
		[Watch (5,1,1)]
		[TV (12,1,1)]
		[Mac (10,14,2)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern SecStatusCode /* OSStatus */ SecTrustSetSignedCertificateTimestamps (/* SecTrustRef* */ IntPtr trust, /* CFArrayRef* */ IntPtr sctArray);

#if NET
		[SupportedOSPlatform ("ios12.1.1")]
		[SupportedOSPlatform ("tvos12.1.1")]
		[SupportedOSPlatform ("macos10.14.2")]
#else
		[iOS (12,1,1)]
		[Watch (5,1,1)]
		[TV (12,1,1)]
		[Mac (10,14,2)]
#endif
		public SecStatusCode SetSignedCertificateTimestamps (IEnumerable<NSData> sct)
		{
			if (sct == null)
				return SecTrustSetSignedCertificateTimestamps (Handle, IntPtr.Zero);

			using (var array = NSArray.FromNSObjects (sct.ToArray ()))
				return SecTrustSetSignedCertificateTimestamps (Handle, array.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios12.1.1")]
		[SupportedOSPlatform ("tvos12.1.1")]
		[SupportedOSPlatform ("macos10.14.2")]
#else
		[iOS (12,1,1)]
		[Watch (5,1,1)]
		[TV (12,1,1)]
		[Mac (10,14,2)]
#endif
		public SecStatusCode SetSignedCertificateTimestamps (NSArray<NSData> sct)
		{
			return SecTrustSetSignedCertificateTimestamps (Handle, sct.GetHandle ());
		}
	}
}
