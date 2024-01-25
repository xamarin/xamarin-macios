#nullable enable

using Foundation;

using ObjCRuntime;

using System;

namespace CoreTelephony {

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
}
