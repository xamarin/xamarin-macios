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
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DeviceTest {

#if !XAMCORE_2_0
		[Test]
		public void CurrentDevice ()
		{
			UIDevice device = UIDevice.CurrentDevice;
			// under iOS 5.1 some API (mobileDevice on client) stopped reporting the identifier
			// this will warn us then this API "fails" too
			Assert.NotNull (device.UniqueIdentifier, "UniqueIdentifier");
		}
#endif

#if !__TVOS__ && !__WATCHOS__
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
			}
			finally {
				device.BatteryMonitoringEnabled = false;
			}
		}
#endif // !__TVOS__ && !__WATCHOS__
	}
}
#endif