using System;
using System.Reflection;

using AppKit;

using Foundation;

using ObjCRuntime;

// Test
// * application .exe is decorated with [Preserve]
// * the decorated (main .exe) assembly is not linked
// * other assemblies are linked
//
// Requirement
// * Link All must be enabled

[assembly: Preserve]

namespace Xamarin.Mac.Linker.Test {

	class PreserveAssembly {

		static int UnusedProperty {
			get; set;
		}

		public void UnusedMethod ()
		{
		}

		static void Main (string [] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			int pcount = typeof (PreserveAssembly).GetProperties (BindingFlags.NonPublic | BindingFlags.Static).Length;
			Test.Log.WriteLine ("{0}\tUnused property ({1}/1) was preserved by linker", pcount == 1 ? "[PASS]" : "[FAIL]", pcount);

			bool m = typeof (PreserveAssembly).GetMethod ("UnusedMethod", BindingFlags.Public | BindingFlags.Instance) is not null;
			Test.Log.WriteLine ("{0}\tUnused method was preserved by linker", m ? "[PASS]" : "[FAIL]");

			Test.Terminate ();
		}
	}
}
