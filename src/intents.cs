//
// Intents bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using CoreLocation;
using UserNotifications;

#if TVOS
using CNPostalAddress = Foundation.NSObject;
using EKRecurrenceRule = Foundation.NSObject;
#else
using Contacts;
using EventKit;
#endif

#if MONOMAC
using AppKit;
using UIImage = Foundation.NSObject;
#else
using UIKit;
using NSImage = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Intents {

	// HACK only to please the generator - which does not (normally) know the type hierarchy in the
	// binding files. The lack of namespace will generate the correct code for the C# compiler
	// this is used for NSMeasurement <UnitType> objects.
	interface NSUnitTemperature : NSUnit { }
	interface NSUnitVolume : NSUnit { }
	interface NSUnitSpeed : NSUnit { }
	interface NSUnitEnergy : NSUnit { }
	interface NSUnitMass : NSUnit { }
	interface NSUnitPower : NSUnit { }

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INBookRestaurantReservationIntentCode : long {
		Success = 0,
		Denied,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INCallCapabilityOptions : ulong {
		AudioCall = (1 << 0),
		VideoCall = (1 << 1)
	}

	[Mac (12, 0)] // used in interface with new Mac (12,0) avilability
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallRecordType : long {
		Unknown = 0,
		Outgoing,
		Missed,
		Received,
		[MacCatalyst (13, 1)]
		Latest,
		[MacCatalyst (13, 1)]
		Voicemail,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Ringing,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		InProgress,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		OnHold,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCancelWorkoutIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HandleInApp' instead.")] // yup just iOS
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HandleInApp' instead.")]
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 7, which is now defined as 'Success').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		Success,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarAirCirculationMode bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INCarAirCirculationMode : long {
		Unknown = 0,
		FreshAir,
		RecirculateAir
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarAudioSource bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INCarAudioSource : long {
		Unknown = 0,
		CarPlay,
		iPod,
		Radio,
		Bluetooth,
		Aux,
		Usb,
		MemoryCard,
		OpticalDrive,
		HardDrive
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarDefroster bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INCarDefroster : long {
		Unknown = 0,
		Front,
		Rear,
		All,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarSeat bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INCarSeat : long {
		Unknown = 0,
		Driver,
		Passenger,
		FrontLeft,
		FrontRight,
		Front,
		RearLeft,
		RearRight,
		Rear,
		ThirdRowLeft,
		ThirdRowRight,
		ThirdRow,
		All
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INConditionalOperator : long {
		All = 0,
		Any,
		None
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INEndWorkoutIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HandleInApp' instead.")] // yup just iOS
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HandleInApp' instead.")]
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 7, which is now defined as 'Success').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		Success,
	}

	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode : long {
		Success,
		Failure,
		Unspecified
	}

	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetAvailableRestaurantReservationBookingsIntentCode : long {
		Success,
		Failure,
		FailureRequestUnsatisfiable,
		FailureRequestUnspecified
	}

	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetRestaurantGuestIntentResponseCode : long {
		Success,
		Failure
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetRideStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable
	}

	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetUserCurrentRestaurantReservationBookingsIntentResponseCode : long {
		Success,
		Failure,
		FailureRequestUnsatisfiable,
		Unspecified
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("INIntentErrorDomain")]
	public enum INIntentErrorCode : long {
		InteractionOperationNotSupported = 1900,
		DonatingInteraction = 1901,
		DeletingAllInteractions = 1902,
		DeletingInteractionWithIdentifiers = 1903,
		DeletingInteractionWithGroupIdentifier = 1904,
		IntentSupportedByMultipleExtension = 2001,
		RestrictedIntentsNotSupportedByExtension = 2002,
		NoHandlerProvidedForIntent = 2003,
		InvalidIntentName = 2004,
		NoAppAvailable = 2005,
		RequestTimedOut = 3001,
		MissingInformation = 3002,
		InvalidUserVocabularyFileLocation = 4000,
		ExtensionLaunchingTimeout = 5000,
		ExtensionBringUpFailed = 5001,
		ImageGeneric = 6000,
		ImageNoServiceAvailable = 6001,
		ImageStorageFailed = 6002,
		ImageLoadingFailed = 6003,
		ImageRetrievalFailed = 6004,
		ImageProxyLoop = 6005,
		ImageProxyInvalid = 6006,
		ImageProxyTimeout = 6007,
		ImageServiceFailure = 6008,
		ImageScalingFailed = 6009,
		PermissionDenied = 6010,
		VoiceShortcutCreationFailed = 7000,
		VoiceShortcutGetFailed = 7001,
		VoiceShortcutDeleteFailed = 7002,
		EncodingGeneric = 8000,
		EncodingFailed = 8001,
		DecodingGeneric = 9000,
		UnableToCreateAppIntentRepresentation = 10000,
		NoAppIntent = 10001,
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INIntentHandlingStatus : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		DeferredToApplication,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		UserConfirmationRequired,
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INInteractionDirection : long {
		Unspecified = 0,
		Outgoing,
		Incoming
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INListRideOptionsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchNoServiceInArea,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable,
		FailureRequiringAppLaunchPreviousRideNeedsCompletion,
		[MacCatalyst (13, 1)]
		FailurePreviousRideNeedsFeedback,
	}

#if NET
	[NoMac]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMessageAttribute : long {
		Unknown = 0,
		Read,
		Unread,
		Flagged,
		Unflagged,
#if NET
		[NoMac]
#endif
		[MacCatalyst (13, 1)]
		Played,
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INMessageAttributeOptions : ulong {
		Read = (1 << 0),
		Unread = (1 << 1),
		Flagged = (1 << 2),
		Unflagged = (1 << 3),
#if NET
		[NoMac]
#endif
		[MacCatalyst (13, 1)]
		Played = (1UL << 4),
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPauseWorkoutIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HandleInApp' instead.")] // yup just iOS
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HandleInApp' instead.")]
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 7, which is now defined as 'Success').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		Success,
	}

	[NoTV, Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPaymentMethodType : long {
		Unknown = 0,
		Checking,
		Savings,
		Brokerage,
		Debit,
		Credit,
		Prepaid,
		Store,
		ApplePay
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPaymentStatus : long {
		Unknown = 0,
		Pending,
		Completed,
		Canceled,
		Failed,
		Unpaid
	}

	[Mac (11, 0), NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPersonSuggestionType : long {
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		None = 0,
		SocialProfile = 1,
		InstantMessageAddress,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INPhotoAttributeOptions bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	[Flags]
	public enum INPhotoAttributeOptions : ulong {
		Photo = (1 << 0),
		Video = (1 << 1),
		Gif = (1 << 2),
		Flash = (1 << 3),
		LandscapeOrientation = (1 << 4),
		PortraitOrientation = (1 << 5),
		Favorite = (1 << 6),
		Selfie = (1 << 7),
		FrontFacingCamera = (1 << 8),
		Screenshot = (1 << 9),
		BurstPhoto = (1 << 10),
		HdrPhoto = (1 << 11),
		SquarePhoto = (1 << 12),
		PanoramaPhoto = (1 << 13),
		TimeLapseVideo = (1 << 14),
		SlowMotionVideo = (1 << 15),
		NoirFilter = (1 << 16),
		ChromeFilter = (1 << 17),
		InstantFilter = (1 << 18),
		TonalFilter = (1 << 19),
		TransferFilter = (1 << 20),
		MonoFilter = (1 << 21),
		FadeFilter = (1 << 22),
		ProcessFilter = (1 << 23),
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		PortraitPhoto = (1uL << 24),
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		LivePhoto = (1uL << 25),
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		LoopPhoto = (1uL << 26),
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		BouncePhoto = (1uL << 27),
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		LongExposurePhoto = (1uL << 28),
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INRadioType bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INRadioType : long {
		Unknown = 0,
		AM,
		FM,
		HD,
		Satellite,
		Dab
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INRelativeReference : long {
		Unknown = 0,
		Next,
		Previous
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INRelativeSetting : long {
		Unknown = 0,
		Lowest,
		Lower,
		Higher,
		Highest
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestPaymentIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailurePaymentsAmountBelowMinimum,
		FailurePaymentsAmountAboveMaximum,
		FailurePaymentsCurrencyUnsupported,
		FailureNoBankAccount,
		[NoMac]
		[MacCatalyst (13, 1)]
		FailureNotEligible,
		[Watch (4, 1)]
		[NoMac]
		[MacCatalyst (13, 1)]
		FailureTermsAndConditionsAcceptanceRequired,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestRideIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchNoServiceInArea,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable,
		FailureRequiringAppLaunchPreviousRideNeedsCompletion,
		[iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		FailureRequiringAppLaunchRideScheduledTooFar,
	}

	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRestaurantReservationUserBookingStatus : ulong {
		Pending,
		Confirmed,
		Denied
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INResumeWorkoutIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HandleInApp' instead.")] // yup just iOS
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HandleInApp' instead.")]
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 7, which is now defined as 'Success').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		Success,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRidePhase : long {
		Unknown = 0,
		Received,
		Confirmed,
		Ongoing,
		Completed,
		ApproachingPickup,
		Pickup
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSaveProfileInCarIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSearchCallHistoryIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[MacCatalyst (13, 1)]
		FailureAppConfigurationRequired,
		[MacCatalyst (13, 1)]
		InProgress,
		[MacCatalyst (13, 1)]
		Success,
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSearchForMessagesIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageServiceNotAvailable,
		[MacCatalyst (13, 1)]
		FailureMessageTooManyResults,
		[iOS (17, 0), MacCatalyst (17, 0)]
		FailureRequiringInAppAuthentication,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSearchForPhotosIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[MacCatalyst (13, 1)]
		FailureAppConfigurationRequired,
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendMessageIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageServiceNotAvailable,
		FailureRequiringInAppAuthentication,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendPaymentIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailurePaymentsAmountBelowMinimum,
		FailurePaymentsAmountAboveMaximum,
		FailurePaymentsCurrencyUnsupported,
		FailureInsufficientFunds,
		FailureNoBankAccount,
		[MacCatalyst (13, 1)]
		FailureNotEligible,
		[Watch (4, 1)]
		[MacCatalyst (13, 1)]
		FailureTermsAndConditionsAcceptanceRequired,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSetAudioSourceInCarIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSetClimateSettingsInCarIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSetDefrosterSettingsInCarIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

#if NET
	[NoMac]
#endif
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSetMessageAttributeIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageNotFound,
		FailureMessageAttributeNotSet
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSetProfileInCarIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSetRadioStationIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureNotSubscribed
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSetSeatSettingsInCarIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSiriAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponseCode' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntentResponseCode' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentResponseCode' instead.")]
	[Native]
	public enum INStartAudioCallIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[MacCatalyst (13, 1)]
		FailureAppConfigurationRequired,
		[MacCatalyst (13, 1)]
		FailureCallingServiceNotAvailable,
		[MacCatalyst (13, 1)]
		FailureContactNotSupportedByApp,
		[MacCatalyst (13, 1)]
		FailureNoValidNumber,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INStartPhotoPlaybackIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[MacCatalyst (13, 1)]
		FailureAppConfigurationRequired,
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponseCode' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntentResponseCode' instead.")]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentResponseCode' instead.")]
	[Native]
	public enum INStartVideoCallIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[MacCatalyst (13, 1)]
		FailureAppConfigurationRequired,
		[MacCatalyst (13, 1)]
		FailureCallingServiceNotAvailable,
		[MacCatalyst (13, 1)]
		FailureContactNotSupportedByApp,
		[MacCatalyst (13, 1)]
		FailureInvalidNumber,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INStartWorkoutIntentResponseCode : long {
		Unspecified = 0,
		Ready = 1,
		ContinueInApp = 2,
		Failure = 3,
		FailureRequiringAppLaunch = 4,
		FailureOngoingWorkout = 5,
		FailureNoMatchingWorkout = 6,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 7, which is now defined as 'Success').")]
		[MacCatalyst (13, 1)]
#if XAMCORE_5_0 && !NET
		[NoWatch]
#endif
		HandleInApp = 7,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[MacCatalyst (13, 1)]
#if XAMCORE_5_0 && !NET
		[NoWatch]
#endif
		Success = 8,
	}

	[Unavailable (PlatformName.MacOSX)]
	[Watch (6, 0)]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INVocabularyStringType : long {
		ContactName = 1,
		ContactGroupName,
		PhotoTag = 100,
		PhotoAlbumName,
		WorkoutActivityName = 200,
		CarProfileName = 300,
		[MacCatalyst (13, 1)]
		CarName,
		[MacCatalyst (13, 1)]
		PaymentsOrganizationName = 400,
		[MacCatalyst (13, 1)]
		PaymentsAccountNickname,
		[MacCatalyst (13, 1)]
		NotebookItemTitle = 500,
		[MacCatalyst (13, 1)]
		NotebookItemGroupName,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		MediaPlaylistTitle = 700,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		MediaMusicArtistName,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		MediaAudiobookTitle,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		MediaAudiobookAuthorName,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		MediaShowTitle,
	}

	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INWorkoutGoalUnitType bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INWorkoutGoalUnitType : long {
		Unknown = 0,
		Inch,
		Meter,
		Foot,
		Mile,
		Yard,
		Second,
		Minute,
		Hour,
		Joule,
		KiloCalorie
	}

	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INWorkoutLocationType bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INWorkoutLocationType : long {
		Unknown = 0,
		Outdoor,
		Indoor
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPersonHandleType : long {
		Unknown = 0,
		EmailAddress,
		PhoneNumber
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAccountType : long {
		Unknown = 0,
		Checking,
		Credit,
		Debit,
		Investment,
		Mortgage,
		Prepaid,
		Saving,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INActivateCarSignalIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAmountType : long {
		Unknown = 0,
		MinimumDue,
		AmountDue,
		CurrentBalance,
		[MacCatalyst (13, 1)]
		MaximumTransferAmount,
		[MacCatalyst (13, 1)]
		MinimumTransferAmount,
		[MacCatalyst (13, 1)]
		StatementBalance,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INBillType : long {
		Unknown = 0,
		AutoInsurance,
		Cable,
		CarLease,
		CarLoan,
		CreditCard,
		Electricity,
		Gas,
		GarbageAndRecycling,
		HealthInsurance,
		HomeInsurance,
		Internet,
		LifeInsurance,
		Mortgage,
		MusicStreaming,
		Phone,
		Rent,
		Sewer,
		StudentLoan,
		TrafficTicket,
		Tuition,
		Utilities,
		Water,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INCarSignalOptions : ulong {
		Audible = (1 << 0),
		Visible = (1 << 1),
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetCarLockStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetCarPowerLevelStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INPayBillIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureInsufficientFunds,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INSearchForBillsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureBillNotFound,
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSetCarLockStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddTasksIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INAppendToNoteIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCannotUpdatePasswordProtectedNote,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INBalanceType : long {
		Unknown = 0,
		Money,
		Points,
		Miles,
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallCapability : long {
		Unknown = 0,
		AudioCall,
		VideoCall,
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallDestinationType : long {
		Unknown = 0,
		Normal,
		Emergency,
		Voicemail,
		Redial,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		CallBack,
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INCallRecordTypeOptions : ulong {
		Outgoing = (1 << 0),
		Missed = (1 << 1),
		Received = (1 << 2),
		Latest = (1 << 3),
		Voicemail = (1 << 4),
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Ringing = (1 << 5),
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		InProgress = (1 << 6),
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		OnHold = (1 << 7),
	}

	[NoWatch, NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCancelRideIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		Success,
		Failure,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCreateNoteIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCreateTaskListIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INDateSearchType : long {
		Unknown = 0,
		ByDueDate,
		ByModifiedDate,
		ByCreatedDate,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INGetVisualCodeIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureAppConfigurationRequired,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INLocationSearchType : long {
		Unknown = 0,
		ByLocationTrigger,
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMessageType : long {
		Unspecified = 0,
		Text,
		Audio,
		DigitalTouch,
		Handwriting,
		Sticker,
		TapbackLiked,
		TapbackDisliked,
		TapbackEmphasized,
		TapbackLoved,
		TapbackQuestioned,
		TapbackLaughed,
		MediaCalendar,
		MediaLocation,
		MediaAddressCard,
		MediaImage,
		MediaVideo,
		MediaPass,
		MediaAudio,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		PaymentSent,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		PaymentRequest,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		PaymentNote,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Animoji,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		ActivitySnippet,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		File,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Link,
		[iOS (17, 0), MacCatalyst (17, 0), Watch (10, 0)]
		Reaction,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INNoteContentType : long {
		Unknown = 0,
		Text,
		Image,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INNotebookItemType : long {
		Unknown = 0,
		Note,
		TaskList,
		Task,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRecurrenceFrequency : long {
		Unknown = 0,
		Minute,
		Hourly,
		Daily,
		Weekly,
		Monthly,
		Yearly,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestPaymentCurrencyAmountUnsupportedReason : long {
		AmountBelowMinimum = 1,
		AmountAboveMaximum,
		CurrencyUnsupported,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestPaymentPayerUnsupportedReason : long {
		CredentialsUnverified = 1,
		NoAccount,
		[Watch (4, 1)]
		[MacCatalyst (13, 1)]
		NoValidHandle,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRideFeedbackTypeOptions : ulong {
		Rate = (1 << 0),
		Tip = (1 << 1),
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSearchForAccountsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureAccountNotFound,
		FailureTermsAndConditionsAcceptanceRequired,
		FailureNotEligible,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSearchForNotebookItemsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendMessageRecipientUnsupportedReason : long {
		NoAccount = 1,
		Offline,
		MessagingServiceNotEnabledForRecipient,
		NoValidHandle,
		RequestedHandleInvalid,
		NoHandleForLabel,
		[Mac (14, 0), MacCatalyst (17, 0)]
		RequiringInAppAuthentication,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendPaymentCurrencyAmountUnsupportedReason : long {
		AmountBelowMinimum = 1,
		AmountAboveMaximum,
		CurrencyUnsupported,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendPaymentPayeeUnsupportedReason : long {
		CredentialsUnverified = 1,
		InsufficientFunds,
		NoAccount,
		[Watch (4, 1)]
		[MacCatalyst (13, 1)]
		NoValidHandle,
	}

	[NoWatch, NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendRideFeedbackIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		Success,
		Failure,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSetTaskAttributeIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSortType : long {
		Unknown = 0,
		AsIs,
		ByDate,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSpatialEvent : long {
		Unknown = 0,
		Arrive,
		Depart,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTaskStatus : long {
		Unknown = 0,
		NotCompleted,
		Completed,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTaskType : long {
		Unknown = 0,
		NotCompletable,
		Completable,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INTransferMoneyIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureInsufficientFunds,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INVisualCodeType : long {
		Unknown = 0,
		Contact,
		RequestPayment,
		SendPayment,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Transit,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Bus,
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		Subway,
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaItemType : long {
		Unknown = 0,
		Song,
		Album,
		Artist,
		Genre,
		Playlist,
		PodcastShow,
		PodcastEpisode,
		PodcastPlaylist,
		MusicStation,
		AudioBook,
		Movie,
		TVShow,
		TVShowEpisode,
		MusicVideo,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PodcastStation,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		RadioStation,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Station,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Music,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		AlgorithmicRadioStation,
		[Watch (6, 2, 1), iOS (13, 4, 1)]
		[MacCatalyst (13, 1)]
		News,
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlayMediaIntentResponseCode : long {
		Unspecified = 0,
		Ready = 1,
		ContinueInApp = 2,
		InProgress = 3,
		Success = 4,
#if XAMCORE_5_0 && !NET
		[NoWatch]
#endif
		HandleInApp = 5,
		Failure = 6,
		FailureRequiringAppLaunch = 7,
		FailureUnknownMediaType = 8,
		FailureNoUnplayedContent = 9,
		FailureRestrictedContent = 10,
		FailureMaxStreamLimitReached = 11,
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlaybackRepeatMode : long {
		Unknown = 0,
		None,
		All,
		One,
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INDailyRoutineSituation : long {
		Morning,
		Evening,
		Home,
		Work,
		School,
		Gym,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Commute,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		HeadphonesConnected,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		ActiveWorkout,
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PhysicalActivityIncomplete,
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INUpcomingMediaPredictionMode : long {
		Default = 0,
		OnlyPredictSuggestedIntents = 1,
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRelevantShortcutRole : long {
		Action,
		Information,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddMediaIntentResponseCode : long {
		Unspecified = 0,
		Ready = 1,
		InProgress = 2,
		Success = 3,
#if XAMCORE_5_0 && !NET
		[NoWatch]
#endif
		HandleInApp = 4,
		Failure = 5,
		FailureRequiringAppLaunch = 6,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddMediaMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddTasksTargetTaskListConfirmationReason : long {
		ListShouldBeCreated = 1,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddTasksTemporalEventTriggerUnsupportedReason : long {
		TimeInPast = 1,
		InvalidRecurrence,
	}

	[Watch (6, 0), NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallAudioRoute : long {
		Unknown = 0,
		SpeakerphoneAudioRoute,
		BluetoothAudioRoute,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INDeleteTasksIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INDeleteTasksTaskListUnsupportedReason : long {
		NoTaskListFound = 1,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INDeleteTasksTaskUnsupportedReason : long {
		Found = 1,
		InApp,
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetReservationDetailsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaAffinityType : long {
		Unknown = 0,
		Like,
		Dislike,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaDestinationType : long {
		Unknown = 0,
		Library,
		Playlist,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaReference : long {
		Unknown = 0,
		CurrentlyPlaying,
		[Watch (7, 4), TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		My,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaSortOrder : long {
		Unknown = 0,
		Newest,
		Oldest,
		Best,
		Worst,
		Popular,
		Unpopular,
		Trending,
		Recommended,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaUserContextSubscriptionStatus : long {
		Unknown = 0,
		NotSubscribed,
		Subscribed,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlayMediaMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlayMediaPlaybackSpeedUnsupportedReason : long {
		BelowMinimum = 1,
		AboveMaximum,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlaybackQueueLocation : long {
		Unknown = 0,
		Now,
		Next,
		Later,
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INReservationActionType : long {
		Unknown = 0,
		CheckIn,
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INReservationStatus : long {
		Unknown = 0,
		Canceled,
		Pending,
		Hold,
		Confirmed,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSearchForMediaIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSearchForMediaMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSetTaskAttributeTemporalEventTriggerUnsupportedReason : long {
		TimeInPast = 1,
		InvalidRecurrence,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSnoozeTasksIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSnoozeTasksTaskUnsupportedReason : long {
		NoTasksFound = 1,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INStartCallCallCapabilityUnsupportedReason : long {
		VideoCallUnsupported = 1,
		MicrophoneNotAccessible,
		CameraNotAccessible,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INStartCallContactUnsupportedReason : long {
		NoContactFound = 1,
		MultipleContactsUnsupported,
		NoHandleForLabel,
		InvalidHandle,
		UnsupportedMmiUssd,
		NoCallHistoryForRedial,
		NoUsableHandleForRedial,
		[Watch (10, 0), NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		RequiringInAppAuthentication,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INStartCallIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		UserConfirmationRequired,
		Failure,
		FailureRequiringAppLaunch,
		FailureCallingServiceNotAvailable,
		FailureContactNotSupportedByApp,
		FailureAirplaneModeEnabled,
		FailureUnableToHandOff,
		FailureAppConfigurationRequired,
		FailureCallInProgress,
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		FailureCallRinging,
		[Watch (10, 0), NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		ResponseCode,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTaskPriority : long {
		Unknown = 0,
		NotFlagged,
		Flagged,
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTemporalEventTriggerTypeOptions : ulong {
		NotScheduled = (1uL << 0),
		ScheduledNonRecurring = (1uL << 1),
		ScheduledRecurring = (1uL << 2),
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTicketedEventCategory : long {
		Unknown = 0,
		Movie,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INUpdateMediaAffinityIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INUpdateMediaAffinityMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddMediaMediaDestinationUnsupportedReason : long {
		PlaylistNameNotFound = 1,
		PlaylistNotEditable = 2,
	}

	[Flags]
	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INDayOfWeekOptions : ulong {
		Monday = (1uL << 0),
		Tuesday = (1uL << 1),
		Wednesday = (1uL << 2),
		Thursday = (1uL << 3),
		Friday = (1uL << 4),
		Saturday = (1uL << 5),
		Sunday = (1uL << 6),
	}

	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INListCarsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INOutgoingMessageType : long {
		Unknown = 0,
		Text,
		Audio,
	}

	[Flags]
	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INShortcutAvailabilityOptions : ulong {
		Mindfulness = (1uL << 0),
		Journaling = (1uL << 1),
		Music = (1uL << 2),
		Podcasts = (1uL << 3),
		Reading = (1uL << 4),
		WrapUpYourDay = (1uL << 5),
		YogaAndStretching = (1uL << 6),
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INStartCallCallRecordToCallBackUnsupportedReason : long {
		NoMatchingCall = 1,
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[Native]
	public enum INAnswerCallIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[Native]
	public enum INHangUpCallIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoCallToHangUp,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	public enum INIntentIdentifier {
		[Field (null)]
		None = -1,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'StartCall' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'StartCall' instead.")]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'StartCall' instead.")]
		[Field ("INStartAudioCallIntentIdentifier")]
		StartAudioCall,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'StartCall' instead.")]
		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'StartCall' instead.")]
		[Field ("INStartVideoCallIntentIdentifier")]
		StartVideoCall,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSearchCallHistoryIntentIdentifier")]
		SearchCallHistory,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetAudioSourceInCarIntentIdentifier")]
		SetAudioSourceInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetClimateSettingsInCarIntentIdentifier")]
		SetClimateSettingsInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetDefrosterSettingsInCarIntentIdentifier")]
		SetDefrosterSettingsInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetSeatSettingsInCarIntentIdentifier")]
		SetSeatSettingsInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetProfileInCarIntentIdentifier")]
		SetProfileInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSaveProfileInCarIntentIdentifier")]
		SaveProfileInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INStartWorkoutIntentIdentifier")]
		StartWorkout,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INPauseWorkoutIntentIdentifier")]
		PauseWorkout,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INEndWorkoutIntentIdentifier")]
		EndWorkout,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INCancelWorkoutIntentIdentifier")]
		CancelWorkout,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INResumeWorkoutIntentIdentifier")]
		ResumeWorkout,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetRadioStationIntentIdentifier")]
		SetRadioStation,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSendMessageIntentIdentifier")]
		SendMessage,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSearchForMessagesIntentIdentifier")]
		SearchForMessages,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
		[Field ("INSetMessageAttributeIntentIdentifier")]
		SetMessageAttribute,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSendPaymentIntentIdentifier")]
		SendPayment,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INRequestPaymentIntentIdentifier")]
		RequestPayment,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSearchForPhotosIntentIdentifier")]
		SearchForPhotos,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INStartPhotoPlaybackIntentIdentifier")]
		StartPhotoPlayback,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INListRideOptionsIntentIdentifier")]
		ListRideOptions,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INRequestRideIntentIdentifier")]
		RequestRide,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INGetRideStatusIntentIdentifier")]
		GetRideStatus,

		[Watch (7, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("INStartCallIntentIdentifier")]
		StartCall,

		[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
		[Field ("INAnswerCallIntentIdentifier")]
		AnswerCall,

		[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
		[Field ("INHangUpCallIntentIdentifier")]
		HangUpCall,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	enum INPersonHandleLabel {
		[Field (null)]
		None,

		[Field ("INPersonHandleLabelHome")]
		Home,

		[Field ("INPersonHandleLabelWork")]
		Work,

		[Field ("INPersonHandleLabeliPhone")]
		iPhone,

		[Field ("INPersonHandleLabelMobile")]
		Mobile,

		[Field ("INPersonHandleLabelMain")]
		Main,

		[Field ("INPersonHandleLabelHomeFax")]
		HomeFax,

		[Field ("INPersonHandleLabelWorkFax")]
		WorkFax,

		[Field ("INPersonHandleLabelPager")]
		Pager,

		[Field ("INPersonHandleLabelOther")]
		Other,

		[Watch (7, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("INPersonHandleLabelSchool")]
		School,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	enum INPersonRelationship {
		[Field (null)]
		None,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipFather")]
		Father,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipMother")]
		Mother,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipParent")]
		Parent,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipBrother")]
		Brother,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipSister")]
		Sister,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipChild")]
		Child,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipFriend")]
		Friend,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipSpouse")]
		Spouse,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipPartner")]
		Partner,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipAssistant")]
		Assistant,

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.MacOSX, 10, 0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Field ("INPersonRelationshipManager")]
		Manager,

		[Watch (6, 0), NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipSon")]
		Son,

		[Watch (6, 0), NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipDaughter")]
		Daughter,
	}

	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	enum INWorkoutNameIdentifier {
		[Field ("INWorkoutNameIdentifierRun")]
		Run,

		[Field ("INWorkoutNameIdentifierSit")]
		Sit,

		[Field ("INWorkoutNameIdentifierSteps")]
		Steps,

		[Field ("INWorkoutNameIdentifierStand")]
		Stand,

		[Field ("INWorkoutNameIdentifierMove")]
		Move,

		[Field ("INWorkoutNameIdentifierWalk")]
		Walk,

		[Field ("INWorkoutNameIdentifierYoga")]
		Yoga,

		[Field ("INWorkoutNameIdentifierDance")]
		Dance,

		[Field ("INWorkoutNameIdentifierCrosstraining")]
		Crosstraining,

		[Field ("INWorkoutNameIdentifierElliptical")]
		Elliptical,

		[Field ("INWorkoutNameIdentifierRower")]
		Rower,

		[Field ("INWorkoutNameIdentifierCycle")]
		Cycle,

		[Field ("INWorkoutNameIdentifierStairs")]
		Stairs,

		[Field ("INWorkoutNameIdentifierOther")]
		Other,

		[Field ("INWorkoutNameIdentifierIndoorrun")]
		Indoorrun,

		[Field ("INWorkoutNameIdentifierIndoorcycle")]
		Indoorcycle,

		[Field ("INWorkoutNameIdentifierIndoorwalk")]
		Indoorwalk,

		[Field ("INWorkoutNameIdentifierExercise")]
		Exercise,

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INWorkoutNameIdentifierHike")]
		Hike,

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INWorkoutNameIdentifierHighIntensityIntervalTraining")]
		HighIntensityIntervalTraining,

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INWorkoutNameIdentifierSwim")]
		Swim,
	}

	[iOS (14, 0), NoMac, NoTV, Watch (7, 0)]
	[MacCatalyst (14, 0)]
	enum INCarChargingConnectorType {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("INCarChargingConnectorTypeJ1772")]
		J1772,

		[Field ("INCarChargingConnectorTypeCCS1")]
		Ccs1,

		[Field ("INCarChargingConnectorTypeCCS2")]
		Ccs2,

		[Field ("INCarChargingConnectorTypeCHAdeMO")]
		ChaDeMo,

		[Field ("INCarChargingConnectorTypeGBTAC")]
		Gbtac,

		[Field ("INCarChargingConnectorTypeGBTDC")]
		Gbtdc,

		[Field ("INCarChargingConnectorTypeTesla")]
		Tesla,

		[Field ("INCarChargingConnectorTypeMennekes")]
		Mennekes,
	}

	// End of enums

	[Mac (11, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Internal]
	[Category]
	[BaseType (typeof (CLPlacemark))]
	interface CLPlacemark_INIntentsAdditions {

		[Static]
		[Export ("placemarkWithLocation:name:postalAddress:")]
		CLPlacemark _GetPlacemark (CLLocation location, [NullAllowed] string name, [NullAllowed] CNPostalAddress postalAddress);
	}

	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INBookRestaurantReservationIntent : NSCopying {
		[MacCatalyst (13, 1)]
		[Export ("initWithRestaurant:bookingDateComponents:partySize:bookingIdentifier:guest:selectedOffer:guestProvidedSpecialRequestText:")]
#if NET
		NativeHandle Constructor (INRestaurant restaurant, NSDateComponents bookingDateComponents, nuint partySize, [NullAllowed] string bookingIdentifier, [NullAllowed] INRestaurantGuest guest, [NullAllowed] INRestaurantOffer selectedOffer, [NullAllowed] string guestProvidedSpecialRequestText);
#else
		// This is correctly nuint but a bug in PMCS generated incorrect code which has shipped.
		NativeHandle Constructor (INRestaurant restaurant, NSDateComponents bookingDateComponents, ulong partySize, [NullAllowed] string bookingIdentifier, [NullAllowed] INRestaurantGuest guest, [NullAllowed] INRestaurantOffer selectedOffer, [NullAllowed] string guestProvidedSpecialRequestText);
#endif
		[Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[Export ("bookingDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents BookingDateComponents { get; set; }

		[Export ("partySize")]
		nuint PartySize { get; set; }

		[NullAllowed, Export ("bookingIdentifier")]
		string BookingIdentifier { get; set; }

		[NullAllowed, Export ("guest", ArgumentSemantic.Copy)]
		INRestaurantGuest Guest { get; set; }

		[NullAllowed, Export ("selectedOffer", ArgumentSemantic.Copy)]
		INRestaurantOffer SelectedOffer { get; set; }

		[NullAllowed, Export ("guestProvidedSpecialRequestText")]
		string GuestProvidedSpecialRequestText { get; set; }
	}

	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INBookRestaurantReservationIntentHandling {

		[Abstract]
		[Export ("handleBookRestaurantReservation:completion:")]
		void HandleBookRestaurantReservation (INBookRestaurantReservationIntent intent, Action<INBookRestaurantReservationIntentResponse> completion);

		[Export ("confirmBookRestaurantReservation:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmBookRestaurantReservation
#endif
		(INBookRestaurantReservationIntent intent, Action<INBookRestaurantReservationIntentResponse> completion);

		[Export ("resolveRestaurantForBookRestaurantReservation:withCompletion:")]
		void ResolveRestaurant (INBookRestaurantReservationIntent intent, Action<INRestaurantResolutionResult> completion);

		[Export ("resolveBookingDateComponentsForBookRestaurantReservation:withCompletion:")]
		void ResolveBookingDate (INBookRestaurantReservationIntent intent, Action<INDateComponentsResolutionResult> completion);

		[Export ("resolvePartySizeForBookRestaurantReservation:withCompletion:")]
		void ResolvePartySize (INBookRestaurantReservationIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolveGuestForBookRestaurantReservation:withCompletion:")]
		void ResolveGuest (INBookRestaurantReservationIntent intent, Action<INRestaurantGuestResolutionResult> completion);

		[Export ("resolveGuestProvidedSpecialRequestTextForBookRestaurantReservation:withCompletion:")]
		void ResolveGuestProvidedSpecialRequest (INBookRestaurantReservationIntent intent, Action<INStringResolutionResult> completion);
	}

	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	interface INBookRestaurantReservationIntentResponse {

		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INBookRestaurantReservationIntentCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INBookRestaurantReservationIntentCode Code { get; }

		[NullAllowed, Export ("userBooking", ArgumentSemantic.Copy)]
		INRestaurantReservationUserBooking UserBooking { get; set; }
	}

	[Mac (11, 0)]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INBooleanResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INBooleanResolutionResult GetSuccess (bool resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INBooleanResolutionResult GetConfirmationRequired ([NullAllowed] NSNumber valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INBooleanResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INBooleanResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INBooleanResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBooleanResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBooleanResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallRecordTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCallRecordType:")]
		INCallRecordTypeResolutionResult SuccessWithResolvedCallRecordType (INCallRecordType resolvedCallRecordType);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCallRecordTypeResolutionResult SuccessWithResolvedValue (INCallRecordType resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCallRecordTypeToConfirm:")]
		INCallRecordTypeResolutionResult ConfirmationRequiredWithCallRecordTypeToConfirm (INCallRecordType callRecordTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCallRecordTypeResolutionResult ConfirmationRequiredWithValueToConfirm (INCallRecordType valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCallRecordTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCallRecordTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCallRecordTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallRecordTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallRecordTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INCancelWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INCancelWorkoutIntentHandling {

		[Abstract]
		[Export ("handleCancelWorkout:completion:")]
		void HandleCancelWorkout (INCancelWorkoutIntent intent, Action<INCancelWorkoutIntentResponse> completion);

		[Export ("confirmCancelWorkout:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmCancelWorkout
#endif
		(INCancelWorkoutIntent intent, Action<INCancelWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForCancelWorkout:withCompletion:")]
		void ResolveWorkoutName (INCancelWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	interface INCancelWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INCancelWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCancelWorkoutIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarAirCirculationModeResolutionResult bound
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarAirCirculationModeResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarAirCirculationMode:")]
		INCarAirCirculationModeResolutionResult SuccessWithResolvedCarAirCirculationMode (INCarAirCirculationMode resolvedCarAirCirculationMode);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarAirCirculationModeResolutionResult SuccessWithResolvedValue (INCarAirCirculationMode resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarAirCirculationModeToConfirm:")]
		INCarAirCirculationModeResolutionResult ConfirmationRequiredWithCarAirCirculationModeToConfirm (INCarAirCirculationMode carAirCirculationModeToConfirm);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarAirCirculationModeResolutionResult ConfirmationRequiredWithValueToConfirm (INCarAirCirculationMode valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCarAirCirculationModeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCarAirCirculationModeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCarAirCirculationModeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarAirCirculationModeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarAirCirculationModeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarAudioSourceResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	interface INCarAudioSourceResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarAudioSource:")]
		INCarAudioSourceResolutionResult SuccessWithResolvedCarAudioSource (INCarAudioSource resolvedCarAudioSource);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarAudioSourceResolutionResult SuccessWithResolvedValue (INCarAudioSource resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarAudioSourceToConfirm:")]
		INCarAudioSourceResolutionResult ConfirmationRequiredWithCarAudioSourceToConfirm (INCarAudioSource carAudioSourceToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarAudioSourceResolutionResult ConfirmationRequiredWithValueToConfirm (INCarAudioSource valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCarAudioSourceResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCarAudioSourceResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCarAudioSourceResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarAudioSourceResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarAudioSourceResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarDefrosterResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	interface INCarDefrosterResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarDefroster:")]
		INCarDefrosterResolutionResult SuccessWithResolvedCarDefroster (INCarDefroster resolvedCarDefroster);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarDefrosterResolutionResult SuccessWithResolvedValue (INCarDefroster resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarDefrosterToConfirm:")]
		INCarDefrosterResolutionResult ConfirmationRequiredWithCarDefrosterToConfirm (INCarDefroster carDefrosterToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarDefrosterResolutionResult ConfirmationRequiredWithValueToConfirm (INCarDefroster valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCarDefrosterResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCarDefrosterResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCarDefrosterResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarDefrosterResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarDefrosterResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarSeatResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	interface INCarSeatResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarSeat:")]
		INCarSeatResolutionResult SuccessWithResolvedCarSeat (INCarSeat resolvedCarSeat);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarSeatResolutionResult SuccessWithResolvedValue (INCarSeat resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarSeatToConfirm:")]
		INCarSeatResolutionResult ConfirmationRequiredWithCarSeatToConfirm (INCarSeat carSeatToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarSeatResolutionResult ConfirmationRequiredWithValueToConfirm (INCarSeat valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCarSeatResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCarSeatResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCarSeatResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarSeatResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarSeatResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCurrencyAmount : NSCopying, NSSecureCoding {

		[Export ("initWithAmount:currencyCode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDecimalNumber amount, string currencyCode);

		[NullAllowed, Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; }

		[NullAllowed, Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCurrencyAmountResolutionResult {

		[Static]
		[Export ("successWithResolvedCurrencyAmount:")]
		INCurrencyAmountResolutionResult GetSuccess (INCurrencyAmount resolvedCurrencyAmount);

		[Static]
		[Export ("disambiguationWithCurrencyAmountsToDisambiguate:")]
		INCurrencyAmountResolutionResult GetDisambiguation (INCurrencyAmount [] currencyAmountsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithCurrencyAmountToConfirm:")]
		INCurrencyAmountResolutionResult GetConfirmationRequired ([NullAllowed] INCurrencyAmount currencyAmountToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCurrencyAmountResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCurrencyAmountResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCurrencyAmountResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCurrencyAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCurrencyAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INDateComponentsRange : NSCopying, NSSecureCoding {

		[Export ("initWithStartDateComponents:endDateComponents:")]
		NativeHandle Constructor ([NullAllowed] NSDateComponents startDateComponents, [NullAllowed] NSDateComponents endDateComponents);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("initWithEKRecurrenceRule:")]
		NativeHandle Constructor (EKRecurrenceRule recurrenceRule);

		// Headers claim the recurrenceRule property is available in macOS, but the parameter type INRecurrenceRule is not, so...
#if NET
		[NoMac]
#endif
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("initWithStartDateComponents:endDateComponents:recurrenceRule:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSDateComponents startDateComponents, [NullAllowed] NSDateComponents endDateComponents, [NullAllowed] INRecurrenceRule recurrenceRule);

		[NullAllowed, Export ("startDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents StartDateComponents { get; }

		[NullAllowed, Export ("endDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents EndDateComponents { get; }

		// Headers claim the recurrenceRule property is available in macOS, but the property type (INRecurrenceRule) is not, so...
#if NET
		[NoMac]
#endif
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("recurrenceRule", ArgumentSemantic.Copy)]
		INRecurrenceRule RecurrenceRule { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("EKRecurrenceRule")]
		[NullAllowed]
		EKRecurrenceRule EKRecurrenceRule { get; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INDateComponentsRangeResolutionResult {

		[Static]
		[Export ("successWithResolvedDateComponentsRange:")]
		INDateComponentsRangeResolutionResult GetSuccess (INDateComponentsRange resolvedDateComponentsRange);

		[Static]
		[Export ("disambiguationWithDateComponentsRangesToDisambiguate:")]
		INDateComponentsRangeResolutionResult GetDisambiguation (INDateComponentsRange [] dateComponentsRangesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithDateComponentsRangeToConfirm:")]
		INDateComponentsRangeResolutionResult GetConfirmationRequired ([NullAllowed] INDateComponentsRange dateComponentsRangeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDateComponentsRangeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDateComponentsRangeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDateComponentsRangeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDateComponentsRangeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDateComponentsRangeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

#if NET
	[NoMac]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INStartAudioCallIntentHandling, INStartVideoCallIntentHandling and INSearchCallHistoryIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 13, 0, message: "Implement 'INStartAudioCallIntentHandling and INSearchCallHistoryIntentHandling' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INStartAudioCallIntentHandling, INStartVideoCallIntentHandling and INSearchCallHistoryIntentHandling' instead.")]
	[Protocol]
	interface INCallsDomainHandling : INStartAudioCallIntentHandling, INSearchCallHistoryIntentHandling
#if !WATCH
	, INStartVideoCallIntentHandling {
#else
	{
#endif
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling and INGetCarPowerLevelStatusIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling and INGetCarPowerLevelStatusIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling and INGetCarPowerLevelStatusIntentHandling' instead.")]
	[Protocol]
	interface INCarCommandsDomainHandling : INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling, INGetCarPowerLevelStatusIntentHandling {
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling and INSaveProfileInCarIntentHandling' instead.")]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling and INSaveProfileInCarIntentHandling' instead.")]
	[Protocol]
	interface INCarPlayDomainHandling : INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling, INSaveProfileInCarIntentHandling {
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling and INResumeWorkoutIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling and INResumeWorkoutIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling and INResumeWorkoutIntentHandling' instead.")]
	[Protocol]
	interface INWorkoutsDomainHandling : INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling, INResumeWorkoutIntentHandling {
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSetRadioStationIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSetRadioStationIntentHandling' instead.")]
	[Protocol]
	interface INRadioDomainHandling : INSetRadioStationIntentHandling {
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSendMessageIntentHandling, INSearchForMessagesIntentHandling and INSetMessageAttributeIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 13, 0, message: "Implement 'INSendMessageIntentHandling and INSearchForMessagesIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSendMessageIntentHandling, INSearchForMessagesIntentHandling and INSetMessageAttributeIntentHandling' instead.")]
	[Protocol]
	interface INMessagesDomainHandling : INSendMessageIntentHandling, INSearchForMessagesIntentHandling
#if !WATCH
	, INSetMessageAttributeIntentHandling {
#else
	{
#endif
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling, INSearchForAccountsIntentHandling and INTransferMoneyIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INSendPaymentIntentHandling and INRequestPaymentIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling, INSearchForAccountsIntentHandling and INTransferMoneyIntentHandling' instead.")]
	[Protocol]
	interface INPaymentsDomainHandling : INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling
#if NET // Added in iOS 11 -> #if __IPHONE_OS_VERSION_MIN_REQUIRED >= 110000
	, INSearchForAccountsIntentHandling, INTransferMoneyIntentHandling
#endif
	{
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSearchForPhotosIntentHandling and INStartPhotoPlaybackIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INSearchForPhotosIntentHandling and INStartPhotoPlaybackIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSearchForPhotosIntentHandling and INStartPhotoPlaybackIntentHandling' instead.")]
	[Protocol]
	interface INPhotosDomainHandling : INSearchForPhotosIntentHandling, INStartPhotoPlaybackIntentHandling {
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling, INCancelRideIntentHandling and INSendRideFeedbackIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling, INCancelRideIntentHandling and INSendRideFeedbackIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling, INCancelRideIntentHandling and INSendRideFeedbackIntentHandling' instead.")]
	[Protocol]
	interface INRidesharingDomainHandling : INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling
#if NET // Added in iOS 11 -> #if __IPHONE_OS_VERSION_MIN_REQUIRED >= 110000
	, INCancelRideIntentHandling, INSendRideFeedbackIntentHandling
#endif
	{
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling and INSearchForNotebookItemsIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling and INSearchForNotebookItemsIntentHandling' instead.")]
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling and INSearchForNotebookItemsIntentHandling' instead.")]
	[Protocol]
	interface INNotebookDomainHandling : INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling, INSearchForNotebookItemsIntentHandling {
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INGetVisualCodeIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Implement 'INGetVisualCodeIntentHandling' instead.")]
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INGetVisualCodeIntentHandling' instead.")]
	[Protocol]
	interface INVisualCodeDomainHandling : INGetVisualCodeIntentHandling {
	}

	[Mac (11, 0), TV (14, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INDoubleResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INDoubleResolutionResult GetSuccess (double resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INDoubleResolutionResult GetConfirmationRequired ([NullAllowed] NSNumber valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDoubleResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDoubleResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDoubleResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDoubleResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDoubleResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Mac (11, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INDateComponentsResolutionResult {

		[Static]
		[Export ("successWithResolvedDateComponents:")]
		INDateComponentsResolutionResult GetSuccess (NSDateComponents resolvedDateComponents);

		[Static]
		[Export ("disambiguationWithDateComponentsToDisambiguate:")]
		INDateComponentsResolutionResult GetDisambiguation (NSDateComponents [] componentsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithDateComponentsToConfirm:")]
		INDateComponentsResolutionResult GetConfirmationRequired ([NullAllowed] NSDateComponents componentsToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDateComponentsResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDateComponentsResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDateComponentsResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDateComponentsResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDateComponentsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INEndWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INEndWorkoutIntentHandling {

		[Abstract]
		[Export ("handleEndWorkout:completion:")]
		void HandleEndWorkout (INEndWorkoutIntent intent, Action<INEndWorkoutIntentResponse> completion);

		[Export ("confirmEndWorkout:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmEndWorkout
#endif
		(INEndWorkoutIntent intent, Action<INEndWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForEndWorkout:withCompletion:")]
		void ResolveWorkoutName (INEndWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INEndWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INEndWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INEndWorkoutIntentResponseCode Code { get; }
	}

	[TV (14, 0), Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INIntentHandlerProviding {

		[Abstract]
		[Export ("handlerForIntent:")]
		[return: NullAllowed]
		NSObject GetHandler (INIntent intent);
	}

	[Mac (11, 0), TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INExtension : INIntentHandlerProviding {
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntent {

		[MacCatalyst (13, 1)]
		[Export ("initWithRestaurant:")]
		NativeHandle Constructor ([NullAllowed] INRestaurant restaurant);

		[NullAllowed, Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void HandleAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmAvailableRestaurantReservationBookingDefaults
#endif
		(INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookingDefaults:withCompletion:")]
		void ResolveAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INRestaurantResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntentResponse {

		[Export ("defaultPartySize")]
		nuint DefaultPartySize { get; }

		[Export ("defaultBookingDate", ArgumentSemantic.Copy)]
		NSDate DefaultBookingDate { get; }

		[NullAllowed, Export ("maximumPartySize", ArgumentSemantic.Copy)]
		NSNumber MaximumPartySize { get; set; }

		[NullAllowed, Export ("minimumPartySize", ArgumentSemantic.Copy)]
		NSNumber MinimumPartySize { get; set; }

		[Export ("providerImage", ArgumentSemantic.Copy)]
		INImage ProviderImage { get; set; }

		[Export ("initWithDefaultPartySize:defaultBookingDate:code:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nuint defaultPartySize, NSDate defaultBookingDate, INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode Code { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INGetAvailableRestaurantReservationBookingsIntent : NSCopying {

		[MacCatalyst (13, 1)]
		[Export ("initWithRestaurant:partySize:preferredBookingDateComponents:maximumNumberOfResults:earliestBookingDateForResults:latestBookingDateForResults:")]
		NativeHandle Constructor (INRestaurant restaurant, nuint partySize, [NullAllowed] NSDateComponents preferredBookingDateComponents, [NullAllowed] NSNumber maximumNumberOfResults, [NullAllowed] NSDate earliestBookingDateForResults, [NullAllowed] NSDate latestBookingDateForResults);

		[Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[Export ("partySize")]
		nuint PartySize { get; set; }

		[NullAllowed, Export ("preferredBookingDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents PreferredBookingDateComponents { get; set; }

		[NullAllowed, Export ("maximumNumberOfResults", ArgumentSemantic.Copy)]
		NSNumber MaximumNumberOfResults { get; set; }

		[NullAllowed, Export ("earliestBookingDateForResults", ArgumentSemantic.Copy)]
		NSDate EarliestBookingDateForResults { get; set; }

		[NullAllowed, Export ("latestBookingDateForResults", ArgumentSemantic.Copy)]
		NSDate LatestBookingDateForResults { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookings:completion:")]
		void HandleAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookings:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmAvailableRestaurantReservationBookings
#endif
		(INGetAvailableRestaurantReservationBookingsIntent intent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookings:withCompletion:")]
		void ResolveAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INRestaurantResolutionResult> completion);

		[Export ("resolvePartySizeForGetAvailableRestaurantReservationBookings:withCompletion:")]
		void ResolvePartySizeAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolvePreferredBookingDateComponentsForGetAvailableRestaurantReservationBookings:withCompletion:")]
		void ResolvePreferredBookingDateAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INDateComponentsResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetAvailableRestaurantReservationBookingsIntentResponse {

		[Export ("initWithAvailableBookings:code:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRestaurantReservationBooking [] availableBookings, INGetAvailableRestaurantReservationBookingsIntentCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetAvailableRestaurantReservationBookingsIntentCode Code { get; }

		[NullAllowed, Export ("localizedRestaurantDescriptionText")]
		string LocalizedRestaurantDescriptionText { get; set; }

		[NullAllowed, Export ("localizedBookingAdvisementText")]
		string LocalizedBookingAdvisementText { get; set; }

		[NullAllowed, Export ("termsAndConditions", ArgumentSemantic.Copy)]
		INTermsAndConditions TermsAndConditions { get; set; }

		[Export ("availableBookings")]
		INRestaurantReservationBooking [] AvailableBookings { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INGetRestaurantGuestIntent {
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetRestaurantGuestIntentHandling {

		[Abstract]
		[Export ("handleGetRestaurantGuest:completion:")]
		void HandleRestaurantGuest (INGetRestaurantGuestIntent intent, Action<INGetRestaurantGuestIntentResponse> completion);

		[Export ("confirmGetRestaurantGuest:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRestaurantGuest
#endif
		(INGetRestaurantGuestIntent guestIntent, Action<INGetRestaurantGuestIntentResponse> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetRestaurantGuestIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INGetRestaurantGuestIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[NullAllowed, Export ("guest", ArgumentSemantic.Copy)]
		INRestaurantGuest Guest { get; set; }

		[NullAllowed, Export ("guestDisplayPreferences", ArgumentSemantic.Copy)]
		INRestaurantGuestDisplayPreferences GuestDisplayPreferences { get; set; }

		[Export ("code")]
		INGetRestaurantGuestIntentResponseCode Code { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor] // DesignatedInitializer below
	interface INGetRideStatusIntent {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetRideStatusIntentHandling {

		[Abstract]
		[Export ("handleGetRideStatus:completion:")]
		void HandleRideStatus (INGetRideStatusIntent intent, Action<INGetRideStatusIntentResponse> completion);

		[Abstract]
		[Export ("startSendingUpdatesForGetRideStatus:toObserver:")]
		void StartSendingUpdates (INGetRideStatusIntent intent, IINGetRideStatusIntentResponseObserver observer);

		[Abstract]
		[Export ("stopSendingUpdatesForGetRideStatus:")]
		void StopSendingUpdates (INGetRideStatusIntent intent);

		[Export ("confirmGetRideStatus:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRideStatus
#endif
		(INGetRideStatusIntent intent, Action<INGetRideStatusIntentResponse> completion);
	}

	interface IINGetRideStatusIntentResponseObserver { }

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetRideStatusIntentResponseObserver {

		[Abstract]
		[Export ("getRideStatusResponseDidUpdate:")]
		void DidUpdateRideStatus (INGetRideStatusIntentResponse response);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INGetRideStatusIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INGetRideStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetRideStatusIntentResponseCode Code { get; }

		[NullAllowed, Export ("rideStatus", ArgumentSemantic.Copy)]
		INRideStatus RideStatus { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INGetUserCurrentRestaurantReservationBookingsIntent : NSCopying {

		[MacCatalyst (13, 1)]
		[Export ("initWithRestaurant:reservationIdentifier:maximumNumberOfResults:earliestBookingDateForResults:")]
		NativeHandle Constructor ([NullAllowed] INRestaurant restaurant, [NullAllowed] string reservationIdentifier, [NullAllowed] NSNumber maximumNumberOfResults, [NullAllowed] NSDate earliestBookingDateForResults);

		[MacCatalyst (13, 1)]
		[Wrap ("this (restaurant, reservationIdentifier, NSNumber.FromNInt (maximumNumberOfResults), earliestBookingDateForResults)")]
		NativeHandle Constructor ([NullAllowed] INRestaurant restaurant, [NullAllowed] string reservationIdentifier, nint maximumNumberOfResults, [NullAllowed] NSDate earliestBookingDateForResults);

		[NullAllowed, Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[NullAllowed, Export ("reservationIdentifier")]
		string ReservationIdentifier { get; set; }

		[NullAllowed, Export ("maximumNumberOfResults", ArgumentSemantic.Copy)]
		NSNumber MaximumNumberOfResults { get; set; }

		[NullAllowed, Export ("earliestBookingDateForResults", ArgumentSemantic.Copy)]
		NSDate EarliestBookingDateForResults { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetUserCurrentRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetUserCurrentRestaurantReservationBookings:completion:")]
		void HandleUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetUserCurrentRestaurantReservationBookings:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmUserCurrentRestaurantReservationBookings
#endif
		(INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetUserCurrentRestaurantReservationBookings:withCompletion:")]
		void ResolveUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INRestaurantResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetUserCurrentRestaurantReservationBookingsIntentResponse {

		[Export ("initWithUserCurrentBookings:code:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRestaurantReservationUserBooking [] userCurrentBookings, INGetUserCurrentRestaurantReservationBookingsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetUserCurrentRestaurantReservationBookingsIntentResponseCode Code { get; }

		[Export ("userCurrentBookings", ArgumentSemantic.Copy)]
		INRestaurantReservationUserBooking [] UserCurrentBookings { get; set; }
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INImage : NSCopying, NSSecureCoding {

		[Static]
		[Export ("imageNamed:")]
		INImage FromName (string name);

		[NoWatch, NoTV, NoiOS, Mac (13, 0)]
		[NoMacCatalyst]
		[Static]
		[Export ("imageWithNSImage:")]
		INImage FromImage (NSImage image);

		[Watch (7, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("systemImageNamed:")]
		INImage FromSystem (string systemImageName);

		[Static]
		[Export ("imageWithImageData:")]
		INImage FromData (NSData imageData);

		[Static]
		[MarshalNativeExceptions]
		[return: NullAllowed]
		[Export ("imageWithURL:")]
		INImage FromUrl (NSUrl url);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithURL:width:height:")]
		[return: NullAllowed]
		INImage FromUrl (NSUrl url, double width, double height);

		// INImage_IntentsUI (IntentsUI)

		[NoMac, NoWatch, NoTV]
		[NoMacCatalyst]
		[Static]
		[Export ("imageWithCGImage:")]
		INImage FromImage (CGImage image);

		[NoMac, NoWatch, NoTV]
		[NoMacCatalyst]
		[Static]
		[Export ("imageWithUIImage:")]
		INImage FromImage (UIImage image);

		[NoMac, NoWatch, NoTV]
		[NoMacCatalyst]
		[Static]
		[Export ("imageSizeForIntentResponse:")]
		CGSize GetImageSize (INIntentResponse response);

		[NoMac, NoWatch, NoTV]
		[NoMacCatalyst]
		[Async]
		[Export ("fetchUIImageWithCompletion:")]
		void FetchImage (Action<UIImage> completion);
	}

	[TV (14, 0), Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INIntegerResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INIntegerResolutionResult GetSuccess (nint resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INIntegerResolutionResult GetConfirmationRequired ([NullAllowed] NSNumber valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INIntegerResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INIntegerResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INIntegerResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INIntegerResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INIntegerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface INIntent : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("identifier")]
		NSString IdentifierString { get; }

		[Unavailable (PlatformName.MacOSX)]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Wrap ("INIntentIdentifierExtensions.GetValue (IdentifierString)")]
		[NullAllowed]
		INIntentIdentifier? Identifier { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("intentDescription")]
		string IntentDescription { get; }

		[Watch (5, 0), Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("suggestedInvocationPhrase")]
		string SuggestedInvocationPhrase { get; set; }

		[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("shortcutAvailability", ArgumentSemantic.Assign)]
		INShortcutAvailabilityOptions ShortcutAvailability { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("donationMetadata", ArgumentSemantic.Copy)]
		INIntentDonationMetadata DonationMetadata { get; set; }

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setImage:forParameterNamed:")]
		void SetImage ([NullAllowed] INImage image, string parameterName);

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageForParameterNamed:")]
		[return: NullAllowed]
		INImage GetImage (string parameterName);

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("keyImage")]
		INImage GetKeyImage ();
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INIntentDonationMetadata : NSCopying, NSSecureCoding {
	}

	interface INIntentResolutionResult<ObjectType> : INIntentResolutionResult { }

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INIntentResolutionResult {

		// Needs to be overriden in subclasses
		// we provide a basic, managed, implementation that throws

		//[Static]
		//[Export ("needsValue")]
		//INIntentResolutionResult NeedsValue { get; }

		//[Static]
		//[Export ("notRequired")]
		//INIntentResolutionResult NotRequired { get; }

		//[Static]
		//[Export ("unsupported")]
		//INIntentResolutionResult Unsupported { get; }

		// [Watch (6,0), iOS (13,0), TV (14,0), Mac (11,0)]
		// [Static]
		// [Export ("unsupportedWithReason:")]
		// INIntentResolutionResult GetUnsupported (nint reason);

		// [Watch (6,0), iOS (13,0), TV (14,0), Mac (11,0)]
		// [Static]
		// [Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		// INIntentResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface INIntentResponse : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("userActivity", ArgumentSemantic.Copy)]
		NSUserActivity UserActivity {
			get;

			[Watch (5, 0)]
			[MacCatalyst (13, 1)]
			set;
		}
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INInteraction : NSSecureCoding, NSCopying {

		[Export ("initWithIntent:response:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INIntent intent, [NullAllowed] INIntentResponse response);

		[Async]
		[Export ("donateInteractionWithCompletion:")]
		void DonateInteraction ([NullAllowed] Action<NSError> completion);

		[Static]
		[Async]
		[Export ("deleteAllInteractionsWithCompletion:")]
		void DeleteAllInteractions ([NullAllowed] Action<NSError> completion);

		[Static]
		[Async]
		[Export ("deleteInteractionsWithIdentifiers:completion:")]
		void DeleteInteractions (string [] identifiers, [NullAllowed] Action<NSError> completion);

		[Static]
		[Async]
		[Export ("deleteInteractionsWithGroupIdentifier:completion:")]
		void DeleteGroupedInteractions (string groupIdentifier, [NullAllowed] Action<NSError> completion);

		[Export ("intent", ArgumentSemantic.Copy)]
		INIntent Intent { get; }

		[NullAllowed, Export ("intentResponse", ArgumentSemantic.Copy)]
		INIntentResponse IntentResponse { get; }

		[Export ("intentHandlingStatus")]
		INIntentHandlingStatus IntentHandlingStatus { get; }

		[Export ("direction", ArgumentSemantic.Assign)]
		INInteractionDirection Direction { get; set; }

		[NullAllowed, Export ("dateInterval", ArgumentSemantic.Copy)]
		NSDateInterval DateInterval { get; set; }

		[Export ("identifier")]
		string Identifier { get; set; }

		[NullAllowed, Export ("groupIdentifier")]
		string GroupIdentifier { get; set; }

		// From INParameter.h INInteraction ()

		[Internal]
		[NoMac, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("parameterValueForParameter:")]
		IntPtr _GetParameterValue (INParameter parameter);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INListRideOptionsIntent {

		[Export ("initWithPickupLocation:dropOffLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation);

		[NullAllowed, Export ("pickupLocation", ArgumentSemantic.Copy)]
		CLPlacemark PickupLocation { get; }

		[NullAllowed, Export ("dropOffLocation", ArgumentSemantic.Copy)]
		CLPlacemark DropOffLocation { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INListRideOptionsIntentHandling {

		[Abstract]
		[Export ("handleListRideOptions:completion:")]
		void HandleListRideOptions (INListRideOptionsIntent intent, Action<INListRideOptionsIntentResponse> completion);

		[Export ("confirmListRideOptions:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmListRideOptions
#endif
		(INListRideOptionsIntent intent, Action<INListRideOptionsIntentResponse> completion);

		[Export ("resolvePickupLocationForListRideOptions:withCompletion:")]
		void ResolvePickupLocation (INListRideOptionsIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveDropOffLocationForListRideOptions:withCompletion:")]
		void ResolveDropOffLocation (INListRideOptionsIntent intent, Action<INPlacemarkResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INListRideOptionsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INListRideOptionsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INListRideOptionsIntentResponseCode Code { get; }

		[NullAllowed, Export ("rideOptions", ArgumentSemantic.Copy)]
		INRideOption [] RideOptions { get; set; }

		[NullAllowed, Export ("paymentMethods", ArgumentSemantic.Copy)]
		INPaymentMethod [] PaymentMethods { get; set; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMessage : NSCopying, NSSecureCoding {

		[Watch (9, 0), NoMac, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:serviceName:audioMessageFile:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType, [NullAllowed] string serviceName, [NullAllowed] INFile audioMessageFile);

		[Watch (6, 1), NoMac, iOS (13, 2)]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:serviceName:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType, [NullAllowed] string serviceName);

		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType);

		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:messageType:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, INMessageType messageType);

		[Export ("initWithIdentifier:content:dateSent:sender:recipients:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients);

		[Export ("identifier")]
		string Identifier { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("conversationIdentifier")]
		string ConversationIdentifier { get; }

		[NullAllowed, Export ("content")]
		string Content { get; }

		[NullAllowed, Export ("dateSent", ArgumentSemantic.Copy)]
		NSDate DateSent { get; }

		[NullAllowed, Export ("sender", ArgumentSemantic.Copy)]
		INPerson Sender { get; }

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }

		[MacCatalyst (13, 1)]
		[Export ("messageType")]
		INMessageType MessageType { get; }

		[Watch (6, 1), NoMac, iOS (13, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("serviceName")]
		string ServiceName { get; }

		[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'AttachmentFile' instead.")]
		[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'AttachmentFile' instead.")]
		[Obsoleted (PlatformName.WatchOS, 10, 0, message: "Use 'AttachmentFile' instead.")]
		[Watch (9, 0), NoMac, iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("audioMessageFile", ArgumentSemantic.Copy)]
		INFile AudioMessageFile { get; }

		[Watch (10, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:serviceName:attachmentFiles:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType, [NullAllowed] string serviceName, [NullAllowed] INFile [] attachmentFiles);

		[Watch (10, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:serviceName:linkMetadata:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, [NullAllowed] string serviceName, [NullAllowed] INMessageLinkMetadata linkMetadata);

		[Watch (10, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:serviceName:messageType:numberOfAttachments:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, [NullAllowed] string serviceName, INMessageType messageType, [NullAllowed] NSNumber numberOfAttachments);

		[NullAllowed]
		[Watch (10, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("attachmentFiles", ArgumentSemantic.Copy)]
		INFile [] AttachmentFiles { get; }

		[NullAllowed, BindAs (typeof (int))]
		[Watch (10, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("numberOfAttachments", ArgumentSemantic.Copy)]
		NSNumber NumberOfAttachments { get; }

		[NullAllowed]
		[Watch (10, 0), NoMac, iOS (17, 0)]
		[Export ("linkMetadata", ArgumentSemantic.Copy)]
		INMessageLinkMetadata LinkMetadata { get; }

	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeOptionsResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedMessageAttributeOptions:")]
		INMessageAttributeOptionsResolutionResult SuccessWithResolvedMessageAttributeOptions (INMessageAttributeOptions resolvedMessageAttributeOptions);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeOptionsResolutionResult SuccessWithResolvedValue (INMessageAttributeOptions resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithMessageAttributeOptionsToConfirm:")]
		INMessageAttributeOptionsResolutionResult ConfirmationRequiredWithMessageAttributeOptionsToConfirm (INMessageAttributeOptions messageAttributeOptionsToConfirm);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INMessageAttributeOptionsResolutionResult ConfirmationRequiredWithValueToConfirm (INMessageAttributeOptions valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INMessageAttributeOptionsResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INMessageAttributeOptionsResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INMessageAttributeOptionsResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMessageAttributeOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMessageAttributeOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

#if NET
	[NoMac]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedMessageAttribute:")]
		INMessageAttributeResolutionResult SuccessWithResolvedMessageAttribute (INMessageAttribute resolvedMessageAttribute);

		[Internal]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeResolutionResult SuccessWithResolvedValue (INMessageAttribute resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithMessageAttributeToConfirm:")]
		INMessageAttributeResolutionResult ConfirmationRequiredWithMessageAttributeToConfirm (INMessageAttribute messageAttributeToConfirm);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INMessageAttributeResolutionResult ConfirmationRequiredWithValueToConfirm (INMessageAttribute valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INMessageAttributeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INMessageAttributeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INMessageAttributeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMessageAttributeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMessageAttributeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INPauseWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INPauseWorkoutIntentHandling {

		[Abstract]
		[Export ("handlePauseWorkout:completion:")]
		void HandlePauseWorkout (INPauseWorkoutIntent intent, Action<INPauseWorkoutIntentResponse> completion);

		[Export ("confirmPauseWorkout:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmPauseWorkout
#endif
		(INPauseWorkoutIntent intent, Action<INPauseWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForPauseWorkout:withCompletion:")]
		void ResolveWorkoutName (INPauseWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INPauseWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INPauseWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INPauseWorkoutIntentResponseCode Code { get; }
	}

	[NoTV, Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPaymentMethod : NSCopying, NSSecureCoding {

		[Export ("initWithType:name:identificationHint:icon:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INPaymentMethodType type, [NullAllowed] string name, [NullAllowed] string identificationHint, [NullAllowed] INImage icon);

		[Export ("type", ArgumentSemantic.Assign)]
		INPaymentMethodType Type { get; }

		[NullAllowed, Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("identificationHint")]
		string IdentificationHint { get; }

		[NullAllowed, Export ("icon", ArgumentSemantic.Copy)]
		INImage Icon { get; }

		[Static]
		[Export ("applePayPaymentMethod")]
		INPaymentMethod ApplePayPaymentMethod { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPaymentRecord : NSCopying, NSSecureCoding {

		[Export ("initWithPayee:payer:currencyAmount:paymentMethod:note:status:feeAmount:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson payee, [NullAllowed] INPerson payer, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] INPaymentMethod paymentMethod, [NullAllowed] string note, INPaymentStatus status, [NullAllowed] INCurrencyAmount feeAmount);

		[Export ("initWithPayee:payer:currencyAmount:paymentMethod:note:status:")]
		NativeHandle Constructor ([NullAllowed] INPerson payee, [NullAllowed] INPerson payer, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] INPaymentMethod paymentMethod, [NullAllowed] string note, INPaymentStatus status);

		[NullAllowed, Export ("payee", ArgumentSemantic.Copy)]
		INPerson Payee { get; }

		[NullAllowed, Export ("payer", ArgumentSemantic.Copy)]
		INPerson Payer { get; }

		[NullAllowed, Export ("currencyAmount", ArgumentSemantic.Copy)]
		INCurrencyAmount CurrencyAmount { get; }

		[NullAllowed, Export ("paymentMethod", ArgumentSemantic.Copy)]
		INPaymentMethod PaymentMethod { get; }

		[NullAllowed, Export ("note")]
		string Note { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INPaymentStatus Status { get; }

		[NullAllowed, Export ("feeAmount", ArgumentSemantic.Copy)]
		INCurrencyAmount FeeAmount { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPerson : NSCopying, NSSecureCoding, INSpeakable {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:")]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier);

		[Watch (7, 0), iOS (14, 0), Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:relationship:")]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, [NullAllowed] string relationship);

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isMe:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, bool isMe);

		[Internal]
		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isMe:suggestionType:")]
		IntPtr InitWithMe (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, bool isMe, INPersonSuggestionType suggestionType);

		[Internal]
		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isContactSuggestion:suggestionType:")]
		IntPtr InitWithContactSuggestion (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, bool isContactSuggestion, INPersonSuggestionType suggestionType);

		[NullAllowed, Export ("personHandle", ArgumentSemantic.Copy)]
		INPersonHandle PersonHandle { get; }

		[NullAllowed, Export ("nameComponents", ArgumentSemantic.Copy)]
		NSPersonNameComponents NameComponents { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		INImage Image { get; }

		[NullAllowed, Export ("contactIdentifier")]
		string ContactIdentifier { get; }

		[NullAllowed, Export ("customIdentifier")]
		string CustomIdentifier { get; }

		[NullAllowed, Export ("relationship"), Protected]
		NSString WeakRelationship { get; }

		[Wrap ("INPersonRelationshipExtensions.GetValue (WeakRelationship)")]
		INPersonRelationship Relationship { get; }

		// Inlined from INInteraction (INPerson) Category

		[NullAllowed, Export ("aliases", ArgumentSemantic.Copy)]
		INPersonHandle [] Aliases { get; }

		[Export ("suggestionType")]
		INPersonSuggestionType SuggestionType { get; }

		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:aliases:suggestionType:")]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, [NullAllowed] INPersonHandle [] aliases, INPersonSuggestionType suggestionType);

		// Inlined from INInteraction (INPerson) Category

		[Unavailable (PlatformName.MacOSX)]
		[MacCatalyst (13, 1)]
		[Export ("siriMatches", ArgumentSemantic.Copy), NullAllowed]
		INPerson [] SiriMatches { get; }

		[MacCatalyst (13, 1)]
		[Export ("isMe")]
		bool IsMe { get; }

		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("contactSuggestion")]
		bool ContactSuggestion { [Bind ("isContactSuggestion")] get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPersonHandle : NSCopying, NSSecureCoding {

		[Export ("value"), NullAllowed]
		string Value { get; }

		[Export ("type")]
		INPersonHandleType Type { get; }

		[MacCatalyst (13, 1)]
		[Export ("label"), NullAllowed, Protected]
		NSString WeakLabel { get; }

		[MacCatalyst (13, 1)]
		[Wrap ("INPersonHandleLabelExtensions.GetValue (WeakLabel)")]
		INPersonHandleLabel Label { get; }

		[MacCatalyst (13, 1)]
		[Wrap ("this (value, type, label.GetConstant ())")]
		NativeHandle Constructor (string value, INPersonHandleType type, INPersonHandleLabel label);

		[DesignatedInitializer]
		[MacCatalyst (13, 1)]
		[Export ("initWithValue:type:label:"), Protected]
		NativeHandle Constructor ([NullAllowed] string value, INPersonHandleType type, [NullAllowed] NSString stringLabel);

		[Export ("initWithValue:type:")]
		NativeHandle Constructor ([NullAllowed] string value, INPersonHandleType type);
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPersonResolutionResult {

		[Static]
		[Export ("successWithResolvedPerson:")]
		INPersonResolutionResult GetSuccess (INPerson resolvedPerson);

		[Static]
		[Export ("disambiguationWithPeopleToDisambiguate:")]
		INPersonResolutionResult GetDisambiguation (INPerson [] peopleToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithPersonToConfirm:")]
		INPersonResolutionResult GetConfirmationRequired ([NullAllowed] INPerson personToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPersonResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPersonResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPersonResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPersonResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPersonResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPlacemarkResolutionResult {

		[Static]
		[Export ("successWithResolvedPlacemark:")]
		INPlacemarkResolutionResult GetSuccess (CLPlacemark resolvedPlacemark);

		[Static]
		[Export ("disambiguationWithPlacemarksToDisambiguate:")]
		INPlacemarkResolutionResult GetDisambiguation (CLPlacemark [] placemarksToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithPlacemarkToConfirm:")]
		INPlacemarkResolutionResult GetConfirmationRequired ([NullAllowed] CLPlacemark placemarkToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPlacemarkResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPlacemarkResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPlacemarkResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPlacemarkResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPlacemarkResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
#if NET || TVOS || __MACCATALYST__
	[DisableDefaultCtor]
#endif
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INPreferences {

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("siriAuthorizationStatus")]
		INSiriAuthorizationStatus SiriAuthorizationStatus { get; }

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async]
		[Export ("requestSiriAuthorization:")]
		void RequestSiriAuthorization (Action<INSiriAuthorizationStatus> handler);

		[Static]
		[Export ("siriLanguageCode")]
		string SiriLanguageCode { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPriceRange : NSCopying, NSSecureCoding {

		[Export ("initWithRangeBetweenPrice:andPrice:currencyCode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDecimalNumber firstPrice, NSDecimalNumber secondPrice, string currencyCode);

		[Internal]
		[Export ("initWithMaximumPrice:currencyCode:")]
		IntPtr InitWithMaximumPrice (NSDecimalNumber maximumPrice, string currencyCode);

		[Internal]
		[Export ("initWithMinimumPrice:currencyCode:")]
		IntPtr InitWithMinimumPrice (NSDecimalNumber minimumPrice, string currencyCode);

		[Export ("initWithPrice:currencyCode:")]
		NativeHandle Constructor (NSDecimalNumber price, string currencyCode);

		[NullAllowed, Export ("minimumPrice")]
		NSDecimalNumber MinimumPrice { get; }

		[NullAllowed, Export ("maximumPrice")]
		NSDecimalNumber MaximumPrice { get; }

		[Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRadioTypeResolutionResult bound
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRadioTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedRadioType:")]
		INRadioTypeResolutionResult SuccessWithResolvedRadioType (INRadioType resolvedRadioType);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INRadioTypeResolutionResult SuccessWithResolvedValue (INRadioType resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithRadioTypeToConfirm:")]
		INRadioTypeResolutionResult ConfirmationRequiredWithRadioTypeToConfirm (INRadioType radioTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INRadioTypeResolutionResult ConfirmationRequiredWithValueToConfirm (INRadioType valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRadioTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRadioTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRadioTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRadioTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRadioTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRelativeReferenceResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedRelativeReference:")]
		INRelativeReferenceResolutionResult SuccessWithResolvedRelativeReference (INRelativeReference resolvedRelativeReference);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INRelativeReferenceResolutionResult SuccessWithResolvedValue (INRelativeReference resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithRelativeReferenceToConfirm:")]
		INRelativeReferenceResolutionResult ConfirmationRequiredWithRelativeReferenceToConfirm (INRelativeReference relativeReferenceToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INRelativeReferenceResolutionResult ConfirmationRequiredWithValueToConfirm (INRelativeReference valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRelativeReferenceResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRelativeReferenceResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRelativeReferenceResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRelativeReferenceResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRelativeReferenceResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRelativeSettingResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedRelativeSetting:")]
		INRelativeSettingResolutionResult SuccessWithResolvedRelativeSetting (INRelativeSetting resolvedRelativeSetting);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INRelativeSettingResolutionResult SuccessWithResolvedValue (INRelativeSetting resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithRelativeSettingToConfirm:")]
		INRelativeSettingResolutionResult ConfirmationRequiredWithRelativeSettingToConfirm (INRelativeSetting relativeSettingToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INRelativeSettingResolutionResult ConfirmationRequiredWithValueToConfirm (INRelativeSetting valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRelativeSettingResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRelativeSettingResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRelativeSettingResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRelativeSettingResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRelativeSettingResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INRequestPaymentIntent {

		[Export ("initWithPayer:currencyAmount:note:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson payer, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] string note);

		[NullAllowed, Export ("payer", ArgumentSemantic.Copy)]
		INPerson Payer { get; }

		[NullAllowed, Export ("currencyAmount", ArgumentSemantic.Copy)]
		INCurrencyAmount CurrencyAmount { get; }

		[NullAllowed, Export ("note")]
		string Note { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INRequestPaymentIntentHandling {

		[Abstract]
		[Export ("handleRequestPayment:completion:")]
		void HandleRequestPayment (INRequestPaymentIntent intent, Action<INRequestPaymentIntentResponse> completion);

		[Export ("confirmRequestPayment:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRequestPayment
#endif
		(INRequestPaymentIntent intent, Action<INRequestPaymentIntentResponse> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Export ("resolvePayerForRequestPayment:withCompletion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolvePayerForRequestPayment:completion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INRequestPaymentPayerResolutionResult> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveCurrencyAmount (INRequestPaymentIntent, Action<INRequestPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveCurrencyAmount (INRequestPaymentIntent, Action<INRequestPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveCurrencyAmount (INRequestPaymentIntent, Action<INRequestPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Export ("resolveCurrencyAmountForRequestPayment:withCompletion:")]
		void ResolveCurrencyAmount (INRequestPaymentIntent intent, Action<INCurrencyAmountResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCurrencyAmountForRequestPayment:completion:")]
		void ResolveCurrencyAmount (INRequestPaymentIntent intent, Action<INRequestPaymentCurrencyAmountResolutionResult> completion);

		[Export ("resolveNoteForRequestPayment:withCompletion:")]
		void ResolveNote (INRequestPaymentIntent intent, Action<INStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INRequestPaymentIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRequestPaymentIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INRequestPaymentIntentResponseCode Code { get; }

		[NullAllowed, Export ("paymentRecord", ArgumentSemantic.Copy)]
		INPaymentRecord PaymentRecord { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INRequestRideIntent {

		[Deprecated (PlatformName.iOS, 10, 3, message: "Use the INDateComponentsRange overload")]
		[Unavailable (PlatformName.WatchOS)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the INDateComponentsRange overload")]
		[Export ("initWithPickupLocation:dropOffLocation:rideOptionName:partySize:paymentMethod:")]
		NativeHandle Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation, [NullAllowed] INSpeakableString rideOptionName, [NullAllowed] NSNumber partySize, [NullAllowed] INPaymentMethod paymentMethod);

		[MacCatalyst (13, 1)]
		[Export ("initWithPickupLocation:dropOffLocation:rideOptionName:partySize:paymentMethod:scheduledPickupTime:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation, [NullAllowed] INSpeakableString rideOptionName, [NullAllowed] NSNumber partySize, [NullAllowed] INPaymentMethod paymentMethod, [NullAllowed] INDateComponentsRange scheduledPickupTime);

		[NullAllowed, Export ("pickupLocation", ArgumentSemantic.Copy)]
		CLPlacemark PickupLocation { get; }

		[NullAllowed, Export ("dropOffLocation", ArgumentSemantic.Copy)]
		CLPlacemark DropOffLocation { get; }

		[NullAllowed, Export ("rideOptionName", ArgumentSemantic.Copy)]
		INSpeakableString RideOptionName { get; }

		[NullAllowed, Export ("partySize", ArgumentSemantic.Copy)]
		NSNumber PartySize { get; }

		[NullAllowed, Export ("paymentMethod", ArgumentSemantic.Copy)]
		INPaymentMethod PaymentMethod { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("scheduledPickupTime", ArgumentSemantic.Copy)]
		INDateComponentsRange ScheduledPickupTime { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INRequestRideIntentHandling {

		[Abstract]
		[Export ("handleRequestRide:completion:")]
		void HandleRequestRide (INRequestRideIntent intent, Action<INRequestRideIntentResponse> completion);

		[Export ("confirmRequestRide:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRequestRide
#endif
		(INRequestRideIntent intent, Action<INRequestRideIntentResponse> completion);

		[Export ("resolvePickupLocationForRequestRide:withCompletion:")]
		void ResolvePickupLocation (INRequestRideIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveDropOffLocationForRequestRide:withCompletion:")]
		void ResolveDropOffLocation (INRequestRideIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveRideOptionNameForRequestRide:withCompletion:")]
		void ResolveRideOptionName (INRequestRideIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolvePartySizeForRequestRide:withCompletion:")]
		void ResolvePartySize (INRequestRideIntent intent, Action<INIntegerResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveScheduledPickupTimeForRequestRide:withCompletion:")]
		void ResolveScheduledPickupTime (INRequestRideIntent intent, Action<INDateComponentsRangeResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INRequestRideIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRequestRideIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INRequestRideIntentResponseCode Code { get; }

		[NullAllowed, Export ("rideStatus", ArgumentSemantic.Copy)]
		INRideStatus RideStatus { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INRestaurant : NSSecureCoding, NSCopying {

		[Export ("initWithLocation:name:vendorIdentifier:restaurantIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CLLocation location, string name, string vendorIdentifier, string restaurantIdentifier);

		[Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("vendorIdentifier")]
		string VendorIdentifier { get; set; }

		[Export ("restaurantIdentifier")]
		string RestaurantIdentifier { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPerson))]
	[DisableDefaultCtor] // The base type, INPerson, has no default ctor.
	interface INRestaurantGuest {

		[Export ("initWithNameComponents:phoneNumber:emailAddress:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string phoneNumber, [NullAllowed] string emailAddress);

		[NullAllowed, Export ("phoneNumber")]
		string PhoneNumber { get; set; }

		[NullAllowed, Export ("emailAddress")]
		string EmailAddress { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INRestaurantGuestDisplayPreferences : NSSecureCoding, NSCopying {

		[Export ("nameFieldFirstNameOptional")]
		bool NameFieldFirstNameOptional { get; set; }

		[Export ("nameFieldLastNameOptional")]
		bool NameFieldLastNameOptional { get; set; }

		[Export ("nameFieldShouldBeDisplayed")]
		bool NameFieldShouldBeDisplayed { get; set; }

		[Export ("emailAddressFieldShouldBeDisplayed")]
		bool EmailAddressFieldShouldBeDisplayed { get; set; }

		[Export ("phoneNumberFieldShouldBeDisplayed")]
		bool PhoneNumberFieldShouldBeDisplayed { get; set; }

		[Export ("nameEditable")]
		bool NameEditable { get; set; }

		[Export ("emailAddressEditable")]
		bool EmailAddressEditable { get; set; }

		[Export ("phoneNumberEditable")]
		bool PhoneNumberEditable { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INRestaurantGuestResolutionResult {

		[Static]
		[Export ("successWithResolvedRestaurantGuest:")]
		INRestaurantGuestResolutionResult GetSuccess (INRestaurantGuest resolvedRestaurantGuest);

		[Static]
		[Export ("disambiguationWithRestaurantGuestsToDisambiguate:")]
		INRestaurantGuestResolutionResult GetDisambiguation (INRestaurantGuest [] restaurantGuestsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithRestaurantGuestToConfirm:")]
		INRestaurantGuestResolutionResult GetConfirmationRequired ([NullAllowed] INRestaurantGuest restaurantGuestToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRestaurantGuestResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRestaurantGuestResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRestaurantGuestResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRestaurantGuestResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRestaurantGuestResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INRestaurantOffer : NSSecureCoding, NSCopying {

		[Export ("offerTitleText")]
		string OfferTitleText { get; set; }

		[Export ("offerDetailText")]
		string OfferDetailText { get; set; }

		[Export ("offerIdentifier")]
		string OfferIdentifier { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INRestaurantReservationBooking : NSSecureCoding, NSCopying {

		[Export ("initWithRestaurant:bookingDate:partySize:bookingIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRestaurant restaurant, NSDate bookingDate, nuint partySize, string bookingIdentifier);

		[Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[NullAllowed, Export ("bookingDescription")]
		string BookingDescription { get; set; }

		[Export ("bookingDate", ArgumentSemantic.Copy)]
		NSDate BookingDate { get; set; }

		[Export ("partySize")]
		nuint PartySize { get; set; }

		[Export ("bookingIdentifier")]
		string BookingIdentifier { get; set; }

		[Export ("bookingAvailable")]
		bool BookingAvailable { [Bind ("isBookingAvailable")] get; set; }

		[NullAllowed, Export ("offers", ArgumentSemantic.Copy)]
		INRestaurantOffer [] Offers { get; set; }

		[Export ("requiresManualRequest")]
		bool RequiresManualRequest { get; set; }

		[Export ("requiresEmailAddress")]
		bool RequiresEmailAddress { get; set; }

		[Export ("requiresName")]
		bool RequiresName { get; set; }

		[Export ("requiresPhoneNumber")]
		bool RequiresPhoneNumber { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INRestaurantReservationBooking))]
	interface INRestaurantReservationUserBooking : NSCopying {

		[Export ("initWithRestaurant:bookingDate:partySize:bookingIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRestaurant restaurant, NSDate bookingDate, nuint partySize, string bookingIdentifier);

		[Export ("initWithRestaurant:bookingDate:partySize:bookingIdentifier:guest:status:dateStatusModified:")]
		NativeHandle Constructor (INRestaurant restaurant, NSDate bookingDate, nuint partySize, string bookingIdentifier, INRestaurantGuest guest, INRestaurantReservationUserBookingStatus status, NSDate dateStatusModified);

		[Export ("guest", ArgumentSemantic.Copy)]
		INRestaurantGuest Guest { get; set; }

		[NullAllowed, Export ("advisementText")]
		string AdvisementText { get; set; }

		[NullAllowed, Export ("selectedOffer", ArgumentSemantic.Copy)]
		INRestaurantOffer SelectedOffer { get; set; }

		[NullAllowed, Export ("guestProvidedSpecialRequestText")]
		string GuestProvidedSpecialRequestText { get; set; }

		[Export ("status", ArgumentSemantic.Assign)]
		INRestaurantReservationUserBookingStatus Status { get; set; }

		[Export ("dateStatusModified", ArgumentSemantic.Copy)]
		NSDate DateStatusModified { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INRestaurantResolutionResult {

		[Static]
		[Export ("successWithResolvedRestaurant:")]
		INRestaurantResolutionResult GetSuccess (INRestaurant resolvedRestaurant);

		[Static]
		[Export ("disambiguationWithRestaurantsToDisambiguate:")]
		INRestaurantResolutionResult GetDisambiguation (INRestaurant [] restaurantsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithRestaurantToConfirm:")]
		INRestaurantResolutionResult GetConfirmationRequired ([NullAllowed] INRestaurant restaurantToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRestaurantResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRestaurantResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRestaurantResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRestaurantResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRestaurantResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INResumeWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INResumeWorkoutIntentHandling {

		[Abstract]
		[Export ("handleResumeWorkout:completion:")]
		void HandleResumeWorkout (INResumeWorkoutIntent intent, Action<INResumeWorkoutIntentResponse> completion);

		[Export ("confirmResumeWorkout:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmResumeWorkout
#endif
		(INResumeWorkoutIntent intent, Action<INResumeWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForResumeWorkout:withCompletion:")]
		void ResolveWorkoutName (INResumeWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INResumeWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INResumeWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INResumeWorkoutIntentResponseCode Code { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRideCompletionStatus : NSCopying, NSSecureCoding {

		[Static]
		[Export ("completed")]
		INRideCompletionStatus GetCompleted ();

		[Static]
		[Export ("completedWithSettledPaymentAmount:")]
		INRideCompletionStatus GetSettledPaymentAmount (INCurrencyAmount settledPaymentAmount);

		[Static]
		[Export ("completedWithOutstandingPaymentAmount:")]
		INRideCompletionStatus GetOutstandingPaymentAmount (INCurrencyAmount outstandingPaymentAmount);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("completedWithOutstandingFeedbackType:")]
		INRideCompletionStatus GetCompleted (INRideFeedbackTypeOptions feedbackType);

		[Static]
		[Export ("canceledByService")]
		INRideCompletionStatus GetCanceledByService ();

		[Static]
		[Export ("canceledByUser")]
		INRideCompletionStatus GetCanceledByUser ();

		[Static]
		[Export ("canceledMissedPickup")]
		INRideCompletionStatus GetCanceledMissedPickup ();

		[NullAllowed, Export ("completionUserActivity", ArgumentSemantic.Strong)]
		NSUserActivity CompletionUserActivity { get; set; }

		[Export ("completed")]
		bool Completed { [Bind ("isCompleted")] get; }

		[Export ("canceled")]
		bool Canceled { [Bind ("isCanceled")] get; }

		[Export ("missedPickup")]
		bool MissedPickup { [Bind ("isMissedPickup")] get; }

		[NullAllowed, Export ("paymentAmount", ArgumentSemantic.Strong)]
		INCurrencyAmount PaymentAmount { get; }

		[MacCatalyst (13, 1)]
		[Export ("feedbackType", ArgumentSemantic.Assign)]
		INRideFeedbackTypeOptions FeedbackType { get; }

		[Export ("outstanding")]
		bool Outstanding { [Bind ("isOutstanding")] get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("defaultTippingOptions", ArgumentSemantic.Strong)]
		NSSet<INCurrencyAmount> DefaultTippingOptions { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRideDriver bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPerson))]
	[DisableDefaultCtor] // xcode 8.2 beta 1 -> NSInvalidArgumentException Reason: *** -[__NSPlaceholderDictionary initWithObjects:forKeys:count:]: attempt to insert nil object from objects[1]
	interface INRideDriver : NSCopying, NSSecureCoding {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:rating:phoneNumber:")]
		[Deprecated (PlatformName.iOS, 10, 2, message: "Use the overload signature instead.")]
		[Unavailable (PlatformName.WatchOS)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload signature instead.")]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string rating, [NullAllowed] string phoneNumber);

		[Export ("initWithPhoneNumber:nameComponents:displayName:image:rating:")]
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		NativeHandle Constructor (string phoneNumber, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string rating);

		[NullAllowed, Export ("rating")]
		string Rating { get; }

		[NullAllowed, Export ("phoneNumber")]
		string PhoneNumber { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRideFareLineItem : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:price:currencyCode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, NSDecimalNumber price, string currencyCode);

		[Export ("title")]
		string Title { get; }

		[Export ("price")]
		NSDecimalNumber Price { get; }

		[Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRideOption : NSCopying, NSSecureCoding {

		[Export ("initWithName:estimatedPickupDate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, NSDate estimatedPickupDate);

		[Export ("name")]
		string Name { get; set; }

		[Export ("estimatedPickupDate", ArgumentSemantic.Copy)]
		NSDate EstimatedPickupDate { get; set; }

		[NullAllowed, Export ("priceRange", ArgumentSemantic.Copy)]
		INPriceRange PriceRange { get; set; }

		[Internal]
		[NullAllowed, Export ("usesMeteredFare", ArgumentSemantic.Copy)]
		NSNumber _UsesMeteredFare { get; set; }

		[NullAllowed, Export ("disclaimerMessage")]
		string DisclaimerMessage { get; set; }

		[NullAllowed, Export ("availablePartySizeOptions", ArgumentSemantic.Copy)]
		INRidePartySizeOption [] AvailablePartySizeOptions { get; set; }

		[NullAllowed, Export ("availablePartySizeOptionsSelectionPrompt")]
		string AvailablePartySizeOptionsSelectionPrompt { get; set; }

		[NullAllowed, Export ("specialPricing")]
		string SpecialPricing { get; set; }

		[NullAllowed, Export ("specialPricingBadgeImage", ArgumentSemantic.Copy)]
		INImage SpecialPricingBadgeImage { get; set; }

		[NullAllowed, Export ("fareLineItems", ArgumentSemantic.Copy)]
		INRideFareLineItem [] FareLineItems { get; set; }

		[NullAllowed, Export ("userActivityForBookingInApplication", ArgumentSemantic.Strong)]
		NSUserActivity UserActivityForBookingInApplication { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRidePartySizeOption : NSCopying, NSSecureCoding {

		[Export ("initWithPartySizeRange:sizeDescription:priceRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSRange partySizeRange, string sizeDescription, [NullAllowed] INPriceRange priceRange);

		[Export ("partySizeRange")]
		NSRange PartySizeRange { get; }

		[Export ("sizeDescription")]
		string SizeDescription { get; }

		[NullAllowed, Export ("priceRange")]
		INPriceRange PriceRange { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // xcode 8.2 beta 1 -> NSInvalidArgumentException Reason: *** -[__NSPlaceholderDictionary initWithObjects:forKeys:count:]: attempt to insert nil object from objects[1]
	interface INRideStatus : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("rideIdentifier")]
		string RideIdentifier { get; set; }

		[Export ("phase", ArgumentSemantic.Assign)]
		INRidePhase Phase { get; set; }

		[NullAllowed, Export ("completionStatus", ArgumentSemantic.Copy)]
		INRideCompletionStatus CompletionStatus { get; set; }

		[NullAllowed, Export ("vehicle", ArgumentSemantic.Copy)]
		INRideVehicle Vehicle { get; set; }

		[NullAllowed, Export ("driver", ArgumentSemantic.Copy)]
		INRideDriver Driver { get; set; }

		[NullAllowed, Export ("estimatedPickupDate", ArgumentSemantic.Copy)]
		NSDate EstimatedPickupDate { get; set; }

		[NullAllowed, Export ("estimatedDropOffDate", ArgumentSemantic.Copy)]
		NSDate EstimatedDropOffDate { get; set; }

		[NullAllowed, Export ("estimatedPickupEndDate", ArgumentSemantic.Copy)]
		NSDate EstimatedPickupEndDate { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("scheduledPickupTime", ArgumentSemantic.Copy)]
		INDateComponentsRange ScheduledPickupTime { get; set; }

		[NullAllowed, Export ("pickupLocation", ArgumentSemantic.Copy)]
		CLPlacemark PickupLocation { get; set; }

		[NullAllowed, Export ("waypoints", ArgumentSemantic.Copy)]
		CLPlacemark [] Waypoints { get; set; }

		[NullAllowed, Export ("dropOffLocation", ArgumentSemantic.Copy)]
		CLPlacemark DropOffLocation { get; set; }

		[NullAllowed, Export ("rideOption", ArgumentSemantic.Copy)]
		INRideOption RideOption { get; set; }

		[NullAllowed, Export ("userActivityForCancelingInApplication", ArgumentSemantic.Strong)]
		NSUserActivity UserActivityForCancelingInApplication { get; set; }

		[NullAllowed, Export ("additionalActionActivities", ArgumentSemantic.Copy)]
		NSUserActivity [] AdditionalActionActivities { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INRideVehicle : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; set; }

		[NullAllowed, Export ("registrationPlate")]
		string RegistrationPlate { get; set; }

		[NullAllowed, Export ("manufacturer")]
		string Manufacturer { get; set; }

		[NullAllowed, Export ("model")]
		string Model { get; set; }

		[NullAllowed, Export ("mapAnnotationImage", ArgumentSemantic.Copy)]
		INImage MapAnnotationImage { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSaveProfileInCarIntent {

		[Deprecated (PlatformName.iOS, 10, 2)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("initWithProfileNumber:profileLabel:"), Internal]
		IntPtr InitWithProfileNumberLabel ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel);

		[MacCatalyst (13, 1)]
		[Export ("initWithProfileNumber:profileName:"), Internal]
		IntPtr InitWithProfileNumberName ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileName);

		[NullAllowed, Export ("profileNumber", ArgumentSemantic.Copy)]
		NSNumber ProfileNumber { get; }

		[Deprecated (PlatformName.iOS, 10, 2, message: "Use 'ProfileName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ProfileName' instead.")]
		[NullAllowed, Export ("profileLabel")]
		string ProfileLabel { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("profileName")]
		string ProfileName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSaveProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSaveProfileInCar:completion:")]
		void HandleSaveProfileInCar (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("confirmSaveProfileInCar:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSaveProfileInCar
#endif
		(INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSaveProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSaveProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveProfileNameForSaveProfileInCar:withCompletion:")]
		void ResolveProfileName (INSaveProfileInCarIntent intent, Action<INStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSaveProfileInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSaveProfileInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSaveProfileInCarIntentResponseCode Code { get; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSearchCallHistoryIntent {

		[MacCatalyst (13, 1)]
		[Export ("initWithDateCreated:recipient:callCapabilities:callTypes:unseen:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities, INCallRecordTypeOptions callTypes, [NullAllowed] NSNumber unseen);

		[MacCatalyst (13, 1)]
		[Wrap ("this (dateCreated, recipient, callCapabilities, callTypes, new NSNumber (unseen))")]
		NativeHandle Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities, INCallRecordTypeOptions callTypes, bool unseen);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Export ("initWithCallType:dateCreated:recipient:callCapabilities:")]
		NativeHandle Constructor (INCallRecordType callType, [NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CallTypes' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CallTypes' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CallTypes' instead.")]
		[Export ("callType", ArgumentSemantic.Assign)]
		INCallRecordType CallType { get; }

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		INDateComponentsRange DateCreated { get; }

		[NullAllowed, Export ("recipient", ArgumentSemantic.Copy)]
		INPerson Recipient { get; }

		[Export ("callCapabilities", ArgumentSemantic.Assign)]
		INCallCapabilityOptions CallCapabilities { get; }

		[MacCatalyst (13, 1)]
		[Export ("callTypes", ArgumentSemantic.Assign)]
		INCallRecordTypeOptions CallTypes { get; }

		[Protected]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("unseen", ArgumentSemantic.Copy)]
		NSNumber WeakUnseen { get; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSearchCallHistoryIntentHandling {

		[Abstract]
		[Export ("handleSearchCallHistory:completion:")]
		void HandleSearchCallHistory (INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Export ("confirmSearchCallHistory:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSearchCallHistory
#endif
		(INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveCallTypes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveCallTypes' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveCallTypes' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveCallTypes' instead.")]
		[Export ("resolveCallTypeForSearchCallHistory:withCompletion:")]
		void ResolveCallType (INSearchCallHistoryIntent intent, Action<INCallRecordTypeResolutionResult> completion);

		[Export ("resolveDateCreatedForSearchCallHistory:withCompletion:")]
		void ResolveDateCreated (INSearchCallHistoryIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveRecipientForSearchCallHistory:withCompletion:")]
		void ResolveRecipient (INSearchCallHistoryIntent intent, Action<INPersonResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCallTypesForSearchCallHistory:withCompletion:")]
		void ResolveCallTypes (INSearchCallHistoryIntent intent, Action<INCallRecordTypeOptionsResolutionResult> completion);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("resolveUnseenForSearchCallHistory:withCompletion:")]
		void ResolveUnseen (INSearchCallHistoryIntent intent, Action<INBooleanResolutionResult> completion);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchCallHistoryIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSearchCallHistoryIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchCallHistoryIntentResponseCode Code { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("callRecords", ArgumentSemantic.Copy)]
		INCallRecord [] CallRecords { get; set; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSearchForMessagesIntent {

		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:speakableGroupNames:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] INSpeakableString [] speakableGroupNames);

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:speakableGroupNames:conversationIdentifiers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] INSpeakableString [] speakableGroupNames, [NullAllowed] string [] conversationIdentifiers);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:groupNames:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] string [] groupNames);

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }

		[Export ("recipientsOperator", ArgumentSemantic.Assign)]
		INConditionalOperator RecipientsOperator { get; }

		[NullAllowed, Export ("senders", ArgumentSemantic.Copy)]
		INPerson [] Senders { get; }

		[Export ("sendersOperator", ArgumentSemantic.Assign)]
		INConditionalOperator SendersOperator { get; }

		[NullAllowed, Export ("searchTerms", ArgumentSemantic.Copy)]
		string [] SearchTerms { get; }

		[Export ("searchTermsOperator", ArgumentSemantic.Assign)]
		INConditionalOperator SearchTermsOperator { get; }

		[Export ("attributes", ArgumentSemantic.Assign)]
		INMessageAttributeOptions Attributes { get; }

		[NullAllowed, Export ("dateTimeRange", ArgumentSemantic.Copy)]
		INDateComponentsRange DateTimeRange { get; }

		[NullAllowed, Export ("identifiers", ArgumentSemantic.Copy)]
		string [] Identifiers { get; }

		[Export ("identifiersOperator", ArgumentSemantic.Assign)]
		INConditionalOperator IdentifiersOperator { get; }

		[NullAllowed, Export ("notificationIdentifiers", ArgumentSemantic.Copy)]
		string [] NotificationIdentifiers { get; }

		[Export ("notificationIdentifiersOperator", ArgumentSemantic.Assign)]
		INConditionalOperator NotificationIdentifiersOperator { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SpeakableGroupNames' instead.")]
		[NullAllowed, Export ("groupNames", ArgumentSemantic.Copy)]
		string [] GroupNames { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Export ("groupNamesOperator", ArgumentSemantic.Assign)]
		INConditionalOperator GroupNamesOperator { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("speakableGroupNames", ArgumentSemantic.Copy)]
		INSpeakableString [] SpeakableGroupNames { get; }

		[MacCatalyst (13, 1)]
		[Export ("speakableGroupNamesOperator", ArgumentSemantic.Assign)]
		INConditionalOperator SpeakableGroupNamesOperator { get; }

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("conversationIdentifiers", ArgumentSemantic.Copy)]
		string [] ConversationIdentifiers { get; }

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Export ("conversationIdentifiersOperator", ArgumentSemantic.Assign)]
		INConditionalOperator ConversationIdentifiersOperator { get; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSearchForMessagesIntentHandling {

		[Abstract]
		[Export ("handleSearchForMessages:completion:")]
		void HandleSearchForMessages (INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion);

		[Export ("confirmSearchForMessages:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSearchForMessages
#endif
		(INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion);

		[Export ("resolveRecipientsForSearchForMessages:withCompletion:")]
		void ResolveRecipients (INSearchForMessagesIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveSendersForSearchForMessages:withCompletion:")]
		void ResolveSenders (INSearchForMessagesIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveAttributesForSearchForMessages:withCompletion:")]
		void ResolveAttributes (INSearchForMessagesIntent intent, Action<INMessageAttributeOptionsResolutionResult> completion);

		[Export ("resolveDateTimeRangeForSearchForMessages:withCompletion:")]
		void ResolveDateTimeRange (INSearchForMessagesIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Export ("resolveGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveGroupNames (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveSpeakableGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveSpeakableGroupNames (INSearchForMessagesIntent intent, Action<INSpeakableStringResolutionResult []> completion);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForMessagesIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSearchForMessagesIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForMessagesIntentResponseCode Code { get; }

		[NullAllowed, Export ("messages", ArgumentSemantic.Copy)]
		INMessage [] Messages { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSearchForPhotosIntent {

		[Export ("initWithDateCreated:locationCreated:albumName:searchTerms:includedAttributes:excludedAttributes:peopleInPhoto:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] CLPlacemark locationCreated, [NullAllowed] string albumName, [NullAllowed] string [] searchTerms, INPhotoAttributeOptions includedAttributes, INPhotoAttributeOptions excludedAttributes, [NullAllowed] INPerson [] peopleInPhoto);

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		INDateComponentsRange DateCreated { get; }

		[NullAllowed, Export ("locationCreated", ArgumentSemantic.Copy)]
		CLPlacemark LocationCreated { get; }

		[NullAllowed, Export ("albumName")]
		string AlbumName { get; }

		[NullAllowed, Export ("searchTerms", ArgumentSemantic.Copy)]
		string [] SearchTerms { get; }

		[Export ("searchTermsOperator", ArgumentSemantic.Assign)]
		INConditionalOperator SearchTermsOperator { get; }

		[Export ("includedAttributes", ArgumentSemantic.Assign)]
		INPhotoAttributeOptions IncludedAttributes { get; }

		[Export ("excludedAttributes", ArgumentSemantic.Assign)]
		INPhotoAttributeOptions ExcludedAttributes { get; }

		[NullAllowed, Export ("peopleInPhoto", ArgumentSemantic.Copy)]
		INPerson [] PeopleInPhoto { get; }

		[Export ("peopleInPhotoOperator", ArgumentSemantic.Assign)]
		INConditionalOperator PeopleInPhotoOperator { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSearchForPhotosIntentHandling {

		[Abstract]
		[Export ("handleSearchForPhotos:completion:")]
		void HandleSearchForPhotos (INSearchForPhotosIntent intent, Action<INSearchForPhotosIntentResponse> completion);

		[Export ("confirmSearchForPhotos:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSearchForPhotos
#endif
		(INSearchForPhotosIntent intent, Action<INSearchForPhotosIntentResponse> completion);

		[Export ("resolveDateCreatedForSearchForPhotos:withCompletion:")]
		void ResolveDateCreated (INSearchForPhotosIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveLocationCreatedForSearchForPhotos:withCompletion:")]
		void ResolveLocationCreated (INSearchForPhotosIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveAlbumNameForSearchForPhotos:withCompletion:")]
		void ResolveAlbumName (INSearchForPhotosIntent intent, Action<INStringResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveSearchTermsForSearchForPhotos:withCompletion:")]
		void ResolveSearchTerms (INSearchForPhotosIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolvePeopleInPhotoForSearchForPhotos:withCompletion:")]
		void ResolvePeopleInPhoto (INSearchForPhotosIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForPhotosIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSearchForPhotosIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForPhotosIntentResponseCode Code { get; }

		[NullAllowed, Export ("searchResultsCount", ArgumentSemantic.Copy)]
		NSNumber SearchResultsCount { get; set; }
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSendMessageIntent : UNNotificationContentProviding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithRecipients:outgoingMessageType:content:speakableGroupName:conversationIdentifier:serviceName:sender:attachments:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, INOutgoingMessageType outgoingMessageType, [NullAllowed] string content, [NullAllowed] INSpeakableString speakableGroupName, [NullAllowed] string conversationIdentifier, [NullAllowed] string serviceName, [NullAllowed] INPerson sender, [NullAllowed] INSendMessageAttachment [] attachments);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[Export ("initWithRecipients:content:speakableGroupName:conversationIdentifier:serviceName:sender:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] INSpeakableString speakableGroupName, [NullAllowed] string conversationIdentifier, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Export ("initWithRecipients:content:groupName:serviceName:sender:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] string groupName, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("outgoingMessageType", ArgumentSemantic.Assign)]
		INOutgoingMessageType OutgoingMessageType { get; }

		[NullAllowed, Export ("content")]
		string Content { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("speakableGroupName", ArgumentSemantic.Copy)]
		INSpeakableString SpeakableGroupName { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("conversationIdentifier")]
		string ConversationIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SpeakableGroupNames' instead.")]
		[NullAllowed, Export ("groupName")]
		string GroupName { get; }

		[NullAllowed, Export ("serviceName")]
		string ServiceName { get; }

		[NullAllowed, Export ("sender", ArgumentSemantic.Copy)]
		INPerson Sender { get; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("attachments", ArgumentSemantic.Copy)]
		INSendMessageAttachment [] Attachments { get; }
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (INIntentDonationMetadata))]
	[DisableDefaultCtor]
	interface INSendMessageIntentDonationMetadata {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("mentionsCurrentUser")]
		bool MentionsCurrentUser { get; set; }

		[Export ("replyToCurrentUser")]
		bool ReplyToCurrentUser { [Bind ("isReplyToCurrentUser")] get; set; }

		[Export ("notifyRecipientAnyway")]
		bool NotifyRecipientAnyway { get; set; }

		[Export ("recipientCount")]
		nuint RecipientCount { get; set; }
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSendMessageIntentHandling {

		[Abstract]
		[Export ("handleSendMessage:completion:")]
		void HandleSendMessage (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

		[Export ("confirmSendMessage:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSendMessage
#endif
		(INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Export ("resolveRecipientsForSendMessage:withCompletion:")]
		void ResolveRecipients (INSendMessageIntent intent, Action<INPersonResolutionResult []> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveRecipientsForSendMessage:completion:")]
		void ResolveRecipients (INSendMessageIntent intent, Action<INSendMessageRecipientResolutionResult []> completion);

		[Export ("resolveContentForSendMessage:withCompletion:")]
		void ResolveContent (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Export ("resolveGroupNameForSendMessage:withCompletion:")]
		void ResolveGroupName (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("resolveOutgoingMessageTypeForSendMessage:withCompletion:")]
		void ResolveOutgoingMessageType (INSendMessageIntent intent, Action<INOutgoingMessageTypeResolutionResult> completion);

#if NET
		[NoMac] // The INSpeakableStringResolutionResult used as a parameter type is not available in macOS
#endif
		[MacCatalyst (13, 1)]
		[Export ("resolveSpeakableGroupNameForSendMessage:withCompletion:")]
		void ResolveSpeakableGroupName (INSendMessageIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSendMessageIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSendMessageIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSendMessageIntentResponseCode Code { get; }

#if NET
		[NoMac] // The INMessage type isn't available in macOS
#endif
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use the 'SentMessages' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use the 'SentMessages' property instead.")]
		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use the 'SentMessages' property instead.")]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sentMessage", ArgumentSemantic.Copy)]
		INMessage SentMessage { get; set; }

		[NoMac]
		[Watch (9, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[NullAllowed, Export ("sentMessages", ArgumentSemantic.Copy)]
		INMessage [] SentMessages { get; set; }
	}

	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSendPaymentIntent {

		[Export ("initWithPayee:currencyAmount:note:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson payee, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] string note);

		[NullAllowed, Export ("payee", ArgumentSemantic.Copy)]
		INPerson Payee { get; }

		[NullAllowed, Export ("currencyAmount", ArgumentSemantic.Copy)]
		INCurrencyAmount CurrencyAmount { get; }

		[NullAllowed, Export ("note")]
		string Note { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSendPaymentIntentHandling {

		[Abstract]
		[Export ("handleSendPayment:completion:")]
		void HandleSendPayment (INSendPaymentIntent intent, Action<INSendPaymentIntentResponse> completion);

		[Export ("confirmSendPayment:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSendPayment
#endif
		(INSendPaymentIntent intent, Action<INSendPaymentIntentResponse> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Export ("resolvePayeeForSendPayment:withCompletion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolvePayeeForSendPayment:completion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INSendPaymentPayeeResolutionResult> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveCurrencyAmount (INSendPaymentIntent, Action<INSendPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveCurrencyAmount (INSendPaymentIntent, Action<INSendPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveCurrencyAmount (INSendPaymentIntent, Action<INSendPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Export ("resolveCurrencyAmountForSendPayment:withCompletion:")]
		void ResolveCurrencyAmount (INSendPaymentIntent intent, Action<INCurrencyAmountResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCurrencyAmountForSendPayment:completion:")]
		void ResolveCurrencyAmount (INSendPaymentIntent intent, Action<INSendPaymentCurrencyAmountResolutionResult> completion);

		[Export ("resolveNoteForSendPayment:withCompletion:")]
		void ResolveNote (INSendPaymentIntent intent, Action<INStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSendPaymentIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSendPaymentIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSendPaymentIntentResponseCode Code { get; }

		[NullAllowed, Export ("paymentRecord", ArgumentSemantic.Copy)]
		INPaymentRecord PaymentRecord { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetAudioSourceInCarIntent {

		[Export ("initWithAudioSource:relativeAudioSourceReference:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INCarAudioSource audioSource, INRelativeReference relativeAudioSourceReference);

		[Export ("audioSource", ArgumentSemantic.Assign)]
		INCarAudioSource AudioSource { get; }

		[Export ("relativeAudioSourceReference", ArgumentSemantic.Assign)]
		INRelativeReference RelativeAudioSourceReference { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetAudioSourceInCarIntentHandling {

		[Abstract]
		[Export ("handleSetAudioSourceInCar:completion:")]
		void HandleSetAudioSourceInCar (INSetAudioSourceInCarIntent intent, Action<INSetAudioSourceInCarIntentResponse> completion);

		[Export ("confirmSetAudioSourceInCar:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetAudioSourceInCar
#endif
		(INSetAudioSourceInCarIntent intent, Action<INSetAudioSourceInCarIntentResponse> completion);

		[Export ("resolveAudioSourceForSetAudioSourceInCar:withCompletion:")]
		void ResolveAudioSource (INSetAudioSourceInCarIntent intent, Action<INCarAudioSourceResolutionResult> completion);

		[Export ("resolveRelativeAudioSourceReferenceForSetAudioSourceInCar:withCompletion:")]
		void ResolveRelativeAudioSourceReference (INSetAudioSourceInCarIntent intent, Action<INRelativeReferenceResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetAudioSourceInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetAudioSourceInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetAudioSourceInCarIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetClimateSettingsInCarIntent {

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Protected]
		[Export ("initWithEnableFan:enableAirConditioner:enableClimateControl:enableAutoMode:airCirculationMode:fanSpeedIndex:fanSpeedPercentage:relativeFanSpeedSetting:temperature:relativeTemperatureSetting:climateZone:")]
		NativeHandle Constructor ([NullAllowed] NSNumber enableFan, [NullAllowed] NSNumber enableAirConditioner, [NullAllowed] NSNumber enableClimateControl, [NullAllowed] NSNumber enableAutoMode, INCarAirCirculationMode airCirculationMode, [NullAllowed] NSNumber fanSpeedIndex, [NullAllowed] NSNumber fanSpeedPercentage, INRelativeSetting relativeFanSpeedSetting, [NullAllowed] NSMeasurement<NSUnitTemperature> temperature, INRelativeSetting relativeTemperatureSetting, INCarSeat climateZone);

		[MacCatalyst (13, 1)]
		[Export ("initWithEnableFan:enableAirConditioner:enableClimateControl:enableAutoMode:airCirculationMode:fanSpeedIndex:fanSpeedPercentage:relativeFanSpeedSetting:temperature:relativeTemperatureSetting:climateZone:carName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (bool?))] NSNumber enableFan, [NullAllowed][BindAs (typeof (bool?))] NSNumber enableAirConditioner, [NullAllowed][BindAs (typeof (bool?))] NSNumber enableClimateControl, [NullAllowed][BindAs (typeof (bool?))] NSNumber enableAutoMode, INCarAirCirculationMode airCirculationMode, [NullAllowed][BindAs (typeof (int?))] NSNumber fanSpeedIndex, [NullAllowed][BindAs (typeof (double?))] NSNumber fanSpeedPercentage, INRelativeSetting relativeFanSpeedSetting, [NullAllowed] NSMeasurement<NSUnitTemperature> temperature, INRelativeSetting relativeTemperatureSetting, INCarSeat climateZone, [NullAllowed] INSpeakableString carName);

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableFan", ArgumentSemantic.Copy)]
		NSNumber EnableFan { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableAirConditioner", ArgumentSemantic.Copy)]
		NSNumber EnableAirConditioner { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableClimateControl", ArgumentSemantic.Copy)]
		NSNumber EnableClimateControl { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableAutoMode", ArgumentSemantic.Copy)]
		NSNumber EnableAutoMode { get; }

		[Export ("airCirculationMode", ArgumentSemantic.Assign)]
		INCarAirCirculationMode AirCirculationMode { get; }

#if NET
		[BindAs (typeof (int?))]
#endif
		[NullAllowed, Export ("fanSpeedIndex", ArgumentSemantic.Copy)]
		NSNumber FanSpeedIndex { get; }

#if NET
		[BindAs (typeof (double?))]
#endif
		[NullAllowed, Export ("fanSpeedPercentage", ArgumentSemantic.Copy)]
		NSNumber FanSpeedPercentage { get; }

		[Export ("relativeFanSpeedSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeFanSpeedSetting { get; }

		[NullAllowed, Export ("temperature", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitTemperature> Temperature { get; }

		[Export ("relativeTemperatureSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeTemperatureSetting { get; }

		[Export ("climateZone", ArgumentSemantic.Assign)]
		INCarSeat ClimateZone { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("carName", ArgumentSemantic.Copy)]
		INSpeakableString CarName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetClimateSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetClimateSettingsInCar:completion:")]
		void HandleSetClimateSettingsInCar (INSetClimateSettingsInCarIntent intent, Action<INSetClimateSettingsInCarIntentResponse> completion);

		[Export ("confirmSetClimateSettingsInCar:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetClimateSettingsInCar
#endif
		(INSetClimateSettingsInCarIntent intent, Action<INSetClimateSettingsInCarIntentResponse> completion);

		[Export ("resolveEnableFanForSetClimateSettingsInCar:withCompletion:")]
		void ResolveEnableFan (INSetClimateSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveEnableAirConditionerForSetClimateSettingsInCar:withCompletion:")]
		void ResolveEnableAirConditioner (INSetClimateSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveEnableClimateControlForSetClimateSettingsInCar:withCompletion:")]
		void ResolveEnableClimateControl (INSetClimateSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveEnableAutoModeForSetClimateSettingsInCar:withCompletion:")]
		void ResolveEnableAutoMode (INSetClimateSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveAirCirculationModeForSetClimateSettingsInCar:withCompletion:")]
		void ResolveAirCirculationMode (INSetClimateSettingsInCarIntent intent, Action<INCarAirCirculationModeResolutionResult> completion);

		[Export ("resolveFanSpeedIndexForSetClimateSettingsInCar:withCompletion:")]
		void ResolveFanSpeedIndex (INSetClimateSettingsInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolveFanSpeedPercentageForSetClimateSettingsInCar:withCompletion:")]
		void ResolveFanSpeedPercentage (INSetClimateSettingsInCarIntent intent, Action<INDoubleResolutionResult> completion);

		[Export ("resolveRelativeFanSpeedSettingForSetClimateSettingsInCar:withCompletion:")]
		void ResolveRelativeFanSpeedSetting (INSetClimateSettingsInCarIntent intent, Action<INRelativeSettingResolutionResult> completion);

		[Export ("resolveTemperatureForSetClimateSettingsInCar:withCompletion:")]
		void ResolveTemperature (INSetClimateSettingsInCarIntent intent, Action<INTemperatureResolutionResult> completion);

		[Export ("resolveRelativeTemperatureSettingForSetClimateSettingsInCar:withCompletion:")]
		void ResolveRelativeTemperatureSetting (INSetClimateSettingsInCarIntent intent, Action<INRelativeSettingResolutionResult> completion);

		[Export ("resolveClimateZoneForSetClimateSettingsInCar:withCompletion:")]
		void ResolveClimateZone (INSetClimateSettingsInCarIntent intent, Action<INCarSeatResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCarNameForSetClimateSettingsInCar:withCompletion:")]
		void ResolveCarName (INSetClimateSettingsInCarIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetClimateSettingsInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetClimateSettingsInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetClimateSettingsInCarIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetDefrosterSettingsInCarIntent {

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Protected]
		[Export ("initWithEnable:defroster:")]
		NativeHandle Constructor ([NullAllowed] NSNumber enable, INCarDefroster defroster);

		[MacCatalyst (13, 1)]
		[Export ("initWithEnable:defroster:carName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (bool?))] NSNumber enable, INCarDefroster defroster, [NullAllowed] INSpeakableString carName);

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enable", ArgumentSemantic.Copy)]
		NSNumber Enable { get; }

		[Export ("defroster", ArgumentSemantic.Assign)]
		INCarDefroster Defroster { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("carName", ArgumentSemantic.Copy)]
		INSpeakableString CarName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetDefrosterSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetDefrosterSettingsInCar:completion:")]
		void HandleSetDefrosterSettingsInCar (INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

		[Export ("confirmSetDefrosterSettingsInCar:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetDefrosterSettingsInCar
#endif
		(INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

		[Export ("resolveEnableForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveEnable (INSetDefrosterSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveDefrosterForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveDefroster (INSetDefrosterSettingsInCarIntent intent, Action<INCarDefrosterResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCarNameForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveCarName (INSetDefrosterSettingsInCarIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetDefrosterSettingsInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetDefrosterSettingsInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetDefrosterSettingsInCarIntentResponseCode Code { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSetMessageAttributeIntent {

		[Export ("initWithIdentifiers:attribute:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string [] identifiers, INMessageAttribute attribute);

		[NullAllowed, Export ("identifiers", ArgumentSemantic.Copy)]
		string [] Identifiers { get; }

		[Export ("attribute", ArgumentSemantic.Assign)]
		INMessageAttribute Attribute { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSetMessageAttributeIntentHandling {

		[Abstract]
		[Export ("handleSetMessageAttribute:completion:")]
		void HandleSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("confirmSetMessageAttribute:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetMessageAttribute
#endif
		(INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("resolveAttributeForSetMessageAttribute:withCompletion:")]
		void ResolveAttribute (INSetMessageAttributeIntent intent, Action<INMessageAttributeResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetMessageAttributeIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetMessageAttributeIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetMessageAttributeIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetProfileInCarIntent {

		[Deprecated (PlatformName.iOS, 10, 2)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("initWithProfileNumber:profileLabel:defaultProfile:"), Internal]
		IntPtr InitWithProfileNumberLabel ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel, [NullAllowed] NSNumber defaultProfile);

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Export ("initWithProfileNumber:profileName:defaultProfile:"), Internal]
		IntPtr InitWithProfileNumberName ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileName, [NullAllowed] NSNumber defaultProfile);

		[MacCatalyst (13, 1)]
		[Export ("initWithProfileNumber:profileName:defaultProfile:carName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (int?))] NSNumber profileNumber, [NullAllowed] string profileName, [NullAllowed][BindAs (typeof (bool?))] NSNumber defaultProfile, [NullAllowed] INSpeakableString carName);

#if NET
		[BindAs (typeof (int?))]
#endif
		[NullAllowed, Export ("profileNumber", ArgumentSemantic.Copy)]
		NSNumber ProfileNumber { get; }

		[Deprecated (PlatformName.iOS, 10, 2, message: "Use 'ProfileName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ProfileName' instead.")]
		[NullAllowed, Export ("profileLabel")]
		string ProfileLabel { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("profileName")]
		string ProfileName { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("defaultProfile", ArgumentSemantic.Copy)]
		NSNumber DefaultProfile { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("carName", ArgumentSemantic.Copy)]
		INSpeakableString CarName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSetProfileInCar:completion:")]
		void HandleSetProfileInCar (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("confirmSetProfileInCar:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetProfileInCar
#endif
		(INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSetProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSetProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Deprecated (PlatformName.iOS, 11, 0, message: "The property doesn't need to be resolved.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "The property doesn't need to be resolved.")]
		[Export ("resolveDefaultProfileForSetProfileInCar:withCompletion:")]
		void ResolveDefaultProfile (INSetProfileInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCarNameForSetProfileInCar:withCompletion:")]
		void ResolveCarName (INSetProfileInCarIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveProfileNameForSetProfileInCar:withCompletion:")]
		void ResolveProfileName (INSetProfileInCarIntent intent, Action<INStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetProfileInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetProfileInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetProfileInCarIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetRadioStationIntent {

		[Export ("initWithRadioType:frequency:stationName:channel:presetNumber:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INRadioType radioType, [NullAllowed] NSNumber frequency, [NullAllowed] string stationName, [NullAllowed] string channel, [NullAllowed] NSNumber presetNumber);

		[Export ("radioType", ArgumentSemantic.Assign)]
		INRadioType RadioType { get; }

		[NullAllowed, Export ("frequency", ArgumentSemantic.Copy)]
		NSNumber Frequency { get; }

		[NullAllowed, Export ("stationName")]
		string StationName { get; }

		[NullAllowed, Export ("channel")]
		string Channel { get; }

		[NullAllowed, Export ("presetNumber", ArgumentSemantic.Copy)]
		NSNumber PresetNumber { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetRadioStationIntentHandling {

		[Abstract]
		[Export ("handleSetRadioStation:completion:")]
		void HandleSetRadioStation (INSetRadioStationIntent intent, Action<INSetRadioStationIntentResponse> completion);

		[Export ("confirmSetRadioStation:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetRadioStation
#endif
		(INSetRadioStationIntent intent, Action<INSetRadioStationIntentResponse> completion);

		[Export ("resolveRadioTypeForSetRadioStation:withCompletion:")]
		void ResolveRadioType (INSetRadioStationIntent intent, Action<INRadioTypeResolutionResult> completion);

		[Export ("resolveFrequencyForSetRadioStation:withCompletion:")]
		void ResolveFrequency (INSetRadioStationIntent intent, Action<INDoubleResolutionResult> completion);

		[Export ("resolveStationNameForSetRadioStation:withCompletion:")]
		void ResolveStationName (INSetRadioStationIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolveChannelForSetRadioStation:withCompletion:")]
		void ResolveChannel (INSetRadioStationIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolvePresetNumberForSetRadioStation:withCompletion:")]
		void ResolvePresetNumber (INSetRadioStationIntent intent, Action<INIntegerResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetRadioStationIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetRadioStationIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetRadioStationIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetSeatSettingsInCarIntent {

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Protected] // allow subclassing
		[Export ("initWithEnableHeating:enableCooling:enableMassage:seat:level:relativeLevelSetting:")]
		NativeHandle Constructor ([NullAllowed] NSNumber enableHeating, [NullAllowed] NSNumber enableCooling, [NullAllowed] NSNumber enableMassage, INCarSeat seat, [NullAllowed] NSNumber level, INRelativeSetting relativeLevelSetting);

		[MacCatalyst (13, 1)]
		[Export ("initWithEnableHeating:enableCooling:enableMassage:seat:level:relativeLevelSetting:carName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (bool?))] NSNumber enableHeating, [NullAllowed][BindAs (typeof (bool?))] NSNumber enableCooling, [NullAllowed][BindAs (typeof (bool?))] NSNumber enableMassage, INCarSeat seat, [NullAllowed][BindAs (typeof (int?))] NSNumber level, INRelativeSetting relativeLevelSetting, [NullAllowed] INSpeakableString carName);

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableHeating", ArgumentSemantic.Copy)]
		NSNumber EnableHeating { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableCooling", ArgumentSemantic.Copy)]
		NSNumber EnableCooling { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("enableMassage", ArgumentSemantic.Copy)]
		NSNumber EnableMassage { get; }

		[Export ("seat", ArgumentSemantic.Assign)]
		INCarSeat Seat { get; }

#if NET
		[BindAs (typeof (int?))]
#endif
		[NullAllowed, Export ("level", ArgumentSemantic.Copy)]
		NSNumber Level { get; }

		[Export ("relativeLevelSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeLevelSetting { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("carName", ArgumentSemantic.Copy)]
		INSpeakableString CarName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetSeatSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetSeatSettingsInCar:completion:")]
		void HandleSetSeatSettingsInCar (INSetSeatSettingsInCarIntent intent, Action<INSetSeatSettingsInCarIntentResponse> completion);

		[Export ("confirmSetSeatSettingsInCar:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetSeatSettingsInCar
#endif
		(INSetSeatSettingsInCarIntent intent, Action<INSetSeatSettingsInCarIntentResponse> completion);

		[Export ("resolveEnableHeatingForSetSeatSettingsInCar:withCompletion:")]
		void ResolveEnableHeating (INSetSeatSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveEnableCoolingForSetSeatSettingsInCar:withCompletion:")]
		void ResolveEnableCooling (INSetSeatSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveEnableMassageForSetSeatSettingsInCar:withCompletion:")]
		void ResolveEnableMassage (INSetSeatSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveSeatForSetSeatSettingsInCar:withCompletion:")]
		void ResolveSeat (INSetSeatSettingsInCarIntent intent, Action<INCarSeatResolutionResult> completion);

		[Export ("resolveLevelForSetSeatSettingsInCar:withCompletion:")]
		void ResolveLevel (INSetSeatSettingsInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolveRelativeLevelSettingForSetSeatSettingsInCar:withCompletion:")]
		void ResolveRelativeLevelSetting (INSetSeatSettingsInCarIntent intent, Action<INRelativeSettingResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCarNameForSetSeatSettingsInCar:withCompletion:")]
		void ResolveCarName (INSetSeatSettingsInCarIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetSeatSettingsInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetSeatSettingsInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetSeatSettingsInCarIntentResponseCode Code { get; }
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INShareFocusStatusIntent {
		[Export ("initWithFocusStatus:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INFocusStatus focusStatus);

		[NullAllowed, Export ("focusStatus", ArgumentSemantic.Copy)]
		INFocusStatus FocusStatus { get; }
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface INShareFocusStatusIntentHandling {
		[Abstract]
		[Export ("handleShareFocusStatus:completion:")]
		void HandleShareFocusStatus (INShareFocusStatusIntent intent, Action<INShareFocusStatusIntentResponse> completion);

		[Export ("confirmShareFocusStatus:completion:")]
		void ConfirmShareFocusStatus (INShareFocusStatusIntent intent, Action<INShareFocusStatusIntentResponse> completion);
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum INShareFocusStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INShareFocusStatusIntentResponse {
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INShareFocusStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INShareFocusStatusIntentResponseCode Code { get; }
	}

	interface IINSpeakable { }

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSpeakable {

		[Abstract]
		[Export ("spokenPhrase")]
		string SpokenPhrase { get; }

		[Abstract]
		[NullAllowed, Export ("pronunciationHint")]
		string PronunciationHint { get; }

		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("vocabularyIdentifier")]
		string VocabularyIdentifier { get; }

		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("alternativeSpeakableMatches")]
		IINSpeakable [] AlternativeSpeakableMatches { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'VocabularyIdentifier' instead.")]
#if !NET
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'VocabularyIdentifier' instead.")]
#endif
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VocabularyIdentifier' instead.")]
#if !NET
		[Abstract]
#endif
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpeakableString : INSpeakable, NSCopying, NSSecureCoding {

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithVocabularyIdentifier:spokenPhrase:pronunciationHint:")]
		IntPtr InitWithVocabularyIdentifier (string vocabularyIdentifier, string spokenPhrase, [NullAllowed] string pronunciationHint);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithIdentifier:spokenPhrase:pronunciationHint:")]
		IntPtr InitWithIdentifier (string identifier, string spokenPhrase, [NullAllowed] string pronunciationHint);

		[MacCatalyst (13, 1)]
		[Export ("initWithSpokenPhrase:")]
		NativeHandle Constructor (string spokenPhrase);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INSpeakableStringResolutionResult {

		[Static]
		[Export ("successWithResolvedString:")]
		INSpeakableStringResolutionResult GetSuccess (INSpeakableString resolvedString);

		[Static]
		[Export ("disambiguationWithStringsToDisambiguate:")]
		INSpeakableStringResolutionResult GetDisambiguation (INSpeakableString [] stringsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithStringToConfirm:")]
		INSpeakableStringResolutionResult GetConfirmationRequired ([NullAllowed] INSpeakableString stringToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSpeakableStringResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSpeakableStringResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSpeakableStringResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSpeakableStringResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSpeakableStringResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntent' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntent' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntent' instead.")]
	[BaseType (typeof (INIntent))]
	interface INStartAudioCallIntent {

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Export ("initWithContacts:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] contacts);

		[MacCatalyst (13, 1)]
		[Export ("initWithDestinationType:contacts:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INCallDestinationType destinationType, [NullAllowed] INPerson [] contacts);

		[MacCatalyst (13, 1)]
		[Export ("destinationType", ArgumentSemantic.Assign)]
		INCallDestinationType DestinationType { get; }

		[NullAllowed, Export ("contacts", ArgumentSemantic.Copy)]
		INPerson [] Contacts { get; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntentHandling' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentHandling' instead.")]
	[Protocol]
	interface INStartAudioCallIntentHandling {

		[Abstract]
		[Export ("handleStartAudioCall:completion:")]
		void HandleStartAudioCall (INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

		[Export ("confirmStartAudioCall:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmStartAudioCall
#endif
		(INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveDestinationTypeForStartAudioCall:withCompletion:")]
		void ResolveDestinationType (INStartAudioCallIntent intent, Action<INCallDestinationTypeResolutionResult> completion);

		[Export ("resolveContactsForStartAudioCall:withCompletion:")]
		void ResolveContacts (INStartAudioCallIntent intent, Action<INPersonResolutionResult []> completion);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponse' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntentResponse' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentResponse' instead.")]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartAudioCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INStartAudioCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartAudioCallIntentResponseCode Code { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INStartPhotoPlaybackIntent {

		[Export ("initWithDateCreated:locationCreated:albumName:searchTerms:includedAttributes:excludedAttributes:peopleInPhoto:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] CLPlacemark locationCreated, [NullAllowed] string albumName, [NullAllowed] string [] searchTerms, INPhotoAttributeOptions includedAttributes, INPhotoAttributeOptions excludedAttributes, [NullAllowed] INPerson [] peopleInPhoto);

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		INDateComponentsRange DateCreated { get; }

		[NullAllowed, Export ("locationCreated", ArgumentSemantic.Copy)]
		CLPlacemark LocationCreated { get; }

		[NullAllowed, Export ("albumName")]
		string AlbumName { get; }

		[NullAllowed, Export ("searchTerms", ArgumentSemantic.Copy)]
		string [] SearchTerms { get; }

		[Export ("searchTermsOperator", ArgumentSemantic.Assign)]
		INConditionalOperator SearchTermsOperator { get; }

		[Export ("includedAttributes", ArgumentSemantic.Assign)]
		INPhotoAttributeOptions IncludedAttributes { get; }

		[Export ("excludedAttributes", ArgumentSemantic.Assign)]
		INPhotoAttributeOptions ExcludedAttributes { get; }

		[NullAllowed, Export ("peopleInPhoto", ArgumentSemantic.Copy)]
		INPerson [] PeopleInPhoto { get; }

		[Export ("peopleInPhotoOperator", ArgumentSemantic.Assign)]
		INConditionalOperator PeopleInPhotoOperator { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INStartPhotoPlaybackIntentHandling {

		[Abstract]
		[Export ("handleStartPhotoPlayback:completion:")]
		void HandleStartPhotoPlayback (INStartPhotoPlaybackIntent intent, Action<INStartPhotoPlaybackIntentResponse> completion);

		[Export ("confirmStartPhotoPlayback:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmStartPhotoPlayback
#endif
		(INStartPhotoPlaybackIntent intent, Action<INStartPhotoPlaybackIntentResponse> completion);

		[Export ("resolveDateCreatedForStartPhotoPlayback:withCompletion:")]
		void ResolveDateCreated (INStartPhotoPlaybackIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveLocationCreatedForStartPhotoPlayback:withCompletion:")]
		void ResolveLocationCreated (INStartPhotoPlaybackIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveAlbumNameForStartPhotoPlayback:withCompletion:")]
		void ResolveAlbumName (INStartPhotoPlaybackIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolvePeopleInPhotoForStartPhotoPlayback:withCompletion:")]
		void ResolvePeopleInPhoto (INStartPhotoPlaybackIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartPhotoPlaybackIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INStartPhotoPlaybackIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartPhotoPlaybackIntentResponseCode Code { get; }

		[NullAllowed, Export ("searchResultsCount", ArgumentSemantic.Copy)]
		NSNumber SearchResultsCount { get; set; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntent' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntent' instead.")]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntent' instead.")]
	[BaseType (typeof (INIntent))]
	interface INStartVideoCallIntent {

		[Export ("initWithContacts:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson [] contacts);

		[NullAllowed, Export ("contacts", ArgumentSemantic.Copy)]
		INPerson [] Contacts { get; }
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'INStartCallIntentHandling' instead.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentHandling' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntentHandling' instead.")]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentHandling' instead.")]
	[Protocol]
	interface INStartVideoCallIntentHandling {

		[Abstract]
		[Export ("handleStartVideoCall:completion:")]
		void HandleStartVideoCall (INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

		[Export ("confirmStartVideoCall:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmStartVideoCall
#endif
		(INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

		[Export ("resolveContactsForStartVideoCall:withCompletion:")]
		void ResolveContacts (INStartVideoCallIntent intent, Action<INPersonResolutionResult []> completion);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponse' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'INStartCallIntentResponse' instead.")]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentResponse' instead.")]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartVideoCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INStartVideoCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartVideoCallIntentResponseCode Code { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INStartWorkoutIntent {

		[Protected]
		[Export ("initWithWorkoutName:goalValue:workoutGoalUnitType:workoutLocationType:isOpenEnded:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString workoutName, [NullAllowed] NSNumber goalValue, INWorkoutGoalUnitType workoutGoalUnitType, INWorkoutLocationType workoutLocationType, [NullAllowed] NSNumber isOpenEnded);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }

		[NullAllowed, Export ("goalValue", ArgumentSemantic.Copy)]
		NSNumber GoalValue { get; }

		[Export ("workoutGoalUnitType", ArgumentSemantic.Assign)]
		INWorkoutGoalUnitType WorkoutGoalUnitType { get; }

		[Export ("workoutLocationType", ArgumentSemantic.Assign)]
		INWorkoutLocationType WorkoutLocationType { get; }

		[Internal]
		[NullAllowed, Export ("isOpenEnded", ArgumentSemantic.Copy)]
		NSNumber _IsOpenEnded { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INStartWorkoutIntentHandling {

		[Abstract]
		[Export ("handleStartWorkout:completion:")]
		void HandleStartWorkout (INStartWorkoutIntent intent, Action<INStartWorkoutIntentResponse> completion);

		[Export ("confirmStartWorkout:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmStartWorkout
#endif
		(INStartWorkoutIntent intent, Action<INStartWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForStartWorkout:withCompletion:")]
		void ResolveWorkoutName (INStartWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveGoalValueForStartWorkout:withCompletion:")]
		void ResolveGoalValue (INStartWorkoutIntent intent, Action<INDoubleResolutionResult> completion);

		[Export ("resolveWorkoutGoalUnitTypeForStartWorkout:withCompletion:")]
		void ResolveWorkoutGoalUnitType (INStartWorkoutIntent intent, Action<INWorkoutGoalUnitTypeResolutionResult> completion);

		[Export ("resolveWorkoutLocationTypeForStartWorkout:withCompletion:")]
		void ResolveWorkoutLocationType (INStartWorkoutIntent intent, Action<INWorkoutLocationTypeResolutionResult> completion);

		[Export ("resolveIsOpenEndedForStartWorkout:withCompletion:")]
		void ResolveIsOpenEnded (INStartWorkoutIntent intent, Action<INBooleanResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INStartWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartWorkoutIntentResponseCode Code { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INStringResolutionResult {

		[Static]
		[Export ("successWithResolvedString:")]
		INStringResolutionResult GetSuccess (string resolvedString);

		[Static]
		[Export ("disambiguationWithStringsToDisambiguate:")]
		INStringResolutionResult GetDisambiguation (string [] stringsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithStringToConfirm:")]
		INStringResolutionResult GetConfirmationRequired ([NullAllowed] string stringToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INStringResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INStringResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INStringResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INStringResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INStringResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Deprecated (PlatformName.MacOSX, 12, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INTemperatureResolutionResult {

		[Static]
		[Export ("successWithResolvedTemperature:")]
		INTemperatureResolutionResult GetSuccess (NSMeasurement<NSUnitTemperature> resolvedTemperature);

		[Static]
		[Export ("disambiguationWithTemperaturesToDisambiguate:")]
		INTemperatureResolutionResult GetDisambiguation (NSMeasurement<NSUnitTemperature> [] temperaturesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithTemperatureToConfirm:")]
		INTemperatureResolutionResult GetConfirmationRequired ([NullAllowed] NSMeasurement<NSUnitTemperature> temperatureToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTemperatureResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTemperatureResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTemperatureResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTemperatureResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTemperatureResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INTermsAndConditions : NSSecureCoding, NSCopying {

		[Export ("initWithLocalizedTermsAndConditionsText:privacyPolicyURL:termsAndConditionsURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string localizedTermsAndConditionsText, [NullAllowed] NSUrl privacyPolicyUrl, [NullAllowed] NSUrl termsAndConditionsUrl);

		[Export ("localizedTermsAndConditionsText")]
		string LocalizedTermsAndConditionsText { get; }

		[NullAllowed, Export ("privacyPolicyURL")]
		NSUrl PrivacyPolicyUrl { get; }

		[NullAllowed, Export ("termsAndConditionsURL")]
		NSUrl TermsAndConditionsUrl { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[Watch (6, 0)]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INVocabulary {

		[Static]
		[Export ("sharedVocabulary")]
		INVocabulary SharedVocabulary { get; }

		[Export ("setVocabularyStrings:ofType:")]
		void SetVocabularyStrings (NSOrderedSet<NSString> vocabulary, INVocabularyStringType type);

		[MacCatalyst (13, 1)]
		[Export ("setVocabulary:ofType:")]
		void SetVocabulary (NSOrderedSet<IINSpeakable> vocabulary, INVocabularyStringType type);

		[Export ("removeAllVocabularyStrings")]
		void RemoveAllVocabularyStrings ();
	}

	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutGoalUnitTypeResolutionResult bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutGoalUnitTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedWorkoutGoalUnitType:")]
		INWorkoutGoalUnitTypeResolutionResult SuccessWithResolvedWorkoutGoalUnitType (INWorkoutGoalUnitType resolvedWorkoutGoalUnitType);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutGoalUnitTypeResolutionResult SuccessWithResolvedValue (INWorkoutGoalUnitType resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithWorkoutGoalUnitTypeToConfirm:")]
		INWorkoutGoalUnitTypeResolutionResult ConfirmationRequiredWithWorkoutGoalUnitTypeToConfirm (INWorkoutGoalUnitType workoutGoalUnitTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INWorkoutGoalUnitTypeResolutionResult ConfirmationRequiredWithValueToConfirm (INWorkoutGoalUnitType valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INWorkoutGoalUnitTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INWorkoutGoalUnitTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INWorkoutGoalUnitTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INWorkoutGoalUnitTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INWorkoutGoalUnitTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutLocationTypeResolutionResult bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutLocationTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedWorkoutLocationType:")]
		INWorkoutLocationTypeResolutionResult SuccessWithResolvedWorkoutLocationType (INWorkoutLocationType resolvedWorkoutLocationType);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutLocationTypeResolutionResult SuccessWithResolvedValue (INWorkoutLocationType resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithWorkoutLocationTypeToConfirm:")]
		INWorkoutLocationTypeResolutionResult ConfirmationRequiredWithWorkoutLocationTypeToConfirm (INWorkoutLocationType workoutLocationTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INWorkoutLocationTypeResolutionResult ConfirmationRequiredWithValueToConfirm (INWorkoutLocationType valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INWorkoutLocationTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INWorkoutLocationTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INWorkoutLocationTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INWorkoutLocationTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INWorkoutLocationTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSUserActivity))]
	interface NSUserActivity_IntentsAdditions {

		[Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("interaction")]
		INInteraction GetInteraction ();

		[Watch (5, 0), NoTV, Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("suggestedInvocationPhrase")]
		[return: NullAllowed]
		string GetSuggestedInvocationPhrase ();

		[Watch (5, 0), NoTV, Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setSuggestedInvocationPhrase:")]
		void SetSuggestedInvocationPhrase ([NullAllowed] string suggestedInvocationPhrase);

		[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("shortcutAvailability")]
		INShortcutAvailabilityOptions GetShortcutAvailability ();

		[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setShortcutAvailability:")]
		void SetShortcutAvailability (INShortcutAvailabilityOptions shortcutAvailabilityOptions);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INActivateCarSignalIntent {

		[DesignatedInitializer]
		[Export ("initWithCarName:signals:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString carName, INCarSignalOptions signals);

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }

		[Export ("signals", ArgumentSemantic.Assign)]
		INCarSignalOptions Signals { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INActivateCarSignalIntentHandling {

		[Abstract]
		[Export ("handleActivateCarSignal:completion:")]
		void HandleActivateCarSignal (INActivateCarSignalIntent intent, Action<INActivateCarSignalIntentResponse> completion);

		[Export ("confirmActivateCarSignal:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmActivateCarSignal
#endif
		(INActivateCarSignalIntent intent, Action<INActivateCarSignalIntentResponse> completion);

		[Export ("resolveCarNameForActivateCarSignal:withCompletion:")]
		void ResolveCarName (INActivateCarSignalIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveSignalsForActivateCarSignal:withCompletion:")]
		void ResolveSignals (INActivateCarSignalIntent intent, Action<INCarSignalOptionsResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INBillDetails : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithBillType:paymentStatus:billPayee:amountDue:minimumDue:lateFee:dueDate:paymentDate:")]
		NativeHandle Constructor (INBillType billType, INPaymentStatus paymentStatus, [NullAllowed] INBillPayee billPayee, [NullAllowed] INCurrencyAmount amountDue, [NullAllowed] INCurrencyAmount minimumDue, [NullAllowed] INCurrencyAmount lateFee, [NullAllowed] NSDateComponents dueDate, [NullAllowed] NSDateComponents paymentDate);

		[Export ("billType", ArgumentSemantic.Assign)]
		INBillType BillType { get; set; }

		[Export ("paymentStatus", ArgumentSemantic.Assign)]
		INPaymentStatus PaymentStatus { get; set; }

		[Export ("billPayee", ArgumentSemantic.Copy), NullAllowed]
		INBillPayee BillPayee { get; set; }

		[Export ("amountDue", ArgumentSemantic.Copy), NullAllowed]
		INCurrencyAmount AmountDue { get; set; }

		[Export ("minimumDue", ArgumentSemantic.Copy), NullAllowed]
		INCurrencyAmount MinimumDue { get; set; }

		[Export ("lateFee", ArgumentSemantic.Copy), NullAllowed]
		INCurrencyAmount LateFee { get; set; }

		[Export ("dueDate", ArgumentSemantic.Copy), NullAllowed]
		NSDateComponents DueDate { get; set; }

		[Export ("paymentDate", ArgumentSemantic.Copy), NullAllowed]
		NSDateComponents PaymentDate { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INBillPayee : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithNickname:number:organizationName:")]
		NativeHandle Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, [NullAllowed] INSpeakableString organizationName);

		[Export ("nickname", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString Nickname { get; }

		[Export ("accountNumber"), NullAllowed]
		string AccountNumber { get; }

		[Export ("organizationName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString OrganizationName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INBillPayeeResolutionResult {

		[Static]
		[Export ("successWithResolvedBillPayee:")]
		INBillPayeeResolutionResult GetSuccess (INBillPayee resolvedBillPayee);

		[Static]
		[Export ("disambiguationWithBillPayeesToDisambiguate:")]
		INBillPayeeResolutionResult GetDisambiguation (INBillPayee [] billPayeesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithBillPayeeToConfirm:")]
		INBillPayeeResolutionResult GetConfirmationRequired ([NullAllowed] INBillPayee billPayeeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INBillPayeeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INBillPayeeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INBillPayeeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBillPayeeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBillPayeeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INBillTypeResolutionResult {

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedValue:")]
		INBillTypeResolutionResult SuccessWithResolvedValue (INBillType resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedBillType:")]
		INBillTypeResolutionResult SuccessWithResolvedBillType (INBillType resolvedBillType);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INBillTypeResolutionResult ConfirmationRequiredWithValueToConfirm (INBillType valueToConfirm);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithBillTypeToConfirm:")]
		INBillTypeResolutionResult ConfirmationRequiredWithBillTypeToConfirm (INBillType billTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INBillTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INBillTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INBillTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBillTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBillTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarSignalOptionsResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarSignalOptions:")]
		INCarSignalOptionsResolutionResult SuccessWithResolvedCarSignalOptions (INCarSignalOptions resolvedCarSignalOptions);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarSignalOptionsResolutionResult SuccessWithResolvedValue (INCarSignalOptions resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarSignalOptionsToConfirm:")]
		INCarSignalOptionsResolutionResult ConfirmationRequiredWithCarSignalOptionsToConfirm (INCarSignalOptions carSignalOptionsToConfirm);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarSignalOptionsResolutionResult ConfirmationRequiredWithValueToConfirm (INCarSignalOptions valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCarSignalOptionsResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCarSignalOptionsResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCarSignalOptionsResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarSignalOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarSignalOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INGetCarLockStatusIntent {

		[DesignatedInitializer]
		[Export ("initWithCarName:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString carName);

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetCarLockStatusIntentHandling {

		[Abstract]
		[Export ("handleGetCarLockStatus:completion:")]
		void HandleGetCarLockStatus (INGetCarLockStatusIntent intent, Action<INGetCarLockStatusIntentResponse> completion);

		[Export ("confirmGetCarLockStatus:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmGetCarLockStatus
#endif
		(INGetCarLockStatusIntent intent, Action<INGetCarLockStatusIntentResponse> completion);

		[Export ("resolveCarNameForGetCarLockStatus:withCompletion:")]
		void ResolveCarName (INGetCarLockStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INGetCarLockStatusIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INGetCarLockStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetCarLockStatusIntentResponseCode Code { get; }

#if false // I wish BindAs was a thing right now
		[BindAs (typeof (bool?))]
#endif
		[Internal]
		[NullAllowed, Export ("locked", ArgumentSemantic.Copy)]
		NSNumber _Locked { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INGetCarPowerLevelStatusIntent {

		[DesignatedInitializer]
		[Export ("initWithCarName:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString carName);

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetCarPowerLevelStatusIntentHandling {

		[Abstract]
		[Export ("handleGetCarPowerLevelStatus:completion:")]
		void HandleGetCarPowerLevelStatus (INGetCarPowerLevelStatusIntent intent, Action<INGetCarPowerLevelStatusIntentResponse> completion);

		[NoWatch, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("startSendingUpdatesForGetCarPowerLevelStatus:toObserver:")]
		void StartSendingUpdates (INGetCarPowerLevelStatusIntent intent, IINGetCarPowerLevelStatusIntentResponseObserver observer);

		[NoWatch, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("stopSendingUpdatesForGetCarPowerLevelStatus:")]
		void StopSendingUpdates (INGetCarPowerLevelStatusIntent intent);

		[Export ("confirmGetCarPowerLevelStatus:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmGetCarPowerLevelStatus
#endif
		(INGetCarPowerLevelStatusIntent intent, Action<INGetCarPowerLevelStatusIntentResponse> completion);

		[Export ("resolveCarNameForGetCarPowerLevelStatus:withCompletion:")]
		void ResolveCarName (INGetCarPowerLevelStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	interface IINGetCarPowerLevelStatusIntentResponseObserver { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface INGetCarPowerLevelStatusIntentResponseObserver {

		[Abstract]
		[Export ("getCarPowerLevelStatusResponseDidUpdate:")]
		void DidUpdate (INGetCarPowerLevelStatusIntentResponse response);
	}

	// Just to please the generator that at this point does not know the hierarchy
	interface NSUnitLength : NSUnit { }

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INGetCarPowerLevelStatusIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INGetCarPowerLevelStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetCarPowerLevelStatusIntentResponseCode Code { get; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("carIdentifier")]
		string CarIdentifier { get; set; }

#if false // I wish BindAs was a thing right now
		[BindAs (typeof (float?))]
#endif
		[Internal]
		[NullAllowed, Export ("fuelPercentRemaining", ArgumentSemantic.Copy)]
		NSNumber _FuelPercentRemaining { get; set; }

#if false // I wish BindAs was a thing right now
		[BindAs (typeof (float?))]
#endif
		[Internal]
		[NullAllowed, Export ("chargePercentRemaining", ArgumentSemantic.Copy)]
		NSNumber _ChargePercentRemaining { get; set; }

		[NullAllowed, Export ("distanceRemaining", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemaining { get; set; }

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("charging", ArgumentSemantic.Copy)]
		NSNumber Charging { get; set; }

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[BindAs (typeof (double?))]
		[NullAllowed, Export ("minutesToFull", ArgumentSemantic.Copy)]
		NSNumber MinutesToFull { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumDistance", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> MaximumDistance { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("distanceRemainingElectric", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemainingElectric { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumDistanceElectric", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> MaximumDistanceElectric { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("distanceRemainingFuel", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemainingFuel { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumDistanceFuel", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> MaximumDistanceFuel { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("consumptionFormulaArguments", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ConsumptionFormulaArguments { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("chargingFormulaArguments", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ChargingFormulaArguments { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("dateOfLastStateUpdate", ArgumentSemantic.Copy)]
		NSDateComponents DateOfLastStateUpdate { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[BindAs (typeof (INCarChargingConnectorType))]
		[NullAllowed, Export ("activeConnector")]
		NSString ActiveConnector { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumBatteryCapacity", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitEnergy> MaximumBatteryCapacity { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("currentBatteryCapacity", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitEnergy> CurrentBatteryCapacity { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("minimumBatteryCapacity", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitEnergy> MinimumBatteryCapacity { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INPayBillIntent {

		[DesignatedInitializer]
		[Export ("initWithBillPayee:fromAccount:transactionAmount:transactionScheduledDate:transactionNote:billType:dueDate:")]
		NativeHandle Constructor ([NullAllowed] INBillPayee billPayee, [NullAllowed] INPaymentAccount fromAccount, [NullAllowed] INPaymentAmount transactionAmount, [NullAllowed] INDateComponentsRange transactionScheduledDate, [NullAllowed] string transactionNote, INBillType billType, [NullAllowed] INDateComponentsRange dueDate);

		[Export ("billPayee", ArgumentSemantic.Copy), NullAllowed]
		INBillPayee BillPayee { get; }

		[Export ("fromAccount", ArgumentSemantic.Copy), NullAllowed]
		INPaymentAccount FromAccount { get; }

		[Export ("transactionAmount", ArgumentSemantic.Copy), NullAllowed]
		INPaymentAmount TransactionAmount { get; }

		[Export ("transactionScheduledDate", ArgumentSemantic.Copy), NullAllowed]
		INDateComponentsRange TransactionScheduledDate { get; }

		[Export ("transactionNote"), NullAllowed]
		string TransactionNote { get; }

		[Export ("billType", ArgumentSemantic.Assign)]
		INBillType BillType { get; }

		[Export ("dueDate", ArgumentSemantic.Copy), NullAllowed]
		INDateComponentsRange DueDate { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INPayBillIntentHandling {

#if NET
		[Abstract]
#endif
		[Export ("handlePayBill:completion:")]
		void HandlePayBill (INPayBillIntent intent, Action<INPayBillIntentResponse> completion);

		[Export ("confirmPayBill:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmPayBill
#endif
		(INPayBillIntent intent, Action<INPayBillIntentResponse> completion);

		[Export ("resolveBillPayeeForPayBill:withCompletion:")]
		void ResolveBillPayee (INPayBillIntent intent, Action<INBillPayeeResolutionResult> completion);

		[Export ("resolveFromAccountForPayBill:withCompletion:")]
		void ResolveFromAccount (INPayBillIntent intent, Action<INPaymentAccountResolutionResult> completion);

		[Export ("resolveTransactionAmountForPayBill:withCompletion:")]
		void ResolveTransactionAmount (INPayBillIntent intent, Action<INPaymentAmountResolutionResult> completion);

		[Export ("resolveTransactionScheduledDateForPayBill:withCompletion:")]
		void ResolveTransactionScheduledDate (INPayBillIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveTransactionNoteForPayBill:withCompletion:")]
		void ResolveTransactionNote (INPayBillIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolveBillTypeForPayBill:withCompletion:")]
		void ResolveBillType (INPayBillIntent intent, Action<INBillTypeResolutionResult> completion);

		[Export ("resolveDueDateForPayBill:withCompletion:")]
		void ResolveDueDate (INPayBillIntent intent, Action<INDateComponentsRangeResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INPayBillIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INPayBillIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INPayBillIntentResponseCode Code { get; }

		[NullAllowed, Export ("fromAccount", ArgumentSemantic.Copy)]
		INPaymentAccount FromAccount { get; set; }

		[NullAllowed, Export ("billDetails", ArgumentSemantic.Copy)]
		INBillDetails BillDetails { get; set; }

		[NullAllowed, Export ("transactionAmount", ArgumentSemantic.Copy)]
		INPaymentAmount TransactionAmount { get; set; }

		[NullAllowed, Export ("transactionScheduledDate", ArgumentSemantic.Copy)]
		INDateComponentsRange TransactionScheduledDate { get; set; }

		[NullAllowed, Export ("transactionNote", ArgumentSemantic.Copy)]
		string TransactionNote { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INPaymentAccount : NSCopying, NSSecureCoding {

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Please use '.ctor (INSpeakableString, string, INAccountType, INSpeakableString, INBalanceAmount, INBalanceAmount)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Please use '.ctor (INSpeakableString, string, INAccountType, INSpeakableString, INBalanceAmount, INBalanceAmount)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use '.ctor (INSpeakableString, string, INAccountType, INSpeakableString, INBalanceAmount, INBalanceAmount)' instead.")]
		[Export ("initWithNickname:number:accountType:organizationName:")]
		NativeHandle Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, INAccountType accountType, [NullAllowed] INSpeakableString organizationName);

		[MacCatalyst (13, 1)]
		[Export ("initWithNickname:number:accountType:organizationName:balance:secondaryBalance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, INAccountType accountType, [NullAllowed] INSpeakableString organizationName, [NullAllowed] INBalanceAmount balance, [NullAllowed] INBalanceAmount secondaryBalance);

		[Export ("nickname", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString Nickname { get; }

		[Export ("accountNumber"), NullAllowed]
		string AccountNumber { get; }

		[Export ("accountType", ArgumentSemantic.Assign)]
		INAccountType AccountType { get; }

		[Export ("organizationName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString OrganizationName { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("balance", ArgumentSemantic.Copy)]
		INBalanceAmount Balance { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("secondaryBalance", ArgumentSemantic.Copy)]
		INBalanceAmount SecondaryBalance { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPaymentAccountResolutionResult {

		[Static]
		[Export ("successWithResolvedPaymentAccount:")]
		INPaymentAccountResolutionResult GetSuccess (INPaymentAccount resolvedPaymentAccount);

		[Static]
		[Export ("disambiguationWithPaymentAccountsToDisambiguate:")]
		INPaymentAccountResolutionResult GetDisambiguation (INPaymentAccount [] paymentAccountsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithPaymentAccountToConfirm:")]
		INPaymentAccountResolutionResult GetConfirmationRequired ([NullAllowed] INPaymentAccount paymentAccountToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPaymentAccountResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPaymentAccountResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPaymentAccountResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentAccountResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentAccountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INPaymentAmount : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithAmountType:amount:")]
		NativeHandle Constructor (INAmountType amountType, INCurrencyAmount amount);

		[Export ("amount", ArgumentSemantic.Copy), NullAllowed]
		INCurrencyAmount Amount { get; }

		[Export ("amountType", ArgumentSemantic.Assign)]
		INAmountType AmountType { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPaymentAmountResolutionResult {

		[Static]
		[Export ("successWithResolvedPaymentAmount:")]
		INPaymentAmountResolutionResult GetSuccess (INPaymentAmount resolvedPaymentAmount);

		[Static]
		[Export ("disambiguationWithPaymentAmountsToDisambiguate:")]
		INPaymentAmountResolutionResult GetDisambiguation (INPaymentAmount [] paymentAmountsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithPaymentAmountToConfirm:")]
		INPaymentAmountResolutionResult GetConfirmationRequired ([NullAllowed] INPaymentAmount paymentAmountToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPaymentAmountResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPaymentAmountResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPaymentAmountResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPaymentStatusResolutionResult {

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("successWithResolvedPaymentStatus:")]
		INPaymentStatusResolutionResult SuccessWithResolvedPaymentStatus (INPaymentStatus resolvedPaymentStatus);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INPaymentStatusResolutionResult SuccessWithResolvedValue (INPaymentStatus resolvedValue);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithPaymentStatusToConfirm:")]
		INPaymentStatusResolutionResult ConfirmationRequiredWithPaymentStatusToConfirm (INPaymentStatus paymentStatusToConfirm);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INPaymentStatusResolutionResult ConfirmationRequiredWithValueToConfirm (INPaymentStatus valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPaymentStatusResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPaymentStatusResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPaymentStatusResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentStatusResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentStatusResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INSearchForBillsIntent {

		[DesignatedInitializer]
		[Export ("initWithBillPayee:paymentDateRange:billType:status:dueDateRange:")]
		NativeHandle Constructor ([NullAllowed] INBillPayee billPayee, [NullAllowed] INDateComponentsRange paymentDateRange, INBillType billType, INPaymentStatus status, [NullAllowed] INDateComponentsRange dueDateRange);

		[Export ("billPayee", ArgumentSemantic.Copy), NullAllowed]
		INBillPayee BillPayee { get; }

		[Export ("paymentDateRange", ArgumentSemantic.Copy), NullAllowed]
		INDateComponentsRange PaymentDateRange { get; }

		[Export ("billType", ArgumentSemantic.Assign)]
		INBillType BillType { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INPaymentStatus Status { get; }

		[Export ("dueDateRange", ArgumentSemantic.Copy), NullAllowed]
		INDateComponentsRange DueDateRange { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSearchForBillsIntentHandling {

#if NET
		[Abstract]
#endif
		[Export ("handleSearchForBills:completion:")]
		void HandleSearch (INSearchForBillsIntent intent, Action<INSearchForBillsIntentResponse> completion);

		[Export ("confirmSearchForBills:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSearch
#endif
		(INSearchForBillsIntent intent, Action<INSearchForBillsIntentResponse> completion);

		[Export ("resolveBillPayeeForSearchForBills:withCompletion:")]
		void ResolveBillPayee (INSearchForBillsIntent intent, Action<INBillPayeeResolutionResult> completion);

		[Export ("resolvePaymentDateRangeForSearchForBills:withCompletion:")]
		void ResolvePaymentDateRange (INSearchForBillsIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveBillTypeForSearchForBills:withCompletion:")]
		void ResolveBillType (INSearchForBillsIntent intent, Action<INBillTypeResolutionResult> completion);

		[Export ("resolveStatusForSearchForBills:withCompletion:")]
		void ResolveStatus (INSearchForBillsIntent intent, Action<INPaymentStatusResolutionResult> completion);

		[Export ("resolveDueDateRangeForSearchForBills:withCompletion:")]
		void ResolveDueDateRange (INSearchForBillsIntent intent, Action<INDateComponentsRangeResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INSearchForBillsIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INSearchForBillsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForBillsIntentResponseCode Code { get; }

		[NullAllowed, Export ("bills", ArgumentSemantic.Copy)]
		INBillDetails [] Bills { get; set; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INSetCarLockStatusIntent {

		[Protected]
		[DesignatedInitializer]
		[Export ("initWithLocked:carName:")]
		NativeHandle Constructor ([NullAllowed] NSNumber locked, [NullAllowed] INSpeakableString carName);

#if false // I wish BindAs was a thing right now
		[BindAs (typeof (bool?))]
#endif
		[Internal]
		[Export ("locked", ArgumentSemantic.Copy), NullAllowed]
		NSNumber _Locked { get; }

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSetCarLockStatusIntentHandling {

		[Abstract]
		[Export ("handleSetCarLockStatus:completion:")]
		void HandleSetCarLockStatus (INSetCarLockStatusIntent intent, Action<INSetCarLockStatusIntentResponse> completion);

		[Export ("confirmSetCarLockStatus:completion:")]
		void
#if NET // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetCarLockStatus
#endif
		(INSetCarLockStatusIntent intent, Action<INSetCarLockStatusIntentResponse> completion);

		[Export ("resolveLockedForSetCarLockStatus:withCompletion:")]
		void ResolveLocked (INSetCarLockStatusIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveCarNameForSetCarLockStatus:withCompletion:")]
		void ResolveCarName (INSetCarLockStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INSetCarLockStatusIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INSetCarLockStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetCarLockStatusIntentResponseCode Code { get; }
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INActivateCarSignalIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		NativeHandle Constructor (INActivateCarSignalIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INActivateCarSignalIntentResponseCode Code { get; }

		[Export ("signals")]
		INCarSignalOptions Signals { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INAccountTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedAccountType:")]
		INAccountTypeResolutionResult GetSuccess (INAccountType resolvedAccountType);

		[Static]
		[Export ("confirmationRequiredWithAccountTypeToConfirm:")]
		INAccountTypeResolutionResult GetConfirmationRequired (INAccountType accountTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INAccountTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INAccountTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INAccountTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INAccountTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INAccountTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INAddTasksIntent {

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithTargetTaskList:taskTitles:spatialEventTrigger:temporalEventTrigger:priority:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INTaskList targetTaskList, [NullAllowed] INSpeakableString [] taskTitles, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger, INTaskPriority priority);

		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the constructor with 'INTaskPriority priority' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the constructor with 'INTaskPriority priority' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the constructor with 'INTaskPriority priority' instead.")]
		[Export ("initWithTargetTaskList:taskTitles:spatialEventTrigger:temporalEventTrigger:")]
		NativeHandle Constructor ([NullAllowed] INTaskList targetTaskList, [NullAllowed] INSpeakableString [] taskTitles, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[NullAllowed, Export ("targetTaskList", ArgumentSemantic.Copy)]
		INTaskList TargetTaskList { get; }

		[NullAllowed, Export ("taskTitles", ArgumentSemantic.Copy)]
		INSpeakableString [] TaskTitles { get; }

		[NullAllowed, Export ("spatialEventTrigger", ArgumentSemantic.Copy)]
		INSpatialEventTrigger SpatialEventTrigger { get; }

		[NullAllowed, Export ("temporalEventTrigger", ArgumentSemantic.Copy)]
		INTemporalEventTrigger TemporalEventTrigger { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("priority", ArgumentSemantic.Assign)]
		INTaskPriority Priority { get; }
	}

	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INAddTasksIntentHandling {

		[Abstract]
		[Export ("handleAddTasks:completion:")]
		void HandleAddTasks (INAddTasksIntent intent, Action<INAddTasksIntentResponse> completion);

		[Export ("confirmAddTasks:completion:")]
		void Confirm (INAddTasksIntent intent, Action<INAddTasksIntentResponse> completion);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ResolveTargetTaskList (Action<INAddTasksTargetTaskListResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'ResolveTargetTaskList (Action<INAddTasksTargetTaskListResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveTargetTaskList (Action<INAddTasksTargetTaskListResolutionResult>)' overload instead.")]
		[Export ("resolveTargetTaskListForAddTasks:withCompletion:")]
		void ResolveTargetTaskList (INAddTasksIntent intent, Action<INTaskListResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTargetTaskListForAddTasks:completion:")]
		void ResolveTargetTaskList (INAddTasksIntent intent, Action<INAddTasksTargetTaskListResolutionResult> completionHandler);

		[Export ("resolveTaskTitlesForAddTasks:withCompletion:")]
		void ResolveTaskTitles (INAddTasksIntent intent, Action<INSpeakableStringResolutionResult []> completion);

		[Export ("resolveSpatialEventTriggerForAddTasks:withCompletion:")]
		void ResolveSpatialEventTrigger (INAddTasksIntent intent, Action<INSpatialEventTriggerResolutionResult> completion);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ResolveTemporalEventTrigger (Action<INAddTasksTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'ResolveTemporalEventTrigger (Action<INAddTasksTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveTemporalEventTrigger (Action<INAddTasksTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Export ("resolveTemporalEventTriggerForAddTasks:withCompletion:")]
		void ResolveTemporalEventTrigger (INAddTasksIntent intent, Action<INTemporalEventTriggerResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTemporalEventTriggerForAddTasks:completion:")]
		void ResolveTemporalEventTrigger (INAddTasksIntent intent, Action<INAddTasksTemporalEventTriggerResolutionResult> completionHandler);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePriorityForAddTasks:withCompletion:")]
		void ResolvePriority (INAddTasksIntent intent, Action<INTaskPriorityResolutionResult> completion);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INAddTasksIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INAddTasksIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INAddTasksIntentResponseCode Code { get; }

		[NullAllowed, Export ("modifiedTaskList", ArgumentSemantic.Copy)]
		INTaskList ModifiedTaskList { get; set; }

		[NullAllowed, Export ("addedTasks", ArgumentSemantic.Copy)]
		INTask [] AddedTasks { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INAppendToNoteIntent {

		[Export ("initWithTargetNote:content:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INNote targetNote, [NullAllowed] INNoteContent content);

		[NullAllowed, Export ("targetNote", ArgumentSemantic.Copy)]
		INNote TargetNote { get; }

		[NullAllowed, Export ("content", ArgumentSemantic.Copy)]
		INNoteContent Content { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INAppendToNoteIntentHandling {

		[Abstract]
		[Export ("handleAppendToNote:completion:")]
		void HandleAppendToNote (INAppendToNoteIntent intent, Action<INAppendToNoteIntentResponse> completion);

		[Export ("confirmAppendToNote:completion:")]
		void Confirm (INAppendToNoteIntent intent, Action<INAppendToNoteIntentResponse> completion);

		[Export ("resolveTargetNoteForAppendToNote:withCompletion:")]
		void ResolveTargetNoteForAppend (INAppendToNoteIntent intent, Action<INNoteResolutionResult> completion);

		[Export ("resolveContentForAppendToNote:withCompletion:")]
		void ResolveContentForAppend (INAppendToNoteIntent intent, Action<INNoteContentResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INAppendToNoteIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INAppendToNoteIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INAppendToNoteIntentResponseCode Code { get; }

		[NullAllowed, Export ("note", ArgumentSemantic.Copy)]
		INNote Note { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INBalanceAmount : NSCopying, NSSecureCoding {

		[Export ("initWithAmount:balanceType:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDecimalNumber amount, INBalanceType balanceType);

		[Export ("initWithAmount:currencyCode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDecimalNumber amount, string currencyCode);

		[NullAllowed, Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; }

		[Export ("balanceType", ArgumentSemantic.Assign)]
		INBalanceType BalanceType { get; }

		[NullAllowed, Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INBalanceTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedBalanceType:")]
		INBalanceTypeResolutionResult GetSuccess (INBalanceType resolvedBalanceType);

		[Static]
		[Export ("confirmationRequiredWithBalanceTypeToConfirm:")]
		INBalanceTypeResolutionResult GetConfirmationRequired (INBalanceType balanceTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INBalanceTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INBalanceTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INBalanceTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBalanceTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBalanceTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallDestinationTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedCallDestinationType:")]
		INCallDestinationTypeResolutionResult GetSuccess (INCallDestinationType resolvedCallDestinationType);

		[Static]
		[Export ("confirmationRequiredWithCallDestinationTypeToConfirm:")]
		INCallDestinationTypeResolutionResult GetConfirmationRequired (INCallDestinationType callDestinationTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCallDestinationTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCallDestinationTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCallDestinationTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallDestinationTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallDestinationTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCallRecord : NSCopying, NSSecureCoding {

		[Watch (7, 4), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("initWithIdentifier:dateCreated:callRecordType:callCapability:callDuration:unseen:participants:numberOfCalls:isCallerIdBlocked:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen, [NullAllowed] INPerson [] participants, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfCalls, [NullAllowed][BindAs (typeof (bool?))] NSNumber isCallerIdBlocked);

		[Deprecated (PlatformName.iOS, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?, int?)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?, int?)' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 4, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?, int?)' instead.")]
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:dateCreated:caller:callRecordType:callCapability:callDuration:unseen:numberOfCalls:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, [NullAllowed] INPerson caller, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfCalls);

		[Watch (7, 4), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("initWithIdentifier:dateCreated:callRecordType:callCapability:callDuration:unseen:numberOfCalls:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfCalls);

		[Deprecated (PlatformName.iOS, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 4, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 3, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Export ("initWithIdentifier:dateCreated:caller:callRecordType:callCapability:callDuration:unseen:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, [NullAllowed] INPerson caller, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed] NSNumber callDuration, [NullAllowed] NSNumber unseen);

		[Watch (7, 4), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("initWithIdentifier:dateCreated:callRecordType:callCapability:callDuration:unseen:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen);

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		NSDate DateCreated { get; }

		[Deprecated (PlatformName.iOS, 14, 5)]
		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.WatchOS, 7, 4)]
		[Deprecated (PlatformName.MacCatalyst, 14, 5)]
		[NullAllowed, Export ("caller", ArgumentSemantic.Copy)]
		INPerson Caller { get; }

		[Export ("callRecordType")]
		INCallRecordType CallRecordType { get; }

		[Export ("callCapability")]
		INCallCapability CallCapability { get; }

		[Protected]
		[NullAllowed, Export ("callDuration", ArgumentSemantic.Copy)]
		NSNumber WeakCallDuration { get; }

		[Protected]
		[NullAllowed, Export ("unseen", ArgumentSemantic.Copy)]
		NSNumber WeakUnseen { get; }

		[BindAs (typeof (int?))]
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("numberOfCalls", ArgumentSemantic.Copy)]
		NSNumber NumberOfCalls { get; }

		[BindAs (typeof (bool?))]
		[Watch (7, 4), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("isCallerIdBlocked", ArgumentSemantic.Copy)]
		NSNumber IsCallerIdBlocked { get; }

		[NullAllowed]
		[Watch (7, 4), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("participants", ArgumentSemantic.Copy)]
		INPerson [] Participants { get; }
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallRecordTypeOptionsResolutionResult {

		[Static]
		[Export ("successWithResolvedCallRecordTypeOptions:")]
		INCallRecordTypeOptionsResolutionResult GetSuccess (INCallRecordTypeOptions resolvedCallRecordTypeOptions);

		[Static]
		[Export ("confirmationRequiredWithCallRecordTypeOptionsToConfirm:")]
		INCallRecordTypeOptionsResolutionResult GetConfirmationRequired (INCallRecordTypeOptions callRecordTypeOptionsToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCallRecordTypeOptionsResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCallRecordTypeOptionsResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCallRecordTypeOptionsResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallRecordTypeOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallRecordTypeOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoWatch, NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INCancelRideIntent {

		[Export ("initWithRideIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string rideIdentifier);

		[Export ("rideIdentifier")]
		string RideIdentifier { get; }
	}

	[NoWatch, NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INCancelRideIntentHandling {

		[Abstract]
		[Export ("handleCancelRide:completion:")]
		void HandleCancelRide (INCancelRideIntent intent, Action<INCancelRideIntentResponse> completion);

		[Export ("confirmCancelRide:completion:")]
		void Confirm (INCancelRideIntent intent, Action<INCancelRideIntentResponse> completion);
	}

	[NoWatch, NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCancelRideIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INCancelRideIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCancelRideIntentResponseCode Code { get; }

		[NullAllowed, Export ("cancellationFee", ArgumentSemantic.Copy)]
		INCurrencyAmount CancellationFee { get; set; }

		[NullAllowed, Export ("cancellationFeeThreshold", ArgumentSemantic.Copy)]
		NSDateComponents CancellationFeeThreshold { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INCreateNoteIntent {

		[Export ("initWithTitle:content:groupName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] INNoteContent content, [NullAllowed] INSpeakableString groupName);

		[NullAllowed, Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[NullAllowed, Export ("content", ArgumentSemantic.Copy)]
		INNoteContent Content { get; }

		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INCreateNoteIntentHandling {

		[Abstract]
		[Export ("handleCreateNote:completion:")]
		void HandleCreateNote (INCreateNoteIntent intent, Action<INCreateNoteIntentResponse> completion);

		[Export ("confirmCreateNote:completion:")]
		void Confirm (INCreateNoteIntent intent, Action<INCreateNoteIntentResponse> completion);

		[Export ("resolveTitleForCreateNote:withCompletion:")]
		void ResolveTitle (INCreateNoteIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveContentForCreateNote:withCompletion:")]
		void ResolveContent (INCreateNoteIntent intent, Action<INNoteContentResolutionResult> completion);

		[Export ("resolveGroupNameForCreateNote:withCompletion:")]
		void ResolveGroupName (INCreateNoteIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCreateNoteIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INCreateNoteIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCreateNoteIntentResponseCode Code { get; }

		[NullAllowed, Export ("createdNote", ArgumentSemantic.Copy)]
		INNote CreatedNote { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INCreateTaskListIntent {

		[Export ("initWithTitle:taskTitles:groupName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] INSpeakableString [] taskTitles, [NullAllowed] INSpeakableString groupName);

		[NullAllowed, Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[NullAllowed, Export ("taskTitles", ArgumentSemantic.Copy)]
		INSpeakableString [] TaskTitles { get; }

		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INCreateTaskListIntentHandling {

		[Abstract]
		[Export ("handleCreateTaskList:completion:")]
		void HandleCreateTaskList (INCreateTaskListIntent intent, Action<INCreateTaskListIntentResponse> completion);

		[Export ("confirmCreateTaskList:completion:")]
		void Confirm (INCreateTaskListIntent intent, Action<INCreateTaskListIntentResponse> completion);

		[Export ("resolveTitleForCreateTaskList:withCompletion:")]
		void ResolveTitle (INCreateTaskListIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveTaskTitlesForCreateTaskList:withCompletion:")]
		void ResolveTaskTitles (INCreateTaskListIntent intent, Action<INSpeakableStringResolutionResult []> completion);

		[Export ("resolveGroupNameForCreateTaskList:withCompletion:")]
		void ResolveGroupName (INCreateTaskListIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCreateTaskListIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INCreateTaskListIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCreateTaskListIntentResponseCode Code { get; }

		[NullAllowed, Export ("createdTaskList", ArgumentSemantic.Copy)]
		INTaskList CreatedTaskList { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INDateSearchTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedDateSearchType:")]
		INDateSearchTypeResolutionResult GetSuccess (INDateSearchType resolvedDateSearchType);

		[Static]
		[Export ("confirmationRequiredWithDateSearchTypeToConfirm:")]
		INDateSearchTypeResolutionResult GetConfirmationRequired (INDateSearchType dateSearchTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDateSearchTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDateSearchTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDateSearchTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDateSearchTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDateSearchTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INGetVisualCodeIntent {

		[Export ("initWithVisualCodeType:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INVisualCodeType visualCodeType);

		[Export ("visualCodeType", ArgumentSemantic.Assign)]
		INVisualCodeType VisualCodeType { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INGetVisualCodeIntentHandling {

		[Abstract]
		[Export ("handleGetVisualCode:completion:")]
		void HandleGetVisualCode (INGetVisualCodeIntent intent, Action<INGetVisualCodeIntentResponse> completion);

		[Export ("confirmGetVisualCode:completion:")]
		void Confirm (INGetVisualCodeIntent intent, Action<INGetVisualCodeIntentResponse> completion);

		[Export ("resolveVisualCodeTypeForGetVisualCode:withCompletion:")]
		void ResolveVisualCodeType (INGetVisualCodeIntent intent, Action<INVisualCodeTypeResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INGetVisualCodeIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INGetVisualCodeIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetVisualCodeIntentResponseCode Code { get; }

		[NullAllowed, Export ("visualCodeImage", ArgumentSemantic.Copy)]
		INImage VisualCodeImage { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INNoteContent))]
	interface INImageNoteContent : NSSecureCoding, NSCopying {

		[Export ("initWithImage:")]
		NativeHandle Constructor (INImage image);

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		INImage Image { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INLocationSearchTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedLocationSearchType:")]
		INLocationSearchTypeResolutionResult GetSuccess (INLocationSearchType resolvedLocationSearchType);

		[Static]
		[Export ("confirmationRequiredWithLocationSearchTypeToConfirm:")]
		INLocationSearchTypeResolutionResult GetConfirmationRequired (INLocationSearchType locationSearchTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INLocationSearchTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INLocationSearchTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INLocationSearchTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INLocationSearchTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INLocationSearchTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INNote : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:contents:groupName:createdDateComponents:modifiedDateComponents:identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString title, INNoteContent [] contents, [NullAllowed] INSpeakableString groupName, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier);

		[Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[Export ("contents", ArgumentSemantic.Copy)]
		INNoteContent [] Contents { get; }

		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }

		[NullAllowed, Export ("createdDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents CreatedDateComponents { get; }

		[NullAllowed, Export ("modifiedDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents ModifiedDateComponents { get; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INNoteContent : NSSecureCoding, NSCopying {
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INNoteContentResolutionResult {

		[Static]
		[Export ("successWithResolvedNoteContent:")]
		INNoteContentResolutionResult GetSuccess (INNoteContent resolvedNoteContent);

		[Static]
		[Export ("disambiguationWithNoteContentsToDisambiguate:")]
		INNoteContentResolutionResult GetDisambiguation (INNoteContent [] noteContentsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithNoteContentToConfirm:")]
		INNoteContentResolutionResult GetConfirmationRequired ([NullAllowed] INNoteContent noteContentToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INNoteContentResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INNoteContentResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INNoteContentResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNoteContentResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNoteContentResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Not used anymore.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Not used anymore.")]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Not used anymore.")]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INNoteContentTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedNoteContentType:")]
		INNoteContentTypeResolutionResult GetSuccess (INNoteContentType resolvedNoteContentType);

		[Static]
		[Export ("confirmationRequiredWithNoteContentTypeToConfirm:")]
		INNoteContentTypeResolutionResult GetConfirmationRequired (INNoteContentType noteContentTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INNoteContentTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INNoteContentTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INNoteContentTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNoteContentTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNoteContentTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INNoteResolutionResult {

		[Static]
		[Export ("successWithResolvedNote:")]
		INNoteResolutionResult GetSuccess (INNote resolvedNote);

		[Static]
		[Export ("disambiguationWithNotesToDisambiguate:")]
		INNoteResolutionResult GetDisambiguation (INNote [] notesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithNoteToConfirm:")]
		INNoteResolutionResult GetConfirmationRequired ([NullAllowed] INNote noteToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INNoteResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INNoteResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INNoteResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNoteResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNoteResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INNotebookItemTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedNotebookItemType:")]
		INNotebookItemTypeResolutionResult GetSuccess (INNotebookItemType resolvedNotebookItemType);

		[Static]
		[Export ("disambiguationWithNotebookItemTypesToDisambiguate:")]
		INNotebookItemTypeResolutionResult GetDisambiguation (NSNumber [] notebookItemTypesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithNotebookItemTypeToConfirm:")]
		INNotebookItemTypeResolutionResult GetConfirmationRequired (INNotebookItemType notebookItemTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INNotebookItemTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INNotebookItemTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INNotebookItemTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNotebookItemTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNotebookItemTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INParameter : NSCopying, NSSecureCoding {

		[Static]
		[Export ("parameterForClass:keyPath:")]
		INParameter GetParameter (Class aClass, string keyPath);

		[Static]
		[Wrap ("GetParameter (new Class (type), keyPath)")]
		INParameter GetParameter (Type type, string keyPath);

		[Export ("parameterClass")]
		Class ParameterClass { get; }

		[Wrap ("Class.Lookup (ParameterClass)")]
		Type ParameterType { get; }

		[Export ("parameterKeyPath")]
		string ParameterKeyPath { get; }

		[Export ("isEqualToParameter:")]
		bool IsEqualTo (INParameter parameter);

		[Export ("setIndex:forSubKeyPath:")]
		void SetIndex (nuint index, string subKeyPath);

		[Export ("indexForSubKeyPath:")]
		nuint GetIndex (string subKeyPath);
	}

#if NET
	[NoMac]
#elif MONOMAC
	[Obsoleted (PlatformName.MacOSX, 10,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRecurrenceRule : NSCopying, NSSecureCoding {

		[Export ("initWithInterval:frequency:")]
		NativeHandle Constructor (nuint interval, INRecurrenceFrequency frequency);

		[Watch (7, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithInterval:frequency:weeklyRecurrenceDays:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nuint interval, INRecurrenceFrequency frequency, INDayOfWeekOptions weeklyRecurrenceDays);

		[Export ("interval")]
		nuint Interval { get; }

		[Export ("frequency")]
		INRecurrenceFrequency Frequency { get; }

		[Watch (7, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("weeklyRecurrenceDays")]
		INDayOfWeekOptions WeeklyRecurrenceDays { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INCurrencyAmountResolutionResult))]
	[DisableDefaultCtor]
	interface INRequestPaymentCurrencyAmountResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INRequestPaymentCurrencyAmountResolutionResult GetUnsupported (INRequestPaymentCurrencyAmountUnsupportedReason reason);

		[Export ("initWithCurrencyAmountResolutionResult:")]
		NativeHandle Constructor (INCurrencyAmountResolutionResult currencyAmountResolutionResult);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRequestPaymentCurrencyAmountResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRequestPaymentCurrencyAmountResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRequestPaymentCurrencyAmountResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRequestPaymentCurrencyAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRequestPaymentCurrencyAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INRequestPaymentPayerResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INRequestPaymentPayerResolutionResult GetUnsupported (INRequestPaymentPayerUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		NativeHandle Constructor (INPersonResolutionResult personResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedPerson:")]
		INRequestPaymentPayerResolutionResult GetSuccess (INPerson resolvedPerson);

		[New]
		[Static]
		[Export ("disambiguationWithPeopleToDisambiguate:")]
		INRequestPaymentPayerResolutionResult GetDisambiguation (INPerson [] peopleToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithPersonToConfirm:")]
		INRequestPaymentPayerResolutionResult GetConfirmationRequired ([NullAllowed] INPerson personToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INRequestPaymentPayerResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INRequestPaymentPayerResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INRequestPaymentPayerResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRequestPaymentPayerResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRequestPaymentPayerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSearchForAccountsIntent {

		[Export ("initWithAccountNickname:accountType:organizationName:requestedBalanceType:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString accountNickname, INAccountType accountType, [NullAllowed] INSpeakableString organizationName, INBalanceType requestedBalanceType);

		[NullAllowed, Export ("accountNickname", ArgumentSemantic.Copy)]
		INSpeakableString AccountNickname { get; }

		[Export ("accountType", ArgumentSemantic.Assign)]
		INAccountType AccountType { get; }

		[NullAllowed, Export ("organizationName", ArgumentSemantic.Copy)]
		INSpeakableString OrganizationName { get; }

		[Export ("requestedBalanceType", ArgumentSemantic.Assign)]
		INBalanceType RequestedBalanceType { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSearchForAccountsIntentHandling {

		[Abstract]
		[Export ("handleSearchForAccounts:completion:")]
		void HandleSearchForAccounts (INSearchForAccountsIntent intent, Action<INSearchForAccountsIntentResponse> completion);

		[Export ("confirmSearchForAccounts:completion:")]
		void Confirm (INSearchForAccountsIntent intent, Action<INSearchForAccountsIntentResponse> completion);

		[Export ("resolveAccountNicknameForSearchForAccounts:withCompletion:")]
		void ResolveAccountNickname (INSearchForAccountsIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveAccountTypeForSearchForAccounts:withCompletion:")]
		void ResolveAccountType (INSearchForAccountsIntent intent, Action<INAccountTypeResolutionResult> completion);

		[Export ("resolveOrganizationNameForSearchForAccounts:withCompletion:")]
		void ResolveOrganizationName (INSearchForAccountsIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveRequestedBalanceTypeForSearchForAccounts:withCompletion:")]
		void ResolveRequestedBalanceType (INSearchForAccountsIntent intent, Action<INBalanceTypeResolutionResult> completion);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForAccountsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSearchForAccountsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForAccountsIntentResponseCode Code { get; }

		[NullAllowed, Export ("accounts", ArgumentSemantic.Copy)]
		INPaymentAccount [] Accounts { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSearchForNotebookItemsIntent {

		[Deprecated (PlatformName.WatchOS, 4, 2, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Deprecated (PlatformName.iOS, 11, 2, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType);

		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the constructor with 'INTemporalEventTriggerTypeOptions temporalEventTriggerTypes' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the constructor with 'INTemporalEventTriggerTypeOptions temporalEventTriggerTypes' instead.")]
		[Watch (4, 2)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the constructor with 'INTemporalEventTriggerTypeOptions temporalEventTriggerTypes' instead.")]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:notebookItemIdentifier:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType, [NullAllowed] string notebookItemIdentifier);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:temporalEventTriggerTypes:taskPriority:notebookItemIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType, INTemporalEventTriggerTypeOptions temporalEventTriggerTypes, INTaskPriority taskPriority, [NullAllowed] string notebookItemIdentifier);

		[NullAllowed, Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[NullAllowed, Export ("content")]
		string Content { get; }

		[Export ("itemType", ArgumentSemantic.Assign)]
		INNotebookItemType ItemType { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INTaskStatus Status { get; }

		[NullAllowed, Export ("location", ArgumentSemantic.Copy)]
		CLPlacemark Location { get; }

		[Export ("locationSearchType", ArgumentSemantic.Assign)]
		INLocationSearchType LocationSearchType { get; }

		[NullAllowed, Export ("dateTime", ArgumentSemantic.Copy)]
		INDateComponentsRange DateTime { get; }

		[Export ("dateSearchType", ArgumentSemantic.Assign)]
		INDateSearchType DateSearchType { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("temporalEventTriggerTypes", ArgumentSemantic.Assign)]
		INTemporalEventTriggerTypeOptions TemporalEventTriggerTypes { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("taskPriority", ArgumentSemantic.Assign)]
		INTaskPriority TaskPriority { get; }

		[Watch (4, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("notebookItemIdentifier")]
		string NotebookItemIdentifier { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSearchForNotebookItemsIntentHandling {

		[Abstract]
		[Export ("handleSearchForNotebookItems:completion:")]
		void HandleSearchForNotebookItems (INSearchForNotebookItemsIntent intent, Action<INSearchForNotebookItemsIntentResponse> completion);

		[Export ("confirmSearchForNotebookItems:completion:")]
		void Confirm (INSearchForNotebookItemsIntent intent, Action<INSearchForNotebookItemsIntentResponse> completion);

		[Export ("resolveTitleForSearchForNotebookItems:withCompletion:")]
		void ResolveTitle (INSearchForNotebookItemsIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveContentForSearchForNotebookItems:withCompletion:")]
		void ResolveContent (INSearchForNotebookItemsIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolveItemTypeForSearchForNotebookItems:withCompletion:")]
		void ResolveItemType (INSearchForNotebookItemsIntent intent, Action<INNotebookItemTypeResolutionResult> completion);

		[Export ("resolveStatusForSearchForNotebookItems:withCompletion:")]
		void ResolveStatus (INSearchForNotebookItemsIntent intent, Action<INTaskStatusResolutionResult> completion);

		[Export ("resolveLocationForSearchForNotebookItems:withCompletion:")]
		void ResolveLocation (INSearchForNotebookItemsIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveLocationSearchTypeForSearchForNotebookItems:withCompletion:")]
		void ResolveLocationSearchType (INSearchForNotebookItemsIntent intent, Action<INLocationSearchTypeResolutionResult> completion);

		[Export ("resolveDateTimeForSearchForNotebookItems:withCompletion:")]
		void ResolveDateTime (INSearchForNotebookItemsIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveDateSearchTypeForSearchForNotebookItems:withCompletion:")]
		void ResolveDateSearchType (INSearchForNotebookItemsIntent intent, Action<INDateSearchTypeResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTemporalEventTriggerTypesForSearchForNotebookItems:withCompletion:")]
		void ResolveTemporalEventTriggerTypes (INSearchForNotebookItemsIntent intent, Action<INTemporalEventTriggerTypeOptionsResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTaskPriorityForSearchForNotebookItems:withCompletion:")]
		void ResolveTaskPriority (INSearchForNotebookItemsIntent intent, Action<INTaskPriorityResolutionResult> completion);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForNotebookItemsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSearchForNotebookItemsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForNotebookItemsIntentResponseCode Code { get; }

		[NullAllowed, Export ("notes", ArgumentSemantic.Copy)]
		INNote [] Notes { get; set; }

		[NullAllowed, Export ("taskLists", ArgumentSemantic.Copy)]
		INTaskList [] TaskLists { get; set; }

		[NullAllowed, Export ("tasks", ArgumentSemantic.Copy)]
		INTask [] Tasks { get; set; }

		[Export ("sortType", ArgumentSemantic.Assign)]
		INSortType SortType { get; set; }
	}

	[Mac (12, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INSendMessageRecipientResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSendMessageRecipientResolutionResult GetUnsupported (INSendMessageRecipientUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		NativeHandle Constructor (INPersonResolutionResult personResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedPerson:")]
		INSendMessageRecipientResolutionResult GetSuccess (INPerson resolvedPerson);

		[New]
		[Static]
		[Export ("disambiguationWithPeopleToDisambiguate:")]
		INSendMessageRecipientResolutionResult GetDisambiguation (INPerson [] peopleToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithPersonToConfirm:")]
		INSendMessageRecipientResolutionResult GetConfirmationRequired ([NullAllowed] INPerson personToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSendMessageRecipientResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSendMessageRecipientResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSendMessageRecipientResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSendMessageRecipientResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSendMessageRecipientResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INCurrencyAmountResolutionResult))]
	[DisableDefaultCtor]
	interface INSendPaymentCurrencyAmountResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSendPaymentCurrencyAmountResolutionResult GetUnsupported (INSendPaymentCurrencyAmountUnsupportedReason reason);

		[Export ("initWithCurrencyAmountResolutionResult:")]
		NativeHandle Constructor (INCurrencyAmountResolutionResult currencyAmountResolutionResult);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSendPaymentCurrencyAmountResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSendPaymentCurrencyAmountResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSendPaymentCurrencyAmountResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSendPaymentCurrencyAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSendPaymentCurrencyAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INSendPaymentPayeeResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSendPaymentPayeeResolutionResult GetUnsupported (INSendPaymentPayeeUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		NativeHandle Constructor (INPersonResolutionResult personResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedPerson:")]
		INSendPaymentPayeeResolutionResult GetSuccess (INPerson resolvedPerson);

		[New]
		[Static]
		[Export ("disambiguationWithPeopleToDisambiguate:")]
		INSendPaymentPayeeResolutionResult GetDisambiguation (INPerson [] peopleToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithPersonToConfirm:")]
		INSendPaymentPayeeResolutionResult GetConfirmationRequired ([NullAllowed] INPerson personToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSendPaymentPayeeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSendPaymentPayeeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSendPaymentPayeeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSendPaymentPayeeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSendPaymentPayeeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoWatch, NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INSendRideFeedbackIntent {

		[Export ("initWithRideIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string rideIdentifier);

		[Export ("rideIdentifier")]
		string RideIdentifier { get; }

		[NullAllowed, Export ("rating", ArgumentSemantic.Copy)]
		NSNumber Rating { get; set; }

		[NullAllowed, Export ("tip", ArgumentSemantic.Copy)]
		INCurrencyAmount Tip { get; set; }
	}

	[NoWatch, NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSendRideFeedbackIntentHandling {

		[Abstract]
		[Export ("handleSendRideFeedback:completion:")]
		void HandleSendRideFeedback (INSendRideFeedbackIntent sendRideFeedbackintent, Action<INSendRideFeedbackIntentResponse> completion);

		[Export ("confirmSendRideFeedback:completion:")]
		void Confirm (INSendRideFeedbackIntent sendRideFeedbackIntent, Action<INSendRideFeedbackIntentResponse> completion);
	}

	[NoWatch, NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSendRideFeedbackIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSendRideFeedbackIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSendRideFeedbackIntentResponseCode Code { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSetTaskAttributeIntent {

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithTargetTask:taskTitle:status:priority:spatialEventTrigger:temporalEventTrigger:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INTask targetTask, [NullAllowed] INSpeakableString taskTitle, INTaskStatus status, INTaskPriority priority, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the 'INTaskPriority priority' overload instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'INTaskPriority priority' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'INTaskPriority priority' overload instead.")]
		[Export ("initWithTargetTask:status:spatialEventTrigger:temporalEventTrigger:")]
		NativeHandle Constructor ([NullAllowed] INTask targetTask, INTaskStatus status, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[NullAllowed, Export ("targetTask", ArgumentSemantic.Copy)]
		INTask TargetTask { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("taskTitle", ArgumentSemantic.Copy)]
		INSpeakableString TaskTitle { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INTaskStatus Status { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("priority", ArgumentSemantic.Assign)]
		INTaskPriority Priority { get; }

		[NullAllowed, Export ("spatialEventTrigger", ArgumentSemantic.Copy)]
		INSpatialEventTrigger SpatialEventTrigger { get; }

		[NullAllowed, Export ("temporalEventTrigger", ArgumentSemantic.Copy)]
		INTemporalEventTrigger TemporalEventTrigger { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSetTaskAttributeIntentHandling {

		[Abstract]
		[Export ("handleSetTaskAttribute:completion:")]
		void HandleSetTaskAttribute (INSetTaskAttributeIntent intent, Action<INSetTaskAttributeIntentResponse> completion);

		[Export ("confirmSetTaskAttribute:completion:")]
		void Confirm (INSetTaskAttributeIntent intent, Action<INSetTaskAttributeIntentResponse> completion);

		[Export ("resolveTargetTaskForSetTaskAttribute:withCompletion:")]
		void ResolveTargetTask (INSetTaskAttributeIntent intent, Action<INTaskResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTaskTitleForSetTaskAttribute:withCompletion:")]
		void ResolveTaskTitle (INSetTaskAttributeIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveStatusForSetTaskAttribute:withCompletion:")]
		void ResolveStatus (INSetTaskAttributeIntent intent, Action<INTaskStatusResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePriorityForSetTaskAttribute:withCompletion:")]
		void ResolvePriority (INSetTaskAttributeIntent intent, Action<INTaskPriorityResolutionResult> completion);

		[Export ("resolveSpatialEventTriggerForSetTaskAttribute:withCompletion:")]
		void ResolveSpatialEventTrigger (INSetTaskAttributeIntent intent, Action<INSpatialEventTriggerResolutionResult> completion);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ResolveTemporalEventTrigger (INSetTaskAttributeIntent Action<INSetTaskAttributeTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'ResolveTemporalEventTrigger (INSetTaskAttributeIntent Action<INSetTaskAttributeTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveTemporalEventTrigger (INSetTaskAttributeIntent Action<INSetTaskAttributeTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Export ("resolveTemporalEventTriggerForSetTaskAttribute:withCompletion:")]
		void ResolveTemporalEventTrigger (INSetTaskAttributeIntent intent, Action<INTemporalEventTriggerResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTemporalEventTriggerForSetTaskAttribute:completion:")]
		void ResolveTemporalEventTrigger (INSetTaskAttributeIntent intent, Action<INSetTaskAttributeTemporalEventTriggerResolutionResult> completionHandler);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetTaskAttributeIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSetTaskAttributeIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetTaskAttributeIntentResponseCode Code { get; }

		[NullAllowed, Export ("modifiedTask", ArgumentSemantic.Copy)]
		INTask ModifiedTask { get; set; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpatialEventTrigger : NSCopying, NSSecureCoding {

		[Export ("initWithPlacemark:event:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CLPlacemark placemark, INSpatialEvent @event);

		[Export ("placemark", ArgumentSemantic.Copy)]
		CLPlacemark Placemark { get; }

		[Export ("event", ArgumentSemantic.Assign)]
		INSpatialEvent Event { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INSpatialEventTriggerResolutionResult {

		[Static]
		[Export ("successWithResolvedSpatialEventTrigger:")]
		INSpatialEventTriggerResolutionResult GetSuccess (INSpatialEventTrigger resolvedSpatialEventTrigger);

		[Static]
		[Export ("disambiguationWithSpatialEventTriggersToDisambiguate:")]
		INSpatialEventTriggerResolutionResult GetDisambiguation (INSpatialEventTrigger [] spatialEventTriggersToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithSpatialEventTriggerToConfirm:")]
		INSpatialEventTriggerResolutionResult GetConfirmationRequired ([NullAllowed] INSpatialEventTrigger spatialEventTriggerToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSpatialEventTriggerResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSpatialEventTriggerResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSpatialEventTriggerResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSpatialEventTriggerResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSpatialEventTriggerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTask : NSCopying, NSSecureCoding {

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithTitle:status:taskType:spatialEventTrigger:temporalEventTrigger:createdDateComponents:modifiedDateComponents:identifier:priority:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString title, INTaskStatus status, INTaskType taskType, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier, INTaskPriority priority);

		[Export ("initWithTitle:status:taskType:spatialEventTrigger:temporalEventTrigger:createdDateComponents:modifiedDateComponents:identifier:")]
		NativeHandle Constructor (INSpeakableString title, INTaskStatus status, INTaskType taskType, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier);

		[Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INTaskStatus Status { get; }

		[Export ("taskType", ArgumentSemantic.Assign)]
		INTaskType TaskType { get; }

		[NullAllowed, Export ("spatialEventTrigger", ArgumentSemantic.Copy)]
		INSpatialEventTrigger SpatialEventTrigger { get; }

		[NullAllowed, Export ("temporalEventTrigger", ArgumentSemantic.Copy)]
		INTemporalEventTrigger TemporalEventTrigger { get; }

		[NullAllowed, Export ("createdDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents CreatedDateComponents { get; }

		[NullAllowed, Export ("modifiedDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents ModifiedDateComponents { get; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("priority", ArgumentSemantic.Assign)]
		INTaskPriority Priority { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTaskList : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:tasks:groupName:createdDateComponents:modifiedDateComponents:identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString title, INTask [] tasks, [NullAllowed] INSpeakableString groupName, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier);

		[Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[Export ("tasks", ArgumentSemantic.Copy)]
		INTask [] Tasks { get; }

		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }

		[NullAllowed, Export ("createdDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents CreatedDateComponents { get; }

		[NullAllowed, Export ("modifiedDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents ModifiedDateComponents { get; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTaskListResolutionResult {

		[Static]
		[Export ("successWithResolvedTaskList:")]
		INTaskListResolutionResult GetSuccess (INTaskList resolvedTaskList);

		[Static]
		[Export ("disambiguationWithTaskListsToDisambiguate:")]
		INTaskListResolutionResult GetDisambiguation (INTaskList [] taskListsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithTaskListToConfirm:")]
		INTaskListResolutionResult GetConfirmationRequired ([NullAllowed] INTaskList taskListToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTaskListResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTaskListResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTaskListResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskListResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskListResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTaskResolutionResult {

		[Static]
		[Export ("successWithResolvedTask:")]
		INTaskResolutionResult GetSuccess (INTask resolvedTask);

		[Static]
		[Export ("disambiguationWithTasksToDisambiguate:")]
		INTaskResolutionResult GetDisambiguation (INTask [] tasksToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithTaskToConfirm:")]
		INTaskResolutionResult GetConfirmationRequired ([NullAllowed] INTask taskToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTaskResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTaskResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTaskResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTaskStatusResolutionResult {

		[Static]
		[Export ("successWithResolvedTaskStatus:")]
		INTaskStatusResolutionResult GetSuccess (INTaskStatus resolvedTaskStatus);

		[Static]
		[Export ("confirmationRequiredWithTaskStatusToConfirm:")]
		INTaskStatusResolutionResult GetConfirmationRequired (INTaskStatus taskStatusToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTaskStatusResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTaskStatusResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTaskStatusResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskStatusResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskStatusResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTemporalEventTrigger : NSCopying, NSSecureCoding {

		[Export ("initWithDateComponentsRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INDateComponentsRange dateComponentsRange);

		[Export ("dateComponentsRange", ArgumentSemantic.Copy)]
		INDateComponentsRange DateComponentsRange { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTemporalEventTriggerResolutionResult {

		[Static]
		[Export ("successWithResolvedTemporalEventTrigger:")]
		INTemporalEventTriggerResolutionResult GetSuccess (INTemporalEventTrigger resolvedTemporalEventTrigger);

		[Static]
		[Export ("disambiguationWithTemporalEventTriggersToDisambiguate:")]
		INTemporalEventTriggerResolutionResult GetDisambiguation (INTemporalEventTrigger [] temporalEventTriggersToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithTemporalEventTriggerToConfirm:")]
		INTemporalEventTriggerResolutionResult GetConfirmationRequired ([NullAllowed] INTemporalEventTrigger temporalEventTriggerToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTemporalEventTriggerResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTemporalEventTriggerResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTemporalEventTriggerResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTemporalEventTriggerResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTemporalEventTriggerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INNoteContent))]
	[DisableDefaultCtor]
	interface INTextNoteContent : NSSecureCoding, NSCopying {

		[Export ("initWithText:")]
		NativeHandle Constructor (string text);

		[NullAllowed, Export ("text")]
		string Text { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INTransferMoneyIntent {

		[Export ("initWithFromAccount:toAccount:transactionAmount:transactionScheduledDate:transactionNote:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPaymentAccount fromAccount, [NullAllowed] INPaymentAccount toAccount, [NullAllowed] INPaymentAmount transactionAmount, [NullAllowed] INDateComponentsRange transactionScheduledDate, [NullAllowed] string transactionNote);

		[NullAllowed, Export ("fromAccount", ArgumentSemantic.Copy)]
		INPaymentAccount FromAccount { get; }

		[NullAllowed, Export ("toAccount", ArgumentSemantic.Copy)]
		INPaymentAccount ToAccount { get; }

		[NullAllowed, Export ("transactionAmount", ArgumentSemantic.Copy)]
		INPaymentAmount TransactionAmount { get; }

		[NullAllowed, Export ("transactionScheduledDate", ArgumentSemantic.Copy)]
		INDateComponentsRange TransactionScheduledDate { get; }

		[NullAllowed, Export ("transactionNote")]
		string TransactionNote { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INTransferMoneyIntentHandling {

		[Abstract]
		[Export ("handleTransferMoney:completion:")]
		void HandleTransferMoney (INTransferMoneyIntent intent, Action<INTransferMoneyIntentResponse> completion);

		[Export ("confirmTransferMoney:completion:")]
		void Confirm (INTransferMoneyIntent intent, Action<INTransferMoneyIntentResponse> completion);

		[Export ("resolveFromAccountForTransferMoney:withCompletion:")]
		void ResolveFromAccount (INTransferMoneyIntent intent, Action<INPaymentAccountResolutionResult> completion);

		[Export ("resolveToAccountForTransferMoney:withCompletion:")]
		void ResolveToAccount (INTransferMoneyIntent intent, Action<INPaymentAccountResolutionResult> completion);

		[Export ("resolveTransactionAmountForTransferMoney:withCompletion:")]
		void ResolveTransactionAmount (INTransferMoneyIntent intent, Action<INPaymentAmountResolutionResult> completion);

		[Export ("resolveTransactionScheduledDateForTransferMoney:withCompletion:")]
		void ResolveTransactionScheduledDate (INTransferMoneyIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveTransactionNoteForTransferMoney:withCompletion:")]
		void ResolveTransactionNote (INTransferMoneyIntent intent, Action<INStringResolutionResult> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INTransferMoneyIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INTransferMoneyIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INTransferMoneyIntentResponseCode Code { get; }

		[NullAllowed, Export ("fromAccount", ArgumentSemantic.Copy)]
		INPaymentAccount FromAccount { get; set; }

		[NullAllowed, Export ("toAccount", ArgumentSemantic.Copy)]
		INPaymentAccount ToAccount { get; set; }

		[NullAllowed, Export ("transactionAmount", ArgumentSemantic.Copy)]
		INPaymentAmount TransactionAmount { get; set; }

		[NullAllowed, Export ("transactionScheduledDate", ArgumentSemantic.Copy)]
		INDateComponentsRange TransactionScheduledDate { get; set; }

		[NullAllowed, Export ("transactionNote")]
		string TransactionNote { get; set; }

		[NullAllowed, Export ("transferFee", ArgumentSemantic.Copy)]
		INCurrencyAmount TransferFee { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INVisualCodeTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedVisualCodeType:")]
		INVisualCodeTypeResolutionResult GetSuccess (INVisualCodeType resolvedVisualCodeType);

		[Static]
		[Export ("confirmationRequiredWithVisualCodeTypeToConfirm:")]
		INVisualCodeTypeResolutionResult GetConfirmationRequired (INVisualCodeType visualCodeTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INVisualCodeTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INVisualCodeTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INVisualCodeTypeResolutionResult Unsupported { get; }

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INVisualCodeTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Watch (6, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INVisualCodeTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (5, 0), NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INDefaultCardTemplate : NSCopying, NSSecureCoding {

		[Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		INImage Image { get; set; }

		[Export ("initWithTitle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title);
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMediaItem : NSCopying, NSSecureCoding {

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:title:type:artwork:artist:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string identifier, [NullAllowed] string title, INMediaItemType type, [NullAllowed] INImage artwork, [NullAllowed] string artist);

		[Export ("initWithIdentifier:title:type:artwork:")]
		NativeHandle Constructor ([NullAllowed] string identifier, [NullAllowed] string title, INMediaItemType type, [NullAllowed] INImage artwork);

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("type", ArgumentSemantic.Assign)]
		INMediaItemType Type { get; }

		[NullAllowed, Export ("artwork", ArgumentSemantic.Copy)]
		INImage Artwork { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("artist")]
		string Artist { get; }
	}

	[Watch (5, 0), NoTV, Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INObject : INSpeakable, NSCopying, NSSecureCoding {

		[Export ("initWithIdentifier:displayString:pronunciationHint:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString, [NullAllowed] string pronunciationHint);

		[Export ("initWithIdentifier:displayString:")]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString);

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithIdentifier:displayString:subtitleString:displayImage:")]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString, [NullAllowed] string subtitleString, [NullAllowed] INImage displayImage);

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithIdentifier:displayString:pronunciationHint:subtitleString:displayImage:")]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString, [NullAllowed] string pronunciationHint, [NullAllowed] string subtitleString, [NullAllowed] INImage displayImage);

		// Inlined by INSpeakable
		//[NullAllowed, Export ("identifier", ArgumentSemantic.Strong)]
		//string Identifier { get; }

		[Export ("displayString")]
		string DisplayString { get; }

		// Inlined by INSpeakable
		//[NullAllowed, Export ("pronunciationHint", ArgumentSemantic.Strong)]
		//string PronunciationHint { get; }

		[Sealed]
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("alternativeSpeakableMatches")]
		[return: NullAllowed]
		INSpeakableString [] GetAlternativeSpeakableMatches ();

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("subtitleString")]
		string SubtitleString { get; set; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("displayImage", ArgumentSemantic.Strong)]
		INImage DisplayImage { get; set; }

		// Not [Sealed] since the 'AlternativeSpeakableMatches' inlined property is read-only
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setAlternativeSpeakableMatches:")]
		void SetAlternativeSpeakableMatches ([NullAllowed] INSpeakableString [] alternativeSpeakableMatches);
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INPlayMediaIntent {

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithMediaItems:mediaContainer:playShuffled:playbackRepeatMode:resumePlayback:playbackQueueLocation:playbackSpeed:mediaSearch:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INMediaItem [] mediaItems, [NullAllowed] INMediaItem mediaContainer, [NullAllowed, BindAs (typeof (bool?))] NSNumber playShuffled, INPlaybackRepeatMode playbackRepeatMode, [NullAllowed, BindAs (typeof (bool?))] NSNumber resumePlayback, INPlaybackQueueLocation playbackQueueLocation, [NullAllowed, BindAs (typeof (double?))] NSNumber playbackSpeed, [NullAllowed] INMediaSearch mediaSearch);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the '.ctor (INMediaItem [], INMediaItem, bool?, INPlaybackRepeatMode, bool?, INPlaybackQueueLocation, double?, INMediaSearch)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the '.ctor (INMediaItem [], INMediaItem, bool?, INPlaybackRepeatMode, bool?, INPlaybackQueueLocation, double?, INMediaSearch)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the '.ctor (INMediaItem [], INMediaItem, bool?, INPlaybackRepeatMode, bool?, INPlaybackQueueLocation, double?, INMediaSearch)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the '.ctor (INMediaItem [], INMediaItem, bool?, INPlaybackRepeatMode, bool?, INPlaybackQueueLocation, double?, INMediaSearch)' instead.")]
		[Export ("initWithMediaItems:mediaContainer:playShuffled:playbackRepeatMode:resumePlayback:")]
		NativeHandle Constructor ([NullAllowed] INMediaItem [] mediaItems, [NullAllowed] INMediaItem mediaContainer, [NullAllowed, BindAs (typeof (bool?))] NSNumber playShuffled, INPlaybackRepeatMode playbackRepeatMode, [NullAllowed, BindAs (typeof (bool?))] NSNumber resumePlayback);

		[NullAllowed, Export ("mediaItems", ArgumentSemantic.Copy)]
		INMediaItem [] MediaItems { get; }

		[NullAllowed, Export ("mediaContainer", ArgumentSemantic.Copy)]
		INMediaItem MediaContainer { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("playShuffled", ArgumentSemantic.Copy)]
		NSNumber PlayShuffled { get; }

		[Export ("playbackRepeatMode", ArgumentSemantic.Assign)]
		INPlaybackRepeatMode PlaybackRepeatMode { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("resumePlayback", ArgumentSemantic.Copy)]
		NSNumber ResumePlayback { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playbackQueueLocation", ArgumentSemantic.Assign)]
		INPlaybackQueueLocation PlaybackQueueLocation { get; }

		[BindAs (typeof (double?))]
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("playbackSpeed", ArgumentSemantic.Copy)]
		NSNumber PlaybackSpeed { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("mediaSearch", ArgumentSemantic.Copy)]
		INMediaSearch MediaSearch { get; }
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INPlayMediaIntentHandling {

		[Abstract]
		[Export ("handlePlayMedia:completion:")]
		void HandlePlayMedia (INPlayMediaIntent intent, Action<INPlayMediaIntentResponse> completion);

		[Export ("confirmPlayMedia:completion:")]
		void Confirm (INPlayMediaIntent intent, Action<INPlayMediaIntentResponse> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveMediaItemsForPlayMedia:withCompletion:")]
		void ResolveMediaItems (INPlayMediaIntent intent, Action<NSArray<INPlayMediaMediaItemResolutionResult>> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlayShuffledForPlayMedia:withCompletion:")]
		void ResolvePlayShuffled (INPlayMediaIntent intent, Action<INBooleanResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlaybackRepeatModeForPlayMedia:withCompletion:")]
		void ResolvePlaybackRepeatMode (INPlayMediaIntent intent, Action<INPlaybackRepeatModeResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveResumePlaybackForPlayMedia:withCompletion:")]
		void ResolveResumePlayback (INPlayMediaIntent intent, Action<INBooleanResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlaybackQueueLocationForPlayMedia:withCompletion:")]
		void ResolvePlaybackQueueLocation (INPlayMediaIntent intent, Action<INPlaybackQueueLocationResolutionResult> completion);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlaybackSpeedForPlayMedia:withCompletion:")]
		void ResolvePlaybackSpeed (INPlayMediaIntent intent, Action<INPlayMediaPlaybackSpeedResolutionResult> completion);
	}

	[Watch (5, 0), TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INPlayMediaIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INPlayMediaIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INPlayMediaIntentResponseCode Code { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[NullAllowed, Export ("nowPlayingInfo", ArgumentSemantic.Copy)]
		NSDictionary WeakNowPlayingInfo { get; set; }
	}

	[Abstract]
	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRelevanceProvider : NSCopying, NSSecureCoding {
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INRelevanceProvider))]
	[DisableDefaultCtor]
	interface INDateRelevanceProvider {

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		[Export ("initWithStartDate:endDate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDate startDate, [NullAllowed] NSDate endDate);
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INRelevanceProvider))]
	[DisableDefaultCtor]
	interface INLocationRelevanceProvider {

		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; }

		[Export ("initWithRegion:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CLRegion region);
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INRelevanceProvider))]
	[DisableDefaultCtor]
	interface INDailyRoutineRelevanceProvider {

		[Export ("situation")]
		INDailyRoutineSituation Situation { get; }

		[Export ("initWithSituation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INDailyRoutineSituation situation);
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRelevantShortcut : NSSecureCoding, NSCopying {

		[Export ("relevanceProviders", ArgumentSemantic.Copy)]
		INRelevanceProvider [] RelevanceProviders { get; set; }

		[NullAllowed, Export ("watchTemplate", ArgumentSemantic.Copy)]
		INDefaultCardTemplate WatchTemplate { get; set; }

		[iOS (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("widgetKind")]
		string WidgetKind { get; set; }

		[Export ("shortcutRole", ArgumentSemantic.Assign)]
		INRelevantShortcutRole ShortcutRole { get; set; }

		[Export ("shortcut", ArgumentSemantic.Copy)]
		INShortcut Shortcut { get; }

		[Export ("initWithShortcut:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INShortcut shortcut);
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRelevantShortcutStore {

		[Static]
		[Export ("defaultStore", ArgumentSemantic.Strong)]
		INRelevantShortcutStore DefaultStore { get; }

		[Async]
		[Export ("setRelevantShortcuts:completionHandler:")]
		void SetRelevantShortcuts (INRelevantShortcut [] shortcuts, [NullAllowed] Action<NSError> completionHandler);
	}

	[Watch (5, 0), NoTV, Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INShortcut : NSSecureCoding, NSCopying {

		[NullAllowed, Export ("intent", ArgumentSemantic.Copy)]
		INIntent Intent { get; }

		[NullAllowed, Export ("userActivity", ArgumentSemantic.Strong)]
		NSUserActivity UserActivity { get; }

		[Export ("initWithIntent:")]
		NativeHandle Constructor (INIntent intent);

		[Export ("initWithUserActivity:")]
		NativeHandle Constructor (NSUserActivity userActivity);
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INUpcomingMediaManager {

		[Static]
		[Export ("sharedManager")]
		INUpcomingMediaManager SharedManager { get; }

		[Export ("setSuggestedMediaIntents:")]
		void SetSuggestedMediaIntents (NSOrderedSet<INPlayMediaIntent> intents);

		[Export ("setPredictionMode:forType:")]
		void SetPredictionMode (INUpcomingMediaPredictionMode mode, INMediaItemType type);
	}

	[Watch (5, 0), NoTV, Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INVoiceShortcut : NSSecureCoding, NSCopying {

		[Export ("identifier", ArgumentSemantic.Strong)]
		NSUuid Identifier { get; }

		[Export ("invocationPhrase")]
		string InvocationPhrase { get; }

		[Export ("shortcut", ArgumentSemantic.Copy)]
		INShortcut Shortcut { get; }
	}

	delegate void INVoiceShortcutCenterGetVoiceShortcutsHandler ([NullAllowed] INVoiceShortcut [] voiceShortcuts, NSError error);

	[Watch (5, 0), NoTV, Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INVoiceShortcutCenter {

		[Static]
		[Export ("sharedCenter", ArgumentSemantic.Strong)]
		INVoiceShortcutCenter SharedCenter { get; }

		[Async]
		[Export ("getAllVoiceShortcutsWithCompletion:")]
		void GetAllVoiceShortcuts (INVoiceShortcutCenterGetVoiceShortcutsHandler completionHandler);

		[Async]
		[Export ("getVoiceShortcutWithIdentifier:completion:")]
		void GetVoiceShortcut (NSUuid identifier, Action<INVoiceShortcut, NSError> completionHandler);

		[Export ("setShortcutSuggestions:")]
		void SetShortcutSuggestions (INShortcut [] suggestions);
	}

	// TODO: We need to inline these into NSString once we figure out how the API is used.
	//[Watch (5,0)]
	//[Category]
	//[BaseType (typeof (NSString))]
	//interface NSString_Intents {
	//	// +(NSString * _Nonnull)deferredLocalizedIntentsStringWithFormat:(NSString * _Nonnull)format, ... __attribute__((format(NSString, 1, 2)));
	//	[Static, Internal]
	//	[Export ("deferredLocalizedIntentsStringWithFormat:", IsVariadic = true)]
	//	string DeferredLocalizedIntentsStringWithFormat (string format, IntPtr varArgs);

	//	// +(NSString * _Nonnull)deferredLocalizedIntentsStringWithFormat:(NSString * _Nonnull)format fromTable:(NSString * _Nullable)table, ... __attribute__((format(NSString, 1, 3)));
	//	[Static, Internal]
	//	[Export ("deferredLocalizedIntentsStringWithFormat:fromTable:", IsVariadic = true)]
	//	string DeferredLocalizedIntentsStringWithFormat (string format, [NullAllowed] string table, IntPtr varArgs);

	//	// +(NSString * _Nonnull)deferredLocalizedIntentsStringWithFormat:(NSString * _Nonnull)format fromTable:(NSString * _Nullable)table arguments:(va_list)arguments __attribute__((format(NSString, 1, 0)));
	//	[Static, Internal]
	//	[Export ("deferredLocalizedIntentsStringWithFormat:fromTable:arguments:", IsVariadic = true)]
	//	string DeferredLocalizedIntentsStringWithFormat (string format, [NullAllowed] string table, IntPtr arguments);
	//}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INAddMediaIntent {

		[Export ("initWithMediaItems:mediaSearch:mediaDestination:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INMediaItem [] mediaItems, [NullAllowed] INMediaSearch mediaSearch, [NullAllowed] INMediaDestination mediaDestination);

		[NullAllowed, Export ("mediaItems", ArgumentSemantic.Copy)]
		INMediaItem [] MediaItems { get; }

		[NullAllowed, Export ("mediaSearch", ArgumentSemantic.Copy)]
		INMediaSearch MediaSearch { get; }

		[NullAllowed, Export ("mediaDestination", ArgumentSemantic.Copy)]
		INMediaDestination MediaDestination { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INAddMediaIntentHandling {

		[Abstract]
		[Export ("handleAddMedia:completion:")]
		void HandleAddMedia (INAddMediaIntent intent, Action<INAddMediaIntentResponse> completion);

		[Export ("confirmAddMedia:completion:")]
		void Confirm (INAddMediaIntent intent, Action<INAddMediaIntentResponse> completion);

		[Export ("resolveMediaItemsForAddMedia:withCompletion:")]
		void ResolveMediaItems (INAddMediaIntent intent, Action<INAddMediaMediaItemResolutionResult []> completion);

		[Export ("resolveMediaDestinationForAddMedia:withCompletion:")]
		void ResolveMediaDestination (INAddMediaIntent intent, Action<INAddMediaMediaDestinationResolutionResult> completion);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INAddMediaIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INAddMediaIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INAddMediaIntentResponseCode Code { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INMediaItemResolutionResult))]
	[DisableDefaultCtor]
	interface INAddMediaMediaItemResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INAddMediaMediaItemResolutionResult GetUnsupported (INAddMediaMediaItemUnsupportedReason reason);

		[Export ("initWithMediaItemResolutionResult:")]
		NativeHandle Constructor (INMediaItemResolutionResult mediaItemResolutionResult);

		// Inlined from parent class to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedMediaItem:")]
		INAddMediaMediaItemResolutionResult GetSuccess (INMediaItem resolvedMediaItem);

		[New]
		[Static]
		[Export ("successesWithResolvedMediaItems:")]
		INAddMediaMediaItemResolutionResult [] GetSuccesses (INMediaItem [] resolvedMediaItems);

		[New]
		[Static]
		[Export ("disambiguationWithMediaItemsToDisambiguate:")]
		INAddMediaMediaItemResolutionResult GetDisambiguation (INMediaItem [] mediaItemsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithMediaItemToConfirm:")]
		INAddMediaMediaItemResolutionResult GetConfirmationRequired ([NullAllowed] INMediaItem mediaItemToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INAddMediaMediaItemResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INAddMediaMediaItemResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INAddMediaMediaItemResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INAddMediaMediaItemResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INAddMediaMediaItemResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INTaskListResolutionResult))]
	[DisableDefaultCtor]
	interface INAddTasksTargetTaskListResolutionResult {

		[Static]
		[Export ("confirmationRequiredWithTaskListToConfirm:forReason:")]
		INAddTasksTargetTaskListResolutionResult GetConfirmationRequired ([NullAllowed] INTaskList taskListToConfirm, INAddTasksTargetTaskListConfirmationReason reason);

		[Export ("initWithTaskListResolutionResult:")]
		NativeHandle Constructor (INTaskListResolutionResult taskListResolutionResult);

		// Inlined from parent class to avoid bug like 43205
		[New]
		[Static]
		[Export ("successWithResolvedTaskList:")]
		INAddTasksTargetTaskListResolutionResult GetSuccess (INTaskList resolvedTaskList);

		[New]
		[Static]
		[Export ("disambiguationWithTaskListsToDisambiguate:")]
		INAddTasksTargetTaskListResolutionResult GetDisambiguation (INTaskList [] taskListsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithTaskListToConfirm:")]
		INAddTasksTargetTaskListResolutionResult GetConfirmationRequired ([NullAllowed] INTaskList taskListToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INAddTasksTargetTaskListResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INAddTasksTargetTaskListResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INAddTasksTargetTaskListResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INAddTasksTargetTaskListResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INAddTasksTargetTaskListResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INTemporalEventTriggerResolutionResult))]
	[DisableDefaultCtor]
	interface INAddTasksTemporalEventTriggerResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INAddTasksTemporalEventTriggerResolutionResult GetUnsupported (INAddTasksTemporalEventTriggerUnsupportedReason reason);

		[Export ("initWithTemporalEventTriggerResolutionResult:")]
		NativeHandle Constructor (INTemporalEventTriggerResolutionResult temporalEventTriggerResolutionResult);

		// Inlined from parent to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedTemporalEventTrigger:")]
		INAddTasksTemporalEventTriggerResolutionResult GetSuccess (INTemporalEventTrigger resolvedTemporalEventTrigger);

		[New]
		[Static]
		[Export ("disambiguationWithTemporalEventTriggersToDisambiguate:")]
		INAddTasksTemporalEventTriggerResolutionResult GetDisambiguation (INTemporalEventTrigger [] temporalEventTriggersToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithTemporalEventTriggerToConfirm:")]
		INAddTasksTemporalEventTriggerResolutionResult GetConfirmationRequired ([NullAllowed] INTemporalEventTrigger temporalEventTriggerToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INAddTasksTemporalEventTriggerResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INAddTasksTemporalEventTriggerResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INAddTasksTemporalEventTriggerResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INAddTasksTemporalEventTriggerResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INAddTasksTemporalEventTriggerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INAirline : NSCopying, NSSecureCoding {

		[Export ("initWithName:iataCode:icaoCode:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string name, [NullAllowed] string iataCode, [NullAllowed] string icaoCode);

		[NullAllowed, Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("iataCode")]
		string IataCode { get; }

		[NullAllowed, Export ("icaoCode")]
		string IcaoCode { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INAirport : NSCopying, NSSecureCoding {

		[Export ("initWithName:iataCode:icaoCode:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string name, [NullAllowed] string iataCode, [NullAllowed] string icaoCode);

		[NullAllowed, Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("iataCode")]
		string IataCode { get; }

		[NullAllowed, Export ("icaoCode")]
		string IcaoCode { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INAirportGate : NSCopying, NSSecureCoding {

		[Export ("initWithAirport:terminal:gate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INAirport airport, [NullAllowed] string terminal, [NullAllowed] string gate);

		[Export ("airport", ArgumentSemantic.Copy)]
		INAirport Airport { get; }

		[NullAllowed, Export ("terminal")]
		string Terminal { get; }

		[NullAllowed, Export ("gate")]
		string Gate { get; }
	}

	[Watch (6, 0), NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallCapabilityResolutionResult {

		[Static]
		[Export ("successWithResolvedCallCapability:")]
		INCallCapabilityResolutionResult GetSuccess (INCallCapability resolvedCallCapability);

		[Static]
		[Export ("confirmationRequiredWithCallCapabilityToConfirm:")]
		INCallCapabilityResolutionResult GetConfirmationRequired (INCallCapability callCapabilityToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCallCapabilityResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCallCapabilityResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCallCapabilityResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallCapabilityResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallCapabilityResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INDeleteTasksIntent {

		[Export ("initWithTaskList:tasks:all:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INTaskList taskList, [NullAllowed] INTask [] tasks, [NullAllowed][BindAs (typeof (bool?))] NSNumber all);

		[NullAllowed, Export ("taskList", ArgumentSemantic.Copy)]
		INTaskList TaskList { get; }

		[NullAllowed, Export ("tasks", ArgumentSemantic.Copy)]
		INTask [] Tasks { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("all", ArgumentSemantic.Copy)]
		NSNumber All { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INDeleteTasksIntentHandling {

		[Abstract]
		[Export ("handleDeleteTasks:completion:")]
		void HandleDeleteTasks (INDeleteTasksIntent intent, Action<INDeleteTasksIntentResponse> completion);

		[Export ("confirmDeleteTasks:completion:")]
		void Confirm (INDeleteTasksIntent intent, Action<INDeleteTasksIntentResponse> completion);

		[Export ("resolveTaskListForDeleteTasks:withCompletion:")]
		void ResolveTaskList (INDeleteTasksIntent intent, Action<INDeleteTasksTaskListResolutionResult> completion);

		[Export ("resolveTasksForDeleteTasks:withCompletion:")]
		void ResolveTasks (INDeleteTasksIntent intent, Action<INDeleteTasksTaskResolutionResult []> completion);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INDeleteTasksIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INDeleteTasksIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INDeleteTasksIntentResponseCode Code { get; }

		[NullAllowed, Export ("deletedTasks", ArgumentSemantic.Copy)]
		INTask [] DeletedTasks { get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INTaskListResolutionResult))]
	[DisableDefaultCtor]
	interface INDeleteTasksTaskListResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INDeleteTasksTaskListResolutionResult GetUnsupported (INDeleteTasksTaskListUnsupportedReason reason);

		[Export ("initWithTaskListResolutionResult:")]
		NativeHandle Constructor (INTaskListResolutionResult taskListResolutionResult);

		// Inlined from parent class to avoid bug like 43205
		[New]
		[Static]
		[Export ("successWithResolvedTaskList:")]
		INDeleteTasksTaskListResolutionResult GetSuccess (INTaskList resolvedTaskList);

		[New]
		[Static]
		[Export ("disambiguationWithTaskListsToDisambiguate:")]
		INDeleteTasksTaskListResolutionResult GetDisambiguation (INTaskList [] taskListsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithTaskListToConfirm:")]
		INDeleteTasksTaskListResolutionResult GetConfirmationRequired ([NullAllowed] INTaskList taskListToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDeleteTasksTaskListResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDeleteTasksTaskListResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDeleteTasksTaskListResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDeleteTasksTaskListResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDeleteTasksTaskListResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.WatchOS, 8, 0)]
	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INTaskResolutionResult))]
	[DisableDefaultCtor]
	interface INDeleteTasksTaskResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INDeleteTasksTaskResolutionResult GetUnsupported (INDeleteTasksTaskUnsupportedReason reason);

		[Export ("initWithTaskResolutionResult:")]
		NativeHandle Constructor (INTaskResolutionResult taskResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedTask:")]
		INDeleteTasksTaskResolutionResult GetSuccess (INTask resolvedTask);

		[New]
		[Static]
		[Export ("disambiguationWithTasksToDisambiguate:")]
		INDeleteTasksTaskResolutionResult GetDisambiguation (INTask [] tasksToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithTaskToConfirm:")]
		INDeleteTasksTaskResolutionResult GetConfirmationRequired ([NullAllowed] INTask taskToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDeleteTasksTaskResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDeleteTasksTaskResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDeleteTasksTaskResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDeleteTasksTaskResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDeleteTasksTaskResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), Mac (11, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INEnergyResolutionResult {

		[Static]
		[Export ("successWithResolvedEnergy:")]
		INEnergyResolutionResult GetSuccess (NSMeasurement<NSUnitEnergy> resolvedEnergy);

		[Static]
		[Export ("disambiguationWithEnergyToDisambiguate:")]
		INEnergyResolutionResult GetDisambiguation (NSMeasurement<NSUnitEnergy> [] energyToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithEnergyToConfirm:")]
		INEnergyResolutionResult GetConfirmationRequired ([NullAllowed] NSMeasurement<NSUnitEnergy> energyToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INEnergyResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INEnergyResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INEnergyResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INEnergyResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INEnergyResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), Mac (11, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INEnumResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INEnumResolutionResult GetSuccess (nint resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INEnumResolutionResult GetConfirmationRequired (nint valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INEnumResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INEnumResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INEnumResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INEnumResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INEnumResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INFile : NSSecureCoding {

		[Static]
		[Export ("fileWithData:filename:typeIdentifier:")]
		INFile Create (NSData data, string filename, [NullAllowed] string typeIdentifier);

		[Static]
		[Export ("fileWithFileURL:filename:typeIdentifier:")]
		INFile Create (NSUrl fileUrl, [NullAllowed] string filename, [NullAllowed] string typeIdentifier);

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[Export ("filename")]
		string Filename { get; set; }

		[NullAllowed, Export ("typeIdentifier")]
		string TypeIdentifier { get; }

		[NullAllowed, Export ("fileURL", ArgumentSemantic.Strong)]
		NSUrl FileUrl { get; }

		[Watch (8, 3), Mac (12, 1), iOS (15, 2)]
		[MacCatalyst (15, 2)]
		[Export ("removedOnCompletion")]
		bool RemovedOnCompletion { get; set; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INFileResolutionResult {

		[Static]
		[Export ("successWithResolvedFile:")]
		INFileResolutionResult GetSuccess (INFile resolvedFile);

		[Static]
		[Export ("disambiguationWithFilesToDisambiguate:")]
		INFileResolutionResult GetDisambiguation (INFile [] filesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithFileToConfirm:")]
		INFileResolutionResult GetConfirmationRequired ([NullAllowed] INFile fileToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INFileResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INFileResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INFileResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INFileResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INFileResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INFlight : NSCopying, NSSecureCoding {

		[Export ("initWithAirline:flightNumber:boardingTime:flightDuration:departureAirportGate:arrivalAirportGate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INAirline airline, string flightNumber, [NullAllowed] INDateComponentsRange boardingTime, INDateComponentsRange flightDuration, INAirportGate departureAirportGate, INAirportGate arrivalAirportGate);

		[Export ("airline", ArgumentSemantic.Copy)]
		INAirline Airline { get; }

		[Export ("flightNumber")]
		string FlightNumber { get; }

		[NullAllowed, Export ("boardingTime", ArgumentSemantic.Copy)]
		INDateComponentsRange BoardingTime { get; }

		[Export ("flightDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange FlightDuration { get; }

		[Export ("departureAirportGate", ArgumentSemantic.Copy)]
		INAirportGate DepartureAirportGate { get; }

		[Export ("arrivalAirportGate", ArgumentSemantic.Copy)]
		INAirportGate ArrivalAirportGate { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INFlightReservation : NSCopying, NSSecureCoding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:reservedSeat:flight:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, [NullAllowed] INSeat reservedSeat, INFlight flight);

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:reservedSeat:flight:")]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] INSeat reservedSeat, INFlight flight);

		[NullAllowed, Export ("reservedSeat", ArgumentSemantic.Copy)]
		INSeat ReservedSeat { get; }

		[Export ("flight", ArgumentSemantic.Copy)]
		INFlight Flight { get; }
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum INFocusStatusAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum INUnsendMessagesIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageNotFound,
		FailurePastUnsendTimeLimit,
		FailureMessageTypeUnsupported,
		FailureUnsupportedOnService,
		FailureMessageServiceNotAvailable,
		FailureRequiringInAppAuthentication,
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum INEditMessageIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageNotFound,
		FailurePastEditTimeLimit,
		FailureMessageTypeUnsupported,
		FailureUnsupportedOnService,
		FailureMessageServiceNotAvailable,
		FailureRequiringInAppAuthentication,
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INFocusStatus : NSCopying, NSSecureCoding {
		[Export ("initWithIsFocused:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (bool?))] NSNumber isFocused);

		[Export ("isFocused", ArgumentSemantic.Copy)]
		[BindAs (typeof (bool?))]
		[NullAllowed]
		NSNumber IsFocused { get; }
	}

	[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface INFocusStatusCenter {
		[Static]
		[Export ("defaultCenter", ArgumentSemantic.Strong)]
		INFocusStatusCenter DefaultCenter { get; }

		[Export ("focusStatus")]
		INFocusStatus FocusStatus { get; }

		[Export ("authorizationStatus")]
		INFocusStatusAuthorizationStatus AuthorizationStatus { get; }

		[Async]
		[Export ("requestAuthorizationWithCompletionHandler:")]
		void RequestAuthorization ([NullAllowed] Action<INFocusStatusAuthorizationStatus> completionHandler);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INGetReservationDetailsIntent {

		[Export ("initWithReservationContainerReference:reservationItemReferences:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INSpeakableString reservationContainerReference, [NullAllowed] INSpeakableString [] reservationItemReferences);

		[NullAllowed, Export ("reservationContainerReference", ArgumentSemantic.Copy)]
		INSpeakableString ReservationContainerReference { get; }

		[NullAllowed, Export ("reservationItemReferences", ArgumentSemantic.Copy)]
		INSpeakableString [] ReservationItemReferences { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INGetReservationDetailsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INGetReservationDetailsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetReservationDetailsIntentResponseCode Code { get; }

		[NullAllowed, Export ("reservations", ArgumentSemantic.Copy)]
		INReservation [] Reservations { get; set; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INLengthResolutionResult {

		[Static]
		[Export ("successWithResolvedLength:")]
		INLengthResolutionResult GetSuccess (NSMeasurement<NSUnitLength> resolvedLength);

		[Static]
		[Export ("disambiguationWithLengthsToDisambiguate:")]
		INLengthResolutionResult GetDisambiguation (NSMeasurement<NSUnitLength> [] lengthsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithLengthToConfirm:")]
		INLengthResolutionResult GetConfirmationRequired ([NullAllowed] NSMeasurement<NSUnitLength> lengthToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INLengthResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INLengthResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INLengthResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INLengthResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INLengthResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INLodgingReservation : NSCopying, NSSecureCoding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:lodgingBusinessLocation:reservationDuration:numberOfAdults:numberOfChildren:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, CLPlacemark lodgingBusinessLocation, INDateComponentsRange reservationDuration, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfAdults, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfChildren);

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:lodgingBusinessLocation:reservationDuration:numberOfAdults:numberOfChildren:")]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, CLPlacemark lodgingBusinessLocation, INDateComponentsRange reservationDuration, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfAdults, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfChildren);

		[Export ("lodgingBusinessLocation", ArgumentSemantic.Copy)]
		CLPlacemark LodgingBusinessLocation { get; }

		[Export ("reservationDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange ReservationDuration { get; }

		[BindAs (typeof (int?))]
		[NullAllowed, Export ("numberOfAdults", ArgumentSemantic.Copy)]
		NSNumber NumberOfAdults { get; }

		[BindAs (typeof (int?))]
		[NullAllowed, Export ("numberOfChildren", ArgumentSemantic.Copy)]
		NSNumber NumberOfChildren { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMassResolutionResult {

		[Static]
		[Export ("successWithResolvedMass:")]
		INMassResolutionResult GetSuccess (NSMeasurement<NSUnitMass> resolvedMass);

		[Static]
		[Export ("disambiguationWithMassToDisambiguate:")]
		INMassResolutionResult GetDisambiguation (NSMeasurement<NSUnitMass> [] massToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithMassToConfirm:")]
		INMassResolutionResult GetConfirmationRequired ([NullAllowed] NSMeasurement<NSUnitMass> massToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INMassResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INMassResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INMassResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMassResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMassResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMediaAffinityTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedMediaAffinityType:")]
		INMediaAffinityTypeResolutionResult GetSuccess (INMediaAffinityType resolvedMediaAffinityType);

		[Static]
		[Export ("confirmationRequiredWithMediaAffinityTypeToConfirm:")]
		INMediaAffinityTypeResolutionResult GetConfirmationRequired (INMediaAffinityType mediaAffinityTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INMediaAffinityTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INMediaAffinityTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INMediaAffinityTypeResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMediaAffinityTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMediaAffinityTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMediaDestination : NSCopying, NSSecureCoding {

		[Static]
		[Export ("libraryDestination")]
		INMediaDestination CreateLibraryDestination ();

		[Static]
		[Export ("playlistDestinationWithName:")]
		INMediaDestination CreatePlaylistDestination (string playlistName);

		[Export ("mediaDestinationType", ArgumentSemantic.Assign)]
		INMediaDestinationType MediaDestinationType { get; }

		[NullAllowed, Export ("playlistName")]
		string PlaylistName { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMediaDestinationResolutionResult {

		[Static]
		[Export ("successWithResolvedMediaDestination:")]
		INMediaDestinationResolutionResult GetSuccess (INMediaDestination resolvedMediaDestination);

		[Static]
		[Export ("disambiguationWithMediaDestinationsToDisambiguate:")]
		INMediaDestinationResolutionResult GetDisambiguation (INMediaDestination [] mediaDestinationsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithMediaDestinationToConfirm:")]
		INMediaDestinationResolutionResult GetConfirmationRequired ([NullAllowed] INMediaDestination mediaDestinationToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INMediaDestinationResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INMediaDestinationResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INMediaDestinationResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMediaDestinationResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMediaDestinationResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMediaItemResolutionResult {

		[Static]
		[Export ("successWithResolvedMediaItem:")]
		INMediaItemResolutionResult GetSuccess (INMediaItem resolvedMediaItem);

		[Static]
		[Export ("successesWithResolvedMediaItems:")]
		INMediaItemResolutionResult [] GetSuccesses (INMediaItem [] resolvedMediaItems);

		[Static]
		[Export ("disambiguationWithMediaItemsToDisambiguate:")]
		INMediaItemResolutionResult GetDisambiguation (INMediaItem [] mediaItemsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithMediaItemToConfirm:")]
		INMediaItemResolutionResult GetConfirmationRequired ([NullAllowed] INMediaItem mediaItemToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INMediaItemResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INMediaItemResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INMediaItemResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMediaItemResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMediaItemResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMediaSearch : NSCopying, NSSecureCoding {

		[Export ("initWithMediaType:sortOrder:mediaName:artistName:albumName:genreNames:moodNames:releaseDate:reference:mediaIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INMediaItemType mediaType, INMediaSortOrder sortOrder, [NullAllowed] string mediaName, [NullAllowed] string artistName, [NullAllowed] string albumName, [NullAllowed] string [] genreNames, [NullAllowed] string [] moodNames, [NullAllowed] INDateComponentsRange releaseDate, INMediaReference reference, [NullAllowed] string mediaIdentifier);

		[Export ("mediaType", ArgumentSemantic.Assign)]
		INMediaItemType MediaType { get; }

		[Export ("sortOrder", ArgumentSemantic.Assign)]
		INMediaSortOrder SortOrder { get; }

		[NullAllowed, Export ("mediaName")]
		string MediaName { get; }

		[NullAllowed, Export ("artistName")]
		string ArtistName { get; }

		[NullAllowed, Export ("albumName")]
		string AlbumName { get; }

		[NullAllowed, Export ("genreNames", ArgumentSemantic.Copy)]
		string [] GenreNames { get; }

		[NullAllowed, Export ("moodNames", ArgumentSemantic.Copy)]
		string [] MoodNames { get; }

		[NullAllowed, Export ("releaseDate", ArgumentSemantic.Copy)]
		INDateComponentsRange ReleaseDate { get; }

		[Export ("reference", ArgumentSemantic.Assign)]
		INMediaReference Reference { get; }

		[NullAllowed, Export ("mediaIdentifier")]
		string MediaIdentifier { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INUserContext))]
	interface INMediaUserContext {

		[Export ("subscriptionStatus", ArgumentSemantic.Assign)]
		INMediaUserContextSubscriptionStatus SubscriptionStatus { get; set; }

		[BindAs (typeof (int?))]
		[NullAllowed, Export ("numberOfLibraryItems", ArgumentSemantic.Copy)]
		NSNumber NumberOfLibraryItems { get; set; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INObjectResolutionResult {

		[Static]
		[Export ("successWithResolvedObject:")]
		INObjectResolutionResult GetSuccess (INObject resolvedObject);

		[Static]
		[Export ("disambiguationWithObjectsToDisambiguate:")]
		INObjectResolutionResult GetDisambiguation (INObject [] objectsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithObjectToConfirm:")]
		INObjectResolutionResult GetConfirmationRequired ([NullAllowed] INObject objectToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INObjectResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INObjectResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INObjectResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INObjectResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INObjectResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPaymentMethodResolutionResult {

		[Static]
		[Export ("successWithResolvedPaymentMethod:")]
		INPaymentMethodResolutionResult GetSuccess (INPaymentMethod resolvedPaymentMethod);

		[Static]
		[Export ("disambiguationWithPaymentMethodsToDisambiguate:")]
		INPaymentMethodResolutionResult GetDisambiguation (INPaymentMethod [] paymentMethodsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithPaymentMethodToConfirm:")]
		INPaymentMethodResolutionResult GetConfirmationRequired ([NullAllowed] INPaymentMethod paymentMethodToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPaymentMethodResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPaymentMethodResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPaymentMethodResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentMethodResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentMethodResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INMediaItemResolutionResult))]
	[DisableDefaultCtor]
	interface INPlayMediaMediaItemResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INPlayMediaMediaItemResolutionResult GetUnsupported (INPlayMediaMediaItemUnsupportedReason reason);

		[Export ("initWithMediaItemResolutionResult:")]
		NativeHandle Constructor (INMediaItemResolutionResult mediaItemResolutionResult);

		// Inlined from parent class to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedMediaItem:")]
		INPlayMediaMediaItemResolutionResult GetSuccess (INMediaItem resolvedMediaItem);

		[New]
		[Static]
		[Export ("successesWithResolvedMediaItems:")]
		INPlayMediaMediaItemResolutionResult [] GetSuccesses (INMediaItem [] resolvedMediaItems);

		[New]
		[Static]
		[Export ("disambiguationWithMediaItemsToDisambiguate:")]
		INPlayMediaMediaItemResolutionResult GetDisambiguation (INMediaItem [] mediaItemsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithMediaItemToConfirm:")]
		INPlayMediaMediaItemResolutionResult GetConfirmationRequired ([NullAllowed] INMediaItem mediaItemToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPlayMediaMediaItemResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPlayMediaMediaItemResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPlayMediaMediaItemResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPlayMediaMediaItemResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPlayMediaMediaItemResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INDoubleResolutionResult))]
	[DisableDefaultCtor]
	interface INPlayMediaPlaybackSpeedResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INPlayMediaPlaybackSpeedResolutionResult UnsupportedForReason (INPlayMediaPlaybackSpeedUnsupportedReason reason);

		[Export ("initWithDoubleResolutionResult:")]
		NativeHandle Constructor (INDoubleResolutionResult doubleResolutionResult);

		// Inlined from parent class to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedValue:")]
		INDoubleResolutionResult GetSuccess (double resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INDoubleResolutionResult GetConfirmationRequired ([NullAllowed][BindAs (typeof (double?))] NSNumber valueToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INDoubleResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INDoubleResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INDoubleResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDoubleResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDoubleResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INPlaybackQueueLocationResolutionResult {

		[Static]
		[Export ("successWithResolvedPlaybackQueueLocation:")]
		INPlaybackQueueLocationResolutionResult GetSuccess (INPlaybackQueueLocation resolvedPlaybackQueueLocation);

		[Static]
		[Export ("confirmationRequiredWithPlaybackQueueLocationToConfirm:")]
		INPlaybackQueueLocationResolutionResult GetConfirmationRequired (INPlaybackQueueLocation playbackQueueLocationToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPlaybackQueueLocationResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPlaybackQueueLocationResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPlaybackQueueLocationResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPlaybackQueueLocationResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPlaybackQueueLocationResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPlaybackRepeatModeResolutionResult {

		[Static]
		[Export ("successWithResolvedPlaybackRepeatMode:")]
		INPlaybackRepeatModeResolutionResult GetSuccess (INPlaybackRepeatMode resolvedPlaybackRepeatMode);

		[Static]
		[Export ("confirmationRequiredWithPlaybackRepeatModeToConfirm:")]
		INPlaybackRepeatModeResolutionResult GetConfirmationRequired (INPlaybackRepeatMode playbackRepeatModeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INPlaybackRepeatModeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INPlaybackRepeatModeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INPlaybackRepeatModeResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPlaybackRepeatModeResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPlaybackRepeatModeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRentalCar : NSCopying, NSSecureCoding {

		[Export ("initWithRentalCompanyName:type:make:model:rentalCarDescription:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string rentalCompanyName, [NullAllowed] string type, [NullAllowed] string make, [NullAllowed] string model, [NullAllowed] string rentalCarDescription);

		[Export ("rentalCompanyName")]
		string RentalCompanyName { get; }

		[NullAllowed, Export ("type")]
		string Type { get; }

		[NullAllowed, Export ("make")]
		string Make { get; }

		[NullAllowed, Export ("model")]
		string Model { get; }

		[NullAllowed, Export ("rentalCarDescription")]
		string RentalCarDescription { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INRentalCarReservation : NSCopying, NSSecureCoding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:rentalCar:rentalDuration:pickupLocation:dropOffLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, INRentalCar rentalCar, INDateComponentsRange rentalDuration, [NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation);

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:rentalCar:rentalDuration:pickupLocation:dropOffLocation:")]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, INRentalCar rentalCar, INDateComponentsRange rentalDuration, [NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation);

		[Export ("rentalCar", ArgumentSemantic.Copy)]
		INRentalCar RentalCar { get; }

		[Export ("rentalDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange RentalDuration { get; }

		[NullAllowed, Export ("pickupLocation", ArgumentSemantic.Copy)]
		CLPlacemark PickupLocation { get; }

		[NullAllowed, Export ("dropOffLocation", ArgumentSemantic.Copy)]
		CLPlacemark DropOffLocation { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INReservation : NSCopying, NSSecureCoding {

		[Export ("itemReference", ArgumentSemantic.Copy)]
		INSpeakableString ItemReference { get; }

		[NullAllowed, Export ("reservationNumber")]
		string ReservationNumber { get; }

		[NullAllowed, Export ("bookingTime", ArgumentSemantic.Copy)]
		NSDate BookingTime { get; }

		[Export ("reservationStatus", ArgumentSemantic.Assign)]
		INReservationStatus ReservationStatus { get; }

		[NullAllowed, Export ("reservationHolderName")]
		string ReservationHolderName { get; }

		[NullAllowed, Export ("actions", ArgumentSemantic.Copy)]
		INReservationAction [] Actions { get; }

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INReservationAction : NSCopying, NSSecureCoding {

		[Export ("initWithType:validDuration:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INReservationActionType type, INDateComponentsRange validDuration, NSUserActivity userActivity);

		[Export ("type", ArgumentSemantic.Assign)]
		INReservationActionType Type { get; }

		[Export ("validDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange ValidDuration { get; }

		[Export ("userActivity", ArgumentSemantic.Copy)]
		NSUserActivity UserActivity { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INRestaurantReservation : NSCopying, NSSecureCoding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:reservationDuration:partySize:restaurantLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, INDateComponentsRange reservationDuration, [NullAllowed][BindAs (typeof (int?))] NSNumber partySize, CLPlacemark restaurantLocation);

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:reservationDuration:partySize:restaurantLocation:")]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, INDateComponentsRange reservationDuration, [NullAllowed][BindAs (typeof (int?))] NSNumber partySize, CLPlacemark restaurantLocation);

		[Export ("reservationDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange ReservationDuration { get; }

		[BindAs (typeof (int?))]
		[NullAllowed, Export ("partySize", ArgumentSemantic.Copy)]
		NSNumber PartySize { get; }

		[Export ("restaurantLocation", ArgumentSemantic.Copy)]
		CLPlacemark RestaurantLocation { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INSearchForMediaIntent {

		[Export ("initWithMediaItems:mediaSearch:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INMediaItem [] mediaItems, [NullAllowed] INMediaSearch mediaSearch);

		[NullAllowed, Export ("mediaItems", ArgumentSemantic.Copy)]
		INMediaItem [] MediaItems { get; }

		[NullAllowed, Export ("mediaSearch", ArgumentSemantic.Copy)]
		INMediaSearch MediaSearch { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSearchForMediaIntentHandling {

		[Abstract]
		[Export ("handleSearchForMedia:completion:")]
		void HandleSearch (INSearchForMediaIntent intent, Action<INSearchForMediaIntentResponse> completion);

		[Export ("confirmSearchForMedia:completion:")]
		void Confirm (INSearchForMediaIntent intent, Action<INSearchForMediaIntentResponse> completion);

		[Export ("resolveMediaItemsForSearchForMedia:withCompletion:")]
		void ResolveMediaItems (INSearchForMediaIntent intent, Action<INSearchForMediaMediaItemResolutionResult []> completion);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForMediaIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSearchForMediaIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForMediaIntentResponseCode Code { get; }

		[NullAllowed, Export ("mediaItems", ArgumentSemantic.Copy)]
		INMediaItem [] MediaItems { get; set; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSeat : NSCopying, NSSecureCoding {

		[Export ("initWithSeatSection:seatRow:seatNumber:seatingType:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string seatSection, [NullAllowed] string seatRow, [NullAllowed] string seatNumber, [NullAllowed] string seatingType);

		[NullAllowed, Export ("seatSection")]
		string SeatSection { get; }

		[NullAllowed, Export ("seatRow")]
		string SeatRow { get; }

		[NullAllowed, Export ("seatNumber")]
		string SeatNumber { get; }

		[NullAllowed, Export ("seatingType")]
		string SeatingType { get; }
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INTemporalEventTriggerResolutionResult))]
	[DisableDefaultCtor]
	interface INSetTaskAttributeTemporalEventTriggerResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult GetUnsupported (INSetTaskAttributeTemporalEventTriggerUnsupportedReason reason);

		[Export ("initWithTemporalEventTriggerResolutionResult:")]
		NativeHandle Constructor (INTemporalEventTriggerResolutionResult temporalEventTriggerResolutionResult);

		// Inlined from parent to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedTemporalEventTrigger:")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult GetSuccess (INTemporalEventTrigger resolvedTemporalEventTrigger);

		[New]
		[Static]
		[Export ("disambiguationWithTemporalEventTriggersToDisambiguate:")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult GetDisambiguation (INTemporalEventTrigger [] temporalEventTriggersToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithTemporalEventTriggerToConfirm:")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult GetConfirmationRequired ([NullAllowed] INTemporalEventTrigger temporalEventTriggerToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSetTaskAttributeTemporalEventTriggerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INSnoozeTasksIntent {

		[Export ("initWithTasks:nextTriggerTime:all:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INTask [] tasks, [NullAllowed] INDateComponentsRange nextTriggerTime, [NullAllowed][BindAs (typeof (bool?))] NSNumber all);

		[NullAllowed, Export ("tasks", ArgumentSemantic.Copy)]
		INTask [] Tasks { get; }

		[NullAllowed, Export ("nextTriggerTime", ArgumentSemantic.Copy)]
		INDateComponentsRange NextTriggerTime { get; }

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("all", ArgumentSemantic.Copy)]
		NSNumber All { get; }
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSnoozeTasksIntentHandling {

		[Abstract]
		[Export ("handleSnoozeTasks:completion:")]
		void HandleSnoozeTasks (INSnoozeTasksIntent intent, Action<INSnoozeTasksIntentResponse> completion);

		[Export ("confirmSnoozeTasks:completion:")]
		void Confirm (INSnoozeTasksIntent intent, Action<INSnoozeTasksIntentResponse> completion);

		[Export ("resolveTasksForSnoozeTasks:withCompletion:")]
		void ResolveTasks (INSnoozeTasksIntent intent, Action<INSnoozeTasksTaskResolutionResult []> completion);

		[Export ("resolveNextTriggerTimeForSnoozeTasks:withCompletion:")]
		void ResolveNextTriggerTime (INSnoozeTasksIntent intent, Action<INDateComponentsRangeResolutionResult> completion);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSnoozeTasksIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSnoozeTasksIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSnoozeTasksIntentResponseCode Code { get; }

		[NullAllowed, Export ("snoozedTasks", ArgumentSemantic.Copy)]
		INTask [] SnoozedTasks { get; set; }
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INTaskResolutionResult))]
	[DisableDefaultCtor]
	interface INSnoozeTasksTaskResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSnoozeTasksTaskResolutionResult GetUnsupported (INSnoozeTasksTaskUnsupportedReason reason);

		[Export ("initWithTaskResolutionResult:")]
		NativeHandle Constructor (INTaskResolutionResult taskResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedTask:")]
		INSnoozeTasksTaskResolutionResult GetSuccess (INTask resolvedTask);

		[New]
		[Static]
		[Export ("disambiguationWithTasksToDisambiguate:")]
		INSnoozeTasksTaskResolutionResult GetDisambiguation (INTask [] tasksToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithTaskToConfirm:")]
		INSnoozeTasksTaskResolutionResult GetConfirmationRequired ([NullAllowed] INTask taskToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSnoozeTasksTaskResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSnoozeTasksTaskResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSnoozeTasksTaskResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSnoozeTasksTaskResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSnoozeTasksTaskResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INSpeedResolutionResult {

		[Static]
		[Export ("successWithResolvedSpeed:")]
		INSpeedResolutionResult GetSuccess (NSMeasurement<NSUnitSpeed> resolvedSpeed);

		[Static]
		[Export ("disambiguationWithSpeedToDisambiguate:")]
		INSpeedResolutionResult GetDisambiguation (NSMeasurement<NSUnitSpeed> [] speedToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithSpeedToConfirm:")]
		INSpeedResolutionResult GetConfirmationRequired ([NullAllowed] NSMeasurement<NSUnitSpeed> speedToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSpeedResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSpeedResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSpeedResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSpeedResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSpeedResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INCallCapabilityResolutionResult))]
	[DisableDefaultCtor]
	interface INStartCallCallCapabilityResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INStartCallCallCapabilityResolutionResult GetUnsupported (INStartCallCallCapabilityUnsupportedReason reason);

		[Export ("initWithCallCapabilityResolutionResult:")]
		NativeHandle Constructor (INCallCapabilityResolutionResult callCapabilityResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedCallCapability:")]
		INStartCallCallCapabilityResolutionResult GetSuccess (INCallCapability resolvedCallCapability);

		[New]
		[Static]
		[Export ("confirmationRequiredWithCallCapabilityToConfirm:")]
		INStartCallCallCapabilityResolutionResult GetConfirmationRequired (INCallCapability callCapabilityToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INStartCallCallCapabilityResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INStartCallCallCapabilityResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INStartCallCallCapabilityResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INStartCallCallCapabilityResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INStartCallCallCapabilityResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INStartCallContactResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INStartCallContactResolutionResult GetUnsupported (INStartCallContactUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		NativeHandle Constructor (INPersonResolutionResult personResolutionResult);

		// Inlined from parent class to avoid bug 43205 scenario
		[New]
		[Static]
		[Export ("successWithResolvedPerson:")]
		INStartCallContactResolutionResult GetSuccess (INPerson resolvedPerson);

		[New]
		[Static]
		[Export ("disambiguationWithPeopleToDisambiguate:")]
		INStartCallContactResolutionResult GetDisambiguation (INPerson [] peopleToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithPersonToConfirm:")]
		INStartCallContactResolutionResult GetConfirmationRequired ([NullAllowed] INPerson personToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INStartCallContactResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INStartCallContactResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INStartCallContactResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INStartCallContactResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INStartCallContactResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INStartCallIntent : UNNotificationContentProviding {

		[Watch (7, 0), NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithCallRecordFilter:callRecordToCallBack:audioRoute:destinationType:contacts:callCapability:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INCallRecordFilter callRecordFilter, [NullAllowed] INCallRecord callRecordToCallBack, INCallAudioRoute audioRoute, INCallDestinationType destinationType, [NullAllowed] INPerson [] contacts, INCallCapability callCapability);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor (INCallRecordFilter, INCallRecord, INCallAudioRoute, INCallDestinationType, INPerson[], INCallCapability)' overload instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use '.ctor (INCallRecordFilter, INCallRecord, INCallAudioRoute, INCallDestinationType, INPerson[], INCallCapability)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor (INCallRecordFilter, INCallRecord, INCallAudioRoute, INCallDestinationType, INPerson[], INCallCapability)' overload instead.")]
		[Export ("initWithAudioRoute:destinationType:contacts:recordTypeForRedialing:callCapability:")]
		NativeHandle Constructor (INCallAudioRoute audioRoute, INCallDestinationType destinationType, [NullAllowed] INPerson [] contacts, INCallRecordType recordTypeForRedialing, INCallCapability callCapability);

		[Watch (7, 0), NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("callRecordFilter", ArgumentSemantic.Copy)]
		INCallRecordFilter CallRecordFilter { get; }

		[Watch (7, 0), NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("callRecordToCallBack", ArgumentSemantic.Copy)]
		INCallRecord CallRecordToCallBack { get; }

		[Export ("audioRoute", ArgumentSemantic.Assign)]
		INCallAudioRoute AudioRoute { get; }

		[Export ("destinationType", ArgumentSemantic.Assign)]
		INCallDestinationType DestinationType { get; }

		[NullAllowed, Export ("contacts", ArgumentSemantic.Copy)]
		INPerson [] Contacts { get; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.WatchOS, 7, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("recordTypeForRedialing", ArgumentSemantic.Assign)]
		INCallRecordType RecordTypeForRedialing { get; }

		[Export ("callCapability", ArgumentSemantic.Assign)]
		INCallCapability CallCapability { get; }
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INStartCallIntentHandling {

		[Abstract]
		[Export ("handleStartCall:completion:")]
		void HandleStartCall (INStartCallIntent intent, Action<INStartCallIntentResponse> completion);

		[Export ("confirmStartCall:completion:")]
		void Confirm (INStartCallIntent intent, Action<INStartCallIntentResponse> completion);

		[Watch (7, 0), NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("resolveCallRecordToCallBackForStartCall:withCompletion:")]
		void ResolveCallRecordToCallBack (INStartCallIntent intent, Action<INCallRecordResolutionResult> completion);

		[Export ("resolveDestinationTypeForStartCall:withCompletion:")]
		void ResolveDestinationType (INStartCallIntent intent, Action<INCallDestinationTypeResolutionResult> completion);

		[Export ("resolveContactsForStartCall:withCompletion:")]
		void ResolveContacts (INStartCallIntent intent, Action<NSArray<INStartCallContactResolutionResult>> completion);

		[Export ("resolveCallCapabilityForStartCall:withCompletion:")]
		void ResolveCallCapability (INStartCallIntent intent, Action<INStartCallCallCapabilityResolutionResult> completion);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INStartCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartCallIntentResponseCode Code { get; }
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTaskPriorityResolutionResult {

		[Static]
		[Export ("successWithResolvedTaskPriority:")]
		INTaskPriorityResolutionResult GetSuccess (INTaskPriority resolvedTaskPriority);

		[Static]
		[Export ("confirmationRequiredWithTaskPriorityToConfirm:")]
		INTaskPriorityResolutionResult ConfirmationRequired (INTaskPriority taskPriorityToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTaskPriorityResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTaskPriorityResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTaskPriorityResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskPriorityResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskPriorityResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTemporalEventTriggerTypeOptionsResolutionResult {

		[Static]
		[Export ("successWithResolvedTemporalEventTriggerTypeOptions:")]
		INTemporalEventTriggerTypeOptionsResolutionResult GetSuccess (INTemporalEventTriggerTypeOptions resolvedTemporalEventTriggerTypeOptions);

		[Static]
		[Export ("confirmationRequiredWithTemporalEventTriggerTypeOptionsToConfirm:")]
		INTemporalEventTriggerTypeOptionsResolutionResult ConfirmationRequired (INTemporalEventTriggerTypeOptions temporalEventTriggerTypeOptionsToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTemporalEventTriggerTypeOptionsResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTemporalEventTriggerTypeOptionsResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTemporalEventTriggerTypeOptionsResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTemporalEventTriggerTypeOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTemporalEventTriggerTypeOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTicketedEvent : NSCopying, NSSecureCoding {

		[Export ("initWithCategory:name:eventDuration:location:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INTicketedEventCategory category, string name, INDateComponentsRange eventDuration, [NullAllowed] CLPlacemark location);

		[Export ("category", ArgumentSemantic.Assign)]
		INTicketedEventCategory Category { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("eventDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange EventDuration { get; }

		[NullAllowed, Export ("location", ArgumentSemantic.Copy)]
		CLPlacemark Location { get; }
	}

	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INTicketedEventReservation : NSCopying, NSSecureCoding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:reservedSeat:event:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, [NullAllowed] INSeat reservedSeat, INTicketedEvent @event);

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:reservedSeat:event:")]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] INSeat reservedSeat, INTicketedEvent @event);

		[Export ("event", ArgumentSemantic.Copy)]
		INTicketedEvent Event { get; }

		[NullAllowed, Export ("reservedSeat", ArgumentSemantic.Copy)]
		INSeat ReservedSeat { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INTimeIntervalResolutionResult {

		[Static]
		[Export ("successWithResolvedTimeInterval:")]
		INTimeIntervalResolutionResult GetSuccess (double resolvedTimeInterval);

		[Static]
		[Export ("confirmationRequiredWithTimeIntervalToConfirm:")]
		INTimeIntervalResolutionResult ConfirmationRequired (double timeIntervalToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INTimeIntervalResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INTimeIntervalResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INTimeIntervalResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTimeIntervalResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTimeIntervalResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INTrainReservation : NSCopying, NSSecureCoding {

		[Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:reservedSeat:trainTrip:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, [NullAllowed] INSeat reservedSeat, INTrainTrip trainTrip);

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:reservedSeat:trainTrip:")]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] INSeat reservedSeat, INTrainTrip trainTrip);

		[NullAllowed, Export ("reservedSeat", ArgumentSemantic.Copy)]
		INSeat ReservedSeat { get; }

		[Export ("trainTrip", ArgumentSemantic.Copy)]
		INTrainTrip TrainTrip { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTrainTrip : NSCopying, NSSecureCoding {

		[Export ("initWithProvider:trainName:trainNumber:tripDuration:departureStationLocation:departurePlatform:arrivalStationLocation:arrivalPlatform:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string provider, [NullAllowed] string trainName, [NullAllowed] string trainNumber, INDateComponentsRange tripDuration, CLPlacemark departureStationLocation, [NullAllowed] string departurePlatform, CLPlacemark arrivalStationLocation, [NullAllowed] string arrivalPlatform);

		[NullAllowed, Export ("provider")]
		string Provider { get; }

		[NullAllowed, Export ("trainName")]
		string TrainName { get; }

		[NullAllowed, Export ("trainNumber")]
		string TrainNumber { get; }

		[Export ("tripDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange TripDuration { get; }

		[Export ("departureStationLocation", ArgumentSemantic.Copy)]
		CLPlacemark DepartureStationLocation { get; }

		[NullAllowed, Export ("departurePlatform")]
		string DeparturePlatform { get; }

		[Export ("arrivalStationLocation", ArgumentSemantic.Copy)]
		CLPlacemark ArrivalStationLocation { get; }

		[NullAllowed, Export ("arrivalPlatform")]
		string ArrivalPlatform { get; }
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult), Name = "INURLResolutionResult")]
	[DisableDefaultCtor]
	interface INUrlResolutionResult {

		[Static]
		[Export ("successWithResolvedURL:")]
		INUrlResolutionResult GetSuccess (NSUrl resolvedUrl);

		[Static]
		[Export ("disambiguationWithURLsToDisambiguate:")]
		INUrlResolutionResult GetDisambiguation (NSUrl [] urlsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithURLToConfirm:")]
		INUrlResolutionResult GetConfirmationRequired ([NullAllowed] NSUrl urlToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INUrlResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INUrlResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INUrlResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INUrlResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INUrlResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INUpdateMediaAffinityIntent {

		[Export ("initWithMediaItems:mediaSearch:affinityType:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INMediaItem [] mediaItems, [NullAllowed] INMediaSearch mediaSearch, INMediaAffinityType affinityType);

		[NullAllowed, Export ("mediaItems", ArgumentSemantic.Copy)]
		INMediaItem [] MediaItems { get; }

		[NullAllowed, Export ("mediaSearch", ArgumentSemantic.Copy)]
		INMediaSearch MediaSearch { get; }

		[Export ("affinityType", ArgumentSemantic.Assign)]
		INMediaAffinityType AffinityType { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INUpdateMediaAffinityIntentHandling {

		[Abstract]
		[Export ("handleUpdateMediaAffinity:completion:")]
		void HandleUpdateMediaAffinity (INUpdateMediaAffinityIntent intent, Action<INUpdateMediaAffinityIntentResponse> completion);

		[Export ("confirmUpdateMediaAffinity:completion:")]
		void Confirm (INUpdateMediaAffinityIntent intent, Action<INUpdateMediaAffinityIntentResponse> completion);

		[Export ("resolveMediaItemsForUpdateMediaAffinity:withCompletion:")]
		void ResolveMediaItems (INUpdateMediaAffinityIntent intent, Action<NSArray<INUpdateMediaAffinityMediaItemResolutionResult>> completion);

		[Export ("resolveAffinityTypeForUpdateMediaAffinity:withCompletion:")]
		void ResolveAffinityType (INUpdateMediaAffinityIntent intent, Action<INMediaAffinityTypeResolutionResult> completion);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INUpdateMediaAffinityIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INUpdateMediaAffinityIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INUpdateMediaAffinityIntentResponseCode Code { get; }
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INMediaItemResolutionResult))]
	[DisableDefaultCtor]
	interface INUpdateMediaAffinityMediaItemResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INUpdateMediaAffinityMediaItemResolutionResult GetUnsupported (INUpdateMediaAffinityMediaItemUnsupportedReason reason);

		[Export ("initWithMediaItemResolutionResult:")]
		NativeHandle Constructor (INMediaItemResolutionResult mediaItemResolutionResult);

		// Inlined from parent class to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedMediaItem:")]
		INUpdateMediaAffinityMediaItemResolutionResult GetSuccess (INMediaItem resolvedMediaItem);

		[New]
		[Static]
		[Export ("successesWithResolvedMediaItems:")]
		INUpdateMediaAffinityMediaItemResolutionResult [] GetSuccesses (INMediaItem [] resolvedMediaItems);

		[New]
		[Static]
		[Export ("disambiguationWithMediaItemsToDisambiguate:")]
		INUpdateMediaAffinityMediaItemResolutionResult GetDisambiguation (INMediaItem [] mediaItemsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithMediaItemToConfirm:")]
		INUpdateMediaAffinityMediaItemResolutionResult GetConfirmationRequired ([NullAllowed] INMediaItem mediaItemToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INUpdateMediaAffinityMediaItemResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INUpdateMediaAffinityMediaItemResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INUpdateMediaAffinityMediaItemResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INUpdateMediaAffinityMediaItemResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INUpdateMediaAffinityMediaItemResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INUserContext : NSSecureCoding {

		[Export ("becomeCurrent")]
		void BecomeCurrent ();
	}

	[Watch (6, 0), NoTV, Mac (11, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INVolumeResolutionResult {

		[Static]
		[Export ("successWithResolvedVolume:")]
		INVolumeResolutionResult GetSuccess (NSMeasurement<NSUnitVolume> resolvedVolume);

		[Static]
		[Export ("disambiguationWithVolumeToDisambiguate:")]
		INVolumeResolutionResult GetDisambiguation (NSMeasurement<NSUnitVolume> [] volumeToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithVolumeToConfirm:")]
		INVolumeResolutionResult GetConfirmationRequired ([NullAllowed] NSMeasurement<NSUnitVolume> volumeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INVolumeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INVolumeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INVolumeResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INVolumeResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INVolumeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INMediaDestinationResolutionResult))]
	[DisableDefaultCtor]
	interface INAddMediaMediaDestinationResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INAddMediaMediaDestinationResolutionResult GetUnsupported (INAddMediaMediaDestinationUnsupportedReason reason);

		[Export ("initWithMediaDestinationResolutionResult:")]
		NativeHandle Constructor (INMediaDestinationResolutionResult mediaDestinationResolutionResult);

		// Inlined from parent class to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedMediaDestination:")]
		INAddMediaMediaDestinationResolutionResult GetSuccess (INMediaDestination resolvedMediaDestination);

		[New]
		[Static]
		[Export ("disambiguationWithMediaDestinationsToDisambiguate:")]
		INAddMediaMediaDestinationResolutionResult GetDisambiguation (INMediaDestination [] mediaDestinationsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithMediaDestinationToConfirm:")]
		INAddMediaMediaDestinationResolutionResult GetConfirmationRequired ([NullAllowed] INMediaDestination mediaDestinationToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INAddMediaMediaDestinationResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INAddMediaMediaDestinationResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INAddMediaMediaDestinationResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INAddMediaMediaDestinationResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INAddMediaMediaDestinationResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (6, 0), TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INMediaItemResolutionResult))]
	interface INSearchForMediaMediaItemResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSearchForMediaMediaItemResolutionResult GetUnsupported (INSearchForMediaMediaItemUnsupportedReason reason);

		[Export ("initWithMediaItemResolutionResult:")]
		NativeHandle Constructor (INMediaItemResolutionResult mediaItemResolutionResult);

		// Inlined from parent class to avoid bug like 43205

		[New]
		[Static]
		[Export ("successWithResolvedMediaItem:")]
		INSearchForMediaMediaItemResolutionResult GetSuccess (INMediaItem resolvedMediaItem);

		[New]
		[Static]
		[Export ("successesWithResolvedMediaItems:")]
		INSearchForMediaMediaItemResolutionResult [] GetSuccesses (INMediaItem [] resolvedMediaItems);

		[New]
		[Static]
		[Export ("disambiguationWithMediaItemsToDisambiguate:")]
		INSearchForMediaMediaItemResolutionResult GetDisambiguation (INMediaItem [] mediaItemsToDisambiguate);

		[New]
		[Static]
		[Export ("confirmationRequiredWithMediaItemToConfirm:")]
		INSearchForMediaMediaItemResolutionResult GetConfirmationRequired ([NullAllowed] INMediaItem mediaItemToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INSearchForMediaMediaItemResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INSearchForMediaMediaItemResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INSearchForMediaMediaItemResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSearchForMediaMediaItemResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSearchForMediaMediaItemResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}



	[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_ShareExtension {

		[return: NullAllowed]
		[Export ("intent")]
		INIntent GetIntent ();
	}

	[Watch (7, 0), NoTV, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INBoatReservation : NSCopying, NSSecureCoding {

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:reservedSeat:boatTrip:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, [NullAllowed] INSeat reservedSeat, [NullAllowed] INBoatTrip boatTrip);

		[NullAllowed, Export ("reservedSeat", ArgumentSemantic.Copy)]
		INSeat ReservedSeat { get; }

		[NullAllowed, Export ("boatTrip", ArgumentSemantic.Copy)]
		INBoatTrip BoatTrip { get; }
	}

	[Watch (7, 0), NoTV, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INBoatTrip : NSCopying, NSSecureCoding {

		[Export ("initWithProvider:boatName:boatNumber:tripDuration:departureBoatTerminalLocation:arrivalBoatTerminalLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string provider, [NullAllowed] string boatName, [NullAllowed] string boatNumber, INDateComponentsRange tripDuration, CLPlacemark departureBoatTerminalLocation, CLPlacemark arrivalBoatTerminalLocation);

		[NullAllowed, Export ("provider")]
		string Provider { get; }

		[NullAllowed, Export ("boatName")]
		string BoatName { get; }

		[NullAllowed, Export ("boatNumber")]
		string BoatNumber { get; }

		[Export ("tripDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange TripDuration { get; }

		[Export ("departureBoatTerminalLocation", ArgumentSemantic.Copy)]
		CLPlacemark DepartureBoatTerminalLocation { get; }

		[Export ("arrivalBoatTerminalLocation", ArgumentSemantic.Copy)]
		CLPlacemark ArrivalBoatTerminalLocation { get; }
	}

	[Watch (7, 0), NoTV, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INBusReservation : NSCopying, NSSecureCoding {

		[Export ("initWithItemReference:reservationNumber:bookingTime:reservationStatus:reservationHolderName:actions:URL:reservedSeat:busTrip:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSpeakableString itemReference, [NullAllowed] string reservationNumber, [NullAllowed] NSDate bookingTime, INReservationStatus reservationStatus, [NullAllowed] string reservationHolderName, [NullAllowed] INReservationAction [] actions, [NullAllowed] NSUrl url, [NullAllowed] INSeat reservedSeat, [NullAllowed] INBusTrip busTrip);

		[NullAllowed, Export ("reservedSeat", ArgumentSemantic.Copy)]
		INSeat ReservedSeat { get; }

		[Export ("busTrip", ArgumentSemantic.Copy)]
		INBusTrip BusTrip { get; }
	}

	[Watch (7, 0), NoTV, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INBusTrip : NSCopying, NSSecureCoding {

		[Export ("initWithProvider:busName:busNumber:tripDuration:departureBusStopLocation:departurePlatform:arrivalBusStopLocation:arrivalPlatform:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string provider, [NullAllowed] string busName, [NullAllowed] string busNumber, INDateComponentsRange tripDuration, CLPlacemark departureBusStopLocation, [NullAllowed] string departurePlatform, CLPlacemark arrivalBusStopLocation, [NullAllowed] string arrivalPlatform);

		[NullAllowed, Export ("provider")]
		string Provider { get; }

		[NullAllowed, Export ("busName")]
		string BusName { get; }

		[NullAllowed, Export ("busNumber")]
		string BusNumber { get; }

		[Export ("tripDuration", ArgumentSemantic.Copy)]
		INDateComponentsRange TripDuration { get; }

		[Export ("departureBusStopLocation", ArgumentSemantic.Copy)]
		CLPlacemark DepartureBusStopLocation { get; }

		[NullAllowed, Export ("departurePlatform")]
		string DeparturePlatform { get; }

		[Export ("arrivalBusStopLocation", ArgumentSemantic.Copy)]
		CLPlacemark ArrivalBusStopLocation { get; }

		[NullAllowed, Export ("arrivalPlatform")]
		string ArrivalPlatform { get; }
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCallRecordFilter : NSCopying, NSSecureCoding {

		[Export ("initWithParticipants:callTypes:callCapability:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson [] participants, INCallRecordTypeOptions callTypes, INCallCapability callCapability);

		[NullAllowed, Export ("participants", ArgumentSemantic.Copy)]
		INPerson [] Participants { get; }

		[Export ("callTypes", ArgumentSemantic.Assign)]
		INCallRecordTypeOptions CallTypes { get; }

		[Export ("callCapability", ArgumentSemantic.Assign)]
		INCallCapability CallCapability { get; }
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INCallRecordResolutionResult {

		[Static]
		[Export ("successWithResolvedCallRecord:")]
		INCallRecordResolutionResult GetSuccess (INCallRecord resolvedCallRecord);

		[Static]
		[Export ("disambiguationWithCallRecordsToDisambiguate:")]
		INCallRecordResolutionResult GetDisambiguation (INCallRecord [] callRecordsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithCallRecordToConfirm:")]
		INCallRecordResolutionResult GetConfirmationRequired ([NullAllowed] INCallRecord callRecordToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INCallRecordResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INCallRecordResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INCallRecordResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallRecordResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallRecordResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCar : NSCopying, NSSecureCoding {

		[Export ("initWithCarIdentifier:displayName:year:make:model:color:headUnit:supportedChargingConnectors:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string carIdentifier, [NullAllowed] string displayName, [NullAllowed] string year, [NullAllowed] string make, [NullAllowed] string model, [NullAllowed] CGColor color, [NullAllowed] INCarHeadUnit headUnit, [BindAs (typeof (INCarChargingConnectorType []))] NSString [] supportedChargingConnectors);

		[Export ("carIdentifier")]
		string CarIdentifier { get; }

		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed, Export ("year")]
		string Year { get; }

		[NullAllowed, Export ("make")]
		string Make { get; }

		[NullAllowed, Export ("model")]
		string Model { get; }

		[NullAllowed, Export ("color")]
		CGColor Color { get; }

		[NullAllowed, Export ("headUnit", ArgumentSemantic.Copy)]
		INCarHeadUnit HeadUnit { get; }

		[BindAs (typeof (INCarChargingConnectorType []))]
		[Export ("supportedChargingConnectors", ArgumentSemantic.Copy)]
		NSString [] SupportedChargingConnectors { get; }

		[Export ("setMaximumPower:forChargingConnectorType:")]
		void SetMaximumPower (NSMeasurement<NSUnitPower> power, [BindAs (typeof (INCarChargingConnectorType))] NSString chargingConnectorType);

		[Export ("maximumPowerForChargingConnectorType:")]
		[return: NullAllowed]
		NSMeasurement<NSUnitPower> GetMaximumPower ([BindAs (typeof (INCarChargingConnectorType))] NSString chargingConnectorType);
	}

	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCarHeadUnit : NSCopying, NSSecureCoding {

		[Export ("initWithBluetoothIdentifier:iAP2Identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string bluetoothIdentifier, [NullAllowed] string iAP2Identifier);

		[NullAllowed, Export ("bluetoothIdentifier")]
		string BluetoothIdentifier { get; }

		[NullAllowed, Export ("iAP2Identifier")]
		string Iap2Identifier { get; }
	}

	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INIntent))]
	[DesignatedDefaultCtor]
	interface INListCarsIntent {

	}

	interface IINListCarsIntentHandling { }

	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface INListCarsIntentHandling {

		[Abstract]
		[Export ("handleListCars:completion:")]
		void HandleListCars (INListCarsIntent intent, Action<INListCarsIntentResponse> completion);

		[Export ("confirmListCars:completion:")]
		void ConfirmListCars (INListCarsIntent intent, Action<INListCarsIntentResponse> completion);
	}

	[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INListCarsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INListCarsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INListCarsIntentResponseCode Code { get; }

		[NullAllowed, Export ("cars", ArgumentSemantic.Copy)]
		INCar [] Cars { get; set; }
	}

	[Watch (7, 0), NoTV, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INObjectCollection<ObjectType> : NSCopying, NSSecureCoding
		where ObjectType : NSObject {

		[Export ("sections", ArgumentSemantic.Copy)]
		INObjectSection<ObjectType> [] Sections { get; }

		[Export ("allItems", ArgumentSemantic.Copy)]
		ObjectType [] AllItems { get; }

		[Export ("usesIndexedCollation")]
		bool UsesIndexedCollation { get; set; }

		[Export ("initWithSections:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INObjectSection<ObjectType> [] sections);

		[Export ("initWithItems:")]
		NativeHandle Constructor (ObjectType [] items);
	}

	[Watch (7, 0), NoTV, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INObjectSection<ObjectType> : NSCopying, NSSecureCoding
		where ObjectType : NSObject {

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("items", ArgumentSemantic.Copy)]
		ObjectType [] Items { get; }

		[Export ("initWithTitle:items:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, ObjectType [] items);
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INOutgoingMessageTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedOutgoingMessageType:")]
		INOutgoingMessageTypeResolutionResult GetSuccess (INOutgoingMessageType resolvedOutgoingMessageType);

		[Static]
		[Export ("confirmationRequiredWithOutgoingMessageTypeToConfirm:")]
		INOutgoingMessageTypeResolutionResult GetConfirmationRequired (INOutgoingMessageType outgoingMessageTypeToConfirm);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INOutgoingMessageTypeResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INOutgoingMessageTypeResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INOutgoingMessageTypeResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INOutgoingMessageTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INOutgoingMessageTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSendMessageAttachment {

		[Static]
		[Export ("attachmentWithAudioMessageFile:")]
		INSendMessageAttachment Create (INFile audioMessageFile);

		[NullAllowed, Export ("audioMessageFile", ArgumentSemantic.Copy)]
		INFile AudioMessageFile { get; }
	}

	[Watch (7, 0), NoTV, Mac (12, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INCallRecordResolutionResult))]
	[DisableDefaultCtor]
	interface INStartCallCallRecordToCallBackResolutionResult {

		[Static]
		[Export ("successWithResolvedCallRecord:")]
		INStartCallCallRecordToCallBackResolutionResult GetSuccess (INCallRecord resolvedCallRecord);

		[Static]
		[Export ("disambiguationWithCallRecordsToDisambiguate:")]
		INStartCallCallRecordToCallBackResolutionResult GetDisambiguation (INCallRecord [] callRecordsToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithCallRecordToConfirm:")]
		INStartCallCallRecordToCallBackResolutionResult GetConfirmationRequired ([NullAllowed] INCallRecord callRecordToConfirm);

		[Static]
		[Export ("unsupportedForReason:")]
		INStartCallCallRecordToCallBackResolutionResult GetUnsupported (INStartCallCallRecordToCallBackUnsupportedReason reason);

		[Export ("initWithCallRecordResolutionResult:")]
		NativeHandle Constructor (INCallRecordResolutionResult callRecordResolutionResult);

		// Fixes bug 43205. We need to return the inherited type not the base type
		// because users won't be able to downcast easily

		[New]
		[Static]
		[Export ("needsValue")]
		INStartCallCallRecordToCallBackResolutionResult NeedsValue { get; }

		[New]
		[Static]
		[Export ("notRequired")]
		INStartCallCallRecordToCallBackResolutionResult NotRequired { get; }

		[New]
		[Static]
		[Export ("unsupported")]
		INStartCallCallRecordToCallBackResolutionResult Unsupported { get; }

		[New]
		[Static]
		[Export ("unsupportedWithReason:")]
		INStartCallCallRecordToCallBackResolutionResult GetUnsupported (nint reason);

		[New]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INStartCallCallRecordToCallBackResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Watch (7, 4), NoTV, Mac (11, 3), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCallGroup : NSCopying, NSSecureCoding {

		[Export ("initWithGroupName:groupId:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string groupName, [NullAllowed] string groupId);

		[NullAllowed, Export ("groupName")]
		string GroupName { get; }

		[NullAllowed, Export ("groupId")]
		string GroupId { get; }
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[BaseType (typeof (INIntent))]
	interface INAnswerCallIntent {

		[Export ("initWithAudioRoute:callIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCallAudioRoute audioRoute, [NullAllowed] string callIdentifier);

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("audioRoute", ArgumentSemantic.Assign)]
		INCallAudioRoute AudioRoute { get; }

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed, Export ("callIdentifier")]
		string CallIdentifier { get; }
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[Protocol]
	interface INAnswerCallIntentHandling {

		[Abstract]
		[Export ("handleAnswerCall:completion:")]
		void HandleAnswerCall (INAnswerCallIntent intent, Action<INAnswerCallIntentResponse> completion);

		[Export ("confirmAnswerCall:completion:")]
		void ConfirmAnswerCall (INAnswerCallIntent intent, Action<INAnswerCallIntentResponse> completion);
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INAnswerCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INAnswerCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INAnswerCallIntentResponseCode Code { get; }

		[Mac (13, 3)]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("callRecords", ArgumentSemantic.Copy)]
		INCallRecord [] CallRecords { get; set; }
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[BaseType (typeof (INIntent))]
	interface INHangUpCallIntent {

		[Mac (13, 1), iOS (16, 2)]
		[MacCatalyst (16, 2)]
		[Export ("initWithCallIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] string callIdentifier);

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed, Export ("callIdentifier")]
		string CallIdentifier { get; }
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[Protocol]
	interface INHangUpCallIntentHandling {

		[Abstract]
		[Export ("handleHangUpCall:completion:")]
		void HandleHangUpCall (INHangUpCallIntent intent, Action<INHangUpCallIntentResponse> completion);

		[Export ("confirmHangUpCall:completion:")]
		void ConfirmHangUpCall (INHangUpCallIntent intent, Action<INHangUpCallIntentResponse> completion);
	}

	[Watch (9, 4), NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INHangUpCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INHangUpCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INHangUpCallIntentResponseCode Code { get; }
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INUnsendMessagesIntentResponse {
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INUnsendMessagesIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INUnsendMessagesIntentResponseCode Code { get; }
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntent))]
	interface INUnsendMessagesIntent {
		[Export ("initWithMessageIdentifiers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string [] messageIdentifiers);

		[NullAllowed, Export ("messageIdentifiers", ArgumentSemantic.Copy)]
		string [] MessageIdentifiers { get; }
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface INMessageLinkMetadata : NSCopying, NSSecureCoding {
		[Export ("initWithSiteName:summary:title:openGraphType:linkURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string siteName, [NullAllowed] string summary, [NullAllowed] string title, [NullAllowed] string openGraphType, [NullAllowed] NSUrl linkUrl);

		[NullAllowed, Export ("siteName")]
		string SiteName { get; set; }

		[NullAllowed, Export ("summary")]
		string Summary { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("openGraphType")]
		string OpenGraphType { get; set; }

		[NullAllowed, Export ("linkURL", ArgumentSemantic.Copy)]
		NSUrl LinkUrl { get; set; }
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INEditMessageIntentResponse {
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INEditMessageIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INEditMessageIntentResponseCode Code { get; }
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntent))]
	interface INEditMessageIntent {
		[Export ("initWithMessageIdentifier:editedContent:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string messageIdentifier, [NullAllowed] string editedContent);

		[NullAllowed, Export ("messageIdentifier")]
		string MessageIdentifier { get; }

		[NullAllowed, Export ("editedContent")]
		string EditedContent { get; }
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface INEditMessageIntentHandling {
		[Abstract]
		[Export ("handleEditMessage:completion:")]
		void HandleEditMessage (INEditMessageIntent intent, Action<INEditMessageIntentResponse> completion);

		[Export ("confirmEditMessage:completion:")]
		void ConfirmEditMessage (INEditMessageIntent intent, Action<INEditMessageIntentResponse> completion);

		[Export ("resolveEditedContentForEditMessage:withCompletion:")]
		void ResolveEditedContent (INEditMessageIntent intent, Action<INStringResolutionResult> completion);
	}

	[Watch (10, 0), NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface INUnsendMessagesIntentHandling {
		[Abstract]
		[Export ("handleUnsendMessages:completion:")]
		void HandleUnsendMessages (INUnsendMessagesIntent intent, Action<INUnsendMessagesIntentResponse> completion);

		[Export ("confirmUnsendMessages:completion:")]
		void ConfirmUnsendMessages (INUnsendMessagesIntent intent, Action<INUnsendMessagesIntentResponse> completion);
	}

}
