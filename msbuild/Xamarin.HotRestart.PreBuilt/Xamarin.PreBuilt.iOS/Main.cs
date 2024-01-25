using Foundation;

using ObjCRuntime;

using System;
using System.Runtime.InteropServices;

using UIKit;

namespace Xamarin.PreBuilt.iOS {
	public class Applications {
		static void Main (string [] args)
			=> Xamarin.iOS.HotRestart.Application.Run (args, (string frameworkPath) => Dlfcn.dlopen (frameworkPath, 0), () => TerminateApplication ());

		static void TerminateApplication ()
		{
			Console.WriteLine ("Closing the running application to re-launch...");

			Console.WriteLine ("Trying Exit Option 1...");
			Exit (0);
			Console.WriteLine ("Option 1 failed. Trying Exit Option 2...");
			NSThread.Exit ();
			Console.WriteLine ("Option 2 failed. Trying Exit Option 3...");
			TerminateWithSuccess ();
			Console.WriteLine ("Option 3 failed. Trying Exit Option 4...");
			throw new Xamarin.iOS.HotRestart.KillApplicationException ();
		}

		[DllImport ("__Internal", EntryPoint = "exit")]
		static extern void Exit (int status);

		static void TerminateWithSuccess ()
		{
			var selector = new Selector ("terminateWithSuccess");

			UIApplication.SharedApplication.PerformSelector (selector, UIApplication.SharedApplication, 0);
		}
	}
}
