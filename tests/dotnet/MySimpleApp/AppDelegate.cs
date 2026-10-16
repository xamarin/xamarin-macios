using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			if (Environment.GetEnvironmentVariable ("CRASH_ON_LAUNCH") == "1")
				Environment.FailFast ("Crashing on launch as requested by the CRASH_ON_LAUNCH environment variable.");
#if KEEP_LARIGHT
			if (OperatingSystem.IsIOSVersionAtLeast (17))
				GC.KeepAlive (typeof (LocalAuthentication.LARight));
#elif KEEP_LAPERSISTEDRIGHT
			if (OperatingSystem.IsIOSVersionAtLeast (17))
				GC.KeepAlive (typeof (LocalAuthentication.LAPersistedRight));
#elif KEEP_LACONTEXT
			using (var context = new LocalAuthentication.LAContext ())
				Console.WriteLine (context.BiometryType);
#endif

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
		}
	}
}
