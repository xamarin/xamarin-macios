//
// Unit tests for CMClock
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using CoreMedia;
using Foundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.CoreMedia;
using MonoTouch.UIKit;
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMClockTest {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

#if !MONOMAC // The CMAudioClockCreate API is only available on iOS
		[Test]
		public void CreateAudioClock ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("CMClock is new in 6.0");

			CMClockError ce;
			using (var clock = CMClock.CreateAudioClock (out ce))
			{
				Assert.AreEqual (CMClockError.None, ce);
			}
		}
#endif

		[Test]
		public void HostTimeClock ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			using (var clock = CMClock.HostTimeClock) {
				Assert.That (clock.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (CFGetRetainCount (clock.Handle), Is.GreaterThanOrEqualTo ((nint) 1), "RetainCount");
			}
		}
	}
}

#endif // !__WATCHOS__
