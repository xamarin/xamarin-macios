//
// Unit tests for AVQueuePlayer
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using AVFoundation;
#else
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class QueuePlayerTest {
		[Test]
		public void NullAllowedTest ()
		{
			using (var player = new AVQueuePlayer ()) {
				using (var item = new AVPlayerItem (NSUrl.FromString ("http://example.org"))) {
					player.CanInsert (item, null);
					player.InsertItem (item, null);
				}
			}
		}
	}
}

#endif // !__WATCHOS__
