//
// Unit tests for HKCategoryValueSleepAnalysis
//
// Authors:
//	TJ Lambert  <TJ.Lambert@microsoft.com>
//
// Copyright 2022 Xamarin Inc. All rights reserved.
//

#if HAS_HEALTHKIT

using System;

using Foundation;

using HealthKit;

using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HKCategoryValueSleepAnalysisTest {

		[Test]
		public void GetAsleepValuesTest ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);

			var sleepValues = HKCategoryValueSleepAnalysisAsleep.GetAsleepValues ();
			Assert.IsNotNull (sleepValues, "Asleep Values should not return null.");
		}
	}
}
#endif // HAS_HEALTHKIT
