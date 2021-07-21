using CoreFoundation;
using ObjCRuntime;
using Foundation;

using System;

namespace Chip {

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Native]
	public enum ChipOnboardingPayloadType : ulong {
		QRCode = 0,
		ManualCode,
		Nfc,
		Admin,
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Native]
	public enum ChipPairingStatus : ulong {
		SecurePairingSuccess = 0,
		SecurePairingFailed,
		UnknownStatus,
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[ErrorDomain ("CHIPErrorDomain")]
	[Native]
	public enum ChipErrorCode : long {
		Success = 0,
		UndefinedError = 1,
		InvalidStringLength = 2,
		InvalidIntegerValue = 3,
		InvalidArgument = 4,
		InvalidMessageLength = 5,
		InvalidState = 6,
		WrongAddressType = 7,
		IntegrityCheckFailed = 8,
		DuplicateExists = 9,
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Flags]
	[Native]
	public enum ChipRendezvousInformationFlags : ulong {
		None = 0,
		SoftAP = 1uL << 0,
		Ble = 1uL << 1,
		OnNetwork = 1uL << 2,
		AllMask = SoftAP | Ble | OnNetwork,
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Native]
	public enum ChipCommissioningFlow : ulong {
		Standard = 0,
		UserActionRequired = 1,
		Custom = 2,
		Invalid = 3,
	}
	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Native]
	public enum ChipOptionalQRCodeInfoType : ulong {
		Unknown,
		String,
		Int32,
	}


	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPDevice")]
	[DisableDefaultCtor]
	interface ChipDevice
	{
		[Export ("openPairingWindow:error:")]
		bool OpenPairingWindow (nuint duration, [NullAllowed] out NSError error);

		[Export ("openPairingWindowWithPIN:discriminator:setupPIN:error:")]
		[return: NullAllowed]
		string OpenPairingWindow (nuint duration, nuint discriminator, nuint setupPin, [NullAllowed] out NSError error);

		[Export ("isActive")]
		bool IsActive { get; }
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPCluster")]
	[DisableDefaultCtor]
	interface ChipCluster
	{
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	delegate void ChipResponseHandler ([NullAllowed] NSError error, [NullAllowed] NSDictionary data);

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPApplicationBasic")]
	[DisableDefaultCtor]
	interface ChipApplicationBasic
	{
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeVendorNameWithResponseHandler:")]
		void ReadAttributeVendorName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeVendorIdWithResponseHandler:")]
		void ReadAttributeVendorId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationNameWithResponseHandler:")]
		void ReadAttributeApplicationName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductIdWithResponseHandler:")]
		void ReadAttributeProductId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationIdWithResponseHandler:")]
		void ReadAttributeApplication (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCatalogVendorIdWithResponseHandler:")]
		void ReadAttributeCatalogVendorId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationSatusWithResponseHandler:")]
		void ReadAttributeApplicationSatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPBarrierControl")]
	[DisableDefaultCtor]
	interface ChipBarrierControl
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("barrierControlGoToPercent:responseHandler:")]
		void GoToPercent (byte percentOpen, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("barrierControlStop:")]
		void Stop (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBarrierMovingStateWithResponseHandler:")]
		void ReadAttributeBarrierMovingState (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBarrierSafetyStatusWithResponseHandler:")]
		void ReadAttributeBarrierSafetyStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBarrierCapabilitiesWithResponseHandler:")]
		void ReadAttributeBarrierCapabilities (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBarrierPositionWithResponseHandler:")]
		void ReadAttributeBarrierPosition (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPBasic")]
	[DisableDefaultCtor]
	interface ChipBasic
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Export ("mfgSpecificPing:")]
		void GetMfgSpecificPing (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInteractionModelVersionWithResponseHandler:")]
		void ReadAttributeInteractionModelVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeVendorNameWithResponseHandler:")]
		void ReadAttributeVendorName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeVendorIDWithResponseHandler:")]
		void ReadAttributeVendorId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductNameWithResponseHandler:")]
		void ReadAttributeProductName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductIDWithResponseHandler:")]
		void ReadAttributeProductId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeUserLabelWithResponseHandler:")]
		void ReadAttributeUserLabel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeUserLabelWithValue:responseHandler:")]
		void WriteAttributeUserLabel (string value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLocationWithResponseHandler:")]
		void ReadAttributeLocation (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeLocationWithValue:responseHandler:")]
		void WriteAttributeLocation (string value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeHardwareVersionWithResponseHandler:")]
		void ReadAttributeHardwareVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeHardwareVersionStringWithResponseHandler:")]
		void ReadAttributeHardwareVersionString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSoftwareVersionWithResponseHandler:")]
		void ReadAttributeSoftwareVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSoftwareVersionStringWithResponseHandler:")]
		void ReadAttributeSoftwareVersionString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeManufacturingDateWithResponseHandler:")]
		void ReadAttributeManufacturingDate (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePartNumberWithResponseHandler:")]
		void ReadAttributePartNumber (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductURLWithResponseHandler:")]
		void ReadAttributeProductUrl (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductLabelWithResponseHandler:")]
		void ReadAttributeProductLabel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSerialNumberWithResponseHandler:")]
		void ReadAttributeSerialNumber (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLocalConfigDisabledWithResponseHandler:")]
		void ReadAttributeLocalConfigDisabled (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeLocalConfigDisabledWithValue:responseHandler:")]
		void WriteAttributeLocalConfigDisabled (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPBinding")]
	[DisableDefaultCtor]
	interface ChipBinding
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("bind:groupId:endpointId:clusterId:responseHandler:")]
		void Bind (ulong nodeId, ushort groupId, byte endpointId, ushort clusterId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("unbind:groupId:endpointId:clusterId:responseHandler:")]
		void Unbind (ulong nodeId, ushort groupId, byte endpointId, ushort clusterId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPColorControl")]
	[DisableDefaultCtor]
	interface ChipColorControl
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveColor:rateY:optionsMask:optionsOverride:responseHandler:")]
		void MoveColor (short rateX, short rateY, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveColorTemperature:rate:colorTemperatureMinimum:colorTemperatureMaximum:optionsMask:optionsOverride:responseHandler:")]
		void MoveColorTemperature (byte moveMode, ushort rate, ushort colorTemperatureMinimum, ushort colorTemperatureMaximum, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveHue:rate:optionsMask:optionsOverride:responseHandler:")]
		void MoveHue (byte moveMode, byte rate, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveSaturation:rate:optionsMask:optionsOverride:responseHandler:")]
		void MoveSaturation (byte moveMode, byte rate, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToColor:colorY:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void MoveToColor (ushort colorX, ushort colorY, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToColorTemperature:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void MoveToColorTemperature (ushort colorTemperature, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToHue:direction:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void MoveToHue (byte hue, byte direction, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToHueAndSaturation:saturation:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void MoveToHueAndSaturation (byte hue, byte saturation, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToSaturation:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void MoveToSaturation (byte saturation, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stepColor:stepY:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void StepColor (short stepX, short stepY, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stepColorTemperature:stepSize:transitionTime:colorTemperatureMinimum:colorTemperatureMaximum:optionsMask:optionsOverride:responseHandler:")]
		void StepColorTemperature (byte stepMode, ushort stepSize, ushort transitionTime, ushort colorTemperatureMinimum, ushort colorTemperatureMaximum, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stepHue:stepSize:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void StepHue (byte stepMode, byte stepSize, byte transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stepSaturation:stepSize:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void StepSaturation (byte stepMode, byte stepSize, byte transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stopMoveStep:optionsOverride:responseHandler:")]
		void StopMoveStep (byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentHueWithResponseHandler:")]
		void ReadAttributeCurrentHue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentHueWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentHue (ushort minInterval, ushort maxInterval, byte change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentHueWithResponseHandler:")]
		void ReportAttributeCurrentHue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentSaturationWithResponseHandler:")]
		void ReadAttributeCurrentSaturation (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentSaturationWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentSaturation (ushort minInterval, ushort maxInterval, byte change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentSaturationWithResponseHandler:")]
		void ReportAttributeCurrentSaturation (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRemainingTimeWithResponseHandler:")]
		void ReadAttributeRemainingTime (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentXWithResponseHandler:")]
		void ReadAttributeCurrentX (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentXWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentX (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentXWithResponseHandler:")]
		void ReportAttributeCurrentX (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentYWithResponseHandler:")]
		void ReadAttributeCurrentY (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentYWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentY (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentYWithResponseHandler:")]
		void ReportAttributeCurrentY (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeDriftCompensationWithResponseHandler:")]
		void ReadAttributeDriftCompensation (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCompensationTextWithResponseHandler:")]
		void ReadAttributeCompensationText (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorTemperatureWithResponseHandler:")]
		void ReadAttributeColorTemperature (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeColorTemperatureWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeColorTemperature (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeColorTemperatureWithResponseHandler:")]
		void ReportAttributeColorTemperature (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorModeWithResponseHandler:")]
		void ReadAttributeColorMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorControlOptionsWithResponseHandler:")]
		void ReadAttributeColorControlOptions (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorControlOptionsWithValue:responseHandler:")]
		void WriteAttributeColorControlOptions (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNumberOfPrimariesWithResponseHandler:")]
		void ReadAttributeNumberOfPrimaries (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary1XWithResponseHandler:")]
		void ReadAttributePrimary1X (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary1YWithResponseHandler:")]
		void ReadAttributePrimary1Y (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary1IntensityWithResponseHandler:")]
		void ReadAttributePrimary1Intensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary2XWithResponseHandler:")]
		void ReadAttributePrimary2X (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary2YWithResponseHandler:")]
		void ReadAttributePrimary2Y (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary2IntensityWithResponseHandler:")]
		void ReadAttributePrimary2Intensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary3XWithResponseHandler:")]
		void ReadAttributePrimary3X (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary3YWithResponseHandler:")]
		void ReadAttributePrimary3Y (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary3IntensityWithResponseHandler:")]
		void ReadAttributePrimary3Intensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary4XWithResponseHandler:")]
		void ReadAttributePrimary4X (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary4YWithResponseHandler:")]
		void ReadAttributePrimary4Y (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary4IntensityWithResponseHandler:")]
		void ReadAttributePrimary4Intensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary5XWithResponseHandler:")]
		void ReadAttributePrimary5X (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary5YWithResponseHandler:")]
		void ReadAttributePrimary5Y (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary5IntensityWithResponseHandler:")]
		void ReadAttributePrimary5Intensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary6XWithResponseHandler:")]
		void ReadAttributePrimary6X (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary6YWithResponseHandler:")]
		void ReadAttributePrimary6Y (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePrimary6IntensityWithResponseHandler:")]
		void ReadAttributePrimary6Intensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWhitePointXWithResponseHandler:")]
		void ReadAttributeWhitePointX (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeWhitePointXWithValue:responseHandler:")]
		void WriteAttributeWhitePointX (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWhitePointYWithResponseHandler:")]
		void ReadAttributeWhitePointY (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeWhitePointYWithValue:responseHandler:")]
		void WriteAttributeWhitePointY (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointRXWithResponseHandler:")]
		void ReadAttributeColorPointRX (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointRXWithValue:responseHandler:")]
		void WriteAttributeColorPointRX (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointRYWithResponseHandler:")]
		void ReadAttributeColorPointRY (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointRYWithValue:responseHandler:")]
		void WriteAttributeColorPointRY (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointRIntensityWithResponseHandler:")]
		void ReadAttributeColorPointRIntensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointRIntensityWithValue:responseHandler:")]
		void WriteAttributeColorPointRIntensity (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointGXWithResponseHandler:")]
		void ReadAttributeColorPointGX (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointGXWithValue:responseHandler:")]
		void WriteAttributeColorPointGX (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointGYWithResponseHandler:")]
		void ReadAttributeColorPointGY (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointGYWithValue:responseHandler:")]
		void WriteAttributeColorPointGY (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointGIntensityWithResponseHandler:")]
		void ReadAttributeColorPointGIntensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointGIntensityWithValue:responseHandler:")]
		void WriteAttributeColorPointGIntensity (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointBXWithResponseHandler:")]
		void ReadAttributeColorPointBX (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointBXWithValue:responseHandler:")]
		void WriteAttributeColorPointBX (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointBYWithResponseHandler:")]
		void ReadAttributeColorPointBY (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointBYWithValue:responseHandler:")]
		void WriteAttributeColorPointBY (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorPointBIntensityWithResponseHandler:")]
		void ReadAttributeColorPointBIntensity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeColorPointBIntensityWithValue:responseHandler:")]
		void WriteAttributeColorPointBIntensity (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEnhancedCurrentHueWithResponseHandler:")]
		void ReadAttributeEnhancedCurrentHue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEnhancedColorModeWithResponseHandler:")]
		void ReadAttributeEnhancedColorMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorLoopActiveWithResponseHandler:")]
		void ReadAttributeColorLoopActive (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorLoopDirectionWithResponseHandler:")]
		void ReadAttributeColorLoopDirection (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorLoopTimeWithResponseHandler:")]
		void ReadAttributeColorLoopTime (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorCapabilitiesWithResponseHandler:")]
		void ReadAttributeColorCapabilities (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorTempPhysicalMinWithResponseHandler:")]
		void ReadAttributeColorTempPhysicalMin (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorTempPhysicalMaxWithResponseHandler:")]
		void ReadAttributeColorTempPhysicalMax (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCoupleColorTempToLevelMinMiredsWithResponseHandler:")]
		void ReadAttributeCoupleColorTempToLevelMinMireds (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeStartUpColorTemperatureMiredsWithResponseHandler:")]
		void ReadAttributeStartUpColorTemperatureMireds (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeStartUpColorTemperatureMiredsWithValue:responseHandler:")]
		void WriteAttributeStartUpColorTemperatureMireds (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPDescriptor")]
	[DisableDefaultCtor]
	interface ChipDescriptor
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeDeviceListWithResponseHandler:")]
		void ReadAttributeDeviceList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeServerListWithResponseHandler:")]
		void ReadAttributeServerList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClientListWithResponseHandler:")]
		void ReadAttributeClientList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePartsListWithResponseHandler:")]
		void ReadAttributePartsList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPDoorLock")]
	[DisableDefaultCtor]
	interface ChipDoorLock
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearAllPins:")]
		void ClearAllPins (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearAllRfids:")]
		void ClearAllRfids (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearHolidaySchedule:responseHandler:")]
		void ClearHolidaySchedule (byte scheduleId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearPin:responseHandler:")]
		void ClearPin (ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearRfid:responseHandler:")]
		void ClearRfid (ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearWeekdaySchedule:userId:responseHandler:")]
		void ClearWeekdaySchedule (byte scheduleId, ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearYeardaySchedule:userId:responseHandler:")]
		void ClearYeardaySchedule (byte scheduleId, ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getHolidaySchedule:responseHandler:")]
		void GetHolidaySchedule (byte scheduleId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getLogRecord:responseHandler:")]
		void GetLogRecord (ushort logIndex, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getPin:responseHandler:")]
		void GetPin (ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getRfid:responseHandler:")]
		void GetRfid (ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getUserType:responseHandler:")]
		void GetUserType (ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getWeekdaySchedule:userId:responseHandler:")]
		void GetWeekdaySchedule (byte scheduleId, ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getYeardaySchedule:userId:responseHandler:")]
		void GetYeardaySchedule (byte scheduleId, ushort userId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("lockDoor:responseHandler:")]
		void LockDoor (string pin, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setHolidaySchedule:localStartTime:localEndTime:operatingModeDuringHoliday:responseHandler:")]
		void SetHolidaySchedule (byte scheduleId, uint localStartTime, uint localEndTime, byte operatingModeDuringHoliday, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setPin:userStatus:userType:pin:responseHandler:")]
		void SetPin (ushort userId, byte userStatus, byte userType, string pin, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setRfid:userStatus:userType:id:responseHandler:")]
		void SetRfid (ushort userId, byte userStatus, byte userType, string id, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setUserType:userType:responseHandler:")]
		void SetUserType (ushort userId, byte userType, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setWeekdaySchedule:userId:daysMask:startHour:startMinute:endHour:endMinute:responseHandler:")]
		void SetWeekdaySchedule (byte scheduleId, ushort userId, byte daysMask, byte startHour, byte startMinute, byte endHour, byte endMinute, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setYeardaySchedule:userId:localStartTime:localEndTime:responseHandler:")]
		void SetYeardaySchedule (byte scheduleId, ushort userId, uint localStartTime, uint localEndTime, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("unlockDoor:responseHandler:")]
		void UnlockDoor (string pin, ChipResponseHandler responseHandler);
		[Async (ResultTypeName = "ChipReadAttributeResult")]

		[Export ("unlockWithTimeout:pin:responseHandler:")]
		void UnlockWithTimeout (ushort timeoutInSeconds, string pin, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLockStateWithResponseHandler:")]
		void ReadAttributeLockState (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeLockStateWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeLockState (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeLockStateWithResponseHandler:")]
		void ReportAttributeLockState (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLockTypeWithResponseHandler:")]
		void ReadAttributeLockType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeActuatorEnabledWithResponseHandler:")]
		void ReadAttributeActuatorEnabled (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPGeneralCommissioning")]
	[DisableDefaultCtor]
	interface ChipGeneralCommissioning
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("armFailSafe:breadcrumb:timeoutMs:responseHandler:")]
		void ArmFailSafe (ushort expiryLengthSeconds, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("commissioningComplete:")]
		void CommissioningComplete (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setRegulatoryConfig:countryCode:breadcrumb:timeoutMs:responseHandler:")]
		void SetRegulatoryConfig (byte location, string countryCode, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeFabricIdWithResponseHandler:")]
		void ReadAttributeFabricId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBreadcrumbWithResponseHandler:")]
		void ReadAttributeBreadcrumb (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeBreadcrumbWithValue:responseHandler:")]
		void WriteAttributeBreadcrumb (ulong value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPGroupKeyManagement")]
	[DisableDefaultCtor]
	interface ChipGroupKeyManagement
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeGroupsWithResponseHandler:")]
		void ReadAttributeGroups (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeGroupKeysWithResponseHandler:")]
		void ReadAttributeGroupKeys (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPGroups")]
	[DisableDefaultCtor]
	interface ChipGroups
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addGroup:groupName:responseHandler:")]
		void AddGroup (ushort groupId, string groupName, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addGroupIfIdentifying:groupName:responseHandler:")]
		void AddGroupIfIdentifying (ushort groupId, string groupName, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getGroupMembership:groupList:responseHandler:")]
		void GetGroupMembership (byte groupCount, ushort groupList, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeAllGroups:")]
		void RemoveAllGroups (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeGroup:responseHandler:")]
		void RemoveGroup (ushort groupId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("viewGroup:responseHandler:")]
		void ViewGroup (ushort groupId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNameSupportWithResponseHandler:")]
		void ReadAttributeNameSupport (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPIdentify")]
	[DisableDefaultCtor]
	interface ChipIdentify
	{
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("identify:responseHandler:")]
		void Identify (ushort identifyTime, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("identifyQuery:")]
		void IdentifyQuery (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeIdentifyTimeWithResponseHandler:")]
		void ReadAttributeIdentifyTime (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeIdentifyTimeWithValue:responseHandler:")]
		void WriteAttributeIdentifyTime (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPLevelControl")]
	[DisableDefaultCtor]
	interface ChipLevelControl
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("move:rate:optionMask:optionOverride:responseHandler:")]
		void Move (byte moveMode, byte rate, byte optionMask, byte optionOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToLevel:transitionTime:optionMask:optionOverride:responseHandler:")]
		void MoveToLevel (byte level, ushort transitionTime, byte optionMask, byte optionOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveToLevelWithOnOff:transitionTime:responseHandler:")]
		void MoveToLevelWithOnOff (byte level, ushort transitionTime, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("moveWithOnOff:rate:responseHandler:")]
		void MoveWithOnOff (byte moveMode, byte rate, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("step:stepSize:transitionTime:optionMask:optionOverride:responseHandler:")]
		void Step (byte stepMode, byte stepSize, ushort transitionTime, byte optionMask, byte optionOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stepWithOnOff:stepSize:transitionTime:responseHandler:")]
		void StepWithOnOff (byte stepMode, byte stepSize, ushort transitionTime, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stop:optionOverride:responseHandler:")]
		void Stop (byte optionMask, byte optionOverride, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stopWithOnOff:")]
		void StopWithOnOff (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentLevelWithResponseHandler:")]
		void ReadAttributeCurrentLevel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentLevelWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentLevel (ushort minInterval, ushort maxInterval, byte change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentLevelWithResponseHandler:")]
		void ReportAttributeCurrentLevel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPLowPower")]
	[DisableDefaultCtor]
	interface ChipLowPower
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("sleep:")]
		void Sleep (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPNetworkCommissioning")]
	[DisableDefaultCtor]
	interface ChipNetworkCommissioning
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addThreadNetwork:breadcrumb:timeoutMs:responseHandler:")]
		void AddThreadNetwork (NSData operationalDataset, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addWiFiNetwork:credentials:breadcrumb:timeoutMs:responseHandler:")]
		void AddWiFiNetwork (NSData ssid, NSData credentials, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("disableNetwork:breadcrumb:timeoutMs:responseHandler:")]
		void DisableNetwork (NSData networkId, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("enableNetwork:breadcrumb:timeoutMs:responseHandler:")]
		void EnableNetwork (NSData networkId, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getLastNetworkCommissioningResult:responseHandler:")]
		void GetLastNetworkCommissioningResult (uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeNetwork:breadcrumb:timeoutMs:responseHandler:")]
		void RemoveNetwork (NSData networkId, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("scanNetworks:breadcrumb:timeoutMs:responseHandler:")]
		void ScanNetworks (NSData ssid, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("updateThreadNetwork:breadcrumb:timeoutMs:responseHandler:")]
		void UpdateThreadNetwork (NSData operationalDataset, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("updateWiFiNetwork:credentials:breadcrumb:timeoutMs:responseHandler:")]
		void UpdateWiFiNetwork (NSData ssid, NSData credentials, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPOnOff")]
	[DisableDefaultCtor]
	interface ChipOnOff
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("off:")]
		void Off (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("on:")]
		void On (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("toggle:")]
		void Toggle (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOnOffWithResponseHandler:")]
		void ReadAttributeOnOff (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeOnOffWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeOnOff (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeOnOffWithResponseHandler:")]
		void ReportAttributeOnOff (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPOperationalCredentials")]
	[DisableDefaultCtor]
	interface ChipOperationalCredentials
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addOpCert:iCACertificate:iPKValue:caseAdminNode:adminVendorId:responseHandler:")]
		void AddOpCert (NSData noc, NSData iCACertificate, NSData iPKValue, ulong caseAdminNode, ushort adminVendorId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("opCSRRequest:responseHandler:")]
		void OpCsrRequest (NSData csrNonce, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeAllFabrics:")]
		void RemoveAllFabrics (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeFabric:nodeId:vendorId:responseHandler:")]
		void RemoveFabric (ulong fabricId, ulong nodeId, ushort vendorId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setFabric:responseHandler:")]
		void SetFabric (ushort vendorId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("updateFabricLabel:responseHandler:")]
		void UpdateFabricLabel (string label, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeFabricsListWithResponseHandler:")]
		void ReadAttributeFabricsList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPPumpConfigurationAndControl")]
	[DisableDefaultCtor]
	interface ChipPumpConfigurationAndControl
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxPressureWithResponseHandler:")]
		void ReadAttributeMaxPressure (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxSpeedWithResponseHandler:")]
		void ReadAttributeMaxSpeed (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxFlowWithResponseHandler:")]
		void ReadAttributeMaxFlow (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEffectiveOperationModeWithResponseHandler:")]
		void ReadAttributeEffectiveOperationMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEffectiveControlModeWithResponseHandler:")]
		void ReadAttributeEffectiveControlMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCapacityWithResponseHandler:")]
		void ReadAttributeCapacity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCapacityWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCapacity (ushort minInterval, ushort maxInterval, short change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCapacityWithResponseHandler:")]
		void ReportAttributeCapacity (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOperationModeWithResponseHandler:")]
		void ReadAttributeOperationMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOperationModeWithValue:responseHandler:")]
		void WriteAttributeOperationMode (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPScenes")]
	[DisableDefaultCtor]
	interface ChipScenes
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addScene:sceneId:transitionTime:sceneName:clusterId:length:value:responseHandler:")]
		void AddScene (ushort groupId, byte sceneId, ushort transitionTime, string sceneName, ushort clusterId, byte length, byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getSceneMembership:responseHandler:")]
		void GetSceneMembership (ushort groupId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("recallScene:sceneId:transitionTime:responseHandler:")]
		void RecallScene (ushort groupId, byte sceneId, ushort transitionTime, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeAllScenes:responseHandler:")]
		void RemoveAllScenes (ushort groupId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeScene:sceneId:responseHandler:")]
		void RemoveScene (ushort groupId, byte sceneId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("storeScene:sceneId:responseHandler:")]
		void StoreScene (ushort groupId, byte sceneId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("viewScene:sceneId:responseHandler:")]
		void ViewScene (ushort groupId, byte sceneId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSceneCountWithResponseHandler:")]
		void ReadAttributeSceneCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentSceneWithResponseHandler:")]
		void ReadAttributeCurrentScene (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentGroupWithResponseHandler:")]
		void ReadAttributeCurrentGroup (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSceneValidWithResponseHandler:")]
		void ReadAttributeSceneValid (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNameSupportWithResponseHandler:")]
		void ReadAttributeNameSupport (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPSwitch")]
	[DisableDefaultCtor]
	interface ChipSwitch
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNumberOfPositionsWithResponseHandler:")]
		void ReadAttributeNumberOfPositions (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionWithResponseHandler:")]
		void ReadAttributeCurrentPosition (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPosition (ushort minInterval, ushort maxInterval, byte change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionWithResponseHandler:")]
		void ReportAttributeCurrentPosition (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPTemperatureMeasurement")]
	[DisableDefaultCtor]
	interface ChipTemperatureMeasurement
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMeasuredValueWithResponseHandler:")]
		void ReadAttributeMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeMeasuredValueWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeMeasuredValue (ushort minInterval, ushort maxInterval, short change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeMeasuredValueWithResponseHandler:")]
		void ReportAttributeMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMinMeasuredValueWithResponseHandler:")]
		void ReadAttributeMinMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxMeasuredValueWithResponseHandler:")]
		void ReadAttributeMaxMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPTestCluster")]
	[DisableDefaultCtor]
	interface ChipTestCluster
	{
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("test:")]
		void Test (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("testNotHandled:")]
		void TestNotHandled (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("testSpecific:")]
		void TestSpecific (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("testUnknownCommand:")]
		void TestUnknownCommand (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBooleanWithResponseHandler:")]
		void ReadAttributeBoolean (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeBooleanWithValue:responseHandler:")]
		void WriteAttributeBoolean (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBitmap8WithResponseHandler:")]
		void ReadAttributeBitmap8 (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeBitmap8WithValue:responseHandler:")]
		void WriteAttributeBitmap8 (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBitmap16WithResponseHandler:")]
		void ReadAttributeBitmap16 (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeBitmap16WithValue:responseHandler:")]
		void WriteAttributeBitmap16 (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBitmap32WithResponseHandler:")]
		void ReadAttributeBitmap32 (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeBitmap32WithValue:responseHandler:")]
		void WriteAttributeBitmap32 (uint value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBitmap64WithResponseHandler:")]
		void ReadAttributeBitmap64 (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeBitmap64WithValue:responseHandler:")]
		void WriteAttributeBitmap64 (ulong value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt8uWithResponseHandler:")]
		void ReadAttributeInt8u (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt8uWithValue:responseHandler:")]
		void WriteAttributeInt8u (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt16uWithResponseHandler:")]
		void ReadAttributeInt16u (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt16uWithValue:responseHandler:")]
		void WriteAttributeInt16u (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt32uWithResponseHandler:")]
		void ReadAttributeInt32u (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt32uWithValue:responseHandler:")]
		void WriteAttributeInt32u (uint value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt64uWithResponseHandler:")]
		void ReadAttributeInt64u (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt64uWithValue:responseHandler:")]
		void WriteAttributeInt64u (ulong value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt8sWithResponseHandler:")]
		void ReadAttributeInt8s (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt8sWithValue:responseHandler:")]
		void WriteAttributeInt8s (sbyte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt16sWithResponseHandler:")]
		void ReadAttributeInt16s (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt16sWithValue:responseHandler:")]
		void WriteAttributeInt16s (short value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt32sWithResponseHandler:")]
		void ReadAttributeInt32s (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt32sWithValue:responseHandler:")]
		void WriteAttributeInt32s (int value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInt64sWithResponseHandler:")]
		void ReadAttributeInt64s (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeInt64sWithValue:responseHandler:")]
		void WriteAttributeInt64s (long value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEnum8WithResponseHandler:")]
		void ReadAttributeEnum8 (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeEnum8WithValue:responseHandler:")]
		void WriteAttributeEnum8 (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEnum16WithResponseHandler:")]
		void ReadAttributeEnum16 (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeEnum16WithValue:responseHandler:")]
		void WriteAttributeEnum16 (ushort value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOctetStringWithResponseHandler:")]
		void ReadAttributeOctetString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOctetStringWithValue:responseHandler:")]
		void WriteAttributeOctetString (NSData value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeListInt8uWithResponseHandler:")]
		void ReadAttributeListInt8u (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeListOctetStringWithResponseHandler:")]
		void ReadAttributeListOctetString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeListStructOctetStringWithResponseHandler:")]
		void ReadAttributeListStructOctetString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPThermostat")]
	[DisableDefaultCtor]
	interface ChipThermostat
	{
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("clearWeeklySchedule:")]
		void ClearWeeklySchedule (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getRelayStatusLog:")]
		void GetRelayStatusLog (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getWeeklySchedule:modeToReturn:responseHandler:")]
		void GetWeeklySchedule (byte daysToReturn, byte modeToReturn, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setWeeklySchedule:dayOfWeekForSequence:modeForSequence:payload:responseHandler:")]
		void SetWeeklySchedule (byte numberOfTransitionsForSequence, byte dayOfWeekForSequence, byte modeForSequence, byte payload, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("setpointRaiseLower:amount:responseHandler:")]
		void SetpointRaiseLower (byte mode, sbyte amount, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLocalTemperatureWithResponseHandler:")]
		void ReadAttributeLocalTemperature (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeLocalTemperatureWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeLocalTemperature (ushort minInterval, ushort maxInterval, short change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeLocalTemperatureWithResponseHandler:")]
		void ReportAttributeLocalTemperature (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOccupiedCoolingSetpointWithResponseHandler:")]
		void ReadAttributeOccupiedCoolingSetpoint (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOccupiedCoolingSetpointWithValue:responseHandler:")]
		void WriteAttributeOccupiedCoolingSetpoint (short value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOccupiedHeatingSetpointWithResponseHandler:")]
		void ReadAttributeOccupiedHeatingSetpoint (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOccupiedHeatingSetpointWithValue:responseHandler:")]
		void WriteAttributeOccupiedHeatingSetpoint (short value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeControlSequenceOfOperationWithResponseHandler:")]
		void ReadAttributeControlSequenceOfOperation (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeControlSequenceOfOperationWithValue:responseHandler:")]
		void WriteAttributeControlSequenceOfOperation (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSystemModeWithResponseHandler:")]
		void ReadAttributeSystemMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeSystemModeWithValue:responseHandler:")]
		void WriteAttributeSystemMode (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPOnboardingPayloadParser")]
	[DisableDefaultCtor]
	interface ChipOnboardingPayloadParser
	{
		[Static]
		[Export ("setupPayloadForOnboardingPayload:ofType:error:")]
		[return: NullAllowed]
		ChipSetupPayload SetupPayload (string onboardingPayload, ChipOnboardingPayloadType type, [NullAllowed] out NSError error);
	}

	interface IChipDevicePairingDelegate {}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof (NSObject), Name="CHIPDevicePairingDelegate")]
	interface ChipDevicePairingDelegate
	{
		[Export ("onStatusUpdate:")]
		void OnStatusUpdate (ChipPairingStatus status);

		[Export ("onPairingComplete:")]
		void OnPairingComplete ([NullAllowed] NSError error);

		[Export ("onPairingDeleted:")]
		void OnPairingDeleted ([NullAllowed] NSError error);

		[Export ("onAddressUpdated:")]
		void OnAddressUpdated ([NullAllowed] NSError error);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPDeviceController")]
	[DisableDefaultCtor]
	interface ChipDeviceController
	{
		[Export ("isRunning")]
		bool IsRunning { get; }

		[Export ("pairDevice:discriminator:setupPINCode:error:")]
		bool PairDevice (ulong deviceId, ushort discriminator, uint setupPinCode, [NullAllowed] out NSError error);

		[Export ("pairDevice:address:port:discriminator:setupPINCode:error:")]
		bool PairDevice (ulong deviceId, string address, ushort port, ushort discriminator, uint setupPinCode, [NullAllowed] out NSError error);

		[Export ("pairDeviceWithoutSecurity:address:port:error:")]
		bool PairDeviceWithoutSecurity (ulong deviceId, string address, ushort port, [NullAllowed] out NSError error);

		[Export ("pairDevice:onboardingPayload:onboardingPayloadType:error:")]
		bool PairDevice (ulong deviceId, string onboardingPayload, ChipOnboardingPayloadType onboardingPayloadType, [NullAllowed] out NSError error);

		[Export ("setListenPort:")]
		void SetListenPort (ushort port);

		[Export ("unpairDevice:error:")]
		bool UnpairDevice (ulong deviceId, [NullAllowed] out NSError error);

		[Export ("stopDevicePairing:error:")]
		bool StopDevicePairing (ulong deviceId, [NullAllowed] out NSError error);

		[Export ("updateDevice:fabricId:")]
		void UpdateDevice (ulong deviceId, ulong fabricId);

		[Export ("getPairedDevice:error:")]
		[return: NullAllowed]
		ChipDevice GetPairedDevice (ulong deviceId, [NullAllowed] out NSError error);

		[Static]
		[Export ("sharedController")]
		ChipDeviceController SharedController { get; }

		[Export ("getControllerNodeId")]
		NSNumber ControllerNodeId { get; }

		[Export ("setPairingDelegate:queue:")]
		void SetPairingDelegate (IChipDevicePairingDelegate @delegate, DispatchQueue queue);

		[Export ("startup:")]
		bool Startup ([NullAllowed] IChipPersistentStorageDelegate storageDelegate);

		[Export ("shutdown")]
		bool Shutdown ();
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPError")]
	[DisableDefaultCtor]
	interface ChipError
	{
		[Static]
		[Export ("errorForCHIPErrorCode:")]
		[return: NullAllowed]
		NSError Create (int errorCode);

		[Static]
		[Export ("errorToCHIPErrorCode:")]
		int ConvertToChipErrorCode (NSError errorCode);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPManualSetupPayloadParser")]
	[DisableDefaultCtor]
	interface ChipManualSetupPayloadParser
	{
		[Export ("initWithDecimalStringRepresentation:")]
		IntPtr Constructor (string decimalStringRepresentation);

		[Export ("populatePayload:")]
		[return: NullAllowed]
		ChipSetupPayload PopulatePayload ([NullAllowed] out NSError error);
	}

	interface IChipPersistentStorageDelegate {}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof (NSObject), Name="CHIPPersistentStorageDelegate")]
	interface ChipPersistentStorageDelegate
	{
		[Abstract]
		[Export ("CHIPGetKeyValue:")]
		[return: NullAllowed]
		string GetValue (string key);

		[Abstract]
		[Export ("CHIPSetKeyValue:value:")]
		void SetValue (string key, string value);

		[Abstract]
		[Export ("CHIPDeleteKeyValue:")]
		void DeleteValue (string key);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPOptionalQRCodeInfo")]
	[DisableDefaultCtor]
	interface ChipOptionalQRCodeInfo
	{
		[Export ("infoType", ArgumentSemantic.Strong)]
		NSNumber InfoType { get; set; }

		[Export ("tag", ArgumentSemantic.Strong)]
		NSNumber Tag { get; set; }

		[Export ("integerValue", ArgumentSemantic.Strong)]
		NSNumber IntegerValue { get; set; }

		[Export ("stringValue", ArgumentSemantic.Strong)]
		string StringValue { get; set; }
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPSetupPayload")]
	[DisableDefaultCtor]
	interface ChipSetupPayload
	{
		[Export ("version", ArgumentSemantic.Strong)]
		NSNumber Version { get; set; }

		[Export ("vendorID", ArgumentSemantic.Strong)]
		NSNumber VendorId { get; set; }

		[Export ("productID", ArgumentSemantic.Strong)]
		NSNumber ProductId { get; set; }

		[Export ("commissioningFlow", ArgumentSemantic.Assign)]
		ChipCommissioningFlow CommissioningFlow { get; set; }

		[Export ("requiresCustomFlow")]
		bool RequiresCustomFlow { get; set; }

		[Export ("rendezvousInformation", ArgumentSemantic.Assign)]
		ChipRendezvousInformationFlags RendezvousInformation { get; set; }

		[Export ("discriminator", ArgumentSemantic.Strong)]
		NSNumber Discriminator { get; set; }

		[Export ("setUpPINCode", ArgumentSemantic.Strong)]
		NSNumber SetUpPinCode { get; set; }

		[Export ("serialNumber", ArgumentSemantic.Strong)]
		string SerialNumber { get; set; }

		[Export ("getAllOptionalVendorData:")]
		[return: NullAllowed]
		ChipOptionalQRCodeInfo[] GetAllOptionalVendorData ([NullAllowed] out NSError error);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject), Name="CHIPQRCodeSetupPayloadParser")]
	[DisableDefaultCtor]
	interface ChipQRCodeSetupPayloadParser
	{
		[Export ("initWithBase38Representation:")]
		IntPtr Constructor (string base38Representation);

		[Export ("populatePayload:")]
		[return: NullAllowed]
		ChipSetupPayload PopulatePayload ([NullAllowed] out NSError error);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPAccountLogin")]
	[DisableDefaultCtor]
	interface ChipAccountLogin
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("getSetupPIN:responseHandler:")]
		void GetSetupPin (string tempAccountIdentifier, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("login:setupPIN:responseHandler:")]
		void Login (string tempAccountIdentifier, string setupPin, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPApplicationLauncher")]
	[DisableDefaultCtor]
	interface ChipApplicationLauncher
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("launchApp:catalogVendorId:applicationId:responseHandler:")]
		void LaunchApp (string data, ushort catalogVendorId, string applicationId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationLauncherListWithResponseHandler:")]
		void ReadAttributeApplicationLauncherList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPAudioOutput")]
	[DisableDefaultCtor]
	interface ChipAudioOutput
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("renameOutput:name:responseHandler:")]
		void RenameOutput (byte index, string name, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("selectOutput:responseHandler:")]
		void SelectOutput (byte index, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAudioOutputListWithResponseHandler:")]
		void ReadAttributeAudioOutputList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPBinaryInputBasic")]
	[DisableDefaultCtor]
	interface ChipBinaryInputBasic
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOutOfServiceWithResponseHandler:")]
		void ReadAttributeOutOfService (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOutOfServiceWithValue:responseHandler:")]
		void WriteAttributeOutOfService (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePresentValueWithResponseHandler:")]
		void ReadAttributePresentValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributePresentValueWithValue:responseHandler:")]
		void WriteAttributePresentValue (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributePresentValueWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributePresentValue (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributePresentValueWithResponseHandler:")]
		void ReportAttributePresentValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeStatusFlagsWithResponseHandler:")]
		void ReadAttributeStatusFlags (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeStatusFlagsWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeStatusFlags (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeStatusFlagsWithResponseHandler:")]
		void ReportAttributeStatusFlags (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPBridgedDeviceBasic")]
	[DisableDefaultCtor]
	interface ChipBridgedDeviceBasic
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeVendorNameWithResponseHandler:")]
		void ReadAttributeVendorName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeVendorIDWithResponseHandler:")]
		void ReadAttributeVendorId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductNameWithResponseHandler:")]
		void ReadAttributeProductName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeUserLabelWithResponseHandler:")]
		void ReadAttributeUserLabel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeUserLabelWithValue:responseHandler:")]
		void WriteAttributeUserLabel (string value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeHardwareVersionWithResponseHandler:")]
		void ReadAttributeHardwareVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeHardwareVersionStringWithResponseHandler:")]
		void ReadAttributeHardwareVersionString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSoftwareVersionWithResponseHandler:")]
		void ReadAttributeSoftwareVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSoftwareVersionStringWithResponseHandler:")]
		void ReadAttributeSoftwareVersionString (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeManufacturingDateWithResponseHandler:")]
		void ReadAttributeManufacturingDate (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePartNumberWithResponseHandler:")]
		void ReadAttributePartNumber (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductURLWithResponseHandler:")]
		void ReadAttributeProductUrl (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeProductLabelWithResponseHandler:")]
		void ReadAttributeProductLabel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSerialNumberWithResponseHandler:")]
		void ReadAttributeSerialNumber (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeReachableWithResponseHandler:")]
		void ReadAttributeReachable (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPContentLaunch")]
	[DisableDefaultCtor]
	interface ChipContentLaunch
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("launchContent:data:responseHandler:")]
		void LaunchContent (byte autoPlay, string data, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("launchURL:displayString:responseHandler:")]
		void LaunchUrl (string contentUrl, string displayString, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAcceptsHeaderListWithResponseHandler:")]
		void ReadAttributeAcceptsHeaderList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSupportedStreamingTypesWithResponseHandler:")]
		void ReadAttributeSupportedStreamingTypes (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPEthernetNetworkDiagnostics")]
	[DisableDefaultCtor]
	interface ChipEthernetNetworkDiagnostics
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("resetCounts:")]
		void ResetCounts (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePacketRxCountWithResponseHandler:")]
		void ReadAttributePacketRxCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePacketTxCountWithResponseHandler:")]
		void ReadAttributePacketTxCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxErrCountWithResponseHandler:")]
		void ReadAttributeTxErrCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCollisionCountWithResponseHandler:")]
		void ReadAttributeCollisionCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOverrunCountWithResponseHandler:")]
		void ReadAttributeOverrunCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPFixedLabel")]
	[DisableDefaultCtor]
	interface ChipFixedLabel
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLabelListWithResponseHandler:")]
		void ReadAttributeLabelList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPGeneralDiagnostics")]
	[DisableDefaultCtor]
	interface ChipGeneralDiagnostics
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNetworkInterfacesWithResponseHandler:")]
		void ReadAttributeNetworkInterfaces (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRebootCountWithResponseHandler:")]
		void ReadAttributeRebootCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPKeypadInput")]
	[DisableDefaultCtor]
	interface ChipKeypadInput
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("sendKey:responseHandler:")]
		void SendKey (byte keyCode, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPMediaInput")]
	[DisableDefaultCtor]
	interface ChipMediaInput
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("hideInputStatus:")]
		void HideInputStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("renameInput:name:responseHandler:")]
		void RenameInput (byte index, string name, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("selectInput:responseHandler:")]
		void SelectInput (byte index, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("showInputStatus:")]
		void ShowInputStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMediaInputListWithResponseHandler:")]
		void ReadAttributeMediaInputList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPMediaPlayback")]
	[DisableDefaultCtor]
	interface ChipMediaPlayback
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaFastForward:")]
		void FastForward (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaNext:")]
		void Next (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaPause:")]
		void Pause (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaPlay:")]
		void Play (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaPrevious:")]
		void Previous (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaRewind:")]
		void Rewind (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaSkipBackward:responseHandler:")]
		void SkipBackward (ulong deltaPositionMilliseconds, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaSkipForward:responseHandler:")]
		void SkipForward (ulong deltaPositionMilliseconds, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaSkipSeek:responseHandler:")]
		void SkipSeek (ulong position, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaStartOver:")]
		void StartOver (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaStop:")]
		void Stop (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPRelativeHumidityMeasurement")]
	[DisableDefaultCtor]
	interface ChipRelativeHumidityMeasurement
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMeasuredValueWithResponseHandler:")]
		void ReadAttributeMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeMeasuredValueWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeMeasuredValue (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeMeasuredValueWithResponseHandler:")]
		void ReportAttributeMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMinMeasuredValueWithResponseHandler:")]
		void ReadAttributeMinMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxMeasuredValueWithResponseHandler:")]
		void ReadAttributeMaxMeasuredValue (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPSoftwareDiagnostics")]
	[DisableDefaultCtor]
	interface ChipSoftwareDiagnostics
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("resetWatermarks:")]
		void ResetWatermarks (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentHeapHighWatermarkWithResponseHandler:")]
		void ReadAttributeCurrentHeapHighWatermark (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPTargetNavigator")]
	[DisableDefaultCtor]
	interface ChipTargetNavigator
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("navigateTarget:data:responseHandler:")]
		void NavigateTarget (byte target, string data, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTargetNavigatorListWithResponseHandler:")]
		void ReadAttributeTargetNavigatorList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPTrustedRootCertificates")]
	[DisableDefaultCtor]
	interface ChipTrustedRootCertificates
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addTrustedRootCertificate:responseHandler:")]
		void AddTrustedRootCertificate (NSData rootCertificate, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeTrustedRootCertificate:responseHandler:")]
		void RemoveTrustedRootCertificate (NSData trustedRootIdentifier, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPTvChannel")]
	[DisableDefaultCtor]
	interface ChipTvChannel
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("changeChannel:responseHandler:")]
		void ChangeChannel (string match, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("changeChannelByNumber:minorNumber:responseHandler:")]
		void ChangeChannelByNumber (ushort majorNumber, ushort minorNumber, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("skipChannel:responseHandler:")]
		void SkipChannel (ushort count, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTvChannelListWithResponseHandler:")]
		void ReadAttributeTvChannelList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTvChannelLineupWithResponseHandler:")]
		void ReadAttributeTvChannelLineup (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentTvChannelWithResponseHandler:")]
		void ReadAttributeCurrentTvChannel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPWakeOnLan")]
	[DisableDefaultCtor]
	interface ChipWakeOnLan
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWakeOnLanMacAddressWithResponseHandler:")]
		void ReadAttributeWakeOnLanMacAddress(ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Mac (12,0), Watch (8,0), TV (15,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (ChipCluster), Name="CHIPWindowCovering")]
	[DisableDefaultCtor]
	interface ChipWindowCovering
	{

		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringDownClose:")]
		void DownClose (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringGoToLiftPercentage:responseHandler:")]
		void GoToLiftPercentage (byte percentageLiftValue, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringGoToLiftValue:responseHandler:")]
		void GoToLiftValue (ushort liftValue, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringGoToTiltPercentage:responseHandler:")]
		void GoToTiltPercentage (byte percentageTiltValue, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringGoToTiltValue:responseHandler:")]
		void GoToTiltValue (ushort tiltValue, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringStop:")]
		void Stop (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringUpOpen:")]
		void UpOpen (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWindowCoveringTypeWithResponseHandler:")]
		void ReadAttributeWindowCoveringType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeWindowCoveringTypeWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeWindowCoveringType (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeWindowCoveringTypeWithResponseHandler:")]
		void ReportAttributeWindowCoveringType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionLiftWithResponseHandler:")]
		void ReadAttributeCurrentPositionLift (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionLiftWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPositionLift (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionLiftWithResponseHandler:")]
		void ReportAttributeCurrentPositionLift (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionTiltWithResponseHandler:")]
		void ReadAttributeCurrentPositionTilt (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionTiltWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPositionTilt (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionTiltWithResponseHandler:")]
		void ReportAttributeCurrentPositionTilt (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeConfigStatusWithResponseHandler:")]
		void ReadAttributeConfigStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeConfigStatusWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeConfigStatus (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeConfigStatusWithResponseHandler:")]
		void ReportAttributeConfigStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInstalledOpenLimitLiftWithResponseHandler:")]
		void ReadAttributeInstalledOpenLimitLift (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInstalledClosedLimitLiftWithResponseHandler:")]
		void ReadAttributeInstalledClosedLimitLift (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInstalledOpenLimitTiltWithResponseHandler:")]
		void ReadAttributeInstalledOpenLimitTilt (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeInstalledClosedLimitTiltWithResponseHandler:")]
		void ReadAttributeInstalledClosedLimitTilt (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeModeWithResponseHandler:")]
		void ReadAttributeMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeModeWithValue:responseHandler:")]
		void WriteAttributeMode (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}


}
