using System;
using Foundation;
using CoreMedia;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMClockOrTimebaseTest {

		[Test]
		public void RetainReleaseTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var clock = CMClock.HostTimeClock;
			var timebase = Runtime.GetINativeObject<CMClockOrTimebase> (clock.Handle, false);
			// we should be able to dispose the clock and the timebase with no crashes.
			Assert.AreEqual (clock.Handle, timebase.Handle);
			clock.Dispose ();
			timebase.Dispose ();
		}

	}
}
