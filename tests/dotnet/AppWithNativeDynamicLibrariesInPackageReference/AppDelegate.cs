using System;
using System.Runtime.InteropServices;

using Foundation;

namespace NativeDynamicLibraryReferencesApp {
	public class Program {
		[DllImport ("libtest.dylib")]
		static extern int theUltimateAnswer ();

		static int Main (string [] args)
		{
			Console.WriteLine ($"Dynamic library: {theUltimateAnswer ()}");

			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
