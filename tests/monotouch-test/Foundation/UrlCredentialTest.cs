//
// Unit tests for NSUrlCredential
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System.Security.Cryptography.X509Certificates;
using Foundation;
using ObjCRuntime;
using Security;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

using MonoTouchFixtures.Security;
using Xamarin.Utils;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlCredentialTest {

		SecTrust GetTrust ()
		{
			X509Certificate x = new X509Certificate (CertificateTest.mail_google_com);
			using (var policy = SecPolicy.CreateBasicX509Policy ())
				return new SecTrust (x, policy);
		}

		[Test]
		public void Ctor_Trust ()
		{
			using (var trust = GetTrust ())
			using (var creds = new NSUrlCredential (trust)) {
				Assert.Null (creds.Certificates, "Certificates");
				Assert.False (creds.HasPassword, "HasPassword");
				Assert.Null (creds.SecIdentity, "SecIdentity");
				Assert.Null (creds.Password, "Password");
				var expectedPersistence = NSUrlCredentialPersistence.ForSession;
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 8))
					expectedPersistence = (NSUrlCredentialPersistence) uint.MaxValue;
#endif
				Assert.That (creds.Persistence, Is.EqualTo (expectedPersistence), "Persistence");
				Assert.Null (creds.User, "User");
			}
		}

		[Test]
		public void FromTrust ()
		{
			using (var trust = GetTrust ())
			using (var creds = NSUrlCredential.FromTrust (trust)) {
				Assert.Null (creds.Certificates, "Certificates");
				Assert.False (creds.HasPassword, "HasPassword");
				Assert.Null (creds.SecIdentity, "SecIdentity");
				Assert.Null (creds.Password, "Password");
				var expectedPersistence = NSUrlCredentialPersistence.ForSession;
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 8))
					expectedPersistence = (NSUrlCredentialPersistence) uint.MaxValue;
#endif
				Assert.That (creds.Persistence, Is.EqualTo (expectedPersistence), "Persistence");
				Assert.Null (creds.User, "User");
			}
		}
	}
}
