using System;
using System.Collections.Generic;
using System.Reflection;

using AppKit;
using Foundation;

// Test
// * application .exe only depends on Foundation and AppKit
//
// Requirement
// * Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	class Framework {
		static void TestFrameworks ()
		{
			var fields = typeof (NSObject).GetFields (BindingFlags.NonPublic | BindingFlags.Static);

			// This is not meant to be a complete mapping, just to show better debug messages.
			var mapping = new Dictionary<string, string> {
				{ "fl", "Foundation" },
				{ "al", "AppKit" },
				{ "ab", "AddressBook" },
				{ "ct", "CoreText" },
				{ "wl", "WebKit" },
				{ "zl", "QuartzCore" },
				{ "ql", "QTKit" },
				{ "ll", "Security" },
				{ "zc", "QuartzComposer" },
				{ "cw", "CoreWlan" },
				{ "cl", "CoreLocation" },
				{ "sk", "SceneKit" },
				{ "mk", "MapKit" },
				{ "bc", "BusinessChat" },
				{ "un", "UserNotifications" },
				{ "ck", "CloudKit" },
			};

			foreach (var field in fields) {
				if (field.FieldType != typeof (IntPtr))
					continue;

				if (!mapping.TryGetValue (field.Name, out var framework))
					framework = "unknown";

				switch (field.Name) {
				case "selConformsToProtocolHandle":
				case "selDescriptionXHandle":
				case "selHashXHandle":
				case "selIsEqual_XHandle":
				case "class_ptr":
					// Unrelated fields
					continue;
				case "fl": // Foundation
				case "al": // AppKit
					Test.Log.WriteLine ($"[PASS] As expected found field NSObject.{field.Name} for framework '{framework}'.");
					continue;
				case "zl": // QuartzCore (CoreAnimation)
				case "cl": // CoreLocation
				case "ll": // Security
				case "sk": // SceneKit
				case "mk": // MapKit
				case "bc": // BusinessChat
				case "un": // UserNotifications
						   // This needs investigation, it should be possible to link at least some of these frameworks away.
						   // https://github.com/xamarin/xamarin-macios/issues/6542
					continue;
				default:
					Test.Log.WriteLine ($"[FAIL] Unexpectedly found field NSObject.{field.Name} for framework '{framework}'.");
					break;
				}
			}
		}

		static void Main (string [] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			TestFrameworks ();

			Test.Terminate ();
		}
	}
}
