#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreMedia;
#else
using MonoTouch.CoreMedia;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia
{

	[TestFixture]
	[Preserve(AllMembers = true)]
	public class CMClockOrTimebaseTest
	{

		[Test]
		public void RetainReleaseTest ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			var clock = CMClock.HostTimeClock;
			var timebase = new CMClockOrTimebase (clock.Handle);
			// we should be able to dispose the clock and the timebase with no crashes.
			Assert.AreEqual (clock.Handle, timebase.Handle);
			clock.Dispose ();
			timebase.Dispose ();
		}

	}
}

#endif // !__WATCHOS__