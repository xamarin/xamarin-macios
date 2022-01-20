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

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios10.0")]
#else
	[Mac (10,13)]
	[Watch (4,0)]
	[iOS (10,0)]
#endif
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
#if NET
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0 use 'CBManagerState' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CBManagerState' instead.")]
	[NoWatch]
#endif
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
#if NET
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0 use 'CBManagerState' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CBManagerState' instead.")]
	[NoWatch]
#endif
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
#if !NET
	[Watch (4,0)]
#endif
	[Native]
	public enum CBPeripheralState : long {
		Disconnected,
		Connecting,
		Connected,
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.13")]
#else
		[iOS (9,0)]
		[Mac (10,13)]
#endif
		Disconnecting,
	}

#if !NET
	// NSInteger -> CBPeripheralManager.h
#if !NET
	[Watch (4,0)]
#endif
	[Native]
	public enum CBPeripheralManagerAuthorizationStatus : long {
		NotDetermined,
		Restricted,
		Denied,
		Authorized,
	}
#endif // !NET

	// NSUInteger -> CBCharacteristic.h
#if !NET
	[Watch (4,0)]
#endif
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
#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		NotifyEncryptionRequired = 0x100,
#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		IndicateEncryptionRequired = 0x200
	}

#if !NET
	[Watch (4,0)]
#endif
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
	}

#if !NET
	[Watch (4,0)]
#endif
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
#if !NET
	[Watch (4,0)]
#endif
	[Native]
	public enum CBCharacteristicWriteType : long {
		WithResponse,
		WithoutResponse
	}

	// NSUInteger -> CBCharacteristic.h
#if NET
	[SupportedOSPlatform ("macos10.9")]
#else
	[Mac (10,9)]
	[Watch (4,0)]
#endif
	[Flags]
	[Native]
	public enum CBAttributePermissions : ulong {
		Readable	= 1,
		Writeable	= 1 << 1,
		ReadEncryptionRequired	= 1 << 2,
		WriteEncryptionRequired	= 1 << 3
	}

	// NSInteger -> CBPeripheralManager.h
#if !NET
	[Watch (4,0)]
#endif
	[Native]
	public enum CBPeripheralManagerConnectionLatency : long {
		Low = 0,
		Medium,
		High
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Watch (6,0)]
	[Mac (10,15)]
#endif
	[Native]
	public enum CBConnectionEvent : long {
		Disconnected = 0,
		Connected = 1,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Watch (6,0)]
	[NoMac]
#endif
	[Flags]
	[Native]
	public enum CBCentralManagerFeature : ulong {
		ExtendedScanAndConnect = 1uL << 0,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Watch (6,0)]
	[Mac (10,15)]
#endif
	[Native]
	public enum CBManagerAuthorization : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		AllowedAlways,
	}
}
