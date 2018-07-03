//
// MAAudibleMedia Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using MediaAccessibility;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaAccessibility;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaAccessibility {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class AudibleMediaTest {

		[Test]
		public void PreferredCharacteristics ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);

			if (TestRuntime.CheckXcodeVersion (7, 0)) {
				Assert.NotNull (MAAudibleMedia.GetPreferredCharacteristics ());
			} else {
				Assert.Null (MAAudibleMedia.GetPreferredCharacteristics ());
			}
		}
	}
}

#endif // !__WATCHOS__
