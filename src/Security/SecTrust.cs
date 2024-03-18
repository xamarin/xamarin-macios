// 
// SecTrust.cs: Implements the managed SecTrust wrapper.
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//

#nullable enable

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

	public delegate void SecTrustCallback (SecTrust? trust, SecTrustResult trustResult);
	public delegate void SecTrustWithErrorCallback (SecTrust? trust, bool result, NSError? /* CFErrorRef _Nullable */ error);

	public partial class SecTrust {

		public SecTrust (SecCertificate certificate, SecPolicy policy)
		{
			if (certificate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificate));

			Initialize (certificate.Handle, policy);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustCopyPolicies (IntPtr /* SecTrustRef */ trust, ref IntPtr /* CFArrayRef* */ policies);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecPolicy [] GetPolicies ()
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
			if (policy is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (policy));

			SetPolicies (policy.Handle);
		}

		public void SetPolicies (IEnumerable<SecPolicy> policies)
		{
			if (policies is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (policies));

			using (var array = NSArray.FromNSObjects (policies.ToArray ()))
				SetPolicies (array.Handle);
		}

		public void SetPolicies (NSArray policies)
		{
			if (policies is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (policies));

			SetPolicies (policies.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustGetNetworkFetchAllowed (IntPtr /* SecTrustRef */ trust, [MarshalAs (UnmanagedType.I1)] out bool /* Boolean* */ allowFetch);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetNetworkFetchAllowed (IntPtr /* SecTrustRef */ trust, [MarshalAs (UnmanagedType.I1)] bool /* Boolean */ allowFetch);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
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
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustCopyCustomAnchorCertificates (IntPtr /* SecTrustRef */ trust, out IntPtr /* CFArrayRef* */ anchors);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecCertificate [] GetCustomAnchorCertificates ()
		{
			IntPtr p;
			SecStatusCode result = SecTrustCopyCustomAnchorCertificates (Handle, out p);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
			return NSArray.ArrayFromHandle<SecCertificate> (p);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		unsafe extern static SecStatusCode /* OSStatus */ SecTrustEvaluateAsync (IntPtr /* SecTrustRef */ trust, IntPtr /* dispatch_queue_t */ queue, BlockLiteral* block);

#if !NET
		internal delegate void TrustEvaluateHandler (IntPtr block, IntPtr trust, SecTrustResult trustResult);
		static readonly TrustEvaluateHandler evaluate = TrampolineEvaluate;

		[MonoPInvokeCallback (typeof (TrustEvaluateHandler))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineEvaluate (IntPtr block, IntPtr trust, SecTrustResult trustResult)
		{
			var del = BlockLiteral.GetTarget<SecTrustCallback> (block);
			if (del is not null) {
				var t = trust == IntPtr.Zero ? null : new SecTrust (trust, false);
				del (t, trustResult);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Evaluate (DispatchQueue, SecTrustWithErrorCallback)' instead.")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public SecStatusCode Evaluate (DispatchQueue queue, SecTrustCallback handler)
		{
			// headers have `dispatch_queue_t _Nullable queue` but it crashes... don't trust headers, even for SecTrust
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, SecTrustResult, void> trampoline = &TrampolineEvaluate;
				using var block = new BlockLiteral (trampoline, handler, typeof (SecTrust), nameof (TrampolineEvaluate));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (evaluate, handler);
#endif
				return SecTrustEvaluateAsync (Handle, queue.Handle, &block);
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
		unsafe static extern SecStatusCode SecTrustEvaluateAsyncWithError (IntPtr /* SecTrustRef */ trust, IntPtr /* dispatch_queue_t */ queue, BlockLiteral* block);

#if !NET
		internal delegate void TrustEvaluateErrorHandler (IntPtr block, IntPtr trust, byte result, IntPtr /* CFErrorRef _Nullable */  error);
		static readonly TrustEvaluateErrorHandler evaluate_error = TrampolineEvaluateError;

		[MonoPInvokeCallback (typeof (TrustEvaluateErrorHandler))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineEvaluateError (IntPtr block, IntPtr trust, byte result, IntPtr /* CFErrorRef _Nullable */  error)
		{
			var del = BlockLiteral.GetTarget<SecTrustWithErrorCallback> (block);
			if (del is not null) {
				var t = trust == IntPtr.Zero ? null : new SecTrust (trust, false);
				var e = error == IntPtr.Zero ? null : new NSError (error);
				del (t, result != 0, e);
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
		[BindingImpl (BindingImplOptions.Optimizable)]
		public SecStatusCode Evaluate (DispatchQueue queue, SecTrustWithErrorCallback handler)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, byte, IntPtr, void> trampoline = &TrampolineEvaluateError;
				using var block = new BlockLiteral (trampoline, handler, typeof (SecTrust), nameof (TrampolineEvaluateError));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (evaluate_error, handler);
#endif
				return SecTrustEvaluateAsyncWithError (Handle, queue.Handle, &block);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustGetTrustResult (IntPtr /* SecTrustRef */ trust, out SecTrustResult /* SecTrustResultType */ result);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
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
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
		[TV (12, 0)]
		[iOS (12, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		static extern bool SecTrustEvaluateWithError (/* SecTrustRef */ IntPtr trust, out /* CFErrorRef** */ IntPtr error);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
		[TV (12, 0)]
		[iOS (12, 0)]
#endif
		public bool Evaluate (out NSError? error)
		{
			var result = SecTrustEvaluateWithError (Handle, out var err);
			error = err == IntPtr.Zero ? null : new NSError (err);
			return result;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* CFDictionaryRef */ SecTrustCopyResult (IntPtr /* SecTrustRef */ trust);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSDictionary GetResult ()
		{
			return new NSDictionary (SecTrustCopyResult (Handle), true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode /* OSStatus */ SecTrustSetOCSPResponse (IntPtr /* SecTrustRef */ trust, IntPtr /* CFTypeRef */ responseData);

		// the API accept the handle for a single policy or an array of them
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		void SetOCSPResponse (IntPtr ocsp)
		{
			SecStatusCode result = SecTrustSetOCSPResponse (Handle, ocsp);
			if (result != SecStatusCode.Success)
				throw new InvalidOperationException (result.ToString ());
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public void SetOCSPResponse (NSData ocspResponse)
		{
			if (ocspResponse is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ocspResponse));

			SetOCSPResponse (ocspResponse.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public void SetOCSPResponse (IEnumerable<NSData> ocspResponses)
		{
			if (ocspResponses is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ocspResponses));

			using (var array = NSArray.FromNSObjects (ocspResponses.ToArray ()))
				SetOCSPResponse (array.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public void SetOCSPResponse (NSArray ocspResponses)
		{
			if (ocspResponses is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ocspResponses));

			SetOCSPResponse (ocspResponses.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios12.1.1")]
		[SupportedOSPlatform ("tvos12.1.1")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 1, 1)]
		[Watch (5, 1, 1)]
		[TV (12, 1, 1)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern SecStatusCode /* OSStatus */ SecTrustSetSignedCertificateTimestamps (/* SecTrustRef* */ IntPtr trust, /* CFArrayRef* */ IntPtr sctArray);

#if NET
		[SupportedOSPlatform ("ios12.1.1")]
		[SupportedOSPlatform ("tvos12.1.1")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 1, 1)]
		[Watch (5, 1, 1)]
		[TV (12, 1, 1)]
#endif
		public SecStatusCode SetSignedCertificateTimestamps (IEnumerable<NSData> sct)
		{
			if (sct is null)
				return SecTrustSetSignedCertificateTimestamps (Handle, IntPtr.Zero);

			using (var array = NSArray.FromNSObjects (sct.ToArray ()))
				return SecTrustSetSignedCertificateTimestamps (Handle, array.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios12.1.1")]
		[SupportedOSPlatform ("tvos12.1.1")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 1, 1)]
		[Watch (5, 1, 1)]
		[TV (12, 1, 1)]
#endif
		public SecStatusCode SetSignedCertificateTimestamps (NSArray<NSData> sct)
		{
			return SecTrustSetSignedCertificateTimestamps (Handle, sct.GetHandle ());
		}
	}
}
