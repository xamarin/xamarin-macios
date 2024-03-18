using System;
using System.Runtime.InteropServices;

using Foundation;

namespace AppWithXCFrameworkWithStaticLibraryInPackageReference {
	public class Program {
		[DllImport ("__Internal")]
		static extern int theUltimateAnswer ();

		static int Main (string [] args)
		{
			Console.WriteLine ($"Static library from XCFramework: {theUltimateAnswer ()}");

			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
