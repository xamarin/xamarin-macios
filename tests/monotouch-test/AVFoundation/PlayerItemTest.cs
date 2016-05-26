//
// Unit tests for AVPlayerItem
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using AVFoundation;
using Foundation;
using UIKit;
#else
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayerItemTest {

		[Test]
		public void FromAssert_Null ()
		{
			// Apple's AVCustomEdit samples calls this with `nil`
			Assert.Null (AVPlayerItem.FromAsset (null), "1");

			if (TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Null (AVPlayerItem.FromAsset (null, null), "2");
		}
	}
}

#endif // !__WATCHOS__
