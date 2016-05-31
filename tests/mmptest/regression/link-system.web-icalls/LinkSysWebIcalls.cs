using System;
using System.Web;
using System.IO;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * application references System.Web.dll
// * linker _must_ include all icalls
// ? that's what the mmp linker did, not clear (no history) why ? I could not find reflection usage of them ?!?
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class GdiPlus2 {

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);
#pragma warning disable 219
			// that will ensure System.Web reference is not removed
			var sw = typeof (HttpApplication);
#pragma warning restore 219
			int mcount = Type.GetType ("System.Web.Util.ICalls, System.Web").GetMethods (BindingFlags.Public | BindingFlags.Static).Length;
			Test.Log.WriteLine ("{0}\tSystem.Web.Util.ICalls {1}/3 icalls protected by linker", mcount == 3 ? "[PASS]" : "[FAIL]", mcount);

			Test.Terminate ();
		}
	}
}