#if NET
using System;

using CoreBluetooth;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AccessorySetupKit {
	[Native]
	[iOS (18, 0)]
	public enum ASAccessoryState : long {
		Unauthorized            = 0,
		AwaitingAuthorization   = 10,
		Authorized              = 20,
	}

	[Flags]
	[Native]
	[iOS (18, 0)]
	public enum ASAccessoryRenameOptions : ulong {
	    Ssid = 1U << 0,
	}

	[Flags]
	[Native]
	[iOS (18, 0)]
	public enum ASAccessorySupportOptions : ulong {
		BluetoothPairingLE = 1U << 1,
		BluetoothTransportBridging = 1U << 2,
	}

	[Native]
	[iOS (18, 0)]
	public enum ASDiscoveryDescriptorRange : long {
		Default = 0,
		Immediate = 10,
	}

	[Flags]
	[Native]
	[iOS (18, 0)]
	public enum ASPickerDisplayItemSetupOptions : long {
		Rename = 1 << 0,
		ConfirmAuthorization = 1 << 1,
		FinishInApp = 1 << 2,
	}

	[BaseType (typeof (NSObject))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASAccessory {
		[Export ("state", ArgumentSemantic.Assign)]
		ASAccessoryState State { get; }

		[Export ("bluetoothIdentifier", ArgumentSemantic.Copy), NullAllowed]
		NSUuid BluetoothIdentifier { get; }

		[Export ("displayName", ArgumentSemantic.Copy)]
		string DisplayName { get; }

		[Export ("SSID", ArgumentSemantic.Copy), NullAllowed]
		string Ssid { get; }

		[Export ("descriptor", ArgumentSemantic.Copy)]
		ASDiscoveryDescriptor Descriptor { get; }

		[Export ("bluetoothTransportBridgingIdentifier", ArgumentSemantic.Copy), NullAllowed]
		NSData BluetoothTransportBridgingIdentifier { get; }
	}

	[Native]
	[iOS (18, 0)]
	public enum ASAccessoryEventType : long {
		Unknown             = 0,
		Activated           = 10,
		Invalidated         = 11,
		MigrationComplete   = 20,
		AccessoryAdded      = 30,
		AccessoryRemoved    = 31,
		AccessoryChanged    = 32,
		PickerDidPresent    = 40,
		PickerDidDismiss    = 50,
		PickerSetupBridging = 60,
		PickerSetupFailed   = 70,
		PickerSetupPairing  = 80,
		PickerSetupRename   = 90,
	}

	[BaseType (typeof (NSObject))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASAccessoryEvent {
		[Export ("eventType", ArgumentSemantic.Assign)]
		ASAccessoryEventType EventType { get; }

		[Export ("accessory", ArgumentSemantic.Copy), NullAllowed]
		ASAccessory Accessory { get; }

		[Export ("error", ArgumentSemantic.Copy), NullAllowed]
		NSError Error { get; }
	}

	delegate void ASAccessorySessionCompletionHandler ([NullAllowed] NSError error);

	[BaseType (typeof (NSObject))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASAccessorySession {
		[Export ("accessories", ArgumentSemantic.Copy)]
		ASAccessory [] Accessories { get; }

		[Export ("activateWithQueue:eventHandler:")]
		void Activate (DispatchQueue queue, Action<ASAccessoryEvent> eventHandler);

		[Export ("invalidate")]
		void Invalidate ();

		[Async]
		[Export ("showPickerWithCompletionHandler:")]
		void ShowPicker (ASAccessorySessionCompletionHandler completionHandler);

		[Async]
		[Export ("showPickerForDisplayItems:completionHandler:")]
		void ShowPicker (ASPickerDisplayItem [] displayItems, ASAccessorySessionCompletionHandler completionHandler);

		[Async]
		[Export ("finishAuthorization:settings:completionHandler:")]
		void FinishAuthorization (ASAccessory accessory, ASAccessorySettings settings, ASAccessorySessionCompletionHandler completionHandler);

		[Async]
		[Export ("removeAccessory:completionHandler:")]
		void RemoveAccessory (ASAccessory accessory, ASAccessorySessionCompletionHandler completionHandler);

		[Async]
		[Export ("renameAccessory:options:completionHandler:")]
		void RenameAccessory (ASAccessory accessory, ASAccessoryRenameOptions renameOptions, ASAccessorySessionCompletionHandler completionHandler);

		[Async]
		[Export ("failAuthorization:completionHandler:")]
		void FailAuthorization (ASAccessory accessory, ASAccessorySessionCompletionHandler completionHandler);
	}

	[BaseType (typeof (NSObject))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASAccessorySettings {
		[Export ("defaultSettings")]
		[Static]
		ASAccessorySettings DefaultSettings { get; }

		[Export ("SSID", ArgumentSemantic.Copy), NullAllowed]
		string Ssid { get; set; }

		[Export ("bluetoothTransportBridgingIdentifier", ArgumentSemantic.Copy), NullAllowed]
		NSData BluetoothTransportBridgingIdentifier { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASDiscoveryDescriptor {
		[Export ("supportedOptions", ArgumentSemantic.Assign)]
		ASAccessorySupportOptions SupportedOptions { get; set; }

		[Export ("bluetoothCompanyIdentifier", ArgumentSemantic.Assign)]
		ushort /* ASBluetoothCompanyIdentifier */ BluetoothCompanyIdentifier { get; set; }

		[Export ("bluetoothManufacturerDataBlob", ArgumentSemantic.Copy), NullAllowed]
		NSData BluetoothManufacturerDataBlob { get; set; }

		[Export ("bluetoothManufacturerDataMask", ArgumentSemantic.Copy), NullAllowed]
		NSData BluetoothManufacturerDataMask { get; set; }

		[iOS (18, 2)]
		[Export ("bluetoothNameSubstringCompareOptions", ArgumentSemantic.Assign), NullAllowed]
		NSStringCompareOptions BluetoothNameSubstringCompareOptions { get; set; }

		[Export ("bluetoothNameSubstring", ArgumentSemantic.Copy), NullAllowed]
		string BluetoothNameSubstring { get; set; }

		[Export ("bluetoothRange", ArgumentSemantic.Assign)]
		ASDiscoveryDescriptorRange BluetoothRange { get; set; }

		[Export ("bluetoothServiceDataBlob", ArgumentSemantic.Copy), NullAllowed]
		NSData BluetoothServiceDataBlob { get; set; }

		[Export ("bluetoothServiceDataMask", ArgumentSemantic.Copy), NullAllowed]
		NSData BluetoothServiceDataMask { get; set; }

		[Export ("bluetoothServiceUUID", ArgumentSemantic.Copy), NullAllowed]
		CBUUID BluetoothServiceUuid { get; set; }
		[Export ("SSID", ArgumentSemantic.Copy), NullAllowed]
		string Ssid { get; set; }

		[Export ("SSIDPrefix", ArgumentSemantic.Copy), NullAllowed]
		string SsidPrefix { get; set; }
	}

	[Native]
	[iOS (18, 0)]
	[ErrorDomain ("ASErrorDomain")]
	enum ASErrorCode : long {
		Success             = 0,
		Unknown             = 1,
		ActivationFailed    = 100,
		ConnectionFailed    = 150,
		DiscoveryTimeout    = 200,
		ExtensionNotFound   = 300,
		Invalidated         = 400,
		InvalidRequest      = 450,
		PickerAlreadyActive = 500,
		PickerRestricted    = 550,
		UserCancelled       = 700,
		UserRestricted      = 750,
	}

	[BaseType (typeof (NSObject))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASPickerDisplayItem {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("productImage", ArgumentSemantic.Copy)]
		UIImage ProductImage { get; }

		[Export ("descriptor", ArgumentSemantic.Copy)]
		ASDiscoveryDescriptor Descriptor { get; set; }

		[Export ("renameOptions", ArgumentSemantic.Assign)]
		ASAccessoryRenameOptions RenameOptions { get; set; }

		[Export ("setupOptions", ArgumentSemantic.Assign)]
		ASPickerDisplayItemSetupOptions SetupOptions { get; set; }

		[Export ("initWithName:productImage:descriptor:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, UIImage productImage, ASDiscoveryDescriptor descriptor);
	}

	[BaseType (typeof (ASPickerDisplayItem))]
	[iOS (18, 0)]
	[DisableDefaultCtor]
	interface ASMigrationDisplayItem {
		[Export ("peripheralIdentifier", ArgumentSemantic.Copy), NullAllowed]
		NSUuid PeripheralIdentifier { get; set; }

		[Export ("hotspotSSID", ArgumentSemantic.Copy), NullAllowed]
		string HotspotSsid { get; set; }

		// re-exposed from base
		[Export ("initWithName:productImage:descriptor:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, UIImage productImage, ASDiscoveryDescriptor descriptor);
	}
}

#endif // !NET
