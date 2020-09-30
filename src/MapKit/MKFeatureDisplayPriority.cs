using System;
using ObjCRuntime;

namespace MapKit {

#if !WATCH || (WATCH && !XAMCORE_4_0)

	// .net does not allow float-based enumerations
	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
#if WATCH && !XAMCORE_4_0
	[Obsolete ("This API is not available on this platform.")]
#endif
	public static class MKFeatureDisplayPriority {
		public const float Required = 1000f;
		public const float DefaultHigh = 750f;
		public const float DefaultLow = 250f;
	}
#endif
}
