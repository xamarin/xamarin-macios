using Foundation;
using ObjCRuntime;
using System;

namespace CoreTelephony {

	public partial class CTCall {
#if !COREBUILD
		[Deprecated (PlatformName.iOS, 10, 0, message : Constants.UseCallKitInstead)]
		public string StateDialing {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateDialing");
			}
		}

		[Deprecated (PlatformName.iOS, 10, 0, message : Constants.UseCallKitInstead)]
		public string StateIncoming {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateIncoming");
			}
		}

		[Deprecated (PlatformName.iOS, 10, 0, message : Constants.UseCallKitInstead)]
		public string StateConnected {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateConnected");
			}
		}

		[Deprecated (PlatformName.iOS, 10, 0, message : Constants.UseCallKitInstead)]
		public string StateDisconnected {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateDisconnected");
			}
		}
#endif // !COREBUILD
	}

	// untyped enum -> CoreTelephonyDefines.h
	// in header file this is used inside a CTError structure where the domain is a SInt32
	public enum CTErrorDomain {
		NoError = 0, Posix = 1, Mach = 2
	}

	[Obsoleted (PlatformName.iOS, 14,0, message: Constants.UseCallKitInstead)]
	[iOS (9,0)]
	[Native]
	public enum CTCellularDataRestrictedState : ulong {
		Unknown,
		Restricted,
		NotRestricted
	}

	[Obsoleted (PlatformName.iOS, 14,0, message: Constants.UseCallKitInstead)]
	[iOS (12,0)]
	[Native]
	public enum CTCellularPlanProvisioningAddPlanResult : long {
		Unknown,
		Fail,
		Success,
	}
}
