#if MONOMAC

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreWlan;
using Security;

using NUnit.Framework;
using MonoTouchFixtures.Security;

namespace MonoTouchFixtures.CoreWlan {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CWKeychainTests {

		CWKeychainDomain domain;
		NSData ssid;

		[SetUp]
		public void SetUp ()
		{
			domain = CWKeychainDomain.System;
			// always return a new one so that test do not mess with eachother
			ssid = NSData.FromString (Guid.NewGuid ().ToString ());
		}

		[Test]
		public void TryFindWiFiEAPIdentityMissingTest ()
			=> Assert.False (CWKeychain.TryFindWiFiEAPIdentity (domain, ssid, out var secIdentity));

		[Test]
		public void TryDeleteWiFiEAPUsernameAndPasswordMissingTest ()
			=> Assert.False (CWKeychain.TryDeleteWiFiEAPUsernameAndPassword (domain, ssid));

		[Test]
		public void TryDeleteWiFiPasswordMissingTest ()
			=> Assert.True (CWKeychain.TryDeleteWiFiPassword (domain, ssid));

		[Test]
		public void TryFindWiFiEAPUsernameAndPasswordMissingTest ()
			=> Assert.False (CWKeychain.TryFindWiFiEAPUsernameAndPassword (domain, ssid, out string username, out string password));

		[Test]
		public void TryFindWiFiPasswordMissingTest ()
			=> Assert.False (CWKeychain.TryFindWiFiPassword (domain, ssid, out string password));

		[Test]
		public void TrySetWiFiEAPIdentityTest ()
		{
			// false because the ssid is not present
			var identity = IdentityTest.GetIdentity ();
			Assert.False (CWKeychain.TrySetWiFiEAPIdentity (domain, ssid, identity));
			// remove it to clean behind
			CWKeychain.TryDeleteWiFiEAPUsernameAndPassword (domain, ssid);
		}

		[Test]
		public void TrySetWiFiEAPUsernameAndPasswordTest ()
		{
			// all false because the ssid does not exist.
			Assert.False (CWKeychain.TrySetWiFiEAPUsernameAndPassword (domain, ssid, "mandel", "test"), "Both present");
			Assert.False (CWKeychain.TrySetWiFiEAPUsernameAndPassword (domain, ssid, "mandel", null), "Null pwd");
			Assert.False (CWKeychain.TrySetWiFiEAPUsernameAndPassword (domain, ssid, null, "test"), "Null user");
		}

		[Test]
		public void TrySetWiFiPasswordTest ()
			=> Assert.True (CWKeychain.TrySetWiFiPassword (domain, ssid, "password"));

	}
}
#endif
