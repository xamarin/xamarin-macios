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
using Foundation;
using MultipeerConnectivity;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MultipeerConnectivity {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class PeerIDTest {

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			using (var peer = new MCPeerID ("MyDisplayName")) {
				Assert.AreEqual ("MyDisplayName", peer.DisplayName, "DisplayName");
			}
		}

		[Test]
		public void LocalPeer ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 10, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			using (var peer = new MCPeerID ("myself")) {
				Assert.That (peer.DisplayName, Is.EqualTo ("myself"), "DisplayName");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
