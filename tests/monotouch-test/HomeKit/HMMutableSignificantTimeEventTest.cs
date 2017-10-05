//
// Unit tests for HMMutableSignificantTimeEventTest
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
	public class HMMutableSignificantTimeEventTest
	{
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void SignificantEventPropertyTest ()
		{
			using (var obj = new HMMutableSignificantTimeEvent (HMSignificantEvent.Sunrise, null)) {
				Assert.AreEqual (HMSignificantEvent.Sunrise, obj.SignificantEvent, "1 SignificantEvent Getter");
				obj.SignificantEvent = HMSignificantEvent.Sunset;
				Assert.AreEqual (HMSignificantEvent.Sunset, obj.SignificantEvent, "2 PresenceType Setter");
			}
		}
	}
}

#endif