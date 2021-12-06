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
using Foundation;
using MediaAccessibility;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MediaAccessibility {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class AudibleMediaTest {

		[Test]
		public void PreferredCharacteristics ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			if (TestRuntime.CheckXcodeVersion (7, 0)) {
				Assert.NotNull (MAAudibleMedia.GetPreferredCharacteristics ());
			} else {
				Assert.Null (MAAudibleMedia.GetPreferredCharacteristics ());
			}
		}
	}
}

#endif // !__WATCHOS__
