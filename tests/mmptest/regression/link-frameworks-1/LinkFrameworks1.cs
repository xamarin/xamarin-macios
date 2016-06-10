using System;
using System.Reflection;
using MonoMac.Foundation;
using MonoMac.AppKit;

// Test
// * application .exe only depends on Foundation and AppKit
//
// Requirement
// * Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class Framework {

		static FieldInfo[] fields;

		static void Check (string framework, string fieldName, bool exists)
		{
			bool found = false;
			foreach (var fi in fields) {
				if (fi.Name == fieldName) {
					found = true;
					break;
				}
			}
			Test.Log.WriteLine ("[{0}] {1} is {2}", found == exists ? "PASS" : "FAIL", framework, exists ? "present" : "absent");
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			fields = typeof(NSObject).GetFields (BindingFlags.NonPublic | BindingFlags.Static);

			// Foundation, AppKit will be present (would be hard to remove)
			Check ("Foundation", "fl", true);
			Check ("AppKit", "al", true);
			// Otherwise we should be able to eliminate the others
			Check ("AddressBook", "ab", false);
			Check ("CoreText", "ct", false);
			Check ("WebKit", "wl", false);
			Check ("Quartz", "zl", false);				// CoreAnimation
			Check ("QTKit", "ql", false);
			Check ("Security", "ll", false);
			Check ("QuartzComposer", "zc", false);
			Check ("CoreWlan", "cw", false);
			Check ("PdfKit", "pk", false);
			Check ("ImageKit", "ik", false);
			Check ("ScriptingBridge", "sb", false);
			Check ("AVFoundation", "av", false);
			// not everyone of them are in NSObjectMac.cs
			Check ("SceneKit", "sk", false);
			Check ("CoreBluetooth", "bl", false);
			Check ("StoreKit", "st", false);
			Check ("GameKit", "gk", false);

			Test.Terminate ();
		}
	}
}