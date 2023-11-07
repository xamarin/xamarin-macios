//
// Unit tests for CBPeer
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using Foundation;
using CoreBluetooth;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreBluetooth {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CBPeerTest {
		[Test]
		public void Constructor ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 13, throwIfOtherPlatform: false);

			// crash at dispose time in beta 4 (and 5)
			// the type is undocumented but I think it's should be abstract (not user creatable)
			//			using (var p = new CBPeer ()) {
			//				Assert.NotNull (p);
			//			}
		}
	}
}

#endif // !__WATCHOS__
