using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
		}

		[DllImport ("__Internal")]
		static extern IntPtr getNoLibPrefix ();
	}
}
