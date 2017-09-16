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
		Received
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INCancelWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout
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
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout
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
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Native]
	public enum INMessageAttribute : nint {
		Unknown = 0,
		Read,
		Unread,
		Flagged,
		Unflagged
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
		Unflagged = (1 << 3)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INPauseWorkoutIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout
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
		FailureNoBankAccount
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRequestRideIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
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
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch,
		FailureNoMatchingWorkout
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
		FailureMessageServiceNotAvailable
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
		FailureNoBankAccount
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
		FailureNoMatchingWorkout
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
		void ConfirmBookRestaurantReservation (INBookRestaurantReservationIntent intent, Action<INBookRestaurantReservationIntentResponse> completion);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INCallRecordTypeResolutionResult GetSuccess (INCallRecordType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCallRecordTypeResolutionResult GetConfirmationRequired (INCallRecordType valueToConfirm);

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
		void ConfirmCancelWorkout (INCancelWorkoutIntent intent, Action<INCancelWorkoutIntentResponse> completion);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INCarAirCirculationModeResolutionResult GetSuccess (INCarAirCirculationMode resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarAirCirculationModeResolutionResult GetConfirmationRequired (INCarAirCirculationMode valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INCarAudioSourceResolutionResult GetSuccess (INCarAudioSource resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarAudioSourceResolutionResult GetConfirmationRequired (INCarAudioSource valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INCarDefrosterResolutionResult GetSuccess (INCarDefroster resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarDefrosterResolutionResult GetConfirmationRequired (INCarDefroster valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INCarSeatResolutionResult GetSuccess (INCarSeat resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarSeatResolutionResult GetConfirmationRequired (INCarSeat valueToConfirm);

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
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSDateComponents startDateComponents, [NullAllowed] NSDateComponents endDateComponents);

		[NullAllowed, Export ("startDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents StartDateComponents { get; }

		[NullAllowed, Export ("endDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents EndDateComponents { get; }
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
	interface INPaymentsDomainHandling : INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling {
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
	interface INRidesharingDomainHandling : INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling {
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
		void ConfirmEndWorkout (INEndWorkoutIntent intent, Action<INEndWorkoutIntentResponse> completion);

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
		void ConfirmAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

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
		void ConfirmAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

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
		void ConfirmRestaurantGuest (INGetRestaurantGuestIntent guestIntent, Action<INGetRestaurantGuestIntentResponse> completion);
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
	interface INGetRideStatusIntent {
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
		void ConfirmRideStatus (INGetRideStatusIntent intent, Action<INGetRideStatusIntentResponse> completion);
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
		void ConfirmUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

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
		void ConfirmListRideOptions (INListRideOptionsIntent intent, Action<INListRideOptionsIntentResponse> completion);

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

		[Export ("initWithIdentifier:content:dateSent:sender:recipients:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients);

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("content")]
		string Content { get; }

		[NullAllowed, Export ("dateSent", ArgumentSemantic.Copy)]
		NSDate DateSent { get; }

		[NullAllowed, Export ("sender", ArgumentSemantic.Copy)]
		INPerson Sender { get; }

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeOptionsResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeOptionsResolutionResult GetSuccess (INMessageAttributeOptions resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INMessageAttributeOptionsResolutionResult GetConfirmationRequired (INMessageAttributeOptions valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeResolutionResult GetSuccess (INMessageAttribute resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INMessageAttributeResolutionResult GetConfirmationRequired (INMessageAttribute valueToConfirm);

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
		void ConfirmPauseWorkout (INPauseWorkoutIntent intent, Action<INPauseWorkoutIntentResponse> completion);

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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPersonHandle : NSCopying, NSSecureCoding {

		[Export ("value")]
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
		IntPtr Constructor (string value, INPersonHandleType type, [NullAllowed] NSString stringLabel);

		[Export ("initWithValue:type:")]
		IntPtr Constructor (string value, INPersonHandleType type);
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

		[Static]
		[Export ("successWithResolvedValue:")]
		INRadioTypeResolutionResult GetSuccess (INRadioType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INRadioTypeResolutionResult GetConfirmationRequired (INRadioType valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INRelativeReferenceResolutionResult GetSuccess (INRelativeReference resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INRelativeReferenceResolutionResult GetConfirmationRequired (INRelativeReference valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INRelativeSettingResolutionResult GetSuccess (INRelativeSetting resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INRelativeSettingResolutionResult GetConfirmationRequired (INRelativeSetting valueToConfirm);

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
		void ConfirmRequestPayment (INRequestPaymentIntent intent, Action<INRequestPaymentIntentResponse> completion);

		[Export ("resolvePayerForRequestPayment:withCompletion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[Export ("resolveCurrencyAmountForRequestPayment:withCompletion:")]
		void ResolveCurrencyAmount (INRequestPaymentIntent intent, Action<INCurrencyAmountResolutionResult> completion);

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
		void ConfirmRequestRide (INRequestRideIntent intent, Action<INRequestRideIntentResponse> completion);

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
		void ConfirmResumeWorkout (INResumeWorkoutIntent intent, Action<INResumeWorkoutIntentResponse> completion);

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

		[Export ("outstanding")]
		bool Outstanding { [Bind ("isOutstanding")] get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRideDriver bound
	[BaseType (typeof (INPerson))]
	[DisableDefaultCtor] // xcode 8.2 beta 1 -> NSInvalidArgumentException Reason: *** -[__NSPlaceholderDictionary initWithObjects:forKeys:count:]: attempt to insert nil object from objects[1]
	interface INRideDriver : NSCopying, NSSecureCoding {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:rating:phoneNumber:")]
		[Deprecated (PlatformName.iOS, 10,2, message:"Use the overload signature instead")]
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

		[Deprecated (PlatformName.iOS, 10,2, message:"Use ProfileName instead")]
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
		void ConfirmSaveProfileInCar (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

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

		[Export ("initWithCallType:dateCreated:recipient:callCapabilities:")]
		[DesignatedInitializer]
		IntPtr Constructor (INCallRecordType callType, [NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities);

		[Export ("callType", ArgumentSemantic.Assign)]
		INCallRecordType CallType { get; }

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		INDateComponentsRange DateCreated { get; }

		[NullAllowed, Export ("recipient", ArgumentSemantic.Copy)]
		INPerson Recipient { get; }

		[Export ("callCapabilities", ArgumentSemantic.Assign)]
		INCallCapabilityOptions CallCapabilities { get; }
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
		void ConfirmSearchCallHistory (INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Export ("resolveCallTypeForSearchCallHistory:withCompletion:")]
		void ResolveCallType (INSearchCallHistoryIntent intent, Action<INCallRecordTypeResolutionResult> completion);

		[Export ("resolveDateCreatedForSearchCallHistory:withCompletion:")]
		void ResolveDateCreated (INSearchCallHistoryIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveRecipientForSearchCallHistory:withCompletion:")]
		void ResolveRecipient (INSearchCallHistoryIntent intent, Action<INPersonResolutionResult> completion);
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (INIntent))]
	interface INSearchForMessagesIntent {

		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:groupNames:")]
		[DesignatedInitializer]
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

		[NullAllowed, Export ("groupNames", ArgumentSemantic.Copy)]
		string [] GroupNames { get; }

		[Export ("groupNamesOperator", ArgumentSemantic.Assign)]
		INConditionalOperator GroupNamesOperator { get; }
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
		void ConfirmSearchForMessages (INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion);

		[Export ("resolveRecipientsForSearchForMessages:withCompletion:")]
		void ResolveRecipients (INSearchForMessagesIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveSendersForSearchForMessages:withCompletion:")]
		void ResolveSenders (INSearchForMessagesIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveAttributesForSearchForMessages:withCompletion:")]
		void ResolveAttributes (INSearchForMessagesIntent intent, Action<INMessageAttributeOptionsResolutionResult> completion);

		[Export ("resolveDateTimeRangeForSearchForMessages:withCompletion:")]
		void ResolveDateTimeRange (INSearchForMessagesIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveGroupNames (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);
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
		void ConfirmSearchForPhotos (INSearchForPhotosIntent intent, Action<INSearchForPhotosIntentResponse> completion);

		[Export ("resolveDateCreatedForSearchForPhotos:withCompletion:")]
		void ResolveDateCreated (INSearchForPhotosIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveLocationCreatedForSearchForPhotos:withCompletion:")]
		void ResolveLocationCreated (INSearchForPhotosIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveAlbumNameForSearchForPhotos:withCompletion:")]
		void ResolveAlbumName (INSearchForPhotosIntent intent, Action<INStringResolutionResult> completion);

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

		[Export ("initWithRecipients:content:groupName:serviceName:sender:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] string groupName, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }

		[NullAllowed, Export ("content")]
		string Content { get; }

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
		void ConfirmSendMessage (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

		[Export ("resolveRecipientsForSendMessage:withCompletion:")]
		void ResolveRecipients (INSendMessageIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveContentForSendMessage:withCompletion:")]
		void ResolveContent (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolveGroupNameForSendMessage:withCompletion:")]
		void ResolveGroupName (INSendMessageIntent intent, Action<INStringResolutionResult> completion);
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
		void ConfirmSendPayment (INSendPaymentIntent intent, Action<INSendPaymentIntentResponse> completion);

		[Export ("resolvePayeeForSendPayment:withCompletion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[Export ("resolveCurrencyAmountForSendPayment:withCompletion:")]
		void ResolveCurrencyAmount (INSendPaymentIntent intent, Action<INCurrencyAmountResolutionResult> completion);

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
		void ConfirmSetAudioSourceInCar (INSetAudioSourceInCarIntent intent, Action<INSetAudioSourceInCarIntentResponse> completion);

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
		void ConfirmSetClimateSettingsInCar (INSetClimateSettingsInCarIntent intent, Action<INSetClimateSettingsInCarIntentResponse> completion);

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
		void ConfirmSetDefrosterSettingsInCar (INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

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
		void ConfirmSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

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

		[Deprecated (PlatformName.iOS, 10, 2, message: "Use ProfileName instead")]
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
		void ConfirmSetProfileInCar (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSetProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSetProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

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
		void ConfirmSetRadioStation (INSetRadioStationIntent intent, Action<INSetRadioStationIntentResponse> completion);

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
		void ConfirmSetSeatSettingsInCar (INSetSeatSettingsInCarIntent intent, Action<INSetSeatSettingsInCarIntentResponse> completion);

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

		[Abstract]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpeakableString : INSpeakable {

		[Export ("initWithIdentifier:spokenPhrase:pronunciationHint:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, string spokenPhrase, [NullAllowed] string pronunciationHint);

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

		[Export ("initWithContacts:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INPerson [] contacts);

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
		void ConfirmStartAudioCall (INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

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
		void ConfirmStartPhotoPlayback (INStartPhotoPlaybackIntent intent, Action<INStartPhotoPlaybackIntentResponse> completion);

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
		void ConfirmStartVideoCall (INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

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
		void ConfirmStartWorkout (INStartWorkoutIntent intent, Action<INStartWorkoutIntentResponse> completion);

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

		[Advice ("This API is not allowed in extensions")]
		[Export ("setVocabularyStrings:ofType:")]
		void SetVocabularyStrings (NSOrderedSet<NSString> vocabulary, INVocabularyStringType type);

		[Advice ("This API is not allowed in extensions")]
		[Export ("removeAllVocabularyStrings")]
		void RemoveAllVocabularyStrings ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.WatchOS, 3, 2)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutGoalUnitTypeResolutionResult bound
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutGoalUnitTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutGoalUnitTypeResolutionResult GetSuccess (INWorkoutGoalUnitType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INWorkoutGoalUnitTypeResolutionResult GetConfirmationRequired (INWorkoutGoalUnitType valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutLocationTypeResolutionResult GetSuccess (INWorkoutLocationType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INWorkoutLocationTypeResolutionResult GetConfirmationRequired (INWorkoutLocationType valueToConfirm);

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
		void ConfirmActivateCarSignal (INActivateCarSignalIntent intent, Action<INActivateCarSignalIntentResponse> completion);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INBillTypeResolutionResult GetSuccess (INBillType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INBillTypeResolutionResult GetConfirmationRequired (INBillType valueToConfirm);

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

		[Static]
		[Export ("successWithResolvedValue:")]
		INCarSignalOptionsResolutionResult GetSuccess (INCarSignalOptions resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCarSignalOptionsResolutionResult GetConfirmationRequired (INCarSignalOptions valueToConfirm);

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
		void ConfirmGetCarLockStatus (INGetCarLockStatusIntent intent, Action<INGetCarLockStatusIntentResponse> completion);

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
		void ConfirmGetCarPowerLevelStatus (INGetCarPowerLevelStatusIntent intent, Action<INGetCarPowerLevelStatusIntentResponse> completion);

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
		void ConfirmPayBill (INPayBillIntent intent, Action<INPayBillIntentResponse> completion);

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

		[DesignatedInitializer]
		[Export ("initWithNickname:number:accountType:organizationName:")]
		IntPtr Constructor (INSpeakableString nickname, [NullAllowed] string accountNumber, INAccountType accountType, [NullAllowed] INSpeakableString organizationName);

		[Export ("nickname", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString Nickname { get; }

		[Export ("accountNumber"), NullAllowed]
		string AccountNumber { get; }

		[Export ("accountType", ArgumentSemantic.Assign)]
		INAccountType AccountType { get; }

		[Export ("organizationName", ArgumentSemantic.Copy), NullAllowed]
		INSpeakableString OrganizationName { get; }
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

		[Static]
		[Export ("successWithResolvedValue:")]
		INPaymentStatusResolutionResult GetSuccess (INPaymentStatus resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INPaymentStatusResolutionResult GetConfirmationRequired (INPaymentStatus valueToConfirm);

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
		void ConfirmSearch (INSearchForBillsIntent intent, Action<INSearchForBillsIntentResponse> completion);

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
		void ConfirmSetCarLockStatus (INSetCarLockStatusIntent intent, Action<INSetCarLockStatusIntentResponse> completion);

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
}
#endif // XAMCORE_2_0
