// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

using AppKit;

using Foundation;

using ObjCRuntime;

// Test
// * application is linked without any i18n support
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class KeepResources2 {

		static void Main (string [] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			string [] resources = typeof (int).Assembly.GetManifestResourceNames ();
			Test.Log.WriteLine ("{0}\ti18n {1}/3 data files present: {2}", resources.Length <= 3 ? "[PASS]" : "[FAIL]", resources.Length, string.Join (", ", resources));

			var ss = typeof (System.ComponentModel.TypeConverter);
			resources = ss.Assembly.GetManifestResourceNames ();
			Test.Log.WriteLine ("{0}\tSystemSounds {1}/0 .wav files present", resources.Length == 0 ? "[PASS]" : "[FAIL]", resources.Length);

			Test.Terminate ();
		}
	}
}
