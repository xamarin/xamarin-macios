//
// Unit tests for HMSignificantTimeEventTest
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
	public class HMSignificantTimeEventTest
	{
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void SignificantEventPropertyTest ()
		{
			using (var obj = new HMSignificantTimeEvent (HMSignificantEvent.Sunrise, null)) {
				Assert.AreEqual (HMSignificantEvent.Sunrise, obj.SignificantEvent, "1 SignificantEvent Getter");
			}
		}
	}
}

#endif