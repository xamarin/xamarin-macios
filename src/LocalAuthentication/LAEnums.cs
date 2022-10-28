using System;
using ObjCRuntime;
using Foundation;

namespace LocalAuthentication {

	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum LAPolicy : long {
		[Mac (10,12,2)]
		DeviceOwnerAuthenticationWithBiometrics = 1,
		DeviceOwnerAuthentication = 2,
		[NoiOS][Mac (10,15)]
		DeviceOwnerAuthenticationWithWatch = 3,
		[NoiOS][Mac (10,15)]
		OwnerAuthenticationWithBiometricsOrWatch = 4,
	}

	[iOS (8,0)]
	[Mac (10, 10)]
	[Native ("LAError")]
	[ErrorDomain ("LAErrorDomain")]
	public enum LAStatus : long {
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

#if !NET
		/// Authentication could not start, because Touch ID is not available on the device.
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'BiometryNotAvailable' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'BiometryNotAvailable' instead.")]
		TouchIDNotAvailable  = BiometryNotAvailable,

		/// Authentication could not start, because Touch ID has no enrolled fingers.
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'BiometryNotEnrolled' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'BiometryNotEnrolled' instead.")]
		TouchIDNotEnrolled   = BiometryNotEnrolled,

		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'BiometryLockout' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,13, message: "Use 'BiometryLockout' instead.")]
		TouchIDLockout       = BiometryLockout,
#endif
		AppCancel            = -9,
		InvalidContext       = -10,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		WatchNotAvailable    = -11,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		BiometryNotPaired    = -12,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		BiometryDisconnected = -13,
		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		InvalidDimension     = -14,

		BiometryNotAvailable = -6,
		BiometryNotEnrolled = -7,
		BiometryLockout = -8,

		NotInteractive       = -1004,
	}

	[iOS (9,0), Mac (10,11), Watch (3,0), TV (11,0)]
	[Native]
	public enum LACredentialType : long {
		ApplicationPassword = 0,
		[iOS (13,4), Mac (10,15,4), NoWatch, NoTV]
		SmartCardPin = -3,
	}

	[iOS (9,0)]
	[Mac (10,11)]
	[Native]
	public enum LAAccessControlOperation : long {
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
