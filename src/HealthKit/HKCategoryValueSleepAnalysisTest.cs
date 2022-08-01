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
			var sleepValues = GetAsleepValues ();
			Assert.IsNull (sleepValues, "Asleep Values should not actually return null - but let's see for testing");
		}
	}
}
#endif // HAS_HEALTHKIT
