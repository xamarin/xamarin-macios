using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System;

namespace XamCore.CoreTelephony {

	public partial class CTCall {
#if !COREBUILD
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use CallKit")]
		public string StateDialing {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateDialing");
			}
		}

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use CallKit")]
		public string StateIncoming {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateIncoming");
			}
		}

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use CallKit")]
		public string StateConnected {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateConnected");
			}
		}

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use CallKit")]
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

	[iOS (9,0)]
	[Native]
	public enum CTCellularDataRestrictedState : nuint {
		Unknown,
		Restricted,
		NotRestricted
	}
}
