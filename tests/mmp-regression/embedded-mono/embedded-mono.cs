using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using AppKit;
using ObjCRuntime;

namespace Xamarin.Mac.Linker.Test {

	class SystemMono {
		static void Main (string [] args)
		{
			NSApplication.Init ();

			TestDlopenSystem ();
			TestDlopenEmbedded ();

			Test.Terminate ();
		}

		[DllImport ("libSystem.dylib")]
		public static extern IntPtr dlopen (string path, int mode);

		static void TestDlopenSystem ()
		{
			try {
				IntPtr h = dlopen ("libc.dylib", 0);
				if (h == IntPtr.Zero) {
					Test.Log.WriteLine ("[FAIL] TestDlopenSystem: Could not dlopen libc.dylib");
				} else {
					Test.Log.WriteLine ("[SUCCESS] TestDlopenSystem: dlopen libc.dylib: 0x{0}", h.ToString ("x"));
					Dlfcn.dlclose (h);
				}
			} catch (Exception ex) {
				Test.Log.WriteLine ("[FAIL] TestDlopenSystem: {0}", ex);
			}
		}
		[DllImport ("libTest.dylib")]
		static extern int the_answer ();

		static void TestDlopenEmbedded ()
		{
			try {
				var a = the_answer ();
				Test.Log.WriteLine ("[SUCCESS] TestDlopenEmbedded: {0}", a);
			} catch (Exception ex) {
				Test.Log.WriteLine ("[FAIL] TestDlopenEmbedded: {0}", ex);
			}
		}
	}
}
