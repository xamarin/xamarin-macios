#nullable enable

using System;

namespace HealthKit {
	public partial class HKSampleQuery {
		// #define HKObjectQueryNoLimit (0)
		// in iOS 9.3 SDK this was changed to `static const NSUInteger`
		public const int NoLimit = 0;
	}
}
