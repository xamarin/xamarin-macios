//
// Unit tests for NetworkReachability
//
// Authors:
//	Marek Safar <msafar@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
using ObjCRuntime;
using SystemConfiguration;
using NUnit.Framework;
using System.Net;

namespace MonoTouchFixtures.SystemConfiguration {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NetworkReachabilityTest {
		[Test]
		public void CtorNameAddress ()
		{
			using (var nr = new NetworkReachability ("apple.com")) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags));
				Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "Reachable");
			}
		}

		[Test]
		public void CtorIPAddress ()
		{
			using (var nr = new NetworkReachability (IPAddress.Loopback)) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#1");

				// inconsistent results across different iOS versions
				// < 9.0 -> Reachable | IsLocalAddress
				// 9.x -> Reachable | IsLocalAddress | IsDirect
				// 10.0 -> Reachable
				// so we're only checking the (most important) Reachable flag
				Assert.True ((flags & NetworkReachabilityFlags.Reachable) != 0, "Reachable");
			}

			using (var nr = new NetworkReachability (new IPAddress (new byte [] { 10, 99, 99, 99 }))) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#2");
				//Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "#2 Reachable");
			}

			using (var nr = new NetworkReachability (IPAddress.IPv6Loopback)) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#3");
				//Assert.That (flags, Is.EqualTo (
				//	NetworkReachabilityFlags.TransientConnection | NetworkReachabilityFlags.Reachable | NetworkReachabilityFlags.ConnectionRequired), "#3 Reachable");
			}

			using (var nr = new NetworkReachability (IPAddress.Parse ("2001:4860:4860::8844"))) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#4");

				// TODO: Will probably change when IPv6 is enabled locally
				//Assert.That (flags, Is.EqualTo (
				//	NetworkReachabilityFlags.TransientConnection | NetworkReachabilityFlags.Reachable | NetworkReachabilityFlags.ConnectionRequired), "#4 Reachable");
			}

		}

		[Test]
		public void CtorIPAddressPair ()
		{
			var address = Dns.GetHostAddresses ("apple.com") [0];
			using (var nr = new NetworkReachability (IPAddress.Loopback, address)) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#1");
				CheckLoopbackFlags (flags, "1", true);
			}

			using (var nr = new NetworkReachability (null, address)) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#2");
				Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "#2 Reachable");
			}

			using (var nr = new NetworkReachability (IPAddress.Loopback, null)) {
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#3");
				CheckLoopbackFlags (flags, "3", false);
			}
		}

		void CheckLoopbackFlags (NetworkReachabilityFlags flags, string number, bool has_address)
		{
			var noFlags = (NetworkReachabilityFlags) 0;
			var otherFlags = (flags & ~(NetworkReachabilityFlags.Reachable | NetworkReachabilityFlags.IsLocalAddress | NetworkReachabilityFlags.IsDirect));

			// Different versions of OSes report different flags. Trying to
			// figure out which OS versions have which flags set turned out to
			// be a never-ending game of whack-a-mole, so just don't assert
			// that any specific flags are set.
			Assert.AreEqual (noFlags, otherFlags, $"#{number} No other flags: {flags.ToString ()}");
		}

		[Test]
		public void Ctor_Invalid ()
		{
			try {
				new NetworkReachability ((string) null);
				Assert.Fail ("#1");
			} catch (ArgumentNullException) {
			}

			try {
				new NetworkReachability ((IPAddress) null);
				Assert.Fail ("#2");
			} catch (ArgumentNullException) {
			}

			try {
				new NetworkReachability (null, null);
				Assert.Fail ("#3");
			} catch (ArgumentException) {
			}
		}
	}
}

#endif // !__WATCHOS__
