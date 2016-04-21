using System;

using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.MediaAccessibility {

#if XAMCORE_4_0
	[Static]
	public interface MACaptionAppearance {
		[iOS (7,0)][Mac (10,9)]
		[Notification]
		[Field ("kMACaptionAppearanceSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}
#endif

	[Static]
	public interface MAAudibleMedia {
		[iOS (8,0)][Mac (10,10)]
		[Notification]
		[Field ("kMAAudibleMediaSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}

	[Static]
	public interface MAMediaCharacteristic {
		[iOS (7,0)][Mac (10,9)]
		[Field ("MAMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		NSString DescribesMusicAndSoundForAccessibility { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("MAMediaCharacteristicDescribesVideoForAccessibility")]
		NSString DescribesVideoForAccessibility { get; }

		[iOS (7,0)][Mac (10,9)]
		[Field ("MAMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		NSString TranscribesSpokenDialogForAccessibility { get; }
	}
}