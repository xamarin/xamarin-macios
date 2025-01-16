//
// corebluetooth.cs: API definitions for CoreBluetooth
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2011-2013 Xamarin Inc
//
using System.ComponentModel;

using ObjCRuntime;
using Foundation;
using System;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreBluetooth {

	/// <summary>Class that uniquely identifies a Bluetooth device.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreBluetooth/CBAttribute">Apple documentation for <c>CBAttribute</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CBAttribute {
		[Export ("UUID")]
		CBUUID UUID { get; [NotImplemented] set; }
	}

	/// <summary>Contains options for initializing a Bluetooth central manager.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("CBCentralManager")]
	interface CBCentralInitOptions {
		[Export ("OptionShowPowerAlertKey")]
		bool ShowPowerAlert { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("OptionRestoreIdentifierKey")]
		string RestoreIdentifier { get; set; }
	}

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreBluetooth/CBManager">Apple documentation for <c>CBManager</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CBManager {
		[Export ("state", ArgumentSemantic.Assign)]
		CBManagerState State { get; }

		[Internal]
		[iOS (13, 0)]
		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("authorization", ArgumentSemantic.Assign)]
		CBManagerAuthorization _IAuthorization { get; }

		[Internal]
		[iOS (13, 1)]
		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorization", ArgumentSemantic.Assign)]
		CBManagerAuthorization _SAuthorization { get; }

		[TV (13, 0)]
		[NoiOS]
		[NoMacCatalyst]
		[Static]
		[Export ("authorization", ArgumentSemantic.Assign)]
		CBManagerAuthorization Authorization { get; }
	}

	[iOS (13, 0), TV (13, 0), NoMac]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("CBConnectionEventMatchingOptionsKeys")]
	interface CBConnectionEventMatchingOptions {
		NSUuid [] PeripheralUuids { get; set; }
		CBUUID [] ServiceUuids { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface CBConnectionEventMatchingOptionsKeys {
		[Field ("CBConnectionEventMatchingOptionPeripheralUUIDs")]
		NSString PeripheralUuidsKey { get; }
		[Field ("CBConnectionEventMatchingOptionServiceUUIDs")]
		NSString ServiceUuidsKey { get; }
	}

	[StrongDictionary ("CBConnectPeripheralOptionsKeys")]
	interface CBConnectPeripheralOptions {
		[MacCatalyst (13, 1)]
		bool NotifyOnConnection { get; set; }
		bool NotifyOnDisconnection { get; set; }
		[MacCatalyst (13, 1)]
		bool NotifyOnNotification { get; set; }
		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		bool EnableTransportBridging { get; set; }
		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		bool RequiresAncs { get; set; }
		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		bool EnableAutoReconnect { get; }
	}

	[Static]
	[Internal]
	interface CBConnectPeripheralOptionsKeys {
		[MacCatalyst (13, 1)]
		[Field ("CBConnectPeripheralOptionNotifyOnConnectionKey")]
		NSString NotifyOnConnectionKey { get; }
		[Field ("CBConnectPeripheralOptionNotifyOnDisconnectionKey")]
		NSString NotifyOnDisconnectionKey { get; }
		[MacCatalyst (13, 1)]
		[Field ("CBConnectPeripheralOptionNotifyOnNotificationKey")]
		NSString NotifyOnNotificationKey { get; }
		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("CBConnectPeripheralOptionEnableTransportBridgingKey")]
		NSString EnableTransportBridgingKey { get; }
		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("CBConnectPeripheralOptionRequiresANCS")]
		NSString RequiresAncsKey { get; }
		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("CBConnectPeripheralOptionEnableAutoReconnect")]
		NSString EnableAutoReconnectKey { get; }
	}

	/// <summary>Represents the local central device in Bluetooth LE.  Use the CBCentralManager to scan, discover and connect to remote peripherals.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBCentralManager_Class/index.html">Apple documentation for <c>CBCentralManager</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBManager), Delegates = new [] { "WeakDelegate" }, Events = new [] { typeof (CBCentralManagerDelegate) })]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBCentralManager {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		ICBCentralManagerDelegate Delegate { get; set; }

		[Export ("initWithDelegate:queue:")]
		[PostGet ("WeakDelegate")]
		NativeHandle Constructor ([NullAllowed] ICBCentralManagerDelegate centralDelegate, [NullAllowed] DispatchQueue queue);

		[DesignatedInitializer]
		[MacCatalyst (13, 1)]
		[Export ("initWithDelegate:queue:options:")]
		[PostGet ("WeakDelegate")]
		NativeHandle Constructor ([NullAllowed] ICBCentralManagerDelegate centralDelegate, [NullAllowed] DispatchQueue queue, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("this (centralDelegate, queue, options.GetDictionary ())")]
		NativeHandle Constructor ([NullAllowed] ICBCentralManagerDelegate centralDelegate, [NullAllowed] DispatchQueue queue, CBCentralInitOptions options);

		[Export ("scanForPeripheralsWithServices:options:"), Internal]
		void ScanForPeripherals ([NullAllowed] NSArray serviceUUIDs, [NullAllowed] NSDictionary options);

		[Export ("stopScan")]
		void StopScan ();

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("connectPeripheral:options:")]
		void ConnectPeripheral (CBPeripheral peripheral, [NullAllowed] NSDictionary options);

		[Wrap ("ConnectPeripheral (peripheral, options.GetDictionary ())")]
		void ConnectPeripheral (CBPeripheral peripheral, [NullAllowed] CBConnectPeripheralOptions options);

		[Export ("cancelPeripheralConnection:")]
		void CancelPeripheralConnection (CBPeripheral peripheral);

		[Field ("CBCentralManagerScanOptionAllowDuplicatesKey")]
		NSString ScanOptionAllowDuplicatesKey { get; }

#if !NET
		[Obsolete ("Use 'CBConnectPeripheralOptions' instead.")]
		[Field ("CBConnectPeripheralOptionNotifyOnDisconnectionKey")]
		NSString OptionNotifyOnDisconnectionKey { get; }

		[Obsolete ("Use 'CBConnectPeripheralOptions' instead.")]
		[Field ("CBConnectPeripheralOptionNotifyOnConnectionKey")]
		NSString OptionNotifyOnConnectionKey { get; }

		[Obsolete ("Use 'CBConnectPeripheralOptions' instead.")]
		[Field ("CBConnectPeripheralOptionNotifyOnNotificationKey")]
		NSString OptionNotifyOnNotificationKey { get; }
#endif

		[MacCatalyst (13, 1)]
		[Field ("CBConnectPeripheralOptionStartDelayKey")]
		NSString OptionStartDelayKey { get; }

		[Field ("CBCentralManagerOptionRestoreIdentifierKey")]
		[MacCatalyst (13, 1)]
		NSString OptionRestoreIdentifierKey { get; }

		[Field ("CBCentralManagerRestoredStatePeripheralsKey")]
		[MacCatalyst (13, 1)]
		NSString RestoredStatePeripheralsKey { get; }

		[Field ("CBCentralManagerRestoredStateScanServicesKey")]
		[MacCatalyst (13, 1)]
		NSString RestoredStateScanServicesKey { get; }

		[Field ("CBCentralManagerRestoredStateScanOptionsKey")]
		[MacCatalyst (13, 1)]
		NSString RestoredStateScanOptionsKey { get; }

		[MacCatalyst (13, 1)]
		[Export ("retrievePeripheralsWithIdentifiers:")]
		CBPeripheral [] RetrievePeripheralsWithIdentifiers ([Params] NSUuid [] identifiers);

		[MacCatalyst (13, 1)]
		[Export ("retrieveConnectedPeripheralsWithServices:")]
		CBPeripheral [] RetrieveConnectedPeripherals ([Params] CBUUID [] serviceUUIDs);

		[Field ("CBCentralManagerOptionShowPowerAlertKey")]
		[MacCatalyst (13, 1)]
		NSString OptionShowPowerAlertKey { get; }

		[iOS (16, 0), NoMac, TV (16, 0), MacCatalyst (16, 0)]
		[Field ("CBCentralManagerOptionDeviceAccessForMedia")]
		NSString OptionDeviceAccessForMedia { get; }

		[Field ("CBCentralManagerScanOptionSolicitedServiceUUIDsKey")]
		[MacCatalyst (13, 1)]
		NSString ScanOptionSolicitedServiceUUIDsKey { get; }

		[MacCatalyst (13, 1)]
		[Export ("isScanning")]
		bool IsScanning { get; }

		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportsFeatures:")]
		bool SupportsFeatures (CBCentralManagerFeature features);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("registerForConnectionEventsWithOptions:")]
		void RegisterForConnectionEvents ([NullAllowed] NSDictionary options);

		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("RegisterForConnectionEvents (options.GetDictionary ())")]
		void RegisterForConnectionEvents ([NullAllowed] CBConnectionEventMatchingOptions options);
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("AdvertisementDataKeys")]
	interface AdvertisementData {
		string LocalName { get; set; }
		NSData ManufacturerData { get; set; }
		NSDictionary<CBUUID, NSData> ServiceData { get; set; }
		CBUUID [] ServiceUuids { get; set; }
		CBUUID [] OverflowServiceUuids { get; set; }
		NSNumber TxPowerLevel { get; set; }
		bool IsConnectable { get; set; }
		CBUUID [] SolicitedServiceUuids { get; set; }
	}

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("CBAdvertisementDataOverflowServiceUUIDsKey")]
		NSString OverflowServiceUuidsKey { get; }

		[Field ("CBAdvertisementDataTxPowerLevelKey")]
		NSString TxPowerLevelKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBAdvertisementDataIsConnectable")]
		NSString IsConnectableKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBAdvertisementDataSolicitedServiceUUIDsKey")]
		NSString SolicitedServiceUuidsKey { get; }
	}

	/// <summary>Possible values for the options parameter in calls to <see cref="M:CoreBluetooth.CBCentralManager.ScanForPeripherals(CoreBluetooth.CBUUID[],Foundation.NSDictionary)" />.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("PeripheralScanningOptionsKeys")]
	interface PeripheralScanningOptions { }

	[MacCatalyst (13, 1)]
	[StrongDictionary ("RestoredStateKeys")]
	interface RestoredState {
		CBPeripheral [] Peripherals { get; set; }
		CBPeripheral [] ScanServices { get; set; }
		PeripheralScanningOptions ScanOptions { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static, Internal]
	interface RestoredStateKeys {
		[MacCatalyst (13, 1)]
		[Field ("CBCentralManagerRestoredStatePeripheralsKey")]
		NSString PeripheralsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBCentralManagerRestoredStateScanServicesKey")]
		NSString ScanServicesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBCentralManagerRestoredStateScanOptionsKey")]
		NSString ScanOptionsKey { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:CoreBluetooth.CBCentralManagerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:CoreBluetooth.CBCentralManagerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:CoreBluetooth.CBCentralManagerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:CoreBluetooth.CBCentralManagerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ICBCentralManagerDelegate { }

	/// <summary>Delegate objects for <see cref="T:CoreBluetooth.CBCentralManager" /> objects.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBCentralManagerDelegate_Protocol/index.html">Apple documentation for <c>CBCentralManagerDelegate</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CBCentralManagerDelegate {
		[Abstract]
		[Export ("centralManagerDidUpdateState:")]
		void UpdatedState (CBCentralManager central);

#if !NET
		[NoTV]
		// Available in iOS 5.0 through iOS 8.4. Deprecated in iOS 7.0.
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 8, 4)]
		[NoMacCatalyst]
		[Export ("centralManager:didRetrievePeripherals:"), EventArgs ("CBPeripherals")]
		void RetrievedPeripherals (CBCentralManager central, CBPeripheral [] peripherals);
#endif

#if !NET
		[NoTV]
		// Available in iOS 5.0 through iOS 8.4. Deprecated in iOS 7.0.
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 8, 4)]
		[NoMacCatalyst]
		[Export ("centralManager:didRetrieveConnectedPeripherals:"), EventArgs ("CBPeripherals")]
		void RetrievedConnectedPeripherals (CBCentralManager central, CBPeripheral [] peripherals);
#endif

		[Export ("centralManager:didDiscoverPeripheral:advertisementData:RSSI:"), EventArgs ("CBDiscoveredPeripheral")]
#if XAMCORE_5_0
		void DiscoveredPeripheral (CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi);
#else
		void DiscoveredPeripheral (CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI);
#endif

		[Export ("centralManager:didConnectPeripheral:"), EventArgs ("CBPeripheral")]
		void ConnectedPeripheral (CBCentralManager central, CBPeripheral peripheral);

		[Export ("centralManager:didFailToConnectPeripheral:error:"), EventArgs ("CBPeripheralError")]
		void FailedToConnectPeripheral (CBCentralManager central, CBPeripheral peripheral, [NullAllowed] NSError error);

		[Export ("centralManager:didDisconnectPeripheral:error:"), EventArgs ("CBPeripheralError")]
		void DisconnectedPeripheral (CBCentralManager central, CBPeripheral peripheral, [NullAllowed] NSError error);

		[Export ("centralManager:willRestoreState:"), EventArgs ("CBWillRestore")]
		void WillRestoreState (CBCentralManager central, NSDictionary dict);

		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("centralManager:connectionEventDidOccur:forPeripheral:"), EventArgs ("CBPeripheralConnectionEvent")]
		void ConnectionEventDidOccur (CBCentralManager central, CBConnectionEvent connectionEvent, CBPeripheral peripheral);

		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("centralManager:didUpdateANCSAuthorizationForPeripheral:"), EventArgs ("CBAncsAuthorizationUpdate")]
		void DidUpdateAncsAuthorization (CBCentralManager central, CBPeripheral peripheral);

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0), EventArgs ("CBPeripheralDiconnectionEvent")]
		[Export ("centralManager:didDisconnectPeripheral:timestamp:isReconnecting:error:")]
		void DidDisconnectPeripheral (CBCentralManager central, CBPeripheral peripheral, double timestamp, bool isReconnecting, [NullAllowed] NSError error);
	}

	/// <summary>Keys used to lookup dictionary values from the NSDictionary received as a parameter in <see cref="M:CoreBluetooth.CBCentralManagerDelegate.DiscoveredPeripheral(CoreBluetooth.CBCentralManager,CoreBluetooth.CBPeripheral,Foundation.NSDictionary,Foundation.NSNumber)" />.</summary>
	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("CBAdvertisementDataOverflowServiceUUIDsKey")]
		NSString DataOverflowServiceUUIDsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBAdvertisementDataIsConnectable")]
		NSString IsConnectable { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBAdvertisementDataSolicitedServiceUUIDsKey")]
		NSString DataSolicitedServiceUUIDsKey { get; }

	}

	/// <summary>Characteristics of a remote peripheral.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBCharacteristic_Class/index.html">Apple documentation for <c>CBCharacteristic</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBAttribute))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBCharacteristic {

		[Export ("properties")]
		CBCharacteristicProperties Properties { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableCharacteristic")] set; }

		[NullAllowed]
		[Export ("value", ArgumentSemantic.Retain)]
		NSData Value { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableCharacteristic")] set; }

		[NullAllowed]
		[Export ("descriptors", ArgumentSemantic.Retain)]
		CBDescriptor [] Descriptors { get; [NotImplemented ("Not available on CBCharacteristic, only available on CBMutableCharacteristic")] set; }

		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("isBroadcasted")]
		bool IsBroadcasted { get; }

		[Export ("isNotifying")]
		bool IsNotifying { get; }

		[NullAllowed]
		[Export ("service", ArgumentSemantic.Weak)]
		CBService Service { get; }
	}

	/// <summary>A mutable <see cref="T:CoreBluetooth.CBCharacteristic" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBMutableCharacteristic_Class/index.html">Apple documentation for <c>CBMutableCharacteristic</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBCharacteristic))]
	[DisableDefaultCtor]
	interface CBMutableCharacteristic {

		[NoTV]
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithType:properties:value:permissions:")]
		[PostGet ("UUID")]
		[PostGet ("Value")]
		NativeHandle Constructor (CBUUID uuid, CBCharacteristicProperties properties, [NullAllowed] NSData value, CBAttributePermissions permissions);

		[Export ("permissions", ArgumentSemantic.Assign)]
		CBAttributePermissions Permissions { get; set; }

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

		[Export ("subscribedCentrals")]
		[NullAllowed]
		CBCentral [] SubscribedCentrals { get; }
	}

	/// <summary>An immutable description of a peripheral's characteristic. See also <see cref="T:CoreBluetooth.CBMutableDescriptor" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBDescriptor_Class/index.html">Apple documentation for <c>CBDescriptor</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBAttribute))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBDescriptor {

		[Export ("value", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSObject Value { get; }

		[NullAllowed]
		[Export ("characteristic", ArgumentSemantic.Weak)]
		CBCharacteristic Characteristic { get; }
	}

	/// <summary>A mutable <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:Corebluetooth.CBDescriptor&amp;scope=Xamarin" title="T:Corebluetooth.CBDescriptor">T:Corebluetooth.CBDescriptor</a></format>.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBMutableDescriptor_Class/index.html">Apple documentation for <c>CBMutableDescriptor</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBDescriptor))]
	[DisableDefaultCtor]
	interface CBMutableDescriptor {
		[NoTV]
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithType:value:")]
		[PostGet ("UUID")]
		[PostGet ("Value")]
		NativeHandle Constructor (CBUUID uuid, [NullAllowed] NSObject descriptorValue);
	}

	/// <summary>Represents a CoreBluetooth peripheral.</summary>
	///     <remarks>
	///       <para>
	/// 	CBPeripherals represent a remote peripheral that the device
	/// 	has discovered or has discovered and connected to.
	///       </para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBPeripheral_Class/index.html">Apple documentation for <c>CBPeripheral</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBPeer), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (CBPeripheralDelegate) })]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBPeripheral : NSCopying {
		[Export ("name", ArgumentSemantic.Retain)]
		[DisableZeroCopy]
		[NullAllowed]
		string Name { get; }

		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("RSSI", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSNumber RSSI { get; }

#if !NET
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Export ("isConnected")]
		bool IsConnected { get; }
#endif

		[Export ("services", ArgumentSemantic.Retain)]
		[NullAllowed]
		CBService [] Services { get; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		ICBPeripheralDelegate Delegate { get; set; }

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

		[MacCatalyst (13, 1)]
		[Export ("maximumWriteValueLengthForType:")]
		nuint GetMaximumWriteValueLength (CBCharacteristicWriteType type);

		[MacCatalyst (13, 1)]
		[Export ("state")]
		CBPeripheralState State { get; }

		[MacCatalyst (13, 1)]
		[Export ("canSendWriteWithoutResponse")]
		bool CanSendWriteWithoutResponse { get; }

		[MacCatalyst (13, 1)]
		[Export ("openL2CAPChannel:")]
		void OpenL2CapChannel (ushort psm);

		[iOS (13, 0), TV (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("ancsAuthorized")]
		bool AncsAuthorized { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:CoreBluetooth.CBPeripheralDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:CoreBluetooth.CBPeripheralDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:CoreBluetooth.CBPeripheralDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:CoreBluetooth.CBPeripheralDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ICBPeripheralDelegate { }

	/// <summary>Delegate object for <see cref="T:CoreBluetooth.CBPeripheral" />. Provides methods called on events relating to discovery, exploration, and interaction with a remote peripheral.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBPeripheralDelegate_Protocol/index.html">Apple documentation for <c>CBPeripheralDelegate</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CBPeripheralDelegate {
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'RssiRead' instead.")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'RssiRead' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'RssiRead' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RssiRead' instead.")]
		[Export ("peripheralDidUpdateRSSI:error:"), EventArgs ("NSError", true)]
		void RssiUpdated (CBPeripheral peripheral, [NullAllowed] NSError error);

		[MacCatalyst (13, 1)]
		[Export ("peripheral:didReadRSSI:error:"), EventArgs ("CBRssi")]
		void RssiRead (CBPeripheral peripheral, NSNumber rssi, [NullAllowed] NSError error);

		[Export ("peripheral:didDiscoverServices:"), EventArgs ("NSError", true)]
#if XAMCORE_5_0
		void DiscoveredServices (CBPeripheral peripheral, [NullAllowed] NSError error);
#else
		void DiscoveredService (CBPeripheral peripheral, [NullAllowed] NSError error);
#endif

		[Export ("peripheral:didDiscoverIncludedServicesForService:error:"), EventArgs ("CBService")]
		void DiscoveredIncludedService (CBPeripheral peripheral, CBService service, [NullAllowed] NSError error);

		[Export ("peripheral:didDiscoverCharacteristicsForService:error:"), EventArgs ("CBService")]
#if NET
		void DiscoveredCharacteristics (CBPeripheral peripheral, CBService service, [NullAllowed] NSError error);
#else
		void DiscoveredCharacteristic (CBPeripheral peripheral, CBService service, [NullAllowed] NSError error);
#endif

		[Export ("peripheral:didUpdateValueForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void UpdatedCharacterteristicValue (CBPeripheral peripheral, CBCharacteristic characteristic, [NullAllowed] NSError error);

		[Export ("peripheral:didWriteValueForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void WroteCharacteristicValue (CBPeripheral peripheral, CBCharacteristic characteristic, [NullAllowed] NSError error);

		[Export ("peripheral:didUpdateNotificationStateForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void UpdatedNotificationState (CBPeripheral peripheral, CBCharacteristic characteristic, [NullAllowed] NSError error);

		[Export ("peripheral:didDiscoverDescriptorsForCharacteristic:error:"), EventArgs ("CBCharacteristic")]
		void DiscoveredDescriptor (CBPeripheral peripheral, CBCharacteristic characteristic, [NullAllowed] NSError error);

		[Export ("peripheral:didUpdateValueForDescriptor:error:"), EventArgs ("CBDescriptor")]
		void UpdatedValue (CBPeripheral peripheral, CBDescriptor descriptor, [NullAllowed] NSError error);

		[Export ("peripheral:didWriteValueForDescriptor:error:"), EventArgs ("CBDescriptor")]
		void WroteDescriptorValue (CBPeripheral peripheral, CBDescriptor descriptor, [NullAllowed] NSError error);

#if !NET
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 8, 4)]
		[NoMacCatalyst]
		[Export ("peripheralDidInvalidateServices:")]
		void InvalidatedService (CBPeripheral peripheral);
#endif // !NET

		[Export ("peripheralDidUpdateName:")]
		void UpdatedName (CBPeripheral peripheral);

		[Export ("peripheral:didModifyServices:"), EventArgs ("CBPeripheralServices")]
		void ModifiedServices (CBPeripheral peripheral, CBService [] services);

		[MacCatalyst (13, 1)]
		[EventArgs ("CBPeripheralOpenL2CapChannel")]
		[Export ("peripheral:didOpenL2CAPChannel:error:")]
		void DidOpenL2CapChannel (CBPeripheral peripheral, [NullAllowed] CBL2CapChannel channel, [NullAllowed] NSError error);

		[MacCatalyst (13, 1)]
		[Export ("peripheralIsReadyToSendWriteWithoutResponse:")]
		void IsReadyToSendWriteWithoutResponse (CBPeripheral peripheral);
	}

	/// <summary>Represents the services of a remote peripheral.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBService_Class/index.html">Apple documentation for <c>CBService</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBAttribute))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBService {
		[MacCatalyst (13, 1)]
		[Export ("isPrimary")]
#if NET
		bool Primary { get; }
#else
		bool Primary { get; [NotImplemented ("Not available on 'CBService', only available on 'CBMutableService'.")] set; }
#endif

		[Export ("includedServices", ArgumentSemantic.Retain)]
		[NullAllowed]
		CBService [] IncludedServices { get; [NotImplemented ("Not available on 'CBService', only available on CBMutableService.")] set; }

		[Export ("characteristics", ArgumentSemantic.Retain)]
		[NullAllowed]
		CBCharacteristic [] Characteristics { get; [NotImplemented ("Not available on 'CBService', only available on CBMutableService.")] set; }

		[NullAllowed]
		[Export ("peripheral", ArgumentSemantic.Weak)]
		CBPeripheral Peripheral { get; }

	}

	/// <summary>A mutable <see cref="T:CoreBluetooth.CBService" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBMutableService_Class/index.html">Apple documentation for <c>CBMutableService</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBService))]
	[DisableDefaultCtor]
	interface CBMutableService {
		[NoTV]
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithType:primary:")]
		[PostGet ("UUID")]
		NativeHandle Constructor (CBUUID uuid, bool primary);

		[Export ("includedServices", ArgumentSemantic.Retain)]
		[Override]
		[NullAllowed]
		CBService [] IncludedServices { get; set; }  // TODO: check array type

		[Export ("characteristics", ArgumentSemantic.Retain)]
		[Override]
		[NullAllowed]
		CBCharacteristic [] Characteristics { get; set; }   // TODO: check array type
	}

	/// <summary>Universal Unique Identifiers for the Bluetooth stack.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBUUID_Class/index.html">Apple documentation for <c>CBUUID</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash (at dispose time) on OSX
	interface CBUUID : NSCopying {
		[Export ("data")]
		NSData Data { get; }

		[Static]
		[MarshalNativeExceptions]
		[Export ("UUIDWithString:")]
		CBUUID FromString (string theString);

		[Static]
		[Export ("UUIDWithData:")]
		CBUUID FromData (NSData theData);

		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("UUIDWithCFUUID:")]
		CBUUID FromCFUUID (IntPtr theUUID);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("UUIDWithNSUUID:")]
		CBUUID FromNSUuid (NSUuid theUUID);

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

		[Internal]
		[Field ("CBUUIDValidRangeString")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Obsoleted (PlatformName.MacOSX, 10, 13)]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NSString CBUUIDValidRangeString { get; }

#if MONOMAC && !NET
		[Internal]
		[Field ("CBUUIDCharacteristicValidRangeString")]
		NSString CBUUIDCharacteristicValidRangeString { get; }
#else
		[MacCatalyst (13, 1)]
		[Field ("CBUUIDCharacteristicValidRangeString")]
		NSString CharacteristicValidRangeString { get; }
#endif

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("CBUUIDCharacteristicObservationScheduleString")]
		NSString CharacteristicObservationScheduleString { get; }

		[MacCatalyst (13, 1)]
		[Field ("CBUUIDL2CAPPSMCharacteristicString")]
		NSString L2CapPsmCharacteristicString { get; }

		[MacCatalyst (13, 1)]
		[Export ("UUIDString")]
		string Uuid { get; }
	}

	/// <summary>An Attribute Protocol request for reading or writing.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBATTRequest_class/index.html">Apple documentation for <c>CBATTRequest</c></related>
	[MacCatalyst (13, 1)]
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
		[NullAllowed]
		NSData Value { get; set; }
	}

	/// <summary>Used to identify centrals (that are not the current device).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBCentral_Class/index.html">Apple documentation for <c>CBCentral</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CBPeer))]
	// `delloc` a default instance crash applications and a default instance, without the ability to change the UUID, does not make sense
	[DisableDefaultCtor]
	interface CBCentral : NSCopying {
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("identifier")]
		NSUuid Identifier { get; }

		// Introduced with iOS7, but does not have NS_AVAILABLE
		[Export ("maximumUpdateValueLength")]
		nuint MaximumUpdateValueLength { get; }
	}

	/// <summary>Manages published services per the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:Coreblutooth.CBPeripheral&amp;scope=Xamarin" title="T:Coreblutooth.CBPeripheral">T:Coreblutooth.CBPeripheral</a></format> device's GATT database.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBPeripheralManager_Class/index.html">Apple documentation for <c>CBPeripheralManager</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CBManager), Delegates = new [] { "WeakDelegate" }, Events = new [] { typeof (CBPeripheralManagerDelegate) })]
	interface CBPeripheralManager {

		[Export ("init")]
		NativeHandle Constructor ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("initWithDelegate:queue:")]
		[PostGet ("WeakDelegate")]
		NativeHandle Constructor ([NullAllowed] ICBPeripheralManagerDelegate peripheralDelegate, [NullAllowed] DispatchQueue queue);

		[NoTV]
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithDelegate:queue:options:")]
		[PostGet ("WeakDelegate")]
		NativeHandle Constructor ([NullAllowed] ICBPeripheralManagerDelegate peripheralDelegate, [NullAllowed] DispatchQueue queue, [NullAllowed] NSDictionary options);

		[NullAllowed]
		[Wrap ("WeakDelegate")]
		ICBPeripheralManagerDelegate Delegate { get; set; }

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

		[Wrap ("StartAdvertising (options.GetDictionary ())")]
		void StartAdvertising ([NullAllowed] StartAdvertisingOptions options);

		[Export ("stopAdvertising")]
		void StopAdvertising ();

		[Export ("setDesiredConnectionLatency:forCentral:")]
		void SetDesiredConnectionLatency (CBPeripheralManagerConnectionLatency latency, CBCentral connectedCentral);

		[Export ("updateValue:forCharacteristic:onSubscribedCentrals:")]
		bool UpdateValue (NSData value, CBMutableCharacteristic characteristic, [NullAllowed] CBCentral [] subscribedCentrals);

		[MacCatalyst (13, 1)]
		[Export ("publishL2CAPChannelWithEncryption:")]
		void PublishL2CapChannel (bool encryptionRequired);

		[MacCatalyst (13, 1)]
		[Export ("unpublishL2CAPChannel:")]
		void UnpublishL2CapChannel (ushort psm);

		[Field ("CBPeripheralManagerOptionShowPowerAlertKey")]
		NSString OptionShowPowerAlertKey { get; }

		[Field ("CBPeripheralManagerOptionRestoreIdentifierKey")]
		NSString OptionRestoreIdentifierKey { get; }

		[Field ("CBPeripheralManagerRestoredStateServicesKey")]
		NSString RestoredStateServicesKey { get; }

		[Field ("CBPeripheralManagerRestoredStateAdvertisementDataKey")]
		NSString RestoredStateAdvertisementDataKey { get; }

#if !NET
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CBManager.Authorization' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'CBManager.Authorization' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'CBManager.Authorization' instead.")]
		[Static]
		[Export ("authorizationStatus")]
		CBPeripheralManagerAuthorizationStatus AuthorizationStatus { get; }
#endif // !NET
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:CoreBluetooth.CBPeripheralManagerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:CoreBluetooth.CBPeripheralManagerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:CoreBluetooth.CBPeripheralManagerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:CoreBluetooth.CBPeripheralManagerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ICBPeripheralManagerDelegate { }

	/// <summary>Delegate object for <see cref="T:CoreBluetooth.CBPeripheralManager" />. Adds methods for events relating to availability, publishing, advertising, and subscription.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreBluetooth/Reference/CBPeripheralManagerDelegate_Protocol/index.html">Apple documentation for <c>CBPeripheralManagerDelegate</c></related>
	[MacCatalyst (13, 1)]
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
		void AdvertisingStarted (CBPeripheralManager peripheral, [NullAllowed] NSError error);

		[Export ("peripheralManager:didAddService:error:"), EventArgs ("CBPeripheralManagerService")]
		void ServiceAdded (CBPeripheralManager peripheral, CBService service, [NullAllowed] NSError error);

		[Export ("peripheralManager:central:didSubscribeToCharacteristic:"), EventArgs ("CBPeripheralManagerSubscription")]
		void CharacteristicSubscribed (CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic);

		[Export ("peripheralManager:central:didUnsubscribeFromCharacteristic:"), EventArgs ("CBPeripheralManagerSubscription")]
		void CharacteristicUnsubscribed (CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic);

		[Export ("peripheralManager:didReceiveReadRequest:"), EventArgs ("CBATTRequest")]
		void ReadRequestReceived (CBPeripheralManager peripheral, CBATTRequest request);

		[Export ("peripheralManager:didReceiveWriteRequests:"), EventArgs ("CBATTRequests")]
		void WriteRequestsReceived (CBPeripheralManager peripheral, CBATTRequest [] requests);

		[Export ("peripheralManagerIsReadyToUpdateSubscribers:")]
		void ReadyToUpdateSubscribers (CBPeripheralManager peripheral);

		[MacCatalyst (13, 1)]
		[EventArgs ("CBPeripheralManagerOpenL2CapChannel")]
		[Export ("peripheralManager:didOpenL2CAPChannel:error:")]
		void DidOpenL2CapChannel (CBPeripheralManager peripheral, [NullAllowed] CBL2CapChannel channel, [NullAllowed] NSError error);

		[MacCatalyst (13, 1)]
		[EventArgs ("CBPeripheralManagerL2CapChannelOperation")]
		[Export ("peripheralManager:didUnpublishL2CAPChannel:error:")]
		void DidUnpublishL2CapChannel (CBPeripheralManager peripheral, ushort psm, [NullAllowed] NSError error);

		[MacCatalyst (13, 1)]
		[EventArgs ("CBPeripheralManagerL2CapChannelOperation")]
		[Export ("peripheralManager:didPublishL2CAPChannel:error:")]
		void DidPublishL2CapChannel (CBPeripheralManager peripheral, ushort psm, [NullAllowed] NSError error);
	}

	/// <summary>Class that represents a central or perhipheral Bluetooth device.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreBluetooth/CBPeer">Apple documentation for <c>CBPeer</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // CBPeer.h: - (instancetype)init NS_UNAVAILABLE;
	interface CBPeer : NSCopying {
#if !NET
		[Internal]
		[NoTV]
		[NoMac]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		[Export ("UUID")]
		IntPtr _UUID { get; }
#endif

		[Export ("identifier")]
		NSUuid Identifier { get; }
	}

	// The type is available in 32bits macOS 10.13 even if most properties are 64 bits only
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "CBL2CAPChannel")]
	interface CBL2CapChannel {

		[Export ("peer")]
		CBPeer Peer { get; }

		[Export ("inputStream")]
		NSInputStream InputStream { get; }

		[Export ("outputStream")]
		NSOutputStream OutputStream { get; }

		[Export ("PSM")]
		/* uint16_t */
		ushort Psm { get; }
	}
}
