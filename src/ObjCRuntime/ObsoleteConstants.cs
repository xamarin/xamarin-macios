using System;

namespace ObjCRuntime {

	// we use this to avoid multiple similar strings for the same purpose
	// which reduce the size of the metadata inside our platform assemblies
	// once adopted everywhere then updating  strings will be much easier
	partial class Constants {

		internal const string UseCallKitInstead = "Use the 'CallKit' API instead.";

		internal const string UseNetworkInstead = "Use 'Network.framework' instead.";

		internal const string WatchKitRemoved = "The WatchKit framework has been removed from iOS.";

		internal const string UnavailableOniOS = "This type is not available on iOS.";

		internal const string UnavailableOnWatchOS = "This type is not available on watchOS.";

		internal const string MacOS32bitsUnavailable = "This framework is not available on 64bits macOS versions.";

		internal const string iAdRemoved = "The iAd framework has been removed from iOS.";

		internal const string UnavailableOnMacOS = "This type is not available on macOS.";

		internal const string UnavailableOnThisPlatform = "This type is not available on this Platform.";

		internal const string RemovedFromChip = "This API has been removed from the 'CHIP' framework.";

		internal const string TypeUnavailable = "This type has been removed from the current platform.";

		internal const string ChipRemoved = "The CHIP framework has been removed from iOS.";

		internal const string ApiRemovedGeneral = "This API has been removed from the framework.";

		internal const string RemovedFromHomeKit = "This API has been removed from the 'HomeKit' framework.";

		internal const string BrokenBinding = "This API was incorrectly bound and does not work correctly. Use the new version indicated in the associated Obsolete attribute.";

		internal const string RemovedFromPassKit = "This API has been removed from the 'PassKit' framework.";

		internal const string NewsstandKitRemoved = "The NewsstandKit framework has been removed from iOS.";

		internal const string AssetsLibraryRemoved = "The AssetsLibrary framework has been removed from iOS, use the 'Photos' API instead.";
	}
}
