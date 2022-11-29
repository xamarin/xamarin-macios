// HKCategoryValueSleepAnalysisAsleep

//
// HKCategoryValueSleepAnalysisAsleep.cs:
//
// Authors:
//  TJ Lambert (TJ.Lambert@microsoft.com
//
// Copyright 2022 Microsoft Corp.
//

#nullable enable

#if !TVOS

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace HealthKit {
	public partial class HKCategoryValueSleepAnalysisAsleep {

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
#endif // NET
		[DllImport (Constants.HealthKitLibrary)]
		static extern NativeHandle HKCategoryValueSleepAnalysisAsleepValues ();

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
#endif // NET
		public static HashSet<HKCategoryValueSleepAnalysis> GetAsleepValues ()
		{
			using var values = Runtime.GetNSObject<NSSet<NSNumber>> (HKCategoryValueSleepAnalysisAsleepValues ())!;
			var hashSet = new HashSet<HKCategoryValueSleepAnalysis> ();
			foreach (NSNumber value in values) {
				hashSet.Add ((HKCategoryValueSleepAnalysis) (int) value);
			}
			return hashSet;
		}
	}
}

#endif // !TVOS
