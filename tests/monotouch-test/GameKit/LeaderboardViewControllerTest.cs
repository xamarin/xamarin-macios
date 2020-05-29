//
// Unit tests for GKLeaderboardViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

using System;
using System.IO;
using System.Threading;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using GameKit;
using NUnit.Framework;

namespace MonoTouchFixtures.GameKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LeaderboardViewControllerTest {

		[Test]
		public void DefaultCtor ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
			using (var vc = new GKLeaderboardViewController ()) {
				Assert.Null (vc.Category, "Category");
				Assert.Null (vc.Delegate, "Delegate");
				// default Scope vary by iOS version and can't be changed on iOS7 - not worth testing
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
