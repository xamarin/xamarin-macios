using System;
using System.IO;

using AppKit;
using ObjCRuntime;
using Foundation;


namespace Xamarin.Mac.Linker.Test {

	class CustomBundleName {
		static void Main (string [] args)
		{
			NSApplication.Init ();

			if (Directory.Exists (Path.Combine (NSBundle.MainBundle.BundlePath, "Contents", "CustomBundleName")) &&
				!Directory.Exists (Path.Combine (NSBundle.MainBundle.BundlePath, "Contents", "MonoBundle")))
				Test.Log.WriteLine ("[SUCCESS]");
			else
				Test.Log.WriteLine ("[FAIL]");

			Test.Terminate ();
		}
	}
}
