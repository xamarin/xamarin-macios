//
// Enums.cs: Enums definitions for CoreBluetooth
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2011-2014 Xamarin Inc
//

using System;

using ObjCRuntime;

#nullable enable

namespace CoreBluetooth {

	[MacCatalyst (13, 1)]
	[Native]
	public enum CBManagerState : long {
		Unknown = 0,
		Resetting,
		Unsupported,
		Unauthorized,
		PoweredOff,
		PoweredOn
	}

	// NSInteger -> CBCentralManager.h
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CBManagerState' instead.")]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CBManagerState' instead.")]
	[Native]
	public enum CBCentralManagerState : long {
		Unknown = CBManagerState.Unknown,
		Resetting = CBManagerState.Resetting,
		Unsupported = CBManagerState.Unsupported,
		Unauthorized = CBManagerState.Unauthorized,
		PoweredOff = CBManagerState.PoweredOff,
		PoweredOn = CBManagerState.PoweredOn
	}

	// NSInteger -> CBPeripheralManager.h
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CBManagerState' instead.")]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CBManagerState' instead.")]
	[Native]
	public enum CBPeripheralManagerState : long {
		Unknown = CBManagerState.Unknown,
		Resetting = CBManagerState.Resetting,
		Unsupported = CBManagerState.Unsupported,
		Unauthorized = CBManagerState.Unauthorized,
		PoweredOff = CBManagerState.PoweredOff,
		PoweredOn = CBManagerState.PoweredOn
	}

	// NSInteger -> CBPeripheralManager.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CBPeripheralState : long {
		Disconnected,
		Connecting,
		Connected,
		[MacCatalyst (13, 1)]
		Disconnecting,
	}

#if !NET
	// NSInteger -> CBPeripheralManager.h
	[Native]
	public enum CBPeripheralManagerAuthorizationStatus : long {
		NotDetermined,
		Restricted,
		Denied,
		Authorized,
	}
#endif // !NET

	// NSUInteger -> CBCharacteristic.h
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum CBCharacteristicProperties : ulong {
		Broadcast = 1,
		Read = 2,
		WriteWithoutResponse = 4,
		Write = 8,
		Notify = 16,
		Indicate = 32,
		AuthenticatedSignedWrites = 64,
		ExtendedProperties = 128,
		[MacCatalyst (13, 1)]
		NotifyEncryptionRequired = 0x100,
		[MacCatalyst (13, 1)]
		IndicateEncryptionRequired = 0x200
	}

	[MacCatalyst (13, 1)]
	[ErrorDomain ("CBErrorDomain")]
	[Native] // NSInteger -> CBError.h
	public enum CBError : long {
		None = 0,
		Unknown = 0,
		InvalidParameters,
		InvalidHandle,
		NotConnected,
		OutOfSpace,
		OperationCancelled,
		ConnectionTimeout,
		PeripheralDisconnected,
		UUIDNotAllowed,
		AlreadyAdvertising,
		ConnectionFailed,
		ConnectionLimitReached,
		UnknownDevice,
		OperationNotSupported,
		PeerRemovedPairingInformation,
		EncryptionTimedOut,
		TooManyLEPairedDevices = 16,
		LEGattExceededBackgroundNotificationLimit = 17,
		LEGattNearBackgroundNotificationLimit = 18,
	}

	[MacCatalyst (13, 1)]
	[ErrorDomain ("CBATTErrorDomain")]
	[Native] // NSInteger -> CBError.h
	public enum CBATTError : long {
		Success = 0,
		InvalidHandle,
		ReadNotPermitted,
		WriteNotPermitted,
		InvalidPdu,
		InsufficientAuthentication,
		RequestNotSupported,
		InvalidOffset,
		InsufficientAuthorization,
		PrepareQueueFull,
		AttributeNotFound,
		AttributeNotLong,
		InsufficientEncryptionKeySize,
		InvalidAttributeValueLength,
		UnlikelyError,
		InsufficientEncryption,
		UnsupportedGroupType,
		InsufficientResources
	}

	// NSInteger -> CBPeripheral.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CBCharacteristicWriteType : long {
		WithResponse,
		WithoutResponse
	}

	// NSUInteger -> CBCharacteristic.h
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum CBAttributePermissions : ulong {
		Readable = 1,
		Writeable = 1 << 1,
		ReadEncryptionRequired = 1 << 2,
		WriteEncryptionRequired = 1 << 3
	}

	// NSInteger -> CBPeripheralManager.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CBPeripheralManagerConnectionLatency : long {
		Low = 0,
		Medium,
		High
	}

	[iOS (13, 0), TV (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CBConnectionEvent : long {
		Disconnected = 0,
		Connected = 1,
	}

	[Flags, iOS (13, 0), TV (13, 0), Watch (6, 0), NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CBCentralManagerFeature : ulong {
		ExtendedScanAndConnect = 1uL << 0,
	}

	[iOS (13, 0), TV (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CBManagerAuthorization : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		AllowedAlways,
	}
}
