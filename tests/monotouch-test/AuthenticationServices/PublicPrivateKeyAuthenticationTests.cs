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
			Assert.IsNotNull (PublicPrivateKeyAuthentication.GetAllSupportedPublicKeyCredentialDescriptorTransports (), "The transports should not be null");
		}
	}
}
#endif
