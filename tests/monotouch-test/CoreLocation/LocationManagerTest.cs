// Copyright 2019 Microsoft Corporation

#if __IOS__

using Foundation;
using CoreLocation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreLocation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LocationManagerTest {

		[Test]
		public void DeferredLocationUpdatesAvailable ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			// deprecated - mention not to call it, but unclear what it returns to existing code
			Assert.False (CLLocationManager.DeferredLocationUpdatesAvailable, "DeferredLocationUpdatesAvailable");
		}
	}
}

#endif
