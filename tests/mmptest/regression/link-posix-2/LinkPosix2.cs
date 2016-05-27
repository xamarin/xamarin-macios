using System;
using System.IO;
using System.Reflection;
using Mono.Posix;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application references Mono.Posix
// * application call method which pinvoke into libc
// * linker does not include libMonoPosixHelper.dylib in the application bundle
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class Posix2 {

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

#pragma warning disable 618
			var host = Syscall.GetHostName ();
#pragma warning restore 618
			Test.Log.WriteLine ("{0}\t{1} {2}", !String.IsNullOrEmpty (host) ? "[PASS]" : "[FAIL]", "Syscall.GetHostName returned", host);

			string path = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			string dylib = Path.Combine (path, "libMonoPosixHelper.dylib");
			bool bundled = File.Exists (dylib);
			Test.Log.WriteLine ("{0}\t{1} : {2}", !bundled ? "[PASS]" : "[FAIL]", "MonoPosixHelper not present in bundle", dylib);

			Test.Terminate ();
		}
	}
}