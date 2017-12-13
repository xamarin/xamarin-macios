using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;

namespace XamCore.LocalAuthentication {

	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum LAPolicy : nint {
		[Mac (10,12,2)]
		DeviceOwnerAuthenticationWithBiometrics = 1,
		DeviceOwnerAuthentication = 2
	}

	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	[ErrorDomain ("LAErrorDomain")]
	public enum LAStatus : nint {
		Success = 0,
		/// Authentication was not successful, because user failed to provide valid credentials.
		AuthenticationFailed = -1,
		/// Authentication was canceled by user (e.g. tapped Cancel button).
		UserCancel           = -2,	    
		/// Authentication was canceled, because the user tapped the fallback button (Enter Password).
		UserFallback         = -3,	    
		/// Authentication was canceled by system (e.g. another application went to foreground).
		SystemCancel         = -4,	    
		/// Authentication could not start, because passcode is not set on the device.
		PasscodeNotSet       = -5,
		/// Authentication could not start, because Touch ID is not available on the device.
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'BiometryNotAvailable' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'BiometryNotAvailable' instead.")]
		TouchIDNotAvailable  = -6,	    

		/// Authentication could not start, because Touch ID has no enrolled fingers.
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'BiometryNotEnrolled' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'BiometryNotEnrolled' instead.")]
		TouchIDNotEnrolled   = -7,

		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'BiometryLockout' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'BiometryLockout' instead.")]
		TouchIDLockout       = -8,
		AppCancel            = -9,
		InvalidContext       = -10,

		BiometryNotAvailable = TouchIDNotAvailable,
		BiometryNotEnrolled = TouchIDNotEnrolled,
		BiometryLockout = TouchIDLockout,

		NotInteractive       = -1004,
	}

	[iOS (9,0), Mac (10,11), Watch (3,0), TV (11,0)]
	[Native]
	public enum LACredentialType : nint {
		ApplicationPassword = 0
	}

	[iOS (9,0)]
	[Mac (10,11)]
	[Native]
	public enum LAAccessControlOperation : nint {
		CreateItem,
		UseItem,
		CreateKey,
		UseKeySign,
		[iOS (10,0)][Mac (10,12)]
		UseKeyDecrypt,
		[iOS (10,0)][Mac (10,12)]
		UseKeyKeyExchange,
	}
}
