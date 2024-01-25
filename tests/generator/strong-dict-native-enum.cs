using System;

using Foundation;

using ObjCRuntime;

using Vision;

namespace StrongDictsNativeEnums {

	[Static]
	interface SomeKeys {
		[Field ("NSSomeNativeKey", "__Internal")]
		NSString TrackingLevelKey { get; }
	}

	[StrongDictionary ("SomeKeys")]
	interface SomeOptions {
		VNRequestTrackingLevel TrackingLevel { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface UseOptions {
		[Export ("setOptions:")]
		void SetOptions (NSDictionary weakOptions);

		[Wrap ("SetOptions (options?.Dictionary)")]
		void SetOptions (SomeOptions options);
	}
}
