
#if !__WATCHOS__

using System;

using Foundation;
using Metal;
using ObjCRuntime;

using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DeviceTest {

		[Test]
		public void System ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			var d = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (d is null)
				Assert.Inconclusive ("Metal is not supported");

			// if we get an instance it must be valid, i.e. not an empty wrapper
			Assert.That (d.Handle, Is.Not.EqualTo (NativeHandle.Zero), "Handle");

			// and if we ask again we need to get a valid instance again
			d.Dispose ();
			Assert.That (d.Handle, Is.EqualTo (NativeHandle.Zero), "Disposed");

			d = MTLDevice.SystemDefault;
			Assert.That (d.Handle, Is.Not.EqualTo (NativeHandle.Zero), "Handle-2");
		}
	}
}

#endif // !__WATCHOS__
