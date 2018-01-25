//
// Intents bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 // The Intents framework uses generics which is only supported in Unified

using System;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreLocation;
using XamCore.Contacts;
using XamCore.EventKit;

#if MONOMAC
using UIImage = XamCore.Foundation.NSObject;
#else
using XamCore.UIKit;
#endif

namespace XamCore.Intents {

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INBookRestaurantReservationIntentCode : nint {
		Success = 0,
		Denied,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	[Flags]
	public enum INCallCapabilityOptions : nuint {
		AudioCall = (1 << 0),
		VideoCall = (1 << 1)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INCallRecordType : nint {
		Unknown = 0,
		Outgoing,
		Missed,
		Received,
		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		Latest,
		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		Voicemail,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INCancelWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message:"Use 'HandleInApp' instead.")] // yup just iOS
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[NoWatch, iOS (11,0)]
		Success,
		[NoWatch, iOS (11,0)]
		HandleInApp,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarAirCirculationMode bound
	[Native]
	public enum INCarAirCirculationMode : nint {
		Unknown = 0,
		FreshAir,
		RecirculateAir
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarAudioSource bound
	[Native]
	public enum INCarAudioSource : nint {
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarDefroster bound
	[Native]
	public enum INCarDefroster : nint {
		Unknown = 0,
		Front,
		Rear,
		All,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INCarSeat bound
	[Native]
	public enum INCarSeat : nint {
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INConditionalOperator : nint {
		All = 0,
		Any,
		None
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INEndWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message:"Use 'HandleInApp' instead.")] // yup just iOS
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[NoWatch, iOS (11,0)]
		Success,
		[NoWatch, iOS (11,0)]
		HandleInApp,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode : nint {
		Success,
		Failure,
		Unspecified
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetAvailableRestaurantReservationBookingsIntentCode : nint {
		Success,
		Failure,
		FailureRequestUnsatisfiable,
		FailureRequestUnspecified
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetRestaurantGuestIntentResponseCode : nint {
		Success,
		Failure
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetRideStatusIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetUserCurrentRestaurantReservationBookingsIntentResponseCode : nint {
		Success,
		Failure,
		FailureRequestUnsatisfiable,
		Unspecified
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	[ErrorDomain ("INIntentErrorDomain")]
	public enum INIntentErrorCode : nint {
		InteractionOperationNotSupported = 1900,
		DonatingInteraction = 1901,
		DeletingAllInteractions = 1902,
		DeletingInteractionWithIdentifiers = 1903,
		DeletingInteractionWithGroupIdentifier = 1904,
		IntentSupportedByMultipleExtension = 2001,
		RestrictedIntentsNotSupportedByExtension = 2002,
		NoHandlerProvidedForIntent = 2003,
		InvalidIntentName = 2004,
		RequestTimedOut = 3001,
		InvalidUserVocabularyFileLocation = 4000,
		ExtensionLaunchingTimeout = 5000,
		ExtensionBringUpFailed = 5001,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INIntentHandlingStatus : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		DeferredToApplication
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INInteractionDirection : nint {
		Unspecified = 0,
		Outgoing,
		Incoming
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INListRideOptionsIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchNoServiceInArea,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable,
		FailureRequiringAppLaunchPreviousRideNeedsCompletion,
		[iOS (11,0), Watch (4,0)]
		FailurePreviousRideNeedsFeedback,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INMessageAttribute : nint {
		Unknown = 0,
		Read,
		Unread,
		Flagged,
		Unflagged,
		[iOS (11,0), Mac (10,13, onlyOn64:true), Watch (4,0)]
		Played,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	[Flags]
	public enum INMessageAttributeOptions : nuint {
		Read = (1 << 0),
		Unread = (1 << 1),
		Flagged = (1 << 2),
		Unflagged = (1 << 3),
		[iOS (11,0), Mac (10,13, onlyOn64:true), Watch (4,0)]
		Played = (1UL << 4),
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INPauseWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HandleInApp' instead.")] // yup just iOS
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[NoWatch, iOS (11,0)]
		Success,
		[NoWatch, iOS (11,0)]
		HandleInApp,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INPaymentMethodType : nint {
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INPaymentStatus : nint {
		Unknown = 0,
		Pending,
		Completed,
		Canceled,
		Failed,
		Unpaid
	}

	[Native]
	public enum INPersonSuggestionType : nint {
		SocialProfile = 1,
		InstantMessageAddress
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INPhotoAttributeOptions bound
	[Native]
	[Flags]
	public enum INPhotoAttributeOptions : nuint {
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
		ProcessFilter = (1 << 23)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INRadioType bound
	[Native]
	public enum INRadioType : nint {
		Unknown = 0,
		AM,
		FM,
		HD,
		Satellite,
		Dab
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRelativeReference : nint {
		Unknown = 0,
		Next,
		Previous
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRelativeSetting : nint {
		Unknown = 0,
		Lowest,
		Lower,
		Higher,
		Highest
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRequestPaymentIntentResponseCode : nint {
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
		[iOS (11,0), Watch (4,0)]
		FailureNotEligible,
		[iOS (11,1), Watch (4,1)]
		FailureTermsAndConditionsAcceptanceRequired,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRequestRideIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchNoServiceInArea,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable,
		FailureRequiringAppLaunchPreviousRideNeedsCompletion
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRestaurantReservationUserBookingStatus : nuint {
		Pending,
		Confirmed,
		Denied
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INResumeWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HandleInApp' instead.")] // yup just iOS
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout,
		[NoWatch, iOS (11,0)]
		Success,
		[NoWatch, iOS (11,0)]
		HandleInApp,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRidePhase : nint {
		Unknown = 0,
		Received,
		Confirmed,
		Ongoing,
		Completed,
		ApproachingPickup,
		Pickup
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSaveProfileInCarIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INSearchCallHistoryIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		FailureAppConfigurationRequired,
		[iOS (11,0), Mac (10,13, onlyOn64:true), Watch (4,0)]
		InProgress,
		[iOS (11,0), Mac (10,13, onlyOn64:true), Watch (4,0)]
		Success,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INSearchForMessagesIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageServiceNotAvailable,
		[iOS (11,0), Mac (10,13, onlyOn64:true), Watch (4,0)]
		FailureMessageTooManyResults,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSearchForPhotosIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		FailureAppConfigurationRequired,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INSendMessageIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageServiceNotAvailable
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSendPaymentIntentResponseCode : nint {
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
		[Introduced (PlatformName.iOS, 11, 0)]
		[Introduced (PlatformName.MacOSX, 10, 13, PlatformArchitecture.Arch64)]
		[Introduced (PlatformName.WatchOS, 4, 0)]
		FailureNotEligible,
		[iOS (11,1), Watch (4,1)]
		FailureTermsAndConditionsAcceptanceRequired,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetAudioSourceInCarIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetClimateSettingsInCarIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetDefrosterSettingsInCarIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetMessageAttributeIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureMessageNotFound,
		FailureMessageAttributeNotSet
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetProfileInCarIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetRadioStationIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureNotSubscribed
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INSetSeatSettingsInCarIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSiriAuthorizationStatus : nint {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INStartAudioCallIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		FailureAppConfigurationRequired,
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		FailureCallingServiceNotAvailable,
		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		FailureContactNotSupportedByApp,
		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		FailureNoValidNumber,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INStartPhotoPlaybackIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[Introduced (PlatformName.iOS, 10, 2)]
		FailureAppConfigurationRequired,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INStartVideoCallIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		FailureAppConfigurationRequired,
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		FailureCallingServiceNotAvailable,
		[Watch (4,0), iOS (11,0)]
		FailureContactNotSupportedByApp,
		[Watch (4,0), iOS (11,0)]
		FailureInvalidNumber,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INStartWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureOngoingWorkout,
		FailureNoMatchingWorkout,
		[Watch (4,0), iOS (11,0)]
		Success,
		[Watch (4,0), iOS (11,0)]
		HandleInApp,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Native]
	public enum INVocabularyStringType : nint {
		ContactName = 1,
		ContactGroupName,
		PhotoTag = 100,
		PhotoAlbumName,
		WorkoutActivityName = 200,
		CarProfileName = 300,
		[Introduced (PlatformName.iOS, 10, 3)]
		CarName,
		[Introduced (PlatformName.iOS, 10, 3)]
		PaymentsOrganizationName = 400,
		[Introduced (PlatformName.iOS, 10, 3)]
		PaymentsAccountNickname,
		[iOS (11,0)]
		NotebookItemTitle = 500,
		[iOS (11,0)]
		NotebookItemGroupName,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INWorkoutGoalUnitType bound
	[Native]
	public enum INWorkoutGoalUnitType : nint {
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INWorkoutLocationType bound
	[Native]
	public enum INWorkoutLocationType : nint {
		Unknown = 0,
		Outdoor,
		Indoor
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INPersonHandleType : nint {
		Unknown = 0,
		EmailAddress,
		PhoneNumber
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INAccountType : nint {
		Unknown = 0,
		Checking,
		Credit,
		Debit,
		Investment,
		Mortgage,
		Prepaid,
		Saving,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INActivateCarSignalIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INAmountType : nint {
		Unknown = 0,
		MinimumDue,
		AmountDue,
		CurrentBalance,
		[iOS (11,0), Watch (4,0)]
		MaximumTransferAmount,
		[iOS (11,0), Watch (4,0)]
		MinimumTransferAmount,
		[iOS (11,0), Watch (4,0)]
		StatementBalance,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INBillType : nint {
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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	[Flags]
	public enum INCarSignalOptions : nuint {
		Audible = (1 << 0),
		Visible = (1 << 1),
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetCarLockStatusIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INGetCarPowerLevelStatusIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INPayBillIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureInsufficientFunds,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSearchForBillsIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureBillNotFound,
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSetCarLockStatusIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INAddTasksIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INAppendToNoteIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCannotUpdatePasswordProtectedNote,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INBalanceType : nint {
		Unknown = 0,
		Money,
		Points,
		Miles,
	}

	[Watch (3,2), Mac (10,12, onlyOn64:true), iOS (10,0)]
	[Native]
	public enum INCallCapability : nint {
		Unknown = 0,
		AudioCall,
		VideoCall,
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[Native]
	public enum INCallDestinationType : nint {
		Unknown = 0,
		Normal,
		Emergency,
		Voicemail,
		Redial,
	}

	[Watch (3,2), Mac (10,12, onlyOn64:true), iOS (10,0)]
	[Native]
	[Flags]
	public enum INCallRecordTypeOptions : nuint {
		Outgoing = (1 << 0),
		Missed = (1 << 1),
		Received = (1 << 2),
		Latest = (1 << 3),
		Voicemail = (1 << 4),
	}

	[NoWatch, NoMac, iOS (11,0)]
	[Native]
	public enum INCancelRideIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		Success,
		Failure,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INCreateNoteIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INCreateTaskListIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INDateSearchType : nint {
		Unknown = 0,
		ByDueDate,
		ByModifiedDate,
		ByCreatedDate,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INGetVisualCodeIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureAppConfigurationRequired,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INLocationSearchType : nint {
		Unknown = 0,
		ByLocationTrigger,
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[Native]
	public enum INMessageType : nint {
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INNoteContentType : nint {
		Unknown = 0,
		Text,
		Image,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INNotebookItemType : nint {
		Unknown = 0,
		Note,
		TaskList,
		Task,
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[Native]
	public enum INRecurrenceFrequency : nint {
		Unknown = 0,
		Minute,
		Hourly,
		Daily,
		Weekly,
		Monthly,
		Yearly,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INRequestPaymentCurrencyAmountUnsupportedReason : nint {
		AmountBelowMinimum = 1,
		AmountAboveMaximum,
		CurrencyUnsupported,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INRequestPaymentPayerUnsupportedReason : nint {
		CredentialsUnverified = 1,
		NoAccount,
		[Watch (4,1), iOS (11,1)]
		NoValidHandle,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INRideFeedbackTypeOptions : nuint {
		Rate = (1 << 0),
		Tip = (1 << 1),
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSearchForAccountsIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureAccountNotFound,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSearchForNotebookItemsIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[Native]
	public enum INSendMessageRecipientUnsupportedReason : nint {
		NoAccount = 1,
		Offline,
		MessagingServiceNotEnabledForRecipient,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSendPaymentCurrencyAmountUnsupportedReason : nint {
		AmountBelowMinimum = 1,
		AmountAboveMaximum,
		CurrencyUnsupported,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSendPaymentPayeeUnsupportedReason : nint {
		CredentialsUnverified = 1,
		InsufficientFunds,
		NoAccount,
		[Watch (4,1), iOS (11,1)]
		NoValidHandle,
	}

	[NoWatch, NoMac, iOS (11,0)]
	[Native]
	public enum INSendRideFeedbackIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		Success,
		Failure,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSetTaskAttributeIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSortType : nint {
		Unknown = 0,
		AsIs,
		ByDate,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INSpatialEvent : nint {
		Unknown = 0,
		Arrive,
		Depart,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INTaskStatus : nint {
		Unknown = 0,
		NotCompleted,
		Completed,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INTaskType : nint {
		Unknown = 0,
		NotCompletable,
		Completable,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INTransferMoneyIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureCredentialsUnverified,
		FailureInsufficientFunds,
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Native]
	public enum INVisualCodeType : nint {
		Unknown = 0,
		Contact,
		RequestPayment,
		SendPayment,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	public enum INIntentIdentifier {
		[Unavailable (PlatformName.MacOSX)]
		[Field ("INStartAudioCallIntentIdentifier")]
		StartAudioCall,

		[Unavailable (PlatformName.MacOSX)]
		[Unavailable (PlatformName.WatchOS)]
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
		GetRideStatus
	}

	[Introduced (PlatformName.iOS, 10, 2)]
	[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 2)]
	[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	enum INPersonRelationship {
		[Field (null)]
		None,

		[Field ("INPersonRelationshipFather")]
		Father,

		[Field ("INPersonRelationshipMother")]
		Mother,

		[Field ("INPersonRelationshipParent")]
		Parent,

		[Field ("INPersonRelationshipBrother")]
		Brother,

		[Field ("INPersonRelationshipSister")]
		Sister,

		[Field ("INPersonRelationshipChild")]
		Child,

		[Field ("INPersonRelationshipFriend")]
		Friend,

		[Field ("INPersonRelationshipSpouse")]
		Spouse,

		[Field ("INPersonRelationshipPartner")]
		Partner,

		[Field ("INPersonRelationshipAssistant")]
		Assistant,

		[Field ("INPersonRelationshipManager")]
		Manager,
	}

	[Introduced (PlatformName.iOS, 10, 2)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	// End of enums

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Internal]
	[Category]
	[BaseType (typeof (CLPlacemark))]
	interface CLPlacemark_INIntentsAdditions {

		[Static]
		[Export ("placemarkWithLocation:name:postalAddress:")]
		CLPlacemark _GetPlacemark (CLLocation location, [NullAllowed] string name, [NullAllowed] CNPostalAddress postalAddress);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INBookRestaurantReservationIntent : NSCopying {

		[iOS (11,0)]
		[Export ("initWithRestaurant:bookingDateComponents:partySize:bookingIdentifier:guest:selectedOffer:guestProvidedSpecialRequestText:")]
		IntPtr Constructor (INRestaurant restaurant, NSDateComponents bookingDateComponents, nuint partySize, [NullAllowed] string bookingIdentifier, [NullAllowed] INRestaurantGuest guest, [NullAllowed] INRestaurantOffer selectedOffer, [NullAllowed] string guestProvidedSpecialRequestText);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INBookRestaurantReservationIntentHandling {

		[Abstract]
		[Export ("handleBookRestaurantReservation:completion:")]
		void HandleBookRestaurantReservation (INBookRestaurantReservationIntent intent, Action<INBookRestaurantReservationIntentResponse> completion);

		[Export ("confirmBookRestaurantReservation:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	interface INBookRestaurantReservationIntentResponse {

		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INBookRestaurantReservationIntentCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INBookRestaurantReservationIntentCode Code { get; }

		[NullAllowed, Export ("userBooking", ArgumentSemantic.Copy)]
		INRestaurantReservationUserBooking UserBooking { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallRecordTypeResolutionResult {

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCallRecordType:")]
		INCallRecordTypeResolutionResult SuccessWithResolvedCallRecordType (INCallRecordType resolvedCallRecordType);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCallRecordTypeResolutionResult SuccessWithResolvedValue (INCallRecordType resolvedValue);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCallRecordTypeToConfirm:")]
		INCallRecordTypeResolutionResult ConfirmationRequiredWithCallRecordTypeToConfirm (INCallRecordType callRecordTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INCancelWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INCancelWorkoutIntentHandling {

		[Abstract]
		[Export ("handleCancelWorkout:completion:")]
		void HandleCancelWorkout (INCancelWorkoutIntent intent, Action<INCancelWorkoutIntentResponse> completion);

		[Export ("confirmCancelWorkout:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmCancelWorkout
#endif
		(INCancelWorkoutIntent intent, Action<INCancelWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForCancelWorkout:withCompletion:")]
		void ResolveWorkoutName (INCancelWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCancelWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCancelWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCancelWorkoutIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarAirCirculationModeResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarAirCirculationModeResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarAirCirculationMode:")]
		INCarAirCirculationModeResolutionResult SuccessWithResolvedCarAirCirculationMode (INCarAirCirculationMode resolvedCarAirCirculationMode);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarAirCirculationModeResolutionResult SuccessWithResolvedValue (INCarAirCirculationMode resolvedValue);

		[iOS (11,0)]
		[Static]
		[Export ("confirmationRequiredWithCarAirCirculationModeToConfirm:")]
		INCarAirCirculationModeResolutionResult ConfirmationRequiredWithCarAirCirculationModeToConfirm (INCarAirCirculationMode carAirCirculationModeToConfirm);

		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarAudioSourceResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarAudioSourceResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarAudioSource:")]
		INCarAudioSourceResolutionResult SuccessWithResolvedCarAudioSource (INCarAudioSource resolvedCarAudioSource);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarAudioSourceResolutionResult SuccessWithResolvedValue (INCarAudioSource resolvedValue);

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarAudioSourceToConfirm:")]
		INCarAudioSourceResolutionResult ConfirmationRequiredWithCarAudioSourceToConfirm (INCarAudioSource carAudioSourceToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarDefrosterResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarDefrosterResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarDefroster:")]
		INCarDefrosterResolutionResult SuccessWithResolvedCarDefroster (INCarDefroster resolvedCarDefroster);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarDefrosterResolutionResult SuccessWithResolvedValue (INCarDefroster resolvedValue);

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarDefrosterToConfirm:")]
		INCarDefrosterResolutionResult ConfirmationRequiredWithCarDefrosterToConfirm (INCarDefroster carDefrosterToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarSeatResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarSeatResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarSeat:")]
		INCarSeatResolutionResult SuccessWithResolvedCarSeat (INCarSeat resolvedCarSeat);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarSeatResolutionResult SuccessWithResolvedValue (INCarSeat resolvedValue);

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarSeatToConfirm:")]
		INCarSeatResolutionResult ConfirmationRequiredWithCarSeatToConfirm (INCarSeat carSeatToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCurrencyAmount : NSCopying, NSSecureCoding {

		[Export ("initWithAmount:currencyCode:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSDecimalNumber amount, string currencyCode);

		[NullAllowed, Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; }

		[NullAllowed, Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INDateComponentsRange : NSCopying, NSSecureCoding {

		[Export ("initWithStartDateComponents:endDateComponents:")]
		IntPtr Constructor ([NullAllowed] NSDateComponents startDateComponents, [NullAllowed] NSDateComponents endDateComponents);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithEKRecurrenceRule:")]
		IntPtr Constructor (EKRecurrenceRule recurrenceRule);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithStartDateComponents:endDateComponents:recurrenceRule:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSDateComponents startDateComponents, [NullAllowed] NSDateComponents endDateComponents, [NullAllowed] INRecurrenceRule recurrenceRule);

		[NullAllowed, Export ("startDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents StartDateComponents { get; }

		[NullAllowed, Export ("endDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents EndDateComponents { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("recurrenceRule", ArgumentSemantic.Copy)]
		INRecurrenceRule RecurrenceRule { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("EKRecurrenceRule")]
		[NullAllowed]
		EKRecurrenceRule EKRecurrenceRule { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Protocol]
	interface INCallsDomainHandling : INStartAudioCallIntentHandling, INSearchCallHistoryIntentHandling
#if !WATCH
	, INStartVideoCallIntentHandling {
#else
	{
#endif
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INCarCommandsDomainHandling : INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling, INGetCarPowerLevelStatusIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INCarPlayDomainHandling : INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling, INSaveProfileInCarIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INWorkoutsDomainHandling : INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling, INResumeWorkoutIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INRadioDomainHandling : INSetRadioStationIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INMessagesDomainHandling : INSendMessageIntentHandling, INSearchForMessagesIntentHandling
#if !WATCH
	, INSetMessageAttributeIntentHandling {
#else
	{
#endif
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol] 
	interface INPaymentsDomainHandling : INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling
#if XAMCORE_4_0 // Added in iOS 11 -> #if __IPHONE_OS_VERSION_MIN_REQUIRED >= 110000
	, INSearchForAccountsIntentHandling, INTransferMoneyIntentHandling
#endif
	{
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INPhotosDomainHandling : INSearchForPhotosIntentHandling, INStartPhotoPlaybackIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INRidesharingDomainHandling : INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling 
#if XAMCORE_4_0 // Added in iOS 11 -> #if __IPHONE_OS_VERSION_MIN_REQUIRED >= 110000
	, INCancelRideIntentHandling, INSendRideFeedbackIntentHandling
#endif
	{
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Protocol]
	interface INNotebookDomainHandling : INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling, INSearchForNotebookItemsIntentHandling {
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Protocol]
	interface INVisualCodeDomainHandling : INGetVisualCodeIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INEndWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INEndWorkoutIntentHandling {

		[Abstract]
		[Export ("handleEndWorkout:completion:")]
		void HandleEndWorkout (INEndWorkoutIntent intent, Action<INEndWorkoutIntentResponse> completion);

		[Export ("confirmEndWorkout:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmEndWorkout
#endif
		(INEndWorkoutIntent intent, Action<INEndWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForEndWorkout:withCompletion:")]
		void ResolveWorkoutName (INEndWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INEndWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INEndWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INEndWorkoutIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INIntentHandlerProviding {

		[Abstract]
		[Export ("handlerForIntent:")]
		[return: NullAllowed]
		NSObject GetHandler (INIntent intent);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	interface INExtension : INIntentHandlerProviding {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntent {

		[iOS (11,0)]
		[Export ("initWithRestaurant:")]
		IntPtr Constructor ([NullAllowed] INRestaurant restaurant);

		[NullAllowed, Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void HandleAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmAvailableRestaurantReservationBookingDefaults
#endif
		(INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookingDefaults:withCompletion:")]
		void ResolveAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INRestaurantResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntentResponse {

		[Export ("defaultPartySize")]
		nuint DefaultPartySize { get; }

		[Export ("defaultBookingDate", ArgumentSemantic.Copy)]
		NSDate DefaultBookingDate { get; }

		[NullAllowed, Export ("maximumPartySize", ArgumentSemantic.Assign)]
		NSNumber MaximumPartySize { get; set; }

		[NullAllowed, Export ("minimumPartySize", ArgumentSemantic.Assign)]
		NSNumber MinimumPartySize { get; set; }

		[Export ("providerImage", ArgumentSemantic.Copy)]
		INImage ProviderImage { get; set; }

		[Export ("initWithDefaultPartySize:defaultBookingDate:code:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (nuint defaultPartySize, NSDate defaultBookingDate, INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INGetAvailableRestaurantReservationBookingsIntent : NSCopying {

		[iOS (11,0)]
		[Export ("initWithRestaurant:partySize:preferredBookingDateComponents:maximumNumberOfResults:earliestBookingDateForResults:latestBookingDateForResults:")]
		IntPtr Constructor (INRestaurant restaurant, nuint partySize, [NullAllowed] NSDateComponents preferredBookingDateComponents, [NullAllowed] NSNumber maximumNumberOfResults, [NullAllowed] NSDate earliestBookingDateForResults, [NullAllowed] NSDate latestBookingDateForResults);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookings:completion:")]
		void HandleAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookings:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetAvailableRestaurantReservationBookingsIntentResponse {

		[Export ("initWithAvailableBookings:code:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRestaurantReservationBooking [] availableBookings, INGetAvailableRestaurantReservationBookingsIntentCode code, [NullAllowed] NSUserActivity userActivity);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INGetRestaurantGuestIntent {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INGetRestaurantGuestIntentHandling {

		[Abstract]
		[Export ("handleGetRestaurantGuest:completion:")]
		void HandleRestaurantGuest (INGetRestaurantGuestIntent intent, Action<INGetRestaurantGuestIntentResponse> completion);

		[Export ("confirmGetRestaurantGuest:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRestaurantGuest
#endif
		(INGetRestaurantGuestIntent guestIntent, Action<INGetRestaurantGuestIntentResponse> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetRestaurantGuestIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INGetRestaurantGuestIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[NullAllowed, Export ("guest", ArgumentSemantic.Copy)]
		INRestaurantGuest Guest { get; set; }

		[NullAllowed, Export ("guestDisplayPreferences", ArgumentSemantic.Copy)]
		INRestaurantGuestDisplayPreferences GuestDisplayPreferences { get; set; }

		[Export ("code")]
		INGetRestaurantGuestIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor] // DesignatedInitializer below
	interface INGetRideStatusIntent {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRideStatus
#endif
		(INGetRideStatusIntent intent, Action<INGetRideStatusIntentResponse> completion);
	}

	interface IINGetRideStatusIntentResponseObserver { }
	
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetRideStatusIntentResponseObserver {

		[Abstract]
		[Export ("getRideStatusResponseDidUpdate:")]
		void DidUpdateRideStatus (INGetRideStatusIntentResponse response);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INGetRideStatusIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INGetRideStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetRideStatusIntentResponseCode Code { get; }

		[NullAllowed, Export ("rideStatus", ArgumentSemantic.Copy)]
		INRideStatus RideStatus { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INGetUserCurrentRestaurantReservationBookingsIntent : NSCopying {

		[iOS (11,0)]
		[Export ("initWithRestaurant:reservationIdentifier:maximumNumberOfResults:earliestBookingDateForResults:")]
		IntPtr Constructor ([NullAllowed] INRestaurant restaurant, [NullAllowed] string reservationIdentifier, [NullAllowed] NSNumber maximumNumberOfResults, [NullAllowed] NSDate earliestBookingDateForResults);

		[iOS (11,0)]
		[Wrap ("this (restaurant, reservationIdentifier, NSNumber.FromNInt (maximumNumberOfResults), earliestBookingDateForResults)")]
		IntPtr Constructor ([NullAllowed] INRestaurant restaurant, [NullAllowed] string reservationIdentifier, nint maximumNumberOfResults, [NullAllowed] NSDate earliestBookingDateForResults);

		[NullAllowed, Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[NullAllowed, Export ("reservationIdentifier")]
		string ReservationIdentifier { get; set; }

		[NullAllowed, Export ("maximumNumberOfResults", ArgumentSemantic.Copy)]
		NSNumber MaximumNumberOfResults { get; set; }

		[NullAllowed, Export ("earliestBookingDateForResults", ArgumentSemantic.Copy)]
		NSDate EarliestBookingDateForResults { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INGetUserCurrentRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetUserCurrentRestaurantReservationBookings:completion:")]
		void HandleUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetUserCurrentRestaurantReservationBookings:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmUserCurrentRestaurantReservationBookings
#endif
		(INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetUserCurrentRestaurantReservationBookings:withCompletion:")]
		void ResolveUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INRestaurantResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	interface INGetUserCurrentRestaurantReservationBookingsIntentResponse {

		[Export ("initWithUserCurrentBookings:code:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRestaurantReservationUserBooking [] userCurrentBookings, INGetUserCurrentRestaurantReservationBookingsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetUserCurrentRestaurantReservationBookingsIntentResponseCode Code { get; }

		[Export ("userCurrentBookings", ArgumentSemantic.Copy)]
		INRestaurantReservationUserBooking [] UserCurrentBookings { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INImage : NSCopying, NSSecureCoding {

		[Static]
		[Export ("imageNamed:")]
		INImage FromName (string name);

		[Static]
		[Export ("imageWithImageData:")]
		INImage FromData (NSData imageData);

		[Static]
		[MarshalNativeExceptions]
		[Export ("imageWithURL:")]
		INImage FromUrl (NSUrl url);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Static]
		[Export ("imageWithURL:width:height:")]
		[return: NullAllowed]
		INImage FromUrl (NSUrl url, double width, double height);

		// INImage_IntentsUI (IntentsUI)

		[NoMac, NoWatch]
		[Static]
		[Export ("imageWithCGImage:")]
		INImage FromImage (CGImage image);

		[NoMac, NoWatch]
		[Static]
		[Export ("imageWithUIImage:")]
		INImage FromImage (UIImage image);

		[NoMac, NoWatch]
		[Static]
		[Export ("imageSizeForIntentResponse:")]
		CGSize GetImageSize (INIntentResponse response);

		[NoMac, NoWatch, iOS (11,0)]
		[Async]
		[Export ("fetchUIImageWithCompletion:")]
		void FetchImage (Action<UIImage> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface INIntent : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("identifier")]
		NSString IdentifierString { get; }

		[Unavailable (PlatformName.MacOSX)]
		[Wrap ("INIntentIdentifierExtensions.GetValue (IdentifierString)")]
		INIntentIdentifier? Identifier { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("intentDescription")]
		string IntentDescription { get; }
	}

	interface INIntentResolutionResult<ObjectType> : INIntentResolutionResult { }

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface INIntentResponse : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("userActivity", ArgumentSemantic.Copy)]
		NSUserActivity UserActivity { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INInteraction : NSSecureCoding, NSCopying {

		[Export ("initWithIntent:response:")]
		[DesignatedInitializer]
		IntPtr Constructor (INIntent intent, [NullAllowed] INIntentResponse response);

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
		[iOS (11,0), Watch (4,0), NoMac]
		[Export ("parameterValueForParameter:")]
		IntPtr _GetParameterValue (INParameter parameter);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INListRideOptionsIntent {

		[Export ("initWithPickupLocation:dropOffLocation:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation);

		[NullAllowed, Export ("pickupLocation", ArgumentSemantic.Copy)]
		CLPlacemark PickupLocation { get; }

		[NullAllowed, Export ("dropOffLocation", ArgumentSemantic.Copy)]
		CLPlacemark DropOffLocation { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INListRideOptionsIntentHandling {

		[Abstract]
		[Export ("handleListRideOptions:completion:")]
		void HandleListRideOptions (INListRideOptionsIntent intent, Action<INListRideOptionsIntentResponse> completion);

		[Export ("confirmListRideOptions:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INListRideOptionsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INListRideOptionsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INListRideOptionsIntentResponseCode Code { get; }

		[NullAllowed, Export ("rideOptions", ArgumentSemantic.Copy)]
		INRideOption [] RideOptions { get; set; }

		[NullAllowed, Export ("paymentMethods", ArgumentSemantic.Copy)]
		INPaymentMethod [] PaymentMethods { get; set; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMessage : NSCopying, NSSecureCoding {

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:messageType:")]
		IntPtr Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, INMessageType messageType);

		[Export ("initWithIdentifier:content:dateSent:sender:recipients:")]
		IntPtr Constructor (string identifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients);

		[Export ("identifier")]
		string Identifier { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
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

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("messageType")]
		INMessageType MessageType { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeOptionsResolutionResult {

		[Watch (4,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedMessageAttributeOptions:")]
		INMessageAttributeOptionsResolutionResult SuccessWithResolvedMessageAttributeOptions (INMessageAttributeOptions resolvedMessageAttributeOptions);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeOptionsResolutionResult SuccessWithResolvedValue (INMessageAttributeOptions resolvedValue);

		[Watch (4,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithMessageAttributeOptionsToConfirm:")]
		INMessageAttributeOptionsResolutionResult ConfirmationRequiredWithMessageAttributeOptionsToConfirm (INMessageAttributeOptions messageAttributeOptionsToConfirm);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeResolutionResult {

		[Watch (4,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedMessageAttribute:")]
		INMessageAttributeResolutionResult SuccessWithResolvedMessageAttribute (INMessageAttribute resolvedMessageAttribute);

		[Internal]
		[Introduced (PlatformName.MacOSX, 10, 12)]
		[Introduced (PlatformName.WatchOS, 3, 2)]
		[Introduced (PlatformName.iOS, 10, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeResolutionResult SuccessWithResolvedValue (INMessageAttribute resolvedValue);

		[Watch (4,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithMessageAttributeToConfirm:")]
		INMessageAttributeResolutionResult ConfirmationRequiredWithMessageAttributeToConfirm (INMessageAttribute messageAttributeToConfirm);

		[Internal]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INPauseWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INPauseWorkoutIntentHandling {

		[Abstract]
		[Export ("handlePauseWorkout:completion:")]
		void HandlePauseWorkout (INPauseWorkoutIntent intent, Action<INPauseWorkoutIntentResponse> completion);

		[Export ("confirmPauseWorkout:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmPauseWorkout
#endif
		(INPauseWorkoutIntent intent, Action<INPauseWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForPauseWorkout:withCompletion:")]
		void ResolveWorkoutName (INPauseWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INPauseWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INPauseWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INPauseWorkoutIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPaymentMethod : NSCopying, NSSecureCoding {

		[Export ("initWithType:name:identificationHint:icon:")]
		[DesignatedInitializer]
		IntPtr Constructor (INPaymentMethodType type, [NullAllowed] string name, [NullAllowed] string identificationHint, [NullAllowed] INImage icon);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPaymentRecord : NSCopying, NSSecureCoding {

		[Export ("initWithPayee:payer:currencyAmount:paymentMethod:note:status:feeAmount:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson payee, [NullAllowed] INPerson payer, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] INPaymentMethod paymentMethod, [NullAllowed] string note, INPaymentStatus status, [NullAllowed] INCurrencyAmount feeAmount);

		[Export ("initWithPayee:payer:currencyAmount:paymentMethod:note:status:")]
		IntPtr Constructor ([NullAllowed] INPerson payee, [NullAllowed] INPerson payer, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] INPaymentMethod paymentMethod, [NullAllowed] string note, INPaymentStatus status);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPerson : NSCopying, NSSecureCoding, INSpeakable {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier);

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

		[Introduced (PlatformName.iOS, 10, 0)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[NullAllowed, Export ("relationship"), Protected]
		NSString WeakRelationship { get; }

		[Introduced (PlatformName.iOS, 10, 0)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[Wrap ("INPersonRelationshipExtensions.GetValue (WeakRelationship)")]
		INPersonRelationship Relationship { get; }

		// Inlined from INInteraction (INPerson) Category

		[NullAllowed, Export ("aliases", ArgumentSemantic.Copy)]
		INPersonHandle [] Aliases { get; }

		[Export ("suggestionType")]
		INPersonSuggestionType SuggestionType { get; }

		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:aliases:suggestionType:")]
		IntPtr Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, [NullAllowed] INPersonHandle [] aliases, INPersonSuggestionType suggestionType);

		// Inlined from INInteraction (INPerson) Category

		[Introduced (PlatformName.iOS, 10, 3)]
		[Unavailable (PlatformName.MacOSX)]
		[Export ("siriMatches", ArgumentSemantic.Copy), NullAllowed]
		INPerson [] SiriMatches { get; }

		[Mac (10,13, onlyOn64:true), iOS (11,0), Watch (4,0)]
		[Export ("isMe")]
		bool IsMe { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPersonHandle : NSCopying, NSSecureCoding {

		[Export ("value"), NullAllowed]
		string Value { get; }

		[Export ("type")]
		INPersonHandleType Type { get; }

		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[Export ("label"), NullAllowed, Protected]
		NSString WeakLabel { get; }

		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[Wrap ("INPersonHandleLabelExtensions.GetValue (WeakLabel)")]
		INPersonHandleLabel Label { get; }

		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[Wrap ("this (value, type, label.GetConstant ())")]
		IntPtr Constructor (string value, INPersonHandleType type, INPersonHandleLabel label);

		[DesignatedInitializer]
		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[Export ("initWithValue:type:label:"), Protected]
		IntPtr Constructor ([NullAllowed] string value, INPersonHandleType type, [NullAllowed] NSString stringLabel);

		[Export ("initWithValue:type:")]
		IntPtr Constructor ([NullAllowed] string value, INPersonHandleType type);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
#if XAMCORE_4_0
	[DisableDefaultCtor]
#endif	
	[BaseType (typeof (NSObject))]
	interface INPreferences {

		[NoWatch]
		[Static]
		[Export ("siriAuthorizationStatus")]
		INSiriAuthorizationStatus SiriAuthorizationStatus { get; }

		[NoWatch]
		[Static]
		[Async]
		[Export ("requestSiriAuthorization:")]
		void RequestSiriAuthorization (Action<INSiriAuthorizationStatus> handler);

		[Static]
		[Export ("siriLanguageCode")]
		string SiriLanguageCode { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPriceRange : NSCopying, NSSecureCoding {

		[Export ("initWithRangeBetweenPrice:andPrice:currencyCode:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSDecimalNumber firstPrice, NSDecimalNumber secondPrice, string currencyCode);

		[Internal]
		[Export ("initWithMaximumPrice:currencyCode:")]
		IntPtr InitWithMaximumPrice (NSDecimalNumber maximumPrice, string currencyCode);

		[Internal]
		[Export ("initWithMinimumPrice:currencyCode:")]
		IntPtr InitWithMinimumPrice (NSDecimalNumber minimumPrice, string currencyCode);

		[Export ("initWithPrice:currencyCode:")]
		IntPtr Constructor (NSDecimalNumber price, string currencyCode);

		[NullAllowed, Export ("minimumPrice")]
		NSDecimalNumber MinimumPrice { get; }

		[NullAllowed, Export ("maximumPrice")]
		NSDecimalNumber MaximumPrice { get; }

		[Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRadioTypeResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRadioTypeResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedRadioType:")]
		INRadioTypeResolutionResult SuccessWithResolvedRadioType (INRadioType resolvedRadioType);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INRadioTypeResolutionResult SuccessWithResolvedValue (INRadioType resolvedValue);

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithRadioTypeToConfirm:")]
		INRadioTypeResolutionResult ConfirmationRequiredWithRadioTypeToConfirm (INRadioType radioTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRelativeReferenceResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedRelativeReference:")]
		INRelativeReferenceResolutionResult SuccessWithResolvedRelativeReference (INRelativeReference resolvedRelativeReference);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INRelativeReferenceResolutionResult SuccessWithResolvedValue (INRelativeReference resolvedValue);

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithRelativeReferenceToConfirm:")]
		INRelativeReferenceResolutionResult ConfirmationRequiredWithRelativeReferenceToConfirm (INRelativeReference relativeReferenceToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRelativeSettingResolutionResult {

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedRelativeSetting:")]
		INRelativeSettingResolutionResult SuccessWithResolvedRelativeSetting (INRelativeSetting resolvedRelativeSetting);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INRelativeSettingResolutionResult SuccessWithResolvedValue (INRelativeSetting resolvedValue);

		[iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithRelativeSettingToConfirm:")]
		INRelativeSettingResolutionResult ConfirmationRequiredWithRelativeSettingToConfirm (INRelativeSetting relativeSettingToConfirm);

		[Internal]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INRequestPaymentIntent {

		[Export ("initWithPayer:currencyAmount:note:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson payer, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] string note);

		[NullAllowed, Export ("payer", ArgumentSemantic.Copy)]
		INPerson Payer { get; }

		[NullAllowed, Export ("currencyAmount", ArgumentSemantic.Copy)]
		INCurrencyAmount CurrencyAmount { get; }

		[NullAllowed, Export ("note")]
		string Note { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INRequestPaymentIntentHandling {

		[Abstract]
		[Export ("handleRequestPayment:completion:")]
		void HandleRequestPayment (INRequestPaymentIntent intent, Action<INRequestPaymentIntentResponse> completion);

		[Export ("confirmRequestPayment:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmRequestPayment
#endif
		(INRequestPaymentIntent intent, Action<INRequestPaymentIntentResponse> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Export ("resolvePayerForRequestPayment:withCompletion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[Watch (4,0), iOS (11,0)]
		[Export ("resolvePayerForRequestPayment:completion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INRequestPaymentPayerResolutionResult> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveCurrencyAmount (INRequestPaymentIntent, Action<INRequestPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveCurrencyAmount (INRequestPaymentIntent, Action<INRequestPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Export ("resolveCurrencyAmountForRequestPayment:withCompletion:")]
		void ResolveCurrencyAmount (INRequestPaymentIntent intent, Action<INCurrencyAmountResolutionResult> completion);

		[Watch (4,0), iOS (11,0)]
		[Export ("resolveCurrencyAmountForRequestPayment:completion:")]
		void ResolveCurrencyAmount (INRequestPaymentIntent intent, Action<INRequestPaymentCurrencyAmountResolutionResult> completion);

		[Export ("resolveNoteForRequestPayment:withCompletion:")]
		void ResolveNote (INRequestPaymentIntent intent, Action<INStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INRequestPaymentIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRequestPaymentIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INRequestPaymentIntentResponseCode Code { get; }

		[NullAllowed, Export ("paymentRecord", ArgumentSemantic.Copy)]
		INPaymentRecord PaymentRecord { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INRequestRideIntent {

		[Deprecated (PlatformName.iOS, 10, 3, message: "Use the INDateComponentsRange overload")]
		[Unavailable (PlatformName.WatchOS)]
		[Export ("initWithPickupLocation:dropOffLocation:rideOptionName:partySize:paymentMethod:")]
		IntPtr Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation, [NullAllowed] INSpeakableString rideOptionName, [NullAllowed] NSNumber partySize, [NullAllowed] INPaymentMethod paymentMethod);

		[Introduced (PlatformName.iOS, 10, 3)]
		[Export ("initWithPickupLocation:dropOffLocation:rideOptionName:partySize:paymentMethod:scheduledPickupTime:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation, [NullAllowed] INSpeakableString rideOptionName, [NullAllowed] NSNumber partySize, [NullAllowed] INPaymentMethod paymentMethod, [NullAllowed] INDateComponentsRange scheduledPickupTime);

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

		[Introduced (PlatformName.iOS, 10, 3)]
		[NullAllowed, Export ("scheduledPickupTime", ArgumentSemantic.Copy)]
		INDateComponentsRange ScheduledPickupTime { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INRequestRideIntentHandling {

		[Abstract]
		[Export ("handleRequestRide:completion:")]
		void HandleRequestRide (INRequestRideIntent intent, Action<INRequestRideIntentResponse> completion);

		[Export ("confirmRequestRide:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

		[Introduced (PlatformName.iOS, 10, 3)]
		[Export ("resolveScheduledPickupTimeForRequestRide:withCompletion:")]
		void ResolveScheduledPickupTime (INRequestRideIntent intent, Action<INDateComponentsRangeResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INRequestRideIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRequestRideIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INRequestRideIntentResponseCode Code { get; }

		[NullAllowed, Export ("rideStatus", ArgumentSemantic.Copy)]
		INRideStatus RideStatus { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	interface INRestaurant : NSSecureCoding, NSCopying {

		[Export ("initWithLocation:name:vendorIdentifier:restaurantIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (CLLocation location, string name, string vendorIdentifier, string restaurantIdentifier);

		[Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("vendorIdentifier")]
		string VendorIdentifier { get; set; }

		[Export ("restaurantIdentifier")]
		string RestaurantIdentifier { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INPerson))]
	[DisableDefaultCtor] // The base type, INPerson, has no default ctor.
	interface INRestaurantGuest {

		[Export ("initWithNameComponents:phoneNumber:emailAddress:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string phoneNumber, [NullAllowed] string emailAddress);

		[NullAllowed, Export ("phoneNumber")]
		string PhoneNumber { get; set; }

		[NullAllowed, Export ("emailAddress")]
		string EmailAddress { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	interface INRestaurantOffer : NSSecureCoding, NSCopying {

		[Export ("offerTitleText")]
		string OfferTitleText { get; set; }

		[Export ("offerDetailText")]
		string OfferDetailText { get; set; }

		[Export ("offerIdentifier")]
		string OfferIdentifier { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	interface INRestaurantReservationBooking : NSSecureCoding, NSCopying {

		[Export ("initWithRestaurant:bookingDate:partySize:bookingIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRestaurant restaurant, NSDate bookingDate, nuint partySize, string bookingIdentifier);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INRestaurantReservationBooking))]
	interface INRestaurantReservationUserBooking : NSCopying {

		[Export ("initWithRestaurant:bookingDate:partySize:bookingIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRestaurant restaurant, NSDate bookingDate, nuint partySize, string bookingIdentifier);

		[Export ("initWithRestaurant:bookingDate:partySize:bookingIdentifier:guest:status:dateStatusModified:")]
		IntPtr Constructor (INRestaurant restaurant, NSDate bookingDate, nuint partySize, string bookingIdentifier, INRestaurantGuest guest, INRestaurantReservationUserBookingStatus status, NSDate dateStatusModified);

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

		[Export ("dateStatusModified", ArgumentSemantic.Assign)]
		NSDate DateStatusModified { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INResumeWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INResumeWorkoutIntentHandling {

		[Abstract]
		[Export ("handleResumeWorkout:completion:")]
		void HandleResumeWorkout (INResumeWorkoutIntent intent, Action<INResumeWorkoutIntentResponse> completion);

		[Export ("confirmResumeWorkout:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmResumeWorkout
#endif
		(INResumeWorkoutIntent intent, Action<INResumeWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForResumeWorkout:withCompletion:")]
		void ResolveWorkoutName (INResumeWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INResumeWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INResumeWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INResumeWorkoutIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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

		[Watch (4,0), iOS (11,0)]
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

		[Watch (4,0), iOS (11,0)]
		[Export ("feedbackType", ArgumentSemantic.Assign)]
		INRideFeedbackTypeOptions FeedbackType { get; }

		[Export ("outstanding")]
		bool Outstanding { [Bind ("isOutstanding")] get; }

		[Watch (4,0), iOS (11,0)]
		[NullAllowed, Export ("defaultTippingOptions", ArgumentSemantic.Strong)]
		NSSet<INCurrencyAmount> DefaultTippingOptions { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRideDriver bound
	[BaseType (typeof (INPerson))]
	[DisableDefaultCtor] // xcode 8.2 beta 1 -> NSInvalidArgumentException Reason: *** -[__NSPlaceholderDictionary initWithObjects:forKeys:count:]: attempt to insert nil object from objects[1]
	interface INRideDriver : NSCopying, NSSecureCoding {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:rating:phoneNumber:")]
		[Deprecated (PlatformName.iOS, 10,2, message:"Use the overload signature instead.")]
		[Unavailable (PlatformName.WatchOS)]
		IntPtr Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string rating, [NullAllowed] string phoneNumber);

		[Export ("initWithPhoneNumber:nameComponents:displayName:image:rating:")]
		[Introduced (PlatformName.iOS, 10,2)]
		[DesignatedInitializer]
		IntPtr Constructor (string phoneNumber, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string rating);

		[NullAllowed, Export ("rating")]
		string Rating { get; }

		[NullAllowed, Export ("phoneNumber")]
		string PhoneNumber { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRideFareLineItem : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:price:currencyCode:")]
		[DesignatedInitializer]
		IntPtr Constructor (string title, NSDecimalNumber price, string currencyCode);

		[Export ("title")]
		string Title { get; }

		[Export ("price")]
		NSDecimalNumber Price { get; }

		[Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRideOption : NSCopying, NSSecureCoding {

		[Export ("initWithName:estimatedPickupDate:")]
		[DesignatedInitializer]
		IntPtr Constructor (string name, NSDate estimatedPickupDate);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRidePartySizeOption : NSCopying, NSSecureCoding {

		[Export ("initWithPartySizeRange:sizeDescription:priceRange:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSRange partySizeRange, string sizeDescription, [NullAllowed] INPriceRange priceRange);

		[Export ("partySizeRange")]
		NSRange PartySizeRange { get; }

		[Export ("sizeDescription")]
		string SizeDescription { get; }

		[NullAllowed, Export ("priceRange")]
		INPriceRange PriceRange { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

		[Introduced (PlatformName.iOS, 10, 3)]
		[Introduced (PlatformName.WatchOS, 3, 2)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSaveProfileInCarIntent {

		[Deprecated (PlatformName.iOS, 10, 2)]
		[Export ("initWithProfileNumber:profileLabel:"), Internal]
		IntPtr InitWithProfileNumberLabel ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel);

		[Introduced (PlatformName.iOS, 10, 2)]
		[Export ("initWithProfileNumber:profileName:"), Internal]
		IntPtr InitWithProfileNumberName ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileName);

		[NullAllowed, Export ("profileNumber", ArgumentSemantic.Copy)]
		NSNumber ProfileNumber { get; }

		[Deprecated (PlatformName.iOS, 10,2, message:"Use 'ProfileName' instead.")]
		[NullAllowed, Export ("profileLabel")]
		string ProfileLabel { get; }

		[Introduced (PlatformName.iOS, 10, 2)]
		[NullAllowed, Export ("profileName")]
		string ProfileName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSaveProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSaveProfileInCar:completion:")]
		void HandleSaveProfileInCar (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("confirmSaveProfileInCar:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSaveProfileInCar
#endif
		(INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSaveProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSaveProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Introduced (PlatformName.iOS, 10, 2)]
		[Export ("resolveProfileNameForSaveProfileInCar:withCompletion:")]
		void ResolveProfileName (INSaveProfileInCarIntent intent, Action<INStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSaveProfileInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSaveProfileInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSaveProfileInCarIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntent))]
	interface INSearchCallHistoryIntent {

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithDateCreated:recipient:callCapabilities:callTypes:unseen:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities, INCallRecordTypeOptions callTypes, [NullAllowed] NSNumber unseen);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Wrap ("this (dateCreated, recipient, callCapabilities, callTypes, new NSNumber (unseen))")]
		IntPtr Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities, INCallRecordTypeOptions callTypes, bool unseen);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Export ("initWithCallType:dateCreated:recipient:callCapabilities:")]
		IntPtr Constructor (INCallRecordType callType, [NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CallTypes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CallTypes' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CallTypes' instead.")]
		[Export ("callType", ArgumentSemantic.Assign)]
		INCallRecordType CallType { get; }

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		INDateComponentsRange DateCreated { get; }

		[NullAllowed, Export ("recipient", ArgumentSemantic.Copy)]
		INPerson Recipient { get; }

		[Export ("callCapabilities", ArgumentSemantic.Assign)]
		INCallCapabilityOptions CallCapabilities { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("callTypes", ArgumentSemantic.Assign)]
		INCallRecordTypeOptions CallTypes { get; }

		[Protected]
		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("unseen", ArgumentSemantic.Copy)]
		NSNumber WeakUnseen { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Protocol]
	interface INSearchCallHistoryIntentHandling {

		[Abstract]
		[Export ("handleSearchCallHistory:completion:")]
		void HandleSearchCallHistory (INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Export ("confirmSearchCallHistory:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSearchCallHistory
#endif
		(INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveCallTypes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveCallTypes' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveCallTypes' instead.")]
		[Export ("resolveCallTypeForSearchCallHistory:withCompletion:")]
		void ResolveCallType (INSearchCallHistoryIntent intent, Action<INCallRecordTypeResolutionResult> completion);

		[Export ("resolveDateCreatedForSearchCallHistory:withCompletion:")]
		void ResolveDateCreated (INSearchCallHistoryIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveRecipientForSearchCallHistory:withCompletion:")]
		void ResolveRecipient (INSearchCallHistoryIntent intent, Action<INPersonResolutionResult> completion);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("resolveCallTypesForSearchCallHistory:withCompletion:")]
		void ResolveCallTypes (INSearchCallHistoryIntent intent, Action<INCallRecordTypeOptionsResolutionResult> completion);

		[Watch (4,0), NoMac, iOS (11,0)]
		[Export ("resolveUnseenForSearchCallHistory:withCompletion:")]
		void ResolveUnseen (INSearchCallHistoryIntent intent, Action<INBooleanResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchCallHistoryIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSearchCallHistoryIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchCallHistoryIntentResponseCode Code { get; }

		[Mac (10,13, onlyOn64:true), iOS (11,0), Watch (4,0)]
		[NullAllowed, Export ("callRecords", ArgumentSemantic.Copy)]
		INCallRecord [] CallRecords { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntent))]
	interface INSearchForMessagesIntent {

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:speakableGroupNames:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] INSpeakableString [] speakableGroupNames);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:groupNames:")]
		IntPtr Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] string [] groupNames);

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
		[NullAllowed, Export ("groupNames", ArgumentSemantic.Copy)]
		string [] GroupNames { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNamesOperator' instead.")]
		[Export ("groupNamesOperator", ArgumentSemantic.Assign)]
		INConditionalOperator GroupNamesOperator { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("speakableGroupNames", ArgumentSemantic.Copy)]
		INSpeakableString [] SpeakableGroupNames { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("speakableGroupNamesOperator", ArgumentSemantic.Assign)]
		INConditionalOperator SpeakableGroupNamesOperator { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Protocol]
	interface INSearchForMessagesIntentHandling {
		
		[Abstract]
		[Export ("handleSearchForMessages:completion:")]
		void HandleSearchForMessages (INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion);

		[Export ("confirmSearchForMessages:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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
		[Export ("resolveGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveGroupNames (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("resolveSpeakableGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveSpeakableGroupNames (INSearchForMessagesIntent intent, Action<INSpeakableStringResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForMessagesIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSearchForMessagesIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForMessagesIntentResponseCode Code { get; }

		[NullAllowed, Export ("messages", ArgumentSemantic.Copy)]
		INMessage [] Messages { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INSearchForPhotosIntent {

		[Export ("initWithDateCreated:locationCreated:albumName:searchTerms:includedAttributes:excludedAttributes:peopleInPhoto:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] CLPlacemark locationCreated, [NullAllowed] string albumName, [NullAllowed] string [] searchTerms, INPhotoAttributeOptions includedAttributes, INPhotoAttributeOptions excludedAttributes, [NullAllowed] INPerson [] peopleInPhoto);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INSearchForPhotosIntentHandling {

		[Abstract]
		[Export ("handleSearchForPhotos:completion:")]
		void HandleSearchForPhotos (INSearchForPhotosIntent intent, Action<INSearchForPhotosIntentResponse> completion);

		[Export ("confirmSearchForPhotos:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

		[Watch (4,0), iOS (11,0)]
		[Export ("resolveSearchTermsForSearchForPhotos:withCompletion:")]
		void ResolveSearchTerms (INSearchForPhotosIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolvePeopleInPhotoForSearchForPhotos:withCompletion:")]
		void ResolvePeopleInPhoto (INSearchForPhotosIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForPhotosIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSearchForPhotosIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForPhotosIntentResponseCode Code { get; }

		[NullAllowed, Export ("searchResultsCount", ArgumentSemantic.Copy)]
		NSNumber SearchResultsCount { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntent))]
	interface INSendMessageIntent {

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithRecipients:content:speakableGroupName:conversationIdentifier:serviceName:sender:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] INSpeakableString speakableGroupName, [NullAllowed] string conversationIdentifier, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Export ("initWithRecipients:content:groupName:serviceName:sender:")]
		IntPtr Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] string groupName, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }

		[NullAllowed, Export ("content")]
		string Content { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("speakableGroupName", ArgumentSemantic.Copy)]
		INSpeakableString SpeakableGroupName { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[NullAllowed, Export ("conversationIdentifier")]
		string ConversationIdentifier { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[NullAllowed, Export ("groupName")]
		string GroupName { get; }

		[NullAllowed, Export ("serviceName")]
		string ServiceName { get; }

		[NullAllowed, Export ("sender", ArgumentSemantic.Copy)]
		INPerson Sender { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Protocol]
	interface INSendMessageIntentHandling {

		[Abstract]
		[Export ("handleSendMessage:completion:")]
		void HandleSendMessage (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

		[Export ("confirmSendMessage:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSendMessage
#endif
		(INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveRecipients (INSendMessageIntent, Action<INSendMessageRecipientResolutionResult []>)' instead.")]
		[Export ("resolveRecipientsForSendMessage:withCompletion:")]
		void ResolveRecipients (INSendMessageIntent intent, Action<INPersonResolutionResult []> completion);

		[Watch (4,0), iOS (11,0), Mac (10,13, onlyOn64:true)]
		[Export ("resolveRecipientsForSendMessage:completion:")]
		void ResolveRecipients (INSendMessageIntent intent, Action<INSendMessageRecipientResolutionResult []> completion);

		[Export ("resolveContentForSendMessage:withCompletion:")]
		void ResolveContent (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Export ("resolveGroupNameForSendMessage:withCompletion:")]
		void ResolveGroupName (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[Watch (4,0), iOS (11,0), Mac (10,13, onlyOn64:true)]
		[Export ("resolveSpeakableGroupNameForSendMessage:withCompletion:")]
		void ResolveSpeakableGroupName (INSendMessageIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSendMessageIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSendMessageIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSendMessageIntentResponseCode Code { get; }

		[Watch (4,0), iOS (11,0), Mac (10,13, onlyOn64:true)]
		[NullAllowed, Export ("sentMessage", ArgumentSemantic.Copy)]
		INMessage SentMessage { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INSendPaymentIntent {

		[Export ("initWithPayee:currencyAmount:note:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson payee, [NullAllowed] INCurrencyAmount currencyAmount, [NullAllowed] string note);

		[NullAllowed, Export ("payee", ArgumentSemantic.Copy)]
		INPerson Payee { get; }

		[NullAllowed, Export ("currencyAmount", ArgumentSemantic.Copy)]
		INCurrencyAmount CurrencyAmount { get; }

		[NullAllowed, Export ("note")]
		string Note { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INSendPaymentIntentHandling {

		[Abstract]
		[Export ("handleSendPayment:completion:")]
		void HandleSendPayment (INSendPaymentIntent intent, Action<INSendPaymentIntentResponse> completion);

		[Export ("confirmSendPayment:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSendPayment
#endif
		(INSendPaymentIntent intent, Action<INSendPaymentIntentResponse> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Export ("resolvePayeeForSendPayment:withCompletion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[Watch (4,0), iOS (11,0)]
		[Export ("resolvePayeeForSendPayment:completion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INSendPaymentPayeeResolutionResult> completion);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ResolveCurrencyAmount (INSendPaymentIntent, Action<INSendPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveCurrencyAmount (INSendPaymentIntent, Action<INSendPaymentCurrencyAmountResolutionResult>)' instead.")]
		[Export ("resolveCurrencyAmountForSendPayment:withCompletion:")]
		void ResolveCurrencyAmount (INSendPaymentIntent intent, Action<INCurrencyAmountResolutionResult> completion);

		[Watch (4,0), iOS (11,0)]
		[Export ("resolveCurrencyAmountForSendPayment:completion:")]
		void ResolveCurrencyAmount (INSendPaymentIntent intent, Action<INSendPaymentCurrencyAmountResolutionResult> completion);

		[Export ("resolveNoteForSendPayment:withCompletion:")]
		void ResolveNote (INSendPaymentIntent intent, Action<INStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSendPaymentIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSendPaymentIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSendPaymentIntentResponseCode Code { get; }

		[NullAllowed, Export ("paymentRecord", ArgumentSemantic.Copy)]
		INPaymentRecord PaymentRecord { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetAudioSourceInCarIntent {

		[Export ("initWithAudioSource:relativeAudioSourceReference:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCarAudioSource audioSource, INRelativeReference relativeAudioSourceReference);

		[Export ("audioSource", ArgumentSemantic.Assign)]
		INCarAudioSource AudioSource { get; }

		[Export ("relativeAudioSourceReference", ArgumentSemantic.Assign)]
		INRelativeReference RelativeAudioSourceReference { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetAudioSourceInCarIntentHandling {

		[Abstract]
		[Export ("handleSetAudioSourceInCar:completion:")]
		void HandleSetAudioSourceInCar (INSetAudioSourceInCarIntent intent, Action<INSetAudioSourceInCarIntentResponse> completion);

		[Export ("confirmSetAudioSourceInCar:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetAudioSourceInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetAudioSourceInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetAudioSourceInCarIntentResponseCode Code { get; }
	}

	// HACK only to please the generator - which does not (normally) know the type hierarchy in the
	// binding files. The lack of namespace will generate the correct code for the C# compiler
	interface NSUnitTemperature : NSUnit {}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetClimateSettingsInCarIntent {

		[Protected]
		[Export ("initWithEnableFan:enableAirConditioner:enableClimateControl:enableAutoMode:airCirculationMode:fanSpeedIndex:fanSpeedPercentage:relativeFanSpeedSetting:temperature:relativeTemperatureSetting:climateZone:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber enableFan, [NullAllowed] NSNumber enableAirConditioner, [NullAllowed] NSNumber enableClimateControl, [NullAllowed] NSNumber enableAutoMode, INCarAirCirculationMode airCirculationMode, [NullAllowed] NSNumber fanSpeedIndex, [NullAllowed] NSNumber fanSpeedPercentage, INRelativeSetting relativeFanSpeedSetting, [NullAllowed] NSMeasurement<NSUnitTemperature> temperature, INRelativeSetting relativeTemperatureSetting, INCarSeat climateZone);

		[Internal]
		[NullAllowed, Export ("enableFan", ArgumentSemantic.Copy)]
		NSNumber _EnableFan { get; }

		[Internal]
		[NullAllowed, Export ("enableAirConditioner", ArgumentSemantic.Copy)]
		NSNumber _EnableAirConditioner { get; }

		[Internal]
		[NullAllowed, Export ("enableClimateControl", ArgumentSemantic.Copy)]
		NSNumber _EnableClimateControl { get; }

		[Internal]
		[NullAllowed, Export ("enableAutoMode", ArgumentSemantic.Copy)]
		NSNumber _EnableAutoMode { get; }

		[Export ("airCirculationMode", ArgumentSemantic.Assign)]
		INCarAirCirculationMode AirCirculationMode { get; }

		[NullAllowed, Export ("fanSpeedIndex", ArgumentSemantic.Copy)]
		NSNumber FanSpeedIndex { get; }

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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetClimateSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetClimateSettingsInCar:completion:")]
		void HandleSetClimateSettingsInCar (INSetClimateSettingsInCarIntent intent, Action<INSetClimateSettingsInCarIntentResponse> completion);

		[Export ("confirmSetClimateSettingsInCar:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetClimateSettingsInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetClimateSettingsInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetClimateSettingsInCarIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetDefrosterSettingsInCarIntent {

		[Protected]
		[Export ("initWithEnable:defroster:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber enable, INCarDefroster defroster);

		[Internal]
		[NullAllowed, Export ("enable", ArgumentSemantic.Copy)]
		NSNumber _Enable { get; }

		[Export ("defroster", ArgumentSemantic.Assign)]
		INCarDefroster Defroster { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetDefrosterSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetDefrosterSettingsInCar:completion:")]
		void HandleSetDefrosterSettingsInCar (INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

		[Export ("confirmSetDefrosterSettingsInCar:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetDefrosterSettingsInCar
#endif
		(INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

		[Export ("resolveEnableForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveEnable (INSetDefrosterSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveDefrosterForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveDefroster (INSetDefrosterSettingsInCarIntent intent, Action<INCarDefrosterResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetDefrosterSettingsInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetDefrosterSettingsInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetDefrosterSettingsInCarIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetMessageAttributeIntent {

		[Export ("initWithIdentifiers:attribute:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] string [] identifiers, INMessageAttribute attribute);

		[NullAllowed, Export ("identifiers", ArgumentSemantic.Copy)]
		string [] Identifiers { get; }

		[Export ("attribute", ArgumentSemantic.Assign)]
		INMessageAttribute Attribute { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetMessageAttributeIntentHandling {

		[Abstract]
		[Export ("handleSetMessageAttribute:completion:")]
		void HandleSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("confirmSetMessageAttribute:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetMessageAttribute
#endif
		(INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("resolveAttributeForSetMessageAttribute:withCompletion:")]
		void ResolveAttribute (INSetMessageAttributeIntent intent, Action<INMessageAttributeResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetMessageAttributeIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetMessageAttributeIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetMessageAttributeIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetProfileInCarIntent {

		[Deprecated (PlatformName.iOS, 10, 2)]
		[Export ("initWithProfileNumber:profileLabel:defaultProfile:"), Internal]
		IntPtr InitWithProfileNumberLabel ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel, [NullAllowed] NSNumber defaultProfile);

		[Introduced (PlatformName.iOS, 10, 2)]
		[Export ("initWithProfileNumber:profileName:defaultProfile:"), Internal]
		IntPtr InitWithProfileNumberName ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileName, [NullAllowed] NSNumber defaultProfile);

		[NullAllowed, Export ("profileNumber", ArgumentSemantic.Copy)]
		NSNumber ProfileNumber { get; }

		[Deprecated (PlatformName.iOS, 10, 2, message: "Use 'ProfileName' instead.")]
		[NullAllowed, Export ("profileLabel")]
		string ProfileLabel { get; }

		[Introduced (PlatformName.iOS, 10, 2)]
		[NullAllowed, Export ("profileName")]
		string ProfileName { get; }

		[Internal]
		[NullAllowed, Export ("defaultProfile", ArgumentSemantic.Copy)]
		NSNumber _DefaultProfile { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSetProfileInCar:completion:")]
		void HandleSetProfileInCar (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("confirmSetProfileInCar:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmSetProfileInCar
#endif
		(INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSetProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSetProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Deprecated (PlatformName.iOS, 11, 0, message: "The property doesn't need to be resolved.")]
		[Export ("resolveDefaultProfileForSetProfileInCar:withCompletion:")]
		void ResolveDefaultProfile (INSetProfileInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Introduced (PlatformName.iOS, 10, 2)]
		[Export ("resolveProfileNameForSetProfileInCar:withCompletion:")]
		void ResolveProfileName (INSetProfileInCarIntent intent, Action<INStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetProfileInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetProfileInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetProfileInCarIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetRadioStationIntent {

		[Export ("initWithRadioType:frequency:stationName:channel:presetNumber:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRadioType radioType, [NullAllowed] NSNumber frequency, [NullAllowed] string stationName, [NullAllowed] string channel, [NullAllowed] NSNumber presetNumber);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetRadioStationIntentHandling {

		[Abstract]
		[Export ("handleSetRadioStation:completion:")]
		void HandleSetRadioStation (INSetRadioStationIntent intent, Action<INSetRadioStationIntentResponse> completion);

		[Export ("confirmSetRadioStation:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetRadioStationIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetRadioStationIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetRadioStationIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INSetSeatSettingsInCarIntent {

		[Protected] // allow subclassing
		[Export ("initWithEnableHeating:enableCooling:enableMassage:seat:level:relativeLevelSetting:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber enableHeating, [NullAllowed] NSNumber enableCooling, [NullAllowed] NSNumber enableMassage, INCarSeat seat, [NullAllowed] NSNumber level, INRelativeSetting relativeLevelSetting);

		[Internal]
		[NullAllowed, Export ("enableHeating", ArgumentSemantic.Copy)]
		NSNumber _EnableHeating { get; }

		[Internal]
		[NullAllowed, Export ("enableCooling", ArgumentSemantic.Copy)]
		NSNumber _EnableCooling { get; }

		[Internal]
		[NullAllowed, Export ("enableMassage", ArgumentSemantic.Copy)]
		NSNumber _EnableMassage { get; }

		[Export ("seat", ArgumentSemantic.Assign)]
		INCarSeat Seat { get; }

		[NullAllowed, Export ("level", ArgumentSemantic.Copy)]
		NSNumber Level { get; }

		[Export ("relativeLevelSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeLevelSetting { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INSetSeatSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetSeatSettingsInCar:completion:")]
		void HandleSetSeatSettingsInCar (INSetSeatSettingsInCarIntent intent, Action<INSetSeatSettingsInCarIntentResponse> completion);

		[Export ("confirmSetSeatSettingsInCar:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetSeatSettingsInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetSeatSettingsInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetSeatSettingsInCarIntentResponseCode Code { get; }
	}

	interface IINSpeakable { }

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Protocol]
	interface INSpeakable {

		[Abstract]
		[NullAllowed, Export ("spokenPhrase")]
		string SpokenPhrase { get; }

		[Abstract]
		[NullAllowed, Export ("pronunciationHint")]
		string PronunciationHint { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("vocabularyIdentifier")]
		string VocabularyIdentifier { get; }

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("alternativeSpeakableMatches")]
		IINSpeakable [] AlternativeSpeakableMatches { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'VocabularyIdentifier' instead.")]
#if !XAMCORE_4_0 // Apple made this @optional in iOS 11
		[Abstract]
#endif
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpeakableString : INSpeakable, NSSecureCoding {

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Internal]
		[Export ("initWithVocabularyIdentifier:spokenPhrase:pronunciationHint:")]
		IntPtr InitWithVocabularyIdentifier (string vocabularyIdentifier, string spokenPhrase, [NullAllowed] string pronunciationHint);

		[Internal]
		[Export ("initWithIdentifier:spokenPhrase:pronunciationHint:")]
		IntPtr InitWithIdentifier (string identifier, string spokenPhrase, [NullAllowed] string pronunciationHint);

		[Introduced (PlatformName.iOS, 10, 2)]
		[Introduced (PlatformName.MacOSX, 10, 12, 2, PlatformArchitecture.Arch64)]
		[Export ("initWithSpokenPhrase:")]
		IntPtr Constructor (string spokenPhrase);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntent))]
	interface INStartAudioCallIntent {

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
		[Export ("initWithContacts:")]
		IntPtr Constructor ([NullAllowed] INPerson [] contacts);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("initWithDestinationType:contacts:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCallDestinationType destinationType, [NullAllowed] INPerson [] contacts);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("destinationType", ArgumentSemantic.Assign)]
		INCallDestinationType DestinationType { get; }

		[NullAllowed, Export ("contacts", ArgumentSemantic.Copy)]
		INPerson [] Contacts { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Protocol]
	interface INStartAudioCallIntentHandling {

		[Abstract]
		[Export ("handleStartAudioCall:completion:")]
		void HandleStartAudioCall (INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

		[Export ("confirmStartAudioCall:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmStartAudioCall
#endif
		(INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

		[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
		[Export ("resolveDestinationTypeForStartAudioCall:withCompletion:")]
		void ResolveDestinationType (INStartAudioCallIntent intent, Action<INCallDestinationTypeResolutionResult> completion);

		[Export ("resolveContactsForStartAudioCall:withCompletion:")]
		void ResolveContacts (INStartAudioCallIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartAudioCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INStartAudioCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartAudioCallIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INStartPhotoPlaybackIntent {

		[Export ("initWithDateCreated:locationCreated:albumName:searchTerms:includedAttributes:excludedAttributes:peopleInPhoto:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] CLPlacemark locationCreated, [NullAllowed] string albumName, [NullAllowed] string [] searchTerms, INPhotoAttributeOptions includedAttributes, INPhotoAttributeOptions excludedAttributes, [NullAllowed] INPerson [] peopleInPhoto);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INStartPhotoPlaybackIntentHandling {

		[Abstract]
		[Export ("handleStartPhotoPlayback:completion:")]
		void HandleStartPhotoPlayback (INStartPhotoPlaybackIntent intent, Action<INStartPhotoPlaybackIntentResponse> completion);

		[Export ("confirmStartPhotoPlayback:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartPhotoPlaybackIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INStartPhotoPlaybackIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartPhotoPlaybackIntentResponseCode Code { get; }

		[NullAllowed, Export ("searchResultsCount", ArgumentSemantic.Copy)]
		NSNumber SearchResultsCount { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntent))]
	interface INStartVideoCallIntent {

		[Export ("initWithContacts:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson [] contacts);

		[NullAllowed, Export ("contacts", ArgumentSemantic.Copy)]
		INPerson [] Contacts { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Unavailable (PlatformName.WatchOS)]
	[Protocol]
	interface INStartVideoCallIntentHandling {

		[Abstract]
		[Export ("handleStartVideoCall:completion:")]
		void HandleStartVideoCall (INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

		[Export ("confirmStartVideoCall:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmStartVideoCall
#endif
		(INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

		[Export ("resolveContactsForStartVideoCall:withCompletion:")]
		void ResolveContacts (INStartVideoCallIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartVideoCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INStartVideoCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartVideoCallIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INStartWorkoutIntent {

		[Protected]
		[Export ("initWithWorkoutName:goalValue:workoutGoalUnitType:workoutLocationType:isOpenEnded:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString workoutName, [NullAllowed] NSNumber goalValue, INWorkoutGoalUnitType workoutGoalUnitType, INWorkoutLocationType workoutLocationType, [NullAllowed] NSNumber isOpenEnded);

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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INStartWorkoutIntentHandling {

		[Abstract]
		[Export ("handleStartWorkout:completion:")]
		void HandleStartWorkout (INStartWorkoutIntent intent, Action<INStartWorkoutIntentResponse> completion);

		[Export ("confirmStartWorkout:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INStartWorkoutIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INStartWorkoutIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INStartWorkoutIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	interface INTermsAndConditions : NSSecureCoding, NSCopying {

		[Export ("initWithLocalizedTermsAndConditionsText:privacyPolicyURL:termsAndConditionsURL:")]
		[DesignatedInitializer]
		IntPtr Constructor (string localizedTermsAndConditionsText, [NullAllowed] NSUrl privacyPolicyUrl, [NullAllowed] NSUrl termsAndConditionsUrl);

		[Export ("localizedTermsAndConditionsText")]
		string LocalizedTermsAndConditionsText { get; }

		[NullAllowed, Export ("privacyPolicyURL")]
		NSUrl PrivacyPolicyUrl { get; }

		[NullAllowed, Export ("termsAndConditionsURL")]
		NSUrl TermsAndConditionsUrl { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INVocabulary {

		[Static]
		[Export ("sharedVocabulary")]
		INVocabulary SharedVocabulary { get; }

		[Advice ("This API is not allowed in extensions.")]
		[Export ("setVocabularyStrings:ofType:")]
		void SetVocabularyStrings (NSOrderedSet<NSString> vocabulary, INVocabularyStringType type);

		[Advice ("This API is not allowed in extensions.")]
		[iOS (11,0)]
		[Export ("setVocabulary:ofType:")]
		void SetVocabulary (NSOrderedSet<IINSpeakable> vocabulary, INVocabularyStringType type);

		[Advice ("This API is not allowed in extensions.")]
		[Export ("removeAllVocabularyStrings")]
		void RemoveAllVocabularyStrings ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutGoalUnitTypeResolutionResult bound
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutGoalUnitTypeResolutionResult {

		[Watch (4,0), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedWorkoutGoalUnitType:")]
		INWorkoutGoalUnitTypeResolutionResult SuccessWithResolvedWorkoutGoalUnitType (INWorkoutGoalUnitType resolvedWorkoutGoalUnitType);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutGoalUnitTypeResolutionResult SuccessWithResolvedValue (INWorkoutGoalUnitType resolvedValue);

		[Watch (4,0), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithWorkoutGoalUnitTypeToConfirm:")]
		INWorkoutGoalUnitTypeResolutionResult ConfirmationRequiredWithWorkoutGoalUnitTypeToConfirm (INWorkoutGoalUnitType workoutGoalUnitTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutLocationTypeResolutionResult bound
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutLocationTypeResolutionResult {

		[Watch (4,0), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedWorkoutLocationType:")]
		INWorkoutLocationTypeResolutionResult SuccessWithResolvedWorkoutLocationType (INWorkoutLocationType resolvedWorkoutLocationType);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutLocationTypeResolutionResult SuccessWithResolvedValue (INWorkoutLocationType resolvedValue);

		[Watch (4,0), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithWorkoutLocationTypeToConfirm:")]
		INWorkoutLocationTypeResolutionResult ConfirmationRequiredWithWorkoutLocationTypeToConfirm (INWorkoutLocationType workoutLocationTypeToConfirm);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Category]
	[BaseType (typeof (NSUserActivity))]
	interface NSUserActivity_IntentsAdditions {

		[NullAllowed, Export ("interaction")]
		INInteraction GetInteraction ();
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INActivateCarSignalIntent {

		[DesignatedInitializer]
		[Export ("initWithCarName:signals:")]
		IntPtr Constructor ([NullAllowed] INSpeakableString carName, INCarSignalOptions signals);

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }

		[Export ("signals", ArgumentSemantic.Assign)]
		INCarSignalOptions Signals { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INActivateCarSignalIntentHandling {

		[Abstract]
		[Export ("handleActivateCarSignal:completion:")]
		void HandleActivateCarSignal (INActivateCarSignalIntent intent, Action<INActivateCarSignalIntentResponse> completion);

		[Export ("confirmActivateCarSignal:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INBillDetails : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithBillType:paymentStatus:billPayee:amountDue:minimumDue:lateFee:dueDate:paymentDate:")]
		IntPtr Constructor (INBillType billType, INPaymentStatus paymentStatus, [NullAllowed] INBillPayee billPayee, [NullAllowed] INCurrencyAmount amountDue, [NullAllowed] INCurrencyAmount minimumDue, [NullAllowed] INCurrencyAmount lateFee, [NullAllowed] NSDateComponents dueDate, [NullAllowed] NSDateComponents paymentDate);

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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INBillPayee : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithNickname:number:organizationName:")]
		IntPtr Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, [NullAllowed] INSpeakableString organizationName);

		[Export ("nickname", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString Nickname { get; }

		[Export ("accountNumber"), NullAllowed]
		string AccountNumber { get; }

		[Export ("organizationName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString OrganizationName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INBillTypeResolutionResult {

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedValue:")]
		INBillTypeResolutionResult SuccessWithResolvedValue (INBillType resolvedValue);

		[iOS (11,0), Watch (4,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedBillType:")]
		INBillTypeResolutionResult SuccessWithResolvedBillType (INBillType resolvedBillType);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INBillTypeResolutionResult ConfirmationRequiredWithValueToConfirm (INBillType valueToConfirm);

		[iOS (11,0), Watch (4,0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarSignalOptionsResolutionResult {

		[iOS (11,0), Watch (4,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedCarSignalOptions:")]
		INCarSignalOptionsResolutionResult SuccessWithResolvedCarSignalOptions (INCarSignalOptions resolvedCarSignalOptions);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedValue:")]
		INCarSignalOptionsResolutionResult SuccessWithResolvedValue (INCarSignalOptions resolvedValue);

		[iOS (11,0), Watch (4,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithCarSignalOptionsToConfirm:")]
		INCarSignalOptionsResolutionResult ConfirmationRequiredWithCarSignalOptionsToConfirm (INCarSignalOptions carSignalOptionsToConfirm);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INGetCarLockStatusIntent {

		[DesignatedInitializer]
		[Export ("initWithCarName:")]
		IntPtr Constructor ([NullAllowed] INSpeakableString carName);

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetCarLockStatusIntentHandling {

		[Abstract]
		[Export ("handleGetCarLockStatus:completion:")]
		void HandleGetCarLockStatus (INGetCarLockStatusIntent intent, Action<INGetCarLockStatusIntentResponse> completion);

		[Export ("confirmGetCarLockStatus:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmGetCarLockStatus
#endif
		(INGetCarLockStatusIntent intent, Action<INGetCarLockStatusIntentResponse> completion);

		[Export ("resolveCarNameForGetCarLockStatus:withCompletion:")]
		void ResolveCarName (INGetCarLockStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INGetCarLockStatusIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INGetCarLockStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetCarLockStatusIntentResponseCode Code { get; }

#if false // I wish BindAs was a thing right now
		[BindAs (typeof (bool?))]
#endif
		[Internal]
		[NullAllowed, Export ("locked", ArgumentSemantic.Copy)]
		NSNumber _Locked { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INGetCarPowerLevelStatusIntent {

		[DesignatedInitializer]
		[Export ("initWithCarName:")]
		IntPtr Constructor ([NullAllowed] INSpeakableString carName);

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetCarPowerLevelStatusIntentHandling {

		[Abstract]
		[Export ("handleGetCarPowerLevelStatus:completion:")]
		void HandleGetCarPowerLevelStatus (INGetCarPowerLevelStatusIntent intent, Action<INGetCarPowerLevelStatusIntentResponse> completion);

		[Export ("confirmGetCarPowerLevelStatus:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
		Confirm
#else
		ConfirmGetCarPowerLevelStatus
#endif
		(INGetCarPowerLevelStatusIntent intent, Action<INGetCarPowerLevelStatusIntentResponse> completion);

		[Export ("resolveCarNameForGetCarPowerLevelStatus:withCompletion:")]
		void ResolveCarName (INGetCarPowerLevelStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	// Just to please the generator that at this point does not know the hierarchy
	interface NSUnitLength : NSUnit { }

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INGetCarPowerLevelStatusIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INGetCarPowerLevelStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetCarPowerLevelStatusIntentResponseCode Code { get; }

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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INPayBillIntent {

		[DesignatedInitializer]
		[Export ("initWithBillPayee:fromAccount:transactionAmount:transactionScheduledDate:transactionNote:billType:dueDate:")]
		IntPtr Constructor ([NullAllowed] INBillPayee billPayee, [NullAllowed] INPaymentAccount fromAccount, [NullAllowed] INPaymentAmount transactionAmount, [NullAllowed] INDateComponentsRange transactionScheduledDate, [NullAllowed] string transactionNote, INBillType billType, [NullAllowed] INDateComponentsRange dueDate);

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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INPayBillIntentHandling {

#if XAMCORE_4_0 // Apple added this Protocol to INPaymentsDomainHandling which is a braking change
		[Abstract]
#endif
		[Export ("handlePayBill:completion:")]
		void HandlePayBill (INPayBillIntent intent, Action<INPayBillIntentResponse> completion);

		[Export ("confirmPayBill:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INPayBillIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INPayBillIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INPaymentAccount : NSCopying, NSSecureCoding {

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Please use '.ctor (INSpeakableString, string, INAccountType, INSpeakableString, INBalanceAmount, INBalanceAmount)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Please use '.ctor (INSpeakableString, string, INAccountType, INSpeakableString, INBalanceAmount, INBalanceAmount)' instead.")]
		[Export ("initWithNickname:number:accountType:organizationName:")]
		IntPtr Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, INAccountType accountType, [NullAllowed] INSpeakableString organizationName);

		[Watch (4,0), iOS (11,0)]
		[Export ("initWithNickname:number:accountType:organizationName:balance:secondaryBalance:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, INAccountType accountType, [NullAllowed] INSpeakableString organizationName, [NullAllowed] INBalanceAmount balance, [NullAllowed] INBalanceAmount secondaryBalance);

		[Export ("nickname", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString Nickname { get; }

		[Export ("accountNumber"), NullAllowed]
		string AccountNumber { get; }

		[Export ("accountType", ArgumentSemantic.Assign)]
		INAccountType AccountType { get; }

		[Export ("organizationName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString OrganizationName { get; }

		[Watch (4,0), iOS (11,0)]
		[NullAllowed, Export ("balance", ArgumentSemantic.Copy)]
		INBalanceAmount Balance { get; }

		[Watch (4,0), iOS (11,0)]
		[NullAllowed, Export ("secondaryBalance", ArgumentSemantic.Copy)]
		INBalanceAmount SecondaryBalance { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INPaymentAmount : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithAmountType:amount:")]
		IntPtr Constructor (INAmountType amountType, INCurrencyAmount amount);

		[Export ("amount", ArgumentSemantic.Copy), NullAllowed]
		INCurrencyAmount Amount { get; }

		[Export ("amountType", ArgumentSemantic.Assign)]
		INAmountType AmountType { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPaymentStatusResolutionResult {

		[Watch (4,0), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("successWithResolvedPaymentStatus:")]
		INPaymentStatusResolutionResult SuccessWithResolvedPaymentStatus (INPaymentStatus resolvedPaymentStatus);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Static]
		[Export ("successWithResolvedValue:")]
		INPaymentStatusResolutionResult SuccessWithResolvedValue (INPaymentStatus resolvedValue);

		[Watch (4,0), iOS (11,0)]
		[Internal]
		[Static]
		[Export ("confirmationRequiredWithPaymentStatusToConfirm:")]
		INPaymentStatusResolutionResult ConfirmationRequiredWithPaymentStatusToConfirm (INPaymentStatus paymentStatusToConfirm);

		[Internal]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INSearchForBillsIntent {

		[DesignatedInitializer]
		[Export ("initWithBillPayee:paymentDateRange:billType:status:dueDateRange:")]
		IntPtr Constructor ([NullAllowed] INBillPayee billPayee, [NullAllowed] INDateComponentsRange paymentDateRange, INBillType billType, INPaymentStatus status, [NullAllowed] INDateComponentsRange dueDateRange);

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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INSearchForBillsIntentHandling {

#if XAMCORE_4_0 // Apple added this Protocol to INPaymentsDomainHandling which is a braking change
		[Abstract]
#endif
		[Export ("handleSearchForBills:completion:")]
		void HandleSearch (INSearchForBillsIntent intent, Action<INSearchForBillsIntentResponse> completion);

		[Export ("confirmSearchForBills:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INSearchForBillsIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INSearchForBillsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForBillsIntentResponseCode Code { get; }

		[NullAllowed, Export ("bills", ArgumentSemantic.Copy)]
		INBillDetails [] Bills { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntent))]
	interface INSetCarLockStatusIntent {

		[Protected]
		[DesignatedInitializer]
		[Export ("initWithLocked:carName:")]
		IntPtr Constructor ([NullAllowed] NSNumber locked, [NullAllowed] INSpeakableString carName);

#if false // I wish BindAs was a thing right now
		[BindAs (typeof (bool?))]
#endif
		[Internal]
		[Export ("locked", ArgumentSemantic.Copy), NullAllowed]
		NSNumber _Locked { get; }

		[Export ("carName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString CarName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INSetCarLockStatusIntentHandling {

		[Abstract]
		[Export ("handleSetCarLockStatus:completion:")]
		void HandleSetCarLockStatus (INSetCarLockStatusIntent intent, Action<INSetCarLockStatusIntentResponse> completion);

		[Export ("confirmSetCarLockStatus:completion:")]
		void
#if XAMCORE_4_0 // Follow Swift's naming, fixes bug https://bugzilla.xamarin.com/show_bug.cgi?id=59164
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

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INSetCarLockStatusIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INSetCarLockStatusIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetCarLockStatusIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 3)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResponse))]
	interface INActivateCarSignalIntentResponse {

		[DesignatedInitializer]
		[Export ("initWithCode:userActivity:")]
		IntPtr Constructor (INActivateCarSignalIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INActivateCarSignalIntentResponseCode Code { get; }

		[Export ("signals")]
		INCarSignalOptions Signals { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	interface INAddTasksIntent {

		[Export ("initWithTargetTaskList:taskTitles:spatialEventTrigger:temporalEventTrigger:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INTaskList targetTaskList, [NullAllowed] INSpeakableString[] taskTitles, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[NullAllowed, Export ("targetTaskList", ArgumentSemantic.Copy)]
		INTaskList TargetTaskList { get; }

		[NullAllowed, Export ("taskTitles", ArgumentSemantic.Copy)]
		INSpeakableString [] TaskTitles { get; }

		[NullAllowed, Export ("spatialEventTrigger", ArgumentSemantic.Copy)]
		INSpatialEventTrigger SpatialEventTrigger { get; }

		[NullAllowed, Export ("temporalEventTrigger", ArgumentSemantic.Copy)]
		INTemporalEventTrigger TemporalEventTrigger { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Protocol]
	interface INAddTasksIntentHandling {

		[Abstract]
		[Export ("handleAddTasks:completion:")]
		void HandleAddTasks (INAddTasksIntent intent, Action<INAddTasksIntentResponse> completion);

		[Export ("confirmAddTasks:completion:")]
		void Confirm (INAddTasksIntent intent, Action<INAddTasksIntentResponse> completion);

		[Export ("resolveTargetTaskListForAddTasks:withCompletion:")]
		void ResolveTargetTaskList (INAddTasksIntent intent, Action<INTaskListResolutionResult> completion);

		[Export ("resolveTaskTitlesForAddTasks:withCompletion:")]
		void ResolveTaskTitles (INAddTasksIntent intent, Action<INSpeakableStringResolutionResult []> completion);

		[Export ("resolveSpatialEventTriggerForAddTasks:withCompletion:")]
		void ResolveSpatialEventTrigger (INAddTasksIntent intent, Action<INSpatialEventTriggerResolutionResult> completion);

		[Export ("resolveTemporalEventTriggerForAddTasks:withCompletion:")]
		void ResolveTemporalEventTrigger (INAddTasksIntent intent, Action<INTemporalEventTriggerResolutionResult> completion);
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INAddTasksIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INAddTasksIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INAddTasksIntentResponseCode Code { get; }

		[NullAllowed, Export ("modifiedTaskList", ArgumentSemantic.Copy)]
		INTaskList ModifiedTaskList { get; set; }

		[NullAllowed, Export ("addedTasks", ArgumentSemantic.Copy)]
		INTask [] AddedTasks { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	interface INAppendToNoteIntent {

		[Export ("initWithTargetNote:content:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INNote targetNote, [NullAllowed] INNoteContent content);

		[NullAllowed, Export ("targetNote", ArgumentSemantic.Copy)]
		INNote TargetNote { get; }

		[NullAllowed, Export ("content", ArgumentSemantic.Copy)]
		INNoteContent Content { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INAppendToNoteIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INAppendToNoteIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INAppendToNoteIntentResponseCode Code { get; }

		[NullAllowed, Export ("note", ArgumentSemantic.Copy)]
		INNote Note { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INBalanceAmount : NSCopying, NSSecureCoding {

		[Export ("initWithAmount:balanceType:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSDecimalNumber amount, INBalanceType balanceType);

		[Export ("initWithAmount:currencyCode:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSDecimalNumber amount, string currencyCode);

		[NullAllowed, Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; }

		[Export ("balanceType", ArgumentSemantic.Assign)]
		INBalanceType BalanceType { get; }

		[NullAllowed, Export ("currencyCode")]
		string CurrencyCode { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
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
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCallRecord : NSCopying, NSSecureCoding {

		[Export ("initWithIdentifier:dateCreated:caller:callRecordType:callCapability:callDuration:unseen:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, [NullAllowed] NSDate dateCreated, [NullAllowed] INPerson caller, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed] NSNumber callDuration, [NullAllowed] NSNumber unseen);

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		NSDate DateCreated { get; }

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
	}

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
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
	}

	[NoWatch, NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INCancelRideIntent {

		[Export ("initWithRideIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string rideIdentifier);

		[Export ("rideIdentifier")]
		string RideIdentifier { get; }
	}

	[NoWatch, NoMac, iOS (11,0)]
	[Protocol]
	interface INCancelRideIntentHandling {

		[Abstract]
		[Export ("handleCancelRide:completion:")]
		void HandleCancelRide (INCancelRideIntent intent, Action<INCancelRideIntentResponse> completion);

		[Export ("confirmCancelRide:completion:")]
		void Confirm (INCancelRideIntent intent, Action<INCancelRideIntentResponse> completion);
	}

	[NoWatch, NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCancelRideIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCancelRideIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCancelRideIntentResponseCode Code { get; }

		[NullAllowed, Export ("cancellationFee", ArgumentSemantic.Assign)]
		INCurrencyAmount CancellationFee { get; set; }

		[NullAllowed, Export ("cancellationFeeThreshold", ArgumentSemantic.Assign)]
		NSDateComponents CancellationFeeThreshold { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INCreateNoteIntent {

		[Export ("initWithTitle:content:groupName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] INNoteContent content, [NullAllowed] INSpeakableString groupName);

		[NullAllowed, Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[NullAllowed, Export ("content", ArgumentSemantic.Copy)]
		INNoteContent Content { get; }

		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCreateNoteIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCreateNoteIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCreateNoteIntentResponseCode Code { get; }

		[NullAllowed, Export ("createdNote", ArgumentSemantic.Copy)]
		INNote CreatedNote { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INCreateTaskListIntent {

		[Export ("initWithTitle:taskTitles:groupName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] INSpeakableString [] taskTitles, [NullAllowed] INSpeakableString groupName);

		[NullAllowed, Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[NullAllowed, Export ("taskTitles", ArgumentSemantic.Copy)]
		INSpeakableString [] TaskTitles { get; }

		[NullAllowed, Export ("groupName", ArgumentSemantic.Copy)]
		INSpeakableString GroupName { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INCreateTaskListIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCreateTaskListIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INCreateTaskListIntentResponseCode Code { get; }

		[NullAllowed, Export ("createdTaskList", ArgumentSemantic.Copy)]
		INTaskList CreatedTaskList { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INGetVisualCodeIntent {

		[Export ("initWithVisualCodeType:")]
		[DesignatedInitializer]
		IntPtr Constructor (INVisualCodeType visualCodeType);

		[Export ("visualCodeType", ArgumentSemantic.Assign)]
		INVisualCodeType VisualCodeType { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INGetVisualCodeIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INGetVisualCodeIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetVisualCodeIntentResponseCode Code { get; }

		[NullAllowed, Export ("visualCodeImage", ArgumentSemantic.Copy)]
		INImage VisualCodeImage { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INNoteContent))]
	interface INImageNoteContent : NSSecureCoding, NSCopying {

		[Export ("initWithImage:")]
		IntPtr Constructor (INImage image);

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		INImage Image { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface INNote : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:contents:groupName:createdDateComponents:modifiedDateComponents:identifier:")]
		IntPtr Constructor (INSpeakableString title, INNoteContent [] contents, [NullAllowed] INSpeakableString groupName, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier);

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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface INNoteContent : NSSecureCoding, NSCopying {
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INNotebookItemTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedNotebookItemType:")]
		INNotebookItemTypeResolutionResult GetSuccess (INNotebookItemType resolvedNotebookItemType);

		[Static]
		[Export ("disambiguationWithNotebookItemTypesToDisambiguate:")]
		INNotebookItemTypeResolutionResult GetDisambiguation (NSNumber[] notebookItemTypesToDisambiguate);

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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INParameter : NSSecureCoding {

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

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRecurrenceRule : NSCopying, NSSecureCoding {

		[Export ("initWithInterval:frequency:")]
		IntPtr Constructor (nuint interval, INRecurrenceFrequency frequency);

		[Export ("interval")]
		nuint Interval { get; }

		[Export ("frequency")]
		INRecurrenceFrequency Frequency { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INCurrencyAmountResolutionResult))]
	[DisableDefaultCtor]
	interface INRequestPaymentCurrencyAmountResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INRequestPaymentCurrencyAmountResolutionResult GetUnsupported (INRequestPaymentCurrencyAmountUnsupportedReason reason);

		[Export ("initWithCurrencyAmountResolutionResult:")]
		IntPtr Constructor (INCurrencyAmountResolutionResult currencyAmountResolutionResult);

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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INRequestPaymentPayerResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INRequestPaymentPayerResolutionResult GetUnsupported (INRequestPaymentPayerUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		IntPtr Constructor (INPersonResolutionResult personResolutionResult);

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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	interface INSearchForAccountsIntent {

		[Export ("initWithAccountNickname:accountType:organizationName:requestedBalanceType:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString accountNickname, INAccountType accountType, [NullAllowed] INSpeakableString organizationName, INBalanceType requestedBalanceType);

		[NullAllowed, Export ("accountNickname", ArgumentSemantic.Copy)]
		INSpeakableString AccountNickname { get; }

		[Export ("accountType", ArgumentSemantic.Assign)]
		INAccountType AccountType { get; }

		[NullAllowed, Export ("organizationName", ArgumentSemantic.Copy)]
		INSpeakableString OrganizationName { get; }

		[Export ("requestedBalanceType", ArgumentSemantic.Assign)]
		INBalanceType RequestedBalanceType { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForAccountsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSearchForAccountsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSearchForAccountsIntentResponseCode Code { get; }

		[NullAllowed, Export ("accounts", ArgumentSemantic.Copy)]
		INPaymentAccount [] Accounts { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	interface INSearchForNotebookItemsIntent {

		[Deprecated (PlatformName.WatchOS, 4, 2, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Deprecated (PlatformName.iOS, 11, 2, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:")]
		IntPtr Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType);

		[Watch (4,2), iOS (11,2)]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:notebookItemIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType, [NullAllowed] string notebookItemIdentifier);

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

		[Watch (4,2), iOS (11,2)]
		[NullAllowed, Export ("notebookItemIdentifier")]
		string NotebookItemIdentifier { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSearchForNotebookItemsIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSearchForNotebookItemsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

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

	[Watch (4,0), Mac (10,13, onlyOn64:true), iOS (11,0)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INSendMessageRecipientResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSendMessageRecipientResolutionResult GetUnsupported (INSendMessageRecipientUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		IntPtr Constructor (INPersonResolutionResult personResolutionResult);

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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INCurrencyAmountResolutionResult))]
	[DisableDefaultCtor]
	interface INSendPaymentCurrencyAmountResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSendPaymentCurrencyAmountResolutionResult GetUnsupported (INSendPaymentCurrencyAmountUnsupportedReason reason);

		[Export ("initWithCurrencyAmountResolutionResult:")]
		IntPtr Constructor (INCurrencyAmountResolutionResult currencyAmountResolutionResult);

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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INPersonResolutionResult))]
	[DisableDefaultCtor]
	interface INSendPaymentPayeeResolutionResult {

		[Static]
		[Export ("unsupportedForReason:")]
		INSendPaymentPayeeResolutionResult GetUnsupported (INSendPaymentPayeeUnsupportedReason reason);

		[Export ("initWithPersonResolutionResult:")]
		IntPtr Constructor (INPersonResolutionResult personResolutionResult);

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
	}

	[NoWatch, NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INSendRideFeedbackIntent {

		[Export ("initWithRideIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string rideIdentifier);

		[Export ("rideIdentifier")]
		string RideIdentifier { get; }

		[NullAllowed, Export ("rating", ArgumentSemantic.Copy)]
		NSNumber Rating { get; set; }

		[NullAllowed, Export ("tip", ArgumentSemantic.Copy)]
		INCurrencyAmount Tip { get; set; }
	}

	[NoWatch, NoMac, iOS (11,0)]
	[Protocol]
	interface INSendRideFeedbackIntentHandling {

		[Abstract]
		[Export ("handleSendRideFeedback:completion:")]
		void HandleSendRideFeedback (INSendRideFeedbackIntent sendRideFeedbackintent, Action<INSendRideFeedbackIntentResponse> completion);

		[Export ("confirmSendRideFeedback:completion:")]
		void Confirm (INSendRideFeedbackIntent sendRideFeedbackIntent, Action<INSendRideFeedbackIntentResponse> completion);
	}

	[NoWatch, NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSendRideFeedbackIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSendRideFeedbackIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSendRideFeedbackIntentResponseCode Code { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	interface INSetTaskAttributeIntent {

		[Export ("initWithTargetTask:status:spatialEventTrigger:temporalEventTrigger:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INTask targetTask, INTaskStatus status, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[NullAllowed, Export ("targetTask", ArgumentSemantic.Copy)]
		INTask TargetTask { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INTaskStatus Status { get; }

		[NullAllowed, Export ("spatialEventTrigger", ArgumentSemantic.Copy)]
		INSpatialEventTrigger SpatialEventTrigger { get; }

		[NullAllowed, Export ("temporalEventTrigger", ArgumentSemantic.Copy)]
		INTemporalEventTrigger TemporalEventTrigger { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[Protocol]
	interface INSetTaskAttributeIntentHandling {

		[Abstract]
		[Export ("handleSetTaskAttribute:completion:")]
		void HandleSetTaskAttribute (INSetTaskAttributeIntent intent, Action<INSetTaskAttributeIntentResponse> completion);

		[Export ("confirmSetTaskAttribute:completion:")]
		void Confirm (INSetTaskAttributeIntent intent, Action<INSetTaskAttributeIntentResponse> completion);

		[Export ("resolveTargetTaskForSetTaskAttribute:withCompletion:")]
		void ResolveTargetTask (INSetTaskAttributeIntent intent, Action<INTaskResolutionResult> completion);

		[Export ("resolveStatusForSetTaskAttribute:withCompletion:")]
		void ResolveStatus (INSetTaskAttributeIntent intent, Action<INTaskStatusResolutionResult> completion);

		[Export ("resolveSpatialEventTriggerForSetTaskAttribute:withCompletion:")]
		void ResolveSpatialEventTrigger (INSetTaskAttributeIntent intent, Action<INSpatialEventTriggerResolutionResult> completion);

		[Export ("resolveTemporalEventTriggerForSetTaskAttribute:withCompletion:")]
		void ResolveTemporalEventTrigger (INSetTaskAttributeIntent intent, Action<INTemporalEventTriggerResolutionResult> completion);
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetTaskAttributeIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetTaskAttributeIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetTaskAttributeIntentResponseCode Code { get; }

		[NullAllowed, Export ("modifiedTask", ArgumentSemantic.Copy)]
		INTask ModifiedTask { get; set; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpatialEventTrigger : NSSecureCoding {

		[Export ("initWithPlacemark:event:")]
		IntPtr Constructor (CLPlacemark placemark, INSpatialEvent @event);

		[Export ("placemark")]
		CLPlacemark Placemark { get; }

		[Export ("event")]
		INSpatialEvent Event { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTask : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:status:taskType:spatialEventTrigger:temporalEventTrigger:createdDateComponents:modifiedDateComponents:identifier:")]
		IntPtr Constructor (INSpeakableString title, INTaskStatus status, INTaskType taskType, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier);

		[Export ("title", ArgumentSemantic.Copy)]
		INSpeakableString Title { get; }

		[Export ("status")]
		INTaskStatus Status { get; }

		[Export ("taskType")]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTaskList : NSCopying, NSSecureCoding {

		[Export ("initWithTitle:tasks:groupName:createdDateComponents:modifiedDateComponents:identifier:")]
		IntPtr Constructor (INSpeakableString title, INTask [] tasks, [NullAllowed] INSpeakableString groupName, [NullAllowed] NSDateComponents createdDateComponents, [NullAllowed] NSDateComponents modifiedDateComponents, [NullAllowed] string identifier);

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

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTemporalEventTrigger : NSCopying, NSSecureCoding {

		[Export ("initWithDateComponentsRange:")]
		IntPtr Constructor (INDateComponentsRange dateComponentsRange);

		[Export ("dateComponentsRange")]
		INDateComponentsRange DateComponentsRange { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INNoteContent))]
	[DisableDefaultCtor]
	interface INTextNoteContent : NSSecureCoding, NSCopying {

		[Export ("initWithText:")]
		IntPtr Constructor (string text);

		[NullAllowed, Export ("text")]
		string Text { get; }
	}

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INTransferMoneyIntent {

		[Export ("initWithFromAccount:toAccount:transactionAmount:transactionScheduledDate:transactionNote:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPaymentAccount fromAccount, [NullAllowed] INPaymentAccount toAccount, [NullAllowed] INPaymentAmount transactionAmount, [NullAllowed] INDateComponentsRange transactionScheduledDate, [NullAllowed] string transactionNote);

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

	[Watch (4,0), NoMac, iOS (11,0)]
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

	[Watch (4,0), NoMac, iOS (11,0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INTransferMoneyIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INTransferMoneyIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

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

	[Watch (4,0), NoMac, iOS (11,0)]
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
	}

}
#endif // XAMCORE_2_0
