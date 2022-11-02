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
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TableViewControllerTest {

#if !__TVOS__
		[Test]
		public void RefreshControl_18744 ()
		{
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
