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
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Ignore ("requires iOS8+");

			if (TestRuntime.CheckSystemAndSDKVersion (9, 0)) {
				Assert.NotNull (MAAudibleMedia.GetPreferredCharacteristics ());
			} else {
				Assert.Null (MAAudibleMedia.GetPreferredCharacteristics ());
			}
		}
	}
}

#endif // !__WATCHOS__
