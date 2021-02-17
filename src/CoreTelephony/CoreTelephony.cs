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
}
