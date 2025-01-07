using System.Runtime.InteropServices;

namespace BindingWithCompressedXCFramework {
	public static class CFunctions {
		[DllImport ("XTest.framework/XTest")]
		static extern int theUltimateAnswer ();

		// This comes from XStaticArTest.framework
		[DllImport ("__Internal")]
		static extern int ar_theUltimateAnswer ();

		// This comes from XStaticObjectTest.framework
		[DllImport ("__Internal", EntryPoint = "theUltimateAnswer")]
		static extern int object_theUltimateAnswer ();
	}
}
