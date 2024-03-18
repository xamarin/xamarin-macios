using System;
using System.Runtime.InteropServices;

namespace XCFrameworkWithStaticLibraryInRuntimesNativeDirectory {
	public static class PInvokes {
		[DllImport ("__Internal")]
		public static extern int theUltimateAnswer ();
	}
}
