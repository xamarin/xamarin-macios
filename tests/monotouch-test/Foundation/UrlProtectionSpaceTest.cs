//
// Unit tests for NSUrlProtectionSpace
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Security.Cryptography.X509Certificates;
using Foundation;
using Security;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

using MonoTouchFixtures.Security;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlProtectionSpaceTest {

		[Test]
		public void Http ()
		{
			using (var ps = new NSUrlProtectionSpace ("www.xamarin.com", 80, NSUrlProtectionSpace.HTTP, null, null)) {
				Assert.That (ps.AuthenticationMethod, Is.EqualTo ("NSURLAuthenticationMethodDefault"), "AuthenticationMethod");
				Assert.Null (ps.DistinguishedNames, "DistinguishedNames");
				Assert.That (ps.Host, Is.EqualTo ("www.xamarin.com"), "Host");
				Assert.False (ps.IsProxy, "IsProxy");
				Assert.That (ps.Port, Is.EqualTo ((nint) 80), "Port");
				Assert.That (ps.Protocol, Is.EqualTo ("http"), "Protocol");
				Assert.Null (ps.ProxyType, "ProxyType");
				Assert.Null (ps.Realm, "Realm");
				Assert.False (ps.ReceivesCredentialSecurely, "ReceivesCredentialSecurely");
				Assert.Null (ps.ServerSecTrust, "ServerSecTrust");
			}
		}

		[Test]
		public void Https ()
		{
			using (var ps = new NSUrlProtectionSpace ("mail.google.com", 443, NSUrlProtectionSpace.HTTPS, null, NSUrlProtectionSpace.AuthenticationMethodHTTPBasic)) {
				if (TestRuntime.CheckXcodeVersion (7, 0)) {
					Assert.That (ps.AuthenticationMethod, Is.EqualTo ("NSURLAuthenticationMethodHTTPBasic"), "AuthenticationMethod");
				} else {
					Assert.That (ps.AuthenticationMethod, Is.EqualTo ("NSURLAuthenticationMethodDefault"), "AuthenticationMethod");
				}
				Assert.Null (ps.DistinguishedNames, "DistinguishedNames");
				Assert.That (ps.Host, Is.EqualTo ("mail.google.com"), "Host");
				Assert.False (ps.IsProxy, "IsProxy");
				Assert.That (ps.Port, Is.EqualTo ((nint) 443), "Port");
				Assert.That (ps.Protocol, Is.EqualTo ("https"), "Protocol");
				Assert.Null (ps.ProxyType, "ProxyType");
				Assert.Null (ps.Realm, "Realm");
				Assert.True (ps.ReceivesCredentialSecurely, "ReceivesCredentialSecurely");
				Assert.Null (ps.ServerSecTrust, "ServerSecTrust");
			}
		}

		[Test]
		public void HttpProxy ()
		{
			using (var ps = new NSUrlProtectionSpace ("www.xamarin.com", 80, NSUrlProtectionSpace.HTTPProxy, "default", NSUrlProtectionSpace.AuthenticationMethodHTTPDigest, false)) {
				Assert.That (ps.AuthenticationMethod, Is.EqualTo ("NSURLAuthenticationMethodHTTPDigest"), "AuthenticationMethod");
				Assert.Null (ps.DistinguishedNames, "DistinguishedNames");
				Assert.That (ps.Host, Is.EqualTo ("www.xamarin.com"), "Host");
				Assert.False (ps.IsProxy, "IsProxy");
				Assert.That (ps.Port, Is.EqualTo ((nint) 80), "Port");
				Assert.That (ps.Protocol, Is.EqualTo ("http"), "Protocol");
				Assert.Null (ps.ProxyType, "ProxyType");
				Assert.That (ps.Realm, Is.EqualTo ("default"), "Realm");
				Assert.True (ps.ReceivesCredentialSecurely, "ReceivesCredentialSecurely");
				Assert.Null (ps.ServerSecTrust, "ServerSecTrust");
			}
		}

		[Test]
		public void HttpProxy_Proxy ()
		{
			using (var ps = new NSUrlProtectionSpace ("www.xamarin.com", 80, NSUrlProtectionSpace.HTTPProxy, "default", NSUrlProtectionSpace.AuthenticationMethodHTTPDigest, true)) {
				Assert.That (ps.AuthenticationMethod, Is.EqualTo ("NSURLAuthenticationMethodHTTPDigest"), "AuthenticationMethod");
				Assert.Null (ps.DistinguishedNames, "DistinguishedNames");
				Assert.That (ps.Host, Is.EqualTo ("www.xamarin.com"), "Host");
				Assert.True (ps.IsProxy, "IsProxy");
				Assert.That (ps.Port, Is.EqualTo ((nint) 80), "Port");
				Assert.That (ps.Protocol, Is.EqualTo ("http"), "Protocol");
				Assert.That (ps.ProxyType, Is.EqualTo ("http"), "ProxyType");
				Assert.Null (ps.Realm, "Realm");
				Assert.True (ps.ReceivesCredentialSecurely, "ReceivesCredentialSecurely");
				Assert.Null (ps.ServerSecTrust, "ServerSecTrust");
			}
		}
	}
}
