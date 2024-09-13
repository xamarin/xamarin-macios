using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using HKQuantityRef = System.IntPtr;
using NSErrorRef = System.IntPtr;

#nullable enable

namespace HealthKit {

	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
	public static class HKAppleSleepingBreathingDisturbances {

		[DllImport (Constants.HealthKitLibrary)]
		unsafe static extern /* NSNumber * _Nullable */ IntPtr HKAppleSleepingBreathingDisturbancesClassificationForQuantity (HKQuantityRef value);

		/// <summary>Get the breathing disturbances classification for a given quantity of breathing disturbance.</summary>
		/// <param name="value">The quantity of the breathing disturbance whose classification to get.</param>
		/// <returns>The breathing disturbances classification for the specified breathing disturbance quantity.</returns>
		public static HKAppleSleepingBreathingDisturbancesClassification? GetClassification (HKQuantity value)
		{
			var ptr = HKAppleSleepingBreathingDisturbancesClassificationForQuantity (value.GetHandle ());
			var number = Runtime.GetNSObject<NSNumber> (ptr);
			if (number is null)
				return null;
			return (HKAppleSleepingBreathingDisturbancesClassification) number.LongValue;
		}

		[DllImport (Constants.HealthKitLibrary)]
		static extern HKQuantityRef HKAppleSleepingBreathingDisturbancesMinimumQuantityForClassification (nint /* HKAppleSleepingBreathingDisturbancesClassification */ classification);

		/// <summary>Get the minimum quantity for a breathing disturbances classification.</summary>
		/// <param name="classification">The classification to get the minimum quantity for.</param>
		/// <returns>The minimum quantity for the specified breathing disturbances classification.</returns>
		public static HKQuantity? GetMinimumQuantity (HKAppleSleepingBreathingDisturbancesClassification classification)
		{
			return Runtime.GetNSObject<HKQuantity> (HKAppleSleepingBreathingDisturbancesMinimumQuantityForClassification ((nint) (long) classification));
		}
	}
}
