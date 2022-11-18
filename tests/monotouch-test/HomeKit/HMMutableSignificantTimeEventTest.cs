//
// Unit tests for HMMutableSignificantTimeEventTest
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//	
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if HAS_HOMEKIT

using System;
using NUnit.Framework;

using Foundation;
using HomeKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.HomeKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HMMutableSignificantTimeEventTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
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

#endif // HAS_HOMEKIT
