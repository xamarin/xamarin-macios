using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMError : long {
		UnexpectedError                         = -1,
		AlreadyExists                           = 1,
		NotFound                                = 2,
		InvalidParameter                        = 3,
		AccessoryNotReachable                   = 4,
		ReadOnlyCharacteristic                  = 5,
		WriteOnlyCharacteristic                 = 6,
		NotificationNotSupported                = 7,
		OperationTimedOut                       = 8,
		AccessoryPoweredOff                     = 9,
		AccessDenied                            = 10,
		ObjectAssociatedToAnotherHome           = 11,
		ObjectNotAssociatedToAnyHome            = 12,
		ObjectAlreadyAssociatedToHome           = 13,
		AccessoryIsBusy                         = 14,
		OperationInProgress                     = 15,
		AccessoryOutOfResources                 = 16,
		InsufficientPrivileges                  = 17,
		AccessoryPairingFailed                  = 18,
		InvalidDataFormatSpecified              = 19,
		NilParameter                            = 20,
		UnconfiguredParameter                   = 21,
		InvalidClass                            = 22,
		OperationCancelled                      = 23,
		RoomForHomeCannotBeInZone               = 24,
		NoActionsInActionSet                    = 25,
		NoRegisteredActionSets                  = 26,
		MissingParameter                        = 27,
		FireDateInPast                          = 28,
		RoomForHomeCannotBeUpdated              = 29,
		ActionInAnotherActionSet                = 30,
		ObjectWithSimilarNameExistsInHome       = 31,
		HomeWithSimilarNameExists               = 32,
		RenameWithSimilarName                   = 33,
		CannotRemoveNonBridgeAccessory          = 34,
		NameContainsProhibitedCharacters        = 35,
		NameDoesNotStartWithValidCharacters     = 36,
		UserIDNotEmailAddress                   = 37,
		UserDeclinedAddingUser                  = 38,
		UserDeclinedRemovingUser                = 39,
		UserDeclinedInvite                      = 40,
		UserManagementFailed                    = 41,
		RecurrenceTooSmall                      = 42,
		InvalidValueType                        = 43,
		ValueLowerThanMinimum                   = 44,
		ValueHigherThanMaximum                  = 45,
		StringLongerThanMaximum                 = 46,
		HomeAccessNotAuthorized                 = 47,
		OperationNotSupported                   = 48,
		MaximumObjectLimitReached               = 49,
		AccessorySentInvalidResponse            = 50,
		StringShorterThanMinimum                = 51,
		GenericError                            = 52,
		SecurityFailure                         = 53,
		CommunicationFailure                    = 54,
		MessageAuthenticationFailed             = 55,
		InvalidMessageSize                      = 56,
		AccessoryDiscoveryFailed                = 57,
		ClientRequestError                      = 58,
		AccessoryResponseError                  = 59,
		NameDoesNotEndWithValidCharacters       = 60,
		AccessoryIsBlocked                      = 61,
		InvalidAssociatedServiceType            = 62,
		ActionSetExecutionFailed                = 63,
		ActionSetExecutionPartialSuccess        = 64,
		ActionSetExecutionInProgress            = 65,
		AccessoryOutOfCompliance                = 66,
		DataResetFailure                        = 67,
		NotificationAlreadyEnabled              = 68,
		RecurrenceMustBeOnSpecifiedBoundaries   = 69,
		DateMustBeOnSpecifiedBoundaries         = 70,
		CannotActivateTriggerTooFarInFuture     = 71,
		RecurrenceTooLarge                      = 72,
		ReadWritePartialSuccess                 = 73,
		ReadWriteFailure                        = 74,
		NotSignedIntoiCloud                     = 75,
		KeychainSyncNotEnabled                  = 76,
		CloudDataSyncInProgress                 = 77,
		NetworkUnavailable                      = 78,
		AddAccessoryFailed                      = 79,
		MissingEntitlement                      = 80,
		CannotUnblockNonBridgeAccessory			= 81,
		DeviceLocked							= 82,
		CannotRemoveBuiltinActionSet			= 83,
		LocationForHomeDisabled					= 84,
		NotAuthorizedForLocationServices		= 85,
		// iOS 9.3
		ReferToUserManual						= 86,
		// iOS 10.0
		InvalidOrMissingAuthorizationData       = 87,
		BridgedAccessoryNotReachable            = 88,
		NotAuthorizedForMicrophoneAccess        = 89,
		// iOS 10.2
		IncompatibleNetwork                     = 90,
		// iOS 11
		NoHomeHub = 91,
		IncompatibleHomeHub = 92, // HMErrorCodeNoCompatibleHomeHub introduced and deprecated on iOS 11. HMErrorCodeIncompatibleHomeHub = HMErrorCodeNoCompatibleHomeHub.
		IncompatibleAccessory = 93,
		ObjectWithSimilarNameExists = 95,
		OwnershipFailure = 96,
		MaximumAccessoriesOfTypeInHome = 97,
		WiFiCredentialGenerationFailed = 98,
		// iOS 14
		EnterpriseNetworkNotSupported = 99,
		TimedOutWaitingForAccessory = 100,
		AccessoryCommunicationFailure = 101,
		FailedToJoinNetwork = 102,
		// iOS 15
		AccessoryIsSuspended = 103,
	}

	
	// conveniance enum (ObjC uses NSString)
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	public enum HMCharacteristicType {
		None,

		[Field ("HMCharacteristicTypePowerState")]
		PowerState,

		[Field ("HMCharacteristicTypeHue")]
		Hue,

		[Field ("HMCharacteristicTypeSaturation")]
		Saturation,

		[Field ("HMCharacteristicTypeBrightness")]
		Brightness,

		[Field ("HMCharacteristicTypeTemperatureUnits")]
		TemperatureUnits,

		[Field ("HMCharacteristicTypeCurrentTemperature")]
		CurrentTemperature,

		[Field ("HMCharacteristicTypeTargetTemperature")]
		TargetTemperature,

		[Field ("HMCharacteristicTypeCurrentHeatingCooling")]
		CurrentHeatingCooling,

		[Field ("HMCharacteristicTypeTargetHeatingCooling")]
		TargetHeatingCooling,

		[Field ("HMCharacteristicTypeCoolingThreshold")]
		CoolingThreshold,

		[Field ("HMCharacteristicTypeHeatingThreshold")]
		HeatingThreshold,

#if !XAMCORE_4_0
		[Obsolete ("This value does not exist anymore and will always return null.")]
		HeatingCoolingStatus,
#endif

		[Field ("HMCharacteristicTypeCurrentRelativeHumidity")]
		CurrentRelativeHumidity,

		[Field ("HMCharacteristicTypeTargetRelativeHumidity")]
		TargetRelativeHumidity,

		[Field ("HMCharacteristicTypeCurrentDoorState")]
		CurrentDoorState,

		[Field ("HMCharacteristicTypeTargetDoorState")]
		TargetDoorState,

		[Field ("HMCharacteristicTypeObstructionDetected")]
		ObstructionDetected,

		[Field ("HMCharacteristicTypeName")]
		Name,

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if TVOS
		[Obsolete ("Starting with tvos11.0 use 'HMAccessory.Manufacturer' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'HMAccessory.Manufacturer' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
#endif
		[Field ("HMCharacteristicTypeManufacturer")]
		Manufacturer,

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if TVOS
		[Obsolete ("Starting with tvos11.0 use 'HMAccessory.Model' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'HMAccessory.Model' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.Model' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'HMAccessory.Model' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.Model' instead.")]
#endif
		[Field ("HMCharacteristicTypeModel")]
		Model,

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if TVOS
		[Obsolete ("Starting with tvos11.0 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.TvOS, 11, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "No longer supported.")]
#endif
		[Field ("HMCharacteristicTypeSerialNumber")]
		SerialNumber,

		[Field ("HMCharacteristicTypeIdentify")]
		Identify,

		[Field ("HMCharacteristicTypeRotationDirection")]
		RotationDirection,

		[Field ("HMCharacteristicTypeRotationSpeed")]
		RotationSpeed,

		[Field ("HMCharacteristicTypeOutletInUse")]
		OutletInUse,

		[Field ("HMCharacteristicTypeVersion")]
		Version,

		[Field ("HMCharacteristicTypeLogs")]
		Logs,

		[Field ("HMCharacteristicTypeAudioFeedback")]
		AudioFeedback,

		[Field ("HMCharacteristicTypeAdminOnlyAccess")]
		AdminOnlyAccess,

		[Field ("HMCharacteristicTypeMotionDetected")]
		MotionDetected,

		[Field ("HMCharacteristicTypeCurrentLockMechanismState")]
		CurrentLockMechanismState,

		[Field ("HMCharacteristicTypeTargetLockMechanismState")]
		TargetLockMechanismState,

		[Field ("HMCharacteristicTypeLockMechanismLastKnownAction")]
		LockMechanismLastKnownAction,

		[Field ("HMCharacteristicTypeLockManagementControlPoint")]
		LockManagementControlPoint,

		[Field ("HMCharacteristicTypeLockManagementAutoSecureTimeout")]
		LockManagementAutoSecureTimeout,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeAirParticulateDensity")]
		AirParticulateDensity,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeAirParticulateSize")]
		AirParticulateSize,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeAirQuality")]
		AirQuality,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeBatteryLevel")]
		BatteryLevel,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCarbonDioxideDetected")]
		CarbonDioxideDetected,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCarbonDioxideLevel")]
		CarbonDioxideLevel,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCarbonDioxidePeakLevel")]
		CarbonDioxidePeakLevel,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCarbonMonoxideDetected")]
		CarbonMonoxideDetected,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCarbonMonoxideLevel")]
		CarbonMonoxideLevel,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCarbonMonoxidePeakLevel")]
		CarbonMonoxidePeakLevel,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeChargingState")]
		ChargingState,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeContactState")]
		ContactState,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCurrentSecuritySystemState")]
		CurrentSecuritySystemState,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCurrentHorizontalTilt")]
		CurrentHorizontalTilt,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCurrentLightLevel")]
		CurrentLightLevel,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCurrentPosition")]
		CurrentPosition,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeCurrentVerticalTilt")]
		CurrentVerticalTilt,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if TVOS
		[Obsolete ("Starting with tvos11.0 use 'HMAccessory.FirmwareVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'HMAccessory.FirmwareVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (9,0)]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
#endif
		[Field ("HMCharacteristicTypeFirmwareVersion")]
		FirmwareVersion,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeHardwareVersion")]
		HardwareVersion,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeHoldPosition")]
		HoldPosition,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeInputEvent")]
		InputEvent,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeLeakDetected")]
		LeakDetected,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeOccupancyDetected")]
		OccupancyDetected,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeOutputState")]
		OutputState,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypePositionState")]
		PositionState,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeSmokeDetected")]
		SmokeDetected,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeSoftwareVersion")]
		SoftwareVersion,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeStatusActive")]
		StatusActive,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeStatusFault")]
		StatusFault,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeStatusJammed")]
		StatusJammed,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeStatusLowBattery")]
		StatusLowBattery,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeStatusTampered")]
		StatusTampered,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeTargetSecuritySystemState")]
		TargetSecuritySystemState,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeTargetHorizontalTilt")]
		TargetHorizontalTilt,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeTargetPosition")]
		TargetPosition,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeTargetVerticalTilt")]
		TargetVerticalTilt,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeStreamingStatus")]
		StreamingStatus,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeSetupStreamEndpoint")]
		SetupStreamEndpoint,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeSupportedVideoStreamConfiguration")]
		SupportedVideoStreamConfiguration,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeSupportedAudioStreamConfiguration")]
		SupportedAudioStreamConfiguration,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeSupportedRTPConfiguration")]
		SupportedRtpConfiguration,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeSelectedStreamConfiguration")]
		SelectedStreamConfiguration,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeVolume")]
		Volume,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeMute")]
		Mute,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeNightVision")]
		NightVision,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeOpticalZoom")]
		OpticalZoom,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeDigitalZoom")]
		DigitalZoom,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeImageRotation")]
		ImageRotation,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMCharacteristicTypeImageMirroring")]
		ImageMirroring,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeActive")]
		Active,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeCurrentAirPurifierState")]
		CurrentAirPurifierState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeTargetAirPurifierState")]
		TargetAirPurifierState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeCurrentFanState")]
		CurrentFanState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeCurrentHeaterCoolerState")]
		CurrentHeaterCoolerState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeCurrentHumidifierDehumidifierState")]
		CurrentHumidifierDehumidifierState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeCurrentSlatState")]
		CurrentSlatState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeWaterLevel")]
		WaterLevel,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeFilterChangeIndication")]
		FilterChangeIndication,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeFilterLifeLevel")]
		FilterLifeLevel,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeFilterResetChangeIndication")]
		FilterResetChangeIndication,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeLockPhysicalControls")]
		LockPhysicalControls,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeSwingMode")]
		SwingMode,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeTargetHeaterCoolerState")]
		TargetHeaterCoolerState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeTargetHumidifierDehumidifierState")]
		TargetHumidifierDehumidifierState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeTargetFanState")]
		TargetFanState,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeSlatType")]
		SlatType,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeCurrentTilt")]
		CurrentTilt,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeTargetTilt")]
		TargetTilt,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeOzoneDensity")]
		OzoneDensity,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeNitrogenDioxideDensity")]
		NitrogenDioxideDensity,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeSulphurDioxideDensity")]
		SulphurDioxideDensity,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypePM2_5Density")]
		PM2_5Density,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypePM10Density")]
		PM10Density,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeVolatileOrganicCompoundDensity")]
		VolatileOrganicCompoundDensity,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeDehumidifierThreshold")]
		DehumidifierThreshold,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMCharacteristicTypeHumidifierThreshold")]
		HumidifierThreshold,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMCharacteristicTypeSecuritySystemAlarmType")]
		SecuritySystemAlarmType,

#if NET
		[SupportedOSPlatform ("ios10.3")]
		[SupportedOSPlatform ("tvos10.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,3)]
		[Watch (3,2)]
		[TV (10,2)]
#endif
		[Field ("HMCharacteristicTypeLabelNamespace")]
		LabelNamespace,

#if NET
		[SupportedOSPlatform ("ios10.3")]
		[SupportedOSPlatform ("tvos10.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,3)]
		[Watch (3,2)]
		[TV (10,2)]
#endif
		[Field ("HMCharacteristicTypeLabelIndex")]
		LabelIndex,

#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (11,0)]
		[Watch (4,0)]
		[TV (11,0)]
#endif
		[Field ("HMCharacteristicTypeColorTemperature")]
		ColorTemperature,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMCharacteristicTypeProgramMode")]
		ProgramMode,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMCharacteristicTypeInUse")]
		InUse,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMCharacteristicTypeSetDuration")]
		SetDuration,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMCharacteristicTypeRemainingDuration")]
		RemainingDuration,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMCharacteristicTypeValveType")]
		ValveType,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMCharacteristicTypeIsConfigured")]
		IsConfigured,
	}

	// conveniance enum (ObjC uses NSString)
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
#endif
	public enum HMCharacteristicMetadataUnits {
		None,
		Celsius,
		Fahrenheit,
		Percentage,
		ArcDegree,
#if NET
		[SupportedOSPlatform ("ios8.3")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[iOS (8,3)]
#endif
		Seconds,
#if NET
		[SupportedOSPlatform ("ios9.3")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[iOS (9,3)]
		[Watch (2,2)]
#endif
		Lux,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		PartsPerMillion,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		MicrogramsPerCubicMeter,
	}

	// conveniance enum (ObjC uses NSString)
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Flags]
	public enum HMServiceType {
		None,

		[Field ("HMServiceTypeLightbulb")]
		LightBulb,

		[Field ("HMServiceTypeSwitch")]
		Switch,

		[Field ("HMServiceTypeThermostat")]
		Thermostat,

		[Field ("HMServiceTypeGarageDoorOpener")]
		GarageDoorOpener,

		[Field ("HMServiceTypeAccessoryInformation")]
		AccessoryInformation,

		[Field ("HMServiceTypeFan")]
		Fan,

		[Field ("HMServiceTypeOutlet")]
		Outlet,

		[Field ("HMServiceTypeLockMechanism")]
		LockMechanism,

		[Field ("HMServiceTypeLockManagement")]
		LockManagement,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeAirQualitySensor")]
		AirQualitySensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeBattery")]
		Battery,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeCarbonDioxideSensor")]
		CarbonDioxideSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeCarbonMonoxideSensor")]
		CarbonMonoxideSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeContactSensor")]
		ContactSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeDoor")]
		Door,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeHumiditySensor")]
		HumiditySensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeLeakSensor")]
		LeakSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeLightSensor")]
		LightSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeMotionSensor")]
		MotionSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeOccupancySensor")]
		OccupancySensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeSecuritySystem")]
		SecuritySystem,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeStatefulProgrammableSwitch")]
		StatefulProgrammableSwitch,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeStatelessProgrammableSwitch")]
		StatelessProgrammableSwitch,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeSmokeSensor")]
		SmokeSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeTemperatureSensor")]
		TemperatureSensor,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeWindow")]
		Window,

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		[Field ("HMServiceTypeWindowCovering")]
		WindowCovering,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMServiceTypeCameraRTPStreamManagement")]
		CameraRtpStreamManagement,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMServiceTypeCameraControl")]
		CameraControl,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMServiceTypeMicrophone")]
		Microphone,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMServiceTypeSpeaker")]
		Speaker,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMServiceTypeDoorbell")]
		Doorbell,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMServiceTypeAirPurifier")]
		AirPurifier,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMServiceTypeVentilationFan")]
		VentilationFan,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMServiceTypeFilterMaintenance")]
		FilterMaintenance,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMServiceTypeHeaterCooler")]
		HeaterCooler,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMServiceTypeHumidifierDehumidifier")]
		HumidifierDehumidifier,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMServiceTypeSlats")]
		Slats,

#if NET
		[SupportedOSPlatform ("ios10.3")]
		[SupportedOSPlatform ("tvos10.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,3)]
		[Watch (3,2)]
		[TV (10,2)]
#endif
		[Field ("HMServiceTypeLabel")]
		Label,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMServiceTypeIrrigationSystem")]
		IrrigationSystem,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMServiceTypeValve")]
		Valve,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMServiceTypeFaucet")]
		Faucet,
	}

	// conveniance enum (ObjC uses NSString)
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
#endif
	public enum HMCharacteristicMetadataFormat {
		None,
		Bool,
		Int,
		Float,
		String,
		Array,
		Dictionary,
		UInt8,
		UInt16,
		UInt32,
		UInt64,
		Data,
		Tlv8
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueDoorState : long {
		Open = 0,
		Closed,
		Opening,
		Closing,
		Stopped
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueHeatingCooling : long {
		Off = 0,
		Heat,
		Cool,
		Auto
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueRotationDirection : long {
		Clockwise = 0,
		CounterClockwise
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTemperatureUnit : long {
		Celsius = 0,
		Fahrenheit
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueLockMechanismState : long {
		Unsecured = 0,
		Secured,
		Jammed,
		Unknown
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	// in iOS 8.3 this was renamed HMCharacteristicValueLockMechanismLastKnownAction but that would be a breaking change for us
	public enum HMCharacteristicValueLockMechanism : long {
		LastKnownActionSecuredUsingPhysicalMovementInterior = 0,
		LastKnownActionUnsecuredUsingPhysicalMovementInterior,
		LastKnownActionSecuredUsingPhysicalMovementExterior,
		LastKnownActionUnsecuredUsingPhysicalMovementExterior,
		LastKnownActionSecuredWithKeypad,
		LastKnownActionUnsecuredWithKeypad,
		LastKnownActionSecuredRemotely,
		LastKnownActionUnsecuredRemotely,
		LastKnownActionSecuredWithAutomaticSecureTimeout,
		LastKnownActionSecuredUsingPhysicalMovement,
		LastKnownActionUnsecuredUsingPhysicalMovement,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueAirParticulate : long {
		Size2_5 = 0,
		Size10
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentSecuritySystemState : long {
		StayArm = 0,
		AwayArm,
		NightArm,
		Disarmed,
		Triggered
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValuePositionState : long {
		Closing = 0,
		Opening,
		Stopped
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetSecuritySystemState : long {
		StayArm = 0,
		AwayArm,
		NightArm,
		Disarm
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueBatteryStatus : long {
		Normal = 0,
		Low
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueJammedStatus : long {
		None = 0,
		Jammed
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTamperedStatus : long {
		None = 0,
		Tampered
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueLeakStatus : long {
		None = 0,
		Detected
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueSmokeDetectionStatus : long {
		None = 0,
		Detected
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueChargingState : long {
		None = 0,
		InProgress,
#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,2)]
		[Watch (3,1,1)]
		[TV (10,1)]
#endif
		NotChargeable,
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueContactState : long {
		Detected = 0,
		None,
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueStatusFault : long {
		NoFault = 0,
		GeneralFault
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCarbonMonoxideDetectionStatus : long {
		NotDetected = 0,
		Detected
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCarbonDioxideDetectionStatus : long {
		NotDetected = 0,
		Detected
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueOccupancyStatus : long {
		NotOccupied = 0,
		Occupied
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,0)]
	[TV (10,0)]
	[iOS (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueSecuritySystemAlarmType : long {
		NoAlarm = 0,
		Unknown
	}

	// conveniance enum (ObjC uses NSString)
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
#endif
	public enum HMActionSetType {
		Unknown = -1,
		WakeUp,
		Sleep,
		HomeDeparture,
		HomeArrival,
		UserDefined,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		TriggerOwned,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	// conveniance enum (ObjC uses NSString)
	public enum HMAccessoryCategoryType {
		[Field ("HMAccessoryCategoryTypeOther")]
		Other = 0,

		[Field ("HMAccessoryCategoryTypeSecuritySystem")]
		SecuritySystem,

		[Field ("HMAccessoryCategoryTypeBridge")]
		Bridge,

		[Field ("HMAccessoryCategoryTypeDoor")]
		Door,

		[Field ("HMAccessoryCategoryTypeDoorLock")]
		DoorLock,

		[Field ("HMAccessoryCategoryTypeFan")]
		Fan,

		[Field ("HMAccessoryCategoryTypeGarageDoorOpener")]
		GarageDoorOpener,

#if !WATCH && !TVOS
		[Obsolete ("Use 'GarageDoorOpener' instead.")]
		DoorOpener = GarageDoorOpener,
#endif

		[Field ("HMAccessoryCategoryTypeLightbulb")]
		Lightbulb,

		[Field ("HMAccessoryCategoryTypeOutlet")]
		Outlet,

		[Field ("HMAccessoryCategoryTypeProgrammableSwitch")]
		ProgrammableSwitch,

		[Field ("HMAccessoryCategoryTypeSensor")]
		Sensor,

		[Field ("HMAccessoryCategoryTypeSwitch")]
		Switch,

		[Field ("HMAccessoryCategoryTypeThermostat")]
		Thermostat,

		[Field ("HMAccessoryCategoryTypeWindow")]
		Window,

		[Field ("HMAccessoryCategoryTypeWindowCovering")]
		WindowCovering,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMAccessoryCategoryTypeRangeExtender")]
		RangeExtender,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMAccessoryCategoryTypeIPCamera")]
		IPCamera,

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		[Field ("HMAccessoryCategoryTypeVideoDoorbell")]
		VideoDoorbell,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMAccessoryCategoryTypeAirPurifier")]
		AirPurifier,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMAccessoryCategoryTypeAirHeater")]
		AirHeater,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMAccessoryCategoryTypeAirConditioner")]
		AirConditioner,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMAccessoryCategoryTypeAirHumidifier")]
		AirHumidifier,

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (3,1,1)]
		[iOS (10,2)]
		[TV (10,1)]
#endif
		[Field ("HMAccessoryCategoryTypeAirDehumidifier")]
		AirDehumidifier,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMAccessoryCategoryTypeSprinkler")]
		Sprinkler,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMAccessoryCategoryTypeFaucet")]
		Faucet,

#if NET
		[SupportedOSPlatform ("tvos11.2")]
		[SupportedOSPlatform ("ios11.2")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[Watch (4,2)]
		[TV (11,2)]
		[iOS (11,2)]
#endif
		[Field ("HMAccessoryCategoryTypeShowerHead")]
		ShowerHead,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	public enum HMSignificantEvent {

		[Field ("HMSignificantEventSunrise")]
		Sunrise,

		[Field ("HMSignificantEventSunset")]
		Sunset,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Watch (6,0)]
	[NoMac]
	[MacCatalyst (14,0)]
#endif
	[Flags]
	[Native]
	public enum HMHomeManagerAuthorizationStatus : ulong {
		Determined = 1 << 0,
		Restricted = 1 << 1,
		Authorized = 1 << 2,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (9,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueAirQuality : long {
		Unknown = 0,
		Excellent,
		Good,
		Fair,
		Inferior,
		Poor
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCameraStreamState : ulong
	{
		Starting = 1,
		Streaming = 2,
		Stopping = 3,
		NotStreaming = 4
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCameraAudioStreamSetting : ulong
	{
		Muted = 1,
		IncomingAudioAllowed = 2,
		BidirectionalAudioAllowed = 3
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueLockPhysicalControlsState : long {
		NotLocked = 0,
		Locked,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentAirPurifierState : long {
		Inactive = 0,
		Idle,
		Active,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetAirPurifierState : long {
		Manual = 0,
		Automatic,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentSlatState : long {
		Stationary = 0,
		Jammed,
		Oscillating,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueSlatType : long {
		Horizontal = 0,
		Vertical,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueFilterChange : long {
		NotNeeded = 0,
		Needed,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentFanState : long {
		Inactive = 0,
		Idle,
		Active,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetFanState : long {
		Manual = 0,
		Automatic,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentHeaterCoolerState : long {
		Inactive = 0,
		Idle,
		Heating,
		Cooling,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetHeaterCoolerState : long {
		Automatic = 0,
		Heat,
		Cool,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentHumidifierDehumidifierState : long {
		Inactive = 0,
		Idle,
		Humidifying,
		Dehumidifying,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetHumidifierDehumidifierState : long {
		Automatic = 0,
		Humidify,
		Dehumidify,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueSwingMode : long {
		Disabled = 0,
		Enabled,
	}

#if NET
	[SupportedOSPlatform ("tvos10.1")]
	[SupportedOSPlatform ("ios10.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,1,1)]
	[TV (10,1)]
	[iOS (10,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueActivationState : long {
		Inactive = 0,
		Active,
	}

#if NET
	[SupportedOSPlatform ("tvos10.2")]
	[SupportedOSPlatform ("ios10.3")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,2)]
	[TV (10,2)]
	[iOS (10,3)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueInputEvent : long {
		SinglePress = 0,
		DoublePress,
		LongPress,
	}

#if NET
	[SupportedOSPlatform ("tvos10.2")]
	[SupportedOSPlatform ("ios10.3")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (3,2)]
	[TV (10,2)]
	[iOS (10,3)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueLabelNamespace : long {
		Dot = 0,
		Numeral,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[iOS (11,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMEventTriggerActivationState : ulong {
		Disabled = 0,
		DisabledNoHomeHub = 1,
		DisabledNoCompatibleHomeHub = 2,
		DisabledNoLocationServicesAuthorization = 3,
		Enabled = 4,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[iOS (11,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMHomeHubState : ulong {
		NotAvailable = 0,
		Connected,
		Disconnected,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[iOS (11,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMPresenceEventType : ulong {
		EveryEntry = 1,
		EveryExit = 2,
		FirstEntry = 3,
		LastExit = 4,
		AtHome = FirstEntry,
		NotAtHome = LastExit,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[iOS (11,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMPresenceEventUserType : ulong {
		CurrentUser = 1,
		HomeUsers = 2,
		CustomUsers = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos11.2")]
	[SupportedOSPlatform ("ios11.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,2)]
	[TV (11,2)]
	[iOS (11,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueProgramMode : long {
		NotScheduled = 0,
		Scheduled,
		ScheduleOverriddenToManual,
	}

#if NET
	[SupportedOSPlatform ("tvos11.2")]
	[SupportedOSPlatform ("ios11.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,2)]
	[TV (11,2)]
	[iOS (11,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueUsageState : long {
		NotInUse = 0,
		InUse,
	}

#if NET
	[SupportedOSPlatform ("tvos11.2")]
	[SupportedOSPlatform ("ios11.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,2)]
	[TV (11,2)]
	[iOS (11,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueValveType : long {
		GenericValve = 0,
		Irrigation,
		ShowerHead,
		WaterFaucet,
	}

#if NET
	[SupportedOSPlatform ("tvos11.2")]
	[SupportedOSPlatform ("ios11.2")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[Watch (4,2)]
	[TV (11,2)]
	[iOS (11,2)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum HMCharacteristicValueConfigurationState : long {
		NotConfigured = 0,
		Configured,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (2,0)]
	[TV (10,0)]
	[NoMac]
	[iOS (8,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetDoorState : long
	{
		Open = 0,
		Closed = 1,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (2,0)]
	[TV (10,0)]
	[NoMac]
	[iOS (8,0)]
#endif
	[Native]
	public enum HMCharacteristicValueCurrentHeatingCooling : long
	{
		Off = 0,
		Heat = 1,
		Cool = 2,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios8.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (2,0)]
	[TV (10,0)]
	[NoMac]
	[iOS (8,0)]
#endif
	[Native]
	public enum HMCharacteristicValueTargetLockMechanismState : long
	{
		Unsecured = 0,
		Secured = 1,
	}
}
