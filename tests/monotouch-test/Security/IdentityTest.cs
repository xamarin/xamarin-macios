//
// SecIdentity Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc.
//

using System;
#if XAMCORE_2_0
using Foundation;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.Security;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class IdentityTest {

		static public SecIdentity GetIdentity ()
		{
			using (var options = NSDictionary.FromObjectAndKey (new NSString ("farscape"), SecImportExport.Passphrase)) {
				NSDictionary[] array;
				if (SecImportExport.ImportPkcs12 (ImportExportTest.farscape_pfx, options, out array) != SecStatusCode.Success)
					Assert.Fail ("ImportPkcs12");
				return new SecIdentity (array [0].LowlevelObjectForKey (SecImportExport.Identity.Handle));
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
			TestRuntime.AssertXcodeVersion (10,0);
			using (var i1 = GetIdentity ()) 
			using (var i2 = new SecIdentity2 (i1)) {
				Assert.That (i1.Certificate.GetCommonName (), Is.EqualTo (i2.Identity.Certificate.GetCommonName ()), "GetCommonName");
			}
		}
	}
}
