using System;
using System.Runtime.InteropServices;

using Foundation;

namespace NativeFileReferencesApp {
	public class Program {
		// This comes from libtest.a
		[DllImport ("__Internal")]
		static extern int theUltimateAnswer ();

		// This comes from libtest2.a
		[DllImport ("__Internal")]
		static extern int getIntOfChocolate ();

		static int Main (string [] args)
		{
			Console.WriteLine ($"libtest.a: {theUltimateAnswer ()}");
			Console.WriteLine ($"libtest2.a: {getIntOfChocolate ()}");

			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
