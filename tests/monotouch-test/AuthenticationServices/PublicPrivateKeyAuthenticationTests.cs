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

		[Test]
		public void GetAllSupportedPublicKeyCredentialDescriptorTransports ()
		{
			var transports = PublicPrivateKeyAuthentication.GetAllSupportedPublicKeyCredentialDescriptorTransports ();
			Assert.IsNotNull (PublicPrivateKeyAuthentication.GetAllSupportedPublicKeyCredentialDescriptorTransports (), "The transports should not be null");

			// since there is no default enum value, make sure there is not
			// more than one ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport.Usb
			var usbCounter = 0;
			foreach (var transport in transports) {
				if (transport == ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport.Usb)
					usbCounter++;
			}
			Assert.LessOrEqual (usbCounter, 1, "There were multiple usb transports found. Add any new transports to GetAllSupportedPublicKeyCredentialDescriptorTransports inside src/AuthenticationServices/PublicPrivateKeyAuthentication.cs");
		}
	}
}
#endif
