//
// Unit tests for NEVpnManager
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using NetworkExtension;
using Xamarin.Utils;

namespace MonoTouchFixtures.NetworkExtension {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VpnManagerTest {

		[Test]
#if APP_EXTENSION
		[Ignore ("App extensions don't have VPN entitlements.")]
#endif
		public void SharedManager ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			var shared = NEVpnManager.SharedManager;
			// https://developer.apple.com/library/ios/documentation/IDEs/Conceptual/AppDistributionGuide/AddingCapabilities/AddingCapabilities.html#//apple_ref/doc/uid/TP40012582-CH26-SW59
			// Enabling Personal VPN (iOS Only)
			if (shared is null)
				Assert.Inconclusive ("Requires enabling Personal PVN (entitlements)");

			Assert.That (shared.Connection.Status, Is.EqualTo (NEVpnStatus.Invalid), "Connection");
#if MONOMAC || __MACCATALYST__
			Assert.That (shared.Enabled, Is.True, "Enabled");
#else
			Assert.That (shared.Enabled, Is.False, "Enabled");
#endif
#if __IOS__
			var HasLocalizedDescription = TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 9, 0);
#elif __MACOS__
			var HasLocalizedDescription = true;
#endif
			if (HasLocalizedDescription) {
				Assert.That (shared.LocalizedDescription, Is.EqualTo ("MonoTouchTest"), "LocalizedDescription");
			} else {
				Assert.That (shared.LocalizedDescription, Is.Null, "LocalizedDescription");
			}
			Assert.That (shared.OnDemandEnabled, Is.False, "OnDemandEnabled");
			Assert.That (shared.OnDemandRules, Is.Null, "OnDemandRules");
			Assert.That (shared.Protocol, Is.Null, "Protocol");
		}

		[Test]
		public void Fields ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			Assert.That (NEVpnError.ConnectionFailed.GetDomain ().ToString (), Is.EqualTo ("NEVPNErrorDomain"), "ErrorDomain");
		}
	}
}

#endif // !__TVOS__
