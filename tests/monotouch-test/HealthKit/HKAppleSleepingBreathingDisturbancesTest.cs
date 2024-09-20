#if HAS_HEALTHKIT

using System;

using Foundation;
using HealthKit;

using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HKAppleSleepingBreathingDisturbancesTest {
		[Test]
		public void RoundtripTest ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var minimum = HKAppleSleepingBreathingDisturbances.GetMinimumQuantity (HKAppleSleepingBreathingDisturbancesClassification.Elevated);
			var classification = HKAppleSleepingBreathingDisturbances.GetClassification (minimum);
			Assert.AreEqual (HKAppleSleepingBreathingDisturbancesClassification.Elevated, classification, "Classification");
		}
	}
}

#endif // HAS_HEALTHKIT
