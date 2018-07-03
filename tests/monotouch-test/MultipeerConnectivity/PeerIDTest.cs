//
// MCPeerID Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__TVOS__ && !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using MultipeerConnectivity;
#else
using MonoTouch.Foundation;
using MonoTouch.MultipeerConnectivity;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MultipeerConnectivity {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class PeerIDTest {

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);
			TestRuntime.AsserttvOSSystemVersion (10, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);

			using (var peer = new MCPeerID ()) {
				Assert.Null (peer.DisplayName, "DisplayName");
			}
		}

		[Test]
		public void LocalPeer ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);
			TestRuntime.AsserttvOSSystemVersion (10, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);

			using (var peer = new MCPeerID ("myself")) {
				Assert.That (peer.DisplayName, Is.EqualTo ("myself"), "DisplayName");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
