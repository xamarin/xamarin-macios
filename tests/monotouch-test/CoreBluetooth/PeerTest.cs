//
// Unit tests for CBPeer
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreBluetooth;
#else
using MonoTouch.CoreBluetooth;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreBluetooth {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CBPeerTest {
		[Test]
		public void Constructor ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 13, throwIfOtherPlatform: false);

			// crash at dispose time in beta 4 (and 5)
			// the type is undocumented but I think it's should be abstract (not user creatable)
//			using (var p = new CBPeer ()) {
//				Assert.NotNull (p);
//			}
		}
	}
}

#endif // !__WATCHOS__
