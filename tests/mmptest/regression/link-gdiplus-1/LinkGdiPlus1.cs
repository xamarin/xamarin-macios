using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application references System.Drawing
// * application creates a Bitmap which pinvoke into gdiplus
// * linker includes libgdiplus.dylib in the application bundle
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class GdiPlus1 {

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			bool gdiplus = false;
			string msg = "Bitmap created";
			try {
				Bitmap b = new Bitmap (32, 32);
				gdiplus = (b != null);
			}
			catch (Exception e) {
				msg = e.ToString ();
			}

			Test.Log.WriteLine ("{0}\t{1}", gdiplus ? "[PASS]" : "[FAIL]", msg);
			
			string path = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			bool bundled = File.Exists (Path.Combine (path, "libgdiplus.dylib"));
			Test.Log.WriteLine ("{0}\t{1}", bundled ? "[PASS]" : "[FAIL]", "libgdiplus present in bundle");
			
			Test.Terminate ();
		}
	}
}