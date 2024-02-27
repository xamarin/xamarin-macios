//
// Unit tests for UIDevice
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !MONOMAC
using System;
using System.IO;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DeviceTest {

#if !__TVOS__ && !__WATCHOS__ && !__MACCATALYST__
		[Test]
		public void Battery ()
		{
			UIDevice device = UIDevice.CurrentDevice;
			Assert.False (device.BatteryMonitoringEnabled, "false");
			Assert.That (device.BatteryState, Is.EqualTo (UIDeviceBatteryState.Unknown), "false/Unknown");
			Assert.That (device.BatteryLevel, Is.EqualTo (-1), "false/-1");

			device.BatteryMonitoringEnabled = true;
			try {
				if (Runtime.Arch == Arch.SIMULATOR) {
					Assert.That (device.BatteryState, Is.EqualTo (UIDeviceBatteryState.Unknown), "true/Unknown");
					Assert.That (device.BatteryLevel, Is.EqualTo (-1), "true/-1");
				} else {
					Assert.That (device.BatteryState, Is.Not.EqualTo (UIDeviceBatteryState.Unknown), "true/Unknown");
					Assert.That (device.BatteryLevel, Is.Not.EqualTo (-1), "true/-1");
				}
			} finally {
				device.BatteryMonitoringEnabled = false;
			}
		}
#endif // !__TVOS__ && !__WATCHOS__ && !__MACCATALYST__
	}
}
#endif
