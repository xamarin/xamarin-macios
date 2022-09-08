using System;
using ObjCRuntime;
using Foundation;

namespace LocalAuthentication {

	[iOS (8,0)]
	[Mac (10, 10)]
	[NoTV]
	[Native]
	public enum LAPolicy : long {
		[Mac (10,12,2), NoWatch]
		DeviceOwnerAuthenticationWithBiometrics = 1,
		DeviceOwnerAuthentication = 2,
		[NoiOS][Mac (10,15)][NoWatch]
		DeviceOwnerAuthenticationWithWatch = 3,
		[NoiOS][Mac (10,15)][NoWatch]
		DeviceOwnerAuthenticationWithBiometricsOrWatch = 4,
		[Obsolete ("Use DeviceOwnerAuthenticationWithBiometricsOrWatch enum value instead.")]
		[NoiOS][Mac (10,15)][NoWatch]
		OwnerAuthenticationWithBiometricsOrWatch = DeviceOwnerAuthenticationWithBiometricsOrWatch,
		[NoMac, NoiOS, NoMacCatalyst, Watch (9,0)]
		DeviceOwnerAuthenticationWithWristDetection = 5,
	}

	[iOS (8,0)]
	[Mac (10, 10)]
	[NoTV]
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
		[NoiOS, NoWatch, NoMacCatalyst]
		WatchNotAvailable    = -11,
		[NoiOS, NoWatch, NoMacCatalyst]
		BiometryNotPaired    = -12,
		[NoiOS, NoWatch, NoMacCatalyst]
		BiometryDisconnected = -13,
		[NoiOS, NoWatch, NoMacCatalyst]
		InvalidDimension     = -14,
		[NoWatch]
		BiometryNotAvailable = -6,
		[NoWatch]
		BiometryNotEnrolled = -7,
		[NoWatch]
		BiometryLockout = -8,
		NotInteractive       = -1004,
	}

	[iOS (9,0), Mac (10,11), Watch (3,0), NoTV]
	[Native]
	public enum LACredentialType : long {
		ApplicationPassword = 0,
		[iOS (13,4), Mac (10,15,4), NoWatch, NoTV]
		SmartCardPin = -3,
	}

	[iOS (9,0)]
	[Mac (10,11)]
	[NoTV]
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

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[Native]
	public enum LARightState : long
	{
		Unknown = 0,
		Authorizing = 1,
		Authorized = 2,
		NotAuthorized = 3,
	}
}
