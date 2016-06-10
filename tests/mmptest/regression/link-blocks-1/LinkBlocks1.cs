using System;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

// Test
// * application uses Blocks and BlockLiteral fields must be preserved
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class NativeBuilder {

		static void Check (string name, MemberInfo mi)
		{
			Test.Log.WriteLine ("{0}\t{1}", mi != null ? "[PASS]" : "[FAIL]", name);
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			int i = 0;
			string b = null;
			NSSet s = new NSSet ("a", "b", "c");
			s.Enumerate (delegate (NSObject obj, ref bool stop) {
				stop = i++ == 1;
				b = obj.ToString ();
			});
			// test behavior (we did not break anything)
			Test.Log.WriteLine ("[{0}]\tBehavior: Stop at item '{1}'", b == "b" ? "PASS" : "FAIL", b);
			// test that BlockLiteral is fully preserved
			int size = Marshal.SizeOf (typeof (BlockLiteral)); // e.g. unused 'reserved' must not be removed
			Test.Log.WriteLine ("[{0}]\tBlockLiteral is {1} bytes", size == 28 ? "PASS" : "FAIL", size);

			Test.Terminate ();
		}
	}
}