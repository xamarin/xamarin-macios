//
// Unit tests for AVPlayerItem
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using AVFoundation;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayerItemTest {

		[Test]
		public void FromAssert_Null ()
		{
			TestRuntime.AssertXcodeVersion (5, 1);
			// Apple's AVCustomEdit samples calls this with `nil`
			Assert.Null (AVPlayerItem.FromAsset (null), "1");

			if (TestRuntime.CheckXcodeVersion (5, 0, 1))
				Assert.Null (AVPlayerItem.FromAsset (null, null), "2");
		}
	}
}

#endif // !__WATCHOS__
