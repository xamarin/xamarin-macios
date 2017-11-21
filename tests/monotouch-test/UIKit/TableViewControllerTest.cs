//
// Unit tests for UITableViewController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TableViewControllerTest {

#if !__TVOS__
		[Test]
		public void RefreshControl_18744 ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6, 0))
				Assert.Ignore ("requires 6.0+");

			using (var rc = new UIRefreshControl ())
			using (var tvc = new UITableViewController ()) {
				Assert.Null (tvc.RefreshControl, "default");
				tvc.RefreshControl = rc;
				Assert.AreSame (tvc.RefreshControl, rc, "same");
				tvc.RefreshControl = null;
				Assert.Null (tvc.RefreshControl, "nullable");
			}
		}
#endif // !__TVOS__
	}
}

#endif // !__WATCHOS__
