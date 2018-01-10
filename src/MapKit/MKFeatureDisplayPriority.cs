#if XAMCORE_2_0 || !MONOMAC
using System;
using ObjCRuntime;

namespace MapKit {

	// .net does not allow float-based enumerations
	[TV (11,0)][NoWatch][iOS (11,0)][Mac (10,13, onlyOn64: true)]
	public static class MKFeatureDisplayPriority {
		public const float Required = 1000f;
		public const float DefaultHigh = 750f;
		public const float DefaultLow = 250f;
	}
}

#endif