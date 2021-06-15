using System;

namespace ObjCRuntime {

	// we use this to avoid multiple similar strings for the same purpose
	// which reduce the size of the metadata inside our platform assemblies
	// once adopted everywhere then updating  strings will be much easier
	partial class Constants {

		internal const string UseCallKitInstead = "Use the 'CallKit' API instead.";

		internal const string WatchKitRemoved = "The WatchKit framework has been removed from iOS.";

		internal const string UnavailableOniOS = "This type is not available on iOS.";

		internal const string UnavailableOnWatchOS = "This type is not available on watchOS.";

		internal const string MacOS32bitsUnavailable = "This framework is not available on 64bits macOS versions.";

		internal const string iAdRemoved = "The iAd framework has been removed from iOS.";
	}
}
