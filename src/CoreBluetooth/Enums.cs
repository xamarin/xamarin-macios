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

namespace CoreBluetooth {

	[Mac (10,13)]
	[Watch (4,0)]
	[iOS (10,0)]
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
	[iOS (6, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CBManagerState' instead.")]
	[NoWatch]
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
	[Watch (4,0)]
	[Native]
	public enum CBPeripheralState : long {
		Disconnected,
		Connecting,
		Connected,
		[iOS (9,0)][Mac (10,13)]
		Disconnecting,
	}

#if !XAMCORE_4_0
	// NSInteger -> CBPeripheralManager.h
	[Watch (4,0)]
	[Native]
	public enum CBPeripheralManagerAuthorizationStatus : long {
		NotDetermined,
		Restricted,
		Denied,
		Authorized,
	}
#endif

	// NSUInteger -> CBCharacteristic.h
	[Watch (4,0)]
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
		[Mac (10,9)]
		NotifyEncryptionRequired = 0x100,
		[Mac (10,9)]
		IndicateEncryptionRequired = 0x200
	}

	[Watch (4,0)]
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
		[iOS (7,1)][Mac (10,13)]
		ConnectionFailed,
		[iOS (9,0)][Mac (10,13)]
		ConnectionLimitReached,
		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		UnknownDevice,
	}

	[Watch (4,0)]
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
	[Watch (4,0)]
	[Native]
	public enum CBCharacteristicWriteType : long {
		WithResponse,
		WithoutResponse
	}

	// NSUInteger -> CBCharacteristic.h
	[Mac (10,9)]
	[Watch (4,0)]
	[Flags]
	[Native]
	public enum CBAttributePermissions : ulong {
		Readable	= 1,
		Writeable	= 1 << 1,
		ReadEncryptionRequired	= 1 << 2,
		WriteEncryptionRequired	= 1 << 3
	}

	// NSInteger -> CBPeripheralManager.h
	[Watch (4,0)]
	[Native]
	public enum CBPeripheralManagerConnectionLatency : long {
		Low = 0,
		Medium,
		High
	}
}
