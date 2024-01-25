//
// Unit tests for HKErrorCode
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if HAS_HEALTHKIT

using System;

using Foundation;

using HealthKit;

using NUnit.Framework;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ErrorTest {

		[Test]
		public void Domain ()
		{
			// the enum exists but we can't load the domain before iOS8
#if MONOMAC
			TestRuntime.AssertXcodeVersion (14, 0);
#else
			TestRuntime.AssertXcodeVersion (6, 0);
#endif

			Assert.That (HKErrorCode.NoError.GetDomain ().ToString (), Is.EqualTo ("com.apple.healthkit"), "Domain");
		}
	}
}

#endif // HAS_HEALTHKIT
