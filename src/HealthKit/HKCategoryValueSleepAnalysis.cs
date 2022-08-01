// HKCategoryValueSleepAnalysis

//
// HKCategoryValueSleepAnalysis.cs:
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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace HealthKit
{
	public partial class HKCategoryValueSleepAnalysis {

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16,0), Mac (13,0), Watch (9,0), NoTV, MacCatalyst (16,0)]
#endif // NET
		[DllImport (Constants.HealthKitLibrary)]
		static extern /* HKCategoryValueSleepAnalysis */ NativeHandle HKCategoryValueSleepAnalysisAsleepValues ();

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16,0), Mac (13,0), Watch (9,0), NoTV, MacCatalyst (16,0)]
#endif // NET
		public static HashSet<HKCategoryValueSleepAnalysis> GetAsleepValues ()
		{
			return Runtime.GetNSObject<HashSet<HKCategoryValueSleepAnalysis>> (HKCategoryValueSleepAnalysisAsleepValues);
		}
	}
}

#endif // !TVOS
