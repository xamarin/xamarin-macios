using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	/// <summary>Enumerates possible failures in Home Kit operations.</summary>
	[MacCatalyst (14, 0)]
	[Native ("HMErrorCode")]
	public enum HMError : long {
		UnexpectedError = -1,
		AlreadyExists = 1,
		NotFound = 2,
		InvalidParameter = 3,
		AccessoryNotReachable = 4,
		ReadOnlyCharacteristic = 5,
		WriteOnlyCharacteristic = 6,
		NotificationNotSupported = 7,
		OperationTimedOut = 8,
		AccessoryPoweredOff = 9,
		AccessDenied = 10,
		ObjectAssociatedToAnotherHome = 11,
		ObjectNotAssociatedToAnyHome = 12,
		ObjectAlreadyAssociatedToHome = 13,
		AccessoryIsBusy = 14,
		OperationInProgress = 15,
		AccessoryOutOfResources = 16,
		InsufficientPrivileges = 17,
		AccessoryPairingFailed = 18,
		InvalidDataFormatSpecified = 19,
		NilParameter = 20,
		UnconfiguredParameter = 21,
		InvalidClass = 22,
		OperationCancelled = 23,
		RoomForHomeCannotBeInZone = 24,
		NoActionsInActionSet = 25,
		NoRegisteredActionSets = 26,
		MissingParameter = 27,
		FireDateInPast = 28,
		RoomForHomeCannotBeUpdated = 29,
		ActionInAnotherActionSet = 30,
		ObjectWithSimilarNameExistsInHome = 31,
		HomeWithSimilarNameExists = 32,
		RenameWithSimilarName = 33,
		CannotRemoveNonBridgeAccessory = 34,
		NameContainsProhibitedCharacters = 35,
		NameDoesNotStartWithValidCharacters = 36,
		UserIDNotEmailAddress = 37,
		UserDeclinedAddingUser = 38,
		UserDeclinedRemovingUser = 39,
		UserDeclinedInvite = 40,
		UserManagementFailed = 41,
		RecurrenceTooSmall = 42,
		InvalidValueType = 43,
		ValueLowerThanMinimum = 44,
		ValueHigherThanMaximum = 45,
		StringLongerThanMaximum = 46,
		HomeAccessNotAuthorized = 47,
		OperationNotSupported = 48,
		MaximumObjectLimitReached = 49,
		AccessorySentInvalidResponse = 50,
		StringShorterThanMinimum = 51,
		GenericError = 52,
		SecurityFailure = 53,
		CommunicationFailure = 54,
		MessageAuthenticationFailed = 55,
		InvalidMessageSize = 56,
		AccessoryDiscoveryFailed = 57,
		ClientRequestError = 58,
		AccessoryResponseError = 59,
		NameDoesNotEndWithValidCharacters = 60,
		AccessoryIsBlocked = 61,
		InvalidAssociatedServiceType = 62,
		ActionSetExecutionFailed = 63,
		ActionSetExecutionPartialSuccess = 64,
		ActionSetExecutionInProgress = 65,
		AccessoryOutOfCompliance = 66,
		DataResetFailure = 67,
		NotificationAlreadyEnabled = 68,
		RecurrenceMustBeOnSpecifiedBoundaries = 69,
		DateMustBeOnSpecifiedBoundaries = 70,
		CannotActivateTriggerTooFarInFuture = 71,
		RecurrenceTooLarge = 72,
		ReadWritePartialSuccess = 73,
		ReadWriteFailure = 74,
		NotSignedIntoiCloud = 75,
		KeychainSyncNotEnabled = 76,
		CloudDataSyncInProgress = 77,
		NetworkUnavailable = 78,
		AddAccessoryFailed = 79,
		MissingEntitlement = 80,
		CannotUnblockNonBridgeAccessory = 81,
		DeviceLocked = 82,
		CannotRemoveBuiltinActionSet = 83,
		LocationForHomeDisabled = 84,
		NotAuthorizedForLocationServices = 85,
		// iOS 9.3
		ReferToUserManual = 86,
		// iOS 10.0
		InvalidOrMissingAuthorizationData = 87,
		BridgedAccessoryNotReachable = 88,
		NotAuthorizedForMicrophoneAccess = 89,
		// iOS 10.2
		IncompatibleNetwork = 90,
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
		// iOS 17.4
		PartialCommunicationFailure = 104,
	}


	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates known types of <see cref="T:HomeKit.HMCharacteristic" />.</summary>
	[MacCatalyst (14, 0)]
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

#if !NET
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

		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Field ("HMCharacteristicTypeManufacturer")]
		Manufacturer,

		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.Model' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.Model' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HMAccessory.Model' instead.")]
		[Field ("HMCharacteristicTypeModel")]
		Model,

		[Deprecated (PlatformName.TvOS, 11, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "No longer supported.")]
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

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeAirParticulateDensity")]
		AirParticulateDensity,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeAirParticulateSize")]
		AirParticulateSize,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeAirQuality")]
		AirQuality,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeBatteryLevel")]
		BatteryLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonDioxideDetected")]
		CarbonDioxideDetected,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonDioxideLevel")]
		CarbonDioxideLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonDioxidePeakLevel")]
		CarbonDioxidePeakLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonMonoxideDetected")]
		CarbonMonoxideDetected,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonMonoxideLevel")]
		CarbonMonoxideLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonMonoxidePeakLevel")]
		CarbonMonoxidePeakLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeChargingState")]
		ChargingState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeContactState")]
		ContactState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentSecuritySystemState")]
		CurrentSecuritySystemState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentHorizontalTilt")]
		CurrentHorizontalTilt,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentLightLevel")]
		CurrentLightLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentPosition")]
		CurrentPosition,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentVerticalTilt")]
		CurrentVerticalTilt,

		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[Field ("HMCharacteristicTypeFirmwareVersion")]
		FirmwareVersion,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeHardwareVersion")]
		HardwareVersion,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeHoldPosition")]
		HoldPosition,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeInputEvent")]
		InputEvent,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLeakDetected")]
		LeakDetected,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOccupancyDetected")]
		OccupancyDetected,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOutputState")]
		OutputState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypePositionState")]
		PositionState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSmokeDetected")]
		SmokeDetected,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSoftwareVersion")]
		SoftwareVersion,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusActive")]
		StatusActive,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusFault")]
		StatusFault,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusJammed")]
		StatusJammed,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusLowBattery")]
		StatusLowBattery,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusTampered")]
		StatusTampered,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetSecuritySystemState")]
		TargetSecuritySystemState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetHorizontalTilt")]
		TargetHorizontalTilt,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetPosition")]
		TargetPosition,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetVerticalTilt")]
		TargetVerticalTilt,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStreamingStatus")]
		StreamingStatus,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSetupStreamEndpoint")]
		SetupStreamEndpoint,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSupportedVideoStreamConfiguration")]
		SupportedVideoStreamConfiguration,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSupportedAudioStreamConfiguration")]
		SupportedAudioStreamConfiguration,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSupportedRTPConfiguration")]
		SupportedRtpConfiguration,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSelectedStreamConfiguration")]
		SelectedStreamConfiguration,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeVolume")]
		Volume,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeMute")]
		Mute,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeNightVision")]
		NightVision,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOpticalZoom")]
		OpticalZoom,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeDigitalZoom")]
		DigitalZoom,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeImageRotation")]
		ImageRotation,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeImageMirroring")]
		ImageMirroring,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeActive")]
		Active,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentAirPurifierState")]
		CurrentAirPurifierState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetAirPurifierState")]
		TargetAirPurifierState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentFanState")]
		CurrentFanState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentHeaterCoolerState")]
		CurrentHeaterCoolerState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentHumidifierDehumidifierState")]
		CurrentHumidifierDehumidifierState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentSlatState")]
		CurrentSlatState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeWaterLevel")]
		WaterLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeFilterChangeIndication")]
		FilterChangeIndication,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeFilterLifeLevel")]
		FilterLifeLevel,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeFilterResetChangeIndication")]
		FilterResetChangeIndication,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLockPhysicalControls")]
		LockPhysicalControls,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSwingMode")]
		SwingMode,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetHeaterCoolerState")]
		TargetHeaterCoolerState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetHumidifierDehumidifierState")]
		TargetHumidifierDehumidifierState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetFanState")]
		TargetFanState,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSlatType")]
		SlatType,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentTilt")]
		CurrentTilt,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetTilt")]
		TargetTilt,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOzoneDensity")]
		OzoneDensity,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeNitrogenDioxideDensity")]
		NitrogenDioxideDensity,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSulphurDioxideDensity")]
		SulphurDioxideDensity,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypePM2_5Density")]
		PM2_5Density,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypePM10Density")]
		PM10Density,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeVolatileOrganicCompoundDensity")]
		VolatileOrganicCompoundDensity,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeDehumidifierThreshold")]
		DehumidifierThreshold,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeHumidifierThreshold")]
		HumidifierThreshold,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSecuritySystemAlarmType")]
		SecuritySystemAlarmType,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLabelNamespace")]
		LabelNamespace,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLabelIndex")]
		LabelIndex,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeColorTemperature")]
		ColorTemperature,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeProgramMode")]
		ProgramMode,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeInUse")]
		InUse,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSetDuration")]
		SetDuration,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeRemainingDuration")]
		RemainingDuration,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeValveType")]
		ValveType,

		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeIsConfigured")]
		IsConfigured,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeInputSourceType")]
		InputSourceType,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeInputDeviceType")]
		InputDeviceType,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeClosedCaptions")]
		ClosedCaptions,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypePowerModeSelection")]
		PowerModeSelection,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeCurrentMediaState")]
		CurrentMediaState,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeRemoteKey")]
		RemoteKey,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypePictureMode")]
		PictureMode,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeConfiguredName")]
		ConfiguredName,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeIdentifier")]
		Identifier,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeActiveIdentifier")]
		ActiveIdentifier,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeTargetVisibilityState")]
		TargetVisibilityState,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeCurrentVisibilityState")]
		CurrentVisibilityState,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeRouterStatus")]
		RouterStatus,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeWANStatusList")]
		WanStatusList,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeWiFiSatelliteStatus")]
		WiFiSatelliteStatus,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeVolumeControlType")]
		VolumeControlType,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeVolumeSelector")]
		VolumeSelector,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMCharacteristicTypeTargetMediaState")]
		TargetMediaState,
	}

	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates units of measure that might be associated with a <see cref="T:HomeKit.HMCharacteristic" />.</summary>
	[MacCatalyst (13, 1)]
	public enum HMCharacteristicMetadataUnits {
		None,
		Celsius,
		Fahrenheit,
		Percentage,
		ArcDegree,
		[MacCatalyst (13, 1)]
		Seconds,
		[MacCatalyst (13, 1)]
		Lux,
		[MacCatalyst (13, 1)]
		PartsPerMillion,
		[MacCatalyst (13, 1)]
		MicrogramsPerCubicMeter,
	}

	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates known types of <see cref="T:HomeKit.HMService" />.</summary>
	[MacCatalyst (14, 0)]
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

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeAirQualitySensor")]
		AirQualitySensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeBattery")]
		Battery,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCarbonDioxideSensor")]
		CarbonDioxideSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCarbonMonoxideSensor")]
		CarbonMonoxideSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeContactSensor")]
		ContactSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeDoor")]
		Door,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeHumiditySensor")]
		HumiditySensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeLeakSensor")]
		LeakSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeLightSensor")]
		LightSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeMotionSensor")]
		MotionSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeOccupancySensor")]
		OccupancySensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSecuritySystem")]
		SecuritySystem,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeStatefulProgrammableSwitch")]
		StatefulProgrammableSwitch,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeStatelessProgrammableSwitch")]
		StatelessProgrammableSwitch,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSmokeSensor")]
		SmokeSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeTemperatureSensor")]
		TemperatureSensor,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeWindow")]
		Window,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeWindowCovering")]
		WindowCovering,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCameraRTPStreamManagement")]
		CameraRtpStreamManagement,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCameraControl")]
		CameraControl,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeMicrophone")]
		Microphone,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSpeaker")]
		Speaker,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeDoorbell")]
		Doorbell,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeAirPurifier")]
		AirPurifier,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeVentilationFan")]
		VentilationFan,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeFilterMaintenance")]
		FilterMaintenance,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeHeaterCooler")]
		HeaterCooler,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeHumidifierDehumidifier")]
		HumidifierDehumidifier,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSlats")]
		Slats,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeLabel")]
		Label,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeIrrigationSystem")]
		IrrigationSystem,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeValve")]
		Valve,

		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeFaucet")]
		Faucet,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMServiceTypeTelevision")]
		Television,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMServiceTypeInputSource")]
		InputSource,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMServiceTypeWiFiRouter")]
		WiFiRouter,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMServiceTypeWiFiSatellite")]
		WiFiSatellite,
	}

	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates the data types that can represent characteristics.</summary>
	[MacCatalyst (13, 1)]
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

	/// <summary>Enumerates the states that a door can be in.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueDoorState : long {
		Open = 0,
		Closed,
		Opening,
		Closing,
		Stopped
	}

	/// <summary>Enumerates the states that an indoor climate control unit can be in.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueHeatingCooling : long {
		Off = 0,
		Heat,
		Cool,
		Auto
	}

	/// <summary>Enumerates rotation directions.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueRotationDirection : long {
		Clockwise = 0,
		CounterClockwise
	}

	/// <summary>Enumerates two common temperature measurement systems.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTemperatureUnit : long {
		Celsius = 0,
		Fahrenheit
	}

	/// <summary>Enumerates the states that a lock can be in.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLockMechanismState : long {
		Unsecured = 0,
		Secured,
		Jammed,
		Unknown
	}

	/// <summary>Enumerates values that describe the last known action that was taken on a lock.</summary>
	[MacCatalyst (14, 0)]
	[Native ("HMCharacteristicValueLockMechanismLastKnownAction")]
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

	/// <summary>Enumerates the particulate sizes supported by HomeKit.</summary>
	[MacCatalyst (14, 0)]
	[Native ("HMCharacteristicValueAirParticulateSize")]
	public enum HMCharacteristicValueAirParticulate : long {
		Size2_5 = 0,
		Size10
	}

	/// <summary>Enumerates the states of a security system.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentSecuritySystemState : long {
		StayArm = 0,
		AwayArm,
		NightArm,
		Disarmed,
		Triggered
	}

	/// <summary>Enumerates the states of an object that opens and closes.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValuePositionState : long {
		Closing = 0,
		Opening,
		Stopped
	}

	/// <summary>Enumerates the status of a security system.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetSecuritySystemState : long {
		StayArm = 0,
		AwayArm,
		NightArm,
		Disarm
	}

	/// <summary>Enumerates the valid battery status values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueBatteryStatus : long {
		Normal = 0,
		Low
	}

	/// <summary>Enumerates the state of a jammed device.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueJammedStatus : long {
		None = 0,
		Jammed
	}

	/// <summary>Enumerates the states of a device that detected tampering.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTamperedStatus : long {
		None = 0,
		Tampered
	}

	/// <summary>Enumerates the values of a device that detects leaks.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLeakStatus : long {
		None = 0,
		Detected
	}

	/// <summary>Enumerates the state of a smoke detector.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSmokeDetectionStatus : long {
		None = 0,
		Detected
	}

	/// <summary>Enumerates the state of battery charging.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueChargingState : long {
		None = 0,
		InProgress,
		[MacCatalyst (14, 0)]
		NotChargeable,
	}

	/// <summary>Enumerates whether contact is detected.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueContactState : long {
		Detected = 0,
		None,
	}

	/// <summary>Enumerates fault states.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueStatusFault : long {
		NoFault = 0,
		GeneralFault
	}

	/// <summary>Enumerates detection values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCarbonMonoxideDetectionStatus : long {
		NotDetected = 0,
		Detected
	}

	/// <summary>Enumerates detection values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCarbonDioxideDetectionStatus : long {
		NotDetected = 0,
		Detected
	}

	/// <summary>Enumerates values for occupancy.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueOccupancyStatus : long {
		NotOccupied = 0,
		Occupied
	}

	/// <summary>Enumerates the security system alarm type.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSecuritySystemAlarmType : long {
		NoAlarm = 0,
		Unknown
	}

	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates the different kinds of <see cref="T:HomeKit.HMActionSet" />.</summary>
	[MacCatalyst (13, 1)]
	public enum HMActionSetType {
		Unknown = -1,
		WakeUp,
		Sleep,
		HomeDeparture,
		HomeArrival,
		UserDefined,
		[MacCatalyst (13, 1)]
		TriggerOwned,
	}

	/// <summary>Enumerates the kind of accessories supported by HomeKit.</summary>
	[MacCatalyst (14, 0)]
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

#if !TVOS
		[Obsolete ("Use 'GarageDoorOpener' instead.")]
		[NoTV]
		[MacCatalyst (13, 1)]
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

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeRangeExtender")]
		RangeExtender,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeIPCamera")]
		IPCamera,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeVideoDoorbell")]
		VideoDoorbell,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirPurifier")]
		AirPurifier,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirHeater")]
		AirHeater,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirConditioner")]
		AirConditioner,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirHumidifier")]
		AirHumidifier,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirDehumidifier")]
		AirDehumidifier,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeSprinkler")]
		Sprinkler,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeFaucet")]
		Faucet,

		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeShowerHead")]
		ShowerHead,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeTelevision")]
		Television,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeTelevisionSetTopBox")]
		TelevisionSetTopBox,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeTelevisionStreamingStick")]
		TelevisionStreamingStick,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeWiFiRouter")]
		WiFiRouter,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeSpeaker")]
		Speaker,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeAudioReceiver")]
		AudioReceiver,

		[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HMAccessoryCategoryTypeAirPort")]
		AirPort,
	}

	/// <summary>Enumerates events such as sunrise and sunset.</summary>
	[MacCatalyst (14, 0)]
	public enum HMSignificantEvent {

		[Field ("HMSignificantEventSunrise")]
		Sunrise,

		[Field ("HMSignificantEventSunset")]
		Sunset,
	}

	[iOS (13, 0), TV (13, 0), NoMac, MacCatalyst (14, 0)]
	[Flags]
	[Native]
	public enum HMHomeManagerAuthorizationStatus : ulong {
		Determined = 1 << 0,
		Restricted = 1 << 1,
		Authorized = 1 << 2,
	}

	/// <summary>Enumerates air quality values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueAirQuality : long {
		Unknown = 0,
		Excellent,
		Good,
		Fair,
		Inferior,
		Poor
	}

	/// <summary>Enumerates the states of an <see cref="T:HomeKit.HMCameraStream" />.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCameraStreamState : ulong {
		Starting = 1,
		Streaming = 2,
		Stopping = 3,
		NotStreaming = 4
	}

	/// <summary>Settings for camera's with audio devices. Used with <see cref="P:HomeKit.HMCameraStream.AudioStreamSetting" />.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCameraAudioStreamSetting : ulong {
		Muted = 1,
		IncomingAudioAllowed = 2,
		BidirectionalAudioAllowed = 3
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that may either be locked or unlocked.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLockPhysicalControlsState : long {
		NotLocked = 0,
		Locked,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the states of an air purifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentAirPurifierState : long {
		Inactive = 0,
		Idle,
		Active,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> of an air purifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetAirPurifierState : long {
		Manual = 0,
		Automatic,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the states of an oscillating slat.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentSlatState : long {
		Stationary = 0,
		Jammed,
		Oscillating,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describe a vertical or horizontal slat.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSlatType : long {
		Horizontal = 0,
		Vertical,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the states of a filter that must be periodically changed.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueFilterChange : long {
		NotNeeded = 0,
		Needed,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the states of a fan.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentFanState : long {
		Inactive = 0,
		Idle,
		Active,
	}

	/// <summary>Enumerates the possible states of a <see cref="T:HomeKit.HMCharacteristic" /> of a fan.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetFanState : long {
		Manual = 0,
		Automatic,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the states of a heat exchanger.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentHeaterCoolerState : long {
		Inactive = 0,
		Idle,
		Heating,
		Cooling,
	}

	/// <summary>Enumerates the possible states of a <see cref="T:HomeKit.HMCharacteristic" /> of a heater / cooler.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetHeaterCoolerState : long {
		Automatic = 0,
		Heat,
		Cool,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the states of a humidifier or dehumidifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentHumidifierDehumidifierState : long {
		Inactive = 0,
		Idle,
		Humidifying,
		Dehumidifying,
	}

	/// <summary>Enumerates the possible states of a <see cref="T:HomeKit.HMCharacteristic" /> of a humidifier or dehumidifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetHumidifierDehumidifierState : long {
		Automatic = 0,
		Humidify,
		Dehumidify,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describe a swing that can be enabled or disabled.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSwingMode : long {
		Disabled = 0,
		Enabled,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that can be active or inactive.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueActivationState : long {
		Inactive = 0,
		Active,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that describes the forms of a button press.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueInputEvent : long {
		SinglePress = 0,
		DoublePress,
		LongPress,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that may either be a dot or a numeral.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLabelNamespace : long {
		Dot = 0,
		Numeral,
	}

	/// <summary>Enumerates the possible states of a <see cref="T:HomeKit.HMEventTrigger" /> including reasons why it might not be enabled.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMEventTriggerActivationState : ulong {
		Disabled = 0,
		DisabledNoHomeHub = 1,
		DisabledNoCompatibleHomeHub = 2,
		DisabledNoLocationServicesAuthorization = 3,
		Enabled = 4,
	}

	/// <summary>Enumerates the possible states of the HomeKit hub.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMHomeHubState : ulong {
		NotAvailable = 0,
		Connected,
		Disconnected,
	}

	/// <summary>Enumerates the type of change in presence that the <see cref="T:HomeKit.HMPresenceEvent" /> represents.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMPresenceEventType : ulong {
		EveryEntry = 1,
		EveryExit = 2,
		FirstEntry = 3,
		LastExit = 4,
		AtHome = FirstEntry,
		NotAtHome = LastExit,
	}

	/// <summary>Enumerates the types of users whose presence or absence is associated with a <see cref="T:HomeKit.HMPresenceEvent" />.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMPresenceEventUserType : ulong {
		CurrentUser = 1,
		HomeUsers = 2,
		CustomUsers = 3,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that can be scheduled or manually overriden.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueProgramMode : long {
		NotScheduled = 0,
		Scheduled,
		ScheduleOverriddenToManual,
	}

	/// <summary>Enumerates the possible states of a <see cref="T:HomeKit.HMCharacteristic" /> of a device that can be in use or out of use.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueUsageState : long {
		NotInUse = 0,
		InUse,
	}

	/// <summary>Enumerates the known types of valve.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueValveType : long {
		GenericValve = 0,
		Irrigation,
		ShowerHead,
		WaterFaucet,
	}

	/// <summary>Enumerates the possible values of a <see cref="T:HomeKit.HMCharacteristic" /> that can be configured or not.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueConfigurationState : long {
		NotConfigured = 0,
		Configured,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[Native]
	public enum HMCharacteristicValueTargetDoorState : long {
		Open = 0,
		Closed = 1,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[Native]
	public enum HMCharacteristicValueCurrentHeatingCooling : long {
		Off = 0,
		Heat = 1,
		Cool = 2,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[Native]
	public enum HMCharacteristicValueTargetLockMechanismState : long {
		Unsecured = 0,
		Secured = 1,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueInputSourceType : long {
		Other = 0,
		HomeScreen = 1,
		Tuner = 2,
		Hdmi = 3,
		CompositeVideo = 4,
		SVideo = 5,
		ComponentVideo = 6,
		Dvi = 7,
		AirPlay = 8,
		Usb = 9,
		Application = 10,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueInputDeviceType : long {
		Other = 0,
		TV = 1,
		Recording = 2,
		Tuner = 3,
		Playback = 4,
		AudioSystem = 5,
		None = 6,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueClosedCaptions : long {
		Disabled = 0,
		Enabled = 1,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValuePowerModeSelection : long {
		Show = 0,
		Hide = 1,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentMediaState : long {
		Playing = 0,
		Paused = 1,
		Stopped = 2,
		Unknown = 3,
		Loading = 4,
		Interrupted = 5,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueRemoteKey : long {
		Rewind = 0,
		FastForward = 1,
		NextTrack = 2,
		PreviousTrack = 3,
		ArrowUp = 4,
		ArrowDown = 5,
		ArrowLeft = 6,
		ArrowRight = 7,
		Select = 8,
		Back = 9,
		Exit = 10,
		PlayPause = 11,
		Play = 12,
		Pause = 13,
		Menu = 14,
		Info = 15,
		Home = 16,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValuePictureMode : long {
		Standard = 0,
		Movie = 1,
		Sport = 2,
		Game = 3,
		Photo = 4,
		Vivid = 5,
		Dark = 6,
		Bright = 7,
		Computer = 8,
		Night = 9,
		Calibrated = 10,
		Custom1 = 11,
		Custom2 = 12,
		Custom3 = 13,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueTargetVisibilityState : long {
		Show = 0,
		Hide = 1,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentVisibilityState : long {
		Shown = 0,
		Hidden = 1,
		Connected = 2,
		AlwaysShown = 3,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueRouterStatus : long {
		Ready = 0,
		NotReady = 1,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueWiFiSatelliteStatus : long {
		Unknown = 0,
		Connected = 1,
		NotConnected = 2,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueVolumeControlType : long {
		None = 0,
		Relative,
		RelativeWithCurrent,
		Absolute,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueVolumeSelector : long {
		VolumeIncrement = 0,
		VolumeDecrement,
	}

	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueTargetMediaState : long {
		Play = 0,
		Pause = 1,
		Stop = 2,
	}
}
