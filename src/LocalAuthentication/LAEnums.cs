using System;
using XamCore.ObjCRuntime;
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
		TouchIDNotAvailable  = -6,	    
		/// Authentication could not start, because Touch ID has no enrolled fingers.
		TouchIDNotEnrolled   = -7,

		TouchIDLockout       = -8,
		AppCancel            = -9,
		InvalidContext       = -10
	}

	[iOS (9,0)]
	[Mac (10,11)]
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
