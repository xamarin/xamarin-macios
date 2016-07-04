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
using XamCore.ObjCRuntime;

namespace XamCore.CoreBluetooth {

	[iOS (10,0)]
	[Native]
	public enum CBManagerState : nint {
		Unknown = 0,
		Resetting,
		Unsupported,
		Unauthorized,
		PoweredOff,
		PoweredOn
	}

	// NSInteger -> CBCentralManager.h
	[Introduced (PlatformName.iOS, 5, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use CBManagerState instead")]
	[Native]
	public enum CBCentralManagerState : nint {
		Unknown = CBManagerState.Unknown,
		Resetting = CBManagerState.Resetting,
		Unsupported = CBManagerState.Unsupported,
		Unauthorized = CBManagerState.Unauthorized,
		PoweredOff = CBManagerState.PoweredOff,
		PoweredOn = CBManagerState.PoweredOn
	}

	// NSInteger -> CBPeripheralManager.h
	[Introduced (PlatformName.iOS, 6, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use CBManagerState instead")]
	[Native]
	public enum CBPeripheralManagerState : nint {
		Unknown = CBManagerState.Unknown,
		Resetting = CBManagerState.Resetting,
		Unsupported = CBManagerState.Unsupported,
		Unauthorized = CBManagerState.Unauthorized,
		PoweredOff = CBManagerState.PoweredOff,
		PoweredOn = CBManagerState.PoweredOn
	}

	// NSInteger -> CBPeripheralManager.h
	[Native]
	public enum CBPeripheralState : nint {
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}

	// NSInteger -> CBPeripheralManager.h
	[Native]
	public enum CBPeripheralManagerAuthorizationStatus : nint {
		NotDetermined,
		Restricted,
		Denied,
		Authorized,
	}

	// NSUInteger -> CBCharacteristic.h
	[Flags]
	[Native]
	public enum CBCharacteristicProperties : nuint_compat_int {
		Broadcast = 1,
		Read = 2,
		WriteWithoutResponse = 4,
		Write = 8,
		Notify = 16,
		Indicate = 32,
		AuthenticatedSignedWrites = 64,
		ExtendedProperties = 128,
		NotifyEncryptionRequired = 0x100,
		IndicateEncryptionRequired = 0x200
	}

	[ErrorDomain ("CBErrorDomain")]
	[Native] // NSInteger -> CBError.h
	public enum CBError : nint {
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
		// iOS7.1
		ConnectionFailed,
		// iOS 9
		ConnectionLimitReached
	}

	[ErrorDomain ("CBATTErrorDomain")]
	[Native] // NSInteger -> CBError.h
	public enum CBATTError : nint {
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
	[Native]
	public enum CBCharacteristicWriteType : nint {
		WithResponse,
		WithoutResponse
	}

	// NSUInteger -> CBCharacteristic.h
	[Flags]
	[Native]
	public enum CBAttributePermissions : nuint_compat_int {
		Readable	= 1,
		Writeable	= 1 << 1,
		ReadEncryptionRequired	= 1 << 2,
		WriteEncryptionRequired	= 1 << 3
	}

	// NSInteger -> CBPeripheralManager.h
	[Native]
	public enum CBPeripheralManagerConnectionLatency : nint {
		Low = 0,
		Medium,
		High
	}
}
