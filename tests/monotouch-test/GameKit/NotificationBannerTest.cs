//
// Unit tests for GKNotificationBanner
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012,2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using Foundation;

using GameKit;

using NUnit.Framework;

namespace MonoTouchFixtures.GameKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NotificationBannerTest {

		[Test]
		public void Show_NSAction_Null ()
		{
			// Once upon a time (circa 2012) using null for the action would have crashed the application
			// but it's not the case anymore (in 2015 / iOS9) and the header files says it's nullable
			GKNotificationBanner.Show ("title", "message", null);
		}
	}
}

#endif // !__WATCHOS__
