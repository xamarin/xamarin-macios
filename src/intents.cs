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

	/// <summary>Flagging enumeration of the types of calls supported by the device.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INCallCapabilityOptions : ulong {
		AudioCall = (1 << 0),
		VideoCall = (1 << 1)
	}

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
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Ringing,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		InProgress,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		OnHold,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INCancelWorkoutIntent" />.</summary>
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
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[MacCatalyst (13, 1)]
		Success,
	}

	/// <summary>Enumerates how air is circulated through the car.</summary>
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

	/// <summary>Enumerates inputs to the car sound system.</summary>
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

	/// <summary>Enumerates defroster locations.</summary>
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

	/// <summary>Enumerates car seat positions.</summary>
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

	/// <summary>Enumerates operators to be used with search predicates.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INConditionalOperator : long {
		All = 0,
		Any,
		None
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INEndWorkoutIntent" />.</summary>
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
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INGetRideStatusIntent" />.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INGetRideStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INListRideOptionsIntent" />.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INListRideOptionsIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
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

	/// <summary>Enumerates the attributes of a message.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMessageAttribute : long {
		Unknown = 0,
		Read,
		Unread,
		Flagged,
		Unflagged,
		[NoMac]
		[MacCatalyst (13, 1)]
		Played,
	}

	/// <summary>Enumerates the statuses of a message.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INMessageAttributeOptions : ulong {
		Read = (1 << 0),
		Unread = (1 << 1),
		Flagged = (1 << 2),
		Unflagged = (1 << 3),
		[NoMac]
		[MacCatalyst (13, 1)]
		Played = (1UL << 4),
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INPauseWorkoutIntent" />.</summary>
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
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[MacCatalyst (13, 1)]
		Success,
	}

	/// <summary>Enumeates payment categories.</summary>
	[NoTV]
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

	/// <summary>Enumerates the states of a payment.</summary>
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

	/// <summary>Enumerates the source of the data for a <see cref="T:Intents.INPerson" /> (see <see cref="P:Intents.INPerson.SuggestionType" />).</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPersonSuggestionType : long {
		[MacCatalyst (13, 1)]
		None = 0,
		SocialProfile = 1,
		InstantMessageAddress,
	}

	/// <summary>Enumerates various photo options.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
		[MacCatalyst (13, 1)]
		PortraitPhoto = (1uL << 24),
		[MacCatalyst (13, 1)]
		LivePhoto = (1uL << 25),
		[MacCatalyst (13, 1)]
		LoopPhoto = (1uL << 26),
		[MacCatalyst (13, 1)]
		BouncePhoto = (1uL << 27),
		[MacCatalyst (13, 1)]
		LongExposurePhoto = (1uL << 28),
	}

	/// <summary>Enumerates the types of radio supported by Intents.</summary>
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

	/// <summary>Enumerates a qualitative sequential movement.</summary>
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

	/// <summary>Enumerates qualitative increases or decreased quantities.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INRequestPaymentIntent" />.</summary>
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
		[NoMac]
		[MacCatalyst (13, 1)]
		FailureTermsAndConditionsAcceptanceRequired,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INRequestRideIntent" />.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestRideIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchNoServiceInArea,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable,
		FailureRequiringAppLaunchPreviousRideNeedsCompletion,
		[iOS (13, 0)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INResumeWorkoutIntent" />.</summary>
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
		[MacCatalyst (13, 1)]
		HandleInApp,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[MacCatalyst (13, 1)]
		Success,
	}

	/// <summary>Enumerates the state of a ride in a vehicle.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSaveProfileInCarIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSearchCallHistoryIntent" />.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSearchForMessagesIntent" />.</summary>
	[NoMac]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSearchForPhotosIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSendMessageIntentResponse" />.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSendPaymentIntent" />.</summary>
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
		[MacCatalyst (13, 1)]
		FailureTermsAndConditionsAcceptanceRequired,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetAudioSourceInCarIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetClimateSettingsInCarIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetDefrosterSettingsInCarIntentResponseCode" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetMessageAttributeIntent" />.</summary>
	[NoMac]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetProfileInCarIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetRadioStationIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetSeatSettingsInCarIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INStartAudioCallIntent" />.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponseCode' instead.")]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INStartPhotoPlaybackIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INStartVideoCallIntent" />.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponseCode' instead.")]
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INStartWorkoutIntent" />.</summary>
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
		HandleInApp = 7,
		[Advice ("The numerical value for this constant was different in iOS 11 and earlier iOS versions (it was 6, which is now defined as 'HandleInApp').")]
		[MacCatalyst (13, 1)]
		Success = 8,
	}

	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Enumerates the kind of goal the workout is striving for.</summary>
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

	/// <summary>Enumerates where the workout is occurring.</summary>
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INWorkoutLocationType bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INWorkoutLocationType : long {
		Unknown = 0,
		Outdoor,
		Indoor
	}

	/// <summary>Enumerates the types of values that are associated with a <see cref="T:Intents.INPersonHandle" />.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPersonHandleType : long {
		Unknown = 0,
		EmailAddress,
		PhoneNumber
	}

	/// <summary>Enumerates types of payment accounts.</summary>
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

	/// <summary>Enumerates the results of an <see cref="T:Intents.INActivateCarSignalIntent" />.</summary>
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

	/// <summary>Enumerates bill-amount types.</summary>
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

	/// <summary>Enumerates common bills.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates the manners in which a car can make itself known.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum INCarSignalOptions : ulong {
		Audible = (1 << 0),
		Visible = (1 << 1),
	}

	/// <summary>Enumerates the results of an <see cref="T:Intents.INGetCarLockStatusIntent" />.</summary>
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

	/// <summary>Enumerates the results of an <see cref="T:Intents.INGetCarPowerLevelStatusIntent" />.</summary>
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

	/// <summary>Enumerates the results of an <see cref="T:Intents.INPayBillIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates the results of an <see cref="T:Intents.INSearchForBillsIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates the results of an <see cref="T:Intents.INSetCarLockStatusIntent" />.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INAddTasksIntent" />.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INAppendToNoteIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates balance units.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INBalanceType : long {
		Unknown = 0,
		Money,
		Points,
		Miles,
	}

	/// <summary>Enumerates call capabilities.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallCapability : long {
		Unknown = 0,
		AudioCall,
		VideoCall,
	}

	/// <summary>Enumerates call destination types.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallDestinationType : long {
		Unknown = 0,
		Normal,
		Emergency,
		Voicemail,
		Redial,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		CallBack,
	}

	/// <summary>Flags that enumerate call types to search for.</summary>
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
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Ringing = (1 << 5),
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		InProgress = (1 << 6),
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		OnHold = (1 << 7),
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INCancelRideIntent" />.</summary>
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCancelRideIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		Success,
		Failure,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INCreateNoteIntent" />.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INCreateTaskListIntent" />.</summary>
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

	/// <summary>Enumerates date types for a search.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INDateSearchType : long {
		Unknown = 0,
		ByDueDate,
		ByModifiedDate,
		ByCreatedDate,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INGetVisualCodeIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates location-based search types.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INLocationSearchType : long {
		Unknown = 0,
		ByLocationTrigger,
	}

	/// <summary>Enumerates message content types.</summary>
	[NoMac]
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
		[Deprecated (PlatformName.iOS, 18, 1, message: "Use 'INMessageReaction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use 'INMessageReaction' instead.")]
		TapbackLiked,
		[Deprecated (PlatformName.iOS, 18, 1, message: "Use 'INMessageReaction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use 'INMessageReaction' instead.")]
		TapbackDisliked,
		[Deprecated (PlatformName.iOS, 18, 1, message: "Use 'INMessageReaction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use 'INMessageReaction' instead.")]
		TapbackEmphasized,
		[Deprecated (PlatformName.iOS, 18, 1, message: "Use 'INMessageReaction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use 'INMessageReaction' instead.")]
		TapbackLoved,
		[Deprecated (PlatformName.iOS, 18, 1, message: "Use 'INMessageReaction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use 'INMessageReaction' instead.")]
		TapbackQuestioned,
		[Deprecated (PlatformName.iOS, 18, 1, message: "Use 'INMessageReaction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use 'INMessageReaction' instead.")]
		TapbackLaughed,
		MediaCalendar,
		MediaLocation,
		MediaAddressCard,
		MediaImage,
		MediaVideo,
		MediaPass,
		MediaAudio,
		[MacCatalyst (13, 1)]
		PaymentSent,
		[MacCatalyst (13, 1)]
		PaymentRequest,
		[MacCatalyst (13, 1)]
		PaymentNote,
		[MacCatalyst (13, 1)]
		Animoji,
		[MacCatalyst (13, 1)]
		ActivitySnippet,
		[MacCatalyst (13, 1)]
		File,
		[MacCatalyst (13, 1)]
		Link,
		[iOS (17, 0), MacCatalyst (17, 0)]
		Reaction,
		[iOS (18, 0), MacCatalyst (18, 0)]
		MediaAnimatedImage,
		[iOS (18, 0), MacCatalyst (18, 0)]
		ThirdPartyAttachment,
	}

	/// <summary>Enumerates note content types.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INNoteContentType : long {
		Unknown = 0,
		Text,
		Image,
	}

	/// <summary>Enumerates notebook item types to include in search results.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INNotebookItemType : long {
		Unknown = 0,
		Note,
		TaskList,
		Task,
	}

	/// <summary>Enumerates repetition frequencies.</summary>
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

	/// <summary>Enumerates reasons that a currency transfer amount is not supported.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestPaymentCurrencyAmountUnsupportedReason : long {
		AmountBelowMinimum = 1,
		AmountAboveMaximum,
		CurrencyUnsupported,
	}

	/// <summary>Enumerates reasons that a payer could not be resolved.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRequestPaymentPayerUnsupportedReason : long {
		CredentialsUnverified = 1,
		NoAccount,
		[MacCatalyst (13, 1)]
		NoValidHandle,
	}

	/// <summary>Enumerates feedback requirements for a ride.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRideFeedbackTypeOptions : ulong {
		Rate = (1 << 0),
		Tip = (1 << 1),
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSearchForAccountsIntent" />.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSearchForNotebookItemsIntent" />.</summary>
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

	/// <summary>Enumerates reasons that a recipient was not supported.</summary>
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

	/// <summary>Enumerates reasons that a transaction amount was not supported.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendPaymentCurrencyAmountUnsupportedReason : long {
		AmountBelowMinimum = 1,
		AmountAboveMaximum,
		CurrencyUnsupported,
	}

	/// <summary>Enumerates reason that a payee was not supported for a payment.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendPaymentPayeeUnsupportedReason : long {
		CredentialsUnverified = 1,
		InsufficientFunds,
		NoAccount,
		[MacCatalyst (13, 1)]
		NoValidHandle,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSendRideFeedbackIntent" />.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSendRideFeedbackIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		Success,
		Failure,
	}

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INSetTaskAttributeIntent" />.</summary>
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

	/// <summary>Enumerates search result sort orders.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSortType : long {
		Unknown = 0,
		AsIs,
		ByDate,
	}

	/// <summary>Enumerates conditions for spatial event triggers.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSpatialEvent : long {
		Unknown = 0,
		Arrive,
		Depart,
	}

	/// <summary>Enumerates task completion statuses.</summary>
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

	/// <summary>Enumerates results codes for the <see cref="T:Intents.INTransferMoneyIntent" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Enumerates visual code semantics.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INVisualCodeType : long {
		Unknown = 0,
		Contact,
		RequestPayment,
		SendPayment,
		[MacCatalyst (13, 1)]
		Transit,
		[MacCatalyst (13, 1)]
		Bus,
		[MacCatalyst (13, 1)]
		Subway,
	}

	[TV (14, 0), NoMac]
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
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PodcastStation,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		RadioStation,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Station,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Music,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		AlgorithmicRadioStation,
		[iOS (13, 4, 1)]
		[MacCatalyst (13, 1)]
		News,
	}

	[TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlayMediaIntentResponseCode : long {
		Unspecified = 0,
		Ready = 1,
		ContinueInApp = 2,
		InProgress = 3,
		Success = 4,
		HandleInApp = 5,
		Failure = 6,
		FailureRequiringAppLaunch = 7,
		FailureUnknownMediaType = 8,
		FailureNoUnplayedContent = 9,
		FailureRestrictedContent = 10,
		FailureMaxStreamLimitReached = 11,
	}

	[TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlaybackRepeatMode : long {
		Unknown = 0,
		None,
		All,
		One,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INDailyRoutineSituation : long {
		Morning,
		Evening,
		Home,
		Work,
		School,
		Gym,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Commute,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		HeadphonesConnected,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		ActiveWorkout,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PhysicalActivityIncomplete,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INUpcomingMediaPredictionMode : long {
		Default = 0,
		OnlyPredictSuggestedIntents = 1,
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INRelevantShortcutRole : long {
		Action,
		Information,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddMediaIntentResponseCode : long {
		Unspecified = 0,
		Ready = 1,
		InProgress = 2,
		Success = 3,
		HandleInApp = 4,
		Failure = 5,
		FailureRequiringAppLaunch = 6,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddMediaMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddTasksTargetTaskListConfirmationReason : long {
		ListShouldBeCreated = 1,
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddTasksTemporalEventTriggerUnsupportedReason : long {
		TimeInPast = 1,
		InvalidRecurrence,
	}

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INCallAudioRoute : long {
		Unknown = 0,
		SpeakerphoneAudioRoute,
		BluetoothAudioRoute,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[NoTV, NoMac, iOS (13, 0)]
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
	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INDeleteTasksTaskListUnsupportedReason : long {
		NoTaskListFound = 1,
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Native]
	public enum INDeleteTasksTaskUnsupportedReason : long {
		Found = 1,
		InApp,
	}

	[NoTV, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaAffinityType : long {
		Unknown = 0,
		Like,
		Dislike,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaDestinationType : long {
		Unknown = 0,
		Library,
		Playlist,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaReference : long {
		Unknown = 0,
		CurrentlyPlaying,
		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		My,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INMediaUserContextSubscriptionStatus : long {
		Unknown = 0,
		NotSubscribed,
		Subscribed,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlayMediaMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlayMediaPlaybackSpeedUnsupportedReason : long {
		BelowMinimum = 1,
		AboveMaximum,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INPlaybackQueueLocation : long {
		Unknown = 0,
		Now,
		Next,
		Later,
	}

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INReservationActionType : long {
		Unknown = 0,
		CheckIn,
	}

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INReservationStatus : long {
		Unknown = 0,
		Canceled,
		Pending,
		Hold,
		Confirmed,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSearchForMediaMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSetTaskAttributeTemporalEventTriggerUnsupportedReason : long {
		TimeInPast = 1,
		InvalidRecurrence,
	}

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INSnoozeTasksTaskUnsupportedReason : long {
		NoTasksFound = 1,
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INStartCallCallCapabilityUnsupportedReason : long {
		VideoCallUnsupported = 1,
		MicrophoneNotAccessible,
		CameraNotAccessible,
	}

	[NoTV, NoMac, iOS (13, 0)]
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
		[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		RequiringInAppAuthentication,
	}

	[NoTV, NoMac, iOS (13, 0)]
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
		FailureCallRinging,
		[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		ResponseCode,
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTaskPriority : long {
		Unknown = 0,
		NotFlagged,
		Flagged,
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTemporalEventTriggerTypeOptions : ulong {
		NotScheduled = (1uL << 0),
		ScheduledNonRecurring = (1uL << 1),
		ScheduledRecurring = (1uL << 2),
	}

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INTicketedEventCategory : long {
		Unknown = 0,
		Movie,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INUpdateMediaAffinityMediaItemUnsupportedReason : long {
		LoginRequired = 1,
		SubscriptionRequired,
		UnsupportedMediaType,
		ExplicitContentSettings,
		CellularDataSettings,
		RestrictedContent,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		ServiceUnavailable,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		RegionRestriction,
	}

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum INAddMediaMediaDestinationUnsupportedReason : long {
		PlaylistNameNotFound = 1,
		PlaylistNotEditable = 2,
	}

	[Flags]
	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INOutgoingMessageType : long {
		Unknown = 0,
		Text,
		Audio,
	}

	[Flags]
	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum INStartCallCallRecordToCallBackUnsupportedReason : long {
		NoMatchingCall = 1,
	}

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
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

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
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

	/// <summary>Enumerates Intents / SiriKit intent types.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum INIntentIdentifier {
		[Field (null)]
		None = -1,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'StartCall' instead.")]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'StartCall' instead.")]
		[Field ("INStartAudioCallIntentIdentifier")]
		StartAudioCall,

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'StartCall' instead.")]
		[Unavailable (PlatformName.MacOSX)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'StartCall' instead.")]
		[Field ("INStartVideoCallIntentIdentifier")]
		StartVideoCall,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSearchCallHistoryIntentIdentifier")]
		SearchCallHistory,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSetAudioSourceInCarIntentIdentifier")]
		SetAudioSourceInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSetClimateSettingsInCarIntentIdentifier")]
		SetClimateSettingsInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSetDefrosterSettingsInCarIntentIdentifier")]
		SetDefrosterSettingsInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSetSeatSettingsInCarIntentIdentifier")]
		SetSeatSettingsInCar,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSetProfileInCarIntentIdentifier")]
		SetProfileInCar,

		[Unavailable (PlatformName.MacOSX)]
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
		[Field ("INSetRadioStationIntentIdentifier")]
		SetRadioStation,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSendMessageIntentIdentifier")]
		SendMessage,

		[Unavailable (PlatformName.MacOSX)]
		[Field ("INSearchForMessagesIntentIdentifier")]
		SearchForMessages,

		[Unavailable (PlatformName.MacOSX)]
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

		[NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("INStartCallIntentIdentifier")]
		StartCall,

		[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
		[Field ("INAnswerCallIntentIdentifier")]
		AnswerCall,

		[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
		[Field ("INHangUpCallIntentIdentifier")]
		HangUpCall,
	}

	/// <summary>Enumerates the types of information associated with a particular value of a <see cref="T:Intents.INPersonHandleType" />.</summary>
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("INPersonHandleLabelSchool")]
		School,
	}

	/// <summary>Enumerates known interpersonal relationships.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	enum INPersonRelationship {
		[Field (null)]
		None,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipFather")]
		Father,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipMother")]
		Mother,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipParent")]
		Parent,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipBrother")]
		Brother,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipSister")]
		Sister,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipChild")]
		Child,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipFriend")]
		Friend,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipSpouse")]
		Spouse,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipPartner")]
		Partner,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipAssistant")]
		Assistant,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipManager")]
		Manager,

		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipSon")]
		Son,

		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("INPersonRelationshipDaughter")]
		Daughter,
	}

	/// <summary>Enumerates known training types.</summary>
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

		[MacCatalyst (13, 1)]
		[Field ("INWorkoutNameIdentifierHike")]
		Hike,

		[MacCatalyst (13, 1)]
		[Field ("INWorkoutNameIdentifierHighIntensityIntervalTraining")]
		HighIntensityIntervalTraining,

		[MacCatalyst (13, 1)]
		[Field ("INWorkoutNameIdentifierSwim")]
		Swim,
	}

	[iOS (14, 0), NoMac, NoTV]
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

		[Deprecated (PlatformName.iOS, 17, 4, message: "Use 'INCarChargingConnectorType.NacsDc' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 4, message: "Use 'INCarChargingConnectorType.NacsDc' instead.")]
		[Field ("INCarChargingConnectorTypeTesla")]
		Tesla,

		[Field ("INCarChargingConnectorTypeMennekes")]
		Mennekes,

		[iOS (17, 4), MacCatalyst (17, 4)]
		[Field ("INCarChargingConnectorTypeNACSDC")]
		NacsDC,

		[iOS (17, 4), MacCatalyst (17, 4)]
		[Field ("INCarChargingConnectorTypeNACSAC")]
		NacsAC,
	}

	[Native]
	[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0), NoTV]
	public enum INMessageReactionType : long {
		Unknown = 0,
		Emoji,
		Generic,
	}

	[Native]
	[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0), NoTV]
	public enum INStickerType : long {
		Unknown = 0,
		Emoji,
		Generic,
	}

	// End of enums

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in reservation-related interactions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INBookRestaurantReservationIntent">Apple documentation for <c>INBookRestaurantReservationIntent</c></related>
	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INBookRestaurantReservationIntent : NSCopying {
		[MacCatalyst (13, 1)]
		[Export ("initWithRestaurant:bookingDateComponents:partySize:bookingIdentifier:guest:selectedOffer:guestProvidedSpecialRequestText:")]
		NativeHandle Constructor (INRestaurant restaurant, NSDateComponents bookingDateComponents, nuint partySize, [NullAllowed] string bookingIdentifier, [NullAllowed] INRestaurantGuest guest, [NullAllowed] INRestaurantOffer selectedOffer, [NullAllowed] string guestProvidedSpecialRequestText);

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
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INBookRestaurantReservationIntentHandling {

		[Abstract]
		[Export ("handleBookRestaurantReservation:completion:")]
		void HandleBookRestaurantReservation (INBookRestaurantReservationIntent intent, Action<INBookRestaurantReservationIntentResponse> completion);

		[Export ("confirmBookRestaurantReservation:completion:")]
		void Confirm (INBookRestaurantReservationIntent intent, Action<INBookRestaurantReservationIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINBookRestaurantReservationIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INBookRestaurantReservationIntentResponse">Apple documentation for <c>INBookRestaurantReservationIntentResponse</c></related>
	[NoTV]
	[Unavailable (PlatformName.MacOSX)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBooleanResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBooleanResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving call records.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCallRecordTypeResolutionResult">Apple documentation for <c>INCallRecordTypeResolutionResult</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallRecordTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedCallRecordType:")]
		INCallRecordTypeResolutionResult GetSuccess (INCallRecordType resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCallRecordTypeToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallRecordTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallRecordTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to cancel the workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCancelWorkoutIntent">Apple documentation for <c>INCancelWorkoutIntent</c></related>
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
		void Confirm (INCancelWorkoutIntent intent, Action<INCancelWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForCancelWorkout:withCompletion:")]
		void ResolveWorkoutName (INCancelWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINCancelWorkoutIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCancelWorkoutIntentResponse">Apple documentation for <c>INCancelWorkoutIntentResponse</c></related>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving air conditioning.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCarAirCirculationModeResolutionResult">Apple documentation for <c>INCarAirCirculationModeResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarAirCirculationModeResolutionResult bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarAirCirculationModeResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedCarAirCirculationMode:")]
		INCarAirCirculationModeResolutionResult GetSuccess (INCarAirCirculationMode resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarAirCirculationModeToConfirm:")]
		INCarAirCirculationModeResolutionResult GetConfirmationRequired (INCarAirCirculationMode valueToConfirm);

#if !XAMCORE_5_0
		[Obsolete ("Use 'GetConfirmationRequired' instead.")]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarAirCirculationModeToConfirm:")]
		INCarAirCirculationModeResolutionResult ConfirmationRequiredWithCarAirCirculationModeToConfirm (INCarAirCirculationMode carAirCirculationModeToConfirm);
#endif // XAMCORE_5_0

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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarAirCirculationModeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarAirCirculationModeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving car audio systems.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCarAudioSourceResolutionResult">Apple documentation for <c>INCarAudioSourceResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarAudioSourceResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	interface INCarAudioSourceResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedCarAudioSource:")]
		INCarAudioSourceResolutionResult GetSuccess (INCarAudioSource resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarAudioSourceToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarAudioSourceResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarAudioSourceResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving car defrosters and their settings.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCarDefrosterResolutionResult">Apple documentation for <c>INCarDefrosterResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarDefrosterResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	interface INCarDefrosterResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedCarDefroster:")]
		INCarDefrosterResolutionResult GetSuccess (INCarDefroster resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarDefrosterToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarDefrosterResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarDefrosterResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving car seats and their settings.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCarSeatResolutionResult">Apple documentation for <c>INCarSeatResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INCarSeatResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[DisableDefaultCtor]
	interface INCarSeatResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedCarSeat:")]
		INCarSeatResolutionResult GetSuccess (INCarSeat resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarSeatToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarSeatResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarSeatResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An amount of money.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCurrencyAmount">Apple documentation for <c>INCurrencyAmount</c></related>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving payments.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INCurrencyAmountResolutionResult">Apple documentation for <c>INCurrencyAmountResolutionResult</c></related>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCurrencyAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
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
		[NoMac]
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
		[NoMac]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving a range of dates.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INDateComponentsRangeResolutionResult">Apple documentation for <c>INDateComponentsRangeResolutionResult</c></related>
	[NoMac]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDateComponentsRangeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDateComponentsRangeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary> Interface combining several interfaces related to various phone-call intents.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INStartAudioCallIntentHandling, INStartVideoCallIntentHandling and INSearchCallHistoryIntentHandling' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INStartAudioCallIntentHandling, INStartVideoCallIntentHandling and INSearchCallHistoryIntentHandling' instead.")]
	[Protocol]
	interface INCallsDomainHandling : INStartAudioCallIntentHandling, INSearchCallHistoryIntentHandling
	, INStartVideoCallIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various car-related intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling and INGetCarPowerLevelStatusIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling and INGetCarPowerLevelStatusIntentHandling' instead.")]
	[Protocol]
	interface INCarCommandsDomainHandling : INActivateCarSignalIntentHandling, INSetCarLockStatusIntentHandling, INGetCarLockStatusIntentHandling, INGetCarPowerLevelStatusIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various CarPlay intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling and INSaveProfileInCarIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling and INSaveProfileInCarIntentHandling' instead.")]
	[Protocol]
	interface INCarPlayDomainHandling : INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling, INSaveProfileInCarIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various workout-related intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling and INResumeWorkoutIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling and INResumeWorkoutIntentHandling' instead.")]
	[Protocol]
	interface INWorkoutsDomainHandling : INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling, INResumeWorkoutIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various radio-related intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSetRadioStationIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSetRadioStationIntentHandling' instead.")]
	[Protocol]
	interface INRadioDomainHandling : INSetRadioStationIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various Message-related intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSendMessageIntentHandling, INSearchForMessagesIntentHandling and INSetMessageAttributeIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSendMessageIntentHandling, INSearchForMessagesIntentHandling and INSetMessageAttributeIntentHandling' instead.")]
	[Protocol]
	interface INMessagesDomainHandling : INSendMessageIntentHandling, INSearchForMessagesIntentHandling
	, INSetMessageAttributeIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various payment-related intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling, INSearchForAccountsIntentHandling and INTransferMoneyIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling, INSearchForAccountsIntentHandling and INTransferMoneyIntentHandling' instead.")]
	[Protocol]
	interface INPaymentsDomainHandling : INSendPaymentIntentHandling, INRequestPaymentIntentHandling, INPayBillIntentHandling, INSearchForBillsIntentHandling
	, INSearchForAccountsIntentHandling, INTransferMoneyIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various photo-related intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INSearchForPhotosIntentHandling and INStartPhotoPlaybackIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INSearchForPhotosIntentHandling and INStartPhotoPlaybackIntentHandling' instead.")]
	[Protocol]
	interface INPhotosDomainHandling : INSearchForPhotosIntentHandling, INStartPhotoPlaybackIntentHandling {
	}

	/// <summary>Interface combining several interfaces related to various ridesharing intents.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling, INCancelRideIntentHandling and INSendRideFeedbackIntentHandling' instead.")]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling, INCancelRideIntentHandling and INSendRideFeedbackIntentHandling' instead.")]
	[Protocol]
	interface INRidesharingDomainHandling : INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling
	, INCancelRideIntentHandling, INSendRideFeedbackIntentHandling {
	}

	/// <summary>Composite interface for adopting all of lists and notes protocols.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling and INSearchForNotebookItemsIntentHandling' instead.")]
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling and INSearchForNotebookItemsIntentHandling' instead.")]
	[Protocol]
	interface INNotebookDomainHandling : INCreateNoteIntentHandling, INAppendToNoteIntentHandling, INAddTasksIntentHandling, INCreateTaskListIntentHandling, INSetTaskAttributeIntentHandling, INSearchForNotebookItemsIntentHandling {
	}

	/// <summary>Composite interface for adopting all of the visual codes protocols.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Implement 'INGetVisualCodeIntentHandling' instead.")]
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'INGetVisualCodeIntentHandling' instead.")]
	[Protocol]
	interface INVisualCodeDomainHandling : INGetVisualCodeIntentHandling {
	}

	[TV (14, 0)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDoubleResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDoubleResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving dates.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INDateComponentsResolutionResult">Apple documentation for <c>INDateComponentsResolutionResult</c></related>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDateComponentsResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDateComponentsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to finish the workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INEndWorkoutIntent">Apple documentation for <c>INEndWorkoutIntent</c></related>
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
		void Confirm (INEndWorkoutIntent intent, Action<INEndWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForEndWorkout:withCompletion:")]
		void ResolveWorkoutName (INEndWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINEndWorkoutIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INEndWorkoutIntentResponse">Apple documentation for <c>INEndWorkoutIntentResponse</c></related>
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

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INIntentHandlerProviding {

		[Abstract]
		[Export ("handlerForIntent:")]
		[return: NullAllowed]
		NSObject GetHandler (INIntent intent);
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INExtension : INIntentHandlerProviding {
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to receive a list of available reservation times.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetAvailableRestaurantReservationBookingDefaultsIntent">Apple documentation for <c>INGetAvailableRestaurantReservationBookingDefaultsIntent</c></related>
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void HandleAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void Confirm (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookingDefaults:withCompletion:")]
		void ResolveAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent intent, Action<INRestaurantResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetAvailableRestaurantReservationBookingDefaultsIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetAvailableRestaurantReservationBookingDefaultsIntentResponse">Apple documentation for <c>INGetAvailableRestaurantReservationBookingDefaultsIntentResponse</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An intention to retrieve restaurant availability.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetAvailableRestaurantReservationBookingsIntent">Apple documentation for <c>INGetAvailableRestaurantReservationBookingsIntent</c></related>
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookings:completion:")]
		void HandleAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookings:completion:")]
		void Confirm (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookings:withCompletion:")]
		void ResolveAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INRestaurantResolutionResult> completion);

		[Export ("resolvePartySizeForGetAvailableRestaurantReservationBookings:withCompletion:")]
		void ResolvePartySizeAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolvePreferredBookingDateComponentsForGetAvailableRestaurantReservationBookings:withCompletion:")]
		void ResolvePreferredBookingDateAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent intent, Action<INDateComponentsResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetAvailableRestaurantReservationBookingsIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetAvailableRestaurantReservationBookingsIntentResponse">Apple documentation for <c>INGetAvailableRestaurantReservationBookingsIntentResponse</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to retrieve information about a particular guest making a reservation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetRestaurantGuestIntent">Apple documentation for <c>INGetRestaurantGuestIntent</c></related>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INGetRestaurantGuestIntent {
	}

	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetRestaurantGuestIntentHandling {

		[Abstract]
		[Export ("handleGetRestaurantGuest:completion:")]
		void HandleRestaurantGuest (INGetRestaurantGuestIntent intent, Action<INGetRestaurantGuestIntentResponse> completion);

		[Export ("confirmGetRestaurantGuest:completion:")]
		void Confirm (INGetRestaurantGuestIntent guestIntent, Action<INGetRestaurantGuestIntentResponse> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetRestaurantGuestIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetRestaurantGuestIntentResponse">Apple documentation for <c>INGetRestaurantGuestIntentResponse</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to get information about the current ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetRideStatusIntent">Apple documentation for <c>INGetRideStatusIntent</c></related>
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
		void Confirm (INGetRideStatusIntent intent, Action<INGetRideStatusIntentResponse> completion);
	}

	interface IINGetRideStatusIntentResponseObserver { }

	/// <summary>Receives periodic updates on ride status.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetRideStatusIntentResponseObserver {

		[Abstract]
		[Export ("getRideStatusResponseDidUpdate:")]
		void DidUpdateRideStatus (INGetRideStatusIntentResponse response);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetRideStatusIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetRideStatusIntentResponse">Apple documentation for <c>INGetRideStatusIntentResponse</c></related>
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

	/// <summary>A request to retrieve the user's current restaurant reservations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetUserCurrentRestaurantReservationBookingsIntent">Apple documentation for <c>INGetUserCurrentRestaurantReservationBookingsIntent</c></related>
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INGetUserCurrentRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetUserCurrentRestaurantReservationBookings:completion:")]
		void HandleUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetUserCurrentRestaurantReservationBookings:completion:")]
		void Confirm (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetUserCurrentRestaurantReservationBookings:withCompletion:")]
		void ResolveUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent intent, Action<INRestaurantResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetUserCurrentRestaurantReservationBookingsIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INGetUserCurrentRestaurantReservationBookingsIntentResponse">Apple documentation for <c>INGetUserCurrentRestaurantReservationBookingsIntentResponse</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

		[NoTV, NoiOS, Mac (13, 0)]
		[NoMacCatalyst]
		[Static]
		[Export ("imageWithNSImage:")]
		INImage FromImage (NSImage image);

		[NoMac, iOS (14, 0)]
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

		[NoMac, NoTV]
		[NoMacCatalyst]
		[Static]
		[Export ("imageWithCGImage:")]
		INImage FromImage (CGImage image);

		[NoMac, NoTV]
		[NoMacCatalyst]
		[Static]
		[Export ("imageWithUIImage:")]
		INImage FromImage (UIImage image);

		[NoMac, NoTV]
		[NoMacCatalyst]
		[Static]
		[Export ("imageSizeForIntentResponse:")]
		CGSize GetImageSize (INIntentResponse response);

		[NoMac, NoTV]
		[NoMacCatalyst]
		[Async]
		[Export ("fetchUIImageWithCompletion:")]
		void FetchImage (Action<UIImage> completion);
	}

	[TV (14, 0)]
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
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INIntegerResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0)]
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

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("suggestedInvocationPhrase")]
		string SuggestedInvocationPhrase { get; set; }

		[NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("shortcutAvailability", ArgumentSemantic.Assign)]
		INShortcutAvailabilityOptions ShortcutAvailability { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("donationMetadata", ArgumentSemantic.Copy)]
		INIntentDonationMetadata DonationMetadata { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("setImage:forParameterNamed:")]
		void SetImage ([NullAllowed] INImage image, string parameterName);

		[MacCatalyst (13, 1)]
		[Export ("imageForParameterNamed:")]
		[return: NullAllowed]
		INImage GetImage (string parameterName);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("keyImage")]
		INImage GetKeyImage ();
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

		// [iOS (13,0), TV (14,0)]
		// [Static]
		// [Export ("unsupportedWithReason:")]
		// INIntentResolutionResult GetUnsupported (nint reason);

		// [iOS (13,0), TV (14,0)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to receive a list of available ride options.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INListRideOptionsIntent">Apple documentation for <c>INListRideOptionsIntent</c></related>
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
		void Confirm (INListRideOptionsIntent intent, Action<INListRideOptionsIntentResponse> completion);

		[Export ("resolvePickupLocationForListRideOptions:withCompletion:")]
		void ResolvePickupLocation (INListRideOptionsIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveDropOffLocationForListRideOptions:withCompletion:")]
		void ResolveDropOffLocation (INListRideOptionsIntent intent, Action<INPlacemarkResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINListRideOptionsIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INListRideOptionsIntentResponse">Apple documentation for <c>INListRideOptionsIntentResponse</c></related>
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

	/// <summary>Encapsulates Intents / SiriKit information regarding a messaging-service message.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INMessage">Apple documentation for <c>INMessage</c></related>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMessage : NSCopying, NSSecureCoding {

		[NoMac, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:serviceName:audioMessageFile:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType, [NullAllowed] string serviceName, [NullAllowed] INFile audioMessageFile);

		[NoMac, iOS (13, 2)]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:serviceName:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType, [NullAllowed] string serviceName);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use a constructor that takes a reaction instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use a constructor that takes a reaction instead.")]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use a constructor that takes a reaction instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use a constructor that takes a reaction instead.")]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:messageType:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, INMessageType messageType);

		[iOS (18, 0), MacCatalyst (18, 0), NoMac]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:serviceName:messageType:referencedMessage:reaction:")]
		NativeHandle Constructor (
				string identifier,
				[NullAllowed] string conversationIdentifier,
				[NullAllowed] string content,
				[NullAllowed] NSDate dateSent,
				[NullAllowed] INPerson sender,
				[NullAllowed] INPerson [] recipients,
				[NullAllowed] INSpeakableString groupName,
				[NullAllowed] string serviceName,
				INMessageType messageType,
				[NullAllowed] INMessage referencedMessage,
				[NullAllowed] INMessageReaction reaction
				);

		[DesignatedInitializer]
		[iOS (18, 0), MacCatalyst (18, 0), NoMac]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:serviceName:messageType:referencedMessage:sticker:reaction:")]
		NativeHandle Constructor (
				string identifier,
				[NullAllowed] string conversationIdentifier,
				[NullAllowed] string content,
				[NullAllowed] NSDate dateSent,
				[NullAllowed] INPerson sender,
				[NullAllowed] INPerson [] recipients,
				[NullAllowed] INSpeakableString groupName,
				[NullAllowed] string serviceName,
				INMessageType messageType,
				[NullAllowed] INMessage referencedMessage,
				[NullAllowed] INSticker sticker,
				[NullAllowed] INMessageReaction reaction
				);

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

		[NoMac, iOS (13, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("serviceName")]
		string ServiceName { get; }

		[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'AttachmentFile' instead.")]
		[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'AttachmentFile' instead.")]
		[NoMac, iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("audioMessageFile", ArgumentSemantic.Copy)]
		INFile AudioMessageFile { get; }

		[NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:messageType:serviceName:attachmentFiles:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, INMessageType messageType, [NullAllowed] string serviceName, [NullAllowed] INFile [] attachmentFiles);

		[NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:serviceName:linkMetadata:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, [NullAllowed] string serviceName, [NullAllowed] INMessageLinkMetadata linkMetadata);

		[NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithIdentifier:conversationIdentifier:content:dateSent:sender:recipients:groupName:serviceName:messageType:numberOfAttachments:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string conversationIdentifier, [NullAllowed] string content, [NullAllowed] NSDate dateSent, [NullAllowed] INPerson sender, [NullAllowed] INPerson [] recipients, [NullAllowed] INSpeakableString groupName, [NullAllowed] string serviceName, INMessageType messageType, [NullAllowed] NSNumber numberOfAttachments);

		[NullAllowed]
		[NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("attachmentFiles", ArgumentSemantic.Copy)]
		INFile [] AttachmentFiles { get; }

		[NullAllowed, BindAs (typeof (int))]
		[NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("numberOfAttachments", ArgumentSemantic.Copy)]
		NSNumber NumberOfAttachments { get; }

		[NullAllowed]
		[NoMac, iOS (17, 0)]
		[Export ("linkMetadata", ArgumentSemantic.Copy)]
		INMessageLinkMetadata LinkMetadata { get; }

		[NullAllowed]
		[NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("sticker", ArgumentSemantic.Copy)]
		INSticker Sticker { get; set; }

		[NullAllowed]
		[NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("reaction", ArgumentSemantic.Copy)]
		INMessageReaction Reaction { get; set; }

	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving messages.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INMessageAttributeOptionsResolutionResult">Apple documentation for <c>INMessageAttributeOptionsResolutionResult</c></related>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeOptionsResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedMessageAttributeOptions:")]
		INMessageAttributeOptionsResolutionResult GetSuccess (INMessageAttributeOptions resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithMessageAttributeOptionsToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMessageAttributeOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMessageAttributeOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving messages.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INMessageAttributeResolutionResult">Apple documentation for <c>INMessageAttributeResolutionResult</c></related>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedMessageAttribute:")]
		INMessageAttributeResolutionResult GetSuccess (INMessageAttribute resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithMessageAttributeToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INMessageAttributeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INMessageAttributeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to pause the workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPauseWorkoutIntent">Apple documentation for <c>INPauseWorkoutIntent</c></related>
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
		void Confirm (INPauseWorkoutIntent intent, Action<INPauseWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForPauseWorkout:withCompletion:")]
		void ResolveWorkoutName (INPauseWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINPauseWorkoutIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPauseWorkoutIntentResponse">Apple documentation for <c>INPauseWorkoutIntentResponse</c></related>
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

	/// <summary>Encapsulates data about a form of payment.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPaymentMethod">Apple documentation for <c>INPaymentMethod</c></related>
	[NoTV]
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

	/// <summary>Encapsulates details about a payment.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPaymentRecord">Apple documentation for <c>INPaymentRecord</c></related>
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

	/// <summary>Encapsulates a person's data, for the purposes of Intents / SiriKit.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPerson">Apple documentation for <c>INPerson</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPerson : NSCopying, NSSecureCoding, INSpeakable {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:")]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:relationship:")]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, [NullAllowed] string relationship);

		[MacCatalyst (13, 1)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isMe:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, bool isMe);

		[Internal]
		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isMe:suggestionType:")]
		IntPtr InitWithMe (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, bool isMe, INPersonSuggestionType suggestionType);

		[Internal]
		[iOS (15, 0), MacCatalyst (15, 0)]
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

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("contactSuggestion")]
		bool ContactSuggestion { [Bind ("isContactSuggestion")] get; }
	}

	/// <summary>The user of the application.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPersonHandle">Apple documentation for <c>INPersonHandle</c></related>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving known people (contacts).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPersonResolutionResult">Apple documentation for <c>INPersonResolutionResult</c></related>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPersonResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPersonResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving named locations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPlacemarkResolutionResult">Apple documentation for <c>INPlacemarkResolutionResult</c></related>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPlacemarkResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPlacemarkResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[Unavailable (PlatformName.MacOSX)]
	[DisableDefaultCtor]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INPreferences {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("siriAuthorizationStatus")]
		INSiriAuthorizationStatus SiriAuthorizationStatus { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Async]
		[Export ("requestSiriAuthorization:")]
		void RequestSiriAuthorization (Action<INSiriAuthorizationStatus> handler);

		[Static]
		[Export ("siriLanguageCode")]
		string SiriLanguageCode { get; }
	}

	/// <summary>Holds information about a pair of prices.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INPriceRange">Apple documentation for <c>INPriceRange</c></related>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving radio formats.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRadioTypeResolutionResult">Apple documentation for <c>INRadioTypeResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRadioTypeResolutionResult bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRadioTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedRadioType:")]
		INRadioTypeResolutionResult GetSuccess (INRadioType resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithRadioTypeToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRadioTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRadioTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving relative locations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRelativeReferenceResolutionResult">Apple documentation for <c>INRelativeReferenceResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRelativeReferenceResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedRelativeReference:")]
		INRelativeReferenceResolutionResult GetSuccess (INRelativeReference resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithRelativeReferenceToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRelativeReferenceResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRelativeReferenceResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving values that have relative settings (higher / lower, more / less).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRelativeSettingResolutionResult">Apple documentation for <c>INRelativeSettingResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INRelativeSettingResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedRelativeSetting:")]
		INRelativeSettingResolutionResult GetSuccess (INRelativeSetting resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithRelativeSettingToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRelativeSettingResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRelativeSettingResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to request a payment.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRequestPaymentIntent">Apple documentation for <c>INRequestPaymentIntent</c></related>
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
		void Confirm (INRequestPaymentIntent intent, Action<INRequestPaymentIntentResponse> completion);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolvePayer (INRequestPaymentIntent, Action<INRequestPaymentPayerResolutionResult>)' instead.")]
		[Export ("resolvePayerForRequestPayment:withCompletion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolvePayerForRequestPayment:completion:")]
		void ResolvePayer (INRequestPaymentIntent intent, Action<INRequestPaymentPayerResolutionResult> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINRequestPaymentIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRequestPaymentIntentResponse">Apple documentation for <c>INRequestPaymentIntentResponse</c></related>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to request a ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRequestRideIntent">Apple documentation for <c>INRequestRideIntent</c></related>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INRequestRideIntent {

		[Deprecated (PlatformName.iOS, 10, 3, message: "Use the INDateComponentsRange overload")]
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
		void Confirm (INRequestRideIntent intent, Action<INRequestRideIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINRequestRideIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRequestRideIntentResponse">Apple documentation for <c>INRequestRideIntentResponse</c></related>
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

	/// <summary>Data about a specific restaurant location.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurant">Apple documentation for <c>INRestaurant</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>A <see cref="T:Intents.INPerson" /> expected at a restaurant reservation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantGuest">Apple documentation for <c>INRestaurantGuest</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>Encapsulates the preferred configuration for presenting guest information for Intents relating to restaurants.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantGuestDisplayPreferences">Apple documentation for <c>INRestaurantGuestDisplayPreferences</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving restaurant reservations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantGuestResolutionResult">Apple documentation for <c>INRestaurantGuestResolutionResult</c></related>
	[Unavailable (PlatformName.MacOSX)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRestaurantGuestResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRestaurantGuestResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>Encapsulates special offers and promotions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantOffer">Apple documentation for <c>INRestaurantOffer</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>A potential restaurant reservation (see also <see cref="T:Intents.INRestaurantReservationUserBooking" />).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantReservationBooking">Apple documentation for <c>INRestaurantReservationBooking</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>A restaurant reservation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantReservationUserBooking">Apple documentation for <c>INRestaurantReservationUserBooking</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving restaurant names.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRestaurantResolutionResult">Apple documentation for <c>INRestaurantResolutionResult</c></related>
	[Unavailable (PlatformName.MacOSX)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRestaurantResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRestaurantResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to resume a paused workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INResumeWorkoutIntent">Apple documentation for <c>INResumeWorkoutIntent</c></related>
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
		void Confirm (INResumeWorkoutIntent intent, Action<INResumeWorkoutIntentResponse> completion);

		[Export ("resolveWorkoutNameForResumeWorkout:withCompletion:")]
		void ResolveWorkoutName (INResumeWorkoutIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINResumeWorkoutIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INResumeWorkoutIntentResponse">Apple documentation for <c>INResumeWorkoutIntentResponse</c></related>
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

	/// <summary>Holds data relating to finished rides, including the completion reason and payment information.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRideCompletionStatus">Apple documentation for <c>INRideCompletionStatus</c></related>
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

	/// <summary>Information of the driver of a requested ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRideDriver">Apple documentation for <c>INRideDriver</c></related>
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRideDriver bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INPerson))]
	[DisableDefaultCtor] // xcode 8.2 beta 1 -> NSInvalidArgumentException Reason: *** -[__NSPlaceholderDictionary initWithObjects:forKeys:count:]: attempt to insert nil object from objects[1]
	interface INRideDriver : NSCopying, NSSecureCoding {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:rating:phoneNumber:")]
		[Deprecated (PlatformName.iOS, 10, 2, message: "Use the overload signature instead.")]
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

	/// <summary>A ride-related charge.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRideFareLineItem">Apple documentation for <c>INRideFareLineItem</c></related>
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

	/// <summary>Holds options relating to a vehicle ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRideOption">Apple documentation for <c>INRideOption</c></related>
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

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("usesMeteredFare", ArgumentSemantic.Copy)]
		NSNumber UsesMeteredFare { get; set; }

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

	/// <summary>Holds the data associated with the number of passengers in a ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRidePartySizeOption">Apple documentation for <c>INRidePartySizeOption</c></related>
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

	/// <summary>Encapsulates the state of a ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRideStatus">Apple documentation for <c>INRideStatus</c></related>
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

	/// <summary>The vehicle used for a requested ride.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INRideVehicle">Apple documentation for <c>INRideVehicle</c></related>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to save a user profile.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSaveProfileInCarIntent">Apple documentation for <c>INSaveProfileInCarIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSaveProfileInCarIntent {
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithProfileNumber:profileName:")]
#if XAMCORE_5_0
		NativeHandle Constructor ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileName);
#else
		NativeHandle Constructor ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel);
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
	}

	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSaveProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSaveProfileInCar:completion:")]
		void HandleSaveProfileInCar (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("confirmSaveProfileInCar:completion:")]
		void Confirm (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSaveProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSaveProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveProfileNameForSaveProfileInCar:withCompletion:")]
		void ResolveProfileName (INSaveProfileInCarIntent intent, Action<INStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSaveProfileInCarIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSaveProfileInCarIntentResponse">Apple documentation for <c>INSaveProfileInCarIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to search the call history.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSearchCallHistoryIntent">Apple documentation for <c>INSearchCallHistoryIntent</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 15, 0)]
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

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (INDateComponentsRange, INPerson, INCallCapabilityOptions, INCallRecordTypeOptions, NSNumber)' instead.")]
		[Export ("initWithCallType:dateCreated:recipient:callCapabilities:")]
		NativeHandle Constructor (INCallRecordType callType, [NullAllowed] INDateComponentsRange dateCreated, [NullAllowed] INPerson recipient, INCallCapabilityOptions callCapabilities);

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

	[NoMac]
	[Deprecated (PlatformName.iOS, 15, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSearchCallHistoryIntentHandling {

		[Abstract]
		[Export ("handleSearchCallHistory:completion:")]
		void HandleSearchCallHistory (INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Export ("confirmSearchCallHistory:completion:")]
		void Confirm (INSearchCallHistoryIntent intent, Action<INSearchCallHistoryIntentResponse> completion);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveCallTypes' instead.")]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSearchCallHistoryIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSearchCallHistoryIntentResponse">Apple documentation for <c>INSearchCallHistoryIntentResponse</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to search their message history.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSearchForMessagesIntent">Apple documentation for <c>INSearchForMessagesIntent</c></related>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSearchForMessagesIntent {

		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'conversationIdentifiers' instead.")]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:speakableGroupNames:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] INSpeakableString [] speakableGroupNames);

		[MacCatalyst (13, 1)]
		[Export ("initWithRecipients:senders:searchTerms:attributes:dateTimeRange:identifiers:notificationIdentifiers:speakableGroupNames:conversationIdentifiers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] INPerson [] senders, [NullAllowed] string [] searchTerms, INMessageAttributeOptions attributes, [NullAllowed] INDateComponentsRange dateTimeRange, [NullAllowed] string [] identifiers, [NullAllowed] string [] notificationIdentifiers, [NullAllowed] INSpeakableString [] speakableGroupNames, [NullAllowed] string [] conversationIdentifiers);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INPerson [], INPerson [], string [], INMessageAttributeOptions, INDateComponentsRange, string [], string [], INSpeakableString [])' instead.")]
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
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SpeakableGroupNames' instead.")]
		[NullAllowed, Export ("groupNames", ArgumentSemantic.Copy)]
		string [] GroupNames { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SpeakableGroupNamesOperator' instead.")]
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

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("conversationIdentifiers", ArgumentSemantic.Copy)]
		string [] ConversationIdentifiers { get; }

		[MacCatalyst (13, 1)]
		[Export ("conversationIdentifiersOperator", ArgumentSemantic.Assign)]
		INConditionalOperator ConversationIdentifiersOperator { get; }
	}

	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSearchForMessagesIntentHandling {

		[Abstract]
		[Export ("handleSearchForMessages:completion:")]
		void HandleSearchForMessages (INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion);

		[Export ("confirmSearchForMessages:completion:")]
		void Confirm (INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion);

		[Export ("resolveRecipientsForSearchForMessages:withCompletion:")]
		void ResolveRecipients (INSearchForMessagesIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveSendersForSearchForMessages:withCompletion:")]
		void ResolveSenders (INSearchForMessagesIntent intent, Action<INPersonResolutionResult []> completion);

		[Export ("resolveAttributesForSearchForMessages:withCompletion:")]
		void ResolveAttributes (INSearchForMessagesIntent intent, Action<INMessageAttributeOptionsResolutionResult> completion);

		[Export ("resolveDateTimeRangeForSearchForMessages:withCompletion:")]
		void ResolveDateTimeRange (INSearchForMessagesIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveSpeakableGroupNames' instead.")]
		[Export ("resolveGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveGroupNames (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveSpeakableGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveSpeakableGroupNames (INSearchForMessagesIntent intent, Action<INSpeakableStringResolutionResult []> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSearchForMessagesIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSearchForMessagesIntentResponse">Apple documentation for <c>INSearchForMessagesIntentResponse</c></related>
	[NoMac]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to search for photos.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSearchForPhotosIntent">Apple documentation for <c>INSearchForPhotosIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
		void Confirm (INSearchForPhotosIntent intent, Action<INSearchForPhotosIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSearchForPhotosIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSearchForPhotosIntentResponse">Apple documentation for <c>INSearchForPhotosIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to send a message.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSendMessageIntent">Apple documentation for <c>INSendMessageIntent</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSendMessageIntent : UNNotificationContentProviding {

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithRecipients:outgoingMessageType:content:speakableGroupName:conversationIdentifier:serviceName:sender:attachments:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, INOutgoingMessageType outgoingMessageType, [NullAllowed] string content, [NullAllowed] INSpeakableString speakableGroupName, [NullAllowed] string conversationIdentifier, [NullAllowed] string serviceName, [NullAllowed] INPerson sender, [NullAllowed] INSendMessageAttachment [] attachments);

		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor (INPerson[], INOutgoingMessageType, string, INSpeakableString, string, string, INPerson, INSendMessageAttachment[])' instead.")]
		[Export ("initWithRecipients:content:speakableGroupName:conversationIdentifier:serviceName:sender:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] INSpeakableString speakableGroupName, [NullAllowed] string conversationIdentifier, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (INPerson [], string, INSpeakableString, string, string, INPerson)' instead.")]
		[Export ("initWithRecipients:content:groupName:serviceName:sender:")]
		NativeHandle Constructor ([NullAllowed] INPerson [] recipients, [NullAllowed] string content, [NullAllowed] string groupName, [NullAllowed] string serviceName, [NullAllowed] INPerson sender);

		[NullAllowed, Export ("recipients", ArgumentSemantic.Copy)]
		INPerson [] Recipients { get; }

		[iOS (14, 0)]
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
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SpeakableGroupNames' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SpeakableGroupNames' instead.")]
		[NullAllowed, Export ("groupName")]
		string GroupName { get; }

		[NullAllowed, Export ("serviceName")]
		string ServiceName { get; }

		[NullAllowed, Export ("sender", ArgumentSemantic.Copy)]
		INPerson Sender { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("attachments", ArgumentSemantic.Copy)]
		INSendMessageAttachment [] Attachments { get; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSendMessageIntentHandling {

		[Abstract]
		[Export ("handleSendMessage:completion:")]
		void HandleSendMessage (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

		[Export ("confirmSendMessage:completion:")]
		void Confirm (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion);

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
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveSpeakableGroupName' instead.")]
		[Export ("resolveGroupNameForSendMessage:withCompletion:")]
		void ResolveGroupName (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("resolveOutgoingMessageTypeForSendMessage:withCompletion:")]
		void ResolveOutgoingMessageType (INSendMessageIntent intent, Action<INOutgoingMessageTypeResolutionResult> completion);

		[NoMac] // The INSpeakableStringResolutionResult used as a parameter type is not available in macOS
		[MacCatalyst (13, 1)]
		[Export ("resolveSpeakableGroupNameForSendMessage:withCompletion:")]
		void ResolveSpeakableGroupName (INSendMessageIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSendMessageIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSendMessageIntentResponse">Apple documentation for <c>INSendMessageIntentResponse</c></related>
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

		[NoMac] // The INMessage type isn't available in macOS
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use the 'SentMessages' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use the 'SentMessages' property instead.")]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sentMessage", ArgumentSemantic.Copy)]
		INMessage SentMessage { get; set; }

		[NoMac]
		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[NullAllowed, Export ("sentMessages", ArgumentSemantic.Copy)]
		INMessage [] SentMessages { get; set; }
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to make a payment.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSendPaymentIntent">Apple documentation for <c>INSendPaymentIntent</c></related>
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
		void Confirm (INSendPaymentIntent intent, Action<INSendPaymentIntentResponse> completion);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolvePayee (INSendPaymentIntent, Action<INSendPaymentPayeeResolutionResult>)' instead.")]
		[Export ("resolvePayeeForSendPayment:withCompletion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INPersonResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolvePayeeForSendPayment:completion:")]
		void ResolvePayee (INSendPaymentIntent intent, Action<INSendPaymentPayeeResolutionResult> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSendPaymentIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSendPaymentIntentResponse">Apple documentation for <c>INSendPaymentIntentResponse</c></related>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to specify the source for audio playback.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetAudioSourceInCarIntent">Apple documentation for <c>INSetAudioSourceInCarIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetAudioSourceInCarIntentHandling {

		[Abstract]
		[Export ("handleSetAudioSourceInCar:completion:")]
		void HandleSetAudioSourceInCar (INSetAudioSourceInCarIntent intent, Action<INSetAudioSourceInCarIntentResponse> completion);

		[Export ("confirmSetAudioSourceInCar:completion:")]
		void Confirm (INSetAudioSourceInCarIntent intent, Action<INSetAudioSourceInCarIntentResponse> completion);

		[Export ("resolveAudioSourceForSetAudioSourceInCar:withCompletion:")]
		void ResolveAudioSource (INSetAudioSourceInCarIntent intent, Action<INCarAudioSourceResolutionResult> completion);

		[Export ("resolveRelativeAudioSourceReferenceForSetAudioSourceInCar:withCompletion:")]
		void ResolveRelativeAudioSourceReference (INSetAudioSourceInCarIntent intent, Action<INRelativeReferenceResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetAudioSourceInCarIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetAudioSourceInCarIntentResponse">Apple documentation for <c>INSetAudioSourceInCarIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to control the climate.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetClimateSettingsInCarIntent">Apple documentation for <c>INSetClimateSettingsInCarIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

		[BindAs (typeof (int?))]
		[NullAllowed, Export ("fanSpeedIndex", ArgumentSemantic.Copy)]
		NSNumber FanSpeedIndex { get; }

		[BindAs (typeof (double?))]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetClimateSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetClimateSettingsInCar:completion:")]
		void HandleSetClimateSettingsInCar (INSetClimateSettingsInCarIntent intent, Action<INSetClimateSettingsInCarIntentResponse> completion);

		[Export ("confirmSetClimateSettingsInCar:completion:")]
		void Confirm (INSetClimateSettingsInCarIntent intent, Action<INSetClimateSettingsInCarIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetClimateSettingsInCarIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetClimateSettingsInCarIntentResponse">Apple documentation for <c>INSetClimateSettingsInCarIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to control the defroster.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetDefrosterSettingsInCarIntent">Apple documentation for <c>INSetDefrosterSettingsInCarIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetDefrosterSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetDefrosterSettingsInCar:completion:")]
		void HandleSetDefrosterSettingsInCar (INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

		[Export ("confirmSetDefrosterSettingsInCar:completion:")]
		void Confirm (INSetDefrosterSettingsInCarIntent intent, Action<INSetDefrosterSettingsInCarIntentResponse> completion);

		[Export ("resolveEnableForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveEnable (INSetDefrosterSettingsInCarIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveDefrosterForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveDefroster (INSetDefrosterSettingsInCarIntent intent, Action<INCarDefrosterResolutionResult> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveCarNameForSetDefrosterSettingsInCar:withCompletion:")]
		void ResolveCarName (INSetDefrosterSettingsInCarIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetDefrosterSettingsInCarIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetDefrosterSettingsInCarIntentResponse">Apple documentation for <c>INSetDefrosterSettingsInCarIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to set a message characteristic.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetMessageAttributeIntent">Apple documentation for <c>INSetMessageAttributeIntent</c></related>
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSetMessageAttributeIntentHandling {

		[Abstract]
		[Export ("handleSetMessageAttribute:completion:")]
		void HandleSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("confirmSetMessageAttribute:completion:")]
		void Confirm (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("resolveAttributeForSetMessageAttribute:withCompletion:")]
		void ResolveAttribute (INSetMessageAttributeIntent intent, Action<INMessageAttributeResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetMessageAttributeIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetMessageAttributeIntentResponse">Apple documentation for <c>INSetMessageAttributeIntentResponse</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to choose a particular user profile.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetProfileInCarIntent">Apple documentation for <c>INSetProfileInCarIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntent))]
	interface INSetProfileInCarIntent {
		[Protected]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'INSpeakableString carName'.")]
		[Export ("initWithProfileNumber:profileName:defaultProfile:")]
#if XAMCORE_5_0
		NativeHandle Constructor ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileName, [NullAllowed] NSNumber defaultProfile);
#else
		NativeHandle Constructor ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel, [NullAllowed] NSNumber defaultProfile);
#endif

		[MacCatalyst (13, 1)]
		[Export ("initWithProfileNumber:profileName:defaultProfile:carName:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (int?))] NSNumber profileNumber, [NullAllowed] string profileName, [NullAllowed][BindAs (typeof (bool?))] NSNumber defaultProfile, [NullAllowed] INSpeakableString carName);

		[BindAs (typeof (int?))]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSetProfileInCar:completion:")]
		void HandleSetProfileInCar (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("confirmSetProfileInCar:completion:")]
		void Confirm (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetProfileInCarIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetProfileInCarIntentResponse">Apple documentation for <c>INSetProfileInCarIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to choose a station.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetRadioStationIntent">Apple documentation for <c>INSetRadioStationIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetRadioStationIntentHandling {

		[Abstract]
		[Export ("handleSetRadioStation:completion:")]
		void HandleSetRadioStation (INSetRadioStationIntent intent, Action<INSetRadioStationIntentResponse> completion);

		[Export ("confirmSetRadioStation:completion:")]
		void Confirm (INSetRadioStationIntent intent, Action<INSetRadioStationIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetRadioStationIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetRadioStationIntentResponse">Apple documentation for <c>INSetRadioStationIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to modify the seat settings.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetSeatSettingsInCarIntent">Apple documentation for <c>INSetSeatSettingsInCarIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

		[BindAs (typeof (int?))]
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
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSetSeatSettingsInCarIntentHandling {

		[Abstract]
		[Export ("handleSetSeatSettingsInCar:completion:")]
		void HandleSetSeatSettingsInCar (INSetSeatSettingsInCarIntent intent, Action<INSetSeatSettingsInCarIntentResponse> completion);

		[Export ("confirmSetSeatSettingsInCar:completion:")]
		void Confirm (INSetSeatSettingsInCarIntent intent, Action<INSetSeatSettingsInCarIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetSeatSettingsInCarIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSetSeatSettingsInCarIntentResponse">Apple documentation for <c>INSetSeatSettingsInCarIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INShareFocusStatusIntent {
		[Export ("initWithFocusStatus:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INFocusStatus focusStatus);

		[NullAllowed, Export ("focusStatus", ArgumentSemantic.Copy)]
		INFocusStatus FocusStatus { get; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface INShareFocusStatusIntentHandling {
		[Abstract]
		[Export ("handleShareFocusStatus:completion:")]
		void HandleShareFocusStatus (INShareFocusStatusIntent intent, Action<INShareFocusStatusIntentResponse> completion);

		[Export ("confirmShareFocusStatus:completion:")]
		void ConfirmShareFocusStatus (INShareFocusStatusIntent intent, Action<INShareFocusStatusIntentResponse> completion);
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum INShareFocusStatusIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
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
		[Abstract]
		[NullAllowed, Export ("vocabularyIdentifier")]
		string VocabularyIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Abstract]
		[NullAllowed, Export ("alternativeSpeakableMatches")]
		IINSpeakable [] AlternativeSpeakableMatches { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'VocabularyIdentifier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VocabularyIdentifier' instead.")]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpeakableString : INSpeakable, NSCopying, NSSecureCoding {

		[MacCatalyst (13, 1)]
		[Export ("initWithVocabularyIdentifier:spokenPhrase:pronunciationHint:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, string spokenPhrase, [NullAllowed] string pronunciationHint);

		[MacCatalyst (13, 1)]
		[Export ("initWithSpokenPhrase:")]
		NativeHandle Constructor (string spokenPhrase);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving arbitrary strings.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INSpeakableStringResolutionResult">Apple documentation for <c>INSpeakableStringResolutionResult</c></related>
	[NoMac]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSpeakableStringResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSpeakableStringResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to start an audio call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartAudioCallIntent">Apple documentation for <c>INStartAudioCallIntent</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntent' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntent' instead.")]
	[BaseType (typeof (INIntent))]
	interface INStartAudioCallIntent {

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (INCallDestinationType, INPerson [])' instead.")]
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

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentHandling' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentHandling' instead.")]
	[Protocol]
	interface INStartAudioCallIntentHandling {

		[Abstract]
		[Export ("handleStartAudioCall:completion:")]
		void HandleStartAudioCall (INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

		[Export ("confirmStartAudioCall:completion:")]
		void Confirm (INStartAudioCallIntent intent, Action<INStartAudioCallIntentResponse> completion);

		[MacCatalyst (13, 1)]
		[Export ("resolveDestinationTypeForStartAudioCall:withCompletion:")]
		void ResolveDestinationType (INStartAudioCallIntent intent, Action<INCallDestinationTypeResolutionResult> completion);

		[Export ("resolveContactsForStartAudioCall:withCompletion:")]
		void ResolveContacts (INStartAudioCallIntent intent, Action<INPersonResolutionResult []> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINStartAudioCallIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartAudioCallIntentResponse">Apple documentation for <c>INStartAudioCallIntentResponse</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponse' instead.")]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to begin a slide show.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartPhotoPlaybackIntent">Apple documentation for <c>INStartPhotoPlaybackIntent</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
		void Confirm (INStartPhotoPlaybackIntent intent, Action<INStartPhotoPlaybackIntentResponse> completion);

		[Export ("resolveDateCreatedForStartPhotoPlayback:withCompletion:")]
		void ResolveDateCreated (INStartPhotoPlaybackIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveLocationCreatedForStartPhotoPlayback:withCompletion:")]
		void ResolveLocationCreated (INStartPhotoPlaybackIntent intent, Action<INPlacemarkResolutionResult> completion);

		[Export ("resolveAlbumNameForStartPhotoPlayback:withCompletion:")]
		void ResolveAlbumName (INStartPhotoPlaybackIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolvePeopleInPhotoForStartPhotoPlayback:withCompletion:")]
		void ResolvePeopleInPhoto (INStartPhotoPlaybackIntent intent, Action<INPersonResolutionResult []> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINStartPhotoPlaybackIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartPhotoPlaybackIntentResponse">Apple documentation for <c>INStartPhotoPlaybackIntentResponse</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to start a video call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartVideoCallIntent">Apple documentation for <c>INStartVideoCallIntent</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntent' instead.")]
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

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentHandling' instead.")]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'INStartCallIntentHandling' instead.")]
	[Protocol]
	interface INStartVideoCallIntentHandling {

		[Abstract]
		[Export ("handleStartVideoCall:completion:")]
		void HandleStartVideoCall (INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

		[Export ("confirmStartVideoCall:completion:")]
		void Confirm (INStartVideoCallIntent intent, Action<INStartVideoCallIntentResponse> completion);

		[Export ("resolveContactsForStartVideoCall:withCompletion:")]
		void ResolveContacts (INStartVideoCallIntent intent, Action<INPersonResolutionResult []> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINStartVideoCallIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartVideoCallIntentResponse">Apple documentation for <c>INStartVideoCallIntentResponse</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'INStartCallIntentResponse' instead.")]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to begin a workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartWorkoutIntent">Apple documentation for <c>INStartWorkoutIntent</c></related>
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

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("isOpenEnded", ArgumentSemantic.Copy)]
		NSNumber IsOpenEnded { get; }
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
		void Confirm (INStartWorkoutIntent intent, Action<INStartWorkoutIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINStartWorkoutIntentHandling" /> interface implementations populate with their extension's results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStartWorkoutIntentResponse">Apple documentation for <c>INStartWorkoutIntentResponse</c></related>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving arbitrary strings.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INStringResolutionResult">Apple documentation for <c>INStringResolutionResult</c></related>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INStringResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INStringResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in temperature-related  interactions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INTemperatureResolutionResult">Apple documentation for <c>INTemperatureResolutionResult</c></related>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Deprecated (PlatformName.MacOSX, 12, 0)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTemperatureResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTemperatureResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>Holds terms and conditions relevant to restaurant reservations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INTermsAndConditions">Apple documentation for <c>INTermsAndConditions</c></related>
	[Unavailable (PlatformName.MacOSX)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in workout-related  interactions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INWorkoutGoalUnitTypeResolutionResult">Apple documentation for <c>INWorkoutGoalUnitTypeResolutionResult</c></related>
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutGoalUnitTypeResolutionResult bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutGoalUnitTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedWorkoutGoalUnitType:")]
		INWorkoutGoalUnitTypeResolutionResult GetSuccess (INWorkoutGoalUnitType resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithWorkoutGoalUnitTypeToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INWorkoutGoalUnitTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INWorkoutGoalUnitTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in workout-related  interactions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/Intents/INWorkoutLocationTypeResolutionResult">Apple documentation for <c>INWorkoutLocationTypeResolutionResult</c></related>
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutLocationTypeResolutionResult bound
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutLocationTypeResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedWorkoutLocationType:")]
		INWorkoutLocationTypeResolutionResult GetSuccess (INWorkoutLocationType resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithWorkoutLocationTypeToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INWorkoutLocationTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
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

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("interaction")]
		INInteraction GetInteraction ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("suggestedInvocationPhrase")]
		[return: NullAllowed]
		string GetSuggestedInvocationPhrase ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setSuggestedInvocationPhrase:")]
		void SetSuggestedInvocationPhrase ([NullAllowed] string suggestedInvocationPhrase);

		[NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("shortcutAvailability")]
		INShortcutAvailabilityOptions GetShortcutAvailability ();

		[NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setShortcutAvailability:")]
		void SetShortcutAvailability (INShortcutAvailabilityOptions shortcutAvailabilityOptions);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> for causing a car to make its presence known by flashing its lights or honking its horn.</summary>
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
		void Confirm (INActivateCarSignalIntent intent, Action<INActivateCarSignalIntentResponse> completion);

		[Export ("resolveCarNameForActivateCarSignal:withCompletion:")]
		void ResolveCarName (INActivateCarSignalIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveSignalsForActivateCarSignal:withCompletion:")]
		void ResolveSignals (INActivateCarSignalIntent intent, Action<INCarSignalOptionsResolutionResult> completion);
	}

	/// <summary>Information relating to a bill.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>The entity to which a bill payment is made.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving bill payments.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBillPayeeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBillPayeeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving bill payments.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INBillTypeResolutionResult {
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedBillType:")]
		INBillTypeResolutionResult GetSuccess (INBillType resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithBillTypeToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBillTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBillTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving a car signaling its whereabouts.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCarSignalOptionsResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedCarSignalOptions:")]
		INCarSignalOptionsResolutionResult GetSuccess (INCarSignalOptions resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithCarSignalOptionsToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCarSignalOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCarSignalOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> for retrieving information on a car’s locks.</summary>
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
		void Confirm (INGetCarLockStatusIntent intent, Action<INGetCarLockStatusIntentResponse> completion);

		[Export ("resolveCarNameForGetCarLockStatus:withCompletion:")]
		void ResolveCarName (INGetCarLockStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetCarLockStatusIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("locked", ArgumentSemantic.Copy)]
		NSNumber Locked { get; set; }
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> for retrieving the current power level of a car.</summary>
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("startSendingUpdatesForGetCarPowerLevelStatus:toObserver:")]
		void StartSendingUpdates (INGetCarPowerLevelStatusIntent intent, IINGetCarPowerLevelStatusIntentResponseObserver observer);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("stopSendingUpdatesForGetCarPowerLevelStatus:")]
		void StopSendingUpdates (INGetCarPowerLevelStatusIntent intent);

		[Export ("confirmGetCarPowerLevelStatus:completion:")]
		void Confirm (INGetCarPowerLevelStatusIntent intent, Action<INGetCarPowerLevelStatusIntentResponse> completion);

		[Export ("resolveCarNameForGetCarPowerLevelStatus:withCompletion:")]
		void ResolveCarName (INGetCarPowerLevelStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	interface IINGetCarPowerLevelStatusIntentResponseObserver { }

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface INGetCarPowerLevelStatusIntentResponseObserver {

		[Abstract]
		[Export ("getCarPowerLevelStatusResponseDidUpdate:")]
		void DidUpdate (INGetCarPowerLevelStatusIntentResponse response);
	}

	// Just to please the generator that at this point does not know the hierarchy
	interface NSUnitLength : NSUnit { }

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSendPaymentIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("carIdentifier")]
		string CarIdentifier { get; set; }

		[BindAs (typeof (float?))]
		[NullAllowed, Export ("fuelPercentRemaining", ArgumentSemantic.Copy)]
		NSNumber FuelPercentRemaining { get; set; }

		[BindAs (typeof (float?))]
		[NullAllowed, Export ("chargePercentRemaining", ArgumentSemantic.Copy)]
		NSNumber ChargePercentRemaining { get; set; }

		[NullAllowed, Export ("distanceRemaining", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemaining { get; set; }

		[MacCatalyst (13, 1)]
		[BindAs (typeof (bool?))]
		[NullAllowed, Export ("charging", ArgumentSemantic.Copy)]
		NSNumber Charging { get; set; }

		[MacCatalyst (13, 1)]
		[BindAs (typeof (double?))]
		[NullAllowed, Export ("minutesToFull", ArgumentSemantic.Copy)]
		NSNumber MinutesToFull { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumDistance", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> MaximumDistance { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("distanceRemainingElectric", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemainingElectric { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumDistanceElectric", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> MaximumDistanceElectric { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("distanceRemainingFuel", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemainingFuel { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumDistanceFuel", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> MaximumDistanceFuel { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("consumptionFormulaArguments", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ConsumptionFormulaArguments { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("chargingFormulaArguments", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ChargingFormulaArguments { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("dateOfLastStateUpdate", ArgumentSemantic.Copy)]
		NSDateComponents DateOfLastStateUpdate { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[BindAs (typeof (INCarChargingConnectorType))]
		[NullAllowed, Export ("activeConnector")]
		NSString ActiveConnector { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("maximumBatteryCapacity", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitEnergy> MaximumBatteryCapacity { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("currentBatteryCapacity", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitEnergy> CurrentBatteryCapacity { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("minimumBatteryCapacity", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitEnergy> MinimumBatteryCapacity { get; set; }
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> for paying a bill.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INPayBillIntentHandling {
		[Abstract]
		[Export ("handlePayBill:completion:")]
		void HandlePayBill (INPayBillIntent intent, Action<INPayBillIntentResponse> completion);

		[Export ("confirmPayBill:completion:")]
		void Confirm (INPayBillIntent intent, Action<INPayBillIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINPayBillIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Account details for a payment intent.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface INPaymentAccount : NSCopying, NSSecureCoding {

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving payments.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentAccountResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentAccountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>Details on the amount of a payment intention.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving payments.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions involving payments.</summary>
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPaymentStatusResolutionResult {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("successWithResolvedPaymentStatus:")]
		INPaymentStatusResolutionResult GetSuccess (INPaymentStatus resolvedValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithPaymentStatusToConfirm:")]
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

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INPaymentStatusResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INPaymentStatusResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> for locating bills.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
	[Unavailable (PlatformName.MacOSX)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0)]
	[Protocol]
	interface INSearchForBillsIntentHandling {
		[Abstract]
		[Export ("handleSearchForBills:completion:")]
		void HandleSearch (INSearchForBillsIntent intent, Action<INSearchForBillsIntentResponse> completion);

		[Export ("confirmSearchForBills:completion:")]
		void Confirm (INSearchForBillsIntent intent, Action<INSearchForBillsIntentResponse> completion);

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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSearchForBillsIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>The name and desired locked/unlocked state of a car.</summary>
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

		[BindAs (typeof (bool?))]
		[Export ("locked", ArgumentSemantic.Copy), NullAllowed]
		NSNumber Locked { get; }

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
		void Confirm (INSetCarLockStatusIntent intent, Action<INSetCarLockStatusIntentResponse> completion);

		[Export ("resolveLockedForSetCarLockStatus:withCompletion:")]
		void ResolveLocked (INSetCarLockStatusIntent intent, Action<INBooleanResolutionResult> completion);

		[Export ("resolveCarNameForSetCarLockStatus:withCompletion:")]
		void ResolveCarName (INSetCarLockStatusIntent intent, Action<INSpeakableStringResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetCarLockStatusIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINActivateCarSignalIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in account-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INAccountTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INAccountTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to add a task to a list.</summary>
	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INAddTasksIntent {

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithTargetTaskList:taskTitles:spatialEventTrigger:temporalEventTrigger:priority:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INTaskList targetTaskList, [NullAllowed] INSpeakableString [] taskTitles, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger, INTaskPriority priority);

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

		[iOS (13, 0)]
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
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveTargetTaskList (Action<INAddTasksTargetTaskListResolutionResult>)' overload instead.")]
		[Export ("resolveTargetTaskListForAddTasks:withCompletion:")]
		void ResolveTargetTaskList (INAddTasksIntent intent, Action<INTaskListResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTargetTaskListForAddTasks:completion:")]
		void ResolveTargetTaskList (INAddTasksIntent intent, Action<INAddTasksTargetTaskListResolutionResult> completionHandler);

		[Export ("resolveTaskTitlesForAddTasks:withCompletion:")]
		void ResolveTaskTitles (INAddTasksIntent intent, Action<INSpeakableStringResolutionResult []> completion);

		[Export ("resolveSpatialEventTriggerForAddTasks:withCompletion:")]
		void ResolveSpatialEventTrigger (INAddTasksIntent intent, Action<INSpatialEventTriggerResolutionResult> completion);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ResolveTemporalEventTrigger (Action<INAddTasksTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveTemporalEventTrigger (Action<INAddTasksTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Export ("resolveTemporalEventTriggerForAddTasks:withCompletion:")]
		void ResolveTemporalEventTrigger (INAddTasksIntent intent, Action<INTemporalEventTriggerResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTemporalEventTriggerForAddTasks:completion:")]
		void ResolveTemporalEventTrigger (INAddTasksIntent intent, Action<INAddTasksTemporalEventTriggerResolutionResult> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePriorityForAddTasks:withCompletion:")]
		void ResolvePriority (INAddTasksIntent intent, Action<INTaskPriorityResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINAddTasksIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to append content to a note.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINAppendToNoteIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>A balance for an account.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions related to account balances.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INBalanceTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INBalanceTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in interactions related to call destinations.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallDestinationTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallDestinationTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>Information about a past call.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INCallRecord : NSCopying, NSSecureCoding {

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("initWithIdentifier:dateCreated:callRecordType:callCapability:callDuration:unseen:participants:numberOfCalls:isCallerIdBlocked:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen, [NullAllowed] INPerson [] participants, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfCalls, [NullAllowed][BindAs (typeof (bool?))] NSNumber isCallerIdBlocked);

		[Deprecated (PlatformName.iOS, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?, int?)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?, int?)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 3, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?, int?)' instead.")]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:dateCreated:caller:callRecordType:callCapability:callDuration:unseen:numberOfCalls:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, [NullAllowed] INPerson caller, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfCalls);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("initWithIdentifier:dateCreated:callRecordType:callCapability:callDuration:unseen:numberOfCalls:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen, [NullAllowed][BindAs (typeof (int?))] NSNumber numberOfCalls);

		[Deprecated (PlatformName.iOS, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 5, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 3, message: "Use '.ctor (string, NSDate, INCallRecordType, INCallCapability, double?, bool?)' instead.")]
		[Export ("initWithIdentifier:dateCreated:caller:callRecordType:callCapability:callDuration:unseen:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, [NullAllowed] INPerson caller, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed] NSNumber callDuration, [NullAllowed] NSNumber unseen);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("initWithIdentifier:dateCreated:callRecordType:callCapability:callDuration:unseen:")]
		NativeHandle Constructor (string identifier, [NullAllowed] NSDate dateCreated, INCallRecordType callRecordType, INCallCapability callCapability, [NullAllowed][BindAs (typeof (double?))] NSNumber callDuration, [NullAllowed][BindAs (typeof (bool?))] NSNumber unseen);

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("dateCreated", ArgumentSemantic.Copy)]
		NSDate DateCreated { get; }

		[Deprecated (PlatformName.iOS, 14, 5)]
		[Deprecated (PlatformName.MacOSX, 12, 0)]
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
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("numberOfCalls", ArgumentSemantic.Copy)]
		NSNumber NumberOfCalls { get; }

		[BindAs (typeof (bool?))]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("isCallerIdBlocked", ArgumentSemantic.Copy)]
		NSNumber IsCallerIdBlocked { get; }

		[NullAllowed]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("participants", ArgumentSemantic.Copy)]
		INPerson [] Participants { get; }
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in call recording-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INCallRecordTypeOptionsResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INCallRecordTypeOptionsResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to cancel a ride.</summary>
	[NoMac, NoTV]
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

	[NoMac, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INCancelRideIntentHandling {

		[Abstract]
		[Export ("handleCancelRide:completion:")]
		void HandleCancelRide (INCancelRideIntent intent, Action<INCancelRideIntentResponse> completion);

		[Export ("confirmCancelRide:completion:")]
		void Confirm (INCancelRideIntent intent, Action<INCancelRideIntentResponse> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINCancelRideIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[NoMac, NoTV]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to create a new note.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINCreateNoteIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to create a new task list.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINCreateTaskListIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INDateSearchTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INDateSearchTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to get a bar or QR code for payment or contact information.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINGetVisualCodeIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>Represents an image within a note.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INNoteContent))]
	interface INImageNoteContent : NSSecureCoding, NSCopying {

		[Export ("initWithImage:")]
		NativeHandle Constructor (INImage image);

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		INImage Image { get; }
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in location search-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INLocationSearchTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INLocationSearchTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>Note content for a single note in an app.</summary>
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

	/// <summary>Base class for note content.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface INNoteContent : NSSecureCoding, NSCopying {
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in note content-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNoteContentResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNoteContentResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in note content type-related interactions.</summary>
	[Deprecated (PlatformName.iOS, 13, 0, message: "Not used anymore.")]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNoteContentTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNoteContentTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in note-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNoteResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNoteResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in notebook item type-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INNotebookItemTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INNotebookItemTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>Represents a custom interface parameter for a developer-defined Siri interaction.</summary>
	[NoMac]
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

	/// <summary>A repetition rule for date ranges.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRecurrenceRule : NSCopying, NSSecureCoding {

		[Export ("initWithInterval:frequency:")]
		NativeHandle Constructor (nuint interval, INRecurrenceFrequency frequency);

		[NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithInterval:frequency:weeklyRecurrenceDays:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nuint interval, INRecurrenceFrequency frequency, INDayOfWeekOptions weeklyRecurrenceDays);

		[Export ("interval")]
		nuint Interval { get; }

		[Export ("frequency")]
		INRecurrenceFrequency Frequency { get; }

		[NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("weeklyRecurrenceDays")]
		INDayOfWeekOptions WeeklyRecurrenceDays { get; }
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in currency-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRequestPaymentCurrencyAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRequestPaymentCurrencyAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in payer-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INRequestPaymentPayerResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INRequestPaymentPayerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to search for accounts information.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSearchForAccountsIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to search for notes, tasks, or reminders.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSearchForNotebookItemsIntent {

		[Deprecated (PlatformName.iOS, 11, 2, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the constructor with 'string notebookItemIdentifier' instead.")]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the constructor with 'INTemporalEventTriggerTypeOptions temporalEventTriggerTypes' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the constructor with 'INTemporalEventTriggerTypeOptions temporalEventTriggerTypes' instead.")]
		[Export ("initWithTitle:content:itemType:status:location:locationSearchType:dateTime:dateSearchType:notebookItemIdentifier:")]
		NativeHandle Constructor ([NullAllowed] INSpeakableString title, [NullAllowed] string content, INNotebookItemType itemType, INTaskStatus status, [NullAllowed] CLPlacemark location, INLocationSearchType locationSearchType, [NullAllowed] INDateComponentsRange dateTime, INDateSearchType dateSearchType, [NullAllowed] string notebookItemIdentifier);

		[iOS (13, 0)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("temporalEventTriggerTypes", ArgumentSemantic.Assign)]
		INTemporalEventTriggerTypeOptions TemporalEventTriggerTypes { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("taskPriority", ArgumentSemantic.Assign)]
		INTaskPriority TaskPriority { get; }

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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTemporalEventTriggerTypesForSearchForNotebookItems:withCompletion:")]
		void ResolveTemporalEventTriggerTypes (INSearchForNotebookItemsIntent intent, Action<INTemporalEventTriggerTypeOptionsResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTaskPriorityForSearchForNotebookItems:withCompletion:")]
		void ResolveTaskPriority (INSearchForNotebookItemsIntent intent, Action<INTaskPriorityResolutionResult> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSearchForNotebookItemsIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in recipient-related message send interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSendMessageRecipientResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSendMessageRecipientResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in currency-related interactions for sending payments.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSendPaymentCurrencyAmountResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSendPaymentCurrencyAmountResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in payee-related interactions for sending payments.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSendPaymentPayeeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSendPaymentPayeeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to send feedback about a ride.</summary>
	[NoTV, NoMac]
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

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INSendRideFeedbackIntentHandling {

		[Abstract]
		[Export ("handleSendRideFeedback:completion:")]
		void HandleSendRideFeedback (INSendRideFeedbackIntent sendRideFeedbackintent, Action<INSendRideFeedbackIntentResponse> completion);

		[Export ("confirmSendRideFeedback:completion:")]
		void Confirm (INSendRideFeedbackIntent sendRideFeedbackIntent, Action<INSendRideFeedbackIntentResponse> completion);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSendRideFeedbackIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[NoTV, NoMac]
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to modify a task attribute, for example, by marking a task complete.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	interface INSetTaskAttributeIntent {

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithTargetTask:taskTitle:status:priority:spatialEventTrigger:temporalEventTrigger:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INTask targetTask, [NullAllowed] INSpeakableString taskTitle, INTaskStatus status, INTaskPriority priority, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'INTaskPriority priority' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'INTaskPriority priority' overload instead.")]
		[Export ("initWithTargetTask:status:spatialEventTrigger:temporalEventTrigger:")]
		NativeHandle Constructor ([NullAllowed] INTask targetTask, INTaskStatus status, [NullAllowed] INSpatialEventTrigger spatialEventTrigger, [NullAllowed] INTemporalEventTrigger temporalEventTrigger);

		[NullAllowed, Export ("targetTask", ArgumentSemantic.Copy)]
		INTask TargetTask { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("taskTitle", ArgumentSemantic.Copy)]
		INSpeakableString TaskTitle { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		INTaskStatus Status { get; }

		[iOS (13, 0)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTaskTitleForSetTaskAttribute:withCompletion:")]
		void ResolveTaskTitle (INSetTaskAttributeIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveStatusForSetTaskAttribute:withCompletion:")]
		void ResolveStatus (INSetTaskAttributeIntent intent, Action<INTaskStatusResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePriorityForSetTaskAttribute:withCompletion:")]
		void ResolvePriority (INSetTaskAttributeIntent intent, Action<INTaskPriorityResolutionResult> completion);

		[Export ("resolveSpatialEventTriggerForSetTaskAttribute:withCompletion:")]
		void ResolveSpatialEventTrigger (INSetTaskAttributeIntent intent, Action<INSpatialEventTriggerResolutionResult> completion);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ResolveTemporalEventTrigger (INSetTaskAttributeIntent Action<INSetTaskAttributeTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ResolveTemporalEventTrigger (INSetTaskAttributeIntent Action<INSetTaskAttributeTemporalEventTriggerResolutionResult>)' overload instead.")]
		[Export ("resolveTemporalEventTriggerForSetTaskAttribute:withCompletion:")]
		void ResolveTemporalEventTrigger (INSetTaskAttributeIntent intent, Action<INTemporalEventTriggerResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveTemporalEventTriggerForSetTaskAttribute:completion:")]
		void ResolveTemporalEventTrigger (INSetTaskAttributeIntent intent, Action<INSetTaskAttributeTemporalEventTriggerResolutionResult> completionHandler);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINSetTaskAttributeIntentHandling" /> interface implementations populate with their extension's results.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in spatial event trigger-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INSpatialEventTriggerResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INSpatialEventTriggerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>A task for the user.</summary>
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTask : NSCopying, NSSecureCoding {

		[iOS (13, 0)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("priority", ArgumentSemantic.Assign)]
		INTaskPriority Priority { get; }
	}

	/// <summary>A list of tasks for the user.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in task list-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskListResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskListResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in task-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in task status-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTaskStatusResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTaskStatusResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>A time-based reminder trigger for a user task.</summary>
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in temporal event trigger-related interactions.</summary>
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INTemporalEventTriggerResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INTemporalEventTriggerResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	/// <summary>The text of a note.</summary>
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

	/// <summary>An <see cref="T:Intents.INIntent" /> indicating the user wishes to transfer funds.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResponse" /> subclass that developers of <see cref="T:Intents.IINTransferMoneyIntentHandling" /> interface implementations populate with their extension's results.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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

	/// <summary>
	///       <see cref="T:Intents.INIntentResolutionResult" /> for resolving parameters in visual code type-related interactions.</summary>
	[Deprecated (PlatformName.iOS, 15, 0)]
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
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unsupportedWithReason:")]
		INVisualCodeTypeResolutionResult GetUnsupported (nint reason);

		[New]
		[iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("confirmationRequiredWithItemToConfirm:forReason:")]
		INVisualCodeTypeResolutionResult GetConfirmationRequired (NSObject itemToConfirm, nint reason);
	}

	[NoMac, NoTV]
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

	[TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMediaItem : NSCopying, NSSecureCoding {

		[iOS (13, 0)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("artist")]
		string Artist { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INObject : INSpeakable, NSCopying, NSSecureCoding {

		[Export ("initWithIdentifier:displayString:pronunciationHint:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString, [NullAllowed] string pronunciationHint);

		[Export ("initWithIdentifier:displayString:")]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithIdentifier:displayString:subtitleString:displayImage:")]
		NativeHandle Constructor ([NullAllowed] string identifier, string displayString, [NullAllowed] string subtitleString, [NullAllowed] INImage displayImage);

		[iOS (14, 0)]
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
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("alternativeSpeakableMatches")]
		[return: NullAllowed]
		INSpeakableString [] GetAlternativeSpeakableMatches ();

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("subtitleString")]
		string SubtitleString { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("displayImage", ArgumentSemantic.Strong)]
		INImage DisplayImage { get; set; }

		// Not [Sealed] since the 'AlternativeSpeakableMatches' inlined property is read-only
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setAlternativeSpeakableMatches:")]
		void SetAlternativeSpeakableMatches ([NullAllowed] INSpeakableString [] alternativeSpeakableMatches);
	}

	[TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INPlayMediaIntent {

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithMediaItems:mediaContainer:playShuffled:playbackRepeatMode:resumePlayback:playbackQueueLocation:playbackSpeed:mediaSearch:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INMediaItem [] mediaItems, [NullAllowed] INMediaItem mediaContainer, [NullAllowed, BindAs (typeof (bool?))] NSNumber playShuffled, INPlaybackRepeatMode playbackRepeatMode, [NullAllowed, BindAs (typeof (bool?))] NSNumber resumePlayback, INPlaybackQueueLocation playbackQueueLocation, [NullAllowed, BindAs (typeof (double?))] NSNumber playbackSpeed, [NullAllowed] INMediaSearch mediaSearch);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the '.ctor (INMediaItem [], INMediaItem, bool?, INPlaybackRepeatMode, bool?, INPlaybackQueueLocation, double?, INMediaSearch)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the '.ctor (INMediaItem [], INMediaItem, bool?, INPlaybackRepeatMode, bool?, INPlaybackQueueLocation, double?, INMediaSearch)' instead.")]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playbackQueueLocation", ArgumentSemantic.Assign)]
		INPlaybackQueueLocation PlaybackQueueLocation { get; }

		[BindAs (typeof (double?))]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("playbackSpeed", ArgumentSemantic.Copy)]
		NSNumber PlaybackSpeed { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("mediaSearch", ArgumentSemantic.Copy)]
		INMediaSearch MediaSearch { get; }
	}

	[TV (14, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INPlayMediaIntentHandling {

		[Abstract]
		[Export ("handlePlayMedia:completion:")]
		void HandlePlayMedia (INPlayMediaIntent intent, Action<INPlayMediaIntentResponse> completion);

		[Export ("confirmPlayMedia:completion:")]
		void Confirm (INPlayMediaIntent intent, Action<INPlayMediaIntentResponse> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveMediaItemsForPlayMedia:withCompletion:")]
		void ResolveMediaItems (INPlayMediaIntent intent, Action<NSArray<INPlayMediaMediaItemResolutionResult>> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlayShuffledForPlayMedia:withCompletion:")]
		void ResolvePlayShuffled (INPlayMediaIntent intent, Action<INBooleanResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlaybackRepeatModeForPlayMedia:withCompletion:")]
		void ResolvePlaybackRepeatMode (INPlayMediaIntent intent, Action<INPlaybackRepeatModeResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolveResumePlaybackForPlayMedia:withCompletion:")]
		void ResolveResumePlayback (INPlayMediaIntent intent, Action<INBooleanResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlaybackQueueLocationForPlayMedia:withCompletion:")]
		void ResolvePlaybackQueueLocation (INPlayMediaIntent intent, Action<INPlaybackQueueLocationResolutionResult> completion);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvePlaybackSpeedForPlayMedia:withCompletion:")]
		void ResolvePlaybackSpeed (INPlayMediaIntent intent, Action<INPlayMediaPlaybackSpeedResolutionResult> completion);
	}

	[TV (14, 0), NoMac]
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
	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRelevanceProvider : NSCopying, NSSecureCoding {
	}

	[NoTV, NoMac]
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

	[NoTV, NoMac]
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

	[NoTV, NoMac]
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

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INRelevantShortcut : NSSecureCoding, NSCopying {

		[Export ("relevanceProviders", ArgumentSemantic.Copy)]
		INRelevanceProvider [] RelevanceProviders { get; set; }

		[NullAllowed, Export ("watchTemplate", ArgumentSemantic.Copy)]
		INDefaultCardTemplate WatchTemplate { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoTV, NoMac]
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

	[NoTV]
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

	[NoTV, NoMac]
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

	[NoTV]
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

	[NoTV]
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
	//
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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
	[NoTV, NoMac, iOS (13, 0)]
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
	[NoTV, NoMac, iOS (13, 0)]
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
	[NoTV, NoMac, iOS (13, 0)]
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
	[NoTV, NoMac, iOS (13, 0)]
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
	[NoTV, NoMac, iOS (13, 0)]
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

	[iOS (13, 0), NoTV]
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

	[iOS (13, 0), NoTV]
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

	[NoTV, iOS (13, 0)]
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

		[Mac (12, 1), iOS (15, 2)]
		[MacCatalyst (15, 2)]
		[Export ("removedOnCompletion")]
		bool RemovedOnCompletion { get; set; }
	}

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INFlightReservation : NSCopying, NSSecureCoding {

		[iOS (14, 0)]
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

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum INFocusStatusAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INLodgingReservation : NSCopying, NSSecureCoding {

		[iOS (14, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INUserContext))]
	interface INMediaUserContext {

		[Export ("subscriptionStatus", ArgumentSemantic.Assign)]
		INMediaUserContextSubscriptionStatus SubscriptionStatus { get; set; }

		[BindAs (typeof (int?))]
		[NullAllowed, Export ("numberOfLibraryItems", ArgumentSemantic.Copy)]
		NSNumber NumberOfLibraryItems { get; set; }
	}

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INRentalCarReservation : NSCopying, NSSecureCoding {

		[iOS (14, 0)]
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

	[NoTV, iOS (13, 0)]
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }
	}

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INRestaurantReservation : NSCopying, NSSecureCoding {

		[iOS (14, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INIntent))]
	[DisableDefaultCtor]
	interface INStartCallIntent : UNNotificationContentProviding {

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithCallRecordFilter:callRecordToCallBack:audioRoute:destinationType:contacts:callCapability:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] INCallRecordFilter callRecordFilter, [NullAllowed] INCallRecord callRecordToCallBack, INCallAudioRoute audioRoute, INCallDestinationType destinationType, [NullAllowed] INPerson [] contacts, INCallCapability callCapability);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor (INCallRecordFilter, INCallRecord, INCallAudioRoute, INCallDestinationType, INPerson[], INCallCapability)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor (INCallRecordFilter, INCallRecord, INCallAudioRoute, INCallDestinationType, INPerson[], INCallCapability)' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use '.ctor (INCallRecordFilter, INCallRecord, INCallAudioRoute, INCallDestinationType, INPerson[], INCallCapability)' overload instead.")]
		[Export ("initWithAudioRoute:destinationType:contacts:recordTypeForRedialing:callCapability:")]
		NativeHandle Constructor (INCallAudioRoute audioRoute, INCallDestinationType destinationType, [NullAllowed] INPerson [] contacts, INCallRecordType recordTypeForRedialing, INCallCapability callCapability);

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("callRecordFilter", ArgumentSemantic.Copy)]
		INCallRecordFilter CallRecordFilter { get; }

		[NoTV, iOS (14, 0)]
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
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Export ("recordTypeForRedialing", ArgumentSemantic.Assign)]
		INCallRecordType RecordTypeForRedialing { get; }

		[Export ("callCapability", ArgumentSemantic.Assign)]
		INCallCapability CallCapability { get; }
	}

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface INStartCallIntentHandling {

		[Abstract]
		[Export ("handleStartCall:completion:")]
		void HandleStartCall (INStartCallIntent intent, Action<INStartCallIntentResponse> completion);

		[Export ("confirmStartCall:completion:")]
		void Confirm (INStartCallIntent intent, Action<INStartCallIntentResponse> completion);

		[NoTV, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
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

	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INTicketedEventReservation : NSCopying, NSSecureCoding {

		[iOS (14, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (INReservation))]
	[DisableDefaultCtor]
	interface INTrainReservation : NSCopying, NSSecureCoding {

		[iOS (14, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INUserContext : NSSecureCoding {

		[Export ("becomeCurrent")]
		void BecomeCurrent ();
	}

	[NoTV, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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

	[TV (14, 0), NoMac, iOS (13, 0)]
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



	[NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_ShareExtension {

		[return: NullAllowed]
		[Export ("intent")]
		INIntent GetIntent ();
	}

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (INIntent))]
	[DesignatedDefaultCtor]
	interface INListCarsIntent {

	}

	interface IINListCarsIntentHandling { }

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface INListCarsIntentHandling {

		[Abstract]
		[Export ("handleListCars:completion:")]
		void HandleListCars (INListCarsIntent intent, Action<INListCarsIntentResponse> completion);

		[Export ("confirmListCars:completion:")]
		void ConfirmListCars (INListCarsIntent intent, Action<INListCarsIntentResponse> completion);
	}

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 0)]
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

	[NoTV, iOS (14, 5)]
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

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
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

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[Protocol]
	interface INAnswerCallIntentHandling {

		[Abstract]
		[Export ("handleAnswerCall:completion:")]
		void HandleAnswerCall (INAnswerCallIntent intent, Action<INAnswerCallIntentResponse> completion);

		[Export ("confirmAnswerCall:completion:")]
		void ConfirmAnswerCall (INAnswerCallIntent intent, Action<INAnswerCallIntentResponse> completion);
	}

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
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

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
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

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[Protocol]
	interface INHangUpCallIntentHandling {

		[Abstract]
		[Export ("handleHangUpCall:completion:")]
		void HandleHangUpCall (INHangUpCallIntent intent, Action<INHangUpCallIntentResponse> completion);

		[Export ("confirmHangUpCall:completion:")]
		void ConfirmHangUpCall (INHangUpCallIntent intent, Action<INHangUpCallIntentResponse> completion);
	}

	[NoTV, Mac (13, 1), iOS (16, 2), MacCatalyst (16, 2)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INHangUpCallIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INHangUpCallIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INHangUpCallIntentResponseCode Code { get; }
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INUnsendMessagesIntentResponse {
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INUnsendMessagesIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INUnsendMessagesIntentResponseCode Code { get; }
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntent))]
	interface INUnsendMessagesIntent {
		[Export ("initWithMessageIdentifiers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string [] messageIdentifiers);

		[NullAllowed, Export ("messageIdentifiers", ArgumentSemantic.Copy)]
		string [] MessageIdentifiers { get; }
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INEditMessageIntentResponse {
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INEditMessageIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INEditMessageIntentResponseCode Code { get; }
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface INUnsendMessagesIntentHandling {
		[Abstract]
		[Export ("handleUnsendMessages:completion:")]
		void HandleUnsendMessages (INUnsendMessagesIntent intent, Action<INUnsendMessagesIntentResponse> completion);

		[Export ("confirmUnsendMessages:completion:")]
		void ConfirmUnsendMessages (INUnsendMessagesIntent intent, Action<INUnsendMessagesIntentResponse> completion);
	}

	[iOS (18, 0), NoTV, MacCatalyst (18, 0), Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INMessageReaction : NSCopying, NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithReactionType:reactionDescription:emoji:")]
		NativeHandle Constructor (INMessageReactionType reactionType, [NullAllowed] string reactionDescription, [NullAllowed] string emoji);

		[Export ("reactionType", ArgumentSemantic.Assign)]
		INMessageReactionType ReactionType { get; }

		[Export ("reactionDescription", ArgumentSemantic.Copy), NullAllowed]
		string ReactionDescription { get; }

		[Export ("emoji", ArgumentSemantic.Copy), NullAllowed]
		string Emoji { get; }
	}

	[iOS (18, 0), NoTV, MacCatalyst (18, 0), Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSticker : NSCopying, NSSecureCoding {
		[Export ("initWithType:emoji:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INStickerType type, [NullAllowed] string emoji);

		[Export ("type", ArgumentSemantic.Assign)]
		INStickerType Type { get; }

		[Export ("emoji", ArgumentSemantic.Copy), NullAllowed]
		string Emoji { get; }
	}
}
