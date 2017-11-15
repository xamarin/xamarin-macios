// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application is linked without any i18n support
// * application uses SystemIcons
// * linker includes resources for all .ico (System.Drawing)
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class KeepResources2 {

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			string [] resources = typeof (int).Assembly.GetManifestResourceNames ();
			Test.Log.WriteLine ("{0}\ti18n {1}/3 data files present: {2}", resources.Length <= 3 ? "[PASS]" : "[FAIL]", resources.Length, string.Join (", ", resources));

			var ss = typeof (System.ComponentModel.TypeConverter);
			resources = ss.Assembly.GetManifestResourceNames ();
			Test.Log.WriteLine ("{0}\tSystemSounds {1}/0 .wav files present", resources.Length == 0 ? "[PASS]" : "[FAIL]", resources.Length);

			var si = typeof (System.Drawing.SystemIcons);
			resources = si.Assembly.GetManifestResourceNames ();
			// Mono 2.10 ships with 5 .ico while Mono 3.0 provides 6 .ico resource files
			Test.Log.WriteLine ("{0}\tSystemIcons {1}/[5-6] .ico files present", resources.Length >= 5 ? "[PASS]" : "[FAIL]", resources.Length);

			Test.Terminate ();
		}
	}
}