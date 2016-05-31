// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application is linked with i18n CJK support
// * application uses SystemSounds
// * linker includes resources for all .wav (System) and i18n + CJK
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class KeepResources1 {

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			int cjk = 0;
			string [] resources = typeof (int).Assembly.GetManifestResourceNames ();
			foreach (string name in resources) {
				if (name.Contains ("cjk"))
					cjk++;
			}
			Test.Log.WriteLine ("{0}\tCJK {1}/5 data files present", cjk == 5 ? "[PASS]" : "[FAIL]", cjk);

			var ss = typeof (System.Media.SystemSounds);
			resources = ss.Assembly.GetManifestResourceNames ();
			Test.Log.WriteLine ("{0}\tSystemSounds {1}/5 .wav files present", resources.Length == 5 ? "[PASS]" : "[FAIL]", resources.Length);

			Test.Terminate ();
		}
	}
}