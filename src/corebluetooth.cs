//
// corebluetooth.cs: API definitions for CoreBluetooth
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2011-2013 Xamarin Inc
//
using ObjCRuntime;
using Foundation;
using System;
using CoreFoundation;

namespace CoreBluetooth {

	[Watch (4,0)]
	[iOS (8,0)]
	[Mac (10,13)]
	[BaseType (typeof (NSObject))]
	interface CBAttribute {
		[Export ("UUID")]
		CBUUID UUID { get; [NotImplemented] set;  }
	}

	[Watch (4,0)]
	[StrongDictionary ("CBCentralManager")]
	interface CBCentralInitOptions {
		[Export ("OptionShowPowerAlertKey")]
		bool ShowPowerAlert { get; set; }

#if !MONOMAC
		[Export ("OptionRestoreIdentifierKey")]
		string RestoreIdentifier { get; set; }
#endif
	}

	[Watch (4,0)]
	[iOS (10,0)]
	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CBManager {
		[Export ("state", ArgumentSemantic.Assign)]
		CBManagerState State { get; }
	}

	[Watch (4,0)]
	[Mac (10, 7)]
	[BaseType (typeof (CBManager), Delegates=new[] {"WeakDelegate"}, Events = new[] { typeof (CBCentralManagerDelegate)})]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBCentralManager {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		CBCentralManagerDelegate Delegate { get; set; }
		
		[Mac (10, 7, onlyOn64: true)] // Was removed from 32-bit in 10.13 unannounced
		[Export ("initWithDelegate:queue:")]
		[PostGet ("WeakDelegate")]
		IntPtr Constructor ([NullAllowed, Protocolize] CBCentralManagerDelegate centralDelegate, [NullAllowed] DispatchQueue queue);

		[DesignatedInitializer]
		[iOS (7,0), Mac (10,9)]
		[Export ("initWithDelegate:queue:options:")]
		[PostGet ("WeakDelegate")]
		IntPtr Constructor ([NullAllowed, Protocolize] CBCentralManagerDelegate centralDelegate, [NullAllowed] DispatchQueue queue, [NullAllowed] NSDictionary options);

		[iOS (7,0), Mac (10,9)]
		[Wrap ("this (centralDelegate, queue, options == null ? null : options.Dictionary)")]
		IntPtr Constructor ([NullAllowed, Protocolize] CBCentralManagerDelegate centralDelegate, [NullAllowed] DispatchQueue queue, CBCentralInitOptions options);

		[NoTV]
		[NoWatch]
		[Availability (Obsoleted = Platform.iOS_9_0)]
		[Mac (10, 7, onlyOn64: true)] // Was removed from 32-bit in 10.13 unannounced
		[Export ("retrievePeripherals:"), Internal]
		void RetrievePeripherals (NSArray peripheralUUIDs);

		[NoTV]
		[NoWatch]
		[Mac (10, 7, onlyOn64: true)] // Was removed from 32-bit in 10.13 unannounced
		[Export ("retrieveConnectedPeripherals")]
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_9_0, Message = "Use 'RetrievePeripheralsWithIdentifiers' instead.")]
		void RetrieveConnectedPeripherals ();

		[Export ("scanForPeripheralsWithServices:options:"), Internal]
		void ScanForPeripherals ([NullAllowed] NSArray serviceUUIDs, [NullAllowed] NSDictionary options);

		[Export ("stopScan")]
		void StopScan ();

		[Export ("connectPeripheral:options:")]
		void ConnectPeripheral (CBPeripheral peripheral, [NullAllowed] NSDictionary options);

		[Export ("cancelPeripheralConnection:")]
		void CancelPeripheralConnection (CBPeripheral peripheral);

		[Field ("CBCentralManagerScanOptionAllowDuplicatesKey")]
		NSString ScanOptionAllowDuplicatesKey { get; }

		[Field ("CBConnectPeripheralOptionNotifyOnDisconnectionKey")]
		NSString OptionNotifyOnDisconnectionKey { get; }

		[Mac (10,13)]
		[iOS (6,0)]
		[Field ("CBConnectPeripheralOptionNotifyOnConnectionKey")]
		NSString OptionNotifyOnConnectionKey { get; }

		[Mac (10,13)]
		[iOS (6,0)]
		[Field ("CBConnectPeripheralOptionNotifyOnNotificationKey")]
		NSString OptionNotifyOnNotificationKey { get; }

		[Mac (10,14)]
		[iOS (11,2)][TV (11,2)][Watch (4,2)]
		[Field ("CBConnectPeripheralOptionStartDelayKey")]
		NSString OptionStartDelayKey { get; }

		[Field ("CBCentralManagerOptionRestoreIdentifierKey")]
		[iOS (7,0)]
		[Mac (10,13)]
		NSString OptionRestoreIdentifierKey { get; }

		[Field ("CBCentralManagerRestoredStatePeripheralsKey")]
		[iOS (7,0)]
		[Mac (10,13)]
		NSString RestoredStatePeripheralsKey { get; }

		[Field ("CBCentralManagerRestoredStateScanServicesKey")]
		[iOS (7,0)]
		[Mac (10,13)]
		NSString RestoredStateScanServicesKey { get; }

		[Field ("CBCentralManagerRestoredStateScanOptionsKey")]
		[iOS (7,0)]
		[Mac (10,13)]
		NSString RestoredStateScanOptionsKey { get; }

		[iOS (7,0), Mac (10,9)]
		[Export ("retrievePeripheralsWithIdentifiers:")]
		CBPeripheral [] RetrievePeripheralsWithIdentifiers ([Params] NSUuid [] identifiers);

		[iOS (7,0), Mac (10,9)]
		[Export ("retrieveConnectedPeripheralsWithServices:")]
		CBPeripheral [] RetrieveConnectedPeripherals ([Params] CBUUID []  serviceUUIDs);

		[Field ("CBCentralManagerOptionShowPowerAlertKey")]
		[iOS (7,0), Mac (10,9)]
		NSString OptionShowPowerAlertKey { get; }

		[Field ("CBCentralManagerScanOptionSolicitedServiceUUIDsKey")]
		[iOS (7,0), Mac (10,9)]
		NSString ScanOptionSolicitedServiceUUIDsKey { get; }

		[iOS (9,0)]
		[Mac (10,13)]
		[Export ("isScanning")]
		bool IsScanning { get; }
	}

	[Watch (4,0)]
	[StrongDictionary ("AdvertisementDataKeys")]
	interface AdvertisementData {
		string LocalName { get; set; }
		NSData ManufacturerData { get; set; }
		
#if XAMCORE_2_0
		NSDictionary <CBUUID, NSData> ServiceData { get; set; }
#else
		NSDictionary ServiceData { get; set; }
#endif
		CBUUID [] ServiceUuids { get; set; }
		CBUUID [] OverflowServiceUuids { get; set; }
		NSNumber TxPowerLevel { get; set; }
		bool IsConnectable { get; set; }
		CBUUID [] SolicitedServiceUuids { get; set; }
	}

	[Watch (4,0)]
	[Static, Internal]
	interface AdvertisementDataKeys {
		[Field ("CBAdvertisementDataLocalNameKey")]
		NSString LocalNameKey { get; }

		[Field ("CBAdvertisementDataManufacturerDataKey")]
		NSString ManufacturerDataKey { get; }

		[Field ("CBAdvertisementDataServiceDataKey")]
		NSString ServiceDataKey { get; }

		[Field ("CBAdvertisementDataServiceUUIDsKey")]
		NSString ServiceUuidsKey { get; }

		[Field ("CBAdvertisementDataOverflowServiceUUIDsKey")]
		NSString OverflowServiceUuidsKey { get; }

		[Field ("CBAdvertisementDataTxPowerLevelKey")]
		NSString TxPowerLevelKey { get; }

		[iOS (7,0), Mac (10,9)]
		[Field ("CBAdvertisementDataIsConnectable")]
		NSString IsConnectableKey { get; }

		[iOS (7,0), Mac (10,9)]
		[Field ("CBAdvertisementDataSolicitedServiceUUIDsKey")]
		NSString SolicitedServiceUuidsKey { get; }
	}

	[Watch (4,0)]
	[StrongDictionary ("PeripheralScanningOptionsKeys")]
	interface PeripheralScanningOptions { }

	[Watch (4,0)]
	[StrongDictionary ("RestoredStateKeys")]
	interface RestoredState {
		CBPeripheral [] Peripherals { get; set; }
		CBPeripheral [] ScanServices { get; set; }
		PeripheralScanningOptions ScanOptions { get; set; }
	}

	[Watch (4,0)]
	[Static, Internal]
	interface RestoredStateKeys {
		[iOS (7,0)]
		[Field ("CBCentralManagerRestoredStatePeripheralsKey")]
		NSString PeripheralsKey { get; }

		[iOS (7,0)]
		[Field ("CBCentralManagerRestoredStateScanServicesKey")]
		NSString ScanServicesKey { get; }

		[iOS (7,0)]
		[Field ("CBCentralManagerRestoredStateScanOptionsKey")]
		NSString ScanOptionsKey { get; }
	}

	[Watch (4,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CBCentralManagerDelegate {
		[Abstract]
		[Export ("centralManagerDidUpdateState:")]
		void UpdatedState (CBCentralManager central);

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_8_4)] // Available in iOS 5.0 through iOS 8.4. Deprecated in iOS 7.0.
		[Export ("centralManager:didRetrievePeripherals:"), EventArgs ("CBPeripherals")]
		void RetrievedPeripherals (CBCentralManager central, CBPeripheral [] peripherals);

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_8_4)] // Available in iOS 5.0 through iOS 8.4. Deprecated in iOS 7.0.
		[Export ("centralManager:didRetrieveConnectedPeripherals:"), EventArgs ("CBPeripherals")]
		void RetrievedConnectedPeripherals (CBCentralManager central, CBPeripheral [] peripherals);

		[Export ("centralManager:didDiscoverPeripheral:advertisementData:RSSI:"), EventArgs ("CBDiscoveredPeripheral")]
		void DiscoveredPeripheral (CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI);

		[Export ("centralManager:didConnectPeripheral:"), EventArgs ("CBPeripheral")]
		void ConnectedPeripheral (CBCentralManager central, CBPeripheral peripheral);

		[Export ("centralManager:didFailToConnectPeripheral:error:"), EventArgs ("CBPeripheralError")]
		void FailedToConnectPeripheral (CBCentralManager central, CBPeripheral peripheral, NSError error);

		[Export ("centralManager:didDisconnectPeripheral:error:"), EventArgs ("CBPeripheralError")]
		void DisconnectedPeripheral (CBCentralManager central, CBPeripheral peripheral, NSError error);
		
		[Export ("centralManager:willRestoreState:"), EventArgs ("CBWillRestore")]
		void WillRestoreState (CBCentralManager central, NSDictionary dict);
	}

	[Watch (4,0)]
	[Static]
	interface CBAdvertisement {
		[Field ("CBAdvertisementDataServiceUUIDsKey")]
		NSString DataServiceUUIDsKey { get; }
		
		[Field ("CBAdvertisementDataLocalNameKey")]
		NSString DataLocalNameKey { get; }
		
		[Field ("CBAdvertisementDataTxPowerLevelKey")]
		NSString DataTxPowerLevelKey { get; }
		
		[Field ("CBAdvertisementDataManufacturerDataKey")]
		NSString DataManufacturerDataKey { get; }
		
		[Field ("CBAdvertisementDataServiceDataKey")]
		NSString DataServiceDataKey { get; }

		[iOS (6,0), Mac (10,9)]
		[Field ("CBAdvertisementDataOverflowServiceUUIDsKey")]
		NSString DataOverflowServiceUUIDsKey { get; }

		[iOS (7,0), Mac (10,9)]
		[Field ("CBAdvertisementDataIsConnectable")]
		NSString IsConnectable { get; }

		[iOS (7,0), Mac (10,9)]
		[Field ("CBAdvertisementDataSolicitedServiceUUIDsKey")]
		NSString DataSolicitedServiceUUIDsKey { get; }

	}

	[Watch (4,0)]
	[BaseType (typeof (CBAttribute))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBCharacteristic {
		
		[Export ("properties")]
		CBCharacteristicProperties Properties { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableCharacteristic")] set; }

		[Export ("value", ArgumentSemantic.Retain)]
		NSData Value { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableCharacteristic")] set;  }

		[Export ("descriptors", ArgumentSemantic.Retain)]
		CBDescriptor [] Descriptors { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableCharacteristic")] set; }

		[Deprecated (PlatformName.iOS, 8,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[Export ("isBroadcasted")]
		bool IsBroadcasted { get;  }

		[Export ("isNotifying")]
		bool IsNotifying { get;  }

		[Export ("service", ArgumentSemantic.Weak)]
		CBService Service { get; }

#if !XAMCORE_2_0
		[iOS (7,0), Export ("subscribedCentrals"), Mac (10,9)]
		CBCentral [] SubscribedCentrals { get; }
#endif
	}

	[Watch (4,0)]
	[iOS (6, 0), Mac (10,9)]
	[BaseType (typeof (CBCharacteristic))]
	[DisableDefaultCtor]
	interface CBMutableCharacteristic {

		[NoTV]
		[NoWatch]
		[DesignatedInitializer]
		[Export ("initWithType:properties:value:permissions:")]
		[PostGet ("UUID")]
		[PostGet ("Value")]
		IntPtr Constructor (CBUUID uuid, CBCharacteristicProperties properties, [NullAllowed] NSData value, CBAttributePermissions permissions);

		[Export ("permissions", ArgumentSemantic.Assign)]
		CBAttributePermissions Permissions { get; set; }

		[NoTV]
		[NoWatch]
		[NullAllowed]
		[Export ("UUID", ArgumentSemantic.Retain)]
		[Override]
		CBUUID UUID { get; [Availability (Obsoleted = Platform.Mac_10_13)] set; }

		[Export ("properties", ArgumentSemantic.Assign)]
		[Override]
		CBCharacteristicProperties Properties { get; set; }

		[NullAllowed]
		[Export ("value", ArgumentSemantic.Retain)]
		[Override]
		NSData Value { get; set; }

		[NullAllowed]
		[Export ("descriptors", ArgumentSemantic.Retain)]
		[Override]
		CBDescriptor [] Descriptors { get; set; }

#if XAMCORE_2_0
		[iOS (7,0), Export ("subscribedCentrals"), Mac (10,9)]
		CBCentral [] SubscribedCentrals { get; }
#endif
	}

	[Watch (4,0)]
	[BaseType (typeof (CBAttribute))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBDescriptor {
		
		[Export ("value", ArgumentSemantic.Retain)]
		NSObject Value { get;  }

		[Export ("characteristic", ArgumentSemantic.Weak)]
		CBCharacteristic Characteristic { get; }
	}

	[Watch (4,0)]
	[iOS (6, 0), Mac (10,9)]
	[BaseType (typeof (CBDescriptor))]
	[DisableDefaultCtor]
	interface CBMutableDescriptor {
		[NoTV]
		[NoWatch]
		[DesignatedInitializer]
		[Export ("initWithType:value:")]
		[PostGet ("UUID")]
		[PostGet ("Value")]
		IntPtr Constructor (CBUUID uuid, NSObject descriptorValue);
	}

	[Watch (4,0)]
	[BaseType (typeof (CBPeer), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof (CBPeripheralDelegate)})]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBPeripheral : NSCopying {
		[Export ("name", ArgumentSemantic.Retain)]
		[DisableZeroCopy]
		string Name { get;  }

		[Deprecated (PlatformName.iOS, 8,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[NoWatch]
		[Export ("RSSI", ArgumentSemantic.Retain)]
		NSNumber RSSI { get;  }

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_9_0)]
		[Export ("isConnected")]
		[Mac (10, 7, onlyOn64: true)] // Was removed from 32-bit in 10.13 unannounced
		bool IsConnected { get;  }

		[Export ("services", ArgumentSemantic.Retain)]
		CBService [] Services { get;  }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		CBPeripheralDelegate Delegate { get; set; }
		
		[Export ("readRSSI")]
		void ReadRSSI ();

		[Export ("discoverServices:"), Internal]
		void DiscoverServices ([NullAllowed] NSArray serviceUUIDs);

		[Export ("discoverIncludedServices:forService:"), Internal]
		void DiscoverIncludedServices ([NullAllowed] NSArray includedServiceUUIDs, CBService forService);

		[Export ("discoverCharacteristics:forService:"), Internal]
		void DiscoverCharacteristics ([NullAllowed] NSArray characteristicUUIDs, CBService forService);

		[Export ("readValueForCharacteristic:")]
		void ReadValue (CBCharacteristic characteristic);

		[Export ("writeValue:forCharacteristic:type:")]
		void WriteValue (NSData data, CBCharacteristic characteristic, CBCharacteristicWriteType type);

		[Export ("setNotifyValue:forCharacteristic:")]
		void SetNotifyValue (bool enabled, CBCharacteristic characteristic);

		[Export ("discoverDescriptorsForCharacteristic:")]
		void DiscoverDescriptors (CBCharacteristic characteristic);

		[Export ("readValueForDescriptor:")]
		void ReadValue (CBDescriptor descriptor);

		[Export ("writeValue:forDescriptor:")]
		void WriteValue (NSData data, CBDescriptor descriptor);

		[iOS (9,0)][Mac (10,12)]
		[Export ("maximumWriteValueLengthForType:")]
		nuint GetMaximumWriteValueLength (CBCharacteristicWriteType type);

#if !XAMCORE_2_0
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_9_0)]
		[Export ("UUID")]
		System.IntPtr UUID { get; }
#endif

		[iOS (7,0), Mac (10,9)]
		[Export ("state")]
		CBPeripheralState State { get; }

		[iOS (11,0)][TV (11,0)][Mac (10,13, onlyOn64: true)]
		[Export ("canSendWriteWithoutResponse")]
		bool CanSendWriteWithoutResponse { get; }

		[iOS (11,0)][TV (11,0)][Mac (10,13, onlyOn64: true)]
		[Export ("openL2CAPChannel:")]
		void OpenL2CapChannel (ushort psm);
	}

	[Watch (4,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CBPeripheralDelegate {
		[Availability (Deprecated=Platform.iOS_8_0, Message="Use 'RssiRead' instead.")]
		[Export ("peripheralDidUpdateRSSI:error:"), EventArgs ("NSError", true)]
		void RssiUpdated (CBPeripheral peripheral, NSError error);

		[iOS (8,0)]
		[Mac (10,13)]
		[Export ("peripheral:didReadRSSI:error:"), EventArgs ("CBRssi")]
		void RssiRead (CBPeripheral peripheral, NSNumber rssi, NSError error);

		// FIXME: TYPO: missing 's' (plural)
		[Export ("peripheral:didDiscoverServices:"), EventArgs ("NSError", true)]
		void DiscoveredService (CBPeripheral peripheral, NSError error);

		[Export ("peripheral:didDiscoverIncludedServicesForService:error:"), EventArgs ("CBService")]
		void DiscoveredIncludedService  (CBPeripheral peripheral, CBService service, NSError error);

		[Export ("peripheral:didDiscoverCharacteristicsForService:error:"), EventArgs ("CBService")]
#if XAMCORE_2_0
		// FIXME: TYPO: missing 's' (plural)
		void DiscoveredCharacteristic (CBPeripheral peripheral, CBService service, NSError error);
#else
		void DiscoverCharacteristic (CBPeripheral peripheral, CBService service, NSError error);
#endif
		
		[Export ("peripheral:didUpdateValueForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void UpdatedCharacterteristicValue (CBPeripheral peripheral, CBCharacteristic characteristic, NSError error);
		 
		[Export ("peripheral:didWriteValueForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void WroteCharacteristicValue (CBPeripheral peripheral, CBCharacteristic characteristic, NSError error);

		[Export ("peripheral:didUpdateNotificationStateForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void UpdatedNotificationState (CBPeripheral peripheral, CBCharacteristic characteristic, NSError error);

		[Export ("peripheral:didDiscoverDescriptorsForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void DiscoveredDescriptor (CBPeripheral peripheral, CBCharacteristic characteristic, NSError error);

		[Export ("peripheral:didUpdateValueForDescriptor:error:"), EventArgs ("CBDescriptor")]
		void UpdatedValue (CBPeripheral peripheral, CBDescriptor descriptor, NSError error);

		[Export ("peripheral:didWriteValueForDescriptor:error:"), EventArgs ("CBDescriptor")]
		void WroteDescriptorValue (CBPeripheral peripheral, CBDescriptor descriptor, NSError error);

		[NoTV]
		[NoWatch]
		[Availability (Introduced = Platform.iOS_6_0, Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_8_4)]
		[Export ("peripheralDidInvalidateServices:")]
		void InvalidatedService (CBPeripheral peripheral);	

		[iOS (6, 0)]
		[Export ("peripheralDidUpdateName:")]
		void UpdatedName (CBPeripheral peripheral);

		[iOS (7,0)]
		[Export ("peripheral:didModifyServices:"), EventArgs ("CBPeripheralServices")]
		void ModifiedServices (CBPeripheral peripheral, CBService [] services);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[EventArgs ("CBPeripheralOpenL2CapChannel")]
		[Export ("peripheral:didOpenL2CAPChannel:error:")]
		void DidOpenL2CapChannel (CBPeripheral peripheral, [NullAllowed] CBL2CapChannel channel, [NullAllowed] NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Export ("peripheralIsReadyToSendWriteWithoutResponse:")]
		void IsReadyToSendWriteWithoutResponse (CBPeripheral peripheral);
	}

	[Watch (4,0)]
	[BaseType (typeof (CBAttribute))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBService {
		[iOS (6,0), Mac (10,9)]
		[Export ("isPrimary")]
		bool Primary { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableService")] set; }

		[Export ("includedServices", ArgumentSemantic.Retain)]
		CBService [] IncludedServices { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableService")] set;  }

		[Export ("characteristics", ArgumentSemantic.Retain)]
		CBCharacteristic [] Characteristics { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableService")] set;  }

		[Export ("peripheral", ArgumentSemantic.Weak)]
		CBPeripheral Peripheral { get; }

	}
		
	[Watch (4,0)]
	[iOS (6, 0), Mac(10,9)]
	[BaseType (typeof (CBService))]
	[DisableDefaultCtor]
	interface CBMutableService {
		[NoTV]
		[NoWatch]
		[DesignatedInitializer]
		[Export ("initWithType:primary:")]
		[PostGet ("UUID")]
		IntPtr Constructor (CBUUID uuid, bool primary);

		[NoTV]
		[NoWatch]
		[Export ("UUID", ArgumentSemantic.Retain)]
		[Override]
		CBUUID UUID { get; [Availability (Obsoleted = Platform.Mac_10_13)] set; }

		[NoTV]
		[NoWatch]
		[Export ("isPrimary")]
		[Override]
		bool Primary { get; set; }

		[Export ("includedServices", ArgumentSemantic.Retain)]
		[Override]
		CBService[] IncludedServices { get; set; }  // TODO: check array type

		[Export ("characteristics", ArgumentSemantic.Retain)]
		[Override]
		CBCharacteristic[] Characteristics { get; set; }	// TODO: check array type
	}

	[Watch (4,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBUUID : NSCopying {
		[Export ("data")]
		NSData Data{ get; }

		[Static]
		[MarshalNativeExceptions]
		[Export ("UUIDWithString:")]
		CBUUID FromString (string theString);

		[Static]
		[Export ("UUIDWithData:")]
		CBUUID FromData (NSData theData);

		[Availability (Deprecated = Platform.iOS_9_0)]
		[NoWatch]
		[Static]
		[Export ("UUIDWithCFUUID:")]
		CBUUID FromCFUUID (IntPtr theUUID);
		
		[Static]
		[iOS (7,0), Mac (10,9)]
		[Export ("UUIDWithNSUUID:")]
		CBUUID FromNSUuid (NSUuid theUUID);

#if !XAMCORE_3_0
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDGenericAccessProfileString")]
		NSString GenericAccessProfileString { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDGenericAttributeProfileString")]
		NSString GenericAttributeProfileString { get; }
#endif

		[Field ("CBUUIDCharacteristicExtendedPropertiesString")]
		NSString CharacteristicExtendedPropertiesString { get; }

		[Field ("CBUUIDCharacteristicUserDescriptionString")]
		NSString CharacteristicUserDescriptionString { get; }

		[Field ("CBUUIDClientCharacteristicConfigurationString")]
		NSString ClientCharacteristicConfigurationString { get; }

		[Field ("CBUUIDServerCharacteristicConfigurationString")]
		NSString ServerCharacteristicConfigurationString { get; }

		[Field ("CBUUIDCharacteristicFormatString")]
		NSString CharacteristicFormatString { get; }

		[Field ("CBUUIDCharacteristicAggregateFormatString")]
		NSString CharacteristicAggregateFormatString { get; }

#if MONOMAC
		[Internal]
		[Field ("CBUUIDValidRangeString")]
		NSString CBUUIDValidRangeString { get; }

		[Internal]
		[Mac (10,13)]
		[Field ("CBUUIDCharacteristicValidRangeString")]
		NSString CBUUIDCharacteristicValidRangeString { get; }
#else
		[iOS (10,0)]
		[TV (10,0)]
		[Field ("CBUUIDCharacteristicValidRangeString")]
		NSString CharacteristicValidRangeString { get; }
#endif

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Field ("CBUUIDL2CAPPSMCharacteristicString")]
		NSString L2CapPsmCharacteristicString { get; }

#if !XAMCORE_3_0
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDDeviceNameString")]
		NSString DeviceNameString { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDAppearanceString")]
		NSString AppearanceString { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDPeripheralPrivacyFlagString")]
		NSString PeripheralPrivacyFlagString { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDReconnectionAddressString")]
		NSString ReconnectionAddressString { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDPeripheralPreferredConnectionParametersString")]
		NSString PeripheralPreferredConnectionParametersString { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Field ("CBUUIDServiceChangedString")]
		NSString ServiceChangedString { get; }
#endif // !XAMCORE_3_0

		[iOS (7,1)][Mac (10,10)]
		[Export ("UUIDString")]
		string Uuid { get; }
	}
		
	[Watch (4,0)]
	[iOS (6,0), Mac(10,9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CBATTRequest {
		[Export ("central", ArgumentSemantic.Retain)]
		CBCentral Central { get; }

		[Export ("characteristic", ArgumentSemantic.Retain)]
		CBCharacteristic Characteristic { get; }

		[Export ("offset")]
		nint Offset { get; }

		[Export ("value", ArgumentSemantic.Copy)]
		NSData Value { get; set; }		
	}

	[Mac (10,9)]
	[Watch (4,0)]
	[iOS (6,0)]
	[BaseType (typeof (CBPeer))]
	// `delloc` a default instance crash applications and a default instance, without the ability to change the UUID, does not make sense
	[DisableDefaultCtor]
	interface CBCentral : NSCopying {
#if !XAMCORE_2_0
		[Export ("UUID")]
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_9_0)]
		IntPtr UUID { get; }
#endif

#if MONOMAC
		// Introduced with iOS7, but does not have NS_AVAILABLE
		// Moved to a new base class, CBPeer, in iOS 8.
		[iOS (7,0)]
		[Export ("identifier")]
		NSUuid Identifier { get; }
#endif

		// Introduced with iOS7, but does not have NS_AVAILABLE
		[iOS (7,0)]
		[Export ("maximumUpdateValueLength")]
		nuint MaximumUpdateValueLength { get; }
	}

	[Watch (4,0)]
	[iOS (6, 0), Mac(10,9)]
	[DisableDefaultCtor]
	[BaseType (typeof (CBManager), Delegates=new[] { "WeakDelegate" }, Events=new[] { typeof (CBPeripheralManagerDelegate) })]
	interface CBPeripheralManager {

		[Mac (10,9, onlyOn64: true)]  // Was removed from 32-bit in 10.13 unannounced
		[Export ("init")]
		IntPtr Constructor ();

		[NoTV]
		[NoWatch]
		[Export ("initWithDelegate:queue:")]
		[PostGet ("WeakDelegate")]
		IntPtr Constructor ([Protocolize] CBPeripheralManagerDelegate peripheralDelegate, [NullAllowed] DispatchQueue queue);

		[NoTV]
		[NoWatch]
		[DesignatedInitializer]
		[iOS (7,0),Mac (10,9)]
		[Export ("initWithDelegate:queue:options:")]
		[PostGet ("WeakDelegate")]
		IntPtr Constructor ([Protocolize] CBPeripheralManagerDelegate peripheralDelegate, [NullAllowed] DispatchQueue queue, [NullAllowed] NSDictionary options);

		[NullAllowed]
		[Wrap ("WeakDelegate")]
		[Protocolize]
		CBPeripheralManagerDelegate Delegate { get; set; }

		[NullAllowed]
		[Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("isAdvertising")]
		bool Advertising { get; }

		[Export ("addService:")]
		void AddService (CBMutableService service);

		[Export ("removeService:")]
		void RemoveService (CBMutableService service);

		[Export ("removeAllServices")]
		void RemoveAllServices ();

		[Export ("respondToRequest:withResult:")]
		void RespondToRequest (CBATTRequest request, CBATTError result); // TODO: Could it return CBATTError?. This won't work because it's a value

		[Export ("startAdvertising:")]
		void StartAdvertising ([NullAllowed] NSDictionary options);

		[Wrap ("StartAdvertising (options == null ? null : options.Dictionary)")]
		void StartAdvertising ([NullAllowed] StartAdvertisingOptions options);

		[Export ("stopAdvertising")]
		void StopAdvertising ();

		[Export ("setDesiredConnectionLatency:forCentral:")]
		void SetDesiredConnectionLatency (CBPeripheralManagerConnectionLatency latency, CBCentral connectedCentral);

		[Export ("updateValue:forCharacteristic:onSubscribedCentrals:")]
		bool UpdateValue (NSData value, CBMutableCharacteristic characteristic, [NullAllowed] CBCentral[] subscribedCentrals);

		[iOS (11,0)][TV (11,0)][Mac (10,13, onlyOn64: true)]
		[Export ("publishL2CAPChannelWithEncryption:")]
		void PublishL2CapChannel (bool encryptionRequired);

		[iOS (11,0)][TV (11,0)][Mac (10,13, onlyOn64: true)]
		[Export ("unpublishL2CAPChannel:")]
		void UnpublishL2CapChannel (ushort psm);

		[Field ("CBPeripheralManagerOptionShowPowerAlertKey")]
		[iOS (7,0)]
		NSString OptionShowPowerAlertKey { get; }

		[Field ("CBPeripheralManagerOptionRestoreIdentifierKey")]
		[iOS (7,0)]
		NSString OptionRestoreIdentifierKey { get; }

		[Field ("CBPeripheralManagerRestoredStateServicesKey")]
		[iOS (7,0)]
		NSString RestoredStateServicesKey { get; }

		[Field ("CBPeripheralManagerRestoredStateAdvertisementDataKey")]
		[iOS (7,0)]
		NSString RestoredStateAdvertisementDataKey { get; }

#if !MONOMAC || !XAMCORE_4_0
		[iOS (7,0)]
		[Static]
		[Export ("authorizationStatus")]
		CBPeripheralManagerAuthorizationStatus AuthorizationStatus { get; }
#endif
	}

	[Watch (4,0)]
	[iOS (6, 0), Mac(10,9)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CBPeripheralManagerDelegate {
		[Abstract]
		[Export ("peripheralManagerDidUpdateState:")]
		void StateUpdated (CBPeripheralManager peripheral);

		[Export ("peripheralManager:willRestoreState:"), EventArgs ("CBWillRestore")]
		void WillRestoreState (CBPeripheralManager peripheral, NSDictionary dict);

		[Export ("peripheralManagerDidStartAdvertising:error:"), EventArgs ("NSError", true)]
		void AdvertisingStarted (CBPeripheralManager peripheral, NSError error);

		[Export ("peripheralManager:didAddService:error:"), EventArgs ("CBPeripheralManagerService")]
		void ServiceAdded (CBPeripheralManager peripheral, CBService service, NSError error);

		[Export ("peripheralManager:central:didSubscribeToCharacteristic:"), EventArgs ("CBPeripheralManagerSubscription")]
		void CharacteristicSubscribed (CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic);

		[Export ("peripheralManager:central:didUnsubscribeFromCharacteristic:"), EventArgs ("CBPeripheralManagerSubscription")]
		void CharacteristicUnsubscribed (CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic);

		[Export ("peripheralManager:didReceiveReadRequest:"), EventArgs ("CBATTRequest")]
		void ReadRequestReceived (CBPeripheralManager peripheral, CBATTRequest request);

		[Export ("peripheralManager:didReceiveWriteRequests:"), EventArgs ("CBATTRequests")]
		void WriteRequestsReceived (CBPeripheralManager peripheral, CBATTRequest[] requests);

		[Export ("peripheralManagerIsReadyToUpdateSubscribers:")]
		void ReadyToUpdateSubscribers (CBPeripheralManager peripheral);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[EventArgs ("CBPeripheralManagerOpenL2CapChannel")]
		[Export ("peripheralManager:didOpenL2CAPChannel:error:")]
		void DidOpenL2CapChannel (CBPeripheralManager peripheral, [NullAllowed] CBL2CapChannel channel, [NullAllowed] NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[EventArgs ("CBPeripheralManagerL2CapChannelOperation")]
		[Export ("peripheralManager:didUnpublishL2CAPChannel:error:")]
		void DidUnpublishL2CapChannel (CBPeripheralManager peripheral, ushort psm, [NullAllowed] NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[EventArgs ("CBPeripheralManagerL2CapChannelOperation")]
		[Export ("peripheralManager:didPublishL2CAPChannel:error:")]
		void DidPublishL2CapChannel (CBPeripheralManager peripheral, ushort psm, [NullAllowed] NSError error);
	}

	[iOS (8, 0)]
	[Mac (10,13)]
	[Watch (4,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // CBPeer.h: - (instancetype)init NS_UNAVAILABLE;
	interface CBPeer : NSCopying {
		[Internal]
		[NoTV]
		[NoWatch]
		[NoMac]
		[Availability (Deprecated = Platform.iOS_7_0, Obsoleted = Platform.iOS_9_0)]
		[Export ("UUID")]
		IntPtr _UUID { get;  }
	
		[iOS (7, 0)]
		[Export ("identifier")]
		NSUuid Identifier { get; }
	}

	// The type is available in 32bits macOS 10.13 even if most properties are 64 bits only
	[Watch (4,0)][iOS (11,0)][TV (11,0)][Mac (10,13)]
	[BaseType (typeof (NSObject), Name = "CBL2CAPChannel")]
	interface CBL2CapChannel {
		[Mac (10,13, onlyOn64: true)]
		[Export ("peer")]
		CBPeer Peer { get; }

		[Mac (10,13, onlyOn64: true)]
		[Export ("inputStream")]
		NSInputStream InputStream { get; }

		[Mac (10,13, onlyOn64: true)]
		[Export ("outputStream")]
		NSOutputStream OutputStream { get; }

		[Mac (10,13, onlyOn64: true)]
		[Export ("PSM")]
		/* uint16_t */ ushort Psm { get; }
	}
}
