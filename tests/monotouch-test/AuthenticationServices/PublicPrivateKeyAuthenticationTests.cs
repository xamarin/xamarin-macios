//
// Copyright 2021 Microsoft Corp
//
// Authors:
//	TJ Lambert (antlambe@microsoft.com)
//

#if !__TVOS__ && !__WATCHOS__

using System;
using Foundation;
using AuthenticationServices;
using NUnit.Framework;

namespace MonoTouchFixtures.AuthenticationServices {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PublicPrivateKeyAuthenticationTests {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
		}

		ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport [] transports = {
			ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport.Usb,
			ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport.Nfc,
			ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport.Bluetooth,
		};

		[Test]
		public void GetAllSupportedPublicKeyCredentialDescriptorTransports ()
		{
			var value = PublicPrivateKeyAuthentication.GetAllSupportedPublicKeyCredentialDescriptorTransports ();

			//Expected: some item equal to< < usb >, < nfc >, < ble > >
			//But was: < Usb, Nfc, Bluetooth >

			Assert.Contains (PublicPrivateKeyAuthentication.GetAllSupportedPublicKeyCredentialDescriptorTransports (), transports, "The three transports are not supported as expected");
		}
	}
}
#endif