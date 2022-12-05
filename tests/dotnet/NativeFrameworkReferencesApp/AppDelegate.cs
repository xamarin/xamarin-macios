using System;
using System.Runtime.InteropServices;

using Foundation;

namespace NativeFrameworkReferencesApp {
	public class Program {
		[DllImport ("XTest.framework/XTest")]
		static extern int theUltimateAnswer ();

		// This comes from XStaticArTest.framework
		[DllImport ("__Internal")]
		static extern int ar_theUltimateAnswer ();

		// This comes from XStaticObjectTest.framework
		[DllImport ("__Internal", EntryPoint = "theUltimateAnswer")]
		static extern int object_theUltimateAnswer ();

		static int Main (string [] args)
		{
			Console.WriteLine ($"Framework: {theUltimateAnswer ()}");
			Console.WriteLine ($"Framework with ar files: {ar_theUltimateAnswer ()}");
			Console.WriteLine ($"Framework with object files: {object_theUltimateAnswer ()}");

			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
