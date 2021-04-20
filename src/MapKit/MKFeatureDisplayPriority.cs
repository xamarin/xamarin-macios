using System;
using System.Runtime.Versioning;
using ObjCRuntime;

#nullable enable

namespace MapKit {

#if !WATCH || (WATCH && !XAMCORE_4_0)

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#else
	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13)]
#endif
#if WATCH && !XAMCORE_4_0
	[Obsolete ("This API is not available on this platform.")]
#endif
	// .net does not allow float-based enumerations
	public static class MKFeatureDisplayPriority {
		public const float Required = 1000f;
		public const float DefaultHigh = 750f;
		public const float DefaultLow = 250f;
	}
#endif
}
