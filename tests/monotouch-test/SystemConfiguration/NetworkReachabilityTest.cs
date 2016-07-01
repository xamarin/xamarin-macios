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
using UIKit;
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
				Assert.That (flags, Is.EqualTo (NetworkReachabilityFlags.Reachable), "#1 Reachable");
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
				var expected = NetworkReachabilityFlags.Reachable;
				if (!UIDevice.CurrentDevice.CheckSystemVersion (9,0))
					expected |= NetworkReachabilityFlags.IsLocalAddress; 
				Assert.That (flags, Is.EqualTo (expected), "#3 Reachable");
			}
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
