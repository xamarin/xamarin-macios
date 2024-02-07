using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using HKQuantityRef = System.IntPtr;
using NSErrorRef = System.IntPtr;

#nullable enable

namespace HealthKit {

#if NET
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[SupportedOSPlatform ("macos13.0")]
#else
	[Watch (8, 0)]
	[iOS (15, 0)]
	[Mac (13, 0)]
#endif
	public static class HKAppleWalkingSteadiness {

		[DllImport (Constants.HealthKitLibrary)]
		unsafe static extern byte HKAppleWalkingSteadinessClassificationForQuantity (HKQuantityRef value, nint* classificationOut, NSErrorRef* errorOut);

		public static bool TryGetClassification (HKQuantity value, out HKAppleWalkingSteadinessClassification? classification, out NSError? error)
		{
			nint classificationOut;
			NSErrorRef errorPtr;

			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			classification = null;

			byte rv;
			unsafe {
				rv = HKAppleWalkingSteadinessClassificationForQuantity (value.GetHandle (), &classificationOut, &errorPtr);
			}

			error = Runtime.GetNSObject<NSError> (errorPtr, false);

			if (rv != 0) {
				classification = (HKAppleWalkingSteadinessClassification) (long) classificationOut;
				return true;
			}
			return false;

		}

		[DllImport (Constants.HealthKitLibrary)]
		static extern HKQuantityRef HKAppleWalkingSteadinessMinimumQuantityForClassification (nint classification);

		public static HKQuantity? GetMinimumQuantity (HKAppleWalkingSteadinessClassification classification)
			=> Runtime.GetNSObject<HKQuantity> (HKAppleWalkingSteadinessMinimumQuantityForClassification ((nint) (long) classification), false);

		[DllImport (Constants.HealthKitLibrary)]
		static extern HKQuantityRef HKAppleWalkingSteadinessMaximumQuantityForClassification (nint classification);

		public static HKQuantity? GetMaximumQuantity (HKAppleWalkingSteadinessClassification classification)
			=> Runtime.GetNSObject<HKQuantity> (HKAppleWalkingSteadinessMaximumQuantityForClassification ((nint) (long) classification), false);
	}

}
