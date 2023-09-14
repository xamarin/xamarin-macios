using System;

using ObjCRuntime;
using Foundation;
using Surface = IOSurface.IOSurface;

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

	[Static]
	interface MAVideoAccommodations {
		[Mac (13, 3), TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
		[Notification]
		[Field ("kMADimFlashingLightsChangedNotification")]
		NSString DimFlashingLightsChangedNotification { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface MAFlashingLightsProcessorResult {
		[Export ("surfaceProcessed")]
		bool SurfaceProcessed { get; }

		[Export ("mitigationLevel")]
		float MitigationLevel { get; }

		[Export ("intensityLevel")]
		float IntensityLevel { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface MAFlashingLightsProcessor {
		[Export ("canProcessSurface:")]
		bool CanProcess (Surface surface);

		[Export ("processSurface:outSurface:timestamp:options:")]
		MAFlashingLightsProcessorResult Process (Surface inSurface, Surface outSurface, double timestamp, [NullAllowed] NSDictionary options);
	}
}
