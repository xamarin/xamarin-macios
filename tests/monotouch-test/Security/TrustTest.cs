//
// SecTrust Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
// Coyright 2019 Microsoft Corporation
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreFoundation;
using Security;
using ObjCRuntime;

using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class TrustTest {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		// SecTrustResult.Unspecified == "Use System Defaults" (valid)

		// some days it seems iOS timeout just a bit too fast and we get a lot of false positives
		// this will try again a few times (giving the network request a chance to get back)
		static SecTrustResult Evaluate (SecTrust trust, bool expect_recoverable = false)
		{
			return Evaluate (trust, out var _, expect_recoverable);
		}

		static SecTrustResult Evaluate (SecTrust trust, out NSError? error, bool expect_recoverable = false)
		{
			error = null;
			SecTrustResult result = SecTrustResult.Deny;
			for (int i = 0; i < 8; i++) {
				trust.Evaluate (out error);
				result = trust.GetTrustResult ();
				if (result != SecTrustResult.RecoverableTrustFailure || expect_recoverable)
					return result;
				NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (i));
			}
			// we have done our best (it has not failed, but did not confirm either)
			// still it can't recover (we and expected it could) most likely due to external factors (e.g. network)
			if (result == SecTrustResult.RecoverableTrustFailure && !expect_recoverable)
				Assert.Inconclusive ("Cannot recover from RecoverableTrustFailure after 8 attempts");
			return result;
		}

		[Test]
		public void Trust_Leaf_Only ()
		{
			X509Certificate x = new X509Certificate (CertificateTest.mail_google_com);
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (x, policy)) {
				Trust_Leaf_Only (trust, policy);
			}
		}

		void Trust_Leaf_Only (SecTrust trust, SecPolicy policy)
		{
			Assert.That (CFGetRetainCount (trust.Handle), Is.EqualTo ((nint) 1), "RetainCount(trust)");
			Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 2), "RetainCount(policy)");
			// that certificate stopped being valid on September 30th, 2013 so we validate it with a date earlier than that
			trust.SetVerifyDate (new DateTime (635108745218945450, DateTimeKind.Utc));
			// the system was able to construct the chain based on the single certificate
			var expectedTrust = SecTrustResult.RecoverableTrustFailure;
#if __MACOS__
			if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9))
				expectedTrust = SecTrustResult.Unspecified;
#endif
			Assert.That (Evaluate (trust, true), Is.EqualTo (expectedTrust), "Evaluate");

			using (var queue = new DispatchQueue ("TrustAsync")) {
				bool assert = false; // we don't want to assert in another queue
				bool called = false;
				var err = trust.Evaluate (DispatchQueue.MainQueue, (t, result) => {
					assert = t.Handle == trust.Handle && result == expectedTrust;
					called = true;
				});
				Assert.That (err, Is.EqualTo (SecStatusCode.Success), "async1/err");
				TestRuntime.RunAsync (TimeSpan.FromSeconds (5), () => { }, () => called);
				Assert.True (assert, "async1");
			}

			if (TestRuntime.CheckXcodeVersion (11, 0)) {
				using (var queue = new DispatchQueue ("TrustErrorAsync")) {
					bool assert = false; // we don't want to assert in another queue
					bool called = false;
					var err = trust.Evaluate (DispatchQueue.MainQueue, (t, result, error) => {
						assert = t.Handle == trust.Handle && !result && error != null;
						called = true;
					});
					Assert.That (err, Is.EqualTo (SecStatusCode.Success), "async2/err");
					TestRuntime.RunAsync (TimeSpan.FromSeconds (5), () => { }, () => called);
					Assert.True (assert, "async2");
				}
			}

#if __MACOS__
			var hasNetworkFetchAllowed = TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9);
#else
			var hasNetworkFetchAllowed = TestRuntime.CheckXcodeVersion (5, 0);
#endif
			if (hasNetworkFetchAllowed) {
				Assert.True (trust.NetworkFetchAllowed, "NetworkFetchAllowed-1");
				trust.NetworkFetchAllowed = false;
				Assert.False (trust.NetworkFetchAllowed, "NetworkFetchAllowed-2");

				trust.SetPolicy (policy);

				var policies = trust.GetPolicies ();
				Assert.That (policies.Length, Is.EqualTo (1), "Policies.Length");
				Assert.That (policies [0].Handle, Is.EqualTo (policy.Handle), "Handle");
			}
		}

		[Test]
		public void HostName_Leaf_Only ()
		{
			X509Certificate x = new X509Certificate (CertificateTest.mail_google_com);
			// a bad hostname (mismatched) is recoverable (e.g. if you change policy)
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.xamarin.com"))
			using (var trust = new SecTrust (x, policy)) {
				// that certificate stopped being valid on September 30th, 2013 so we validate it with a date earlier than that
				trust.SetVerifyDate (new DateTime (635108745218945450, DateTimeKind.Utc));
				Assert.That (Evaluate (trust, true), Is.EqualTo (SecTrustResult.RecoverableTrustFailure), "Evaluate");

				if (TestRuntime.CheckXcodeVersion (5, 0)) {
					Assert.That (trust.GetTrustResult (), Is.EqualTo (SecTrustResult.RecoverableTrustFailure), "GetTrustResult");

					using (var a = NSArray.FromNSObjects (policy))
						trust.SetPolicies (a);

					var policies = trust.GetPolicies ();
					Assert.That (policies.Length, Is.EqualTo (1), "Policies.Length");
					Assert.That (policies [0].Handle, Is.EqualTo (policy.Handle), "Handle");

					var trust_result = SecTrustResult.Invalid;
#if __MACOS__
					if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 13)) {
						trust_result = SecTrustResult.RecoverableTrustFailure;
					} else if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 12)) {
						trust_result = SecTrustResult.Invalid;
					} else if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 8)) {
						trust_result = SecTrustResult.RecoverableTrustFailure;
					}
#else
					if (TestRuntime.CheckXcodeVersion (9, 0))
						trust_result = SecTrustResult.RecoverableTrustFailure; // Result not invalidated starting with Xcode 9 beta 3.
#endif

					// since we modified the `trust` instance it's result was invalidated
					Assert.That (trust.GetTrustResult (), Is.EqualTo (trust_result), "GetTrustResult-2");
				}
			}
		}

		[Test]
		public void NoHostName ()
		{
			X509Certificate x = new X509Certificate (CertificateTest.mail_google_com);
			// a null host name means "*" (accept any name) which is not stated in Apple documentation
			using (var policy = SecPolicy.CreateSslPolicy (true, null))
			using (var trust = new SecTrust (x, policy)) {
				// that certificate stopped being valid on September 30th, 2013 so we validate it with a date earlier than that
				trust.SetVerifyDate (new DateTime (635108745218945450, DateTimeKind.Utc));
				var expectedTrust = SecTrustResult.RecoverableTrustFailure;
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9))
					expectedTrust = SecTrustResult.Unspecified;
#endif
				Assert.That (Evaluate (trust, true), Is.EqualTo (expectedTrust), "Evaluate");

#if __MACOS__
				var hasCreateRevocationPolicy = TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9);
#else
				var hasCreateRevocationPolicy = TestRuntime.CheckXcodeVersion (5, 0);
#endif
				if (hasCreateRevocationPolicy) {
					using (var rev = SecPolicy.CreateRevocationPolicy (SecRevocation.UseAnyAvailableMethod)) {
						List<SecPolicy> list = new List<SecPolicy> () { policy, rev };
						trust.SetPolicies (list);

						var policies = trust.GetPolicies ();
						Assert.That (policies.Length, Is.EqualTo (2), "Policies.Length");
					}
				}
			}
		}

		[Test]
		public void Client_Leaf_Only ()
		{
			X509Certificate x = new X509Certificate (CertificateTest.mail_google_com);
			using (var policy = SecPolicy.CreateSslPolicy (false, null))
			using (var trust = new SecTrust (x, policy)) {
				// that certificate stopped being valid on September 30th, 2013 so we validate it with a date earlier than that
				trust.SetVerifyDate (new DateTime (635108745218945450, DateTimeKind.Utc));
				// a host name is not meaningful for client certificates
				var expectedTrust = SecTrustResult.RecoverableTrustFailure;
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9))
					expectedTrust = SecTrustResult.Unspecified;
#endif
				Assert.That (Evaluate (trust, true), Is.EqualTo (expectedTrust), "Evaluate");

#if __MACOS__
				var hasGetResult = TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9);
#else
				var hasGetResult = TestRuntime.CheckXcodeVersion (5, 0);
#endif
				if (hasGetResult) {
					// by default there's no *custom* anchors
					Assert.Null (trust.GetCustomAnchorCertificates (), "GetCustomAnchorCertificates");

					using (var results = trust.GetResult ()) {
						Assert.That (CFGetRetainCount (results.Handle), Is.EqualTo ((nint) 1), "RetainCount");

						SecTrustResult value = (SecTrustResult) (int) (NSNumber) results [SecTrustResultKey.ResultValue];
						Assert.That (value, Is.EqualTo (SecTrustResult.RecoverableTrustFailure), "ResultValue");
					}
				}
			}
		}

		[Test]
		public void Basic_Leaf_Only ()
		{
			X509Certificate x = new X509Certificate (CertificateTest.mail_google_com);
			using (var policy = SecPolicy.CreateBasicX509Policy ())
			using (var trust = new SecTrust (x, policy)) {
				// that certificate stopped being valid on September 30th, 2013 so we validate it with a date earlier than that
				trust.SetVerifyDate (new DateTime (635108745218945450, DateTimeKind.Utc));
				// SSL certs are a superset of the basic X509 profile
				SecTrustResult result = SecTrustResult.RecoverableTrustFailure;
				Assert.That (Evaluate (trust, result == SecTrustResult.RecoverableTrustFailure), Is.EqualTo (result), "Evaluate");

				var hasOCSPResponse = true;
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9))
					hasOCSPResponse = false;
#else
				if (!TestRuntime.CheckXcodeVersion (5, 0))
					hasOCSPResponse = false;
#endif

				if (hasOCSPResponse) {
					// call GetPolicies without a SetPolicy / SetPolicies
					var policies = trust.GetPolicies ();
					Assert.That (policies.Length, Is.EqualTo (1), "Policies.Length");

					using (var data = new NSData ()) {
						// we do not have an easy way to get the response but the API accepts an empty NSData
						trust.SetOCSPResponse (data);
					}
				}
			}
		}

		[Test]
		public void Trust_NoRoot ()
		{
			X509CertificateCollection certs = new X509CertificateCollection ();
			certs.Add (new X509Certificate (CertificateTest.mail_google_com));
			certs.Add (new X509Certificate (CertificateTest.gts_ca_1c3));
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (certs, policy)) {
				Trust_NoRoot (trust, policy);
			}
		}

		void Trust_NoRoot (SecTrust trust, SecPolicy policy)
		{
			// that certificate stopped being valid on September 30th, 2013 so we validate it with a date earlier than that
			trust.SetVerifyDate (new DateTime (635108745218945450, DateTimeKind.Utc));
			// iOS is not fully happy with the basic constraints: `SecTrustEvaluate  [root AnchorTrusted BasicContraints]`
			// so it returns RecoverableTrustFailure and that affects the Count of trust later (it does not add to what we provided)
			var result = Evaluate (trust, out var trustError, true);
			Assert.That (result, Is.EqualTo (SecTrustResult.RecoverableTrustFailure), $"Evaluate: {trustError}");
			// Evalute must be called prior to Count (Apple documentation)
			Assert.That (trust.Count, Is.EqualTo (3), "Count");

			using (SecKey pkey = trust.GetPublicKey ()) {
				Assert.That (CFGetRetainCount (pkey.Handle), Is.GreaterThanOrEqualTo ((nint) 1), "RetainCount(pkey)");
			}
			if (TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch)) {
				using (SecKey key = trust.GetKey ()) {
					Assert.That (key.BlockSize, Is.EqualTo (32), "BlockSize");
					Assert.That (CFGetRetainCount (key.Handle), Is.GreaterThanOrEqualTo ((nint) 1), "RetainCount(key)");
				}
			}
			if (TestRuntime.CheckXcodeVersion (10, 0)) {
				Assert.False (trust.Evaluate (out var error), "Evaluate");
				Assert.NotNull (error, "error");
			}
		}

		[Test]
		public void Trust_FullChain ()
		{
			X509CertificateCollection certs = new X509CertificateCollection ();
			certs.Add (new X509Certificate (CertificateTest.mail_google_com));
			certs.Add (new X509Certificate (CertificateTest.gts_ca_1c3));
			certs.Add (new X509Certificate (CertificateTest.gts_root_r1));
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (certs, policy)) {
				Trust_FullChain (trust, policy, certs);
			}
		}

		void Trust_FullChain (SecTrust trust, SecPolicy policy, X509CertificateCollection certs)
		{
			// that certificate stopped being valid on December 15th, 2023 so we validate it with a date earlier than that
			trust.SetVerifyDate (new DateTime (638382479747632240, DateTimeKind.Utc));

			SecTrustResult trust_result = SecTrustResult.Unspecified;
			var result = Evaluate (trust, out var trustError, true);
			Assert.That (result, Is.EqualTo (trust_result), $"Evaluate: {trustError}");

			// Evalute must be called prior to Count (Apple documentation)
			Assert.That (trust.Count, Is.EqualTo (3), "Count");

			using (SecCertificate sc1 = trust [0]) {
				// seems the leaf gets an extra one
				Assert.That (CFGetRetainCount (sc1.Handle), Is.GreaterThanOrEqualTo ((nint) 2), "RetainCount(sc1)");
				Assert.That (sc1.SubjectSummary, Is.EqualTo ("mail.google.com"), "SubjectSummary(sc1)");
			}
			using (SecCertificate sc2 = trust [1]) {
				Assert.That (CFGetRetainCount (sc2.Handle), Is.GreaterThanOrEqualTo ((nint) 2), "RetainCount(sc2)");
				Assert.That (sc2.SubjectSummary, Is.EqualTo ("GTS CA 1C3"), "SubjectSummary(sc2)");
			}
			using (SecCertificate sc3 = trust [2]) {
				Assert.That (CFGetRetainCount (sc3.Handle), Is.GreaterThanOrEqualTo ((nint) 2), "RetainCount(sc3)");
				Assert.That (sc3.SubjectSummary, Is.EqualTo ("GTS Root R1"), "SubjectSummary(sc3)");
			}

			Assert.That (trust.GetTrustResult (), Is.EqualTo (trust_result), "GetTrustResult");

			trust.SetAnchorCertificates (certs);
			Assert.That (trust.GetCustomAnchorCertificates ().Length, Is.EqualTo (certs.Count), "GetCustomAnchorCertificates");

			// since we modified the `trust` instance it's result was invalidated (marked as unspecified on iOS 11)
			Assert.That (trust.GetTrustResult (), Is.EqualTo (SecTrustResult.Unspecified), "GetTrustResult-2");

			Assert.True (trust.Evaluate (out var error), $"Evaluate: {error}");
			Assert.Null (error, "error");
		}

		[Test]
		public void Trust2_Leaf_Only ()
		{
			X509Certificate x = new X509Certificate2 (CertificateTest.mail_google_com);
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (x, policy)) {
				Trust_Leaf_Only (trust, policy);
			}
		}

		[Test]
		public void Trust2_NoRoot ()
		{
			X509Certificate2Collection certs = new X509Certificate2Collection ();
			certs.Add (new X509Certificate2 (CertificateTest.mail_google_com));
			certs.Add (new X509Certificate2 (CertificateTest.gts_ca_1c3));
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (certs, policy)) {
				Trust_NoRoot (trust, policy);
			}
		}

		[Test]
		public void Trust2_FullChain ()
		{
			X509Certificate2Collection certs = new X509Certificate2Collection ();
			certs.Add (new X509Certificate2 (CertificateTest.mail_google_com));
			certs.Add (new X509Certificate2 (CertificateTest.gts_ca_1c3));
			certs.Add (new X509Certificate2 (CertificateTest.gts_root_r1));
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (certs, policy)) {
				Trust_FullChain (trust, policy, certs);
			}
		}

		[Test]
		public void Timestamps ()
		{
			TestRuntime.AssertXcodeVersion (10, 1); // old API exposed publicly

			X509Certificate2Collection certs = new X509Certificate2Collection ();
			certs.Add (new X509Certificate2 (CertificateTest.mail_google_com));
			certs.Add (new X509Certificate2 (CertificateTest.gts_ca_1c3));
			certs.Add (new X509Certificate2 (CertificateTest.gts_root_r1));
			using (var policy = SecPolicy.CreateSslPolicy (true, "mail.google.com"))
			using (var trust = new SecTrust (certs, policy)) {
				var a = new NSArray<NSData> ();
				var e = trust.SetSignedCertificateTimestamps (a);
				Assert.That (e, Is.EqualTo (SecStatusCode.Success), "1");
				a = null;
				e = trust.SetSignedCertificateTimestamps (null);
				Assert.That (e, Is.EqualTo (SecStatusCode.Success), "2");

				var i = new NSData [0];
				e = trust.SetSignedCertificateTimestamps (i);
				Assert.That (e, Is.EqualTo (SecStatusCode.Success), "3");
				i = null;
				e = trust.SetSignedCertificateTimestamps (i);
				Assert.That (e, Is.EqualTo (SecStatusCode.Success), "4");
			}
		}
	}
}
