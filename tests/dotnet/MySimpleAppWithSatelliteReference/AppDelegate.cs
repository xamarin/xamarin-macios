using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleAppWithSatelliteReference {
	public class Program {
		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
		}
	}
}
