//
// Unit tests for NotificationQueue
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NotificationQueueTest {

		[Test]
		public void DefaultQueue ()
		{
			Assert.That (NSNotificationQueue.DefaultQueue, Is.TypeOf<NSNotificationQueue> (), "DefaultQueue");
		}
	}
}
