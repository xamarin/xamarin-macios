using System;

using ObjCRuntime;

#nullable enable

namespace MapKit {

#if !WATCH || (WATCH && !NET)

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
#endif
#if WATCH && !NET
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
