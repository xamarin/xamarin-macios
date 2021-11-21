//
// Unit tests for GKLeaderboardViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !__MACCATALYST__

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
using Xamarin.Utils;

namespace MonoTouchFixtures.GameKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LeaderboardViewControllerTest {

		[Test]
		public void DefaultCtor ()
		{
#if MONOMAC
			// fails when executed under BigSur - this has been deprecated for a while (even if it remains working elsewhere)
			if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 11, 0))
				Assert.Inconclusive ("'LeaderboardViewControllerTest' the native 'init' method returned nil.");
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);
#endif
			using (var vc = new GKLeaderboardViewController ()) {
				Assert.Null (vc.Category, "Category");
				Assert.Null (vc.Delegate, "Delegate");
				// default Scope vary by iOS version and can't be changed on iOS7 - not worth testing
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__ && !__MACCATALYST__
