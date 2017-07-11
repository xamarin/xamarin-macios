//
// Unit tests for HMMutablePresenceEventTest
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//	
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if !MONOMAC

using System;
using NUnit.Framework;

using Foundation;
using HomeKit;

namespace MonoTouchFixtures.HomeKit
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HMMutablePresenceEventTest
	{
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void PresenceTypePropertyTest ()
		{
			using (var obj = new HMMutablePresenceEvent (HMPresenceType.AnyUserAtHome)) {
				Assert.AreEqual (HMPresenceType.AnyUserAtHome, obj.PresenceType, "1 PresenceType Getter");
				obj.PresenceType = HMPresenceType.CurrentUserAtHome;
				Assert.AreEqual (HMPresenceType.CurrentUserAtHome, obj.PresenceType, "2 PresenceType Setter");
			}
		}
	}
}

#endif