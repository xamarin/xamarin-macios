using System;
using System.Runtime.InteropServices;

using Foundation;

namespace NativeFrameworkReferencesApp {
	public class Program {
		[DllImport ("XTest.framework/XTest")]
		static extern int theUltimateAnswer ();

		static int Main (string [] args)
		{
			Console.WriteLine ($"XCFramework: {theUltimateAnswer ()}");

			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
