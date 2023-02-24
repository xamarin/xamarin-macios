using System;

using ObjCRuntime;
using Foundation;

namespace MediaAccessibility {

#if NET
	[Static]
	interface MACaptionAppearance {
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("kMACaptionAppearanceSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}
#endif

	[Static]
	interface MAAudibleMedia {
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("kMAAudibleMediaSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}

	[Static]
	interface MAMediaCharacteristic {
		[MacCatalyst (13, 1)]
		[Field ("MAMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		NSString DescribesMusicAndSoundForAccessibility { get; }

		[MacCatalyst (13, 1)]
		[Field ("MAMediaCharacteristicDescribesVideoForAccessibility")]
		NSString DescribesVideoForAccessibility { get; }

		[MacCatalyst (13, 1)]
		[Field ("MAMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		NSString TranscribesSpokenDialogForAccessibility { get; }
	}
}
