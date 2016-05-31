using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMac.Foundation;
using MonoMac.AppKit;

// Test
// * application references [CallerFilePath] which is new in .NET 4.5
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class CallFilePath {

		// https://bugzilla.xamarin.com/show_bug.cgi?id=7114
		public static void Bug7114 ([CallerFilePath] string filePath = null)
		{
			Test.Log.WriteLine ("{0}\tCallerFilePath = {1}", filePath != null ? "[PASS]" : "[FAIL]", filePath);
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			Bug7114 ();

			Test.Terminate ();
		}
	}
}