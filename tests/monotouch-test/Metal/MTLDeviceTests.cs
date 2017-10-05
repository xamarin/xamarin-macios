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

			Assert.That (devices, Is.Not.Empty, "MTLDevice.GetAllDevices");

			Assert.DoesNotThrow (() => {
				MTLDevice.RemoveObserver (refObj);
			});
		}
	}
}
#endif
