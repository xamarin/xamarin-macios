
#if !__WATCHOS__

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	
	[TestFixture]
	public class DeviceTest {
		
		[Test]
		public void System ()
		{
			TestRuntime.AssertXcodeVersion (6,0);

			var d = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (d == null)
				Assert.Inconclusive ("Metal is not supported");

			// if we get an instance it must be valid, i.e. not an empty wrapper
			Assert.That (d.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");

			// and if we ask again we need to get a valid instance again
			d.Dispose ();
			Assert.That (d.Handle, Is.EqualTo (IntPtr.Zero), "Disposed");

			d = MTLDevice.SystemDefault;
			Assert.That (d.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-2");
		}
	}
}

#endif // !__WATCHOS__
