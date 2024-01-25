// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

using AppKit;

using Foundation;

using ObjCRuntime;

// Test
// * application is linked with i18n CJK support
// * linker includes resources for i18n + CJK
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class KeepResources1 {

		static void Main (string [] args)
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

			Test.Terminate ();
		}
	}
}
