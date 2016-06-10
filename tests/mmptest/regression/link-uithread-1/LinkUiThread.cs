// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

// Test
// * By default a DEBUG build will have a it thread checks enabled
//	* link-uithread-1 defines DEBUG (and has debug symbols)
//	* link-uithread-2 does not defined DEBUG (and no debug symbol)
//	* so same source, two test cases
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class Tester : NSObject {
		public static ManualResetEvent mre;
		public static bool success;

		[CompilerGenerated]
		[Export ("foo")]
		public static void Test ()
		{
			try {
				NSApplication.EnsureUIThread ();
#if !DEBUG
				success = true;
#endif
			}
			catch (AppKitThreadAccessException) {
#if DEBUG
				success = true;
#endif
			}
			catch {
			}
			finally {
				mre.Set ();
			}
		}
	}

	class UiThread {
		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			// works on main/ui thread
			NSApplication.EnsureUIThread ();

			Tester.mre = new ManualResetEvent (false);
			ThreadPool.QueueUserWorkItem ((v) => Tester.Test ());
			Tester.mre.WaitOne ();
			Test.Log.WriteLine ("{0}\tEnsureUIThread {1} on non-ui thread: {2}", Tester.success ? "[PASS]" : "[FAIL]",
#if DEBUG
				"enabled",
#else
				"disabled",
#endif
				Tester.success);
			Test.Terminate ();
		}
	}
}
