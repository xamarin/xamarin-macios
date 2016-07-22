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
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreLocation;
using XamCore.Contacts;

// TODO: 	Review all FIXME [Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)] with Introspection tests on macOS 10.12
//			Once we have this ^ we can add the same availability metadata to enums
// TODO: 	Enhance API marked with NS_REFINED_FOR_SWIFT, we should match what Swift exposes https://gist.github.com/dalexsoto/44e363dbeeac96e67849e7c56d9fcca0
//			This ^ will make the API prettier i.e. hiding `NSNumber EnableFan { get; }` and exposing a `bool? EnableFan { get; }` instead
//			We could add some generator support for that instead of manually fixing case by case something like WrapAttribute but also that works on getters/setters props
namespace XamCore.Intents {

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INBookRestaurantReservationIntentCode : nint {
		Success = 0,
		Denied,
		Failure,
		FailureRequiringAppLaunch,
		FailureRequiringAppLaunchMustVerifyCredentials,
		FailureRequiringAppLaunchServiceTemporarilyUnavailable
	}

	[Native]
	[Flags]
	public enum INCallCapabilityOptions : nuint {
		AudioCall = (1 << 0),
		VideoCall = (1 << 1)
	}

	[Native]
	public enum INCallRecordType : nint {
		Unknown = 0,
		Outgoing,
		Missed,
		Received
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
		Rear
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
		ThirdRow
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Native]
	public enum INConditionalOperator : nint {
		All = 0,
		Any,
		None
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
		InvalidUserVocabularyFileLocation = 4000
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Native]
	public enum INIntentHandlingStatus : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		DeferredToApplication,
		Done = Success
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Native]
	public enum INInteractionDirection : nint {
		Unspecified = 0,
		Outgoing,
		Incoming
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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

	[Native]
	public enum INMessageAttribute : nint {
		Unknown = 0,
		Read,
		Unread,
		Flagged,
		Unflagged
	}

	[Native]
	[Flags]
	public enum INMessageAttributeOptions : nuint {
		Read = (1 << 0),
		Unread = (1 << 1),
		Flagged = (1 << 2),
		Unflagged = (1 << 3)
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
		Store
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INPaymentStatus : nint {
		Unknown = 0,
		Pending,
		Completed,
		Canceled,
		Failed
	}

	[Native]
	public enum INPersonSuggestionType : nint {
		SocialProfile = 1,
		InstantMessageAddress
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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

	[Native]
	public enum INRelativeReference : nint {
		Unknown = 0,
		Next,
		Previous
	}

	[Native]
	public enum INRelativeSetting : nint {
		Unknown = 0,
		Lowest,
		Lower,
		Higher,
		Highest
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRequestPaymentIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Native]
	public enum INSearchCallHistoryIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSearchForPhotosIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Native]
	public enum INSendMessageIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INSendPaymentIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[Native]
	public enum INStartAudioCallIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INStartPhotoPlaybackIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Native]
	public enum INStartVideoCallIntentResponseCode : nint {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		Failure,
		FailureRequiringAppLaunch
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Native]
	public enum INVocabularyStringType : nint {
		ContactName = 1,
		ContactGroupName,
		PhotoTag = 100,
		PhotoAlbumName,
		WorkoutActivityName = 200,
		CarProfileName = 300
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-native-enum! INWorkoutLocationType bound
	[Native]
	public enum INWorkoutLocationType : nint {
		Unknown = 0,
		Outdoor,
		Indoor
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Native]
	public enum INPersonHandleType : nint {
		Unknown = 0,
		EmailAddress,
		PhoneNumber
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Native]
	public enum INRestaurantMarketingPreferencesContactMethod : nint {
		Undefined = 0,
		Email
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	public enum INIntentIdentifier {
		[Unavailable (PlatformName.MacOSX)]
		[Field ("INStartAudioCallIntentIdentifier")]
		StartAudioCall,

		[Unavailable (PlatformName.MacOSX)]
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
		GetRideStatus
	}

	// End of enums

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[BaseType (typeof (INIntent))]
	interface INBookRestaurantReservationIntent : NSCopying {

		[Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[NullAllowed, Export ("restaurantMarketingPreferences", ArgumentSemantic.Copy)]
		INRestaurantMarketingPreferences RestaurantMarketingPreferences { get; set; }

		[Export ("bookingDate", ArgumentSemantic.Copy)]
		NSDate BookingDate { get; set; }

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
	[Protocol]
	interface INBookRestaurantReservationIntentHandling {

		[Abstract]
		[Export ("handleBookRestaurantReservation:completion:")]
		void HandleBookRestaurantReservation (INBookRestaurantReservationIntent bookingIntent, Action<INBookRestaurantReservationIntentResponse> completion);

		[Export ("confirmBookRestaurantReservation:completion:")]
		void ConfirmBookRestaurantReservation (INBookRestaurantReservationIntent bookingIntent, Action<INBookRestaurantReservationIntentResponse> completion);

		[Export ("resolveRestaurantForBookRestaurantReservation:completion:")]
		void ResolveRestaurant (INBookRestaurantReservationIntent bookingIntent, Action<INIntentResolutionResult<INRestaurant>> completion);

		[Export ("resolveBookingDateForBookRestaurantReservation:completion:")]
		void ResolveBookingDate (INBookRestaurantReservationIntent bookingIntent, Action<INIntentResolutionResult<NSDate>> completion);

		[Export ("resolvePartySizeForBookRestaurantReservation:completion:")]
		void ResolvePartySize (INBookRestaurantReservationIntent bookingIntent, Action<INIntentResolutionResult<NSNumber>> completion);

		[Export ("resolveGuestForBookRestaurantReservation:completion:")]
		void ResolveGuest (INBookRestaurantReservationIntent bookingIntent, Action<INIntentResolutionResult<INRestaurantGuest>> completion);

		[Export ("resolveGuestProvidedSpecialRequestTextForBookRestaurantReservation:completion:")]
		void ResolveGuestProvidedSpecialRequest (INBookRestaurantReservationIntent bookingIntent, Action<INIntentResolutionResult<NSString>> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallCapabilityOptionsResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INCallCapabilityOptionsResolutionResult GetSuccess (INCallCapabilityOptions resolvedValue);


		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCallCapabilityOptionsResolutionResult GetConfirmationRequired (INCallCapabilityOptions valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INCallRecordTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INCallRecordTypeResolutionResult GetSuccess (INCallRecordType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INCallRecordTypeResolutionResult GetConfirmationRequired (INCallRecordType valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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

		[Static]
		[Export ("needsMoreDetailsForCurrencyAmount:")]
		INCurrencyAmountResolutionResult GetNeedsMoreDetails (INCurrencyAmount currencyAmountToComplete);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[Static]
		[Export ("needsMoreDetailsForDateComponentsRange:")]
		INDateComponentsRangeResolutionResult GetNeedsMoreDetails (INDateComponentsRange dateComponentsRangeToComplete);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Protocol]
	interface INCallsDomainHandling : INStartAudioCallIntentHandling, INStartVideoCallIntentHandling, INSearchCallHistoryIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INCarPlayDomainHandling : INSetAudioSourceInCarIntentHandling, INSetClimateSettingsInCarIntentHandling, INSetDefrosterSettingsInCarIntentHandling, INSetSeatSettingsInCarIntentHandling, INSetProfileInCarIntentHandling, INSaveProfileInCarIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INWorkoutsDomainHandling : INStartWorkoutIntentHandling, INPauseWorkoutIntentHandling, INEndWorkoutIntentHandling, INCancelWorkoutIntentHandling, INResumeWorkoutIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INRadioDomainHandling : INSetRadioStationIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INMessagesDomainHandling : INSendMessageIntentHandling, INSearchForMessagesIntentHandling, INSetMessageAttributeIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INPaymentsDomainHandling : INSendPaymentIntentHandling, INRequestPaymentIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INPhotosDomainHandling : INSearchForPhotosIntentHandling, INStartPhotoPlaybackIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INRidesharingDomainHandling : INListRideOptionsIntentHandling, INRequestRideIntentHandling, INGetRideStatusIntentHandling {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	interface INExtension : INIntentHandlerProviding {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntent {

		[NullAllowed, Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingDefaultsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void HandleAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent bookingDefaultsIntent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void ConfirmAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent bookingDefaultsIntent, Action<INGetAvailableRestaurantReservationBookingDefaultsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookingDefaults:completion:")]
		void ResolveAvailableRestaurantReservationBookingDefaults (INGetAvailableRestaurantReservationBookingDefaultsIntent bookingIntent, Action<INIntentResolutionResult<INRestaurant>> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

		[NullAllowed, Export ("restaurantMarketingPreferences", ArgumentSemantic.Copy)]
		INRestaurantMarketingPreferences RestaurantMarketingPreferences { get; set; }

		[Export ("initWithDefaultPartySize:defaultBookingDate:code:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (nuint defaultPartySize, NSDate defaultBookingDate, INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INGetAvailableRestaurantReservationBookingDefaultsIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INGetAvailableRestaurantReservationBookingsIntent : NSCopying {

		[Export ("restaurant", ArgumentSemantic.Copy)]
		INRestaurant Restaurant { get; set; }

		[Export ("partySize")]
		nuint PartySize { get; set; }

		[NullAllowed, Export ("preferredBookingDate", ArgumentSemantic.Copy)]
		NSDate PreferredBookingDate { get; set; }

		[NullAllowed, Export ("maximumNumberOfResults", ArgumentSemantic.Copy)]
		NSNumber MaximumNumberOfResults { get; set; }

		[NullAllowed, Export ("earliestBookingDateForResults", ArgumentSemantic.Copy)]
		NSDate EarliestBookingDateForResults { get; set; }

		[NullAllowed, Export ("latestBookingDateForResults", ArgumentSemantic.Copy)]
		NSDate LatestBookingDateForResults { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetAvailableRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetAvailableRestaurantReservationBookings:completion:")]
		void HandleAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent showBookingsIntent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetAvailableRestaurantReservationBookings:completion:")]
		void ConfirmAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent showBookingsIntent, Action<INGetAvailableRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetAvailableRestaurantReservationBookings:completion:")]
		void ResolveAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent showBookingsIntent, Action<INIntentResolutionResult<INRestaurant>> completion);

		[Export ("resolvePartySizeForGetAvailableRestaurantReservationBookings:completion:")]
		void ResolvePartySizeAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent showBookingsIntent, Action<INIntentResolutionResult<NSNumber>> completion);
		
		[Export ("resolvePreferredBookingDateForGetAvailableRestaurantReservationBookings:completion:")]
		void ResolvePreferredBookingDateAvailableRestaurantReservationBookings (INGetAvailableRestaurantReservationBookingsIntent showBookingsIntent, Action<INIntentResolutionResult<NSDate>> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[BaseType (typeof (INIntent))]
	interface INGetRestaurantGuestIntent {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetRestaurantGuestIntentHandling {

		[Abstract]
		[Export ("handleGetRestaurantGuest:completion:")]
		void HandleRestaurantGuest (INGetRestaurantGuestIntent guestIntent, Action<INGetRestaurantGuestIntentResponse> completion);

		[Export ("confirmGetRestaurantGuest:completion:")]
		void ConfirmRestaurantGuest (INGetRestaurantGuestIntent guestIntent, Action<INGetRestaurantGuestIntentResponse> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INGetRideStatusIntent {
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INGetRideStatusIntentResponseObserver {

		[Abstract]
		[Export ("getRideStatusResponseDidUpdate:")]
		void DidUpdateRideStatus (INGetRideStatusIntentResponse response);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Protocol]
	interface INGetUserCurrentRestaurantReservationBookingsIntentHandling {

		[Abstract]
		[Export ("handleGetUserCurrentRestaurantReservationBookings:completion:")]
		void HandleUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent showCurrentBookingsIntent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("confirmGetUserCurrentRestaurantReservationBookings:completion:")]
		void ConfirmUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent showCurrentBookingsIntent, Action<INGetUserCurrentRestaurantReservationBookingsIntentResponse> completion);

		[Export ("resolveRestaurantForGetUserCurrentRestaurantReservationBookings:completion:")]
		void ResolveUserCurrentRestaurantReservationBookings (INGetUserCurrentRestaurantReservationBookingsIntent showCurrentBookingsIntent, Action<INIntentResolutionResult<INRestaurant>> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
		[Export ("imageWithURL:")]
		INImage FromUrl (NSUrl url);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INIntentResolutionResult {

		[Static]
		[Export ("needsValue")]
		INIntentResolutionResult NeedsValue { get; }

		[Static]
		[Export ("notRequired")]
		INIntentResolutionResult NotRequired { get; }

		[Static]
		[Export ("unsupported")]
		INIntentResolutionResult Unsupported { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[BaseType (typeof (NSObject))]
	interface INIntentResponse : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("userActivity", ArgumentSemantic.Copy)]
		NSUserActivity UserActivity { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[NullAllowed, Export ("supportsApplePayForPayment", ArgumentSemantic.Copy)]
		NSNumber SupportsApplePayForPayment { get; set; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)] // <- FIXME: Verify if true with Introspection tests on macOS 10.12
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeOptionsResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeOptionsResolutionResult GetSuccess (INMessageAttributeOptions resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INMessageAttributeOptionsResolutionResult GetConfirmationRequired (INMessageAttributeOptions valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)] // <- FIXME: Verify if true with Introspection tests on macOS 10.12
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INMessageAttributeResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INMessageAttributeResolutionResult GetSuccess (INMessageAttribute resolvedValue);


		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INMessageAttributeResolutionResult GetConfirmationRequired (INMessageAttribute valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPaymentRecord : NSCopying, NSSecureCoding {

		[Export ("initWithPayee:payer:currencyAmount:paymentMethod:note:status:")]
		[DesignatedInitializer]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		// Inlined from INInteraction (INPerson) Category

		[NullAllowed, Export ("aliases", ArgumentSemantic.Copy)]
		INPersonHandle [] Aliases { get; }

		[Export ("suggestionType")]
		INPersonSuggestionType SuggestionType { get; }

		[Export ("initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:aliases:suggestionType:")]
		IntPtr Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string contactIdentifier, [NullAllowed] string customIdentifier, [NullAllowed] INPersonHandle [] aliases, INPersonSuggestionType suggestionType);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INPersonHandle : NSCopying, NSSecureCoding {

		[Export ("value")]
		string Value { get; }

		[Export ("type")]
		INPersonHandleType Type { get; }

		[Export ("initWithValue:type:")]
		[DesignatedInitializer]
		IntPtr Constructor (string value, INPersonHandleType type);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[Static]
		[Export ("needsMoreDetailsForPerson:")]
		INPersonResolutionResult GetNeedsMoreDetails (INPerson personToComplete);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INPhotoAttributeOptionsResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	[DisableDefaultCtor]
	interface INPhotoAttributeOptionsResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INPhotoAttributeOptionsResolutionResult GetSuccess (INPhotoAttributeOptions resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INPhotoAttributeOptionsResolutionResult GetConfirmationRequired (INPhotoAttributeOptions valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[Static]
		[Export ("needsMoreDetailsForPlacemark:")]
		INPlacemarkResolutionResult GetNeedsMoreDetails (CLPlacemark placemarkToComplete);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	interface INPreferences {

		[Static]
		[Export ("siriAuthorizationStatus")]
		INSiriAuthorizationStatus SiriAuthorizationStatus { get; }

		[Static]
		[Async]
		[Export ("requestSiriAuthorization:")]
		void RequestSiriAuthorization (Action<INSiriAuthorizationStatus> handler);

		[Static]
		[Export ("siriLanguageCode")]
		string SiriLanguageCode { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INRequestRideIntent {

		[Export ("initWithPickupLocation:dropOffLocation:rideOptionName:partySize:paymentMethodName:usesApplePayForPayment:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] CLPlacemark pickupLocation, [NullAllowed] CLPlacemark dropOffLocation, [NullAllowed] INSpeakableString rideOptionName, [NullAllowed] NSNumber partySize, [NullAllowed] INSpeakableString paymentMethodName, [NullAllowed] NSNumber usesApplePayForPayment);

		[NullAllowed, Export ("pickupLocation", ArgumentSemantic.Copy)]
		CLPlacemark PickupLocation { get; }

		[NullAllowed, Export ("dropOffLocation", ArgumentSemantic.Copy)]
		CLPlacemark DropOffLocation { get; }

		[NullAllowed, Export ("rideOptionName", ArgumentSemantic.Copy)]
		INSpeakableString RideOptionName { get; }

		[NullAllowed, Export ("partySize", ArgumentSemantic.Copy)]
		NSNumber PartySize { get; }

		[NullAllowed, Export ("paymentMethodName", ArgumentSemantic.Copy)]
		INSpeakableString PaymentMethodName { get; }

		[NullAllowed, Export ("usesApplePayForPayment", ArgumentSemantic.Copy)]
		NSNumber UsesApplePayForPayment { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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

		[Export ("resolvePaymentMethodNameForRequestRide:withCompletion:")]
		void ResolvePaymentMethodName (INRequestRideIntent intent, Action<INSpeakableStringResolutionResult> completion);

		[Export ("resolveUsesApplePayForPaymentForRequestRide:withCompletion:")]
		void ResolveUsesApplePayForPayment (INRequestRideIntent intent, Action<INBooleanResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface INRestaurantMarketingPreferences : NSSecureCoding, NSCopying {

		[Export ("initWithContactMethod:guestHasOptedInToMarketing:localizedDescriptionForMarketingEnabled:localizedDescriptionForMarketingNotEnabled:")]
		[DesignatedInitializer]
		IntPtr Constructor (INRestaurantMarketingPreferencesContactMethod contactMethod, bool guestHasOptedInToMarketing, string localizedDescriptionForMarketingEnabled, [NullAllowed] string localizedDescriptionForMarketingNotEnabled);

		[Export ("contactMethod")]
		INRestaurantMarketingPreferencesContactMethod ContactMethod { get; }

		[Export ("guestHasOptedInToMarketing")]
		bool GuestHasOptedInToMarketing { get; set; }

		[Export ("localizedDescriptionForMarketingEnabled")]
		string LocalizedDescriptionForMarketingEnabled { get; }

		[NullAllowed, Export ("localizedDescriptionForMarketingNotEnabled")]
		string LocalizedDescriptionForMarketingNotEnabled { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[BaseType (typeof (INIntent))]
	interface INResumeWorkoutIntent {

		[Export ("initWithWorkoutName:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] INSpeakableString workoutName);

		[NullAllowed, Export ("workoutName", ArgumentSemantic.Copy)]
		INSpeakableString WorkoutName { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INRideDriver bound
	[BaseType (typeof (INPerson))]
	interface INRideDriver : NSCopying, NSSecureCoding {

		[Export ("initWithPersonHandle:nameComponents:displayName:image:rating:phoneNumber:")]
		[DesignatedInitializer]
		IntPtr Constructor (INPersonHandle personHandle, [NullAllowed] NSPersonNameComponents nameComponents, [NullAllowed] string displayName, [NullAllowed] INImage image, [NullAllowed] string rating, [NullAllowed] string phoneNumber);

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
	[BaseType (typeof (INIntent))]
	interface INSaveProfileInCarIntent {

		[Export ("initWithProfileNumber:profileLabel:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel);

		[NullAllowed, Export ("profileNumber", ArgumentSemantic.Copy)]
		NSNumber ProfileNumber { get; }

		[NullAllowed, Export ("profileLabel")]
		string ProfileLabel { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INSaveProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSaveProfileInCar:completion:")]
		void HandleSaveProfileInCar (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("confirmSaveProfileInCar:completion:")]
		void ConfirmSaveProfileInCar (INSaveProfileInCarIntent intent, Action<INSaveProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSaveProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSaveProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolveProfileLabelForSaveProfileInCar:withCompletion:")]
		void ResolveProfileLabel (INSaveProfileInCarIntent intent, Action<INStringResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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

		[Export ("resolveCallCapabilitiesForSearchCallHistory:withCompletion:")]
		void ResolveCallCapabilities (INSearchCallHistoryIntent intent, Action<INCallCapabilityOptionsResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[Export ("resolveSearchTermsForSearchForMessages:withCompletion:")]
		void ResolveSearchTerms (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolveAttributesForSearchForMessages:withCompletion:")]
		void ResolveAttributes (INSearchForMessagesIntent intent, Action<INMessageAttributeOptionsResolutionResult> completion);

		[Export ("resolveDateTimeRangeForSearchForMessages:withCompletion:")]
		void ResolveDateTimeRange (INSearchForMessagesIntent intent, Action<INDateComponentsRangeResolutionResult> completion);

		[Export ("resolveIdentifiersForSearchForMessages:withCompletion:")]
		void ResolveIdentifiers (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolveNotificationIdentifiersForSearchForMessages:withCompletion:")]
		void ResolveNotificationIdentifiers (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolveGroupNamesForSearchForMessages:withCompletion:")]
		void ResolveGroupNames (INSearchForMessagesIntent intent, Action<INStringResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[Export ("resolveSearchTermsForSearchForPhotos:withCompletion:")]
		void ResolveSearchTerms (INSearchForPhotosIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolveIncludedAttributesForSearchForPhotos:withCompletion:")]
		void ResolveIncludedAttributes (INSearchForPhotosIntent intent, Action<INPhotoAttributeOptionsResolutionResult> completion);

		[Export ("resolveExcludedAttributesForSearchForPhotos:withCompletion:")]
		void ResolveExcludedAttributes (INSearchForPhotosIntent intent, Action<INPhotoAttributeOptionsResolutionResult> completion);

		[Export ("resolvePeopleInPhotoForSearchForPhotos:withCompletion:")]
		void ResolvePeopleInPhoto (INSearchForPhotosIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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

		[Export ("resolveServiceNameForSendMessage:withCompletion:")]
		void ResolveServiceName (INSendMessageIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolveSenderForSendMessage:withCompletion:")]
		void ResolveSender (INSendMessageIntent intent, Action<INPersonResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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
	[BaseType (typeof (INIntentResponse))]
	[DisableDefaultCtor]
	interface INSetAudioSourceInCarIntentResponse {

		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSetAudioSourceInCarIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		[Export ("code")]
		INSetAudioSourceInCarIntentResponseCode Code { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INSetClimateSettingsInCarIntent {

		[Export ("initWithEnableFan:enableAirConditioner:enableClimateControl:enableAutoMode:airCirculationMode:fanSpeedIndex:fanSpeedPercentage:relativeFanSpeedSetting:temperature:relativeTemperatureSetting:climateZone:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber enableFan, [NullAllowed] NSNumber enableAirConditioner, [NullAllowed] NSNumber enableClimateControl, [NullAllowed] NSNumber enableAutoMode, INCarAirCirculationMode airCirculationMode, [NullAllowed] NSNumber fanSpeedIndex, [NullAllowed] NSNumber fanSpeedPercentage, INRelativeSetting relativeFanSpeedSetting, [NullAllowed] INTemperature temperature, INRelativeSetting relativeTemperatureSetting, INCarSeat climateZone);

		[NullAllowed, Export ("enableFan", ArgumentSemantic.Copy)]
		NSNumber EnableFan { get; }

		[NullAllowed, Export ("enableAirConditioner", ArgumentSemantic.Copy)]
		NSNumber EnableAirConditioner { get; }

		[NullAllowed, Export ("enableClimateControl", ArgumentSemantic.Copy)]
		NSNumber EnableClimateControl { get; }

		[NullAllowed, Export ("enableAutoMode", ArgumentSemantic.Copy)]
		NSNumber EnableAutoMode { get; }

		[Export ("airCirculationMode", ArgumentSemantic.Assign)]
		INCarAirCirculationMode AirCirculationMode { get; }

		[NullAllowed, Export ("fanSpeedIndex", ArgumentSemantic.Copy)]
		NSNumber FanSpeedIndex { get; }

		[NullAllowed, Export ("fanSpeedPercentage", ArgumentSemantic.Copy)]
		NSNumber FanSpeedPercentage { get; }

		[Export ("relativeFanSpeedSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeFanSpeedSetting { get; }

		[NullAllowed, Export ("temperature", ArgumentSemantic.Copy)]
		INTemperature Temperature { get; }

		[Export ("relativeTemperatureSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeTemperatureSetting { get; }

		[Export ("climateZone", ArgumentSemantic.Assign)]
		INCarSeat ClimateZone { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[BaseType (typeof (INIntent))]
	interface INSetDefrosterSettingsInCarIntent {

		[Export ("initWithEnable:defroster:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber enable, INCarDefroster defroster);

		[NullAllowed, Export ("enable", ArgumentSemantic.Copy)]
		NSNumber Enable { get; }

		[Export ("defroster", ArgumentSemantic.Assign)]
		INCarDefroster Defroster { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[Protocol]
	interface INSetMessageAttributeIntentHandling {

		[Abstract]
		[Export ("handleSetMessageAttribute:completion:")]
		void HandleSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("confirmSetMessageAttribute:completion:")]
		void ConfirmSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion);

		[Export ("resolveIdentifiersForSetMessageAttribute:withCompletion:")]
		void ResolveIdentifiers (INSetMessageAttributeIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolveAttributeForSetMessageAttribute:withCompletion:")]
		void ResolveAttribute (INSetMessageAttributeIntent intent, Action<INMessageAttributeResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[BaseType (typeof (INIntent))]
	interface INSetProfileInCarIntent {

		[Export ("initWithProfileNumber:profileLabel:defaultProfile:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber profileNumber, [NullAllowed] string profileLabel, [NullAllowed] NSNumber defaultProfile);

		[NullAllowed, Export ("profileNumber", ArgumentSemantic.Copy)]
		NSNumber ProfileNumber { get; }

		[NullAllowed, Export ("profileLabel")]
		string ProfileLabel { get; }

		[NullAllowed, Export ("defaultProfile", ArgumentSemantic.Copy)]
		NSNumber DefaultProfile { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Protocol]
	interface INSetProfileInCarIntentHandling {

		[Abstract]
		[Export ("handleSetProfileInCar:completion:")]
		void HandleSetProfileInCar (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("confirmSetProfileInCar:completion:")]
		void ConfirmSetProfileInCar (INSetProfileInCarIntent intent, Action<INSetProfileInCarIntentResponse> completion);

		[Export ("resolveProfileNumberForSetProfileInCar:withCompletion:")]
		void ResolveProfileNumber (INSetProfileInCarIntent intent, Action<INIntegerResolutionResult> completion);

		[Export ("resolveProfileLabelForSetProfileInCar:withCompletion:")]
		void ResolveProfileLabel (INSetProfileInCarIntent intent, Action<INStringResolutionResult> completion);

		[Export ("resolveDefaultProfileForSetProfileInCar:withCompletion:")]
		void ResolveDefaultProfile (INSetProfileInCarIntent intent, Action<INBooleanResolutionResult> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[BaseType (typeof (INIntent))]
	interface INSetSeatSettingsInCarIntent {

		[Export ("initWithEnableHeating:enableCooling:enableMassage:seat:level:relativeLevelSetting:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSNumber enableHeating, [NullAllowed] NSNumber enableCooling, [NullAllowed] NSNumber enableMassage, INCarSeat seat, [NullAllowed] NSNumber level, INRelativeSetting relativeLevelSetting);

		[NullAllowed, Export ("enableHeating", ArgumentSemantic.Copy)]
		NSNumber EnableHeating { get; }

		[NullAllowed, Export ("enableCooling", ArgumentSemantic.Copy)]
		NSNumber EnableCooling { get; }

		[NullAllowed, Export ("enableMassage", ArgumentSemantic.Copy)]
		NSNumber EnableMassage { get; }

		[Export ("seat", ArgumentSemantic.Assign)]
		INCarSeat Seat { get; }

		[NullAllowed, Export ("level", ArgumentSemantic.Copy)]
		NSNumber Level { get; }

		[Export ("relativeLevelSetting", ArgumentSemantic.Assign)]
		INRelativeSetting RelativeLevelSetting { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INSpeakableString : INSpeakable {

		[Export ("initWithIdentifier:spokenPhrase:pronunciationHint:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, string spokenPhrase, [NullAllowed] string pronunciationHint);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
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

		[Export ("resolveSearchTermsForStartPhotoPlayback:withCompletion:")]
		void ResolveSearchTerms (INStartPhotoPlaybackIntent intent, Action<INStringResolutionResult []> completion);

		[Export ("resolveIncludedAttributesForStartPhotoPlayback:withCompletion:")]
		void ResolveIncludedAttributes (INStartPhotoPlaybackIntent intent, Action<INPhotoAttributeOptionsResolutionResult> completion);

		[Export ("resolveExcludedAttributesForStartPhotoPlayback:withCompletion:")]
		void ResolveExcludedAttributes (INStartPhotoPlaybackIntent intent, Action<INPhotoAttributeOptionsResolutionResult> completion);

		[Export ("resolvePeopleInPhotoForStartPhotoPlayback:withCompletion:")]
		void ResolvePeopleInPhoto (INStartPhotoPlaybackIntent intent, Action<INPersonResolutionResult []> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntent))]
	interface INStartWorkoutIntent {

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

		[NullAllowed, Export ("isOpenEnded", ArgumentSemantic.Copy)]
		NSNumber IsOpenEnded { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
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
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface INTemperature : NSCopying, NSSecureCoding {

		[Export ("initWithUnit:value:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUnitTemperature unit, NSNumber value);

		[NullAllowed, Export ("unit", ArgumentSemantic.Copy)]
		NSUnitTemperature Unit { get; }

		[NullAllowed, Export ("value", ArgumentSemantic.Copy)]
		NSNumber Value { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[BaseType (typeof (INIntentResolutionResult))]
	interface INTemperatureResolutionResult {

		[Static]
		[Export ("successWithResolvedTemperature:")]
		INTemperatureResolutionResult GetSuccess (INTemperature resolvedTemperature);

		[Static]
		[Export ("disambiguationWithTemperaturesToDisambiguate:")]
		INTemperatureResolutionResult GetDisambiguation (INTemperature [] temperaturesToDisambiguate);

		[Static]
		[Export ("confirmationRequiredWithTemperatureToConfirm:")]
		INTemperatureResolutionResult GetConfirmationRequired ([NullAllowed] INTemperature temperatureToConfirm);

		[Static]
		[Export ("needsMoreDetailsForTemperature:")]
		INTemperatureResolutionResult GetNeedsMoreDetails (INTemperature temperatureToComplete);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
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
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutGoalUnitTypeResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutGoalUnitTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutGoalUnitTypeResolutionResult GetSuccess (INWorkoutGoalUnitType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INWorkoutGoalUnitTypeResolutionResult GetConfirmationRequired (INWorkoutGoalUnitType valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)] // xtro mac !unknown-type! INWorkoutLocationTypeResolutionResult bound
	[BaseType (typeof (INIntentResolutionResult))]
	interface INWorkoutLocationTypeResolutionResult {

		[Static]
		[Export ("successWithResolvedValue:")]
		INWorkoutLocationTypeResolutionResult GetSuccess (INWorkoutLocationType resolvedValue);

		[Static]
		[Export ("confirmationRequiredWithValueToConfirm:")]
		INWorkoutLocationTypeResolutionResult GetConfirmationRequired (INWorkoutLocationType valueToConfirm);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Category]
	[BaseType (typeof (NSUserActivity))]
	interface NSUserActivity_IntentsAdditions {

		[NullAllowed, Export ("interaction")]
		INInteraction GetInteraction ();
	}
}
#endif // XAMCORE_2_0
