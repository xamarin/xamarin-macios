using System;
using System.IO;
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

			if (File.Exists ("output.mlpd")) {
				Test.Log.WriteLine ("SUCCESS: log output exists");
			} else {
				Test.Log.WriteLine ("FAIL: could not find 'output.mlpd'");
			}

			Test.Terminate ();
		}
	}
}
