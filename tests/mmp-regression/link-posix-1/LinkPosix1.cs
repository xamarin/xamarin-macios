// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Foundation;
using AppKit;
using ObjCRuntime;

// Test
// * application does not reference assembly Mono.Posix.dll
// * application call method which pinvoke into libMonoPosixHelper.dylib
// * linker includes libMonoPosixHelper.dylib in the application bundle
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class Posix1 {

		static void Main (string [] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			bool gzip = false;
			try {
				GZipStream gz = new GZipStream (Stream.Null, CompressionMode.Compress);
				gz.WriteByte (0);
				gz.Close ();
				gzip = true;
			} catch {
			}
			Test.Log.WriteLine ("{0}\t{1}", gzip ? "[PASS]" : "[FAIL]", "GZipStream support");

			string path = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			bool bundled = File.Exists (Path.Combine (path, "libMonoPosixHelper.dylib"));
			Test.Log.WriteLine ("{0}\t{1}", bundled ? "[PASS]" : "[FAIL]", "MonoPosixHelper present in bundle");

			Test.Terminate ();
		}
	}
}
