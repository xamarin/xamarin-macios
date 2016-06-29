using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	[iOS (8,0)]
	[Native]
	public enum HMError : nint {
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
	}

	
	// conveniance enum (ObjC uses NSString)
	[iOS (8,0)]
	public enum HMCharacteristicType {
		None,
		PowerState,
		Hue,
		Saturation,
		Brightness,
		TemperatureUnits,
		CurrentTemperature,
		TargetTemperature,
		CurrentHeatingCooling,
		TargetHeatingCooling,
		CoolingThreshold,
		HeatingThreshold,
		HeatingCoolingStatus,
		CurrentRelativeHumidity,
		TargetRelativeHumidity,
		CurrentDoorState,
		TargetDoorState,
		ObstructionDetected,
		Name,
		Manufacturer,
		Model,
		SerialNumber,
		Identify,
		RotationDirection,
		RotationSpeed,
		OutletInUse,
		Version,
		Logs,
		AudioFeedback,
		AdminOnlyAccess,
		MotionDetected,
		CurrentLockMechanismState,
		TargetLockMechanismState,
		LockMechanismLastKnownAction,
		LockManagementControlPoint,
		LockManagementAutoSecureTimeout,
		[iOS (9,0)]
		AirParticulateDensity,
		[iOS (9,0)]
		AirParticulateSize,
		[iOS (9,0)]
		AirQuality,
		[iOS (9,0)]
		BatteryLevel,
		[iOS (9,0)]
		CarbonDioxideDetected,
		[iOS (9,0)]
		CarbonDioxideLevel,
		[iOS (9,0)]
		CarbonDioxidePeakLevel,
		[iOS (9,0)]
		CarbonMonoxideDetected,
		[iOS (9,0)]
		CarbonMonoxideLevel,
		[iOS (9,0)]
		CarbonMonoxidePeakLevel,
		[iOS (9,0)]
		ChargingState,
		[iOS (9,0)]
		ContactState,
		[iOS (9,0)]
		CurrentSecuritySystemState,
		[iOS (9,0)]
		CurrentHorizontalTilt,
		[iOS (9,0)]
		CurrentLightLevel,
		[iOS (9,0)]
		CurrentPosition,
		[iOS (9,0)]
		CurrentVerticalTilt,
		[iOS (9,0)]
		FirmwareVersion,
		[iOS (9,0)]
		HardwareVersion,
		[iOS (9,0)]
		HoldPosition,
		[iOS (9,0)]
		InputEvent,
		[iOS (9,0)]
		LeakDetected,
		[iOS (9,0)]
		OccupancyDetected,
		[iOS (9,0)]
		OutputState,
		[iOS (9,0)]
		PositionState,
		[iOS (9,0)]
		SmokeDetected,
		[iOS (9,0)]
		SoftwareVersion,
		[iOS (9,0)]
		StatusActive,
		[iOS (9,0)]
		StatusFault,
		[iOS (9,0)]
		StatusJammed,
		[iOS (9,0)]
		StatusLowBattery,
		[iOS (9,0)]
		StatusTampered,
		[iOS (9,0)]
		TargetSecuritySystemState,
		[iOS (9,0)]
		TargetHorizontalTilt,
		[iOS (9,0)]
		TargetPosition,
		[iOS (9,0)]
		TargetVerticalTilt,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		StreamingStatus,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		SupportedVideoStreamConfiguration,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		SupportedAudioStreamConfiguration,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		SupportedRtpConfiguration,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		SelectedStreamConfiguration,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		Volume,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		Mute,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		NightVision,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		OpticalZoom,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		DigitalZoom,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		ImageRotation,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		ImageMirroring,
	}

	// conveniance enum (ObjC uses NSString)
	[iOS (8,0)]
	public enum HMCharacteristicMetadataUnits {
		None,
		Celsius,
		Fahrenheit,
		Percentage,
		ArcDegree,
		[iOS (8,3)]
		Seconds,
		[iOS (9,3)][Watch(2,2)]
		Lux,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		PartsPerMillion,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		MicrogramsPerCubicMeter,
	}

	// conveniance enum (ObjC uses NSString)
	[iOS (8,0)]
	[Flags]
	public enum HMServiceType {
		None,
		LightBulb,
		Switch,
		Thermostat,
		GarageDoorOpener,
		AccessoryInformation,
		Fan,
		Outlet,
		LockMechanism,
		LockManagement,
		[iOS (9,0)]
		AirQualitySensor,
		[iOS (9,0)]
		Battery,
		[iOS (9,0)]
		CarbonDioxideSensor,
		[iOS (9,0)]
		CarbonMonoxideSensor,
		[iOS (9,0)]
		ContactSensor,
		[iOS (9,0)]
		Door,
		[iOS (9,0)]
		HumiditySensor,
		[iOS (9,0)]
		LeakSensor,
		[iOS (9,0)]
		LightSensor,
		[iOS (9,0)]
		MotionSensor,
		[iOS (9,0)]
		OccupancySensor,
		[iOS (9,0)]
		SecuritySystem,
		[iOS (9,0)]
		StatefulProgrammableSwitch,
		[iOS (9,0)]
		StatelessProgrammableSwitch,
		[iOS (9,0)]
		SmokeSensor,
		[iOS (9,0)]
		TemperatureSensor,
		[iOS (9,0)]
		Window,
		[iOS (9,0)]
		WindowCovering,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		CameraRtpStreamManagement,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		CameraControl,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		Microphone,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		Speaker,
	}

	// conveniance enum (ObjC uses NSString)
	[iOS (8,0)]
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

	[iOS (8,0)]
	[Native]
	public enum HMCharacteristicValueDoorState : nint {
		Open = 0,
		Closed,
		Opening,
		Closing,
		Stopped
	}

	[iOS (8,0)]
	[Native]
	public enum HMCharacteristicValueHeatingCooling : nint {
		Off = 0,
		Heat,
		Cool,
		Auto
	}

	[iOS (8,0)]
	[Native]
	public enum HMCharacteristicValueRotationDirection : nint {
		Clockwise = 0,
		CounterClockwise
	}

	[iOS (8,0)]
	[Native]
	public enum HMCharacteristicValueTemperatureUnit : nint {
		Celsius = 0,
		Fahrenheit
	}

	[iOS (8,0)]
	[Native]
	public enum HMCharacteristicValueLockMechanismState : nint {
		Unsecured = 0,
		Secured,
		Jammed,
		Unknown
	}

	[iOS (8,0)]
	[Native]
	// in iOS 8.3 this was renamed HMCharacteristicValueLockMechanismLastKnownAction but that would be a breaking change for us
	public enum HMCharacteristicValueLockMechanism : nint {
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

	[iOS (9,0)]
	[Native]
	public enum HMCharacteristicValueAirParticulate : nint {
		Size2_5 = 0,
		Size10
	}

	[iOS (9,0)]
	[Native]
	public enum HMCharacteristicValueCurrentSecuritySystemState : nint {
		StayArm = 0,
		AwayArm,
		NightArm,
		Disarmed,
		Triggered
	}

	[iOS (9,0)]
	[Native]
	public enum HMCharacteristicValuePositionState : nint {
		Closing = 0,
		Opening,
		Stopped
	}

	[iOS (9,0)]
	[Native]
	public enum HMCharacteristicValueTargetSecuritySystemState : nint {
		StayArm = 0,
		AwayArm,
		NightArm,
		Disarm
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueBatteryStatus : nint {
		Normal = 0,
		Low
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueJammedStatus : nint {
		None = 0,
		Jammed
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueTamperedStatus : nint {
		None = 0,
		Tampered
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueLeakStatus : nint {
		None = 0,
		Detected
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueSmokeDetectionStatus : nint {
		None = 0,
		Detected
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueChargingState : nint {
		None = 0,
		InProgress
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueContactState : nint {
		None = 0,
		Detected
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueStatusFault : nint {
		NoFault = 0,
		GeneralFault
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueCarbonMonoxideDetectionStatus : nint {
		NotDetected = 0,
		Detected
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueCarbonDioxideDetectionStatus : nint {
		NotDetected = 0,
		Detected
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueOccupancyStatus : nint {
		NotOccupied = 0,
		Occupied
	}

	[Watch (3,0), TV (10,0), iOS (10,0)]
	[Native]
	public enum HMCharacteristicValueSecuritySystemAlarmType : nint {
		NoAlarm = 0,
		Unknown
	}

	// conveniance enum (ObjC uses NSString)
	[iOS (9,0)]
	public enum HMActionSetType {
		Unknown = -1,
		WakeUp,
		Sleep,
		HomeDeparture,
		HomeArrival,
		UserDefined,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		TriggerOwned,
	}

	[iOS (9,0)]
	// conveniance enum (ObjC uses NSString)
	public enum HMAccessoryCategoryType {
		Other = 0,
		SecuritySystem,
		Bridge,
		Door,
		DoorLock,
		Fan,
		[Obsolete ("Use GarageDoorOpener instead")]
		DoorOpener,
		Lightbulb,
		Outlet,
		ProgrammableSwitch,
		Sensor,
		Switch,
		Thermostat,
		Window,
		WindowCovering,
		GarageDoorOpener = DoorOpener,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		RangeExtender,
		[iOS (10,0), Watch (3,0), TV (10,0)]
		IPCamera,
	}

	[iOS (9,0)]
	// conveniance enum (ObjC uses NSString)
	public enum HMSignificantEvent {
		Sunrise,
		Sunset,
	}

	[iOS (9,0)]
	[Native]
	public enum HMCharacteristicValueAirQuality : nint {
		Unknown = 0,
		Excellent,
		Good,
		Fair,
		Inferior,
		Poor
	}

	[iOS (10,0)]
	[Native]
	public enum HMCameraStreamState : nuint
	{
		Starting = 1,
		Streaming = 2,
		Stopping = 3,
		NotStreaming = 4
	}

	[iOS (10,0)]
	[Native]
	public enum HMCameraAudioStreamSetting : nuint
	{
		Muted = 1,
		IncomingAudioAllowed = 2,
		BidirectionalAudioAllowed = 3
	}
}
