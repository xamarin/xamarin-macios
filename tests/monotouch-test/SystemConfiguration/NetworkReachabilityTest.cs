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
#if XAMCORE_2_0
using Foundation;
#if !MONOMAC
using UIKit;
#endif
using ObjCRuntime;
using SystemConfiguration;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.SystemConfiguration;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;
using System.Net;

namespace MonoTouchFixtures.SystemConfiguration {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NetworkReachabilityTest
	{
		[Test]
		public void CtorNameAddress ()
		{
			using (var nr = new NetworkReachability ("apple.com"))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags));
				Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "Reachable");
			}
		}

		[Test]
		public void CtorIPAddress ()
		{
			using (var nr = new NetworkReachability (IPAddress.Loopback))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#1");

				// inconsistent results across different iOS versions
				// < 9.0 -> Reachable | IsLocalAddress
				// 9.x -> Reachable | IsLocalAddress | IsDirect
				// 10.0 -> Reachable
				// so we're only checking the (most important) Reachable flag
				Assert.True ((flags & NetworkReachabilityFlags.Reachable) != 0, "Reachable");
			}

			using (var nr = new NetworkReachability (new IPAddress (new byte[] { 10, 99, 99, 99 })))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#2");
				//Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "#2 Reachable");
			}

			using (var nr = new NetworkReachability (IPAddress.IPv6Loopback))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#3");
				//Assert.That (flags, Is.EqualTo (
				//	NetworkReachabilityFlags.TransientConnection | NetworkReachabilityFlags.Reachable | NetworkReachabilityFlags.ConnectionRequired), "#3 Reachable");
			}

			using (var nr = new NetworkReachability (IPAddress.Parse ("2001:4860:4860::8844")))
			{
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
			var address = Dns.GetHostAddresses ("apple.com")[0];
			using (var nr = new NetworkReachability (IPAddress.Loopback, address))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#1");
				CheckLoopbackFlags (flags, "1", true);
			}

			using (var nr = new NetworkReachability (null, address))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#2");
				Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "#2 Reachable");
			}

			using (var nr = new NetworkReachability (IPAddress.Loopback, null))
			{
				NetworkReachabilityFlags flags;

				Assert.IsTrue (nr.TryGetFlags (out flags), "#3");
				CheckLoopbackFlags (flags, "3", false);
			}
		}

		void CheckLoopbackFlags (NetworkReachabilityFlags flags, string number, bool has_address)
		{
			// Varios OS versions do different things.
			// It seems Apple throws a dice before every OS release to see what they'll do that year...
			var noFlags = (NetworkReachabilityFlags) 0;

			var isReachable = (flags & NetworkReachabilityFlags.Reachable) == NetworkReachabilityFlags.Reachable;
			var isLocalAddress = (flags & NetworkReachabilityFlags.IsLocalAddress) == NetworkReachabilityFlags.IsLocalAddress;
			var isDirect = (flags & NetworkReachabilityFlags.IsDirect) == NetworkReachabilityFlags.IsDirect;
			var otherFlags = (flags & ~(NetworkReachabilityFlags.Reachable | NetworkReachabilityFlags.IsLocalAddress | NetworkReachabilityFlags.IsDirect));
			var shouldBeReachable = true;
#if MONOMAC
			if (has_address) {
				shouldBeReachable = true;
			} else {
				shouldBeReachable = !TestRuntime.CheckMacSystemVersion (10, 12);
			}
#elif __IOS__
			if (Runtime.Arch == Arch.DEVICE) {
				shouldBeReachable = !TestRuntime.CheckXcodeVersion (8, 0) || TestRuntime.CheckXcodeVersion (11, 2);
			} else {
				shouldBeReachable = true;
			}
#endif

			Assert.AreEqual (shouldBeReachable, isReachable, $"#{number} Reachable: {flags.ToString ()}");

			bool has_local_address;
			bool has_isdirect;
#if MONOMAC
			if (has_address) {
				has_local_address = false;
				has_isdirect = has_local_address;
			} else {
				has_local_address = !TestRuntime.CheckMacSystemVersion (10, 11);
				has_isdirect = false;
			}
#elif __IOS__
			if (Runtime.Arch == Arch.DEVICE) {
				if (has_address) {
					has_local_address = !TestRuntime.CheckXcodeVersion (6, 0) || TestRuntime.CheckXcodeVersion (11, 2);
					has_isdirect = TestRuntime.CheckXcodeVersion (11, 2);
				} else {
					has_local_address = !TestRuntime.CheckXcodeVersion (7, 0) || TestRuntime.CheckXcodeVersion (11, 2);
					has_isdirect = TestRuntime.CheckXcodeVersion (11, 2);
				}
			} else {
				has_local_address = !TestRuntime.CheckXcodeVersion (7, 0);
				has_isdirect = false;
			}
#else
			if (Runtime.Arch == Arch.DEVICE) {
				has_local_address = true;
				has_isdirect = true;
			} else {
				has_local_address = !TestRuntime.CheckXcodeVersion (7, 0);
				has_isdirect = false;
			}
#endif

			Assert.AreEqual (has_local_address, isLocalAddress, $"#{number} IsLocalAddress: {flags.ToString ()}");
			Assert.AreEqual (has_isdirect, isDirect, $"#{number} IsDirect: {flags.ToString ()}");
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
