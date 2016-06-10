using System;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application references System.Drawing.BitmapData
// * linker must include all the private fields of the class (used or not)
// * linker does NOT include libgdiplus.dylib in the application bundle
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class GdiPlus2 {

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			int fcount = typeof (BitmapData).GetFields (BindingFlags.NonPublic | BindingFlags.Instance).Length;
			Test.Log.WriteLine ("{0}\tBitmapData {1}/17 fields protected by linker", fcount == 17 ? "[PASS]" : "[FAIL]", fcount);

			// there's no [DllImport] on BitmapData so libgdiplus.dylib should not be copied into the bundle
			string path = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			bool bundled = File.Exists (Path.Combine (path, "libgdiplus.dylib"));
			Test.Log.WriteLine ("{0}\t{1}", !bundled ? "[PASS]" : "[FAIL]", "libgdiplus not present in bundle");
			
			Test.Terminate ();
		}
	}
}