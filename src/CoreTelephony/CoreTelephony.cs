using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System;

namespace XamCore.CoreTelephony {

	public partial class CTCall {
#if !COREBUILD
		public string StateDialing {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateDialing");
			}
		}
		
		public string StateIncoming {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateIncoming");
			}
		}
		
		public string StateConnected {
			get {
				return Dlfcn.SlowGetStringConstant (Constants.CoreTelephonyLibrary, "CTCallStateConnected");
			}
		}
		
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
