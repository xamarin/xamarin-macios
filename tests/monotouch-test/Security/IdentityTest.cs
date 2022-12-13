//
// SecIdentity Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//

using System;
using Foundation;
using ObjCRuntime;
using Security;
using NUnit.Framework;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class IdentityTest {

		static public SecIdentity GetIdentity ()
		{
			using (var options = NSDictionary.FromObjectAndKey (new NSString ("farscape"), SecImportExport.Passphrase)) {
				NSDictionary [] array;
				if (SecImportExport.ImportPkcs12 (ImportExportTest.farscape_pfx, options, out array) != SecStatusCode.Success)
					Assert.Fail ("ImportPkcs12");
				return Runtime.GetINativeObject<SecIdentity> (array [0].LowlevelObjectForKey (SecImportExport.Identity.Handle), false);
			}
		}

		[Test]
		public void Identity ()
		{
			using (SecIdentity id = GetIdentity ()) {
				Assert.NotNull (id.PrivateKey, "PrivateKey");
				Assert.NotNull (id.Certificate, "Certificate");
			}
		}

		[Test]
		public void I2 ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			using (var i1 = GetIdentity ())
			using (var i2 = new SecIdentity2 (i1)) {
				Assert.That (i1.Certificate.GetCommonName (), Is.EqualTo (i2.Identity.Certificate.GetCommonName ()), "GetCommonName");
			}
		}

		[Test]
		public void AccessCertificates ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var i1 = GetIdentity ())
			using (var i2 = new SecIdentity2 (i1, i1.Certificate)) {
				int call = 0;
				Assert.True (i2.AccessCertificates ((c) => {
					Assert.That (i1.Certificate.GetCommonName (), Is.EqualTo (c.Certificate.GetCommonName ()), "GetCommonName");
					call++;

				}), "Access");
				Assert.That (call, Is.EqualTo (1), "call");
			}
		}
	}
}
