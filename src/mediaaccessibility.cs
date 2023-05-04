using System;

using ObjCRuntime;
using Foundation;

namespace MediaAccessibility {

#if NET
	[Static]
	interface MACaptionAppearance {
		[Mac (10,9)]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("kMACaptionAppearanceSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}
#endif

	[Static]
	interface MAAudibleMedia {
		[iOS (8, 0)]
		[Mac (10, 10)]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("kMAAudibleMediaSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}

	[Static]
	interface MAMediaCharacteristic {
		[Mac (10, 9)]
		[MacCatalyst (13, 1)]
		[Field ("MAMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		NSString DescribesMusicAndSoundForAccessibility { get; }

		[iOS (8, 0)]
		[Mac (10, 10)]
		[MacCatalyst (13, 1)]
		[Field ("MAMediaCharacteristicDescribesVideoForAccessibility")]
		NSString DescribesVideoForAccessibility { get; }

		[Mac (10, 9)]
		[MacCatalyst (13, 1)]
		[Field ("MAMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		NSString TranscribesSpokenDialogForAccessibility { get; }
	}

	[Static]
	interface MAVideoAccommodations {
		[Mac (13, 3), TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
		[Notification]
		[Field ("kMADimFlashingLightsChangedNotification")]
		NSString DimFlashingLightsChangedNotification { get; }
	}
}
