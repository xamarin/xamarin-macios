using System;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application .exe is decorated with [LinkerSafe]
// * the decorated (main .exe) assembly is linked (even with Link SDK)
// * other assemblies are linked
//
// Requirement
// * Link SDK (not All) must be enabled

[assembly: LinkerSafe]

namespace Xamarin.Mac.Linker.Test {
	
	class SafeToLinkAssembly {

		static int UnusedProperty {
			get; set;
		}

		public void UnusedMethod ()
		{
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			int pcount = typeof (SafeToLinkAssembly).GetProperties (BindingFlags.NonPublic | BindingFlags.Static).Length;
			Test.Log.WriteLine ("{0}\tUnused property ({1}/0) was preserved by linker", pcount == 0 ? "[PASS]" : "[FAIL]", pcount);

			bool m = typeof (SafeToLinkAssembly).GetMethod ("UnusedMethod", BindingFlags.Public | BindingFlags.Instance) == null;
			Test.Log.WriteLine ("{0}\tUnused method was preserved by linker", m ? "[PASS]" : "[FAIL]");

			Test.Terminate ();
		}
	}
}