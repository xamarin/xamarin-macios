#if MONOMAC

using System;
using Foundation;

#if XAMCORE_2_0
using Metal;
#else
using MonoTouch.Metal;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	public class MTLDeviceTests {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void GetAllDevicesTest ()
		{
			NSObject refObj = new NSObject();
			var devices = MTLDevice.GetAllDevices(ref refObj, (IMTLDevice device, NSString notifyName) => { });

#if __MACOS__
			// It's possible to run on a system that does not support metal,
			// in which case we'll get an empty array of devices.
			Assert.IsNotNull (devices, "MTLDevices.GetAllDevices not null");
#else
			Assert.That (devices, Is.Not.Empty, "MTLDevice.GetAllDevices");
#endif

			Assert.DoesNotThrow (() => {
				MTLDevice.RemoveObserver (refObj);
			});
		}
	}
}
#endif
