#if !NET // Excluded from dotnet6 due to native API is not stable
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using Security;

using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Chip {

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum ChipOnboardingPayloadType : ulong {
		QRCode = 0,
		ManualCode,
		Nfc,
		Admin,
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum ChipPairingStatus : ulong {
		SecurePairingSuccess = 0,
		SecurePairingFailed,
		UnknownStatus,
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
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
		UnsupportedAttribute = 10,
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum ChipRendezvousInformationFlags : ulong {
		None = 0,
		SoftAP = 1uL << 0,
		Ble = 1uL << 1,
		OnNetwork = 1uL << 2,
		AllMask = SoftAP | Ble | OnNetwork,
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum ChipCommissioningFlow : ulong {
		Standard = 0,
		UserActionRequired = 1,
		Custom = 2,
		Invalid = 3,
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum ChipOptionalQRCodeInfoType : ulong {
		Unknown,
		String,
		Int32,
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPDevice")]
	[DisableDefaultCtor]
	interface ChipDevice {
		[Export ("openPairingWindow:error:")]
		bool OpenPairingWindow (nuint duration, [NullAllowed] out NSError error);

		[Export ("openPairingWindowWithPIN:discriminator:setupPIN:error:")]
		[return: NullAllowed]
		string OpenPairingWindow (nuint duration, nuint discriminator, nuint setupPin, [NullAllowed] out NSError error);

		[Export ("isActive")]
		bool IsActive { get; }
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPCluster")]
	[DisableDefaultCtor]
	interface ChipCluster {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void ChipResponseHandler ([NullAllowed] NSError error, [NullAllowed] NSDictionary data);

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPApplicationBasic")]
	[DisableDefaultCtor]
	interface ChipApplicationBasic {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("changeStatus:responseHandler:")]
		void ChangeStatus (byte status, ChipResponseHandler responseHandler);

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

#if !NET
		[Obsolete ("This method is removed, use 'ReadAttributeApplicationStatus' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ReadAttributeApplicationSatus (ChipResponseHandler responseHandler);
#endif

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationStatusWithResponseHandler:")]
		void ReadAttributeApplicationStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPBarrierControl")]
	[DisableDefaultCtor]
	interface ChipBarrierControl {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPBasic")]
	[DisableDefaultCtor]
	interface ChipBasic {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Wrap ("WriteAttributeLocalConfigDisabled (Convert.ToBoolean(value), responseHandler)", IsVirtual = true)]
		void WriteAttributeLocalConfigDisabled (byte value, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeLocalConfigDisabledWithValue:responseHandler:")]
		void WriteAttributeLocalConfigDisabled (bool disabled, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeReachableWithResponseHandler:")]
		void ReadAttributeReachable (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPBinding")]
	[DisableDefaultCtor]
	interface ChipBinding {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Wrap ("Bind (nodeId, groupId, (ushort) endpointId, (uint) clusterId, responseHandler)", IsVirtual = true)]
		void Bind (ulong nodeId, ushort groupId, byte endpointId, ushort clusterId, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("bind:groupId:endpointId:clusterId:responseHandler:")]
		void Bind (ulong nodeId, ushort groupId, ushort endpointId, uint clusterId, ChipResponseHandler responseHandler);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Wrap ("Unbind (nodeId, groupId, (ushort) endpointId, (uint) clusterId, responseHandler)", IsVirtual = true)]
		void Unbind (ulong nodeId, ushort groupId, byte endpointId, ushort clusterId, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("unbind:groupId:endpointId:clusterId:responseHandler:")]
		void Unbind (ulong nodeId, ushort groupId, ushort endpointId, uint clusterId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPColorControl")]
	[DisableDefaultCtor]
	interface ChipColorControl {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("colorLoopSet:action:direction:time:startHue:optionsMask:optionsOverride:responseHandler:")]
		void ColorLoopSet (byte updateFlags, byte action, byte direction, ushort time, ushort startHue, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("enhancedMoveHue:rate:optionsMask:optionsOverride:responseHandler:")]
		void EnhancedMoveHue (byte moveMode, ushort rate, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("enhancedMoveToHue:direction:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void EnhancedMoveToHue (ushort enhancedHue, byte direction, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("enhancedMoveToHueAndSaturation:saturation:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void EnhancedMoveToHueAndSaturation (ushort enhancedHue, byte saturation, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("enhancedStepHue:stepSize:transitionTime:optionsMask:optionsOverride:responseHandler:")]
		void EnhancedStepHue (byte stepMode, ushort stepSize, ushort transitionTime, byte optionsMask, byte optionsOverride, ChipResponseHandler responseHandler);

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorLoopStartEnhancedHueWithResponseHandler:")]
		void ReadAttributeColorLoopStartEnhancedHue (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeColorLoopStoredEnhancedHueWithResponseHandler:")]
		void ReadAttributeColorLoopStoredEnhancedHue (ChipResponseHandler responseHandler);

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPDescriptor")]
	[DisableDefaultCtor]
	interface ChipDescriptor {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPDiagnosticLogs")]
	[DisableDefaultCtor]
	interface ChipDiagnosticLogs {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Export ("retrieveLogsRequest:requestedProtocol:transferFileDesignator:responseHandler:")]
		void RetrieveLogsRequest (byte intent, byte requestedProtocol, NSData transferFileDesignator, ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPDoorLock")]
	[DisableDefaultCtor]
	interface ChipDoorLock {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPGroupKeyManagement")]
	[DisableDefaultCtor]
	interface ChipGroupKeyManagement {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPGroups")]
	[DisableDefaultCtor]
	interface ChipGroups {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPIdentify")]
	[DisableDefaultCtor]
	interface ChipIdentify {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPLevelControl")]
	[DisableDefaultCtor]
	interface ChipLevelControl {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPNetworkCommissioning")]
	[DisableDefaultCtor]
	interface ChipNetworkCommissioning {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeFeatureMapWithResponseHandler:")]
		void ReadAttributeFeatureMap (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPOtaSoftwareUpdateProvider")]
	[DisableDefaultCtor]
	interface ChipOtaSoftwareUpdateProvider {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("applyUpdateRequest:newVersion:responseHandler:")]
		void ApplyUpdateRequest (NSData updateToken, uint newVersion, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("notifyUpdateApplied:currentVersion:responseHandler:")]
		void NotifyUpdateApplied (NSData updateToken, uint currentVersion, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("queryImage:productId:imageType:hardwareVersion:currentVersion:protocolsSupported:location:requestorCanConsent:metadataForProvider:responseHandler:")]
		void QueryImage (ushort vendorId, ushort productId, ushort imageType, ushort hardwareVersion, uint currentVersion, byte protocolsSupported, string location, bool requestorCanConsent, NSData metadataForProvider, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPOccupancySensing")]
	[DisableDefaultCtor]
	interface ChipOccupancySensing {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOccupancyWithResponseHandler:")]
		void ReadAttributeOccupancy (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeOccupancyWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeOccupancy (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeOccupancyWithResponseHandler:")]
		void ReportAttributeOccupancy (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOccupancySensorTypeWithResponseHandler:")]
		void ReadAttributeOccupancySensorType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOccupancySensorTypeBitmapWithResponseHandler:")]
		void ReadAttributeOccupancySensorTypeBitmap (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPOnOff")]
	[DisableDefaultCtor]
	interface ChipOnOff {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("off:")]
		void Off (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("offWithEffect:effectVariant:responseHandler:")]
		void OffWithEffect (byte effectId, byte effectVariant, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("on:")]
		void On (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("onWithRecallGlobalScene:")]
		void OnWithRecallGlobalScene (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("onWithTimedOff:onTime:offWaitTime:responseHandler:")]
		void OnWithTimedOff (byte onOffControl, ushort onTime, ushort offWaitTime, ChipResponseHandler responseHandler);

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeGlobalSceneControlWithResponseHandler:")]
		void ReadAttributeGlobalSceneControl (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOnTimeWithResponseHandler:")]
		void ReadAttributeOnTime (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOnTimeWithValue:responseHandler:")]
		void WriteAttributeOnTime (ushort value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOffWaitTimeWithResponseHandler:")]
		void ReadAttributeOffWaitTime (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOffWaitTimeWithValue:responseHandler:")]
		void WriteAttributeOffWaitTime (ushort value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeStartUpOnOffWithResponseHandler:")]
		void ReadAttributeStartUpOnOff (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeStartUpOnOffWithValue:responseHandler:")]
		void WriteAttributeStartUpOnOff (byte value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeFeatureMapWithResponseHandler:")]
		void ReadAttributeFeatureMap (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPOnOffSwitchConfiguration")]
	[DisableDefaultCtor]
	interface ChipOnOffSwitchConfiguration {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSwitchTypeWithResponseHandler:")]
		void ReadAttributeSwitchType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSwitchActionsWithResponseHandler:")]
		void ReadAttributeSwitchActions (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeSwitchActionsWithValue:responseHandler:")]
		void WriteAttributeSwitchActions (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPOperationalCredentials")]
	[DisableDefaultCtor]
	interface ChipOperationalCredentials {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void AddOpCert (NSData noc, NSData iCACertificate, NSData iPKValue, ulong caseAdminNode, ushort adminVendorId, ChipResponseHandler responseHandler);
#endif

		// Parameter names are left to match header files. Without documentation, we cannot know what the parameters signify for certain.
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addNOC:iPKValue:caseAdminNode:adminVendorId:responseHandler:")]
		void AddNoc (NSData nocArray, NSData iPKValue, ulong caseAdminNode, ushort adminVendorId, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addTrustedRootCertificate:responseHandler:")]
		void AddTrustedRootCertificate (NSData rootCertificate, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("opCSRRequest:responseHandler:")]
		void OpCsrRequest (NSData csrNonce, ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void RemoveAllFabrics (ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void SetFabric (ushort vendorId, ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void RemoveFabric (ulong fabricId, ulong nodeId, ushort vendorId, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeFabric:responseHandler:")]
		void RemoveFabric (byte fabricIndex, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("removeTrustedRootCertificate:responseHandler:")]
		void RemoveTrustedRootCertificate (NSData trustedRootIdentifier, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("updateFabricLabel:responseHandler:")]
		void UpdateFabricLabel (string label, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("updateNOC:responseHandler:")]
		void UpdateNoc (NSData nocArray, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeFabricsListWithResponseHandler:")]
		void ReadAttributeFabricsList (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSupportedFabricsWithResponseHandler:")]
		void ReadAttributeSupportedFabrics (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCommissionedFabricsWithResponseHandler:")]
		void ReadAttributeCommissionedFabrics (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPPressureMeasurement")]
	[DisableDefaultCtor]
	interface ChipPressureMeasurement {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPPumpConfigurationAndControl")]
	[DisableDefaultCtor]
	interface ChipPumpConfigurationAndControl {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPScenes")]
	[DisableDefaultCtor]
	interface ChipScenes {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Wrap ("AddScene (groupId, sceneId, transitionTime, sceneName, (uint) clusterId, length, value, responseHandler)", IsVirtual = true)]
		void AddScene (ushort groupId, byte sceneId, ushort transitionTime, string sceneName, ushort clusterId, byte length, byte value, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("addScene:sceneId:transitionTime:sceneName:clusterId:length:value:responseHandler:")]
		void AddScene (ushort groupId, byte sceneId, ushort transitionTime, string sceneName, uint clusterId, byte length, byte value, ChipResponseHandler responseHandler);

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPSwitch")]
	[DisableDefaultCtor]
	interface ChipSwitch {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPTemperatureMeasurement")]
	[DisableDefaultCtor]
	interface ChipTemperatureMeasurement {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPThermostat")]
	[DisableDefaultCtor]
	interface ChipThermostat {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAbsMinHeatSetpointLimitWithResponseHandler:")]
		void ReadAttributeAbsMinHeatSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAbsMaxHeatSetpointLimitWithResponseHandler:")]
		void ReadAttributeAbsMaxHeatSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAbsMinCoolSetpointLimitWithResponseHandler:")]
		void ReadAttributeAbsMinCoolSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAbsMaxCoolSetpointLimitWithResponseHandler:")]
		void ReadAttributeAbsMaxCoolSetpointLimit (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOccupiedCoolingSetpointWithResponseHandler:")]
		void ReadAttributeOccupiedCoolingSetpoint (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOccupiedCoolingSetpointWithValue:responseHandler:")]
		void WriteAttributeOccupiedCoolingSetpoint (short value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMinHeatSetpointLimitWithResponseHandler:")]
		void ReadAttributeMinHeatSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeMinHeatSetpointLimitWithValue:responseHandler:")]
		void WriteAttributeMinHeatSetpointLimit (short value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxHeatSetpointLimitWithResponseHandler:")]
		void ReadAttributeMaxHeatSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeMaxHeatSetpointLimitWithValue:responseHandler:")]
		void WriteAttributeMaxHeatSetpointLimit (short value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMinCoolSetpointLimitWithResponseHandler:")]
		void ReadAttributeMinCoolSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeMinCoolSetpointLimitWithValue:responseHandler:")]
		void WriteAttributeMinCoolSetpointLimit (short value, ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMaxCoolSetpointLimitWithResponseHandler:")]
		void ReadAttributeMaxCoolSetpointLimit (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeMaxCoolSetpointLimitWithValue:responseHandler:")]
		void WriteAttributeMaxCoolSetpointLimit (short value, ChipResponseHandler responseHandler);

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeStartOfWeekWithResponseHandler:")]
		void ReadAttributeStartOfWeek (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNumberOfWeeklyTransitionsWithResponseHandler:")]
		void ReadAttributeNumberOfWeeklyTransitions (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNumberOfDailyTransitionsWithResponseHandler:")]
		void ReadAttributeNumberOfDailyTransitions (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeFeatureMapWithResponseHandler:")]
		void ReadAttributeFeatureMap (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPOnboardingPayloadParser")]
	[DisableDefaultCtor]
	interface ChipOnboardingPayloadParser {
		[Static]
		[Export ("setupPayloadForOnboardingPayload:ofType:error:")]
		[return: NullAllowed]
		ChipSetupPayload SetupPayload (string onboardingPayload, ChipOnboardingPayloadType type, [NullAllowed] out NSError error);
	}

	interface IChipDevicePairingDelegate { }

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject), Name = "CHIPDevicePairingDelegate")]
	interface ChipDevicePairingDelegate {
		[Export ("onStatusUpdate:")]
		void OnStatusUpdate (ChipPairingStatus status);

		[Export ("onPairingComplete:")]
		void OnPairingComplete ([NullAllowed] NSError error);

		[Export ("onPairingDeleted:")]
		void OnPairingDeleted ([NullAllowed] NSError error);

		[Export ("onAddressUpdated:")]
		void OnAddressUpdated ([NullAllowed] NSError error);
	}

	interface IChipKeypair { }

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject), Name = "CHIPKeypair")]
	interface ChipKeypair {
		[Abstract]
		[Export ("initialize")]
		bool Initialize ();

		[Abstract]
		[Export ("ECDSA_sign_hash:")]
		NSData EcdsaSignHash (NSData hash);

		[Abstract]
		[Export ("pubkey")]
		IntPtr /* SecKeyRef _Nullable */ GetPubKeyRef ();
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPManualSetupPayloadParser")]
	[DisableDefaultCtor]
	interface ChipManualSetupPayloadParser {
		[Export ("initWithDecimalStringRepresentation:")]
		NativeHandle Constructor (string decimalStringRepresentation);

		[Export ("populatePayload:")]
		[return: NullAllowed]
		ChipSetupPayload PopulatePayload ([NullAllowed] out NSError error);
	}

	interface IChipPersistentStorageDelegate { }

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject), Name = "CHIPPersistentStorageDelegate")]
	interface ChipPersistentStorageDelegate {
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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPOptionalQRCodeInfo")]
	[DisableDefaultCtor]
	interface ChipOptionalQRCodeInfo {
		[Export ("infoType", ArgumentSemantic.Strong)]
		NSNumber InfoType { get; set; }

		[Export ("tag", ArgumentSemantic.Strong)]
		NSNumber Tag { get; set; }

		[Export ("integerValue", ArgumentSemantic.Strong)]
		NSNumber IntegerValue { get; set; }

		[Export ("stringValue", ArgumentSemantic.Strong)]
		string StringValue { get; set; }
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPSetupPayload")]
	[DisableDefaultCtor]
	interface ChipSetupPayload {
		[Export ("version", ArgumentSemantic.Strong)]
		NSNumber Version { get; set; }

		[Export ("vendorID", ArgumentSemantic.Strong)]
		NSNumber VendorId { get; set; }

		[Export ("productID", ArgumentSemantic.Strong)]
		NSNumber ProductId { get; set; }

		[Export ("commissioningFlow", ArgumentSemantic.Assign)]
		ChipCommissioningFlow CommissioningFlow { get; set; }

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
		ChipOptionalQRCodeInfo [] GetAllOptionalVendorData ([NullAllowed] out NSError error);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (NSObject), Name = "CHIPThreadOperationalDataset")]
	[DisableDefaultCtor]
	interface ChipThreadOperationalDataset {
		[NullAllowed, Export ("networkName")]
		string NetworkName { get; }

		[NullAllowed, Export ("extendedPANID", ArgumentSemantic.Copy)]
		NSData ExtendedPanId { get; }

		[NullAllowed, Export ("masterKey", ArgumentSemantic.Copy)]
		NSData MasterKey { get; }

		// API names are left to match header files. 
		// PSK is likely pre-shared key, but without documentation, we cannot know for certain.
		[NullAllowed, Export ("PSKc", ArgumentSemantic.Copy)]
		NSData PSKc { get; }

		[Export ("channel")]
		ushort Channel { get; set; }

		[NullAllowed, Export ("panID", ArgumentSemantic.Copy)]
		NSData PanId { get; }

		[Export ("initWithNetworkName:extendedPANID:masterKey:PSKc:channel:panID:")]
		IntPtr Constructor (string networkName, NSData extendedPanId, NSData masterKey, NSData PSKc, ushort channel, NSData panId);

		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[Export ("asData")]
		NSData GetAsData ();
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "CHIPQRCodeSetupPayloadParser")]
	[DisableDefaultCtor]
	interface ChipQRCodeSetupPayloadParser {
		[Export ("initWithBase38Representation:")]
		NativeHandle Constructor (string base38Representation);

		[Export ("populatePayload:")]
		[return: NullAllowed]
		ChipSetupPayload PopulatePayload ([NullAllowed] out NSError error);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPAccountLogin")]
	[DisableDefaultCtor]
	interface ChipAccountLogin {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPAdministratorCommissioning")]
	[DisableDefaultCtor]
	interface ChipAdministratorCommissioning {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("openBasicCommissioningWindow:responseHandler:")]
		void OpenBasicCommissioningWindow (ushort commissioningTimeout, ChipResponseHandler responseHandler);

		// Parameter names are left to match header files. 
		// pAKEVerifier may relate to key derivation, but without documentation, we cannot know for certain.
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("openCommissioningWindow:pAKEVerifier:discriminator:iterations:salt:passcodeID:responseHandler:")]
		void OpenCommissioningWindow (ushort commissioningTimeout, NSData pAKEVerifier, ushort discriminator, uint iterations, NSData salt, ushort passcodeId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("revokeCommissioning:")]
		void RevokeCommissioning (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPApplicationLauncher")]
	[DisableDefaultCtor]
	interface ChipApplicationLauncher {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("launchApp:catalogVendorId:applicationId:responseHandler:")]
		void LaunchApp (string data, ushort catalogVendorId, string applicationId, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationLauncherListWithResponseHandler:")]
		void ReadAttributeApplicationLauncherList (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCatalogVendorIdWithResponseHandler:")]
		void ReadAttributeCatalogVendorId (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeApplicationIdWithResponseHandler:")]
		void ReadAttributeApplicationId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPAudioOutput")]
	[DisableDefaultCtor]
	interface ChipAudioOutput {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("renameOutput:name:responseHandler:")]
		void RenameOutput (byte index, string name, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("selectOutput:responseHandler:")]
		void SelectOutput (byte index, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAudioOutputListWithResponseHandler:")]
		void ReadAttributeAudioOutputList (ChipResponseHandler responseHandler);

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentAudioOutputWithResponseHandler:")]
		void ReadAttributeCurrentAudioOutput (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPBinaryInputBasic")]
	[DisableDefaultCtor]
	interface ChipBinaryInputBasic {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOutOfServiceWithResponseHandler:")]
		void ReadAttributeOutOfService (ChipResponseHandler responseHandler);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Wrap ("WriteAttributeOutOfService (Convert.ToBoolean(value), responseHandler)", IsVirtual = true)]
		void WriteAttributeOutOfService (byte value, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeOutOfServiceWithValue:responseHandler:")]
		void WriteAttributeOutOfService (bool boolValue, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePresentValueWithResponseHandler:")]
		void ReadAttributePresentValue (ChipResponseHandler responseHandler);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Wrap ("WriteAttributePresentValue (Convert.ToBoolean(value), responseHandler)", IsVirtual = true)]
		void WriteAttributePresentValue (byte value, ChipResponseHandler responseHandler);
#endif

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributePresentValueWithValue:responseHandler:")]
		void WriteAttributePresentValue (bool boolValue, ChipResponseHandler responseHandler);

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPBridgedDeviceBasic")]
	[DisableDefaultCtor]
	interface ChipBridgedDeviceBasic {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPContentLauncher")]
	[DisableDefaultCtor]
	interface ChipContentLauncher {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("launchContent:data:responseHandler:")]
		void LaunchContent (bool autoPlay, string data, ChipResponseHandler responseHandler);

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPElectricalMeasurement")]
	[DisableDefaultCtor]
	interface ChipElectricalMeasurement {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMeasurementTypeWithResponseHandler:")]
		void ReadAttributeMeasurementType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTotalActivePowerWithResponseHandler:")]
		void ReadAttributeTotalActivePower (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRmsVoltageWithResponseHandler:")]
		void ReadAttributeRmsVoltage (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRmsVoltageMinWithResponseHandler:")]
		void ReadAttributeRmsVoltageMin (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRmsVoltageMaxWithResponseHandler:")]
		void ReadAttributeRmsVoltageMax (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRmsCurrentWithResponseHandler:")]
		void ReadAttributeRmsCurrent (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRmsCurrentMinWithResponseHandler:")]
		void ReadAttributeRmsCurrentMin (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRmsCurrentMaxWithResponseHandler:")]
		void ReadAttributeRmsCurrentMax (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeActivePowerWithResponseHandler:")]
		void ReadAttributeActivePower (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeActivePowerMinWithResponseHandler:")]
		void ReadAttributeActivePowerMin (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeActivePowerMaxWithResponseHandler:")]
		void ReadAttributeActivePowerMax (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPEthernetNetworkDiagnostics")]
	[DisableDefaultCtor]
	interface ChipEthernetNetworkDiagnostics {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPFixedLabel")]
	[DisableDefaultCtor]
	interface ChipFixedLabel {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLabelListWithResponseHandler:")]
		void ReadAttributeLabelList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPFlowMeasurement")]
	[DisableDefaultCtor]
	interface ChipFlowMeasurement {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMeasuredValueWithResponseHandler:")]
		void ReadAttributeMeasuredValue (ChipResponseHandler responseHandler);

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPGeneralDiagnostics")]
	[DisableDefaultCtor]
	interface ChipGeneralDiagnostics {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPKeypadInput")]
	[DisableDefaultCtor]
	interface ChipKeypadInput {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("sendKey:responseHandler:")]
		void SendKey (byte keyCode, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPMediaInput")]
	[DisableDefaultCtor]
	interface ChipMediaInput {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentMediaInputWithResponseHandler:")]
		void ReadAttributeCurrentMediaInput (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPMediaPlayback")]
	[DisableDefaultCtor]
	interface ChipMediaPlayback {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaSeek:responseHandler:")]
		void MediaSeek (ulong position, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaSkipBackward:responseHandler:")]
		void SkipBackward (ulong deltaPositionMilliseconds, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("mediaSkipForward:responseHandler:")]
		void SkipForward (ulong deltaPositionMilliseconds, ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void SkipSeek (ulong position, ChipResponseHandler responseHandler);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPRelativeHumidityMeasurement")]
	[DisableDefaultCtor]
	interface ChipRelativeHumidityMeasurement {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPSoftwareDiagnostics")]
	[DisableDefaultCtor]
	interface ChipSoftwareDiagnostics {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPTargetNavigator")]
	[DisableDefaultCtor]
	interface ChipTargetNavigator {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPThermostatUserInterfaceConfiguration")]
	[DisableDefaultCtor]
	interface ChipThermostatUserInterfaceConfiguration {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTemperatureDisplayModeWithResponseHandler:")]
		void ReadAttributeTemperatureDisplayMode (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeTemperatureDisplayModeWithValue:responseHandler:")]
		void WriteAttributeTemperatureDisplayMode (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeKeypadLockoutWithResponseHandler:")]
		void ReadAttributeKeypadLockout (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeKeypadLockoutWithValue:responseHandler:")]
		void WriteAttributeKeypadLockout (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeScheduleProgrammingVisibilityWithResponseHandler:")]
		void ReadAttributeScheduleProgrammingVisibility (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("writeAttributeScheduleProgrammingVisibilityWithValue:responseHandler:")]
		void WriteAttributeScheduleProgrammingVisibility (byte value, ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPTvChannel")]
	[DisableDefaultCtor]
	interface ChipTvChannel {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

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

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPThreadNetworkDiagnostics")]
	[DisableDefaultCtor]
	interface ChipThreadNetworkDiagnostics {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("resetCounts:")]
		void ResetCounts (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeChannelWithResponseHandler:")]
		void ReadAttributeChannel (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRoutingRoleWithResponseHandler:")]
		void ReadAttributeRoutingRole (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNetworkNameWithResponseHandler:")]
		void ReadAttributeNetworkName (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePanIdWithResponseHandler:")]
		void ReadAttributePanId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeExtendedPanIdWithResponseHandler:")]
		void ReadAttributeExtendedPanId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeMeshLocalPrefixWithResponseHandler:")]
		void ReadAttributeMeshLocalPrefix (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOverrunCountWithResponseHandler:")]
		void ReadAttributeOverrunCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeNeighborTableListWithResponseHandler:")]
		void ReadAttributeNeighborTableList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRouteTableListWithResponseHandler:")]
		void ReadAttributeRouteTableList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePartitionIdWithResponseHandler:")]
		void ReadAttributePartitionId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWeightingWithResponseHandler:")]
		void ReadAttributeWeighting (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeDataVersionWithResponseHandler:")]
		void ReadAttributeDataVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeStableDataVersionWithResponseHandler:")]
		void ReadAttributeStableDataVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLeaderRouterIdWithResponseHandler:")]
		void ReadAttributeLeaderRouterId (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeDetachedRoleCountWithResponseHandler:")]
		void ReadAttributeDetachedRoleCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeChildRoleCountWithResponseHandler:")]
		void ReadAttributeChildRoleCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRouterRoleCountWithResponseHandler:")]
		void ReadAttributeRouterRoleCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeLeaderRoleCountWithResponseHandler:")]
		void ReadAttributeLeaderRoleCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeAttachAttemptCountWithResponseHandler:")]
		void ReadAttributeAttachAttemptCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributePartitionIdChangeCountWithResponseHandler:")]
		void ReadAttributePartitionIdChangeCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBetterPartitionAttachAttemptCountWithResponseHandler:")]
		void ReadAttributeBetterPartitionAttachAttemptCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeParentChangeCountWithResponseHandler:")]
		void ReadAttributeParentChangeCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxTotalCountWithResponseHandler:")]
		void ReadAttributeTxTotalCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxUnicastCountWithResponseHandler:")]
		void ReadAttributeTxUnicastCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxBroadcastCountWithResponseHandler:")]
		void ReadAttributeTxBroadcastCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxAckRequestedCountWithResponseHandler:")]
		void ReadAttributeTxAckRequestedCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxAckedCountWithResponseHandler:")]
		void ReadAttributeTxAckedCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxNoAckRequestedCountWithResponseHandler:")]
		void ReadAttributeTxNoAckRequestedCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxDataCountWithResponseHandler:")]
		void ReadAttributeTxDataCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxDataPollCountWithResponseHandler:")]
		void ReadAttributeTxDataPollCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxBeaconCountWithResponseHandler:")]
		void ReadAttributeTxBeaconCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxBeaconRequestCountWithResponseHandler:")]
		void ReadAttributeTxBeaconRequestCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxOtherCountWithResponseHandler:")]
		void ReadAttributeTxOtherCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxRetryCountWithResponseHandler:")]
		void ReadAttributeTxRetryCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxDirectMaxRetryExpiryCountWithResponseHandler:")]
		void ReadAttributeTxDirectMaxRetryExpiryCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxIndirectMaxRetryExpiryCountWithResponseHandler:")]
		void ReadAttributeTxIndirectMaxRetryExpiryCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxErrCcaCountWithResponseHandler:")]
		void ReadAttributeTxErrCcaCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxErrAbortCountWithResponseHandler:")]
		void ReadAttributeTxErrAbortCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTxErrBusyChannelCountWithResponseHandler:")]
		void ReadAttributeTxErrBusyChannelCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxTotalCountWithResponseHandler:")]
		void ReadAttributeRxTotalCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxUnicastCountWithResponseHandler:")]
		void ReadAttributeRxUnicastCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxBroadcastCountWithResponseHandler:")]
		void ReadAttributeRxBroadcastCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxDataCountWithResponseHandler:")]
		void ReadAttributeRxDataCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxDataPollCountWithResponseHandler:")]
		void ReadAttributeRxDataPollCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxBeaconCountWithResponseHandler:")]
		void ReadAttributeRxBeaconCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxBeaconRequestCountWithResponseHandler:")]
		void ReadAttributeRxBeaconRequestCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxOtherCountWithResponseHandler:")]
		void ReadAttributeRxOtherCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxAddressFilteredCountWithResponseHandler:")]
		void ReadAttributeRxAddressFilteredCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxDestAddrFilteredCountWithResponseHandler:")]
		void ReadAttributeRxDestAddrFilteredCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxDuplicatedCountWithResponseHandler:")]
		void ReadAttributeRxDuplicatedCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxErrNoFrameCountWithResponseHandler:")]
		void ReadAttributeRxErrNoFrameCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxErrUnknownNeighborCountWithResponseHandler:")]
		void ReadAttributeRxErrUnknownNeighborCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxErrInvalidSrcAddrCountWithResponseHandler:")]
		void ReadAttributeRxErrInvalidSrcAddrCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxErrSecCountWithResponseHandler:")]
		void ReadAttributeRxErrSecCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxErrFcsCountWithResponseHandler:")]
		void ReadAttributeRxErrFcsCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRxErrOtherCountWithResponseHandler:")]
		void ReadAttributeRxErrOtherCount (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSecurityPolicyWithResponseHandler:")]
		void ReadAttributeSecurityPolicy (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeChannelMaskWithResponseHandler:")]
		void ReadAttributeChannelMask (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOperationalDatasetComponentsWithResponseHandler:")]
		void ReadAttributeOperationalDatasetComponents (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeActiveNetworkFaultsListWithResponseHandler:")]
		void ReadAttributeActiveNetworkFaultsList (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPWakeOnLan")]
	[DisableDefaultCtor]
	interface ChipWakeOnLan {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWakeOnLanMacAddressWithResponseHandler:")]
		void ReadAttributeWakeOnLanMacAddress (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (ChipCluster), Name = "CHIPWiFiNetworkDiagnostics")]
	[DisableDefaultCtor]
	interface ChipWiFiNetworkDiagnostics {
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("resetCounts:")]
		void ResetCounts (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeBssidWithResponseHandler:")]
		void ReadAttributeBssid (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSecurityTypeWithResponseHandler:")]
		void ReadAttributeSecurityType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeWiFiVersionWithResponseHandler:")]
		void ReadAttributeWiFiVersion (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeChannelNumberWithResponseHandler:")]
		void ReadAttributeChannelNumber (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeRssiWithResponseHandler:")]
		void ReadAttributeRssi (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}

	[Obsoleted (PlatformName.iOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.TvOS, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.MacOSX, 13, 0, message: Constants.ChipRemoved)]
	[Obsoleted (PlatformName.WatchOS, 9, 0, message: Constants.ChipRemoved)]
	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ChipCluster), Name = "CHIPWindowCovering")]
	[DisableDefaultCtor]
	interface ChipWindowCovering {
		[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("initWithDevice:endpoint:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ChipDevice device, ushort endpoint, DispatchQueue queue);

#if !NET
		[Deprecated (PlatformName.iOS, 15, 2)]
		[Deprecated (PlatformName.TvOS, 15, 2)]
		[Deprecated (PlatformName.WatchOS, 8, 3)]
		[Deprecated (PlatformName.MacOSX, 12, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 2)]
		[Wrap ("this (device, (ushort) endpoint, queue)")]
		NativeHandle Constructor (ChipDevice device, byte endpoint, DispatchQueue queue);
#endif

#if !NET
		[Obsolete ("This method is removed, use 'DownOrClose' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void DownClose (ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void GoToLiftPercentage (byte percentageLiftValue, ChipResponseHandler responseHandler);
#endif

		[Internal]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringGoToLiftValue:responseHandler:")]
		void _OldGoToLiftValue (ushort liftValue, ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void GoToTiltPercentage (byte percentageTiltValue, ChipResponseHandler responseHandler);
#endif

		[Internal]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("windowCoveringGoToTiltValue:responseHandler:")]
		void _OldGoToTiltValue (ushort tiltValue, ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed, use 'StopMotion' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void Stop (ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed, use 'UpOrOpen' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void UpOpen (ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed, use 'ReadAttributeType' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ReadAttributeWindowCoveringType (ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ConfigureAttributeWindowCoveringType (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ReportAttributeWindowCoveringType (ChipResponseHandler responseHandler);
#endif

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("downOrClose:")]
		void DownOrClose (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("goToLiftPercentage:liftPercent100thsValue:responseHandler:")]
		void GoToLiftPercentage (byte liftPercentageValue, ushort liftPercent100thsValue, ChipResponseHandler responseHandler);

		[Internal]
		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("goToLiftValue:responseHandler:")]
		void _NewGoToLiftValue (ushort liftValue, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("goToTiltPercentage:tiltPercent100thsValue:responseHandler:")]
		void GoToTiltPercentage (byte tiltPercentageValue, ushort tiltPercent100thsValue, ChipResponseHandler responseHandler);

		[Internal]
		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("goToTiltValue:responseHandler:")]
		void _NewGoToTiltValue (ushort tiltValue, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("stopMotion:")]
		void StopMotion (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("upOrOpen:")]
		void UpOrOpen (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTypeWithResponseHandler:")]
		void ReadAttributeType (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionLiftWithResponseHandler:")]
		void ReadAttributeCurrentPositionLift (ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed, use 'ConfigureAttributeCurrentPositionLiftPercentage' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ConfigureAttributeCurrentPositionLift (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed, use 'ReportAttributeCurrentPositionLiftPercentage' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ReportAttributeCurrentPositionLift (ChipResponseHandler responseHandler);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionTiltWithResponseHandler:")]
		void ReadAttributeCurrentPositionTilt (ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed, use 'ConfigureAttributeCurrentPositionTiltPercentage' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ConfigureAttributeCurrentPositionTilt (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed, use 'ReportAttributeCurrentPositionTiltPercentage' instead.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ReportAttributeCurrentPositionTilt (ChipResponseHandler responseHandler);
#endif

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeConfigStatusWithResponseHandler:")]
		void ReadAttributeConfigStatus (ChipResponseHandler responseHandler);

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ConfigureAttributeConfigStatus (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);
#endif

#if !NET
		[Obsolete ("This method is removed.")]
		[Wrap ("throw new InvalidOperationException (Constants.RemovedFromChip)", IsVirtual = true)]
		[NoMac] // fails on macOS 12 beta 6
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		void ReportAttributeConfigStatus (ChipResponseHandler responseHandler);
#endif

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionLiftPercentageWithResponseHandler:")]
		void ReadAttributeCurrentPositionLiftPercentage (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionLiftPercentageWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPositionLiftPercentage (ushort minInterval, ushort maxInterval, byte change, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionLiftPercentageWithResponseHandler:")]
		void ReportAttributeCurrentPositionLiftPercentage (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionTiltPercentageWithResponseHandler:")]
		void ReadAttributeCurrentPositionTiltPercentage (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionTiltPercentageWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPositionTiltPercentage (ushort minInterval, ushort maxInterval, byte change, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionTiltPercentageWithResponseHandler:")]
		void ReportAttributeCurrentPositionTiltPercentage (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeOperationalStatusWithResponseHandler:")]
		void ReadAttributeOperationalStatus (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeOperationalStatusWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeOperationalStatus (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeOperationalStatusWithResponseHandler:")]
		void ReportAttributeOperationalStatus (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTargetPositionLiftPercent100thsWithResponseHandler:")]
		void ReadAttributeTargetPositionLiftPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeTargetPositionLiftPercent100thsWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeTargetPositionLiftPercent100ths (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeTargetPositionLiftPercent100thsWithResponseHandler:")]
		void ReportAttributeTargetPositionLiftPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeTargetPositionTiltPercent100thsWithResponseHandler:")]
		void ReadAttributeTargetPositionTiltPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeTargetPositionTiltPercent100thsWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeTargetPositionTiltPercent100ths (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeTargetPositionTiltPercent100thsWithResponseHandler:")]
		void ReportAttributeTargetPositionTiltPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeEndProductTypeWithResponseHandler:")]
		void ReadAttributeEndProductType (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionLiftPercent100thsWithResponseHandler:")]
		void ReadAttributeCurrentPositionLiftPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionLiftPercent100thsWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPositionLiftPercent100ths (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionLiftPercent100thsWithResponseHandler:")]
		void ReportAttributeCurrentPositionLiftPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeCurrentPositionTiltPercent100thsWithResponseHandler:")]
		void ReadAttributeCurrentPositionTiltPercent100ths (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeCurrentPositionTiltPercent100thsWithMinInterval:maxInterval:change:responseHandler:")]
		void ConfigureAttributeCurrentPositionTiltPercent100ths (ushort minInterval, ushort maxInterval, ushort change, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeCurrentPositionTiltPercent100thsWithResponseHandler:")]
		void ReportAttributeCurrentPositionTiltPercent100ths (ChipResponseHandler responseHandler);

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

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeSafetyStatusWithResponseHandler:")]
		void ReadAttributeSafetyStatus (ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("configureAttributeSafetyStatusWithMinInterval:maxInterval:responseHandler:")]
		void ConfigureAttributeSafetyStatus (ushort minInterval, ushort maxInterval, ChipResponseHandler responseHandler);

		[NoMac, Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("reportAttributeSafetyStatusWithResponseHandler:")]
		void ReportAttributeSafetyStatus (ChipResponseHandler responseHandler);

		[Async (ResultTypeName = "ChipReadAttributeResult")]
		[Export ("readAttributeClusterRevisionWithResponseHandler:")]
		void ReadAttributeClusterRevision (ChipResponseHandler responseHandler);
	}


}
#endif // !NET
