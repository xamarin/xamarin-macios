
using System.ComponentModel;

namespace HomeKit {

	/// <summary>Enumerates possible failures in Home Kit operations.</summary>
	[MacCatalyst (14, 0)]
	[Native ("HMErrorCode")]
	public enum HMError : long {
		/// <summary>An unexpected error occurred..</summary>
		UnexpectedError = -1,
		/// <summary>The container already has the object and another cannot be added.</summary>
		AlreadyExists = 1,
		/// <summary>The object was not found in the container.</summary>
		NotFound = 2,
		/// <summary>A parameter was invalid.</summary>
		InvalidParameter = 3,
		/// <summary>The accessory could not be reached.</summary>
		AccessoryNotReachable = 4,
		/// <summary>The characteristic cannot be written to.</summary>
		ReadOnlyCharacteristic = 5,
		/// <summary>The characteristic cannot be read.</summary>
		WriteOnlyCharacteristic = 6,
		/// <summary>The notification could not be enabled because it is not supported.</summary>
		NotificationNotSupported = 7,
		/// <summary>The operation timed out.</summary>
		OperationTimedOut = 8,
		/// <summary>The accessory was turned off.</summary>
		AccessoryPoweredOff = 9,
		/// <summary>Access to a resource was denied.</summary>
		AccessDenied = 10,
		/// <summary>The object could not be associated because it was already associated with another home.</summary>
		ObjectAssociatedToAnotherHome = 11,
		/// <summary>The operation could not be performed because the object was not associated with any home.</summary>
		ObjectNotAssociatedToAnyHome = 12,
		/// <summary>The object could not be associated because it was already associated with the home.</summary>
		ObjectAlreadyAssociatedToHome = 13,
		/// <summary>The accessory was busy.</summary>
		AccessoryIsBusy = 14,
		/// <summary>Another operation was in progress.</summary>
		OperationInProgress = 15,
		/// <summary>The accessory was out of resources.</summary>
		AccessoryOutOfResources = 16,
		/// <summary>The user lacked the privilege to perform the operation.</summary>
		InsufficientPrivileges = 17,
		/// <summary>The accessory could not be paired.</summary>
		AccessoryPairingFailed = 18,
		/// <summary>An invalid data format was specified.</summary>
		InvalidDataFormatSpecified = 19,
		/// <summary>A parameter that must not be <see langword="null" /> was supplied as <see langword="null" />.</summary>
		NilParameter = 20,
		/// <summary>The supplied parameter was not configured.</summary>
		UnconfiguredParameter = 21,
		/// <summary>An invalid abstract base class was used.</summary>
		InvalidClass = 22,
		/// <summary>The operation was canceled.</summary>
		OperationCancelled = 23,
		/// <summary>The room that specifies the entire home could not be added to a zone within the home.</summary>
		RoomForHomeCannotBeInZone = 24,
		/// <summary>An operation tried to execute an empty action set.</summary>
		NoActionsInActionSet = 25,
		/// <summary>An operation tried to activate a trigger that had no action sets.</summary>
		NoRegisteredActionSets = 26,
		/// <summary>A required parameter was missing.</summary>
		MissingParameter = 27,
		/// <summary>The date the action was to be fired was in the past.</summary>
		FireDateInPast = 28,
		/// <summary>An attempt was made to update the room for the entire home.</summary>
		RoomForHomeCannotBeUpdated = 29,
		/// <summary>The action was already in another action set and could not be added.</summary>
		ActionInAnotherActionSet = 30,
		/// <summary>The object could not be associated because another existing object has a similar name.</summary>
		ObjectWithSimilarNameExistsInHome = 31,
		/// <summary>A home with a similar name already exists.</summary>
		HomeWithSimilarNameExists = 32,
		/// <summary>An attempt was made to rename an object, but the new name was too similar to an existing object.</summary>
		RenameWithSimilarName = 33,
		/// <summary>The accessory could not be removed because it is bridged.</summary>
		CannotRemoveNonBridgeAccessory = 34,
		/// <summary>The supplied name contains prohibited characters.</summary>
		NameContainsProhibitedCharacters = 35,
		/// <summary>The supplied name started with invalid characters.</summary>
		NameDoesNotStartWithValidCharacters = 36,
		/// <summary>The supplied user ID was not an email address.</summary>
		UserIDNotEmailAddress = 37,
		/// <summary>A user declined to add the user.</summary>
		UserDeclinedAddingUser = 38,
		/// <summary>The user declined to remove a user.</summary>
		UserDeclinedRemovingUser = 39,
		/// <summary>A user declined an invitation from the user.</summary>
		UserDeclinedInvite = 40,
		/// <summary>An unspecified user management error occurred.</summary>
		UserManagementFailed = 41,
		/// <summary>The recurrence length was too small.</summary>
		RecurrenceTooSmall = 42,
		/// <summary>Data of an incorrect type were provided.</summary>
		InvalidValueType = 43,
		/// <summary>A value did not meet its required minimum.</summary>
		ValueLowerThanMinimum = 44,
		/// <summary>A value exceeded its allowed maximum.</summary>
		ValueHigherThanMaximum = 45,
		/// <summary>The maximum string length was exceeded.</summary>
		StringLongerThanMaximum = 46,
		/// <summary>Home access was not authorized.</summary>
		HomeAccessNotAuthorized = 47,
		/// <summary>The operation is not supported.</summary>
		OperationNotSupported = 48,
		/// <summary>The maximum number of objects have already been created or stored.</summary>
		MaximumObjectLimitReached = 49,
		/// <summary>The accessory's response was invalid.</summary>
		AccessorySentInvalidResponse = 50,
		/// <summary>The minimum string length was not met.</summary>
		StringShorterThanMinimum = 51,
		/// <summary>An unspecified error occurred.</summary>
		GenericError = 52,
		/// <summary>There was a security failure.</summary>
		SecurityFailure = 53,
		/// <summary>There was a communication error.</summary>
		CommunicationFailure = 54,
		/// <summary>The message could not be authenticated.</summary>
		MessageAuthenticationFailed = 55,
		/// <summary>The message size was invalid.</summary>
		InvalidMessageSize = 56,
		/// <summary>A failure occurred while attempting to discover an accessory.</summary>
		AccessoryDiscoveryFailed = 57,
		/// <summary>The was a client request error.</summary>
		ClientRequestError = 58,
		/// <summary>The accessory response indicated an error.</summary>
		AccessoryResponseError = 59,
		/// <summary>The supplied name ended with invalid characters.</summary>
		NameDoesNotEndWithValidCharacters = 60,
		/// <summary>The accessory was blocked.</summary>
		AccessoryIsBlocked = 61,
		/// <summary>The associated service type was invalid.</summary>
		InvalidAssociatedServiceType = 62,
		/// <summary>The action set failed to execute.</summary>
		ActionSetExecutionFailed = 63,
		/// <summary>The action set partially succeeded.</summary>
		ActionSetExecutionPartialSuccess = 64,
		/// <summary>The action set was already in progress.</summary>
		ActionSetExecutionInProgress = 65,
		/// <summary>The accessory was out of compliance.</summary>
		AccessoryOutOfCompliance = 66,
		/// <summary>Data resetting failed.</summary>
		DataResetFailure = 67,
		/// <summary>The notification could not be enabled because it already was enabled.</summary>
		NotificationAlreadyEnabled = 68,
		/// <summary>The recurrence was required to be within the specified boundaries, and it was not.</summary>
		RecurrenceMustBeOnSpecifiedBoundaries = 69,
		/// <summary>The date was required to be within specified boundaries, and it was not.</summary>
		DateMustBeOnSpecifiedBoundaries = 70,
		/// <summary>The trigger could not be activated because it takes place too far into the future.</summary>
		CannotActivateTriggerTooFarInFuture = 71,
		/// <summary>The recurrence length was too large.</summary>
		RecurrenceTooLarge = 72,
		/// <summary>The read or write partially succeeded.</summary>
		ReadWritePartialSuccess = 73,
		/// <summary>The read or write failed.</summary>
		ReadWriteFailure = 74,
		/// <summary>The user or application is not signed in to iCloud.</summary>
		NotSignedIntoICloud = 75,
#if !XAMCORE_5_0
		[Obsolete ("Use 'NotSignedIntoICloud' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		NotSignedIntoiCloud = NotSignedIntoICloud,
#endif
		/// <summary>Keychain synchronization was not enabled.</summary>
		KeychainSyncNotEnabled = 76,
		/// <summary>Data was synchronizing.</summary>
		CloudDataSyncInProgress = 77,
		/// <summary>The network was unavailable.</summary>
		NetworkUnavailable = 78,
		/// <summary>The accessory could not be added.</summary>
		AddAccessoryFailed = 79,
		/// <summary>An entitlement was missing.</summary>
		MissingEntitlement = 80,
		/// <summary>An accessory, accessed via a bridge, cannot be unblocked.</summary>
		CannotUnblockNonBridgeAccessory = 81,
		/// <summary>The device is locked.</summary>
		DeviceLocked = 82,
		/// <summary>The user attempted to remove a non-deletable <see cref="HomeKit.HMActionSet" />.</summary>
		CannotRemoveBuiltinActionSet = 83,
		/// <summary>The user has not made their home's location available.</summary>
		LocationForHomeDisabled = 84,
		/// <summary>The user has not explicitly allowed location services.</summary>
		NotAuthorizedForLocationServices = 85,
		// iOS 9.3
		/// <summary>The error should be diagnosed using the user manual.</summary>
		ReferToUserManual = 86,
		// iOS 10.0
		/// <summary>Authorization is required.</summary>
		InvalidOrMissingAuthorizationData = 87,
		/// <summary>The accessory, expected to be reachable via a bridge, is not reachable.</summary>
		BridgedAccessoryNotReachable = 88,
		/// <summary>The user has not explicitly allowed access to the device microphone.</summary>
		NotAuthorizedForMicrophoneAccess = 89,
		// iOS 10.2
		/// <summary>The network was not compatible.</summary>
		IncompatibleNetwork = 90,
		// iOS 11
		/// <summary>No compatible hub could be found.</summary>
		NoHomeHub = 91,
		/// <summary>The hub was not compatible.</summary>
		IncompatibleHomeHub = 92, // HMErrorCodeNoCompatibleHomeHub introduced and deprecated on iOS 11. HMErrorCodeIncompatibleHomeHub = HMErrorCodeNoCompatibleHomeHub.
		/// <summary>The accessory could not be added because it was not compatible with HomeKit.</summary>
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
		// iOS 18.4
		HomeUpgradeRequired = 105,
#if !XAMCORE_5_0
		// iOS 26.1 - Apple removed this enum value from native headers in Xcode 26.4
		QuotaExceeded = 106,
#endif
	}


	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates known types of <see cref="HomeKit.HMCharacteristic" />.</summary>
	[MacCatalyst (14, 0)]
	public enum HMCharacteristicType {
		/// <summary>No type is specified.</summary>
		None,

		/// <summary>A Boolean value that indicates the accessory's power state.</summary>
		[Field ("HMCharacteristicTypePowerState")]
		PowerState,

		/// <summary>A float that indicates the hue of a color in degrees.</summary>
		[Field ("HMCharacteristicTypeHue")]
		Hue,

		/// <summary>A float that indicates the saturation of a color.</summary>
		[Field ("HMCharacteristicTypeSaturation")]
		Saturation,

		/// <summary>An integer brightness percentage.</summary>
		[Field ("HMCharacteristicTypeBrightness")]
		Brightness,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueTemperatureUnit" /> value that indicates the units in which the accessory reports temperature.</summary>
		[Field ("HMCharacteristicTypeTemperatureUnits")]
		TemperatureUnits,

		/// <summary>A float that indicates the current temperature in degrees Celsius.</summary>
		[Field ("HMCharacteristicTypeCurrentTemperature")]
		CurrentTemperature,

		/// <summary>A float that indicates the desired temperature in degrees Celsius.</summary>
		[Field ("HMCharacteristicTypeTargetTemperature")]
		TargetTemperature,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueHeatingCooling" /> value that indicates the heating or cooling state.</summary>
		[Field ("HMCharacteristicTypeCurrentHeatingCooling")]
		CurrentHeatingCooling,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueHeatingCooling" /> that represents the desired heating or cooling state.</summary>
		[Field ("HMCharacteristicTypeTargetHeatingCooling")]
		TargetHeatingCooling,

		/// <summary>A float that indicates the Celsius temperature above which cooling begins.</summary>
		[Field ("HMCharacteristicTypeCoolingThreshold")]
		CoolingThreshold,

		/// <summary>A float that indicates the Celsius temperature below which heating begins.</summary>
		[Field ("HMCharacteristicTypeHeatingThreshold")]
		HeatingThreshold,

		/// <summary>A float that indicates the current relative humidity percentage.</summary>
		[Field ("HMCharacteristicTypeCurrentRelativeHumidity")]
		CurrentRelativeHumidity,

		/// <summary>A float that indicates the desired percentage of relative humidity.</summary>
		[Field ("HMCharacteristicTypeTargetRelativeHumidity")]
		TargetRelativeHumidity,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueDoorState" /> value that indicates the door state.</summary>
		[Field ("HMCharacteristicTypeCurrentDoorState")]
		CurrentDoorState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueDoorState" /> that represents the desired door state.</summary>
		[Field ("HMCharacteristicTypeTargetDoorState")]
		TargetDoorState,

		/// <summary>A Boolean that indicates whether the accessory has detected an obstruction.</summary>
		[Field ("HMCharacteristicTypeObstructionDetected")]
		ObstructionDetected,

		/// <summary>A string that contains the name of the accessory.</summary>
		[Field ("HMCharacteristicTypeName")]
		Name,

		/// <summary>A string that specifies the accessory manufacturer.</summary>
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HMAccessory.Manufacturer' instead.")]
		[Field ("HMCharacteristicTypeManufacturer")]
		Manufacturer,

		/// <summary>A string that specifies the model of the accessory.</summary>
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.Model' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.Model' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HMAccessory.Model' instead.")]
		[Field ("HMCharacteristicTypeModel")]
		Model,

		/// <summary>A string that contains a serial number for an accessory.</summary>
		[Deprecated (PlatformName.TvOS, 11, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "No longer supported.")]
		[Field ("HMCharacteristicTypeSerialNumber")]
		SerialNumber,

		/// <summary>A write-only Boolean that is used to identify the device.</summary>
		[Field ("HMCharacteristicTypeIdentify")]
		Identify,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueRotationDirection" /> value that indicates the accessory's rotatoin.</summary>
		[Field ("HMCharacteristicTypeRotationDirection")]
		RotationDirection,

		/// <summary>A float that indicates the accessory's rotation speed.</summary>
		[Field ("HMCharacteristicTypeRotationSpeed")]
		RotationSpeed,

		/// <summary>A Boolean value that indicates whether an outlet is in use.</summary>
		[Field ("HMCharacteristicTypeOutletInUse")]
		OutletInUse,

		/// <summary>A string that contains the version of the accessory.</summary>
		[Field ("HMCharacteristicTypeVersion")]
		Version,

		/// <summary>TLV8 data, in an NSData object, that contains the log data for the accessory.</summary>
		[Field ("HMCharacteristicTypeLogs")]
		Logs,

		/// <summary>A Boolean that indicates whether a device provides audio feedback.</summary>
		[Field ("HMCharacteristicTypeAudioFeedback")]
		AudioFeedback,

		/// <summary>A Boolean that indicates whether admin access is required.</summary>
		[Field ("HMCharacteristicTypeAdminOnlyAccess")]
		AdminOnlyAccess,

		/// <summary>A Boolean that indicates whether the accessory has detected movement.</summary>
		[Field ("HMCharacteristicTypeMotionDetected")]
		MotionDetected,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueLockMechanismState" /> value that indicates the lock mechanism state.</summary>
		[Field ("HMCharacteristicTypeCurrentLockMechanismState")]
		CurrentLockMechanismState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueLockMechanismState" /> that represents the desired lock mechanism state.</summary>
		[Field ("HMCharacteristicTypeTargetLockMechanismState")]
		TargetLockMechanismState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueLockMechanism" /> value that specifies the last known action taken on the lock.</summary>
		[Field ("HMCharacteristicTypeLockMechanismLastKnownAction")]
		LockMechanismLastKnownAction,

		/// <summary>TLV8 data, in an NSData object, that specifies a control point for lock management.</summary>
		[Field ("HMCharacteristicTypeLockManagementControlPoint")]
		LockManagementControlPoint,

		/// <summary>An unsigned 32-bit integer that describes the time in seconds after which an autolockable device will lock.</summary>
		[Field ("HMCharacteristicTypeLockManagementAutoSecureTimeout")]
		LockManagementAutoSecureTimeout,

		/// <summary>A measure of air quality. </summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeAirParticulateDensity")]
		AirParticulateDensity,

		/// <summary>The size of the particles measured for air quality purposes. The value is an element in the <see cref="HomeKit.HMCharacteristicValueAirParticulate" /> enum.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeAirParticulateSize")]
		AirParticulateSize,

		/// <summary>Measure of air quality. The value is an element in the <see cref="HomeKit.HMCharacteristicValueAirQuality" /> enum.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeAirQuality")]
		AirQuality,

		/// <summary>A power level. The result is a <see langword="byte" /> representing the percentage of charge in the range [0..100]</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeBatteryLevel")]
		BatteryLevel,

		/// <summary>Indicates the presence of CO2. The result is a <see langword="byte" /> where 0 indicates normal CO2 levels.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonDioxideDetected")]
		CarbonDioxideDetected,

		/// <summary>The measured level of CO2. The result is a <see langword="float" /> indicating CO2 parts-per-million.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonDioxideLevel")]
		CarbonDioxideLevel,

		/// <summary>The highest recorded CO2 level. The resultis a <see langword="float" /> indicating CO2 parts-per-million.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonDioxidePeakLevel")]
		CarbonDioxidePeakLevel,

		/// <summary>Indicates the presence of CO. The result is a <see langword="byte" /> where 0 indicates normal CO levels.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonMonoxideDetected")]
		CarbonMonoxideDetected,

		/// <summary>The measured level of CO. The result is a <see langword="float" /> indicating CO parts-per-million.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonMonoxideLevel")]
		CarbonMonoxideLevel,

		/// <summary>The highest measured level of CO. The result is a <see langword="float" /> indicating CO parts-per-million.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCarbonMonoxidePeakLevel")]
		CarbonMonoxidePeakLevel,

		/// <summary>A value in <see cref="HomeKit.HMCharacteristicValueChargingState" />.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeChargingState")]
		ChargingState,

		/// <summary>A value in <see cref="HomeKit.HMCharacteristicValueContactState" />.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeContactState")]
		ContactState,

		/// <summary>The security system state. Will be a value in <see cref="HomeKit.HMCharacteristicValueCurrentSecuritySystemState" />.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentSecuritySystemState")]
		CurrentSecuritySystemState,

		/// <summary>A float measuring the tilt from horizontal in degrees.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentHorizontalTilt")]
		CurrentHorizontalTilt,

		/// <summary>The luminance, in lux.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentLightLevel")]
		CurrentLightLevel,

		/// <summary>A <see langword="byte" /> between 0 and 100, representing the percent a door or window is open.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentPosition")]
		CurrentPosition,

		/// <summary>The current tilt, in degrees.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentVerticalTilt")]
		CurrentVerticalTilt,

		/// <summary>Developers should not use this deprecated field. Developers should use 'HMAccessory.FirmwareVersion' instead.</summary>
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HMAccessory.FirmwareVersion' instead.")]
		[Field ("HMCharacteristicTypeFirmwareVersion")]
		FirmwareVersion,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeHardwareVersion")]
		HardwareVersion,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeHoldPosition")]
		HoldPosition,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeInputEvent")]
		InputEvent,

		/// <summary>A <see langword="byte" /> whose value will be either 0 (no leak detected) or 1 (leak detected).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLeakDetected")]
		LeakDetected,

		/// <summary>A <see langword="byte" /> whose value will either be 0 (no occupancy detected) or 1 (occupancy detected).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOccupancyDetected")]
		OccupancyDetected,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOutputState")]
		OutputState,

		/// <summary>The result will be an element in the <see cref="HomeKit.HMCharacteristicValuePositionState" /> enum.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypePositionState")]
		PositionState,

		/// <summary>A <see langword="byte" /> that will either be 0 (no smoke detected) or 1 (smoke detected).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSmokeDetected")]
		SmokeDetected,

		/// <summary>Returns the accessory's software version in a <see langword="string" />.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSoftwareVersion")]
		SoftwareVersion,

		/// <summary>A Boolean indicating whether a service is active.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusActive")]
		StatusActive,

		/// <summary>A <see langword="byte" /> whose value will either be 0 (no fault) or 1 (the system is in a fault state).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusFault")]
		StatusFault,

		/// <summary>A <see langword="byte" /> whose value will either be 0 (not jammed) or 1 (jammed).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusJammed")]
		StatusJammed,

		/// <summary>A <see langword="byte" /> whose value will either be 0 (battery level is not low) or 1 (battery is low).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusLowBattery")]
		StatusLowBattery,

		/// <summary>A <see langword="byte" /> whose value will either be 0 (no tampering detected) or 1 (tampering detected).</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStatusTampered")]
		StatusTampered,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetSecuritySystemState")]
		TargetSecuritySystemState,

		/// <summary>The desired horizontal tilt, in arc degrees.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetHorizontalTilt")]
		TargetHorizontalTilt,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetPosition")]
		TargetPosition,

		/// <summary>The desired vertical tilt, in arc degrees.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetVerticalTilt")]
		TargetVerticalTilt,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeStreamingStatus")]
		StreamingStatus,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSetupStreamEndpoint")]
		SetupStreamEndpoint,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSupportedVideoStreamConfiguration")]
		SupportedVideoStreamConfiguration,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSupportedAudioStreamConfiguration")]
		SupportedAudioStreamConfiguration,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSupportedRTPConfiguration")]
		SupportedRtpConfiguration,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSelectedStreamConfiguration")]
		SelectedStreamConfiguration,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeVolume")]
		Volume,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeMute")]
		Mute,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeNightVision")]
		NightVision,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOpticalZoom")]
		OpticalZoom,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeDigitalZoom")]
		DigitalZoom,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeImageRotation")]
		ImageRotation,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeImageMirroring")]
		ImageMirroring,

		/// <summary>A Boolean that tells whether a service is active.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeActive")]
		Active,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueCurrentAirPurifierState" /> value that indicates the fan state state.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentAirPurifierState")]
		CurrentAirPurifierState,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetAirPurifierState")]
		TargetAirPurifierState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueCurrentFanState" /> value that indicates the fan state state.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentFanState")]
		CurrentFanState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueCurrentHeaterCoolerState" /> value that indicates the fan state state.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentHeaterCoolerState")]
		CurrentHeaterCoolerState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueCurrentHumidifierDehumidifierState" /> value that indicates the fan state state.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentHumidifierDehumidifierState")]
		CurrentHumidifierDehumidifierState,

		/// <summary>A <see cref="HomeKit.HMCharacteristicValueCurrentSlatState" /> value that indicates the lock mechanism state.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentSlatState")]
		CurrentSlatState,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeWaterLevel")]
		WaterLevel,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeFilterChangeIndication")]
		FilterChangeIndication,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeFilterLifeLevel")]
		FilterLifeLevel,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeFilterResetChangeIndication")]
		FilterResetChangeIndication,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLockPhysicalControls")]
		LockPhysicalControls,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSwingMode")]
		SwingMode,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetHeaterCoolerState")]
		TargetHeaterCoolerState,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetHumidifierDehumidifierState")]
		TargetHumidifierDehumidifierState,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetFanState")]
		TargetFanState,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSlatType")]
		SlatType,

		/// <summary>The current tilt, in degrees.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeCurrentTilt")]
		CurrentTilt,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeTargetTilt")]
		TargetTilt,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeOzoneDensity")]
		OzoneDensity,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeNitrogenDioxideDensity")]
		NitrogenDioxideDensity,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSulphurDioxideDensity")]
		SulphurDioxideDensity,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypePM2_5Density")]
		PM2_5Density,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypePM10Density")]
		PM10Density,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeVolatileOrganicCompoundDensity")]
		VolatileOrganicCompoundDensity,

		/// <summary>The threshold relative humidity at which the dehumidifier starts.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeDehumidifierThreshold")]
		DehumidifierThreshold,

		/// <summary>The relative humidity threshold when the humidifier starts.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeHumidifierThreshold")]
		HumidifierThreshold,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSecuritySystemAlarmType")]
		SecuritySystemAlarmType,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLabelNamespace")]
		LabelNamespace,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeLabelIndex")]
		LabelIndex,

		/// <summary>The color temperature of a light.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeColorTemperature")]
		ColorTemperature,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeProgramMode")]
		ProgramMode,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeInUse")]
		InUse,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeSetDuration")]
		SetDuration,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeRemainingDuration")]
		RemainingDuration,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMCharacteristicTypeValveType")]
		ValveType,

		/// <summary>To be added.</summary>
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
	/// <summary>Enumerates units of measure that might be associated with a <see cref="HomeKit.HMCharacteristic" />.</summary>
	[MacCatalyst (13, 1)]
	public enum HMCharacteristicMetadataUnits {
		/// <summary>Indicates no unit, or a dimensionless unit.</summary>
		None,
		/// <summary>Indicates a unit that measures temperature in degrees Celsius.</summary>
		Celsius,
		/// <summary>Indicates a unit that measures temperature in degrees Fahrenheit.</summary>
		Fahrenheit,
		/// <summary>Indicates a unit that measures percentage.</summary>
		Percentage,
		/// <summary>Indicates a unit that measures degrees of arc.</summary>
		ArcDegree,
		/// <summary>The SI unit of time.</summary>
		[MacCatalyst (13, 1)]
		Seconds,
		/// <summary>The SI unit of luminance.</summary>
		[MacCatalyst (13, 1)]
		Lux,
		/// <summary>A unit often used for measuring concentrations in liquids or gas.</summary>
		[MacCatalyst (13, 1)]
		PartsPerMillion,
		/// <summary>A unit often used for measuring air quality.</summary>
		[MacCatalyst (13, 1)]
		MicrogramsPerCubicMeter,
	}

	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates known types of <see cref="HomeKit.HMService" />.</summary>
	[MacCatalyst (14, 0)]
	[Flags]
	public enum HMServiceType {
		/// <summary>Indicates the absence of a service.</summary>
		None,

		/// <summary>The service represents a light bulb.</summary>
		[Field ("HMServiceTypeLightbulb")]
		LightBulb,

		/// <summary>The service represents a switch.</summary>
		[Field ("HMServiceTypeSwitch")]
		Switch,

		/// <summary>The service represents a thermostat.</summary>
		[Field ("HMServiceTypeThermostat")]
		Thermostat,

		/// <summary>The service represents a garage door opener.</summary>
		[Field ("HMServiceTypeGarageDoorOpener")]
		GarageDoorOpener,

		/// <summary>Indicates accessory information.</summary>
		[Field ("HMServiceTypeAccessoryInformation")]
		AccessoryInformation,

		/// <summary>The service represents a fan.</summary>
		[Field ("HMServiceTypeFan")]
		Fan,

		/// <summary>The service represents an outlet.</summary>
		[Field ("HMServiceTypeOutlet")]
		Outlet,

		/// <summary>The service represents a lock mechanism.</summary>
		[Field ("HMServiceTypeLockMechanism")]
		LockMechanism,

		/// <summary>The service manages locks.</summary>
		[Field ("HMServiceTypeLockManagement")]
		LockManagement,

		/// <summary>A sensor that monitors air quality.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeAirQualitySensor")]
		AirQualitySensor,

		/// <summary>An energy storage device.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeBattery")]
		Battery,

		/// <summary>A sensor that detects CO2.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCarbonDioxideSensor")]
		CarbonDioxideSensor,

		/// <summary>A sensor that detects CO.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCarbonMonoxideSensor")]
		CarbonMonoxideSensor,

		/// <summary>A sensor that detects physical contact.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeContactSensor")]
		ContactSensor,

		/// <summary>A door.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeDoor")]
		Door,

		/// <summary>A sensor that monitors the water content of the air.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeHumiditySensor")]
		HumiditySensor,

		/// <summary>A sensor for detecting seepage.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeLeakSensor")]
		LeakSensor,

		/// <summary>A sensor that monitors luminance.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeLightSensor")]
		LightSensor,

		/// <summary>A device that senses movement.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeMotionSensor")]
		MotionSensor,

		/// <summary>A device that detects occupancy using an unspecified technology.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeOccupancySensor")]
		OccupancySensor,

		/// <summary>A system that can be armed and will trigger alerts.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSecuritySystem")]
		SecuritySystem,

		/// <summary>A switch that maintains internal state and rules.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeStatefulProgrammableSwitch")]
		StatefulProgrammableSwitch,

		/// <summary>A switch that does not maintain internal state.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeStatelessProgrammableSwitch")]
		StatelessProgrammableSwitch,

		/// <summary>A detector used to monitor smoke or fire.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSmokeSensor")]
		SmokeSensor,

		/// <summary>A thermometer.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeTemperatureSensor")]
		TemperatureSensor,

		/// <summary>A pane of glass.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeWindow")]
		Window,

		/// <summary>Drapes or shades.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeWindowCovering")]
		WindowCovering,

		/// <summary>A camera management interface.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCameraRTPStreamManagement")]
		CameraRtpStreamManagement,

		/// <summary>A video camera.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeCameraControl")]
		CameraControl,

		/// <summary>An audio sensor.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeMicrophone")]
		Microphone,

		/// <summary>A speaker.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSpeaker")]
		Speaker,

		/// <summary>A doorbell.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeDoorbell")]
		Doorbell,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeAirPurifier")]
		AirPurifier,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeVentilationFan")]
		VentilationFan,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeFilterMaintenance")]
		FilterMaintenance,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeHeaterCooler")]
		HeaterCooler,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeHumidifierDehumidifier")]
		HumidifierDehumidifier,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeSlats")]
		Slats,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeLabel")]
		Label,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeIrrigationSystem")]
		IrrigationSystem,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMServiceTypeValve")]
		Valve,

		/// <summary>To be added.</summary>
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
		/// <summary>The data format is either unspecified or unknown.</summary>
		None,
		/// <summary>The data are represented by Boolean values.</summary>
		Bool,
		/// <summary>The data are represented by int values.</summary>
		Int,
		/// <summary>The data are represented by float values.</summary>
		Float,
		/// <summary>The data are represented by string values.</summary>
		String,
		/// <summary>The data are represented by arrays.</summary>
		Array,
		/// <summary>The data are represented by dictionary values.</summary>
		Dictionary,
		/// <summary>The data are represented by UInt8 values.</summary>
		UInt8,
		/// <summary>The data are represented by UInt16 values.</summary>
		UInt16,
		/// <summary>The data are represented by UInt32 values.</summary>
		UInt32,
		/// <summary>The data are represented by UInt64 values.</summary>
		UInt64,
		/// <summary>The data are represented by binary blobs.</summary>
		Data,
		/// <summary>The data are represented by Tlv8 values, which are packed 8-bit type and length values, followed by the number of bytes that are specified in the packed length field.</summary>
		Tlv8,
	}

	/// <summary>Enumerates the states that a door can be in.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueDoorState : long {
		/// <summary>The door is open.</summary>
		Open = 0,
		/// <summary>The door is closed.</summary>
		Closed,
		/// <summary>The door is moving from closed to open.</summary>
		Opening,
		/// <summary>The door is moving from opened to closed.</summary>
		Closing,
		/// <summary>The door is stopped somewhere between opened and closed.</summary>
		Stopped,
	}

	/// <summary>Enumerates the states that an indoor climate control unit can be in.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueHeatingCooling : long {
		/// <summary>The unit is off.</summary>
		Off = 0,
		/// <summary>The unit will heat.</summary>
		Heat,
		/// <summary>The unit will cool.</summary>
		Cool,
		/// <summary>The unit will heat or cool as necessary.</summary>
		Auto,
	}

	/// <summary>Enumerates rotation directions.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueRotationDirection : long {
		/// <summary>The object is rotating in a clockwise direction within its coordinate system.</summary>
		Clockwise = 0,
		/// <summary>The object is rotating in a counterclockwise direction within its coordinate system.</summary>
		CounterClockwise,
	}

	/// <summary>Enumerates two common temperature measurement systems.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTemperatureUnit : long {
		/// <summary>Celsius.</summary>
		Celsius = 0,
		/// <summary>Fahrenheit.</summary>
		Fahrenheit,
	}

	/// <summary>Enumerates the states that a lock can be in.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLockMechanismState : long {
		/// <summary>The lock is unlocked.</summary>
		Unsecured = 0,
		/// <summary>The lock is locked.</summary>
		Secured,
		/// <summary>The lock is jammed.</summary>
		Jammed,
		/// <summary>The lock's state is unknown.</summary>
		Unknown,
	}

	/// <summary>Enumerates values that describe the last known action that was taken on a lock.</summary>
	[MacCatalyst (14, 0)]
	[Native ("HMCharacteristicValueLockMechanismLastKnownAction")]
	// in iOS 8.3 this was renamed HMCharacteristicValueLockMechanismLastKnownAction but that would be a breaking change for us
	public enum HMCharacteristicValueLockMechanism : long {
		/// <summary>According to the most recent recorded activity, the lock was locked by physical manipulation from the inside.</summary>
		LastKnownActionSecuredUsingPhysicalMovementInterior = 0,
		/// <summary>According to the most recent recorded activity, the lock was unlocked by physical manipulation from the inside.</summary>
		LastKnownActionUnsecuredUsingPhysicalMovementInterior,
		/// <summary>According to the most recent recorded activity, the lock was locked by physical manipulation from the outside.</summary>
		LastKnownActionSecuredUsingPhysicalMovementExterior,
		/// <summary>According to the most recent recorded activity, the lock was unlocked by physical manipulation from the outside.</summary>
		LastKnownActionUnsecuredUsingPhysicalMovementExterior,
		/// <summary>According to the most recent recorded activity, the lock was locked with a keypad.</summary>
		LastKnownActionSecuredWithKeypad,
		/// <summary>According to the most recent recorded activity, the lock was unlocked with a keypad.</summary>
		LastKnownActionUnsecuredWithKeypad,
		/// <summary>According to the most recent recorded activity, the lock was locked remotely.</summary>
		LastKnownActionSecuredRemotely,
		/// <summary>According to the most recent recorded activity, the lock was unlocked remotely.</summary>
		LastKnownActionUnsecuredRemotely,
		/// <summary>According to the most recent recorded activity, the lock was locked when a timeout expired.</summary>
		LastKnownActionSecuredWithAutomaticSecureTimeout,
		/// <summary>The lock was physically locked.</summary>
		LastKnownActionSecuredUsingPhysicalMovement,
		/// <summary>The lock was physically unlocked.</summary>
		LastKnownActionUnsecuredUsingPhysicalMovement,
	}

	/// <summary>Enumerates the particulate sizes supported by HomeKit.</summary>
	[MacCatalyst (14, 0)]
	[Native ("HMCharacteristicValueAirParticulateSize")]
	public enum HMCharacteristicValueAirParticulate : long {
		/// <summary>Particles of size 2.5 micrometers (2.5 μm).</summary>
		Size2_5 = 0,
		/// <summary>Particles of size 10 micrometers (10 μm).</summary>
		Size10,
	}

	/// <summary>Enumerates the states of a security system.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentSecuritySystemState : long {
		/// <summary>The system is alarmed appropriately for a user who is home and active.</summary>
		StayArm = 0,
		/// <summary>The system is armed and no one is home.</summary>
		AwayArm,
		/// <summary>The system is armed appropriately for when a user is asleep at home.</summary>
		NightArm,
		/// <summary>The system is disarmed.</summary>
		Disarmed,
		/// <summary>The system has been triggered.</summary>
		Triggered,
	}

	/// <summary>Enumerates the states of an object that opens and closes.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValuePositionState : long {
		/// <summary>The mechanism is in the act of closing.</summary>
		Closing = 0,
		/// <summary>The mechanism is in the act of opening.</summary>
		Opening,
		/// <summary>The mechanism has ceased motion.</summary>
		Stopped,
	}

	/// <summary>Enumerates the status of a security system.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetSecuritySystemState : long {
		/// <summary>The system is alarmed in the manner appropriate to the users being home and awake.</summary>
		StayArm = 0,
		/// <summary>The system is armed appropriately for no one being home.</summary>
		AwayArm,
		/// <summary>The system is armed in the manner appropriate to the users sleeping.</summary>
		NightArm,
		/// <summary>The system is disarmed.</summary>
		Disarm,
	}

	/// <summary>Enumerates the valid battery status values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueBatteryStatus : long {
		/// <summary>The charge level is higher than <see cref="HomeKit.HMCharacteristicValueBatteryStatus.Low" />.</summary>
		Normal = 0,
		/// <summary>The charge level is low.</summary>
		Low,
	}

	/// <summary>Enumerates the state of a jammed device.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueJammedStatus : long {
		/// <summary>The device is not jammed.</summary>
		None = 0,
		/// <summary>The device is jammed.</summary>
		Jammed,
	}

	/// <summary>Enumerates the states of a device that detected tampering.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTamperedStatus : long {
		/// <summary>Tampering was not detected.</summary>
		None = 0,
		/// <summary>Tampering has been detected.</summary>
		Tampered,
	}

	/// <summary>Enumerates the values of a device that detects leaks.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLeakStatus : long {
		/// <summary>A leak is not detected.</summary>
		None = 0,
		/// <summary>A leak is detected.</summary>
		Detected,
	}

	/// <summary>Enumerates the state of a smoke detector.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSmokeDetectionStatus : long {
		/// <summary>Smoke is not detected.</summary>
		None = 0,
		/// <summary>Smoke is detected.</summary>
		Detected,
	}

	/// <summary>Enumerates the state of battery charging.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueChargingState : long {
		/// <summary>The battery is not charging.</summary>
		None = 0,
		/// <summary>The battery is charging.</summary>
		InProgress,
		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		NotChargeable,
	}

	/// <summary>Enumerates whether contact is detected.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueContactState : long {
		/// <summary>Contact is detected.</summary>
		Detected = 0,
		/// <summary>Contact is not detected.</summary>
		None,
	}

	/// <summary>Enumerates fault states.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueStatusFault : long {
		/// <summary>There is no fault.</summary>
		NoFault = 0,
		/// <summary>A generic fault state.</summary>
		GeneralFault,
	}

	/// <summary>Enumerates detection values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCarbonMonoxideDetectionStatus : long {
		/// <summary>Carbon monoxide was not detected.</summary>
		NotDetected = 0,
		/// <summary>Carbon monoxide was detected.</summary>
		Detected,
	}

	/// <summary>Enumerates detection values.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCarbonDioxideDetectionStatus : long {
		/// <summary>Carbon dioxide was not detected.</summary>
		NotDetected = 0,
		/// <summary>Carbon dioxide was detected.</summary>
		Detected,
	}

	/// <summary>Enumerates values for occupancy.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueOccupancyStatus : long {
		/// <summary>The area is not occupied.</summary>
		NotOccupied = 0,
		/// <summary>The area is occupied.</summary>
		Occupied,
	}

	/// <summary>Enumerates the security system alarm type.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSecuritySystemAlarmType : long {
		/// <summary>Alarm conditions have been cleared.</summary>
		NoAlarm = 0,
		/// <summary>The alarm type is not known.</summary>
		Unknown,
	}

	// conveniance enum (ObjC uses NSString)
	/// <summary>Enumerates the different kinds of <see cref="HomeKit.HMActionSet" />.</summary>
	[MacCatalyst (13, 1)]
	public enum HMActionSetType {
		/// <summary>The <see cref="HomeKit.HMActionSet" /> is not associated with a known kind.</summary>
		Unknown = -1,
		/// <summary>The <see cref="HomeKit.HMActionSet" /> relates to the user waking.</summary>
		WakeUp,
		/// <summary>The <see cref="HomeKit.HMActionSet" /> relates to the user indicating they are going to sleep.</summary>
		Sleep,
		/// <summary>The <see cref="HomeKit.HMActionSet" /> relates to the user leaving the home location.</summary>
		HomeDeparture,
		/// <summary>The <see cref="HomeKit.HMActionSet" /> relates to the user arriving at the home location.</summary>
		HomeArrival,
		/// <summary>The <see cref="HomeKit.HMActionSet" /> was manually created by the user.</summary>
		UserDefined,
		/// <summary>The <see cref="HomeKit.HMActionSet" /> relates to a user-defined trigger.</summary>
		[MacCatalyst (13, 1)]
		TriggerOwned,
	}

	/// <summary>Enumerates the kind of accessories supported by HomeKit.</summary>
	[MacCatalyst (14, 0)]
	// conveniance enum (ObjC uses NSString)
	public enum HMAccessoryCategoryType {
		/// <summary>An accessory that does not fit into any of the enumerated <see cref="HomeKit.HMAccessoryCategoryType" /> categories.</summary>
		[Field ("HMAccessoryCategoryTypeOther")]
		Other = 0,

		/// <summary>An accessory that monitors, and may have internal rules for controlling, security sensors and cameras.</summary>
		[Field ("HMAccessoryCategoryTypeSecuritySystem")]
		SecuritySystem,

		/// <summary>Connects multiple accessories.</summary>
		[Field ("HMAccessoryCategoryTypeBridge")]
		Bridge,

		/// <summary>Represents a physical door that may be opened or closed.</summary>
		[Field ("HMAccessoryCategoryTypeDoor")]
		Door,

		/// <summary>An accessory that can lock or unlock a door.</summary>
		[Field ("HMAccessoryCategoryTypeDoorLock")]
		DoorLock,

		/// <summary>A motorized air-mover.</summary>
		[Field ("HMAccessoryCategoryTypeFan")]
		Fan,

		/// <summary>An accessory that controls a garage door.</summary>
		[Field ("HMAccessoryCategoryTypeGarageDoorOpener")]
		GarageDoorOpener,

#if !TVOS
		/// <summary>An accessory capable of opening and closing a door.</summary>
		[Obsolete ("Use 'GarageDoorOpener' instead.")]
		[NoTV]
		[MacCatalyst (13, 1)]
		DoorOpener = GarageDoorOpener,
#endif

		/// <summary>Represents a physical lightbulb.</summary>
		[Field ("HMAccessoryCategoryTypeLightbulb")]
		Lightbulb,

		/// <summary>An electrical power outlet.</summary>
		[Field ("HMAccessoryCategoryTypeOutlet")]
		Outlet,

		/// <summary>A switch accessory that may maintain a locally-stored set of rules. (See also <see cref="HomeKit.HMAccessoryCategoryType.Switch" />.)</summary>
		[Field ("HMAccessoryCategoryTypeProgrammableSwitch")]
		ProgrammableSwitch,

		/// <summary>Represents a physical sensor.</summary>
		[Field ("HMAccessoryCategoryTypeSensor")]
		Sensor,

		/// <summary>A switch. (See also <see cref="HomeKit.HMAccessoryCategoryType.ProgrammableSwitch" />.)</summary>
		[Field ("HMAccessoryCategoryTypeSwitch")]
		Switch,

		/// <summary>An accessory that controls heating and cooling systems.</summary>
		[Field ("HMAccessoryCategoryTypeThermostat")]
		Thermostat,

		/// <summary>Represents a physical window.</summary>
		[Field ("HMAccessoryCategoryTypeWindow")]
		Window,

		/// <summary>Represents shades or drapes.</summary>
		[Field ("HMAccessoryCategoryTypeWindowCovering")]
		WindowCovering,

		/// <summary>An accessory that extends network communication.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeRangeExtender")]
		RangeExtender,

		/// <summary>A video camera that can stream its data over IP networks.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeIPCamera")]
		IPCamera,

		/// <summary>Represents a video doorbell.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeVideoDoorbell")]
		VideoDoorbell,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirPurifier")]
		AirPurifier,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirHeater")]
		AirHeater,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirConditioner")]
		AirConditioner,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirHumidifier")]
		AirHumidifier,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeAirDehumidifier")]
		AirDehumidifier,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeSprinkler")]
		Sprinkler,

		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		[Field ("HMAccessoryCategoryTypeFaucet")]
		Faucet,

		/// <summary>To be added.</summary>
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

		/// <summary>A rough estimate of the time of appearance above the horizon of the upper limb of the nearest star.</summary>
		[Field ("HMSignificantEventSunrise")]
		Sunrise,

		/// <summary>A rough estimate of the time of disappearance below the horizon of the upper limb of the nearest star.</summary>
		[Field ("HMSignificantEventSunset")]
		Sunset,
	}

	[NoMac, MacCatalyst (14, 0)]
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
		/// <summary>An indeterminate quality.</summary>
		Unknown = 0,
		/// <summary>The best air quality.</summary>
		Excellent,
		/// <summary>Better than <see cref="HomeKit.HMCharacteristicValueAirQuality.Fair" />, but poorer than <see cref="HomeKit.HMCharacteristicValueAirQuality.Excellent" />.</summary>
		Good,
		/// <summary>The middle value of air quality.</summary>
		Fair,
		/// <summary>Better than <see cref="HomeKit.HMCharacteristicValueAirQuality.Poor" />, but poorer than <see cref="HomeKit.HMCharacteristicValueAirQuality.Fair" />.</summary>
		Inferior,
		/// <summary>The worst air quality.</summary>
		Poor,
	}

	/// <summary>Enumerates the states of an <see cref="HomeKit.HMCameraStream" />.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCameraStreamState : ulong {
		/// <summary>The camera is attempting to start streaming.</summary>
		Starting = 1,
		/// <summary>The camera is streaming video.</summary>
		Streaming = 2,
		/// <summary>The camera is shutting down the video stream.</summary>
		Stopping = 3,
		/// <summary>The camera is not streaming video.</summary>
		NotStreaming = 4,
	}

	/// <summary>Settings for camera's with audio devices. Used with <see cref="HomeKit.HMCameraStream.AudioStreamSetting" />.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCameraAudioStreamSetting : ulong {
		/// <summary>The <see cref="HomeKit.HMCameraAudioStreamSetting" /> describing whether the audio capabilities are muted.</summary>
		Muted = 1,
		/// <summary>The <see cref="HomeKit.HMCameraAudioStreamSetting" /> describing whether camera microphone one-way audio is enabled.</summary>
		IncomingAudioAllowed = 2,
		/// <summary>The <see cref="HomeKit.HMCameraAudioStreamSetting" /> describing whether two-way audio is enabled.</summary>
		BidirectionalAudioAllowed = 3,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that may either be locked or unlocked.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLockPhysicalControlsState : long {
		/// <summary>To be added.</summary>
		NotLocked = 0,
		/// <summary>To be added.</summary>
		Locked,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the states of an air purifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentAirPurifierState : long {
		/// <summary>To be added.</summary>
		Inactive = 0,
		/// <summary>To be added.</summary>
		Idle,
		/// <summary>To be added.</summary>
		Active,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> of an air purifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetAirPurifierState : long {
		/// <summary>To be added.</summary>
		Manual = 0,
		/// <summary>To be added.</summary>
		Automatic,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the states of an oscillating slat.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentSlatState : long {
		/// <summary>To be added.</summary>
		Stationary = 0,
		/// <summary>To be added.</summary>
		Jammed,
		/// <summary>To be added.</summary>
		Oscillating,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describe a vertical or horizontal slat.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSlatType : long {
		/// <summary>To be added.</summary>
		Horizontal = 0,
		/// <summary>To be added.</summary>
		Vertical,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the states of a filter that must be periodically changed.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueFilterChange : long {
		/// <summary>To be added.</summary>
		NotNeeded = 0,
		/// <summary>To be added.</summary>
		Needed,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the states of a fan.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentFanState : long {
		/// <summary>To be added.</summary>
		Inactive = 0,
		/// <summary>To be added.</summary>
		Idle,
		/// <summary>To be added.</summary>
		Active,
	}

	/// <summary>Enumerates the possible states of a <see cref="HomeKit.HMCharacteristic" /> of a fan.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetFanState : long {
		/// <summary>To be added.</summary>
		Manual = 0,
		/// <summary>To be added.</summary>
		Automatic,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the states of a heat exchanger.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentHeaterCoolerState : long {
		/// <summary>To be added.</summary>
		Inactive = 0,
		/// <summary>To be added.</summary>
		Idle,
		/// <summary>To be added.</summary>
		Heating,
		/// <summary>To be added.</summary>
		Cooling,
	}

	/// <summary>Enumerates the possible states of a <see cref="HomeKit.HMCharacteristic" /> of a heater / cooler.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetHeaterCoolerState : long {
		/// <summary>To be added.</summary>
		Automatic = 0,
		/// <summary>To be added.</summary>
		Heat,
		/// <summary>To be added.</summary>
		Cool,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the states of a humidifier or dehumidifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentHumidifierDehumidifierState : long {
		/// <summary>To be added.</summary>
		Inactive = 0,
		/// <summary>To be added.</summary>
		Idle,
		/// <summary>To be added.</summary>
		Humidifying,
		/// <summary>To be added.</summary>
		Dehumidifying,
	}

	/// <summary>Enumerates the possible states of a <see cref="HomeKit.HMCharacteristic" /> of a humidifier or dehumidifier.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueTargetHumidifierDehumidifierState : long {
		/// <summary>To be added.</summary>
		Automatic = 0,
		/// <summary>To be added.</summary>
		Humidify,
		/// <summary>To be added.</summary>
		Dehumidify,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describe a swing that can be enabled or disabled.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueSwingMode : long {
		/// <summary>To be added.</summary>
		Disabled = 0,
		/// <summary>To be added.</summary>
		Enabled,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that can be active or inactive.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueActivationState : long {
		/// <summary>To be added.</summary>
		Inactive = 0,
		/// <summary>To be added.</summary>
		Active,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that describes the forms of a button press.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueInputEvent : long {
		/// <summary>To be added.</summary>
		SinglePress = 0,
		/// <summary>To be added.</summary>
		DoublePress,
		/// <summary>To be added.</summary>
		LongPress,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that may either be a dot or a numeral.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueLabelNamespace : long {
		/// <summary>To be added.</summary>
		Dot = 0,
		/// <summary>To be added.</summary>
		Numeral,
	}

	/// <summary>Enumerates the possible states of a <see cref="HomeKit.HMEventTrigger" /> including reasons why it might not be enabled.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMEventTriggerActivationState : ulong {
		/// <summary>To be added.</summary>
		Disabled = 0,
		/// <summary>To be added.</summary>
		DisabledNoHomeHub = 1,
		/// <summary>To be added.</summary>
		DisabledNoCompatibleHomeHub = 2,
		/// <summary>To be added.</summary>
		DisabledNoLocationServicesAuthorization = 3,
		/// <summary>To be added.</summary>
		Enabled = 4,
	}

	/// <summary>Enumerates the possible states of the HomeKit hub.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMHomeHubState : ulong {
		/// <summary>To be added.</summary>
		NotAvailable = 0,
		/// <summary>To be added.</summary>
		Connected,
		/// <summary>To be added.</summary>
		Disconnected,
	}

	/// <summary>Enumerates the type of change in presence that the <see cref="HomeKit.HMPresenceEvent" /> represents.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMPresenceEventType : ulong {
		/// <summary>To be added.</summary>
		EveryEntry = 1,
		/// <summary>To be added.</summary>
		EveryExit = 2,
		/// <summary>To be added.</summary>
		FirstEntry = 3,
		/// <summary>To be added.</summary>
		LastExit = 4,
		/// <summary>To be added.</summary>
		AtHome = FirstEntry,
		/// <summary>To be added.</summary>
		NotAtHome = LastExit,
	}

	/// <summary>Enumerates the types of users whose presence or absence is associated with a <see cref="HomeKit.HMPresenceEvent" />.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMPresenceEventUserType : ulong {
		/// <summary>To be added.</summary>
		CurrentUser = 1,
		/// <summary>To be added.</summary>
		HomeUsers = 2,
		/// <summary>To be added.</summary>
		CustomUsers = 3,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that can be scheduled or manually overriden.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueProgramMode : long {
		/// <summary>To be added.</summary>
		NotScheduled = 0,
		/// <summary>To be added.</summary>
		Scheduled,
		/// <summary>To be added.</summary>
		ScheduleOverriddenToManual,
	}

	/// <summary>Enumerates the possible states of a <see cref="HomeKit.HMCharacteristic" /> of a device that can be in use or out of use.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueUsageState : long {
		/// <summary>To be added.</summary>
		NotInUse = 0,
		/// <summary>To be added.</summary>
		InUse,
	}

	/// <summary>Enumerates the known types of valve.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueValveType : long {
		/// <summary>To be added.</summary>
		GenericValve = 0,
		/// <summary>To be added.</summary>
		Irrigation,
		/// <summary>To be added.</summary>
		ShowerHead,
		/// <summary>To be added.</summary>
		WaterFaucet,
	}

	/// <summary>Enumerates the possible values of a <see cref="HomeKit.HMCharacteristic" /> that can be configured or not.</summary>
	[MacCatalyst (14, 0)]
	[Native]
	public enum HMCharacteristicValueConfigurationState : long {
		/// <summary>To be added.</summary>
		NotConfigured = 0,
		/// <summary>To be added.</summary>
		Configured,
	}

	/// <summary>Indicates the requested state of a television's door.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[Native]
	public enum HMCharacteristicValueTargetDoorState : long {
		/// <summary>The door is open.</summary>
		Open = 0,
		/// <summary>The door is closed.</summary>
		Closed = 1,
	}

	/// <summary>Indicates the current heating or cooling state.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[Native]
	public enum HMCharacteristicValueCurrentHeatingCooling : long {
		/// <summary>Neither heating nor cooling is active.</summary>
		Off = 0,
		/// <summary>Heating is active.</summary>
		Heat = 1,
		/// <summary>Cooling is active.</summary>
		Cool = 2,
	}

	/// <summary>Indicates the requested state of a lock mechanism.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[Native]
	public enum HMCharacteristicValueTargetLockMechanismState : long {
		/// <summary>The lock mechanism should be unsecured.</summary>
		Unsecured = 0,
		/// <summary>The lock mechanism should be secured.</summary>
		Secured = 1,
	}

	/// <summary>Identifies the category of a television input source.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueInputSourceType : long {
		/// <summary>An input source of another type.</summary>
		Other = 0,
		/// <summary>The television's home screen.</summary>
		HomeScreen = 1,
		/// <summary>A television tuner input.</summary>
		Tuner = 2,
		/// <summary>An HDMI input.</summary>
		Hdmi = 3,
		/// <summary>A composite-video input.</summary>
		CompositeVideo = 4,
		/// <summary>An S-Video input.</summary>
		SVideo = 5,
		/// <summary>A component-video input.</summary>
		ComponentVideo = 6,
		/// <summary>A DVI input.</summary>
		Dvi = 7,
		/// <summary>An AirPlay input.</summary>
		AirPlay = 8,
		/// <summary>A USB input.</summary>
		Usb = 9,
		/// <summary>An application input.</summary>
		Application = 10,
	}

	/// <summary>Identifies the category of a television input device.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueInputDeviceType : long {
		/// <summary>An input device of another type.</summary>
		Other = 0,
		/// <summary>A television.</summary>
		TV = 1,
		/// <summary>A recording device.</summary>
		Recording = 2,
		/// <summary>A tuner.</summary>
		Tuner = 3,
		/// <summary>A playback device.</summary>
		Playback = 4,
		/// <summary>An audio system.</summary>
		AudioSystem = 5,
		/// <summary>No input device.</summary>
		None = 6,
	}

	/// <summary>Indicates whether closed captions are enabled.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueClosedCaptions : long {
		/// <summary>Closed captions are disabled.</summary>
		Disabled = 0,
		/// <summary>Closed captions are enabled.</summary>
		Enabled = 1,
	}

	/// <summary>Indicates whether the power mode selection is shown.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValuePowerModeSelection : long {
		/// <summary>Show the power mode selection.</summary>
		Show = 0,
		/// <summary>Hide the power mode selection.</summary>
		Hide = 1,
	}

	/// <summary>Describes the current playback state of a television.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentMediaState : long {
		/// <summary>Media is playing.</summary>
		Playing = 0,
		/// <summary>Media playback is paused.</summary>
		Paused = 1,
		/// <summary>Media playback is stopped.</summary>
		Stopped = 2,
		/// <summary>The media state is unknown.</summary>
		Unknown = 3,
		/// <summary>Media is loading.</summary>
		Loading = 4,
		/// <summary>Media playback is interrupted.</summary>
		Interrupted = 5,
	}

	/// <summary>Identifies a remote-control command for a television.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueRemoteKey : long {
		/// <summary>Rewind playback.</summary>
		Rewind = 0,
		/// <summary>Fast-forward playback.</summary>
		FastForward = 1,
		/// <summary>Skip to the next track.</summary>
		NextTrack = 2,
		/// <summary>Skip to the previous track.</summary>
		PreviousTrack = 3,
		/// <summary>Move the selection up.</summary>
		ArrowUp = 4,
		/// <summary>Move the selection down.</summary>
		ArrowDown = 5,
		/// <summary>Move the selection left.</summary>
		ArrowLeft = 6,
		/// <summary>Move the selection right.</summary>
		ArrowRight = 7,
		/// <summary>Select the current item.</summary>
		Select = 8,
		/// <summary>Navigate back.</summary>
		Back = 9,
		/// <summary>Exit the current view.</summary>
		Exit = 10,
		/// <summary>Toggle between playing and pausing.</summary>
		PlayPause = 11,
		/// <summary>Start or resume playback.</summary>
		Play = 12,
		/// <summary>Pause playback.</summary>
		Pause = 13,
		/// <summary>Open the menu.</summary>
		Menu = 14,
		/// <summary>Show information about the current item.</summary>
		Info = 15,
		/// <summary>Navigate to the home screen.</summary>
		Home = 16,
	}

	/// <summary>Identifies a picture mode preset for a television.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValuePictureMode : long {
		/// <summary>The standard picture mode.</summary>
		Standard = 0,
		/// <summary>The movie picture mode.</summary>
		Movie = 1,
		/// <summary>The sports picture mode.</summary>
		Sport = 2,
		/// <summary>The game picture mode.</summary>
		Game = 3,
		/// <summary>The photo picture mode.</summary>
		Photo = 4,
		/// <summary>The vivid picture mode.</summary>
		Vivid = 5,
		/// <summary>The dark picture mode.</summary>
		Dark = 6,
		/// <summary>The bright picture mode.</summary>
		Bright = 7,
		/// <summary>The computer picture mode.</summary>
		Computer = 8,
		/// <summary>The night picture mode.</summary>
		Night = 9,
		/// <summary>The calibrated picture mode.</summary>
		Calibrated = 10,
		/// <summary>The first custom picture mode.</summary>
		Custom1 = 11,
		/// <summary>The second custom picture mode.</summary>
		Custom2 = 12,
		/// <summary>The third custom picture mode.</summary>
		Custom3 = 13,
	}

	/// <summary>Indicates whether a television input source should be shown.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueTargetVisibilityState : long {
		/// <summary>Show the input source.</summary>
		Show = 0,
		/// <summary>Hide the input source.</summary>
		Hide = 1,
	}

	/// <summary>Describes the current visibility state of a television input source.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueCurrentVisibilityState : long {
		/// <summary>The input source is shown.</summary>
		Shown = 0,
		/// <summary>The input source is hidden.</summary>
		Hidden = 1,
		/// <summary>The input source is shown when its device is connected.</summary>
		Connected = 2,
		/// <summary>The input source is always shown.</summary>
		AlwaysShown = 3,
	}

	/// <summary>Describes whether a router is ready to provide network service.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueRouterStatus : long {
		/// <summary>The router is ready.</summary>
		Ready = 0,
		/// <summary>The router is not ready.</summary>
		NotReady = 1,
	}

	/// <summary>Describes the connection state of a Wi-Fi satellite.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueWiFiSatelliteStatus : long {
		/// <summary>The satellite connection state is unknown.</summary>
		Unknown = 0,
		/// <summary>The satellite is connected.</summary>
		Connected = 1,
		/// <summary>The satellite is not connected.</summary>
		NotConnected = 2,
	}

	/// <summary>Specifies how a television's volume control operates.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueVolumeControlType : long {
		/// <summary>No volume control is available.</summary>
		None = 0,
		/// <summary>Volume changes are relative, without a current volume value.</summary>
		Relative,
		/// <summary>Volume changes are relative and a current volume value is available.</summary>
		RelativeWithCurrent,
		/// <summary>The volume is set to an absolute value.</summary>
		Absolute,
	}

	/// <summary>Identifies the direction of a relative volume change.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueVolumeSelector : long {
		/// <summary>Increase the volume.</summary>
		VolumeIncrement = 0,
		/// <summary>Decrease the volume.</summary>
		VolumeDecrement,
	}

	/// <summary>Identifies the requested media playback state of a television.</summary>
	[TV (18, 0), NoMac, iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HMCharacteristicValueTargetMediaState : long {
		/// <summary>Start or resume media playback.</summary>
		Play = 0,
		/// <summary>Pause media playback.</summary>
		Pause = 1,
		/// <summary>Stop media playback.</summary>
		Stop = 2,
	}
}
