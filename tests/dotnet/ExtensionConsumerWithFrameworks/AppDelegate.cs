using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleApp {
	public class Program {
		[DllImport ("__Internal")]
		static extern int getUnknownE ();
		[DllImport ("__Internal")]
		static extern int getSomewhatUnknownD ();

		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (getUnknownE ());
			Console.WriteLine (getSomewhatUnknownD ());
			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
		}
	}
}
