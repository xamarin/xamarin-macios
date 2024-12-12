#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace HealthKit {
	/// <summary>This class contains helper functions for the <see cref="HKStateOfMindValenceClassification" /> enum.</summary>
#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Watch (11, 0), NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	public static class HKStateOfMindValence {
		[DllImport (Constants.HealthKitLibrary)]
		static extern IntPtr HKStateOfMindValenceClassificationForValence (double valence);

		/// <summary>Gets the valence classification appropriate for a given valence value.</summary>
		/// <param name="valence">The valence value whose classification to get.</param>
		/// <returns>The valence classification, or null if the specified valence is outside of the supported range of valence values.</returns>
		public static HKStateOfMindValenceClassification? GetClassification (double valence)
		{
			var nsnumber = Runtime.GetNSObject<NSNumber> (HKStateOfMindValenceClassificationForValence (valence), owns: false);
			if (nsnumber is null)
				return null;
			return (HKStateOfMindValenceClassification) (long) nsnumber.LongValue;
		}
	}
}
