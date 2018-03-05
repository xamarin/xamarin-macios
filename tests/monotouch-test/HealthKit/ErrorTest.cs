//
// Unit tests for HKErrorCode
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;

#if XAMCORE_2_0
using Foundation;
using HealthKit;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.HealthKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ErrorTest {

		[Test]
		public void Domain ()
		{
			// the enum exists but we can't load the domain before iOS8
			TestRuntime.AssertXcodeVersion (6, 0);
		
			Assert.That (HKErrorCode.NoError.GetDomain ().ToString (), Is.EqualTo ("com.apple.healthkit"), "Domain");
		}
	}
}

#endif // __TVOS__
