using System;

using ObjCRuntime;
using Foundation;
using Surface = IOSurface.IOSurface;

namespace MediaAccessibility {

#if NET
	/// <summary>Class that contains static methods and fields for accessing and controlling information about the appearance and language of accessibility features on the device.</summary>
	[Static]
	interface MACaptionAppearance {
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("kMACaptionAppearanceSettingsChangedNotification")]
		NSString SettingsChangedNotification { get; }
	}
#endif

	/// <summary>Defines the constant associated with <c>kMAudibleMediaSettingsChangedNotification</c> and accessibility preferred characteristics.</summary>
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

	delegate void MAMusicHapticTrackAvailabilityCallback (bool musicHapticsAvailable);
	delegate void MAMusicHapticTrackStatusObserver (string internationalStandardRecordingCode, bool musicHapticsActive);

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MAMusicHapticsManager {
		[Static]
		[Export ("sharedManager")]
		MAMusicHapticsManager SharedManager { get; }

		[Export ("isActive")]
		bool IsActive { get; }

		[Async]
		[Export ("checkHapticTrackAvailabilityForMediaMatchingCode:completionHandler:")]
		void CheckHapticTrackAvailability (string internationalStandardRecordingCode, [NullAllowed] MAMusicHapticTrackAvailabilityCallback completionHandler);

		[Export ("addStatusObserver:")]
		[return: NullAllowed]
		INSCopying AddStatusObserver (MAMusicHapticTrackStatusObserver statusHandler);

		[Export ("removeStatusObserver:")]
		void RemoveStatusObserver (INSCopying registrationToken);

		[Notification]
		[Field ("MAMusicHapticsManagerActiveStatusDidChangeNotification")]
		NSString ActiveStatusDidChangeNotification { get; }
	}
}
